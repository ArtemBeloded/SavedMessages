﻿using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SavedMessages.Domain.Messages;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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