using kaufor.Data;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection") ?? throw new InvalidOperationException("Connection string 'DefaultConnection' not found.");
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(connectionString));

// Configure Identity with custom password rules
builder.Services.AddDefaultIdentity<IdentityUser>(options =>
{
    options.Password.RequireDigit = false;            // No numeric character required
    options.Password.RequiredLength = 2;             // Minimum password length
    options.Password.RequireNonAlphanumeric = false; // No special character required
    options.Password.RequireUppercase = false;       // No uppercase letter required
    options.Password.RequireLowercase = false;       // No lowercase letter required
}).AddRoles<IdentityRole>()                           // Add roles to the Identity system
  .AddEntityFrameworkStores<ApplicationDbContext>();

builder.Services.AddControllersWithViews();



// Configure the Login Path for unauthenticated users
builder.Services.ConfigureApplicationCookie(options =>
{
    options.LoginPath = "/Account/Login"; // Redirect to login page
    options.AccessDeniedPath = "/Account/AccessDenied"; // Optional: For unauthorized access
});


var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

// Enable Authentication and Authorization middleware
app.UseAuthentication();
app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");
app.MapRazorPages();

// Role and user setup
using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;

    // Create roles and assign users
    await CreateRolesAndUsers(services);
}

app.Run();

// Method to create roles and assign users
async Task CreateRolesAndUsers(IServiceProvider serviceProvider)
{
    var roleManager = serviceProvider.GetRequiredService<RoleManager<IdentityRole>>();
    var userManager = serviceProvider.GetRequiredService<UserManager<IdentityUser>>();

    // Define role names
    string[] roleNames = { "Admin", "Calisan", "Musteri" };

    // Create roles if they don't exist
    foreach (var roleName in roleNames)
    {
        if (!await roleManager.RoleExistsAsync(roleName))
        {
            await roleManager.CreateAsync(new IdentityRole(roleName));
        }
    }

    // Define specific user accounts
    var adminEmail = "Y245012042@sakarya.edu.tr";
    var calisanEmail = "calisan@sakarya.edu.tr";
    var musteriEmail = "musteri@sakayra.edu.tr";

    // Assign Admin role with password "sau"
    var adminUser = await userManager.FindByEmailAsync(adminEmail);
    if (adminUser == null)
    {
        adminUser = new IdentityUser { UserName = adminEmail, Email = adminEmail };
        await userManager.CreateAsync(adminUser, "sau"); // Admin password
    }
    if (!await userManager.IsInRoleAsync(adminUser, "Admin"))
    {
        await userManager.AddToRoleAsync(adminUser, "Admin");
    }

    // Assign Calisan role with password "123456"
    var calisanUser = await userManager.FindByEmailAsync(calisanEmail);
    if (calisanUser == null)
    {
        calisanUser = new IdentityUser { UserName = calisanEmail, Email = calisanEmail };
        await userManager.CreateAsync(calisanUser, "123456"); // Calisan password
    }
    if (!await userManager.IsInRoleAsync(calisanUser, "Calisan"))
    {
        await userManager.AddToRoleAsync(calisanUser, "Calisan");
    }

    // Assign Musteri role with password "654321"
    var musteriUser = await userManager.FindByEmailAsync(musteriEmail);
    if (musteriUser == null)
    {
        musteriUser = new IdentityUser { UserName = musteriEmail, Email = musteriEmail };
        await userManager.CreateAsync(musteriUser, "654321"); // Musteri password
    }
    if (!await userManager.IsInRoleAsync(musteriUser, "Musteri"))
    {
        await userManager.AddToRoleAsync(musteriUser, "Musteri");
    }
}
