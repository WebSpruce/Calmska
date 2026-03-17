using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.EntityFrameworkCore.ValueGeneration;

namespace Calmska.Api.Helper;

public class IntValueGenerator : ValueGenerator<int>
{
    private int _current = 0;

    public override int Next(EntityEntry entry)
        => Interlocked.Increment(ref _current);

    public override bool GeneratesTemporaryValues => false;
}