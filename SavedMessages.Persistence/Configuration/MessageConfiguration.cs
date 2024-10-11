using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SavedMessages.Domain.Messages;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SavedMessages.Persistence.Configuration
{
    internal class MessageConfiguration : IEntityTypeConfiguration<Message>
    {
        public void Configure(EntityTypeBuilder<Message> builder)
        {
            builder
                .HasKey(x => x.Id);

            builder
                .HasOne(f => f.File)
                .WithOne()
                .HasForeignKey<MessageFile>(x => x.MessageId)
                .OnDelete(DeleteBehavior.Cascade);

            builder
                .Property(p => p.IsEdited)
                .HasDefaultValue(false);
        }
    }
}
