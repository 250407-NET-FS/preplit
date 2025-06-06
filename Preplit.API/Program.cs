using System.Text;
using Preplit.API;
using Preplit.Data;
using Preplit.Domain;
using Preplit.Services.Core;
using Preplit.Services.Cards.Queries;
using Preplit.Services.Cards.Commands;
using Preplit.Services.Categories.Queries;
using Preplit.Services.Categories.Commands;
using Preplit.Services.Users.Queries;
using Preplit.Services.Users.Commands;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore;
using System.Security.Claims;
using Microsoft.AspNetCore.Cors.Infrastructure;


var builder = WebApplication.CreateBuilder(args);
var connectionString =
    builder.Configuration.GetConnectionString("DefaultConnection")
        ?? throw new InvalidOperationException("Connection string"
        + "'DefaultConnection' not found.");

builder.Services.AddDbContext<PreplitContext>(options =>
    options.UseSqlServer(connectionString));

builder.Services.AddControllers();

builder.Services.AddAutoMapper(typeof(MappingProfiles).Assembly);
builder.Services.AddMediatR(options =>
{
    // Card Handlers
    options.RegisterServicesFromAssemblyContaining<GetCardDetails.Handler>();
    options.RegisterServicesFromAssemblyContaining<GetCardList.Handler>();
    options.RegisterServicesFromAssemblyContaining<GetCategoryCardList.Handler>();
    options.RegisterServicesFromAssemblyContaining<CreateCard.Handler>();
    options.RegisterServicesFromAssemblyContaining<EditCard.Handler>();
    options.RegisterServicesFromAssemblyContaining<DeleteCard.Handler>();
    // Category Handlers
    options.RegisterServicesFromAssemblyContaining<GetCategoryList.Handler>();
    options.RegisterServicesFromAssemblyContaining<GetUserCategoryList.Handler>();
    options.RegisterServicesFromAssemblyContaining<GetCategoryDetails.Handler>();
    options.RegisterServicesFromAssemblyContaining<CreateCategory.Handler>();
    options.RegisterServicesFromAssemblyContaining<EditCategory.Handler>();
    options.RegisterServicesFromAssemblyContaining<DeleteCategory.Handler>();
    // User Handlers
    options.RegisterServicesFromAssemblyContaining<GetUserList.Handler>();
    options.RegisterServicesFromAssemblyContaining<GetUserDetails.Handler>();
    options.RegisterServicesFromAssemblyContaining<GetLoggedUser.Handler>();
    options.RegisterServicesFromAssemblyContaining<GenerateToken.Handler>();
    options.RegisterServicesFromAssemblyContaining<DeleteUser.Handler>();
});

builder.Services.AddOpenApi();

builder
    .Services.AddIdentityCore<User>(options =>
    {
        options.Lockout.AllowedForNewUsers = false;
        options.Password.RequireDigit = true;
        options.Password.RequiredLength = 8;
        options.Password.RequireNonAlphanumeric = false;
        options.Password.RequireUppercase = true;
    })
    .AddRoles<IdentityRole<Guid>>()
    .AddEntityFrameworkStores<PreplitContext>()
    .AddSignInManager()
    .AddRoleManager<RoleManager<IdentityRole<Guid>>>();

SymmetricSecurityKey key = new(Encoding.UTF8.GetBytes(builder.Configuration["Jwt:Key"]));

builder
    .Services.AddAuthentication(options =>
    {
        options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
        options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
    })
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuerSigningKey = true,
            IssuerSigningKey = key,
            ValidateIssuer = true,
            ValidIssuer = builder.Configuration["Jwt:Issuer"],
            ValidateAudience = true,
            ValidAudience = builder.Configuration["Jwt:Audience"],
            ValidateLifetime = true,
            ClockSkew = TimeSpan.Zero,
            NameClaimType = ClaimTypes.Name,
            RoleClaimType = ClaimTypes.Role,
        };

        options.Events = new JwtBearerEvents
        {
            OnMessageReceived = ctx =>
            {
                // grab the cookie named "jwt" and then User.Identity?.IsAuthenticated should work
                if (ctx.Request.Cookies.TryGetValue("jwt", out var token))
                    ctx.Token = token;
                return Task.CompletedTask;
            },
        };
    });


builder.Services.AddAuthorization();

//swagger
//Adding swagger support
builder.Services.AddEndpointsApiExplorer();

//Modifying this AddSwaggerGen() call to allow us to test/debug our Auth scheme setup in swagger
builder.Services.AddSwaggerGen(c =>
{
    c.AddSecurityDefinition(
        "Bearer",
        new OpenApiSecurityScheme
        {
            Description = "JWT Authorization header using the Bearer scheme",
            Type = SecuritySchemeType.Http,
            Scheme = "bearer"
        }
    );
    c.AddSecurityRequirement(
        new OpenApiSecurityRequirement
        {
            {
                new OpenApiSecurityScheme
                {
                    Reference = new OpenApiReference
                    {
                        Type = ReferenceType.SecurityScheme,
                        Id = "Bearer"
                    }
                },
                Array.Empty<string>()
            }
        }
    );
});

//Can use http requests
builder.Services.AddHttpClient();

//cORS
CorsPolicyBuilder localPolicy = new CorsPolicyBuilder()
    .WithOrigins("http://localhost:5173") // React dev server
    .AllowAnyHeader()
    .AllowAnyMethod(); 

CorsPolicyBuilder azurePolicy = new CorsPolicyBuilder()
    .WithOrigins("preplit-fbg3d5hgecetekhj.westus2-01.azurewebsites.net")
    .AllowAnyHeader()
    .AllowAnyMethod();

builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowReactApp", localPolicy.Build());
    options.AddPolicy("AllowAzureApp", azurePolicy.Build());
});

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
    app.UseSwagger();
    app.UseSwaggerUI();
    app.UseCors("AllowReactApp");
}
else
{
    app.UseCors("AllowAzureApp");

    app.UseExceptionHandler("/Error");
    app.UseHsts();
}
app.UseHttpsRedirection();


app.UseRouting();
app.UseAuthentication();
app.UseAuthorization();


app.MapControllers();


// For first timec
using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;

    try
    {
        var context = services.GetRequiredService<PreplitContext>();
        var userManager = services.GetRequiredService<UserManager<User>>();
        var roleManager = services.GetRequiredService<RoleManager<IdentityRole<Guid>>>();

        await context.Database.MigrateAsync();

        await Seeder.SeedAdminData(userManager, roleManager);
        await Seeder.SeedUserData(userManager);
        await Seeder.SeedCategoryData(userManager, context);
        await Seeder.SeedCardData(userManager, context);
    }
    catch (Exception ex)
    {
        var logger = services.GetRequiredService<ILogger<Program>>();
        logger.LogError(ex, "Error seeding roles");
    }
}

app.Run();

