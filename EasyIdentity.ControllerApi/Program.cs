using EasyIdentity.Core.Constants;
using EasyIdentity.Core.Data;
using EasyIdentity.Core.Entities;
using EasyIdentity.Core.Services.Authentication;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddDbContext<ApplicationDbContext>(options =>
{
    //NOTE: You can use UseSqlServer instead
    options.UseSqlite("DataSource=EasyIdentity.db");
});

builder.Services.AddIdentityCore<ApplicationUser>()
    .AddEntityFrameworkStores<ApplicationDbContext>()
    .AddDefaultTokenProviders();

builder.Services.AddSingleton<ITokenService, TokenService>();

// Microsoft.AspNetCore.Authentication.JwtBearer
var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(builder.Configuration["TokenKey"]
    ?? throw new InvalidOperationException("The Token Key not found!")));

builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        // Authenticate the Token with Cookies
        options.Events = new JwtBearerEvents
        {
            OnMessageReceived = (context) =>
            {
                var hasToken = context.Request.Cookies.TryGetValue(AuthenticationConstants.CookieAccessToken, out var token);
                if (hasToken && !string.IsNullOrEmpty(token))
                {
                    context.Token = token;
                }
                return Task.CompletedTask;
            }
        };
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuerSigningKey = true,
            IssuerSigningKey = key,
            ValidateIssuer = false,
            ValidateAudience = false,
            ValidateLifetime = true,
            ClockSkew = TimeSpan.FromSeconds(5)
        };
    });

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

if (app.Environment.IsDevelopment())
{
    using var scope = app.Services.CreateScope();
    var context = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
    var userManager = scope.ServiceProvider.GetRequiredService<UserManager<ApplicationUser>>();
    await context.Database.MigrateAsync();

    //NOTE: Remove this if you don't want to seed admin user
    // ---
    if (!context.Users.Any(x => x.UserName == "admin"))
    {
        var admin = new ApplicationUser
        {
            UserName = "admin",
            Email = "admin@gmail.com",
            FullName = "Administrator"
        };
        var result = await userManager.CreateAsync(admin, "P@ssw0rd");
    }
    // ---
}

app.UseHttpsRedirection();

app.UseAuthentication();

app.UseAuthorization();

app.MapControllers();

app.Run();
