using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SavedMessages.Domain.Messages;

namespace SavedMessages.Persistence.Configuration
{
    internal class MessageFileConfiguration : IEntityTypeConfiguration<MessageFile>
    {
        public void Configure(EntityTypeBuilder<MessageFile> builder)
        {
            builder.HasKey(x => x.Id);
        }
    }
}
