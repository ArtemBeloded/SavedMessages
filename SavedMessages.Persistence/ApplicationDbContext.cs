﻿using Microsoft.EntityFrameworkCore;
using SavedMessages.Application.Data;
using SavedMessages.Domain.Messages;
using SavedMessages.Domain.Users;

namespace SavedMessages.Persistence
{
    public class ApplicationDbContext : DbContext, IApplicationDbContext, IUnitOfWork
    {
        public ApplicationDbContext(DbContextOptions options)
            : base(options) { }

        public DbSet<Message> Messages { get; set; }

        public DbSet<User> Users { get; set; }

        public DbSet<MessageFile> MessageFiles { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder) 
        {
            modelBuilder.ApplyConfigurationsFromAssembly(typeof(ApplicationDbContext).Assembly);
        }
    }
}
