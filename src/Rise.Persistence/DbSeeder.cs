using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Rise.Domain.Products;
using Rise.Domain.Projects;
using Rise.Domain.News;

namespace Rise.Persistence;
/// <summary>
/// Seeds the database
/// </summary>
/// <param name="dbContext"></param>
/// <param name="roleManager"></param>
/// <param name="userManager"></param>
public class DbSeeder(ApplicationDbContext dbContext, RoleManager<IdentityRole> roleManager, UserManager<IdentityUser> userManager)
{
    const string PasswordDefault = "A1b2C3!";
    public async Task SeedAsync()
    {
        await RolesAsync();
        await UsersAsync();
        await ProductsAsync();
        await NewsAsync();
        await ProjectsAsync();
    }

    private async Task RolesAsync()
    {
        if (dbContext.Roles.Any())
            return;

        await roleManager.CreateAsync(new IdentityRole("Administrator"));
        await roleManager.CreateAsync(new IdentityRole("Secretary"));
        await roleManager.CreateAsync(new IdentityRole("Technician"));
    }

    private async Task UsersAsync()
    {
        if (dbContext.Users.Any())
            return;

        await dbContext.Roles.ToListAsync();

        var admin = new IdentityUser
        {
            UserName = "admin@example.com",
            Email = "admin@example.com",
            EmailConfirmed = true,
        };
        await userManager.CreateAsync(admin, PasswordDefault);

        var secretary = new IdentityUser
        {
            UserName = "secretary@example.com",
            Email = "secretary@example.com",
            EmailConfirmed = true,
        };
        await userManager.CreateAsync(secretary, PasswordDefault);

        var technicianAccount1 = new IdentityUser
        {
            UserName = "technician1@example.com",
            Email = "technician1@example.com",
            EmailConfirmed = true,
        };
        await userManager.CreateAsync(technicianAccount1, PasswordDefault);

        var technicianAccount2 = new IdentityUser
        {
            UserName = "technician2@example.com",
            Email = "technician2@example.com",
            EmailConfirmed = true,
        };
        await userManager.CreateAsync(technicianAccount2, PasswordDefault);

        var user = new IdentityUser
        {
            UserName = "user@example.com",
            Email = "user@example.com",
            EmailConfirmed = true,
        };
        await userManager.CreateAsync(user, PasswordDefault);

        await userManager.AddToRoleAsync(admin, "Administrator");
        await userManager.AddToRoleAsync(secretary, "Secretary");
        await userManager.AddToRoleAsync(technicianAccount1, "Technician");
        await userManager.AddToRoleAsync(technicianAccount2, "Technician");

        dbContext.Technicians.AddRange(
            new Technician("Tech 1", "Awesome", technicianAccount1.Id),
            new Technician("Tech 2", "Less Awesome", technicianAccount2.Id));

        await dbContext.SaveChangesAsync();
    }



    private async Task ProductsAsync()
    {
        if (dbContext.Products.Any())
            return;

        dbContext.Products.AddRange(
            new Product { Name = "Laptop", Description = "15-inch display, 16GB RAM" },
            new Product { Name = "Smartphone", Description = "6.5-inch screen, 128GB storage" },
            new Product { Name = "Headphones", Description = "Wireless noise-cancelling" },
            new Product { Name = "Keyboard", Description = "Mechanical RGB backlit" },
            new Product { Name = "Mouse", Description = "Ergonomic wireless mouse" },
            new Product { Name = "Monitor", Description = "27-inch 4K UHD display" },
            new Product { Name = "Printer", Description = "All-in-one inkjet printer" },
            new Product { Name = "Camera", Description = "Mirrorless 24MP with 4K video" },
            new Product { Name = "Smartwatch", Description = "Heart rate monitor, GPS" },
            new Product { Name = "Speaker", Description = "Bluetooth portable speaker" }
        );

        await dbContext.SaveChangesAsync();
    }

    private async Task NewsAsync()
    {
        if (dbContext.NewsItems.Any())
            return;
        
        dbContext.NewsItems.AddRange(
            new NewsItem
            {
                Title = "Blazor Server Released",
                Description = "Microsoft announces the release of Blazor Server for .NET 8.",
                Type = "Release",
                PublishDate = DateTime.Parse("2025-10-01T09:00:00Z"),
                Content = "Full article: Microsoft has released Blazor Server for .NET 8. Learn about performance improvements and migration guidance.",
                Author = "Jane Doe"
            },
            new NewsItem
            {
                Title = "Rise Platform Update",
                Description = "Rise platform receives a major update with new features and bug fixes.",
                Type = "Update",
                PublishDate = DateTime.Parse("2025-10-05T14:30:00Z"),
                Content = "Full article: This platform update includes several new modules, stability fixes and UX improvements across the product.",
                Author = "John Smith"
            },
            new NewsItem
            {
                Title = "Community Event Scheduled",
                Description = "Join us for the upcoming Rise community event in Amsterdam.",
                Type = "Event",
                PublishDate = DateTime.Parse("2025-10-10T18:00:00Z"),
                Content = "Full article: The Rise community event will feature talks, workshops and networking opportunities. Registration details inside.",
                Author = "Alice Johnson"
            },
            new NewsItem
            {
                Title = "Security Patch Released",
                Description = "A new security patch is available for all Rise applications.",
                Type = "Security",
                PublishDate = DateTime.Parse("2025-09-28T11:15:00Z"),
                Content = "Full article: We recommend applying this security patch immediately — it addresses vulnerabilities in authentication and input validation.",
                Author = "Bob Lee"
            },
            new NewsItem
            {
                Title = "Developer Tips: .NET 8",
                Description = "Top tips for developing with .NET 8 shared by the Rise team.",
                Type = "Tip",
                PublishDate = DateTime.Parse("2025-10-12T08:45:00Z"),
                Content = "Full article: A curated list of best practices, performance tips and tooling improvements for .NET 8 developers.",
                Author = "Chris Evans"
            }
            );
        
        await dbContext.SaveChangesAsync();
    }

    private async Task ProjectsAsync()
    {
        if (dbContext.Projects.Any())
            return;

        var technicians = await dbContext.Technicians.ToListAsync();

        if (!technicians.Any())
            return;

        var addresses = new List<Address>
        {
            new Address("Koningstraat 12", "Bus 3A", "Brussel", "1000"),
            new Address("Meir 45", "", "Antwerpen", "2000"),
            new Address("Veldstraat 78", "2e verdieping", "Gent", "9000"),
            new Address("Rue de la Loi 175", "", "Bruxelles", "1040"),
            new Address("Place Saint-Lambert 8", "Bureau 12", "Liège", "4000"),
        };

        var rnd = new Random(123); // Using a seed so the random is always the same.

        var projects = new List<Project>
        {
            new("Website Redesign", technicians[rnd.Next(technicians.Count)], addresses[0]),
            new("Mobile App Development", technicians[rnd.Next(technicians.Count)], addresses[1]),
            new("Database Migration", technicians[rnd.Next(technicians.Count)], addresses[2]),
            new("E-commerce Platform", technicians[rnd.Next(technicians.Count)], addresses[3]),
            new("CRM Integration", technicians[rnd.Next(technicians.Count)], addresses[4])
        };

        dbContext.Projects.AddRange(projects);
        await dbContext.SaveChangesAsync();
    }
}