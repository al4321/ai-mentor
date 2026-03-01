using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace AIMentor.Database.Models.Message;

public class MessageModelConfiguration : IEntityTypeConfiguration<MessageModel>
{
    public void Configure(EntityTypeBuilder<MessageModel> builder)
    {
        builder
            .HasKey(x => x.Id);
        builder.Property(x=> x.Id)
            .ValueGeneratedOnAdd();

        builder
            .Property(x => x.Content)
            .IsRequired()
            .HasMaxLength(-1);

        builder
            .Property(x => x.Role)
            .IsRequired()
            .HasMaxLength(50);

        builder
            .Property(x => x.CreatedAt)
            .IsRequired()
            .HasConversion(ValueConverters.DateTimeOffsetToUtcDateTime)
            .HasColumnType("timestamp")
            .HasDefaultValueSql("STRFTIME('%Y-%m-%d %H:%M:%f','now')");
    }
}
