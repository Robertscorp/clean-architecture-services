﻿using System.Threading;
using System.Threading.Tasks;

namespace CleanArchitecture.Services
{

    /// <summary>
    /// An Output Port for when authorisation is required by a Use Case.
    /// </summary>
    /// <typeparam name="TAuthorisationFailure">The type of authorisation failure for the Use Case Pipeline.</typeparam>
    public interface IAuthorisationOutputPort<TAuthorisationFailure>
    {

        #region - - - - - - Methods - - - - - -

        /// <summary>
        /// Presents an authorisation failure.
        /// </summary>
        /// <param name="authorisationFailure">The authorisation failure that occurred.</param>
        /// <param name="cancellationToken">The <see cref="CancellationToken"/> used to propagate notifications that the operation should be canceled.</param>
        Task PresentUnauthorisedAsync(TAuthorisationFailure authorisationFailure, CancellationToken cancellationToken);

        #endregion Methods

    }

}
