using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace AIMentor.Database.Models.Session;

public class SessionModelConfiguration : IEntityTypeConfiguration<SessionModel>
{
    public void Configure(EntityTypeBuilder<SessionModel> builder)
    {
            builder
                .HasKey(x => x.Id);

            builder.Property(x=> x.Id)
                .ValueGeneratedOnAdd();

            builder
                .Property(x => x.Name)
                .IsRequired()
                .HasMaxLength(255);

            builder
                .Property(x => x.CreatedAt)
                .IsRequired()
                .HasConversion(ValueConverters.DateTimeOffsetToUtcDateTime)
                .HasColumnType("timestamp")
                .HasDefaultValueSql("STRFTIME('%Y-%m-%d %H:%M:%f','now')");

            builder
                .Property(x => x.UpdatedAt)
                .IsRequired()
                .HasConversion(ValueConverters.DateTimeOffsetToUtcDateTime)
                .HasColumnType("timestamp")
                .HasDefaultValueSql("STRFTIME('%Y-%m-%d %H:%M:%f','now')");

            builder.HasMany(x => x.Messages)
                .WithOne(x => x.Session)
                .HasForeignKey(x => x.SessionId)
                .HasConstraintName("fk_messages_sessions")
                .OnDelete(DeleteBehavior.NoAction);
    }
}
