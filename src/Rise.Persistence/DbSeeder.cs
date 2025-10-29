using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System;
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
            new NewsItem { Title = "Blazor Server Released", PublishDate = DateTime.Parse("2025-10-01T09:00:00Z"), Description = "Microsoft announces the release of Blazor Server for .NET 8.", Type = "Release", Content = "Full article: Microsoft has released Blazor Server for .NET 8. Learn about performance improvements and migration guidance.", Author = "Jane Doe" },
            new NewsItem { Title = "Rise Platform Update", PublishDate = DateTime.Parse("2025-10-05T14:30:00Z"), Description = "Rise platform receives a major update with new features and bug fixes.", Type = "Update", Content = "Full article: This platform update includes several new modules, stability fixes and UX improvements across the product.", Author = "John Smith" },
            new NewsItem { Title = "Community Event Scheduled", PublishDate = DateTime.Parse("2025-10-10T18:00:00Z"), Description = "Join us for the upcoming Rise community event in Amsterdam.", Type = "Event", Content = "Full article: The Rise community event will feature talks, workshops and networking opportunities. Registration details inside.", Author = "Alice Johnson" },
            new NewsItem { Title = "Security Patch Released", PublishDate = DateTime.Parse("2025-09-28T11:15:00Z"), Description = "A new security patch is available for all Rise applications.", Type = "Security", Content = "Full article: We recommend applying this security patch immediately — it addresses vulnerabilities in authentication and input validation.", Author = "Bob Lee" },
            new NewsItem { Title = "Developer Tips: .NET 8", PublishDate = DateTime.Parse("2025-10-12T08:45:00Z"), Description = "Top tips for developing with .NET 8 shared by the Rise team.", Type = "Tip", Content = "Full article: A curated list of best practices, performance tips and tooling improvements for .NET 8 developers.", Author = "Chris Evans" },
            new NewsItem { Title = "Rise Mobile App Launched", PublishDate = DateTime.Parse("2025-10-13T10:00:00Z"), Description = "The Rise mobile app is now available for download.", Type = "Launch", Content = "Full article: The Rise mobile app (iOS & Android) brings core features to your phone, including push notifications and offline support.", Author = "Emily Clark" },
            new NewsItem { Title = "New Documentation Portal", PublishDate = DateTime.Parse("2025-10-14T12:00:00Z"), Description = "Access the new documentation portal for Rise products.", Type = "Documentation", Content = "Full article: The new portal centralizes guides, API references and tutorials. Search and feedback features included.", Author = "Michael Brown" },
            new NewsItem { Title = "User Feedback Survey", PublishDate = DateTime.Parse("2025-10-15T09:30:00Z"), Description = "Participate in our user feedback survey and win prizes.", Type = "Survey", Content = "Full article: Help shape the product roadmap by completing our short survey. Participants can win swag and credits.", Author = "Sarah Lee" },
            new NewsItem { Title = "Performance Improvements", PublishDate = DateTime.Parse("2025-10-16T08:00:00Z"), Description = "Major performance improvements have been deployed.", Type = "Performance", Content = "Full article: Backend optimizations and caching improvements reduce response times across key endpoints.", Author = "David Kim" },
            new NewsItem { Title = "Rise API v2 Released", PublishDate = DateTime.Parse("2025-10-17T11:00:00Z"), Description = "Rise API v2 is now available with enhanced features.", Type = "API", Content = "Full article: API v2 introduces new endpoints, pagination improvements and better error messages. See the changelog for breaking changes.", Author = "Anna White" },
            new NewsItem { Title = "Integration with Azure", PublishDate = DateTime.Parse("2025-10-18T13:00:00Z"), Description = "Rise now integrates seamlessly with Microsoft Azure.", Type = "Integration", Content = "Full article: The Azure connector simplifies deployments and enables managed services integration.", Author = "James Green" },
            new NewsItem { Title = "Bug Bounty Program", PublishDate = DateTime.Parse("2025-10-19T15:00:00Z"), Description = "Join our bug bounty program and help improve security.", Type = "Program", Content = "Full article: Details on scope, rewards and how to submit vulnerabilities responsibly.", Author = "Olivia Black" },
            new NewsItem { Title = "Rise Community Forum", PublishDate = DateTime.Parse("2025-10-20T17:00:00Z"), Description = "Connect with other users on the Rise community forum.", Type = "Community", Content = "Full article: The forum is a place to ask questions, share projects and find collaborators.", Author = "William Scott" },
            new NewsItem { Title = "New Feature: Dark Mode", PublishDate = DateTime.Parse("2025-10-21T19:00:00Z"), Description = "Dark mode is now available in Rise applications.", Type = "Feature", Content = "Full article: Toggle dark mode in user settings. The theme persists across devices and syncs with OS preferences.", Author = "Jessica Adams" },
            new NewsItem { Title = "Rise Webinar Announced", PublishDate = DateTime.Parse("2025-10-22T09:00:00Z"), Description = "Register for our upcoming Rise webinar.", Type = "Webinar", Content = "Full article: Join product leads for a demo and Q&A. Register to receive a recording and slides.", Author = "Matthew Turner" },
            new NewsItem { Title = "Accessibility Improvements", PublishDate = DateTime.Parse("2025-10-23T11:00:00Z"), Description = "Rise apps now offer improved accessibility features.", Type = "Accessibility", Content = "Full article: Updates include improved keyboard navigation, ARIA attributes and color-contrast fixes.", Author = "Laura Hill" },
            new NewsItem { Title = "Rise Partner Program", PublishDate = DateTime.Parse("2025-10-24T13:00:00Z"), Description = "Become a partner and grow with Rise.", Type = "Partner", Content = "Full article: Program benefits, tiers, and how to apply are described in this post.", Author = "Brian Young" },
            new NewsItem { Title = "Cloud Migration Guide", PublishDate = DateTime.Parse("2025-10-25T15:00:00Z"), Description = "Check out our new guide for cloud migration.", Type = "Guide", Content = "Full article: Step-by-step cloud migration checklist and recommended patterns for minimal downtime.", Author = "Samantha King" },
            new NewsItem { Title = "Rise Hackathon Winners", PublishDate = DateTime.Parse("2025-10-26T17:00:00Z"), Description = "Congratulations to the winners of the Rise hackathon.", Type = "Event", Content = "Full article: Recap and highlights from the hackathon, plus interviews with winners.", Author = "Eric Wright" },
            new NewsItem { Title = "New Marketplace Features", PublishDate = DateTime.Parse("2025-10-27T09:00:00Z"), Description = "Explore new features in the Rise marketplace.", Type = "Marketplace", Content = "Full article: New listings, improved search and vendor tools to help sellers manage their products.", Author = "Rachel Evans" },
            new NewsItem { Title = "Rise CLI Updated", PublishDate = DateTime.Parse("2025-10-28T11:00:00Z"), Description = "The Rise CLI has been updated with new commands.", Type = "Tooling", Content = "Full article: New CLI commands simplify scaffolding, deployments and local development workflows.", Author = "Kevin Harris" },
            new NewsItem { Title = "Data Analytics Module", PublishDate = DateTime.Parse("2025-10-29T13:00:00Z"), Description = "Analyze your data with the new analytics module.", Type = "Analytics", Content = "Full article: The analytics module offers dashboards, export features and scheduled reports.", Author = "Natalie Brooks" },
            new NewsItem { Title = "Rise Support Expansion", PublishDate = DateTime.Parse("2025-10-30T15:00:00Z"), Description = "Rise support is now available in more regions.", Type = "Support", Content = "Full article: New regional support centers and extended hours for critical issues.", Author = "Patrick Reed" },
            new NewsItem { Title = "Mobile Push Notifications", PublishDate = DateTime.Parse("2025-10-31T17:00:00Z"), Description = "Receive updates via mobile push notifications.", Type = "Notifications", Content = "Full article: How to enable push notifications and manage preferences across devices.", Author = "Linda Carter" },
            new NewsItem { Title = "Rise API Documentation", PublishDate = DateTime.Parse("2025-11-01T09:00:00Z"), Description = "Check out the updated API documentation.", Type = "Documentation", Content = "Full article: API docs have been expanded with examples, SDKs and migration notes for v2.", Author = "Steven Hall" },
            new NewsItem { Title = "New User Onboarding", PublishDate = DateTime.Parse("2025-11-02T11:00:00Z"), Description = "Improved onboarding experience for new users.", Type = "Onboarding", Content = "Full article: See the step-by-step walkthrough and tips to get started quickly with Rise.", Author = "Megan Fox" },
            new NewsItem { Title = "Rise DevOps Tools", PublishDate = DateTime.Parse("2025-11-03T13:00:00Z"), Description = "DevOps tools are now integrated with Rise.", Type = "DevOps", Content = "Full article: CI/CD integrations, environment management and recommended pipelines for production.", Author = "Ryan Cooper" },
            new NewsItem { Title = "Security Best Practices", PublishDate = DateTime.Parse("2025-11-04T15:00:00Z"), Description = "Learn about security best practices for Rise.", Type = "Security", Content = "Full article: Guidelines on secure development, credential management and incident response.", Author = "Kimberly Wood" },
            new NewsItem { Title = "Rise API Rate Limits", PublishDate = DateTime.Parse("2025-11-05T17:00:00Z"), Description = "API rate limits have been updated for better performance.", Type = "API", Content = "Full article: New rate limit tiers, best practices for retry/backoff and tips to optimize usage.", Author = "Justin Bell" },
            new NewsItem { Title = "Rise UI Kit Released", PublishDate = DateTime.Parse("2025-11-06T09:00:00Z"), Description = "Build apps faster with the new Rise UI Kit.", Type = "UI", Content = "Full article: The UI Kit includes components, design tokens and accessibility-ready patterns.", Author = "Stephanie Moore" },
            new NewsItem { Title = "Rise Training Sessions", PublishDate = DateTime.Parse("2025-11-07T11:00:00Z"), Description = "Sign up for Rise training sessions this month.", Type = "Training", Content = "Full article: Classroom and online training schedules with registration links and prerequisites.", Author = "Gregory Adams" },
            new NewsItem { Title = "Marketplace Vendor Program", PublishDate = DateTime.Parse("2025-11-08T13:00:00Z"), Description = "Become a vendor in the Rise marketplace.", Type = "Marketplace", Content = "Full article: Vendor onboarding steps, fees and promotional opportunities.", Author = "Vanessa Lee" },
            new NewsItem { Title = "Rise API Changelog", PublishDate = DateTime.Parse("2025-11-09T15:00:00Z"), Description = "View the latest changes in the Rise API changelog.", Type = "Changelog", Content = "Full article: A summary of bug fixes, enhancements and breaking changes in recent releases.", Author = "Samuel Clark" },
            new NewsItem { Title = "Rise Community Awards", PublishDate = DateTime.Parse("2025-11-10T17:00:00Z"), Description = "Nominate your peers for the Rise community awards.", Type = "Community", Content = "Full article: Categories, nomination process and judging criteria for the community awards.", Author = "Tina Brooks" }
        
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