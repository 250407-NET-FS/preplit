using Preplit.Domain;

using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace Preplit.Data {
    public class Seeder {
    public static async Task SeedAdminData(UserManager<User> userManager, RoleManager<IdentityRole<Guid>> roleManager)
    {
        const string email = "admin@admin.com";
        const string password = "Admin1234";
        const string fullName = "Admin";

        if (!await roleManager.RoleExistsAsync("Admin"))
        {
            var roleResult = await roleManager.CreateAsync(new IdentityRole<Guid>("Admin"));
            if (!roleResult.Succeeded)
                throw new Exception($"Could not create Admin role: {string.Join("; ", roleResult.Errors.Select(e => e.Description))}");
        }



        if (await userManager.FindByEmailAsync(email) is null)
        {
            var admin = new User
            {
                Id = Guid.NewGuid(),
                UserName = email,
                FullName = fullName,
                Email = email,

            };

            var result = await userManager.CreateAsync(admin, password);
            if (!result.Succeeded)
            {
                var errors = string.Join("; ", result.Errors.Select(e => e.Description));
                throw new Exception($"Seeding admin user failed: {errors}");
            }



            if (!await userManager.IsInRoleAsync(admin, "Admin"))
            {
                await userManager.AddToRoleAsync(admin, "Admin");
            }

        }
    }

    public static async Task SeedUserData(UserManager<User> userManager)
        {
            const string email = "user@user.com";
            const string password = "User1234";
            const string fullName = "User";
            if (await userManager.FindByEmailAsync(email) is null)
            {
                var User = new User
                {
                    Id = Guid.NewGuid(),
                    UserName = email,
                    FullName = fullName,
                    Email = email,

                };

                var result = await userManager.CreateAsync(User, password);
                if (!result.Succeeded)
                {
                    var errors = string.Join("; ", result.Errors.Select(e => e.Description));
                    throw new Exception($"Seeding User user failed: {errors}");
                }
            }
        }

        public static async Task SeedCategoryData(UserManager<User> userManager, PreplitContext context)
        {

            const string userEmail = "user@user.com";
            var user = await userManager.FindByEmailAsync(userEmail);

            if (user is null) { return; }

            if (!context.Categories.Any())
            {

                List<Category> categories = [
                    new("Security", user.Id),
                    new("C#", user.Id),
                    new("SQL", user.Id),
                    new("HTML", user.Id),
                    new("JavaScript", user.Id)
                ];

                context.Categories.AddRange(categories);
                await context.SaveChangesAsync();
            }
        }

        public static async Task SeedCardData(PreplitContext context)
        {
            List<Category> categories = await context.Categories.ToListAsync();
            if (!context.Cards.Any())
            {
                List<Card> cards = [
                    new (
                        "What is the difference between authentication and authorization?",
                        """
                            Gaining access to a resource using some sort of input is authentication while 
                            having permission to use a resource based on a role is authorization.
                        """,
                        categories.Find(c => c.Name.Equals("Security"))!.CategoryId
                    ),
                    new (
                        "How do you close a connection (to a user) on a deployed API, if it were attacked maliciously? (How do you stop a DOS Attack)",
                        """
                            ALTER DATABASE database SET user WITH ROLLBACK immediate;
                            ALTER DATABASE database SET MULTI_USER;
                        """,
                        categories.Find(c => c.Name.Equals("Security"))!.CategoryId
                    ),
                    new (
                        "Explain S. O. L. I. D.",
                        """
                            Single Responsibility - Logic should only have one responsibility based on the overall class definition
                            Open/closed principle - Logic should be open for extension, but closed for modification (why?)
                            Liskov's substitution - Parent classes should be easily replaceable with their derived classes
                            Interface segregation - Applications should not have interfaces that users do need
                            Dependency inversion - High level modules should not need to depend on low-level modules while they both should depend on abstractions
                        """,
                        categories.Find(c => c.Name.Equals("C#"))!.CategoryId
                    ),
                    new (
                        "What are the different types of constructors?",
                        """
                            Default constructors: Automatically generated by the class when no constructors are explicitly defined
                            Parameterized constructors: Contain parameters passed as values to class properties
                            Static constructors: Mainly pass values to static properties or perform one-time actions (ex. singleton)
                            Copy constructors: Passing values from a class object to base properties
                            Private constructors: Ensure class instances are not created, especially when no instance properties or methods are present
                        """,
                        categories.Find(c => c.Name.Equals("C#"))!.CategoryId
                    ),
                    new (
                        "What is an index?",
                        """
                            SQL indexes are data structures that efficiently retrieve data from a database. 
                            They are used to speed up queries and improve database performance by reducing 
                            the amount of data that needs to be scanned to find the desired information.
                        """,
                        categories.Find(c => c.Name.Equals("SQL"))!.CategoryId
                    ),
                    new (
                        "Define composite, foreign, and primary key relations",
                        """
                            A foreign key relation maps an attribute in one table to a primary key attribute
                            in another table. A primary key relation maps a primary key attribute, usually 
                            an INT or SQLSERVER’s GUID, to each row in a table. A composite key relation 
                            combines 2 or more attributes in a table as a primary key; a composite key in
                            1 table could map to a primary key in another table.
                        """,
                        categories.Find(c => c.Name.Equals("SQL"))!.CategoryId
                    ),
                    new (
                        "How would you add a link in HTML?",
                        "<a href=”https://www.google.com”>Click here</a>",
                        categories.Find(c => c.Name.Equals("HTML"))!.CategoryId
                    ),
                    new (
                        "How would you add an image that opens a new window when clicked?",
                        """
                            <a href="https://www.google.com" target="_blank">
                                <img src="/image.png">
                            </a>
                        """,
                        categories.Find(c => c.Name.Equals("HTML"))!.CategoryId
                    ),
                    new (
                        "What is a closure?",
                        """
                            The combination of a function bundled together (enclosed) with references 
                            to its surrounding state (the lexical environment). In other words, a closure
                            gives a function access to its outer scope. In JavaScript, closures are created
                            every time a function is created, at function creation time. (ex. A function 
                            inside another function)
                        """,
                        categories.Find(c => c.Name.Equals("JavaScript"))!.CategoryId
                    ),
                    new (
                        "What is routing?",
                        """
                            It is a way to connect web urls to specified components. Deeplink allows browsers 
                            to track of url changes between route components, especially conditional rendering.
                        """,
                        categories.Find(c => c.Name.Equals("JavaScript"))!.CategoryId
                    )
                ];

                context.Cards.AddRange(cards);
                await context.SaveChangesAsync();
            }
        }
    }
}