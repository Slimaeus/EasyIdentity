using EasyIdentity.Core.Data;
using EasyIdentity.Core.Services;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllersWithViews();

#region DbContext
builder.Services.AddDbContext<ApplicationDbContext>(options =>
{
    //NOTE: You can use UseSqlServer instead
    options.UseSqlite("DataSource=EasyIdentity.db");
});
#endregion

#region Identity
builder.Services.AddIdentity<IdentityUser, IdentityRole>()
    .AddEntityFrameworkStores<ApplicationDbContext>()
    .AddDefaultTokenProviders();

builder.Services.AddScoped<IUserService, UserService>();
#endregion

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

if (app.Environment.IsDevelopment())
{
    using var scope = app.Services.CreateScope();
    var context = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
    var userManager = scope.ServiceProvider.GetRequiredService<UserManager<IdentityUser>>();
    await context.Database.MigrateAsync();

    if (!context.Users.Any(x => x.UserName == "admin"))
    {
        var admin = new IdentityUser
        {
            UserName = "admin",
            Email = "admin@gmail.com"
        };
        var result = await userManager.CreateAsync(admin, "P@ssw0rd");
    }
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();
