﻿namespace NPatterns.Messaging.IoC
{
    public interface IHandler<in T>
    {
        void Handle(T message);
    }
}