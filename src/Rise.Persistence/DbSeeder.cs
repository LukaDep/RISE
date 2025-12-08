using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Rise.Domain.Absences;
using Rise.Domain.Campus;
using Rise.Domain.Contact;
using Rise.Domain.Events;
using Rise.Domain.Grades;
using Rise.Domain.HomeWidgets;
using Rise.Domain.Menu;
using Rise.Domain.News;
using Rise.Domain.Restos;
using Rise.Domain.StudentCards;
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
        await EventsAsync();
        await GradesAsync();
        await RestosAndMenusAsync();
        await WidgetsAsync();
        await UserWidgetsAsync();
    }

    private async Task RolesAsync()
    {
        if (dbContext.Roles.Any())
            return;

        await roleManager.CreateAsync(new IdentityRole("Administrator"));
        await roleManager.CreateAsync(new IdentityRole("Secretary"));
        await roleManager.CreateAsync(new IdentityRole("Lector"));
        await roleManager.CreateAsync(new IdentityRole("Student"));
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

        var lector1 = new IdentityUser
        {
            UserName = "lector@example.com",
            Email = "lector@example.com",
            EmailConfirmed = true,
        };
        await userManager.CreateAsync(lector1, PasswordDefault);

        var student1 = new IdentityUser
        {
            UserName = "jan.vermeulen@student.hogent.be",
            Email = "jan.vermeulen@student.hogent.be",
            EmailConfirmed = true,
        };
        await userManager.CreateAsync(student1, PasswordDefault);

        var student2 = new IdentityUser
        {
            UserName = "marie.dubois@student.hogent.be",
            Email = "marie.dubois@student.hogent.be",
            EmailConfirmed = true,
        };
        await userManager.CreateAsync(student2, PasswordDefault);

        var student3 = new IdentityUser
        {
            UserName = "pieter.janssens@student.hogent.be",
            Email = "pieter.janssens@student.hogent.be",
            EmailConfirmed = true,
        };
        await userManager.CreateAsync(student3, PasswordDefault);

        var student4 = new IdentityUser
        {
            UserName = "sophie.nguyen@student.hogent.be",
            Email = "sophie.nguyen@student.hogent.be",
            EmailConfirmed = true,
        };
        await userManager.CreateAsync(student4, PasswordDefault);

        var student5 = new IdentityUser
        {
            UserName = "thomas.maes@student.hogent.be",
            Email = "thomas.maes@student.hogent.be",
            EmailConfirmed = true,
        };
        await userManager.CreateAsync(student5, PasswordDefault);

        await userManager.AddToRoleAsync(admin, "Administrator");
        await userManager.AddToRoleAsync(secretary, "Secretary");
        await userManager.AddToRoleAsync(lector1, "Lector");
        await userManager.AddToRoleAsync(student1, "Student");
        await userManager.AddToRoleAsync(student2, "Student");
        await userManager.AddToRoleAsync(student3, "Student");
        await userManager.AddToRoleAsync(student4, "Student");
        await userManager.AddToRoleAsync(student5, "Student");

        await dbContext.SaveChangesAsync();
        dbContext.StudentCards.AddRange(
            new StudentCard(
                userId: student1.Id,
                personalNumber: "123456789",
                firstName: "Jan",
                lastName: "Vermeulen",
                birthDate: new DateTime(2002, 5, 15),
                expirationDate: DateTime.UtcNow.AddYears(1),
                profilePicture: "https://api.dicebear.com/7.x/avataaars/svg?seed=Jan_Vermeulen"
            ),
            new StudentCard(
                userId: student2.Id,
                personalNumber: "987654321",
                firstName: "Marie",
                lastName: "Dubois",
                birthDate: new DateTime(2003, 8, 22),
                expirationDate: DateTime.UtcNow.AddYears(1),
                profilePicture: "https://api.dicebear.com/7.x/avataaars/svg?seed=Marie_Dubois"
            ),
            new StudentCard(
                userId: student3.Id,
                personalNumber: "456123789",
                firstName: "Pieter",
                lastName: "Janssens",
                birthDate: new DateTime(2001, 11, 30),
                expirationDate: DateTime.UtcNow.AddYears(1),
                profilePicture: "https://api.dicebear.com/7.x/avataaars/svg?seed=Pieter_Janssens"
            ),
            new StudentCard(
                userId: student4.Id,
                personalNumber: "321654987",
                firstName: "Sophie",
                lastName: "Nguyen",
                birthDate: new DateTime(2002, 3, 7),
                expirationDate: DateTime.UtcNow.AddYears(1),
                profilePicture: "https://api.dicebear.com/7.x/avataaars/svg?seed=Sophie_Nguyen"
            ),
            new StudentCard(
                userId: student5.Id,
                personalNumber: "789456123",
                firstName: "Thomas",
                lastName: "Maes",
                birthDate: new DateTime(2003, 1, 18),
                expirationDate: DateTime.UtcNow.AddYears(1),
                profilePicture: "https://api.dicebear.com/7.x/avataaars/svg?seed=Thomas_Maes"
            )
        );

        await dbContext.SaveChangesAsync();
    }

    private async Task NewsAsync()
    {
        if (dbContext.NewsArticles.Any())
            return;
        dbContext.NewsArticles.AddRange(
            new NewsArticle { Author = "Isabelle Claes", PublishDate = DateTime.Parse("2025-11-25T09:00:00Z"), Title = "VVS over besparing hoger onderwijs", Description = "De Vlaamse regering wil 75 miljoen euro besparen in 2026 op het hoger onderwijs", Type = "Wallie", Content = "De Vlaamse regering wil 75 miljoen euro besparen in 2026 op het hoger onderwijs. Dit gaat gebeuren door de toegang tot de beurzen te verstrengen, de Brusselmiddelen te schrappen en de subsidies voor niet-EER studenten drastisch te verlagen. De besparingen zouden, volgens recente schattingen, leiden tot het verlies van een studiebeurs voor 20.000 studenten. Deze toelage wordt geschrapt voor studenten ouder dan 30 of die minder dan 54 studiepunten (ECTS) opnemen. \n\nOndanks de reeds voorziene uitzonderingen blijft de Vlaamse Vereniging van Studenten zich zorgen maken.", ImageUrl = "https://res.cloudinary.com/drqwrfucv/image/upload/v1764750146/content_oosv53.jpg" },
            new NewsArticle { Author = "Melanie Bytebier", PublishDate = DateTime.Parse("2025-11-28T10:30:00Z"), Title = "Examenrooster en verplaatsen examen", Description = "De examenroosters voor semester 1 zijn gepubliceerd.", Type = "Onderwijs", Content = "De examenroosters voor semester 1 zijn gepubliceerd. Je vindt jouw persoonlijk examenrooster terug via TimeEdit. \n\n \n\nStudenten met een individueel traject en overlappende examens of IOEM-studenten kunnen een inhaalexamen aanvragen.\n\nDe deadline is zondag 7/12/2025. ", ImageUrl = "https://res.cloudinary.com/drqwrfucv/image/upload/v1764750119/thumbnail_vryshz.jpg" },
            new NewsArticle { Author = "Pol Bracke", PublishDate = DateTime.Parse("2025-11-27T09:00:00Z"), Title = "Studenten Groenmanagement versterken biodivers groen op campus.", Description = "Studenten Groenmanagement staken opnieuw de handen uit de mouwen tijdens een (ver)plantactie op de campus. Deze oefening binnen hun opleiding is tegelijkertijd een waardevolle samenwerking die onze campussen biodiverser én onderhoudsvriendelijker maakt.", Type = "Wallie", ImageUrl = "https://res.cloudinary.com/drqwrfucv/image/upload/v1764750631/content_eu5s8g.jpg", Content = "**Studenten Groenmanagement staken opnieuw de handen uit de mouwen tijdens een (ver)plantactie op de campus. Deze oefening binnen hun opleiding is tegelijkertijd een waardevolle samenwerking die onze campussen biodiverser én onderhoudsvriendelijker maakt.**\n\nDe bloemenrijke boord langs de sporthal was verschillende jaren een kleurrijke blikvanger. Maar het intensieve onderhoud maakte duidelijk dat we moesten zoeken naar een duurzamere aanpak in lijn met de visie op **multifunctioneel groen**.\n\nDat houdt in dat het groen op de campus verschillende functies krijgt:\n\n*   **Beleving en welzijn**: ruimte om te wandelen, sporten, ontspannen en elkaar te ontmoeten.\n    \n*   **Klimaatvriendelijke inrichting**: van wadi’s die regenwater langzaam laten infiltreren tot schaduwplekken die verkoeling geven op warme dagen.\n    \n*   **Biodiversiteit**: meer leefruimte creëren voor planten, dieren en insecten.\n    \n\n**Leren door te doen**\n\nOnder begeleiding van lectoren van de opleiding groenmanagement en het team groenbeheer selecteerden de studenten de waardevolle planten uit de boord rond de sporthal en verplantten die naar de pergola naast de sporthal en naast de wadi aan gebouw B  waar ze een nieuwe stek krijgen. Waar de plantenboord zich bevond aan de sporthal wordt gazon ingezaaid.\n\nDeze hands-on opdracht kadert binnen de opleiding Groenmanagement, maar leverde meteen ook een mooie bijdrage aan het campusgroen. De studenten leerden niet alleen over de verschillende plantensoorten, standplaats en ecologisch beheer, ze zagen ook hun  leer- en werkomgeving biodiverser  en mooier worden. Alle plantensoorten zijn inheemse of neo-inheemse soorten die tal van inheemse insectensoorten kunnen ondersteunen.  \n\nTot slot plantten de studenten ook de resterende bloembollen van de bloembollenactie aan rond gebouw C.\n\n**Een bloemrijke campus**\n\nWie volgend voorjaar over de campus wandelt, zal het resultaat niet kunnen negeren: het wordt een biodiverse en kleurige bloemrijke omgeving. Aantrekkelijk voor zowel mensen als vlinders, wilde bijen en tal van andere insecten.  \n\nDe inspanningen van de studenten en personeelsleden die meebouwden aan dit stukje biodivers groen, kunnen alvast op appreciatie rekenen. Het initiatief is een fraaie illustratie van hoe leren, samenwerken en duurzaam campusbeheer hand in hand kunnen gaan." },
            new NewsArticle { Author = "Flor Coussement", PublishDate = DateTime.Parse("2025-11-05T09:00:00Z"), Title = "Loana Boulanger, de nieuwe voorzitter van Revolte aan het woord.", Description = "Loana Boulanger zit in haar tweede bachelor Organisatie en management, richting Business & Languages en is de nieuwe voorzitter van Revolte. Dat ze die rol op zich nam, was niet vanzelfsprekend. 'Eerlijk? We hebben dit jaar een hobbelig parcours afgelegd om iemand te vinden. De knip gooide roet in het eten.'", Type = "Wallie", ImageUrl = "https://res.cloudinary.com/drqwrfucv/image/upload/v1764750202/Untitled_imlmzd.webp", Content = " **Loana Boulanger zit in haar tweede bachelor Organisatie en management, richting Business & Languages en is de nieuwe voorzitter van Revolte. Dat ze die rol op zich nam, was niet vanzelfsprekend. “Eerlijk? We hebben dit jaar een hobbelig parcours afgelegd om iemand te vinden. De knip gooide roet in het eten.”**\n\n\n### **De knip: een kleine regel met grote gevolgen**\n\nSinds dit academiejaar geldt in het hoger onderwijs de zogenaamde _harde knip_: studenten mogen pas aan het derde jaar van hun bachelor beginnen als ze álle vakken van het eerste jaar hebben afgelegd. “Bij ons in Revolte heeft de knip echt impact gehad,” vertelt Loana. “Enkele sterke kandidaten voor het voorzitterschap konden hun mandaat niet verderzetten omdat ze van opleiding moesten veranderen of even vastzaten door de knip. We hebben zelfs een tijd zonder secretaris gezeten. Daardoor liep alles wat stroever en was het moeilijker om continuïteit te bewaren.”\n\nDe gevolgen reiken verder dan Revolte. Ook studentenverenigingen kampen met een dalend aantal actieve leden. “Het engagement ligt overal lager. Veel studenten twijfelen of ze het wel gaan halen met die knip en stellen extra engagement daarom uit. Dat is jammer, want net door actief te zijn in een studentenraad of in een studentenclub, groei je als persoon.”\n\n### **“Ik ben van nature onzeker”**\n\nToen Loana, toen nog ondervoorzitter, de kans kreeg om het voorzitterschap over te nemen, twijfelde ze even. “Ik ben van nature eerder onzeker. Maar toen ik door directie Communicatie werd gevraagd om te spreken op de feestelijke opening van het academiejaar, dacht ik: “Als ik dat podium aankan, kan ik ook de stem van de studenten zijn.” Die ervaring heeft haar een enorme boost gegeven.\n\n### **Een teamspeler met een missie**\n\n\nVoor Loana draait leiderschap niet om haar, maar om het geheel. “Iedereen binnen Revolte bouwt aan het huis. Ik kan rekenen op een fantastische ondervoorzitter, Mohamed Chater, en onze nauwgezette secretaris Maja Van Renterghem. Mohamed wijst me op kleine foutjes of redeneringen, Maja zorgt dat alles strak georganiseerd blijft. En onze participatiecoaches Flor, Jozefien en Robin zijn onmisbaar.”\n\nWat voor haar telt: open communicatie, respect en ruimte voor feedback. “We hoeven het niet altijd eens te zijn, zolang we elkaar maar respectvol benaderen. Dat maakt ons als team sterker.”\n\n#### **Stokpaardje: meer studenten aan boord**\n\nLoana wil vooral studenten enthousiasmeren om zich te engageren. “Binnen de departementen lukt dat nog goed, maar op hogeschoolniveau blijft de drempel hoog. In raden zoals onderwijs, duurzaamheid of AUGent zijn nog te veel lege stoelen. We moeten milder zijn voor gemotiveerde studenten, ook als hun traject niet perfect is. We zijn allemaal nog aan het leren.”\n\n#### **Inspiratie van haar voorgangers**\n\nLoana laat zich inspireren door haar voorgangers Kiara en Arne. “Hun aanpak was top. Ze zijn er nog steeds voor advies. Met Kiara ga ik af en toe nog iets drinken, en Arne zag ik nog op de HOGENT Cup. Fijn om te weten dat ik op hen kan terugvallen.”\n\nOndanks de hindernissen bij de opstart blijft ze optimistisch. “De knip heeft ons even doen wankelen, maar tegelijk heeft het ons dichter bij elkaar gebracht. Revolte is geen eenmanswerk, het is teamwork en daar geloof ik keihard in.”" },
            new NewsArticle { Author = "Katrien Demeestere", PublishDate = DateTime.Parse("2025-10-15T09:00:00Z"), Title = "De waterstofmobiel komt naar HOGENT", Type = "Wallie", ImageUrl = "https://res.cloudinary.com/drqwrfucv/image/upload/v1764750315/Untitled_vq3ux3.webp", Content = "**🌊🚐 De Waterstofmobiel komt naar HOGENT!**\n\nOntdek de toekomst van duurzame energie!Op **woensdag 22 oktober vanaf 10 uur** strijkt de **Festo Waterstofmobiel** neer op de parking tussen **gebouw P en B op** campus Schoonmeersen.\n\nStap binnen in deze rijdende demoruimte en ontdek hoe **automatisering en waterstoftechnologie** hand in hand gaan in de energietransitie.In slechts één uur maak je kennis met:\n\n*   de werking van elektrolysers en waterstoftankstations,\n    \n*   innovatieve opslag- en distributiesystemen,\n    \n*   veilige en efficiënte automatiseringsoplossingen voor de waterstofeconomie.\n    \n\n💡 **Voor wie?**Alle studenten en lesgevers met interesse in techniek, energie en innovatie.Een unieke kans om technologie van morgen vandaag al te beleven!\n\n📅 **Wanneer:** woensdag 22 oktober, vanaf 10 uur \n\n📍 **Waar:** parking tussen gebouw P en B, campus Schoonmmeersen\n\nKom langs, stap in en laat je inspireren door **H₂ in motion**!" },
            new NewsArticle { Author = "Jozefien Willems", PublishDate = DateTime.Parse("2025-10-29T09:00:00Z"), Title = "Wintermarkt 2026", Type = "Onderwijs", ImageUrl = "https://res.cloudinary.com/drqwrfucv/image/upload/v1764750424/content_lj8cbr.jpg", Content = "Beste HOGENTenaars,\n\nDe winter komt eraan, en dat betekent dat onze jaarlijkse **Wintermarkt** van revolte er opnieuw aankomt.\n\n**Wanneer:** donderdag 11 december 2025\n**Tijd:** 10u tot 16u\n**Locatie:** Foyer B, campus Schoonmeersen\n\nZoals elk jaar gaat de opbrengst naar **De Warmste Week**.\n\nWil jij graag mee een standje uitbaten? Alleen, met collega’s, met je dienst/directie of met je studentenvereniging? Schrijf je dan in, Inschrijven kan tot **23 november**.\n\nLet op: bij dubbele ideeën krijgt het eerste team dat zich aanmeldt voorrang.\n\nVanuit **Revolte** zorgen we alvast voor sfeer in ons eigen kraampje met pannenkoeken, jenever, glühwein en warme chocolademelk. Iedereen is welkom om iets aan te bieden: van zelfgemaakte producten en lekkernijen tot creatieve kunstwerken. \n\nSamen maken we er een warme en gezellige editie van en steunen we de warmste week! \n\nVragen of opmerkingen? Mail gerust naar **stuvo.revolte@hogent.be**" },
            new NewsArticle {Title = "Nooit meer honger op de campus.", PublishDate = DateTime.Parse("2025-11-20T09:00:00Z"), Author = "Pol Bracke", Type = "Wallie", Content = "Als student kan je wel wat energie en vitaminen gebruiken want studeren met een knorrende maag lukt echt niet. Op elke campus, of in de nabijheid van jouw campus, vind je een studentenrestaurant of smartfridge terug waar je terecht kan voor een snelle hap, een uitgebreide lunch, een drankje of een heerlijk dessert.\n\nWist je dat STUVO jouw soep, warme maaltijden en salades subsidieert, zodat het allemaal ook financieel behapbaar blijft. Hou je dus van lekkere verzorgde maaltijden tegen studentvriendelijke prijzen? Check dan [hier](https://www.hogent.be/student/catering/) waar je de HOGENT-studentenrestaurants kan vinden.\n\nMet je HOGENT-studentenkaart ben je trouwens ook welkom in de studentenrestaurants van UGent. Handig toch?\n\nLaat het smaken!", ImageUrl = "https://res.cloudinary.com/drqwrfucv/image/upload/v1764754337/content_mtunbt.jpg"}
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

    private async Task WidgetsAsync()
    {
        if (dbContext.Widgets.Any())
            return;
        var widgets = new List<Widget>
        {
            new Widget()
            {
                TypeName = "news",
            },
            new Widget()
            {
                TypeName = "menus",
            },
            new Widget()
            {
                TypeName = "schedule",
            },
            new Widget()
            {
                TypeName = "grades",
            },
            new Widget()
            {
                TypeName = "links",
            }
        };
        dbContext.Widgets.AddRange(widgets);
        await dbContext.SaveChangesAsync();
    }

    private async Task UserWidgetsAsync()
    {
        if (dbContext.UserWidgets.Any())
            return;

        // find users
        var student1 = await dbContext.Users.FirstOrDefaultAsync(u => u.Email == "jan.vermeulen@student.hogent.be");
        var student2 = await dbContext.Users.FirstOrDefaultAsync(u => u.Email == "marie.dubois@student.hogent.be");
        var student3 = await dbContext.Users.FirstOrDefaultAsync(u => u.Email == "pieter.janssens@student.hogent.be");

        // find widgets by type (created in WidgetsAsync)
        var newsWidget = await dbContext.Widgets.FirstOrDefaultAsync(w => w.TypeName == "news");
        var menusWidget = await dbContext.Widgets.FirstOrDefaultAsync(w => w.TypeName == "menus");
        var scheduleWidget = await dbContext.Widgets.FirstOrDefaultAsync(w => w.TypeName == "schedule");
        var gradesWidget = await dbContext.Widgets.FirstOrDefaultAsync(w => w.TypeName == "grades");
        var linksWidget = await dbContext.Widgets.FirstOrDefaultAsync(w => w.TypeName == "links");

        if (student1 == null || student2 == null || student3 == null ||
            newsWidget == null || menusWidget == null || scheduleWidget == null || gradesWidget == null || linksWidget == null)
        {
            // missing prerequisites; skip seeding user widgets
            return;
        }

        dbContext.UserWidgets.AddRange(
            // student1: news + grades
            new UserWidget { Widget = newsWidget, UserId = student1.Id, X = 0, Y = 0, Width = 12, Height = 6, MinWidth = 4 },
            new UserWidget { Widget = gradesWidget, UserId = student1.Id, X = 0, Y = 6, Width = 12, Height = 6, MinWidth = 4 },

            // student2: menus + schedule
            new UserWidget { Widget = menusWidget, UserId = student2.Id, X = 0, Y = 0, Width = 12, Height = 6, MinWidth = 4 },
            new UserWidget { Widget = scheduleWidget, UserId = student2.Id, X = 0, Y = 6, Width = 12, Height = 6, MinWidth = 4 },

            // student3: all widgets (example)
            new UserWidget { Widget = newsWidget, UserId = student3.Id, X = 0, Y = 0, Width = 12, Height = 6, MinWidth = 4 },
            new UserWidget { Widget = menusWidget, UserId = student3.Id, X = 0, Y = 6, Width = 12, Height = 6, MinWidth = 4 },
            new UserWidget { Widget = scheduleWidget, UserId = student3.Id, X = 0, Y = 12, Width = 12, Height = 6, MinWidth = 4 },
            new UserWidget { Widget = gradesWidget, UserId = student3.Id, X = 0, Y = 18, Width = 12, Height = 6, MinWidth = 4 }
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
                (new Building { Name = "Gebouw B", BuildingCode = "GSCHB", Address = "Valentin Vaerwyckweg 1, 9000 Gent", Type = "building", Latitude = 51.03138465268992, Longitude = 3.701414635630698, CampusId = schoonmeersen.Id}, schoonmeersen.Id),
                (new Building { Name = "Gebouw C", BuildingCode = "GSCHC", Address = "Valentin Vaerwyckweg 1, 9000 Gent", Type = "building", Latitude = 51.03195215277462, Longitude = 3.704568306783877, CampusId = schoonmeersen.Id }, schoonmeersen.Id),
                (new Building { Name = "Gebouw D", BuildingCode = "GSCHD", Address = "Valentin Vaerwyckweg 1, 9000 Gent", Type = "building", Latitude = 51.031511099994304, Longitude = 3.702789535635411, CampusId = schoonmeersen.Id }, schoonmeersen.Id),
                (new Building { Name = "Gebouw E", BuildingCode = "GSCHE", Address = "Valentin Vaerwyckweg 1, 9000 Gent", Type = "building", Latitude = 51.0310611550396, Longitude = 3.7045170251451722, CampusId = schoonmeersen.Id }, schoonmeersen.Id),
                (new Building { Name = "Gebouw P", BuildingCode = "GSCHP", Address = "Valentin Vaerwyckweg 1, 9000 Gent", Type = "building", Latitude = 51.03423555310173, Longitude = 3.7019467933058716, CampusId = schoonmeersen.Id }, schoonmeersen.Id),
                (new Building { Name = "Sporthal", BuildingCode = "GSCHS", Address = "Sint-Denijslaan 251, 9000 Gent", Type = "sport", Latitude = 51.03493515601912, Longitude = 3.704163329549105, CampusId = schoonmeersen.Id }, schoonmeersen.Id),
                (new Building { Name = "Gebouw T", BuildingCode = "GSCHT", Address = "Voskenslaan 364A, 9000 Gent", Type = "building", Latitude = 51.028515604065866, Longitude = 3.70666265806964, CampusId = schoonmeersen.Id }, schoonmeersen.Id)
            });
        }

        if (mercator != null)
        {
            allBuildings.AddRange(new[]
            {
                (new Building { Name = "Gebouw C", BuildingCode = "GMRCC",Address = "Nonnemeersstraat 19-21, 9000 Gent", Type = "building", Latitude = 51.04365987772189, Longitude = 3.7133038554040447, CampusId = mercator.Id}, mercator.Id),
                (new Building { Name = "Gebouw D", BuildingCode = "GMRCD", Address = "Nonnemeersstraat 15-17, 9000 Gent", Type = "building", Latitude = 51.04409655468585, Longitude = 3.7139953687425247, CampusId = mercator.Id }, mercator.Id),
                (new Building { Name = "Gebouw E", BuildingCode = "GMRCE", Address = "Nonnemeersstraat 24, 9000 Gent", Type = "building", Latitude = 51.044138254547896, Longitude = 3.7140100810412755, CampusId = mercator.Id }, mercator.Id),
                (new Building { Name = "Gebouw G", BuildingCode = "GMRCG", Address = "Henleykaai 84, 9000 Gent", Type = "building", Latitude = 51.04198292611773, Longitude = 3.715517179744473, CampusId = mercator.Id }, mercator.Id)
            });
        }

        if (bijloke != null)
        {
            allBuildings.AddRange(new[]
            {
                (new Building { Name = "Pauli", BuildingCode = "GBPAU",Address = "J. Kluyskensstraat 2, 9000 Gent", Type = "building", Latitude = 51.04559751827652, Longitude = 3.7185065415747798, CampusId = bijloke.Id }, bijloke.Id),
                (new Building { Name = "Cloquet",BuildingCode = "GBCLO",Address = "Pasteurlaan 2, 9000 Gent", Type = "building", Latitude = 51.0452537498002, Longitude = 3.715110343694497, CampusId = bijloke.Id }, bijloke.Id),
                (new Building { Name = "Marissal",BuildingCode = "GBMAR",Address = "Pasteurlaan 2, 9000 Gent", Type = "building", Latitude = 51.0452537498002, Longitude = 3.715110343694497, CampusId = bijloke.Id }, bijloke.Id),
                (new Building { Name = "Bijlokekaai",BuildingCode = "GBKAA",Address = "Bijlokekaai 5, 9000 Gent", Type = "building", Latitude = 51.04371555532695, Longitude = 3.7193240387340807, CampusId = bijloke.Id }, bijloke.Id),
                (new Building { Name = "Kunstenbibliotheek Huis van de Abdis",BuildingCode = "GBKUB",Address = "Godshuizenlaan 2, 9000 Gent", Type = "library", Latitude = 51.04393486454849, Longitude = 3.717493005440041, CampusId = bijloke.Id }, bijloke.Id)
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
            new Contact { Type = "Organisatie", Name = "HOGENT", PhoneNumber = "09 243 33 33", Email = "info@hogent.be" },
            new Contact { Type = "Departement", Name = "Bedrijf en Organisatie", ContactPerson = "Rudi Madalijns", Email = "Rudi.Madalijns@hogent.be" },
            new Contact { Type = "Departement", Name = "IT en Digitale Innovatie", ContactPerson = "Chantal Teerlinck", Email = "Chantal.Teerlinck@hogent.be" },
            new Contact { Type = "Campus", Name = "Campus Schoonmeersen", PhoneNumber = "09 243 20 04" },
            new Contact { Type = "Campus", Name = "Campus Mercator", PhoneNumber = "09 243 20 16" },
            new Contact { Type = "Directie", Name = "Algemene directie", ContactPerson = "Koen Goethals", Email = "koen.goethals@hogent.be" }
        );

        await dbContext.SaveChangesAsync();
    }


    private async Task EventsAsync()
    {
        if (dbContext.Events.Any())
            return;

        dbContext.Events.AddRange(
            new Event
            {
                Title = "Social Run & Brunch",
                StartDateTime = DateTime.Parse("2025-11-19 09:30"),
                EndDateTime = DateTime.Parse("2025-11-19 11:30"),
                Location = "Campus Schoonmeersen, Valentin Vaerwyckweg 1, 9000 Gent",
                RegistrationLink = "https://events.hogent.be/social-run-brunch",
                Type = "Andere",
                Description = "Sluit je aan voor een gezellige social run. Na de run genieten we van een heerlijke brunch. Iedereen welkom. (EN: Join our social run — run at your pace and enjoy brunch afterwards.)"
            },
            new Event
            {
                Title = "Training Omstaander Grensoverschrijdend Gedrag",
                StartDateTime = DateTime.Parse("2025-11-19 18:00"),
                EndDateTime = DateTime.Parse("2025-11-19 20:00"),
                Location = "Campus St.-Niklaas, Grote Markt 1, 9100 Sint-Niklaas",
                RegistrationLink = "https://events.hogent.be/omstaander-gg",
                Type = "Andere",
                Description = "Een interactieve training waarin studenten leren hoe ze grensoverschrijdend gedrag herkennen en hoe ze als omstaander veilig kunnen ingrijpen."
            },
            new Event
            {
                Title = "Workshop Salsa Cubana",
                StartDateTime = DateTime.Parse("2025-11-20 14:30"),
                EndDateTime = DateTime.Parse("2025-11-20 16:00"),
                Location = "Danszaal 2, sporthal HOGENT, Valentin Vaerwyckweg 1, 9000 Gent, België",
                RegistrationLink = "https://events.hogent.be/salsa-cubana",
                Type = "Welzijn",
                Description = "Onderdeel van 'Goed in je vel'. Een energieke en ontspannende workshop. Iedereen welkom."
            },
            new Event
            {
                Title = "Workshop Bachata Moderna",
                StartDateTime = DateTime.Parse("2025-11-20 16:00"),
                EndDateTime = DateTime.Parse("2025-11-20 17:30"),
                Location = "Danszaal 2, sporthal HOGENT, Valentin Vaerwyckweg 1, 9000 Gent, België",
                RegistrationLink = "https://events.hogent.be/bachata-moderna",
                Type = "Welzijn",
                Description = "Onderdeel van 'Goed in je vel'. Leer moderne bachata-moves in een warme sfeer."
            },
            new Event
            {
                Title = "Workshop Zelfverdediging",
                StartDateTime = DateTime.Parse("2025-11-20 19:00"),
                EndDateTime = DateTime.Parse("2025-11-20 21:00"),
                Location = "Sporthal, Watersportlaan 4, 9000 Gent, België",
                RegistrationLink = "https://events.hogent.be/zelfverdediging",
                Type = "Welzijn",
                Description = "Praktische training waarin je technieken leert om je zelfvertrouwen en fysieke weerbaarheid te versterken."
            },
            new Event
            {
                Title = "Live met Doorbreekbaar: stress, zelfzorg, moodboosters",
                StartDateTime = DateTime.Parse("2025-11-21 12:00"),
                EndDateTime = DateTime.Parse("2025-11-21 13:00"),
                Location = "ONLINE",
                RegistrationLink = "https://events.hogent.be/doorbreekbaar-live-online",
                Type = "Welzijn",
                Description = "Inspirerende live sessie rond mentaal welzijn met praktische tips en tools."
            },
            new Event
            {
                Title = "Sportquiz",
                StartDateTime = DateTime.Parse("2026-02-23 19:00"),
                EndDateTime = DateTime.Parse("2026-02-23 21:00"),
                Location = "Campus Aalst, Arbeidstraat 14, 9300 Aalst",
                RegistrationLink = "https://events.hogent.be/sportquiz",
                Type = "Andere",
                Description = "Een leuke en competitieve sportquiz voor teams van studenten."
            },
            new Event
            {
                Title = "Dodgeball Tornooi",
                StartDateTime = DateTime.Parse("2026-03-23 15:00"),
                EndDateTime = DateTime.Parse("2026-03-23 17:00"),
                Location = "Campus Schoonmeersen, Sporthal, Valentin Vaerwyckweg 1, 9000 Gent",
                RegistrationLink = "https://events.hogent.be/dodgeball-tornooi",
                Type = "Andere",
                Description = "Een actief en spannend dodgeballtoernooi voor alle studenten."
            },
            new Event
            {
                Title = "Introvert in een extraverte wereld",
                StartDateTime = DateTime.Parse("2025-11-24 15:30"),
                EndDateTime = DateTime.Parse("2025-11-24 17:00"),
                Location = "Lange Steenstraat 16-18, 9000 Gent, België",
                RegistrationLink = "https://events.hogent.be/introvert-extraverte-wereld",
                Type = "Welzijn",
                Description = "Workshop die introverte studenten ondersteunt in het omgaan met sociale verwachtingen."
            },
            new Event
            {
                Title = "Zelfliefde",
                StartDateTime = DateTime.Parse("2025-12-15 09:30"),
                EndDateTime = DateTime.Parse("2025-12-15 11:00"),
                Location = "Lange Steenstraat 16-18, 9000 Gent, Belgium",
                RegistrationLink = "https://events.hogent.be/zelfliefde",
                Type = "Welzijn",
                Description = "Een warme sessie die focust op zelfwaarde, mildheid en persoonlijke groei."
            },
            new Event
            {
                Title = "Hoe weet ik wat ik écht wil?",
                StartDateTime = DateTime.Parse("2025-12-15 12:30"),
                EndDateTime = DateTime.Parse("2025-12-15 14:00"),
                Location = "Lange Steenstraat 16-18, 9000 Gent, Belgium",
                RegistrationLink = "https://events.hogent.be/wat-ik-echt-wil",
                Type = "Welzijn",
                Description = "Workshop die inzicht biedt in motivatie, keuzes en richting vinden."
            },
            new Event
            {
                Title = "Zelfzorg tijdens de examens",
                StartDateTime = DateTime.Parse("2025-12-15 19:00"),
                EndDateTime = DateTime.Parse("2025-12-15 20:30"),
                Location = "Lange Steenstraat 16-18, 9000 Gent, Belgium",
                RegistrationLink = "https://events.hogent.be/zelfzorg-examens",
                Type = "Welzijn",
                Description = "Praktische begeleiding voor gezonde stressreductie tijdens de examenperiode."
            },
            new Event
            {
                Title = "Wake-upcall: verhoog je efficiëntie, je levensenthousiasme en je kracht!",
                StartDateTime = DateTime.Parse("2025-12-18 10:30"),
                EndDateTime = DateTime.Parse("2025-12-18 11:30"),
                Location = "ONLINE",
                RegistrationLink = "https://events.hogent.be/wake-upcall-online",
                Type = "Welzijn",
                Description = "Online sessie die je helpt je energie, motivatie en efficiëntie te verhogen."
            }
        );

        await dbContext.SaveChangesAsync();
    }
    private async Task GradesAsync()
    {
        if (dbContext.Grades.Any())
            return;
        var student1 = await dbContext.Users.FirstOrDefaultAsync(u => u.Email == "jan.vermeulen@student.hogent.be");
        var student2 = await dbContext.Users.FirstOrDefaultAsync(u => u.Email == "marie.dubois@student.hogent.be");
        var student3 = await dbContext.Users.FirstOrDefaultAsync(u => u.Email == "pieter.janssens@student.hogent.be");

        if (student1 == null || student2 == null || student3 == null)
        {
            // missing prerequisites; skip seeding user widgets
            return;
        }

        dbContext.Grades.AddRange(
            // Existing grades with userId added
            new Grade
            {
                CourseId = "C30542",
                CourseName = "Web Development 2",
                Year = "2024-2025",
                Semester = 1,
                Name = "Project 1 - Portfolio Website",
                ActivityType = "Assignment",
                MaxPoints = 20,
                Score = 17,
                Feedback = "Excellent structure and clean styling. Minor accessibility issues with ARIA labels.",
                SubmissionDate = DateTime.Parse("2025-03-18T10:25:00Z"),
                Date = DateTime.Parse("2025-03-17T23:59:00Z"),
                UserId = student1.Id
            },
            new Grade
            {
                CourseId = "C30542",
                CourseName = "Web Development 1",
                Year = "2024-2025",
                Semester = 1,
                Name = "Quiz 2 - JavaScript Concepts",
                ActivityType = "Quiz",
                MaxPoints = 10,
                Score = 8,
                Feedback = "Solid understanding of closures; review promises syntax.",
                SubmissionDate = DateTime.Parse("2025-04-02T09:40:00Z"),
                Date = DateTime.Parse("2025-04-01T23:59:00Z"),
                UserId = student1.Id
            },
            new Grade
            {
                CourseId = "C30549",
                CourseName = "Databases",
                Year = "2024-2025",
                Semester = 1,
                Name = "Normalization Assignment",
                ActivityType = "Assignment",
                MaxPoints = 20,
                Score = 14,
                Feedback = "Good normalization work, but ensure all tables have proper keys.",
                SubmissionDate = DateTime.Parse("2025-02-12T15:10:00Z"),
                Date = DateTime.Parse("2025-02-11T23:59:00Z"),
                UserId = student2.Id
            },
            
            new Grade
            {
                CourseId = "C30101",
                CourseName = "Cybersecurity",
                Year = "2024-2025",
                Semester = 1,
                Name = "Midterm Exam",
                ActivityType = "Exam",
                MaxPoints = 20,
                Score = 16,
                Feedback = "Strong understanding of cryptographic principles.",
                SubmissionDate = DateTime.Parse("2024-11-15T14:30:00Z"),
                Date = DateTime.Parse("2024-11-15T14:00:00Z"),
                UserId = student1.Id
            },
            new Grade
            {
                CourseId = "C30102",
                CourseName = "Databases",
                Year = "2024-2025",
                Semester = 1,
                Name = "SQL Assignment",
                ActivityType = "Assignment",
                MaxPoints = 20,
                Score = 18,
                Feedback = "Excellent query optimization.",
                SubmissionDate = DateTime.Parse("2024-10-20T16:00:00Z"),
                Date = DateTime.Parse("2024-10-20T23:59:00Z"),
                UserId = student1.Id
            },
            new Grade
            {
                CourseId = "C30103",
                CourseName = "IT Fundamentals",
                Year = "2024-2025",
                Semester = 1,
                Name = "Hardware Quiz",
                ActivityType = "Quiz",
                MaxPoints = 10,
                Score = 9,
                Feedback = "Very good understanding of computer architecture.",
                SubmissionDate = DateTime.Parse("2024-09-25T10:15:00Z"),
                Date = DateTime.Parse("2024-09-25T10:00:00Z"),
                UserId = student1.Id
            },
            new Grade
            {
                CourseId = "C30104",
                CourseName = "Object-oriented Software Development I",
                Year = "2024-2025",
                Semester = 1,
                Name = "OOP Project",
                ActivityType = "Project",
                MaxPoints = 20,
                Score = 17,
                Feedback = "Good use of inheritance and polymorphism. Consider improving encapsulation.",
                SubmissionDate = DateTime.Parse("2024-12-01T18:00:00Z"),
                Date = DateTime.Parse("2024-12-01T23:59:00Z"),
                UserId = student1.Id
            },
            new Grade
            {
                CourseId = "C30105",
                CourseName = "Software Analysis",
                Year = "2024-2025",
                Semester = 1,
                Name = "UML Diagrams",
                ActivityType = "Assignment",
                MaxPoints = 20,
                Score = 15,
                Feedback = "Good class diagrams, but sequence diagrams need more detail.",
                SubmissionDate = DateTime.Parse("2024-11-10T12:00:00Z"),
                Date = DateTime.Parse("2024-11-10T23:59:00Z"),
                UserId = student1.Id
            },
            new Grade
            {
                CourseId = "C30106",
                CourseName = "Web Development I",
                Year = "2024-2025",
                Semester = 1,
                Name = "HTML/CSS Project",
                ActivityType = "Project",
                MaxPoints = 20,
                Score = 19,
                Feedback = "Excellent responsive design and clean code.",
                SubmissionDate = DateTime.Parse("2024-10-15T20:00:00Z"),
                Date = DateTime.Parse("2024-10-15T23:59:00Z"),
                UserId = student1.Id
            },
            
            new Grade
            {
                CourseId = "C30201",
                CourseName = "Communication Lab",
                Year = "2024-2025",
                Semester = 2,
                Name = "Presentation Skills",
                ActivityType = "Presentation",
                MaxPoints = 20,
                Score = 17,
                Feedback = "Clear communication and good structure. Work on eye contact.",
                SubmissionDate = DateTime.Parse("2025-03-10T11:00:00Z"),
                Date = DateTime.Parse("2025-03-10T11:00:00Z"),
                UserId = student2.Id
            },
            new Grade
            {
                CourseId = "C30202",
                CourseName = "Business & Management",
                Year = "2024-2025",
                Semester = 2,
                Name = "Business Plan",
                ActivityType = "Assignment",
                MaxPoints = 20,
                Score = 16,
                Feedback = "Good market analysis. Financial projections need more detail.",
                SubmissionDate = DateTime.Parse("2025-04-05T15:00:00Z"),
                Date = DateTime.Parse("2025-04-05T23:59:00Z"),
                UserId = student2.Id
            },
            new Grade
            {
                CourseId = "C30203",
                CourseName = "Computer Networks I",
                Year = "2024-2025",
                Semester = 2,
                Name = "Network Configuration",
                ActivityType = "Lab",
                MaxPoints = 20,
                Score = 18,
                Feedback = "Excellent subnet configuration and routing setup.",
                SubmissionDate = DateTime.Parse("2025-03-20T16:30:00Z"),
                Date = DateTime.Parse("2025-03-20T23:59:00Z"),
                UserId = student2.Id
            },
            new Grade
            {
                CourseId = "C30204",
                CourseName = "Object-oriented Software Development II",
                Year = "2024-2025",
                Semester = 2,
                Name = "Design Patterns Project",
                ActivityType = "Project",
                MaxPoints = 20,
                Score = 19,
                Feedback = "Outstanding implementation of design patterns.",
                SubmissionDate = DateTime.Parse("2025-05-15T18:00:00Z"),
                Date = DateTime.Parse("2025-05-15T23:59:00Z"),
                UserId = student2.Id
            },
            new Grade
            {
                CourseId = "C30205",
                CourseName = "Software Development Project I",
                Year = "2024-2025",
                Semester = 2,
                Name = "Team Project",
                ActivityType = "Project",
                MaxPoints = 20,
                Score = 18,
                Feedback = "Great teamwork and solid implementation. Documentation could be improved.",
                SubmissionDate = DateTime.Parse("2025-05-20T20:00:00Z"),
                Date = DateTime.Parse("2025-05-20T23:59:00Z"),
                UserId = student2.Id
            },
            new Grade
            {
                CourseId = "C30206",
                CourseName = "Web Development II",
                Year = "2024-2025",
                Semester = 2,
                Name = "React Application",
                ActivityType = "Project",
                MaxPoints = 20,
                Score = 17,
                Feedback = "Good component structure. Consider using more hooks for state management.",
                SubmissionDate = DateTime.Parse("2025-04-25T19:00:00Z"),
                Date = DateTime.Parse("2025-04-25T23:59:00Z"),
                UserId = student2.Id
            },
            
            new Grade
            {
                CourseId = "C30207",
                CourseName = "Operating Systems",
                Year = "2024-2025",
                Semester = 2,
                Name = "Process Management",
                ActivityType = "Exam",
                MaxPoints = 20,
                Score = 15,
                Feedback = "Good understanding of scheduling algorithms. Review deadlock prevention.",
                SubmissionDate = DateTime.Parse("2025-05-10T09:00:00Z"),
                Date = DateTime.Parse("2025-05-10T09:00:00Z"),
                UserId = student3.Id
            },
            new Grade
            {
                CourseId = "C30208",
                CourseName = "System Engineering Lab",
                Year = "2024-2025",
                Semester = 2,
                Name = "System Integration",
                ActivityType = "Lab",
                MaxPoints = 20,
                Score = 16,
                Feedback = "Solid system setup and configuration. Documentation is thorough.",
                SubmissionDate = DateTime.Parse("2025-04-30T14:00:00Z"),
                Date = DateTime.Parse("2025-04-30T23:59:00Z"),
                UserId = student3.Id
            },
            new Grade
            {
                CourseId = "C30101",
                CourseName = "Cybersecurity",
                Year = "2024-2025",
                Semester = 1,
                Name = "Security Audit",
                ActivityType = "Assignment",
                MaxPoints = 20,
                Score = 14,
                Feedback = "Good vulnerability assessment. Include more mitigation strategies.",
                SubmissionDate = DateTime.Parse("2024-11-20T16:00:00Z"),
                Date = DateTime.Parse("2024-11-20T23:59:00Z"),
                UserId = student3.Id
            },
            new Grade
            {
                CourseId = "C30104",
                CourseName = "Object-oriented Software Development I",
                Year = "2024-2025",
                Semester = 1,
                Name = "Java Fundamentals",
                ActivityType = "Quiz",
                MaxPoints = 10,
                Score = 8,
                Feedback = "Good grasp of OOP concepts. Practice more with interfaces.",
                SubmissionDate = DateTime.Parse("2024-10-05T11:00:00Z"),
                Date = DateTime.Parse("2024-10-05T11:00:00Z"),
                UserId = student3.Id
            }
        );

        await dbContext.SaveChangesAsync();
    }

    private async Task RestosAndMenusAsync()
    {
        if (dbContext.Restos.Any())
            return;

        // Get buildings from Campus Schoonmeersen
        var buildingD = await dbContext.Buildings.FirstOrDefaultAsync(b => b.BuildingCode == "GSCHD");
        var buildingB = await dbContext.Buildings.FirstOrDefaultAsync(b => b.BuildingCode == "GSCHB");
        var buildingP = await dbContext.Buildings.FirstOrDefaultAsync(b => b.BuildingCode == "GSCHP");

        // Get buildings from other campuses
        var buildingMercator = await dbContext.Buildings.FirstOrDefaultAsync(b => b.BuildingCode == "GMRCG");
        var buildingBijlokePauli = await dbContext.Buildings.FirstOrDefaultAsync(b => b.BuildingCode == "GBPAU");
        var buildingBijlokeCloquet = await dbContext.Buildings.FirstOrDefaultAsync(b => b.BuildingCode == "GBCLO");
        var buildingBijlokeMarissal = await dbContext.Buildings.FirstOrDefaultAsync(b => b.BuildingCode == "GBMAR");

        if (buildingD == null || buildingB == null || buildingP == null)
            return;

        var restos = new List<Resto>();

        // R1 - Resto Schoonmeersen D
        restos.Add(new Resto
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
        });

        // R2 - Resto Schoonmeersen B
        restos.Add(new Resto
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
        });

        // R3 - Resto Schoonmeersen P
        restos.Add(new Resto
        {
            Name = "Resto Schoonmeersen P",
            Description = "Studentenrestaurant in gebouw P, campus Schoonmeersen. Dagschotels, broodjes, warme dranken.",
            BuildingId = buildingP.Id,
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
            PhoneNumber = "+32 9 123 45 69",
            Email = "resto.schoonmeersen.p@hogent.be",
            ImageUrl = "https://images.hln.be/ZmQ4ZDdhNGZkMjYxMmM1Yzg0NDgvZGlvLzE3NjQ5NTMzOS9maXQtd2lkdGgvMTIwMA/in-het-studentenrestaurant-van-hogent-mag-je-maar-met-2-aan-een-tafel-van-6-zitten"
        });

        // R4 - Resto Mercator (only if building exists)
        if (buildingMercator != null)
        {
            restos.Add(new Resto
            {
                Name = "Resto Mercator",
                Description = "Bistro bij de faculteit, warme maaltijden en salades.",
                BuildingId = buildingMercator.Id,
                OpeningHours = new Dictionary<DayOfWeek, string>
                {
                    { DayOfWeek.Monday, "09:00-16:00" },
                    { DayOfWeek.Tuesday, "09:00-16:00" },
                    { DayOfWeek.Wednesday, "09:00-16:00" },
                    { DayOfWeek.Thursday, "09:00-16:00" },
                    { DayOfWeek.Friday, "09:00-14:00" }
                },
                IsCurrentlyOpen = false,
                KitchenType = new List<string> { "Hot" },
                PhoneNumber = "+32 9 76 54 321",
                Email = "resto.mercator@hogent.be",
                ImageUrl = "https://www.aldesign.be/wp-content/uploads/2016/12/browse.jpg"
            });
        }

        // R5 - Resto Ledeganck (using Bijloke Pauli as placeholder)
        if (buildingBijlokePauli != null)
        {
            restos.Add(new Resto
            {
                Name = "Resto Ledeganck",
                Description = "Snackcorner met broodjes, panini's, koffie en snelle hapjes.",
                BuildingId = buildingBijlokePauli.Id,
                OpeningHours = new Dictionary<DayOfWeek, string>
                {
                    { DayOfWeek.Monday, "08:00-19:00" },
                    { DayOfWeek.Tuesday, "08:00-19:00" },
                    { DayOfWeek.Wednesday, "08:00-19:00" },
                    { DayOfWeek.Thursday, "08:00-19:00" },
                    { DayOfWeek.Friday, "08:00-17:00" }
                },
                IsCurrentlyOpen = false,
                KitchenType = new List<string> { "FastFood" },
                PhoneNumber = "+32 9 123 45 70",
                Email = "resto.ledeganck@hogent.be",
                ImageUrl = "https://www.aldesign.be/wp-content/uploads/2016/12/browse.jpg"
            });
        }

        // R6 - Campus Resto Vesalius (using Bijloke Cloquet as placeholder)
        if (buildingBijlokeCloquet != null)
        {
            restos.Add(new Resto
            {
                Name = "Campus Resto Vesalius",
                Description = "Kantine met buffet, vegetarische en internationale gerechten.",
                BuildingId = buildingBijlokeCloquet.Id,
                OpeningHours = new Dictionary<DayOfWeek, string>
                {
                    { DayOfWeek.Monday, "07:30-18:00" },
                    { DayOfWeek.Tuesday, "07:30-18:00" },
                    { DayOfWeek.Wednesday, "07:30-18:00" },
                    { DayOfWeek.Thursday, "07:30-18:00" },
                    { DayOfWeek.Friday, "07:30-16:00" }
                },
                IsCurrentlyOpen = false,
                KitchenType = new List<string> { "Hot" },
                PhoneNumber = "+32 9 111 22 33",
                Email = "resto.vesalius@hogent.be",
                ImageUrl = "https://www.aldesign.be/wp-content/uploads/2016/12/browse.jpg"
            });
        }

        // R7 - Resto Sampori (using Bijloke Marissal as placeholder)
        if (buildingBijlokeMarissal != null)
        {
            restos.Add(new Resto
            {
                Name = "Resto Sampori",
                Description = "Gelegen naast lab/praktijklokalen; warme maaltijden en soep van de dag.",
                BuildingId = buildingBijlokeMarissal.Id,
                OpeningHours = new Dictionary<DayOfWeek, string>
                {
                    { DayOfWeek.Monday, "08:30-16:30" },
                    { DayOfWeek.Tuesday, "08:30-16:30" },
                    { DayOfWeek.Wednesday, "08:30-16:30" },
                    { DayOfWeek.Thursday, "08:30-16:30" },
                    { DayOfWeek.Friday, "08:30-15:00" }
                },
                IsCurrentlyOpen = false,
                KitchenType = new List<string> { "Hot", "Cold" },
                PhoneNumber = "+32 9 222 33 44",
                Email = "resto.sampori@hogent.be",
                ImageUrl = "https://www.aldesign.be/wp-content/uploads/2016/12/browse.jpg"
            });
        }

        dbContext.Restos.AddRange(restos);
        await dbContext.SaveChangesAsync();

        // Add menus and menu items
        if (restos.Count >= 3)
        {
            var menus = new List<Menu>();

            // Menus for R1 (Resto Schoonmeersen D) - restos[0]
            menus.Add(new Menu
            {
                RestoId = restos[0].Id,
                Date = DateTime.Parse("2025-10-20T11:30:00"),
                MenuItems = new List<MenuItem>
                {
                    new MenuItem { Name = "Tomatensoep met balletjes", Description = "Dagverse soep", Type = FoodType.Soep, PriceStudent = 1.0, PriceExtern = 2.15, IsVeggie = true, IsVegan = true },
                    new MenuItem { Name = "Varkensgebraad met mosterdsaus", Description = "Vers bereid hoofdgerecht", Type = FoodType.WarmeMaaltijd, PriceStudent = 5.3, PriceExtern = 11.7, IsVeggie = false, IsVegan = false },
                    new MenuItem { Name = "Chocomousse", Description = "Dessert van de dag", Type = FoodType.Dessert, PriceStudent = 1.0, PriceExtern = 2.15, IsVeggie = true, IsVegan = false }
                }
            });

            menus.Add(new Menu
            {
                RestoId = restos[0].Id,
                Date = DateTime.Parse("2025-10-21T11:30:00"),
                MenuItems = new List<MenuItem>
                {
                    new MenuItem { Name = "Champignonsoep", Description = "Dagverse soep", Type = FoodType.Soep, PriceStudent = 1.0, PriceExtern = 2.15, IsVeggie = true, IsVegan = true },
                    new MenuItem { Name = "Stoofvlees met frieten", Description = "Vers bereid hoofdgerecht", Type = FoodType.WarmeMaaltijd, PriceStudent = 5.3, PriceExtern = 11.7, IsVeggie = false, IsVegan = false },
                    new MenuItem { Name = "Pudding vanille", Description = "Dessert van de dag", Type = FoodType.Dessert, PriceStudent = 1.0, PriceExtern = 2.15, IsVeggie = true, IsVegan = false }
                }
            });

            menus.Add(new Menu
            {
                RestoId = restos[0].Id,
                Date = DateTime.Parse("2025-10-22T11:30:00"),
                MenuItems = new List<MenuItem>
                {
                    new MenuItem { Name = "Kervelsoep", Description = "Dagverse soep", Type = FoodType.Soep, PriceStudent = 1.0, PriceExtern = 2.15, IsVeggie = true, IsVegan = false },
                    new MenuItem { Name = "Kabeljauw met puree", Description = "Vers bereid hoofdgerecht", Type = FoodType.WarmeMaaltijd, PriceStudent = 5.3, PriceExtern = 11.7, IsVeggie = false, IsVegan = false },
                    new MenuItem { Name = "Fruitkom", Description = "Dessert van de dag", Type = FoodType.Dessert, PriceStudent = 1.0, PriceExtern = 2.15, IsVeggie = true, IsVegan = true }
                }
            });

            menus.Add(new Menu
            {
                RestoId = restos[0].Id,
                Date = DateTime.Parse("2025-10-23T11:30:00"),
                MenuItems = new List<MenuItem>
                {
                    new MenuItem { Name = "Groentesoep", Description = "Dagverse soep", Type = FoodType.Soep, PriceStudent = 1.0, PriceExtern = 2.15, IsVeggie = true, IsVegan = false },
                    new MenuItem { Name = "Stoofvlees met frieten", Description = "Vers bereid hoofdgerecht", Type = FoodType.WarmeMaaltijd, PriceStudent = 5.3, PriceExtern = 11.7, IsVeggie = false, IsVegan = false },
                    new MenuItem { Name = "Vegan muffin", Description = "Dessert van de dag", Type = FoodType.Dessert, PriceStudent = 1.0, PriceExtern = 2.15, IsVeggie = true, IsVegan = true }
                }
            });

            menus.Add(new Menu
            {
                RestoId = restos[0].Id,
                Date = DateTime.Parse("2025-10-24T11:30:00"),
                MenuItems = new List<MenuItem>
                {
                    new MenuItem { Name = "Wortelsoep", Description = "Dagverse soep", Type = FoodType.Soep, PriceStudent = 1.0, PriceExtern = 2.15, IsVeggie = true, IsVegan = true },
                    new MenuItem { Name = "Groenteburger met quinoa", Description = "Vers bereid hoofdgerecht", Type = FoodType.WarmeMaaltijd, PriceStudent = 5.3, PriceExtern = 11.7, IsVeggie = true, IsVegan = true },
                    new MenuItem { Name = "Fruitkom", Description = "Dessert van de dag", Type = FoodType.Dessert, PriceStudent = 1.0, PriceExtern = 2.15, IsVeggie = true, IsVegan = true }
                }
            });

            // Menus for R2 (Resto Schoonmeersen B) - restos[1]
            menus.Add(new Menu
            {
                RestoId = restos[1].Id,
                Date = DateTime.Parse("2025-10-20T11:30:00"),
                MenuItems = new List<MenuItem>
                {
                    new MenuItem { Name = "Champignonsoep", Description = "Dagverse soep", Type = FoodType.Soep, PriceStudent = 1.0, PriceExtern = 2.15, IsVeggie = true, IsVegan = false },
                    new MenuItem { Name = "Lasagne", Description = "Vers bereid hoofdgerecht", Type = FoodType.WarmeMaaltijd, PriceStudent = 5.3, PriceExtern = 11.7, IsVeggie = false, IsVegan = false },
                    new MenuItem { Name = "Rijstpap", Description = "Dessert van de dag", Type = FoodType.Dessert, PriceStudent = 1.0, PriceExtern = 2.15, IsVeggie = true, IsVegan = false }
                }
            });

            menus.Add(new Menu
            {
                RestoId = restos[1].Id,
                Date = DateTime.Parse("2025-10-21T11:30:00"),
                MenuItems = new List<MenuItem>
                {
                    new MenuItem { Name = "Kervelsoep", Description = "Dagverse soep", Type = FoodType.Soep, PriceStudent = 1.0, PriceExtern = 2.15, IsVeggie = true, IsVegan = false },
                    new MenuItem { Name = "Vegan curry met kikkererwten", Description = "Vers bereid hoofdgerecht", Type = FoodType.WarmeMaaltijd, PriceStudent = 5.3, PriceExtern = 11.7, IsVeggie = true, IsVegan = true },
                    new MenuItem { Name = "Fruitkom", Description = "Dessert van de dag", Type = FoodType.Dessert, PriceStudent = 1.0, PriceExtern = 2.15, IsVeggie = true, IsVegan = true }
                }
            });

            menus.Add(new Menu
            {
                RestoId = restos[1].Id,
                Date = DateTime.Parse("2025-10-22T11:30:00"),
                MenuItems = new List<MenuItem>
                {
                    new MenuItem { Name = "Paprikasoep", Description = "Dagverse soep", Type = FoodType.Soep, PriceStudent = 1.0, PriceExtern = 2.15, IsVeggie = true, IsVegan = false },
                    new MenuItem { Name = "Spaghetti bolognese", Description = "Vers bereid hoofdgerecht", Type = FoodType.WarmeMaaltijd, PriceStudent = 5.3, PriceExtern = 11.7, IsVeggie = false, IsVegan = false },
                    new MenuItem { Name = "Fruitkom", Description = "Dessert van de dag", Type = FoodType.Dessert, PriceStudent = 1.0, PriceExtern = 2.15, IsVeggie = true, IsVegan = true }
                }
            });

            menus.Add(new Menu
            {
                RestoId = restos[1].Id,
                Date = DateTime.Parse("2025-10-23T11:30:00"),
                MenuItems = new List<MenuItem>
                {
                    new MenuItem { Name = "Wortelsoep", Description = "Dagverse soep", Type = FoodType.Soep, PriceStudent = 1.0, PriceExtern = 2.15, IsVeggie = true, IsVegan = true },
                    new MenuItem { Name = "Vegan curry met kikkererwten", Description = "Vers bereid hoofdgerecht", Type = FoodType.WarmeMaaltijd, PriceStudent = 5.3, PriceExtern = 11.7, IsVeggie = true, IsVegan = true },
                    new MenuItem { Name = "Fruitkom", Description = "Dessert van de dag", Type = FoodType.Dessert, PriceStudent = 1.0, PriceExtern = 2.15, IsVeggie = true, IsVegan = true }
                }
            });

            menus.Add(new Menu
            {
                RestoId = restos[1].Id,
                Date = DateTime.Parse("2025-10-24T11:30:00"),
                MenuItems = new List<MenuItem>
                {
                    new MenuItem { Name = "Tomatensoep met balletjes", Description = "Dagverse soep", Type = FoodType.Soep, PriceStudent = 1.0, PriceExtern = 2.15, IsVeggie = true, IsVegan = false },
                    new MenuItem { Name = "Kipfilet met pepersaus", Description = "Vers bereid hoofdgerecht", Type = FoodType.WarmeMaaltijd, PriceStudent = 5.3, PriceExtern = 11.7, IsVeggie = false, IsVegan = false },
                    new MenuItem { Name = "Pudding vanille", Description = "Dessert van de dag", Type = FoodType.Dessert, PriceStudent = 1.0, PriceExtern = 2.15, IsVeggie = true, IsVegan = false }
                }
            });

            // Menus for R3 (Resto Schoonmeersen P) - restos[2]
            menus.Add(new Menu
            {
                RestoId = restos[2].Id,
                Date = DateTime.Parse("2025-10-20T11:30:00"),
                MenuItems = new List<MenuItem>
                {
                    new MenuItem { Name = "Pompoensoep", Description = "Dagverse soep", Type = FoodType.Soep, PriceStudent = 1.0, PriceExtern = 2.15, IsVeggie = true, IsVegan = false },
                    new MenuItem { Name = "Falafel met couscous", Description = "Vers bereid hoofdgerecht", Type = FoodType.WarmeMaaltijd, PriceStudent = 5.3, PriceExtern = 11.7, IsVeggie = true, IsVegan = true },
                    new MenuItem { Name = "Chocomousse", Description = "Dessert van de dag", Type = FoodType.Dessert, PriceStudent = 1.0, PriceExtern = 2.15, IsVeggie = true, IsVegan = false }
                }
            });

            menus.Add(new Menu
            {
                RestoId = restos[2].Id,
                Date = DateTime.Parse("2025-10-21T11:30:00"),
                MenuItems = new List<MenuItem>
                {
                    new MenuItem { Name = "Champignonsoep", Description = "Dagverse soep", Type = FoodType.Soep, PriceStudent = 1.0, PriceExtern = 2.15, IsVeggie = true, IsVegan = false },
                    new MenuItem { Name = "Stoofvlees met frieten", Description = "Vers bereid hoofdgerecht", Type = FoodType.WarmeMaaltijd, PriceStudent = 5.3, PriceExtern = 11.7, IsVeggie = false, IsVegan = false },
                    new MenuItem { Name = "Fruitkom", Description = "Dessert van de dag", Type = FoodType.Dessert, PriceStudent = 1.0, PriceExtern = 2.15, IsVeggie = true, IsVegan = true }
                }
            });

            menus.Add(new Menu
            {
                RestoId = restos[2].Id,
                Date = DateTime.Parse("2025-10-22T11:30:00"),
                MenuItems = new List<MenuItem>
                {
                    new MenuItem { Name = "Tomatensoep met balletjes", Description = "Dagverse soep", Type = FoodType.Soep, PriceStudent = 1.0, PriceExtern = 2.15, IsVeggie = true, IsVegan = true },
                    new MenuItem { Name = "Vegetarische chili sin carne", Description = "Vers bereid hoofdgerecht", Type = FoodType.WarmeMaaltijd, PriceStudent = 5.3, PriceExtern = 11.7, IsVeggie = true, IsVegan = true },
                    new MenuItem { Name = "Chocomousse", Description = "Dessert van de dag", Type = FoodType.Dessert, PriceStudent = 1.0, PriceExtern = 2.15, IsVeggie = true, IsVegan = false }
                }
            });

            menus.Add(new Menu
            {
                RestoId = restos[2].Id,
                Date = DateTime.Parse("2025-10-23T11:30:00"),
                MenuItems = new List<MenuItem>
                {
                    new MenuItem { Name = "Tomatensoep met balletjes", Description = "Dagverse soep", Type = FoodType.Soep, PriceStudent = 1.0, PriceExtern = 2.15, IsVeggie = true, IsVegan = true },
                    new MenuItem { Name = "Vegan curry met kikkererwten", Description = "Vers bereid hoofdgerecht", Type = FoodType.WarmeMaaltijd, PriceStudent = 5.3, PriceExtern = 11.7, IsVeggie = true, IsVegan = true },
                    new MenuItem { Name = "Fruitkom", Description = "Dessert van de dag", Type = FoodType.Dessert, PriceStudent = 1.0, PriceExtern = 2.15, IsVeggie = true, IsVegan = true }
                }
            });

            menus.Add(new Menu
            {
                RestoId = restos[2].Id,
                Date = DateTime.Parse("2025-10-24T11:30:00"),
                MenuItems = new List<MenuItem>
                {
                    new MenuItem { Name = "Tomatensoep met balletjes", Description = "Dagverse soep", Type = FoodType.Soep, PriceStudent = 1.0, PriceExtern = 2.15, IsVeggie = true, IsVegan = true },
                    new MenuItem { Name = "Kipfilet met pepersaus", Description = "Vers bereid hoofdgerecht", Type = FoodType.WarmeMaaltijd, PriceStudent = 5.3, PriceExtern = 11.7, IsVeggie = false, IsVegan = false },
                    new MenuItem { Name = "Vegan muffin", Description = "Dessert van de dag", Type = FoodType.Dessert, PriceStudent = 1.0, PriceExtern = 2.15, IsVeggie = true, IsVegan = true }
                }
            });

            // Menus for R4 (Resto Mercator) - restos[3] if exists
            if (restos.Count > 3)
            {
                menus.Add(new Menu
                {
                    RestoId = restos[3].Id,
                    Date = DateTime.Parse("2025-10-20T11:30:00"),
                    MenuItems = new List<MenuItem>
                    {
                        new MenuItem { Name = "Groentesoep", Description = "Dagverse soep", Type = FoodType.Soep, PriceStudent = 1.0, PriceExtern = 2.15, IsVeggie = true, IsVegan = false },
                        new MenuItem { Name = "Kip tikka masala", Description = "Vers bereid hoofdgerecht", Type = FoodType.WarmeMaaltijd, PriceStudent = 5.3, PriceExtern = 11.7, IsVeggie = false, IsVegan = false },
                        new MenuItem { Name = "Vegan muffin", Description = "Dessert van de dag", Type = FoodType.Dessert, PriceStudent = 1.0, PriceExtern = 2.15, IsVeggie = true, IsVegan = true }
                    }
                });

                menus.Add(new Menu
                {
                    RestoId = restos[3].Id,
                    Date = DateTime.Parse("2025-10-21T11:30:00"),
                    MenuItems = new List<MenuItem>
                    {
                        new MenuItem { Name = "Pompoensoep", Description = "Dagverse soep", Type = FoodType.Soep, PriceStudent = 1.0, PriceExtern = 2.15, IsVeggie = true, IsVegan = false },
                        new MenuItem { Name = "Varkensgebraad met mosterdsaus", Description = "Vers bereid hoofdgerecht", Type = FoodType.WarmeMaaltijd, PriceStudent = 5.3, PriceExtern = 11.7, IsVeggie = false, IsVegan = false },
                        new MenuItem { Name = "Pudding vanille", Description = "Dessert van de dag", Type = FoodType.Dessert, PriceStudent = 1.0, PriceExtern = 2.15, IsVeggie = true, IsVegan = false }
                    }
                });

                menus.Add(new Menu
                {
                    RestoId = restos[3].Id,
                    Date = DateTime.Parse("2025-10-22T11:30:00"),
                    MenuItems = new List<MenuItem>
                    {
                        new MenuItem { Name = "Kervelsoep", Description = "Dagverse soep", Type = FoodType.Soep, PriceStudent = 1.0, PriceExtern = 2.15, IsVeggie = true, IsVegan = true },
                        new MenuItem { Name = "Groenteburger met quinoa", Description = "Vers bereid hoofdgerecht", Type = FoodType.WarmeMaaltijd, PriceStudent = 5.3, PriceExtern = 11.7, IsVeggie = true, IsVegan = true },
                        new MenuItem { Name = "Pudding vanille", Description = "Dessert van de dag", Type = FoodType.Dessert, PriceStudent = 1.0, PriceExtern = 2.15, IsVeggie = true, IsVegan = false }
                    }
                });

                menus.Add(new Menu
                {
                    RestoId = restos[3].Id,
                    Date = DateTime.Parse("2025-10-23T11:30:00"),
                    MenuItems = new List<MenuItem>
                    {
                        new MenuItem { Name = "Wortelsoep", Description = "Dagverse soep", Type = FoodType.Soep, PriceStudent = 1.0, PriceExtern = 2.15, IsVeggie = true, IsVegan = true },
                        new MenuItem { Name = "Kabeljauw met puree", Description = "Vers bereid hoofdgerecht", Type = FoodType.WarmeMaaltijd, PriceStudent = 5.3, PriceExtern = 11.7, IsVeggie = false, IsVegan = false },
                        new MenuItem { Name = "Pudding vanille", Description = "Dessert van de dag", Type = FoodType.Dessert, PriceStudent = 1.0, PriceExtern = 2.15, IsVeggie = true, IsVegan = false }
                    }
                });

                menus.Add(new Menu
                {
                    RestoId = restos[3].Id,
                    Date = DateTime.Parse("2025-10-24T11:30:00"),
                    MenuItems = new List<MenuItem>
                    {
                        new MenuItem { Name = "Tomatensoep met balletjes", Description = "Dagverse soep", Type = FoodType.Soep, PriceStudent = 1.0, PriceExtern = 2.15, IsVeggie = true, IsVegan = true },
                        new MenuItem { Name = "Lasagne", Description = "Vers bereid hoofdgerecht", Type = FoodType.WarmeMaaltijd, PriceStudent = 5.3, PriceExtern = 11.7, IsVeggie = false, IsVegan = false },
                        new MenuItem { Name = "Pudding vanille", Description = "Dessert van de dag", Type = FoodType.Dessert, PriceStudent = 1.0, PriceExtern = 2.15, IsVeggie = true, IsVegan = false }
                    }
                });
            }

            // Menus for R5 (Resto Ledeganck) - restos[4] if exists
            if (restos.Count > 4)
            {
                menus.Add(new Menu
                {
                    RestoId = restos[4].Id,
                    Date = DateTime.Parse("2025-10-20T11:30:00"),
                    MenuItems = new List<MenuItem>
                    {
                        new MenuItem { Name = "Tomatensoep met balletjes", Description = "Dagverse soep", Type = FoodType.Soep, PriceStudent = 1.0, PriceExtern = 2.15, IsVeggie = true, IsVegan = true },
                        new MenuItem { Name = "Groenteburger met quinoa", Description = "Vers bereid hoofdgerecht", Type = FoodType.WarmeMaaltijd, PriceStudent = 5.3, PriceExtern = 11.7, IsVeggie = true, IsVegan = true },
                        new MenuItem { Name = "Chocomousse", Description = "Dessert van de dag", Type = FoodType.Dessert, PriceStudent = 1.0, PriceExtern = 2.15, IsVeggie = true, IsVegan = false }
                    }
                });

                menus.Add(new Menu
                {
                    RestoId = restos[4].Id,
                    Date = DateTime.Parse("2025-10-21T11:30:00"),
                    MenuItems = new List<MenuItem>
                    {
                        new MenuItem { Name = "Wortelsoep", Description = "Dagverse soep", Type = FoodType.Soep, PriceStudent = 1.0, PriceExtern = 2.15, IsVeggie = true, IsVegan = true },
                        new MenuItem { Name = "Groenteburger met quinoa", Description = "Vers bereid hoofdgerecht", Type = FoodType.WarmeMaaltijd, PriceStudent = 5.3, PriceExtern = 11.7, IsVeggie = true, IsVegan = true },
                        new MenuItem { Name = "Chocomousse", Description = "Dessert van de dag", Type = FoodType.Dessert, PriceStudent = 1.0, PriceExtern = 2.15, IsVeggie = true, IsVegan = false }
                    }
                });

                menus.Add(new Menu
                {
                    RestoId = restos[4].Id,
                    Date = DateTime.Parse("2025-10-22T11:30:00"),
                    MenuItems = new List<MenuItem>
                    {
                        new MenuItem { Name = "Tomatensoep met balletjes", Description = "Dagverse soep", Type = FoodType.Soep, PriceStudent = 1.0, PriceExtern = 2.15, IsVeggie = true, IsVegan = true },
                        new MenuItem { Name = "Varkensgebraad met mosterdsaus", Description = "Vers bereid hoofdgerecht", Type = FoodType.WarmeMaaltijd, PriceStudent = 5.3, PriceExtern = 11.7, IsVeggie = false, IsVegan = false },
                        new MenuItem { Name = "Vegan muffin", Description = "Dessert van de dag", Type = FoodType.Dessert, PriceStudent = 1.0, PriceExtern = 2.15, IsVeggie = true, IsVegan = true }
                    }
                });

                menus.Add(new Menu
                {
                    RestoId = restos[4].Id,
                    Date = DateTime.Parse("2025-10-23T11:30:00"),
                    MenuItems = new List<MenuItem>
                    {
                        new MenuItem { Name = "Kervelsoep", Description = "Dagverse soep", Type = FoodType.Soep, PriceStudent = 1.0, PriceExtern = 2.15, IsVeggie = true, IsVegan = true },
                        new MenuItem { Name = "Vegetarische chili sin carne", Description = "Vers bereid hoofdgerecht", Type = FoodType.WarmeMaaltijd, PriceStudent = 5.3, PriceExtern = 11.7, IsVeggie = true, IsVegan = true },
                        new MenuItem { Name = "Pudding vanille", Description = "Dessert van de dag", Type = FoodType.Dessert, PriceStudent = 1.0, PriceExtern = 2.15, IsVeggie = true, IsVegan = false }
                    }
                });

                menus.Add(new Menu
                {
                    RestoId = restos[4].Id,
                    Date = DateTime.Parse("2025-10-24T11:30:00"),
                    MenuItems = new List<MenuItem>
                    {
                        new MenuItem { Name = "Wortelsoep", Description = "Dagverse soep", Type = FoodType.Soep, PriceStudent = 1.0, PriceExtern = 2.15, IsVeggie = true, IsVegan = true },
                        new MenuItem { Name = "Vegetarische chili sin carne", Description = "Vers bereid hoofdgerecht", Type = FoodType.WarmeMaaltijd, PriceStudent = 5.3, PriceExtern = 11.7, IsVeggie = true, IsVegan = true },
                        new MenuItem { Name = "Chocomousse", Description = "Dessert van de dag", Type = FoodType.Dessert, PriceStudent = 1.0, PriceExtern = 2.15, IsVeggie = true, IsVegan = false }
                    }
                });
            }

            // Menus for R6 (Campus Resto Vesalius) - restos[5] if exists
            if (restos.Count > 5)
            {
                menus.Add(new Menu
                {
                    RestoId = restos[5].Id,
                    Date = DateTime.Parse("2025-10-20T17:00:00"),
                    MenuItems = new List<MenuItem>
                    {
                        new MenuItem { Name = "Champignonsoep", Description = "Dagverse soep", Type = FoodType.Soep, PriceStudent = 1.0, PriceExtern = 2.15, IsVeggie = true, IsVegan = true },
                        new MenuItem { Name = "Kabeljauw met puree", Description = "Vers bereid hoofdgerecht", Type = FoodType.WarmeMaaltijd, PriceStudent = 5.3, PriceExtern = 11.7, IsVeggie = false, IsVegan = false },
                        new MenuItem { Name = "Pudding vanille", Description = "Dessert van de dag", Type = FoodType.Dessert, PriceStudent = 1.0, PriceExtern = 2.15, IsVeggie = true, IsVegan = false }
                    }
                });

                menus.Add(new Menu
                {
                    RestoId = restos[5].Id,
                    Date = DateTime.Parse("2025-10-21T17:00:00"),
                    MenuItems = new List<MenuItem>
                    {
                        new MenuItem { Name = "Champignonsoep", Description = "Dagverse soep", Type = FoodType.Soep, PriceStudent = 1.0, PriceExtern = 2.15, IsVeggie = true, IsVegan = true },
                        new MenuItem { Name = "Groenteburger met quinoa", Description = "Vers bereid hoofdgerecht", Type = FoodType.WarmeMaaltijd, PriceStudent = 5.3, PriceExtern = 11.7, IsVeggie = true, IsVegan = true },
                        new MenuItem { Name = "Pudding vanille", Description = "Dessert van de dag", Type = FoodType.Dessert, PriceStudent = 1.0, PriceExtern = 2.15, IsVeggie = true, IsVegan = false }
                    }
                });

                menus.Add(new Menu
                {
                    RestoId = restos[5].Id,
                    Date = DateTime.Parse("2025-10-22T17:00:00"),
                    MenuItems = new List<MenuItem>
                    {
                        new MenuItem { Name = "Groentesoep", Description = "Dagverse soep", Type = FoodType.Soep, PriceStudent = 1.0, PriceExtern = 2.15, IsVeggie = true, IsVegan = true },
                        new MenuItem { Name = "Kipfilet met pepersaus", Description = "Vers bereid hoofdgerecht", Type = FoodType.WarmeMaaltijd, PriceStudent = 5.3, PriceExtern = 11.7, IsVeggie = false, IsVegan = false },
                        new MenuItem { Name = "Chocomousse", Description = "Dessert van de dag", Type = FoodType.Dessert, PriceStudent = 1.0, PriceExtern = 2.15, IsVeggie = true, IsVegan = false }
                    }
                });

                menus.Add(new Menu
                {
                    RestoId = restos[5].Id,
                    Date = DateTime.Parse("2025-10-23T17:00:00"),
                    MenuItems = new List<MenuItem>
                    {
                        new MenuItem { Name = "Kervelsoep", Description = "Dagverse soep", Type = FoodType.Soep, PriceStudent = 1.0, PriceExtern = 2.15, IsVeggie = true, IsVegan = false },
                        new MenuItem { Name = "Vegan curry met kikkererwten", Description = "Vers bereid hoofdgerecht", Type = FoodType.WarmeMaaltijd, PriceStudent = 5.3, PriceExtern = 11.7, IsVeggie = true, IsVegan = true },
                        new MenuItem { Name = "Fruitkom", Description = "Dessert van de dag", Type = FoodType.Dessert, PriceStudent = 1.0, PriceExtern = 2.15, IsVeggie = true, IsVegan = true }
                    }
                });

                menus.Add(new Menu
                {
                    RestoId = restos[5].Id,
                    Date = DateTime.Parse("2025-10-24T17:00:00"),
                    MenuItems = new List<MenuItem>
                    {
                        new MenuItem { Name = "Kervelsoep", Description = "Dagverse soep", Type = FoodType.Soep, PriceStudent = 1.0, PriceExtern = 2.15, IsVeggie = true, IsVegan = false },
                        new MenuItem { Name = "Lasagne", Description = "Vers bereid hoofdgerecht", Type = FoodType.WarmeMaaltijd, PriceStudent = 5.3, PriceExtern = 11.7, IsVeggie = false, IsVegan = false },
                        new MenuItem { Name = "Pudding vanille", Description = "Dessert van de dag", Type = FoodType.Dessert, PriceStudent = 1.0, PriceExtern = 2.15, IsVeggie = true, IsVegan = false }
                    }
                });
            }

            // Menus for R7 (Resto Sampori) - restos[6] if exists
            if (restos.Count > 6)
            {
                menus.Add(new Menu
                {
                    RestoId = restos[6].Id,
                    Date = DateTime.Parse("2025-10-20T09:00:00"),
                    MenuItems = new List<MenuItem>
                    {
                        new MenuItem { Name = "Courgettesoep", Description = "Dagverse soep", Type = FoodType.Soep, PriceStudent = 1.0, PriceExtern = 2.15, IsVeggie = true, IsVegan = false },
                        new MenuItem { Name = "Kabeljauw met puree", Description = "Vers bereid hoofdgerecht", Type = FoodType.WarmeMaaltijd, PriceStudent = 5.3, PriceExtern = 11.7, IsVeggie = false, IsVegan = false },
                        new MenuItem { Name = "Pudding vanille", Description = "Dessert van de dag", Type = FoodType.Dessert, PriceStudent = 1.0, PriceExtern = 2.15, IsVeggie = true, IsVegan = false }
                    }
                });

                menus.Add(new Menu
                {
                    RestoId = restos[6].Id,
                    Date = DateTime.Parse("2025-10-21T09:00:00"),
                    MenuItems = new List<MenuItem>
                    {
                        new MenuItem { Name = "Champignonsoep", Description = "Dagverse soep", Type = FoodType.Soep, PriceStudent = 1.0, PriceExtern = 2.15, IsVeggie = true, IsVegan = true },
                        new MenuItem { Name = "Kip tikka masala", Description = "Vers bereid hoofdgerecht", Type = FoodType.WarmeMaaltijd, PriceStudent = 5.3, PriceExtern = 11.7, IsVeggie = false, IsVegan = false },
                        new MenuItem { Name = "Pudding vanille", Description = "Dessert van de dag", Type = FoodType.Dessert, PriceStudent = 1.0, PriceExtern = 2.15, IsVeggie = true, IsVegan = false }
                    }
                });

                menus.Add(new Menu
                {
                    RestoId = restos[6].Id,
                    Date = DateTime.Parse("2025-10-22T09:00:00"),
                    MenuItems = new List<MenuItem>
                    {
                        new MenuItem { Name = "Tomatensoep met balletjes", Description = "Dagverse soep", Type = FoodType.Soep, PriceStudent = 1.0, PriceExtern = 2.15, IsVeggie = true, IsVegan = false },
                        new MenuItem { Name = "Spaghetti bolognese", Description = "Vers bereid hoofdgerecht", Type = FoodType.WarmeMaaltijd, PriceStudent = 5.3, PriceExtern = 11.7, IsVeggie = false, IsVegan = false },
                        new MenuItem { Name = "Fruitkom", Description = "Dessert van de dag", Type = FoodType.Dessert, PriceStudent = 1.0, PriceExtern = 2.15, IsVeggie = true, IsVegan = true }
                    }
                });

                menus.Add(new Menu
                {
                    RestoId = restos[6].Id,
                    Date = DateTime.Parse("2025-10-23T09:00:00"),
                    MenuItems = new List<MenuItem>
                    {
                        new MenuItem { Name = "Paprikasoep", Description = "Dagverse soep", Type = FoodType.Soep, PriceStudent = 1.0, PriceExtern = 2.15, IsVeggie = true, IsVegan = true },
                        new MenuItem { Name = "Vegetarische chili sin carne", Description = "Vers bereid hoofdgerecht", Type = FoodType.WarmeMaaltijd, PriceStudent = 5.3, PriceExtern = 11.7, IsVeggie = true, IsVegan = true },
                        new MenuItem { Name = "Pudding vanille", Description = "Dessert van de dag", Type = FoodType.Dessert, PriceStudent = 1.0, PriceExtern = 2.15, IsVeggie = true, IsVegan = false }
                    }
                });

                menus.Add(new Menu
                {
                    RestoId = restos[6].Id,
                    Date = DateTime.Parse("2025-10-24T09:00:00"),
                    MenuItems = new List<MenuItem>
                    {
                        new MenuItem { Name = "Groentesoep", Description = "Dagverse soep", Type = FoodType.Soep, PriceStudent = 1.0, PriceExtern = 2.15, IsVeggie = true, IsVegan = false },
                        new MenuItem { Name = "Kipfilet met pepersaus", Description = "Vers bereid hoofdgerecht", Type = FoodType.WarmeMaaltijd, PriceStudent = 5.3, PriceExtern = 11.7, IsVeggie = false, IsVegan = false },
                        new MenuItem { Name = "Chocomousse", Description = "Dessert van de dag", Type = FoodType.Dessert, PriceStudent = 1.0, PriceExtern = 2.15, IsVeggie = true, IsVegan = false }
                    }
                });
            }

            dbContext.Menus.AddRange(menus);
            await dbContext.SaveChangesAsync();


        }


    }
}