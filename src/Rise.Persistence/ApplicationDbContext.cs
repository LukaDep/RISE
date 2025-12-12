using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Rise.Domain.Absences;
using Rise.Domain.Campus;
using Rise.Domain.Contact;
using Rise.Domain.Deadlines;
using Rise.Domain.Grades;
using Rise.Domain.HomeWidgets;
using Rise.Domain.Menu;
using Rise.Domain.News;
using Rise.Domain.Notifications;
using Rise.Domain.Restos;
using Rise.Domain.StudentCards;
using Rise.Domain.Events;

namespace Rise.Persistence;

/// <summary>
/// Entrance to the database, inherits from IdentityDbContext and is basically a Unit Of Work and Repository pattern combined.
/// A <see cref="DbSet"/> is a repository for a specific type of entity.
/// The <see cref="ApplicationDbContext"/> is the Unit Of Work pattern
/// Will look very similar when switching database providers.
/// See https://hogent-web.github.io/csharp/chapters/09/slides/index.html#1
/// See https://enterprisecraftsmanship.com/posts/should-you-abstract-database/
/// </summary>
/// <param name="opts"></param>
public class ApplicationDbContext(DbContextOptions<ApplicationDbContext> opts) : IdentityDbContext<IdentityUser>(opts)
{
    /// <summary>Gets the DbSet for news articles.</summary>
    public DbSet<NewsArticle> NewsArticles => Set<NewsArticle>();
    
    /// <summary>Gets the DbSet for student absences.</summary>
    public DbSet<Absence> Absences => Set<Absence>();
    
    /// <summary>Gets the DbSet for campus entities.</summary>
    public DbSet<Campus> Campuses => Set<Campus>();
    
    /// <summary>Gets the DbSet for campus buildings.</summary>
    public DbSet<Building> Buildings => Set<Building>();
    
    /// <summary>Gets the DbSet for contact information.</summary>
    public DbSet<Contact> Contacts => Set<Contact>();
    
    /// <summary>Gets the DbSet for student grades.</summary>
    public DbSet<Grade> Grades => Set<Grade>();
    
    /// <summary>Gets the DbSet for restaurant menus.</summary>
    public DbSet<Menu> Menus => Set<Menu>();
    
    /// <summary>Gets the DbSet for individual menu items.</summary>
    public DbSet<MenuItem> MenuItems => Set<MenuItem>();
    
    /// <summary>Gets the DbSet for restaurants.</summary>
    public DbSet<Resto> Restos => Set<Resto>();
    
    /// <summary>Gets the DbSet for user notification preferences.</summary>
    public DbSet<NotificationPreferences> NotificationPreferences => Set<NotificationPreferences>();
    
    /// <summary>Gets the DbSet for sent notification history.</summary>
    public DbSet<SentNotification> SentNotifications => Set<SentNotification>();
    
    /// <summary>Gets the DbSet for student cards.</summary>
    public DbSet<StudentCard> StudentCards => Set<StudentCard>();
    
    /// <summary>Gets the DbSet for calendar events.</summary>
    public DbSet<Event> Events => Set<Event>();
    
    /// <summary>Gets the DbSet for push notification subscriptions.</summary>
    public DbSet<PushSubscriptions> PushSubscriptions => Set<PushSubscriptions>();
    
    /// <summary>Gets the DbSet for assignment deadlines.</summary>
    public DbSet<Deadline> Deadlines => Set<Deadline>();
    
    /// <summary>Gets the DbSet for available widget definitions.</summary>
    public DbSet<Widget> Widgets => Set<Widget>();
    
    /// <summary>Gets the DbSet for user-specific widget configurations.</summary>
    public DbSet<UserWidget> UserWidgets => Set<UserWidget>();

    /// <summary>
    /// Configures global conventions for the model.
    /// Sets default max length for strings (255 chars for efficient MySQL/MariaDB indexing)
    /// and precision for decimals (18,2).
    /// </summary>
    /// <param name="configurationBuilder">The builder used to configure conventions.</param>
    protected override void ConfigureConventions(ModelConfigurationBuilder configurationBuilder)
    {
        // All columns in the database have a maxlength of 255.
        // in VARCHAR 255 is the maximum length that can be indexed efficiently in MariaDB/MySQL.
        // Some columns need more length, but these can be set on the configuration level for that Entity in particular.
        configurationBuilder.Properties<string>().HaveMaxLength(255);
        // All decimals columns should have 2 digits after the comma
        configurationBuilder.Properties<decimal>().HavePrecision(18, 2);
    }

    /// <summary>
    /// Configures the entity model by applying all entity configurations from the assembly.
    /// Also handles SQLite compatibility by stripping MySQL-specific default SQL expressions
    /// for CreatedAt and UpdatedAt properties when running tests.
    /// </summary>
    /// <param name="modelBuilder">The builder used to construct the model for this context.</param>
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // Apply all configurations first
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(ApplicationDbContext).Assembly);

        // When using SQLite (tests), strip MySQL-specific default SQL
        if (Database.ProviderName == "Microsoft.EntityFrameworkCore.Sqlite")
        {
            foreach (var entityType in modelBuilder.Model.GetEntityTypes())
            {
                var createdAt = entityType.FindProperty("CreatedAt");
                if (createdAt != null)
                {
                    createdAt.SetDefaultValueSql(null);
                }

                var updatedAt = entityType.FindProperty("UpdatedAt");
                if (updatedAt != null)
                {
                    updatedAt.SetDefaultValueSql(null);
                }
            }
        }
    }

}
