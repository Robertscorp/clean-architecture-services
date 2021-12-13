﻿using CleanArchitecture.Services.Pipeline.Authorisation;

namespace CleanArchitecture.Sample.Pipeline
{

    public class AuthorisationResult : IAuthorisationResult
    {

        #region - - - - - - IAuthorisationResult Implementation - - - - - -

        public bool IsAuthorised { get; set; }

        #endregion IAuthorisationResult Implementation

    }

}