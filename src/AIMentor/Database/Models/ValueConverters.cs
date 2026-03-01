using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace AIMentor.Database.Models;

public static class ValueConverters
{
    public static ValueConverter<DateTimeOffset, DateTime> DateTimeOffsetToUtcDateTime { get; } =
        new(
            dto => dto.UtcDateTime,
            dt => new DateTimeOffset(DateTime.SpecifyKind(dt, DateTimeKind.Utc)));
}

