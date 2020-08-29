namespace Tcs.Core
{
    public interface IValidator<T>
    {
        bool Validate(T value);
    }
}
