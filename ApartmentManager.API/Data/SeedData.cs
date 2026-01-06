using ApartmentManager.Shared.Models;
using Bogus;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace ApartmentManager.API.Data;

public static class SeedData
{
    public static async Task InitializeAsync(IServiceProvider serviceProvider)
    {
        var userManager = serviceProvider.GetRequiredService<UserManager<ApplicationUser>>();
        var roleManager = serviceProvider.GetRequiredService<RoleManager<IdentityRole>>();
        var context = serviceProvider.GetRequiredService<AppDbContext>();

        string[] roleNames = { "Admin", "User" };
        foreach (var roleName in roleNames)
            if (!await roleManager.RoleExistsAsync(roleName))
                await roleManager.CreateAsync(new IdentityRole(roleName));

        var adminEmail = "admin@test.com";
        if (await userManager.FindByEmailAsync(adminEmail) == null)
        {
            var adminUser = new ApplicationUser
            {
                UserName = adminEmail,
                Email = adminEmail,
                Nume = "Admin",
                Prenume = "System",
                ApartamentID = 0
            };
            await userManager.CreateAsync(adminUser, "Admin123!");
            await userManager.AddToRoleAsync(adminUser, "Admin");
        }

        if (await context.Apartamente.AnyAsync()) return;

        var aptIds = 1;
        var aptFaker = new Faker<Apartament>()
            .RuleFor(a => a.NumarApartament, f => (aptIds++).ToString())
            .RuleFor(a => a.Etaj, f => f.Random.Int(0, 4))
            .RuleFor(a => a.Suprafata, f => f.Random.Double(30, 100))
            .RuleFor(a => a.NumarPersoane, f => f.Random.Int(1, 5));

        var apartamente = aptFaker.Generate(20);
        context.Apartamente.AddRange(apartamente);
        await context.SaveChangesAsync();

        var userFaker = new Faker<ApplicationUser>()
            .RuleFor(u => u.UserName, f => f.Internet.Email())
            .RuleFor(u => u.Email, (f, u) => u.UserName)
            .RuleFor(u => u.Nume, f => f.Name.LastName())
            .RuleFor(u => u.Prenume, f => f.Name.FirstName())
            .RuleFor(u => u.ApartamentID, f => f.PickRandom(apartamente).ID);

        var users = userFaker.Generate(30);
        var createdUserIds = new List<string>();

        foreach (var user in users)
            if (await userManager.FindByEmailAsync(user.Email!) == null)
            {
                var result = await userManager.CreateAsync(user, "User123!");
                if (result.Succeeded)
                {
                    await userManager.AddToRoleAsync(user, "User");
                    createdUserIds.Add(user.Id);
                }
            }

        var facturaFaker = new Faker<FacturaIndividuala>()
            .RuleFor(f => f.Tip, f => f.PickRandom("Intreținere", "Gaze", "Lumină", "Apa"))
            .RuleFor(f => f.NumeApartament, (f, fac) => "Ap. " + f.PickRandom(apartamente).NumarApartament)
            .RuleFor(f => f.Luna, f => f.Date.Month())
            .RuleFor(f => f.SumaDePlata, f => f.Finance.Amount(50, 500))
            .RuleFor(f => f.Status, f => f.PickRandom("Plătit", "Neplătit"))
            .RuleFor(f => f.IdApartament, f => f.PickRandom(apartamente).ID);

        var facturi = facturaFaker.Generate(50);
        context.FacturiIndividuale.AddRange(facturi);

        var chelFaker = new Faker<CheltuialaAsociatie>()
            .RuleFor(c => c.Tip, f => f.PickRandom("Curățenie", "Ascensor", "Fond Rulment", "Administrație"))
            .RuleFor(c => c.SumaTotala, f => f.Finance.Amount(100, 2000))
            .RuleFor(c => c.DataFactura, f => f.Date.Recent(30));

        var cheltuieli = chelFaker.Generate(10);
        context.CheltuieliAsociatie.AddRange(cheltuieli);

        var sesizFaker = new Faker<Sesizare>()
            .RuleFor(s => s.Subiect, f => f.Hacker.Phrase())
            .RuleFor(s => s.Descriere, f => f.Lorem.Paragraph())
            .RuleFor(s => s.Status, f => f.PickRandom("Nou", "In Procesare", "Rezolvat"))
            .RuleFor(s => s.IdLocatar,
                f => f.PickRandom(createdUserIds));

        var sesizari = sesizFaker.Generate(15);
        context.Sesizari.AddRange(sesizari);

        await context.SaveChangesAsync();
    }
}