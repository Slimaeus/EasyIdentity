using EasyIdentity.Core.Data;
using EasyIdentity.Core.Entities;
using EasyIdentity.Core.Services;
using EasyIdentity.Core.Services.Authentication;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllersWithViews();

builder.Services.AddSwaggerGen(options =>
{
    var securitySchema = new OpenApiSecurityScheme
    {
        Description = "JWT Authorization header using the Bearer scheme. Example: \"Authorization: Bearer {token}\"",
        Name = "Authorization",
        In = ParameterLocation.Header,
        Type = SecuritySchemeType.Http,
        Scheme = "bearer",
        Reference = new OpenApiReference
        {
            Type = ReferenceType.SecurityScheme,
            Id = "Bearer"
        }
    };

    options.AddSecurityDefinition("Bearer", securitySchema);

    var securityRequirement = new OpenApiSecurityRequirement
                {
                    { securitySchema, new[] { "Bearer" } }
                };

    options.AddSecurityRequirement(securityRequirement);
});
builder.Services.AddEndpointsApiExplorer();

#region DbContext
builder.Services.AddDbContext<ApplicationDbContext>(options =>
{
    //NOTE: You can use UseSqlServer instead
    options.UseSqlite("DataSource=EasyIdentity.db");
});
#endregion

#region Identity
builder.Services.AddIdentity<ApplicationUser, IdentityRole>()
    .AddEntityFrameworkStores<ApplicationDbContext>()
    .AddDefaultTokenProviders();

builder.Services.AddScoped<IUserService, UserService>();
builder.Services.AddSingleton<ITokenService, TokenService>();
#endregion

#region Jwt Bearer
var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(builder.Configuration["TokenKey"]
    ?? throw new InvalidOperationException("The Token Key not found!")));

builder.Services
    .AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
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
#endregion

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

#region Swagger
app.UseSwagger();
app.UseSwaggerUI();
#endregion

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
            Email = "admin@gmail.com"
        };
        var result = await userManager.CreateAsync(admin, "P@ssw0rd");
    }
    // ---
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();
