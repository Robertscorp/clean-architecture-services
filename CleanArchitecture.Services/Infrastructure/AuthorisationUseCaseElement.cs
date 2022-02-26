﻿using CleanArchitecture.Services.Pipeline;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace CleanArchitecture.Services.Infrastructure
{

    /// <summary>
    /// Handles invocation of the Authorisation Enforcer service and presentation of authorisation failures.
    /// </summary>
    /// <typeparam name="TAuthorisationResult">The type of Authorisation Result.</typeparam>
    public class AuthorisationUseCaseElement<TAuthorisationResult> : IUseCaseElement where TAuthorisationResult : IAuthorisationResult
    {

        #region - - - - - - Fields - - - - - -

        private readonly UseCaseServiceResolver m_ServiceResolver;

        #endregion Fields

        #region - - - - - - Constructors - - - - - -

        /// <summary>
        /// Initialises a new instance of the <see cref="AuthorisationUseCaseElement{TAuthorisationResult}"/> class.
        /// </summary>
        /// <param name="serviceResolver">The delegate used to get services.</param>
        public AuthorisationUseCaseElement(UseCaseServiceResolver serviceResolver)
            => this.m_ServiceResolver = serviceResolver ?? throw new ArgumentNullException(nameof(serviceResolver));

        #endregion Constructors

        #region - - - - - - Methods - - - - - -

        private Task<TAuthorisationResult> GetAuthorisationResultAsync<TUseCaseInputPort>(TUseCaseInputPort inputPort, CancellationToken cancellationToken)
            => inputPort is IUseCaseInputPort<IAuthorisationOutputPort<TAuthorisationResult>>
                ? DelegateFactory
                    .GetFunction<(UseCaseServiceResolver, TUseCaseInputPort, CancellationToken), Task<TAuthorisationResult>>(
                        typeof(EnforcerCheckFactory<>).MakeGenericType(typeof(TAuthorisationResult), typeof(TUseCaseInputPort)))?
                    .Invoke((this.m_ServiceResolver, inputPort, cancellationToken))
                : null;

        async Task IUseCaseElement.HandleAsync<TUseCaseInputPort, TUseCaseOutputPort>(
            TUseCaseInputPort inputPort,
            TUseCaseOutputPort outputPort,
            UseCaseElementHandleAsync nextUseCaseElementHandle,
            CancellationToken cancellationToken)
        {
            if (outputPort is IAuthorisationOutputPort<TAuthorisationResult> _OutputPort)
            {
                var _AuthorisationResultAsync = this.GetAuthorisationResultAsync(inputPort, cancellationToken);
                if (_AuthorisationResultAsync != null && !(await _AuthorisationResultAsync).IsAuthorised)
                {
                    await _OutputPort.PresentUnauthorisedAsync(await _AuthorisationResultAsync, cancellationToken).ConfigureAwait(false);
                    return;
                }
            }

            await nextUseCaseElementHandle().ConfigureAwait(false);
        }

        #endregion Methods

        #region - - - - - - Nested Classes - - - - - -

        private class EnforcerCheckFactory<TUseCaseInputPort>
            : IDelegateFactory<(UseCaseServiceResolver, TUseCaseInputPort, CancellationToken), Task<TAuthorisationResult>>
            where TUseCaseInputPort : IUseCaseInputPort<IAuthorisationOutputPort<TAuthorisationResult>>
        {

            #region - - - - - - Methods - - - - - -
            public Func<(UseCaseServiceResolver, TUseCaseInputPort, CancellationToken), Task<TAuthorisationResult>> GetFunction()
                => sripc
                    => sripc.Item1
                        .GetService<IUseCaseAuthorisationEnforcer<TUseCaseInputPort, TAuthorisationResult>>()?
                        .CheckAuthorisationAsync(sripc.Item2, sripc.Item3);

            #endregion Methods

        }

        #endregion Nested Classes

    }

}
