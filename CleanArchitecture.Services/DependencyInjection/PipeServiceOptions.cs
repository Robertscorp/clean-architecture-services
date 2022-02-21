﻿namespace CleanArchitecture.Services.DependencyInjection
{

    internal class PipeServiceOptions
    {

        #region - - - - - - Constructors - - - - - -

        public PipeServiceOptions(Type serviceType)
            => this.PipeService = serviceType;

        #endregion Constructors

        #region - - - - - - Properties - - - - - -

        public Type PipeService { get; }

        public Func<Type, Type, Type, Type>? UseCaseServiceResolver { get; private set; }

        #endregion Properties

        #region - - - - - - Methods - - - - - -

        public void WithUseCaseServiceResolver(Func<Type, Type, Type, Type> useCaseServiceResolver)
            => this.UseCaseServiceResolver = useCaseServiceResolver;

        #endregion Methods

    }

}