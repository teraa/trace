using System;

namespace TwitchLogger.Data.Models
{
    public abstract class EntityBase<T>
        where T : notnull, IEquatable<T>
    {
        public T Id { get; set; } = default!;

        public override int GetHashCode()
            => Id.GetHashCode();

        public override bool Equals(object? obj)
            => obj is not null
                && GetType() == obj.GetType()
                && ((EntityBase<T>)obj).Id.Equals(Id);

    }
}
