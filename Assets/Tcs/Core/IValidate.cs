using System;

namespace Tcs.Core
{
    public interface IValidate<T>
    {
        string ErrorMessage { get; }
        bool IsValid { get; }
        bool Validate();
        void AddValidator(Func<T, bool> validator, string errorMessage);
    }
}
