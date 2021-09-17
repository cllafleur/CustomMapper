﻿using System;

namespace MapperDslLib
{
    public interface IFunctionHandlerProvider
    {
        T Get<T>(string identifier) where T : class;

        void Register<T, TImplementation>(string identifier)
            where TImplementation : class, new();

        void Register<T>(string identifier, Type implementationType);
    }
}