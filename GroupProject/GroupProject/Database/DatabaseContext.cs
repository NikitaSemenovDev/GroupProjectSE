using GroupProject.Database.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace GroupProject.Database
{
    public class DatabaseContext : DbContext
    {
        public DatabaseContext(DbContextOptions<DatabaseContext> options) : base(options)
        { 
        }

        public DbSet<Person> People { get; set; }
        public DbSet<Patient> Patients { get; set; }
        public DbSet<Doctor> Doctors { get; set; }
        public DbSet<Account> Accounts { get; set; }
        public DbSet<DoctorPatient> DoctorPatients { get; set; }
        public DbSet<Role> Roles { get; set; }
        public DbSet<ImageProcessingResult> ImageProcessingResults { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Person>().ToTable("Person");
            modelBuilder.Entity<Account>().ToTable("Account");
            modelBuilder.Entity<DoctorPatient>().ToTable("DoctorPatient");
            modelBuilder.Entity<Role>().ToTable("Role");
            modelBuilder.Entity<ImageProcessingResult>().ToTable("ImageProcessingResult");

            modelBuilder.Entity<Account>()
                .HasMany(a => a.LinkedAccounts)
                .WithOne(dp => dp.Account)
                .HasForeignKey(dp => dp.AccountId);
        }
    }
}
