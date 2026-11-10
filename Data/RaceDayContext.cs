using Microsoft.EntityFrameworkCore;
using RaceDay.Api.Models;
using RaceRoute = RaceDay.Api.Models.Route;
using RaceResult = RaceDay.Api.Models.Results;

namespace RaceDay.Api.Data;

public class RaceDayContext : DbContext
{
    public RaceDayContext(DbContextOptions<RaceDayContext> options) : base(options) { }

    public DbSet<Organiser> Organisers => Set<Organiser>();
    public DbSet<Participant> Participants => Set<Participant>();
    public DbSet<Event> Events => Set<Event>();
    public DbSet<EventCategory> EventCategories => Set<EventCategory>();
    public DbSet<Enrolment> Enrolments => Set<Enrolment>();
    public DbSet<RaceRoute> Routes => Set<RaceRoute>();
    public DbSet<Weather> WeatherRecords => Set<Weather>();
    public DbSet<RaceResult> Results => Set<RaceResult>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Organiser>().HasIndex(x => x.Email).IsUnique();
        modelBuilder.Entity<Participant>().HasIndex(x => x.Email).IsUnique();
        modelBuilder.Entity<Participant>().HasIndex(x => x.IDNumber).IsUnique();
        modelBuilder.Entity<Event>().HasOne(x => x.Organiser).WithMany(x => x.Events).HasForeignKey(x => x.OrganiserID).OnDelete(DeleteBehavior.Restrict);
        modelBuilder.Entity<EventCategory>().HasOne(x => x.Event).WithMany(x => x.Categories).HasForeignKey(x => x.EventID);
        modelBuilder.Entity<EventCategory>().Property(x => x.EntryFee).HasPrecision(10, 2);
        modelBuilder.Entity<EventCategory>().Property(x => x.Distance).HasPrecision(7, 2);
        modelBuilder.Entity<Enrolment>().HasOne(x => x.Participant).WithMany(x => x.Enrolments).HasForeignKey(x => x.ParticipantID).OnDelete(DeleteBehavior.Restrict);
        modelBuilder.Entity<Enrolment>().HasOne(x => x.Event).WithMany(x => x.Enrolments).HasForeignKey(x => x.EventID).OnDelete(DeleteBehavior.Restrict);
        modelBuilder.Entity<Enrolment>().HasOne(x => x.EventCategory).WithMany(x => x.Enrolments).HasForeignKey(x => x.EventCategoryID).OnDelete(DeleteBehavior.Restrict);
        modelBuilder.Entity<Enrolment>().HasIndex(x => new { x.ParticipantID, x.EventID }).IsUnique();
        modelBuilder.Entity<RaceResult>().HasOne(x => x.Enrolment).WithOne(x => x.Results).HasForeignKey<RaceResult>(x => x.EnrolmentID);
        modelBuilder.Entity<Weather>().HasOne(x => x.Event).WithMany(x => x.WeatherRecords).HasForeignKey(x => x.EventID);
        modelBuilder.Entity<RaceRoute>().HasOne(x => x.Event).WithMany(x => x.Routes).HasForeignKey(x => x.EventID);
    }
}
