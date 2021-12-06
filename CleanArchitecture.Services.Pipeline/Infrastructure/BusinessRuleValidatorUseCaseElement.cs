﻿using CleanArchitecture.Services.Pipeline.Validation;

namespace CleanArchitecture.Services.Pipeline.Infrastructure
{

    public class BusinessRuleValidatorUseCaseElement<TValidationResult> : IUseCaseElement where TValidationResult : IValidationResult
    {

        #region - - - - - - Fields - - - - - -

        private readonly IServiceProvider m_ServiceProvider;

        #endregion Fields

        #region - - - - - - Constructors - - - - - -

        public BusinessRuleValidatorUseCaseElement(IServiceProvider serviceProvider)
            => this.m_ServiceProvider = serviceProvider;

        #endregion Constructors

        #region - - - - - - IUseCaseElement Implementation - - - - - -

        public async Task<bool> TryOutputResultAsync<TUseCaseInputPort, TUseCaseOutputPort>(
            TUseCaseInputPort inputPort,
            TUseCaseOutputPort outputPort,
            CancellationToken cancellationToken)
        {
            if (outputPort is not IBusinessRuleValidationOutputPort<TValidationResult> _ValidationOutputPort)
                return false;

            var _Validator = (IUseCaseBusinessRuleValidator<TUseCaseInputPort, TValidationResult>?)this.m_ServiceProvider.GetService(typeof(IUseCaseBusinessRuleValidator<TUseCaseInputPort, TValidationResult>));
            if (_Validator == null)
                return false;

            var _ValidationResult = await _Validator.ValidateAsync(inputPort, cancellationToken).ConfigureAwait(false);
            if (_ValidationResult.IsValid)
                return false;

            await _ValidationOutputPort.PresentBusinessRuleValidationFailureAsync(_ValidationResult, cancellationToken).ConfigureAwait(false);

            return true;
        }

        #endregion IUseCaseElement Implementation

    }

}
