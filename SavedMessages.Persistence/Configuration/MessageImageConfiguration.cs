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
    internal class MessageImageConfiguration : IEntityTypeConfiguration<MessageImage>
    {
        public void Configure(EntityTypeBuilder<MessageImage> builder)
        {
            builder.HasKey(x => x.Id);
        }
    }
}
