using System;

namespace GenFacades
{
    internal sealed class FacadeGenerationException : Exception
    {
        public FacadeGenerationException(string message) : base(message)
        {
        }
    }
}
