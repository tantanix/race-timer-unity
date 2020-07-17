using System;

namespace Tcs.Core.Entity
{
    public class EntityNotFoundException<T> : Exception where T : Entity
    {
    }
}
