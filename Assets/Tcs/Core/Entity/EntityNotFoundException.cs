using System;

namespace Tcs.Core.Entity
{
    public class EntityNotFoundException<T> : Exception where T : Entity
    {
        public string Id { get; private set; }

        public EntityNotFoundException() { }

        public EntityNotFoundException(string id)
        {
            Id = id;
        }

    }
}
