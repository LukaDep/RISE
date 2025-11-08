using System;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Rise.Domain.Absences;
using Rise.Domain.Campus;
using Rise.Domain.CampusInfo;
using Rise.Domain.Contact;
using Rise.Domain.Grades;
using Rise.Domain.Menu;
using Rise.Domain.News;
using Rise.Domain.Restos;
using Rise.Domain.Schedule;
using Rise.Shared.Common;

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
        await NewsAsync();
        await AbsencesAsync();
        await CampusesAsync();
        await ContactsAsync();
        await GradesAsync();
        await RestosAndMenusAsync();
        await ReservationsAsync();
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

        var user = new IdentityUser
        {
            UserName = "user@example.com",
            Email = "user@example.com",
            EmailConfirmed = true,
        };
        await userManager.CreateAsync(user, PasswordDefault);

        await userManager.AddToRoleAsync(admin, "Administrator");
        await userManager.AddToRoleAsync(secretary, "Secretary");
        await dbContext.SaveChangesAsync();
    }


    private async Task NewsAsync()
    {
        if (dbContext.NewsArticles.Any())
            return;
        dbContext.NewsArticles.AddRange(
            new NewsArticle { Title = "Blazor Server Released", PublishDate = DateTime.Parse("2025-10-01T09:00:00Z"), Description = "Microsoft announces the release of Blazor Server for .NET 8.", Type = "Release", Content = "Full article: Microsoft has released Blazor Server for .NET 8. Learn about performance improvements and migration guidance.", Author = "Jane Doe" },
            new NewsArticle { Title = "Rise Platform Update", PublishDate = DateTime.Parse("2025-10-05T14:30:00Z"), Description = "Rise platform receives a major update with new features and bug fixes.", Type = "Update", Content = "Full article: This platform update includes several new modules, stability fixes and UX improvements across the product.", Author = "John Smith" },
            new NewsArticle { Title = "Community Event Scheduled", PublishDate = DateTime.Parse("2025-10-10T18:00:00Z"), Description = "Join us for the upcoming Rise community event in Amsterdam.", Type = "Event", Content = "Full article: The Rise community event will feature talks, workshops and networking opportunities. Registration details inside.", Author = "Alice Johnson" },
            new NewsArticle { Title = "Security Patch Released", PublishDate = DateTime.Parse("2025-09-28T11:15:00Z"), Description = "A new security patch is available for all Rise applications.", Type = "Security", Content = "Full article: We recommend applying this security patch immediately — it addresses vulnerabilities in authentication and input validation.", Author = "Bob Lee" },
            new NewsArticle { Title = "Developer Tips: .NET 8", PublishDate = DateTime.Parse("2025-10-12T08:45:00Z"), Description = "Top tips for developing with .NET 8 shared by the Rise team.", Type = "Tip", Content = "Full article: A curated list of best practices, performance tips and tooling improvements for .NET 8 developers.", Author = "Chris Evans" },
            new NewsArticle { Title = "Rise Mobile App Launched", PublishDate = DateTime.Parse("2025-10-13T10:00:00Z"), Description = "The Rise mobile app is now available for download.", Type = "Launch", Content = "Full article: The Rise mobile app (iOS & Android) brings core features to your phone, including push notifications and offline support.", Author = "Emily Clark" },
            new NewsArticle { Title = "New Documentation Portal", PublishDate = DateTime.Parse("2025-10-14T12:00:00Z"), Description = "Access the new documentation portal for Rise products.", Type = "Documentation", Content = "Full article: The new portal centralizes guides, API references and tutorials. Search and feedback features included.", Author = "Michael Brown" },
            new NewsArticle { Title = "User Feedback Survey", PublishDate = DateTime.Parse("2025-10-15T09:30:00Z"), Description = "Participate in our user feedback survey and win prizes.", Type = "Survey", Content = "Full article: Help shape the product roadmap by completing our short survey. Participants can win swag and credits.", Author = "Sarah Lee" },
            new NewsArticle { Title = "Performance Improvements", PublishDate = DateTime.Parse("2025-10-16T08:00:00Z"), Description = "Major performance improvements have been deployed.", Type = "Performance", Content = "Full article: Backend optimizations and caching improvements reduce response times across key endpoints.", Author = "David Kim" },
            new NewsArticle { Title = "Rise API v2 Released", PublishDate = DateTime.Parse("2025-10-17T11:00:00Z"), Description = "Rise API v2 is now available with enhanced features.", Type = "API", Content = "Full article: API v2 introduces new endpoints, pagination improvements and better error messages. See the changelog for breaking changes.", Author = "Anna White" },
            new NewsArticle { Title = "Integration with Azure", PublishDate = DateTime.Parse("2025-10-18T13:00:00Z"), Description = "Rise now integrates seamlessly with Microsoft Azure.", Type = "Integration", Content = "Full article: The Azure connector simplifies deployments and enables managed services integration.", Author = "James Green" },
            new NewsArticle { Title = "Bug Bounty Program", PublishDate = DateTime.Parse("2025-10-19T15:00:00Z"), Description = "Join our bug bounty program and help improve security.", Type = "Program", Content = "Full article: Details on scope, rewards and how to submit vulnerabilities responsibly.", Author = "Olivia Black" },
            new NewsArticle { Title = "Rise Community Forum", PublishDate = DateTime.Parse("2025-10-20T17:00:00Z"), Description = "Connect with other users on the Rise community forum.", Type = "Community", Content = "Full article: The forum is a place to ask questions, share projects and find collaborators.", Author = "William Scott" },
            new NewsArticle { Title = "New Feature: Dark Mode", PublishDate = DateTime.Parse("2025-10-21T19:00:00Z"), Description = "Dark mode is now available in Rise applications.", Type = "Feature", Content = "Full article: Toggle dark mode in user settings. The theme persists across devices and syncs with OS preferences.", Author = "Jessica Adams" },
            new NewsArticle { Title = "Rise Webinar Announced", PublishDate = DateTime.Parse("2025-10-22T09:00:00Z"), Description = "Register for our upcoming Rise webinar.", Type = "Webinar", Content = "Full article: Join product leads for a demo and Q&A. Register to receive a recording and slides.", Author = "Matthew Turner" },
            new NewsArticle { Title = "Accessibility Improvements", PublishDate = DateTime.Parse("2025-10-23T11:00:00Z"), Description = "Rise apps now offer improved accessibility features.", Type = "Accessibility", Content = "Full article: Updates include improved keyboard navigation, ARIA attributes and color-contrast fixes.", Author = "Laura Hill" },
            new NewsArticle { Title = "Rise Partner Program", PublishDate = DateTime.Parse("2025-10-24T13:00:00Z"), Description = "Become a partner and grow with Rise.", Type = "Partner", Content = "Full article: Program benefits, tiers, and how to apply are described in this post.", Author = "Brian Young" },
            new NewsArticle { Title = "Cloud Migration Guide", PublishDate = DateTime.Parse("2025-10-25T15:00:00Z"), Description = "Check out our new guide for cloud migration.", Type = "Guide", Content = "Full article: Step-by-step cloud migration checklist and recommended patterns for minimal downtime.", Author = "Samantha King" },
            new NewsArticle { Title = "Rise Hackathon Winners", PublishDate = DateTime.Parse("2025-10-26T17:00:00Z"), Description = "Congratulations to the winners of the Rise hackathon.", Type = "Event", Content = "Full article: Recap and highlights from the hackathon, plus interviews with winners.", Author = "Eric Wright" },
            new NewsArticle { Title = "New Marketplace Features", PublishDate = DateTime.Parse("2025-10-27T09:00:00Z"), Description = "Explore new features in the Rise marketplace.", Type = "Marketplace", Content = "Full article: New listings, improved search and vendor tools to help sellers manage their products.", Author = "Rachel Evans" },
            new NewsArticle { Title = "Rise CLI Updated", PublishDate = DateTime.Parse("2025-10-28T11:00:00Z"), Description = "The Rise CLI has been updated with new commands.", Type = "Tooling", Content = "Full article: New CLI commands simplify scaffolding, deployments and local development workflows.", Author = "Kevin Harris" },
            new NewsArticle { Title = "Data Analytics Module", PublishDate = DateTime.Parse("2025-10-29T13:00:00Z"), Description = "Analyze your data with the new analytics module.", Type = "Analytics", Content = "Full article: The analytics module offers dashboards, export features and scheduled reports.", Author = "Natalie Brooks" },
            new NewsArticle { Title = "Rise Support Expansion", PublishDate = DateTime.Parse("2025-10-30T15:00:00Z"), Description = "Rise support is now available in more regions.", Type = "Support", Content = "Full article: New regional support centers and extended hours for critical issues.", Author = "Patrick Reed" },
            new NewsArticle { Title = "Mobile Push Notifications", PublishDate = DateTime.Parse("2025-10-31T17:00:00Z"), Description = "Receive updates via mobile push notifications.", Type = "Notifications", Content = "Full article: How to enable push notifications and manage preferences across devices.", Author = "Linda Carter" },
            new NewsArticle { Title = "Rise API Documentation", PublishDate = DateTime.Parse("2025-11-01T09:00:00Z"), Description = "Check out the updated API documentation.", Type = "Documentation", Content = "Full article: API docs have been expanded with examples, SDKs and migration notes for v2.", Author = "Steven Hall" },
            new NewsArticle { Title = "New User Onboarding", PublishDate = DateTime.Parse("2025-11-02T11:00:00Z"), Description = "Improved onboarding experience for new users.", Type = "Onboarding", Content = "Full article: See the step-by-step walkthrough and tips to get started quickly with Rise.", Author = "Megan Fox" },
            new NewsArticle { Title = "Rise DevOps Tools", PublishDate = DateTime.Parse("2025-11-03T13:00:00Z"), Description = "DevOps tools are now integrated with Rise.", Type = "DevOps", Content = "Full article: CI/CD integrations, environment management and recommended pipelines for production.", Author = "Ryan Cooper" },
            new NewsArticle { Title = "Security Best Practices", PublishDate = DateTime.Parse("2025-11-04T15:00:00Z"), Description = "Learn about security best practices for Rise.", Type = "Security", Content = "Full article: Guidelines on secure development, credential management and incident response.", Author = "Kimberly Wood" },
            new NewsArticle { Title = "Rise API Rate Limits", PublishDate = DateTime.Parse("2025-11-05T17:00:00Z"), Description = "API rate limits have been updated for better performance.", Type = "API", Content = "Full article: New rate limit tiers, best practices for retry/backoff and tips to optimize usage.", Author = "Justin Bell" },
            new NewsArticle { Title = "Rise UI Kit Released", PublishDate = DateTime.Parse("2025-11-06T09:00:00Z"), Description = "Build apps faster with the new Rise UI Kit.", Type = "UI", Content = "Full article: The UI Kit includes components, design tokens and accessibility-ready patterns.", Author = "Stephanie Moore" },
            new NewsArticle { Title = "Rise Training Sessions", PublishDate = DateTime.Parse("2025-11-07T11:00:00Z"), Description = "Sign up for Rise training sessions this month.", Type = "Training", Content = "Full article: Classroom and online training schedules with registration links and prerequisites.", Author = "Gregory Adams" },
            new NewsArticle { Title = "Marketplace Vendor Program", PublishDate = DateTime.Parse("2025-11-08T13:00:00Z"), Description = "Become a vendor in the Rise marketplace.", Type = "Marketplace", Content = "Full article: Vendor onboarding steps, fees and promotional opportunities.", Author = "Vanessa Lee" },
            new NewsArticle { Title = "Rise API Changelog", PublishDate = DateTime.Parse("2025-11-09T15:00:00Z"), Description = "View the latest changes in the Rise API changelog.", Type = "Changelog", Content = "Full article: A summary of bug fixes, enhancements and breaking changes in recent releases.", Author = "Samuel Clark" },
            new NewsArticle { Title = "Rise Community Awards", PublishDate = DateTime.Parse("2025-11-10T17:00:00Z"), Description = "Nominate your peers for the Rise community awards.", Type = "Community", Content = "Full article: Categories, nomination process and judging criteria for the community awards.", Author = "Tina Brooks" }

        );
        await dbContext.SaveChangesAsync();
    }
    private async Task AbsencesAsync()
    {
        if (dbContext.Absences.Any())
            return;

        dbContext.Absences.AddRange(
            new Absence { Name = "Bert Van Vreckem", Reason = "Sick Leave", StartDate = DateTime.Parse("2025-12-23"), EndDate = DateTime.Parse("2025-12-24") },
            new Absence { Name = "Thomas Parmentier", Reason = "Conference", StartDate = DateTime.Parse("2025-12-27"), EndDate = DateTime.Parse("2025-12-29") },
            new Absence { Name = "Chloé De Leenheer", Reason = "Parental Leave", StartDate = DateTime.Parse("2025-11-01"), EndDate = DateTime.Parse("2025-12-31") },
            new Absence { Name = "Jan Decorte", Reason = "Volunteer Work", StartDate = DateTime.Parse("2025-12-10"), EndDate = DateTime.Parse("2025-12-15") },
            new Absence { Name = "Sarah Vermeulen", Reason = "Training", StartDate = DateTime.Parse("2025-12-20"), EndDate = DateTime.Parse("2025-12-22") },
            new Absence { Name = "Marc Desmet", Reason = "Family Emergency", StartDate = DateTime.Parse("2025-12-25"), EndDate = DateTime.Parse("2025-12-26") },
            new Absence { Name = "Lien Decorte", Reason = "Study Leave", StartDate = DateTime.Parse("2025-12-15"), EndDate = DateTime.Parse("2025-12-20") },
            new Absence { Name = "Bert Van Vreckem", Reason = "Unpaid Leave", StartDate = DateTime.Parse("2025-12-01"), EndDate = DateTime.Parse("2025-12-31") },
            new Absence { Name = "Thomas Parmentier", Reason = "Team Building Event", StartDate = DateTime.Parse("2025-12-30"), EndDate = DateTime.Parse("2025-12-30") },
            new Absence { Name = "Chloé De Leenheer", Reason = "Relocation", StartDate = DateTime.Parse("2025-12-28"), EndDate = DateTime.Parse("2025-12-30") },
            new Absence { Name = "Sarah Vermeulen", Reason = "Sick Leave", StartDate = DateTime.Parse("2025-10-29"), EndDate = DateTime.Parse("2025-10-29") }
        );

        await dbContext.SaveChangesAsync();
    }

    private async Task CampusesAsync()
    {
        if (dbContext.Campuses.Any())
            return;

        var campuses = new List<Campus>
        {
            new Campus
            {
                Name = "Campus Aalst",
                Street = "Arbeidstraat",
                HouseNumber = "14",
                City = "Aalst",
                PostalCode = "9300",
                ContactPhone = "09 243 20 15",
                Description = "Campus Aalst is vlot bereikbaar en centraal gelegen op wandelafstand van station Aalst (850 m), bushalte 'Vredeplein' (200 m), parking Hopmarkt (350 m) en parking Keizershallen (350 m).",
                Facilities = ["Cafetaria", "Parking", "Library"],
                Latitude = 50.937566452189515,
                Longitude = 4.033365889741001
            },
            new Campus
            {
                Name = "Campus Bijloke",
                Street = "Godshuizenlaan",
                HouseNumber = "4",
                City = "Gent",
                PostalCode = "9000",
                ContactPhone = "09 243 20 13",
                Description = "Campus Bijloke bevindt zich ideaal gelegen aan de kleine ring van Gent (R40), op 6 minuutjes fietsen of 15 minuten wandelen van het station Gent Sint-Pieters (1,5 km).",
                Facilities = ["Cafetaria", "Parking", "Library"],
                Latitude = 51.045049214656395,
                Longitude = 3.7146069594104048
            },
            new Campus
            {
                Name = "Campus Grote Sikkel",
                Street = "Grote Sikkel",
                HouseNumber = "1",
                City = "Gent",
                PostalCode = "9000",
                ContactPhone = "09 243 20 14",
                Description = "Campus Grote Sikkel is gelegen in het hart van Gent, op wandelafstand van het station Gent-Sint-Pieters (1,2 km) en het centrum (750 m).",
                Facilities = ["Cafetaria", "Parking", "Library"],
                Latitude = 51.05376586462268,
                Longitude = 3.7265563026441186
            },
            new Campus
            {
                Name = "Campus Ledeganck",
                Street = "K.L. Ledeganckstraat",
                HouseNumber = "35",
                City = "Gent",
                PostalCode = "9000",
                ContactPhone = "09 243 20 16",
                Description = "Campus Ledeganck is gelegen nabij het centrum van Gent, op 10 minuutjes wandelen van het station Gent-Sint-Pieters (1,3 km) en op 5 minuutjes van het centrum (500 m).",
                Facilities = ["Cafetaria", "Parking", "Library"],
                Latitude = 51.0365068204106,
                Longitude = 3.724652944967813
            },
            new Campus
            {
                Name = "Campus Lokeren",
                Street = "Groendreef",
                HouseNumber = "31",
                City = "Lokeren",
                PostalCode = "9160",
                ContactPhone = "09 243 20 17",
                Description = "Campus Lokeren is gelegen op wandelafstand van het station Lokeren (850 m) en beschikt over een ruime parking.",
                Facilities = ["Cafetaria", "Parking", "Library"],
                Latitude = 51.10908490773775,
                Longitude = 3.9861375735619933
            },
            new Campus
            {
                Name = "Campus Melle",
                Street = "Brusselsesteenweg",
                HouseNumber = "161",
                City = "Melle",
                PostalCode = "9090",
                ContactPhone = "09 243 20 18",
                Description = "Campus Melle is gelegen langs de Brusselsesteenweg (N9), op 10 minuutjes wandelen van het station Melle (800 m) en beschikt over een ruime parking.",
                Facilities = ["Cafetaria", "Parking", "Library"], 
                Latitude = 51.01430882035527,
                Longitude = 3.7861257779015367
            },
            new Campus
            {
                Name = "Campus Mercator",
                Street = "Wasstraat",
                HouseNumber = "1",
                City = "Gent",
                PostalCode = "9000",
                ContactPhone = "09 243 20 19",
                Description = "Campus Mercator is gelegen nabij het centrum van Gent, op 15 minuutjes wandelen van het station Gent-Sint-Pieters (1,5 km) en op 10 minuutjes van het centrum (800 m).",
                Facilities = ["Cafetaria", "Parking", "Library"],
                Latitude = 51.04246772832513,
                Longitude = 3.715410727376597
            },
            new Campus
            {
                Name = "Campus Schoonmeersen",
                Street = "Valentin Vaerwyckweg",
                HouseNumber = "1",
                City = "Gent",
                PostalCode = "9000",
                ContactPhone = "09 243 20 20",
                Description = "Campus Schoonmeersen is gelegen aan de rand van Gent, op 10 minuutjes fietsen of 20 minuten wandelen van het station Gent-Sint-Pieters (3 km).",
                Facilities = ["Cafetaria", "Parking", "Library", "Sports Center"],
                Latitude = 51.033100067702385,
                Longitude = 3.7030483298315624
            },
            new Campus
            {
                Name = "Campus Vesalius",
                Street = "Keramiekstraat",
                HouseNumber = "80",
                City = "Gent",
                PostalCode = "9000",
                ContactPhone = "09 243 20 21",
                Description = "Campus Vesalius is gelegen nabij het centrum van Gent, op 12 minuutjes wandelen van het station Gent-Sint-Pieters (1,4 km) en op 7 minuutjes van het centrum (600 m).",
                Facilities = ["Cafetaria", "Parking", "Library"],
                Latitude = 51.0198962680327,
                Longitude = 3.727240076846467
            },
            new Campus
            {
                Name = "Proefhoeve Bottelare",
                Street = "Diepestraat",
                HouseNumber = "1",
                City = "Bottelare",
                PostalCode = "9820",
                ContactPhone = "09 243 20 22",
                Description = "De Proefhoeve Bottelare is gelegen in het landelijke Bottelare, op 15 minuutjes rijden van Gent centrum.",
                Facilities = ["Parking", "Gardens"],
                Latitude = 50.961813660026195,
                Longitude = 3.7600818324091763
            },
            new Campus
            {
                Name = "Site Buchtenstraat (FTI Lab)",
                Street = "Buchtenstraat",
                HouseNumber = "9",
                City = "Gent",
                PostalCode = "9000",
                ContactPhone = "09 243 20 23",
                Description = "De FTI Lab locatie in de Buchtenstraat is gelegen nabij het centrum van Gent, op 10 minuutjes wandelen van het station Gent-Sint-Pieters (1,2 km) en op 5 minuutjes van het centrum (500 m).",
                Facilities = ["Lab Facilities", "Parking"],
                Latitude = 51.0289436405976,
                Longitude = 3.6854235892972094
            },
            new Campus
            {
                Name = "Site Geraard de Duivelstraat",
                Street = "Geraard de Duivelstraat",
                HouseNumber = "5",
                City = "Gent",
                PostalCode = "9000",
                ContactPhone = "09 243 20 24",
                Description = "De locatie in de Geraard de Duivelstraat is gelegen nabij het centrum van Gent, op 12 minuutjes wandelen van het station Gent-Sint-Pieters (1,3 km) en op 7 minuutjes van het centrum (600 m).",
                Facilities = ["Office Facilities", "Parking"],
                Latitude = 51.052512156439,
                Longitude = 3.728061840428318
            }
        };

        dbContext.Campuses.AddRange(campuses);
        await dbContext.SaveChangesAsync();

        // Add buildings after campuses are saved
        var schoonmeersen = campuses.FirstOrDefault(c => c.Name == "Campus Schoonmeersen");
        var mercator = campuses.FirstOrDefault(c => c.Name == "Campus Mercator");
        var bijloke = campuses.FirstOrDefault(c => c.Name == "Campus Bijloke");

        var allBuildings = new List<(Building building, Guid campusId)>();

        if (schoonmeersen != null)
        {
            allBuildings.AddRange(new[]
            {
                (new Building { Name = "Gebouw B", Address = "Valentin Vaerwyckweg 1, 9000 Gent", Type = "building", Latitude = 51.03138465268992, Longitude = 3.701414635630698, CampusId = schoonmeersen.Id}, schoonmeersen.Id),
                (new Building { Name = "Gebouw C", Address = "Valentin Vaerwyckweg 1, 9000 Gent", Type = "building", Latitude = 51.03195215277462, Longitude = 3.704568306783877, CampusId = schoonmeersen.Id }, schoonmeersen.Id),
                (new Building { Name = "Gebouw D", Address = "Valentin Vaerwyckweg 1, 9000 Gent", Type = "building", Latitude = 51.031511099994304, Longitude = 3.702789535635411, CampusId = schoonmeersen.Id }, schoonmeersen.Id),
                (new Building { Name = "Gebouw E", Address = "Valentin Vaerwyckweg 1, 9000 Gent", Type = "building", Latitude = 51.0310611550396, Longitude = 3.7045170251451722, CampusId = schoonmeersen.Id }, schoonmeersen.Id),
                (new Building { Name = "Gebouw P", Address = "Valentin Vaerwyckweg 1, 9000 Gent", Type = "building", Latitude = 51.03423555310173, Longitude = 3.7019467933058716, CampusId = schoonmeersen.Id }, schoonmeersen.Id),
                (new Building { Name = "Sporthal", Address = "Sint-Denijslaan 251, 9000 Gent", Type = "sport", Latitude = 51.03493515601912, Longitude = 3.704163329549105, CampusId = schoonmeersen.Id }, schoonmeersen.Id),
                (new Building { Name = "Gebouw T", Address = "Voskenslaan 364A, 9000 Gent", Type = "building", Latitude = 51.028515604065866, Longitude = 3.70666265806964, CampusId = schoonmeersen.Id }, schoonmeersen.Id)
            });
        }

        if (mercator != null)
        {
            allBuildings.AddRange(new[]
            {
                (new Building { Name = "Gebouw C", Address = "Nonnemeersstraat 19-21, 9000 Gent", Type = "building", Latitude = 51.04365987772189, Longitude = 3.7133038554040447, CampusId = mercator.Id }, mercator.Id),
                (new Building { Name = "Gebouw D", Address = "Nonnemeersstraat 15-17, 9000 Gent", Type = "building", Latitude = 51.04409655468585, Longitude = 3.7139953687425247, CampusId = mercator.Id }, mercator.Id),
                (new Building { Name = "Gebouw E", Address = "Nonnemeersstraat 24, 9000 Gent", Type = "building", Latitude = 51.044138254547896, Longitude = 3.7140100810412755, CampusId = mercator.Id }, mercator.Id),
                (new Building { Name = "Gebouw G", Address = "Henleykaai 84, 9000 Gent", Type = "building", Latitude = 51.04198292611773, Longitude = 3.715517179744473, CampusId = mercator.Id }, mercator.Id)
            });
        }

        if (bijloke != null)
        {
            allBuildings.AddRange(new[]
            {
                (new Building { Name = "Pauli", Address = "J. Kluyskensstraat 2, 9000 Gent", Type = "building", Latitude = 51.04559751827652, Longitude = 3.7185065415747798, CampusId = bijloke.Id }, bijloke.Id),
                (new Building { Name = "Cloquet", Address = "Pasteurlaan 2, 9000 Gent", Type = "building", Latitude = 51.0452537498002, Longitude = 3.715110343694497, CampusId = bijloke.Id }, bijloke.Id),
                (new Building { Name = "Marissal", Address = "Pasteurlaan 2, 9000 Gent", Type = "building", Latitude = 51.0452537498002, Longitude = 3.715110343694497, CampusId = bijloke.Id }, bijloke.Id),
                (new Building { Name = "Bijlokekaai", Address = "Bijlokekaai 5, 9000 Gent", Type = "building", Latitude = 51.04371555532695, Longitude = 3.7193240387340807, CampusId = bijloke.Id }, bijloke.Id),
                (new Building { Name = "Kunstenbibliotheek Huis van de Abdis", Address = "Godshuizenlaan 2, 9000 Gent", Type = "library", Latitude = 51.04393486454849, Longitude = 3.717493005440041, CampusId = bijloke.Id }, bijloke.Id)
            });
        }

        foreach (var (building, campusId) in allBuildings)
        {
            dbContext.Buildings.Add(building);
            dbContext.Entry(building).Property("CampusId").CurrentValue = campusId;
        }
        await dbContext.SaveChangesAsync();
    }
    
    private async Task ContactsAsync()
    {
        if (dbContext.Contacts.Any())
            return;

        dbContext.Contacts.AddRange(
            new Contact { Type = "organisatie", Name = "HOGENT", PhoneNumber = "09 243 33 33", Email = "info@hogent.be" },
            new Contact { Type = "departement", Name = "Bedrijf en Organisatie", ContactPerson = "Rudi Madalijns", Email = "Rudi.Madalijns@hogent.be" },
            new Contact { Type = "departement", Name = "IT en Digitale Innovatie", ContactPerson = "Chantal Teerlinck", Email = "Chantal.Teerlinck@hogent.be" },
            new Contact { Type = "campus", Name = "Campus Schoonmeersen", PhoneNumber = "09 243 20 04" },
            new Contact { Type = "campus", Name = "Campus Mercator", PhoneNumber = "09 243 20 16" },
            new Contact { Type = "directie", Name = "Algemene directie", ContactPerson = "Koen Goethals", Email = "koen.goethals@hogent.be" }
        );

        await dbContext.SaveChangesAsync();
    }

    private async Task GradesAsync()
    {
        if (dbContext.Grades.Any())
            return;

        dbContext.Grades.AddRange(
            new Grade
            {
                CourseId = "C30542",
                CourseName = "Web Development 3",
                Year = "2024-2025",
                Semester = 1,
                Name = "Project 1 - Portfolio Website",
                ActivityType = "Assignment",
                MaxPoints = 20,
                Score = 17,
                Feedback = "Excellent structure and clean styling. Minor accessibility issues with ARIA labels.",
                SubmissionDate = DateTime.Parse("2025-03-18T10:25:00Z"),
                Date = DateTime.Parse("2025-03-17T23:59:00Z")
            },
            new Grade
            {
                CourseId = "C30542",
                CourseName = "Web Development 3",
                Year = "2024-2025",
                Semester = 1,
                Name = "Quiz 2 - JavaScript Concepts",
                ActivityType = "Quiz",
                MaxPoints = 10,
                Score = 8,
                Feedback = "Solid understanding of closures; review promises syntax.",
                SubmissionDate = DateTime.Parse("2025-04-02T09:40:00Z"),
                Date = DateTime.Parse("2025-04-01T23:59:00Z")
            },
            new Grade
            {
                CourseId = "C30549",
                CourseName = "Databases 2",
                Year = "2024-2025",
                Semester = 1,
                Name = "Normalization Assignment",
                ActivityType = "Assignment",
                MaxPoints = 20,
                Score = 14,
                Feedback = "Good normalization work, but ensure all tables have proper keys.",
                SubmissionDate = DateTime.Parse("2025-02-12T15:10:00Z"),
                Date = DateTime.Parse("2025-02-11T23:59:00Z")
            }
        );

        await dbContext.SaveChangesAsync();
    }

    private async Task RestosAndMenusAsync()
    {
        if (dbContext.Restos.Any())
            return;

        // First, get buildings
        var buildingD = await dbContext.Buildings.FirstOrDefaultAsync(b => b.Name == "Gebouw D");
        var buildingB = await dbContext.Buildings.FirstOrDefaultAsync(b => b.Name == "Gebouw B" && b.Address.Contains("Valentin"));
        var buildingP = await dbContext.Buildings.FirstOrDefaultAsync(b => b.Name == "Gebouw P");

        if (buildingD == null || buildingB == null || buildingP == null)
            return;

        var resto1 = new Resto
        {
            Name = "Resto Schoonmeersen D",
            Description = "Studentenrestaurant in gebouw D, campus Schoonmeersen. Dagschotels, broodjes, warme dranken.",
            BuildingId = buildingD.Id,
            OpeningHours = new Dictionary<DayOfWeek, string>
            {
                { DayOfWeek.Monday, "08:00-17:00" },
                { DayOfWeek.Tuesday, "08:00-17:00" },
                { DayOfWeek.Wednesday, "08:00-17:00" },
                { DayOfWeek.Thursday, "08:00-17:00" },
                { DayOfWeek.Friday, "08:00-15:00" }
            },
            IsCurrentlyOpen = false,
            KitchenType = new List<string> { "Hot", "Cold" },
            PhoneNumber = "+32 9 123 45 67",
            Email = "resto.schoonmeersen.d@hogent.be",
            ImageUrl = "https://images.hln.be/ZmQ4ZDdhNGZkMjYxMmM1Yzg0NDgvZGlvLzE3NjQ5NTMzOS9maXQtd2lkdGgvMTIwMA/in-het-studentenrestaurant-van-hogent-mag-je-maar-met-2-aan-een-tafel-van-6-zitten"
        };

        var resto2 = new Resto
        {
            Name = "Resto Schoonmeersen B",
            Description = "Studentenrestaurant in gebouw B, campus Schoonmeersen. Dagschotels, broodjes, warme dranken.",
            BuildingId = buildingB.Id,
            OpeningHours = new Dictionary<DayOfWeek, string>
            {
                { DayOfWeek.Monday, "08:00-17:00" },
                { DayOfWeek.Tuesday, "08:00-17:00" },
                { DayOfWeek.Wednesday, "08:00-17:00" },
                { DayOfWeek.Thursday, "08:00-17:00" },
                { DayOfWeek.Friday, "08:00-15:00" }
            },
            IsCurrentlyOpen = false,
            KitchenType = new List<string> { "Hot", "Cold" },
            PhoneNumber = "+32 9 123 45 68",
            Email = "resto.schoonmeersen.b@hogent.be",
            ImageUrl = "https://images.hln.be/ZmQ4ZDdhNGZkMjYxMmM1Yzg0NDgvZGlvLzE3NjQ5NTMzOS9maXQtd2lkdGgvMTIwMA/in-het-studentenrestaurant-van-hogent-mag-je-maar-met-2-aan-een-tafel-van-6-zitten"
        };

        dbContext.Restos.AddRange(resto1, resto2);
        await dbContext.SaveChangesAsync();

        // Add menus and menu items
        var menu1 = new Menu
        {
            RestoId = resto1.Id,
            Date = DateTime.Parse("2025-10-20T11:30:00"),
            MenuItems = new List<MenuItem>
            {
                new MenuItem
                {
                    Name = "Tomatensoep met balletjes",
                    Description = "Dagverse soep",
                    Type = FoodType.Soep,
                    PriceStudent = 1.0,
                    PriceExtern = 2.15,
                    IsVeggie = true,
                    IsVegan = true
                },
                new MenuItem
                {
                    Name = "Varkensgebraad met mosterdsaus",
                    Description = "Vers bereid hoofdgerecht",
                    Type = FoodType.WarmeMaaltijd,
                    PriceStudent = 5.3,
                    PriceExtern = 11.7,
                    IsVeggie = false,
                    IsVegan = false
                }
            }
        };

        dbContext.Menus.Add(menu1);
        await dbContext.SaveChangesAsync();
    }

    private async Task ReservationsAsync()
    {
        if (dbContext.Reservations.Any())
            return;

        dbContext.Reservations.AddRange(
            new Reservation
            {
                StartDateTime = DateTime.Parse("2025-09-01T08:30:00"),
                EndDateTime = DateTime.Parse("2025-09-01T10:30:00"),
                Course = "PBA-TIN/Web Ontwikkeling 2",
                WorkForm = "Hoorcollege",
                Environment = "Digitaal (laptop/PC)",
                Room = "GSCHB.2.009",
                Teacher = "Bert Van Vreckem",
                IsAbsent = false
            },
            new Reservation
            {
                StartDateTime = DateTime.Parse("2025-09-01T11:00:00"),
                EndDateTime = DateTime.Parse("2025-09-01T13:00:00"),
                Course = "PBA-TIN/Databanken II",
                WorkForm = "Activerend hoorcollege",
                Environment = "Digitaal (laptop/PC)",
                Room = "GSCHB.3.012",
                Teacher = "Thomas Parmentier",
                IsAbsent = false
            },
            new Reservation
            {
                StartDateTime = DateTime.Parse("2025-09-01T14:00:00"),
                EndDateTime = DateTime.Parse("2025-09-01T17:00:00"),
                Course = "PBA-TIN/Software Engineering",
                WorkForm = "Practicum",
                Environment = "Digitaal (laptop/PC)",
                Room = "GSCHB.3.026",
                Teacher = "Chloé De Leenheer",
                IsAbsent = false
            }
        );

        await dbContext.SaveChangesAsync();
    }
}