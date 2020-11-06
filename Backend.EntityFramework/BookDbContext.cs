using System;
using System.Linq;
using System.Reflection;
using Backend.EntityFramework.Configuration;
using Microsoft.EntityFrameworkCore;

namespace Backend.EntityFramework
{
    public class BookDbContext : DbContext
    {
        private readonly DbContextConfiguration _config;

        public BookDbContext(DbContextConfiguration config)
        {
            _config = config;
        }

        public BookDbContext(DbContextOptions<BookDbContext> options, DbContextConfiguration config) : base(options)
        {
            _config = config;
        }
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseSqlServer(_config.ConnectionString);
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            var typesToRegister = Assembly.GetExecutingAssembly()
                .GetTypes()
                .Where(t => t.GetInterfaces().Any(gi => gi.IsGenericType && gi.GetGenericTypeDefinition() == typeof(IEntityTypeConfiguration<>)))
                .ToList();


            foreach (var type in typesToRegister)
            {
                dynamic configurationInstance = Activator.CreateInstance(type);
                modelBuilder.ApplyConfiguration(configurationInstance);
            }
        }
    }
}