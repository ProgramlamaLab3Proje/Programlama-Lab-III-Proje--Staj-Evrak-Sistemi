using Microsoft.AspNetCore.Components.RenderTree;
using Microsoft.EntityFrameworkCore;
using StajEvrakSistemi.Models;

namespace StajEvrakSistemi.Data
{
    public class Context : DbContext
    {
        public DbSet<Student> Students { get; set; }
        public DbSet<Admin> Admins { get; set; }
        public DbSet<StudentRecord> StudentRecords { get; set; }
        public DbSet<CSVRecord> CSVRecords { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseSqlite("Data Source = StajEvrakSistemVerileri.db");
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<StudentRecord>()
                .HasOne(r => r.Student) // kayıtlar sadece bir öğrenciye ait olabilir
                .WithMany(s => s.Records)
                .HasForeignKey(r => r.StudentId)
                .OnDelete(DeleteBehavior.Cascade); // öğrenci silinirse kayıtları da silinir

            modelBuilder.Entity<StudentRecord>()
                .Property(r => r.Status) 
                .HasConversion<string>(); // dosya durumu string olarak kaydedilsin

            modelBuilder.Entity<CSVRecord>()
                .HasIndex(d => d.StudentNumber)
                .IsUnique(); // Admin aynı csvi yanlışlıkla iki kez yüklerse veritabanı reddeder, her öğrencinin numarası özel olur.
        }
    }
}
