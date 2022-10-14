using Duende.IdentityServer.AspNetIdentity;
using FIT5032_Assignment_Portfolio.App_Start;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Localization;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Localization;
using System.Security.Claims;
using Microsoft.AspNetCore.SpaServices;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.SpaServices.ReactDevelopmentServer;

var builder = WebApplication.CreateBuilder(args);


IConfiguration Configuration = new ConfigurationBuilder()
        .AddJsonFile("appsettings.json", optional: false)
        .Build();

builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(
        Configuration.GetConnectionString("DefaultConnection")));

builder.Services.AddDefaultIdentity<ApplicationUser>().AddRoles<IdentityRole>()
    .AddEntityFrameworkStores<ApplicationDbContext>();

builder.Services.AddIdentityServer()
    .AddApiAuthorization<ApplicationUser, ApplicationDbContext>();
//.AddProfileService<ProfileService>();

builder.Services.Configure<IdentityOptions>(options =>
{
    // Password settings
    options.Password.RequireDigit = false;
    options.Password.RequiredLength = 1;
    options.Password.RequireNonAlphanumeric = false;
    options.Password.RequireUppercase = false;
    options.Password.RequireLowercase = false;
    options.Password.RequiredUniqueChars = 1;

    // Lockout settings
    options.Lockout.DefaultLockoutTimeSpan = TimeSpan.FromMinutes(30);
    options.Lockout.MaxFailedAccessAttempts = 10;
    options.Lockout.AllowedForNewUsers = true;

    // User settings
    options.User.RequireUniqueEmail = true;
});

//builder.Services.AddAuthentication()
//    .AddIdentityServerJwt();

builder.Services.AddAuthorization(options =>
{
    options.AddPolicy("IsAdmin",
        policy =>
        {
            // Even though we are using JwtClaimTypes in the ProfileService of the IdentityServer
            // the actual user claims are converted to those in ClaimTypes so check for them here
            policy.RequireClaim(ClaimTypes.Role, "administrator");
        });
});

builder.Services.AddControllersWithViews();
builder.Services.AddRazorPages();

// In production, the React files will be served from this directory
//builder.Services.AddSpaStaticFiles(configuration =>
//{
//    configuration.RootPath = "ClientApp/build";
//});

//builder.Services.AddAutoMapper(typeof(Startup));

//builder.Services.AddSingleton<IStringLocalizer>((ctx) =>
//{
//    IStringLocalizerFactory factory = ctx.GetService<IStringLocalizerFactory>();
//    return factory.Create(typeof(SharedResources));
//});

builder.Services.AddLocalization(options => options.ResourcesPath = "Resources");

builder.Services.Configure<RequestLocalizationOptions>(options =>
{
    // State what the default culture for your application is. This will be used if no specific culture
    // can be determined for a given request.
    //options.DefaultRequestCulture = new RequestCulture(culture: Cultures.DefaultCulture, uiCulture: Cultures.DefaultCulture);

    // You must explicitly state which cultures your application supports.
    // These are the cultures the app supports for formatting numbers, dates, etc.
    //options.SupportedCultures = Cultures.SupportedCultures;

    // These are the cultures the app supports for UI strings, i.e. we have localized resources for.
    //options.SupportedUICultures = Cultures.SupportedCultures;

    // You can change which providers are configured to determine the culture for requests, or even add a custom
    // provider with your own logic. The providers will be asked in order to provide a culture for each request,
    // and the first to provide a non-null result that is in the configured supported cultures list will be used.
    // By default, the following built-in providers are configured:
    // - QueryStringRequestCultureProvider, sets culture via "culture" and "ui-culture" query string values, useful for testing
    // - CookieRequestCultureProvider, sets culture via "ASPNET_CULTURE" cookie
    // - AcceptLanguageHeaderRequestCultureProvider, sets culture via the "Accept-Language" request header

    // Remove AcceptLanguageHeaderRequestCultureProvider to eliminate warning in the log files
    options.RequestCultureProviders = new List<IRequestCultureProvider>
                {
                    new QueryStringRequestCultureProvider(),
                    new CookieRequestCultureProvider()
                };

    //options.RequestCultureProviders.Insert(0, new CustomRequestCultureProvider(async context =>
    //{
    //  // My custom request culture logic
    //  return new ProviderCultureResult("en");
    //}));
});

builder.Services.AddScoped<IUserManagementService, UserManagementService>();
builder.Services.AddScoped<IHelperService, HelperService>();

// Add builder.Services to the container.

builder.Services.AddControllersWithViews();

var app = builder.Build();



if (app.Environment.IsDevelopment())
{
    app.UseDeveloperExceptionPage();
    app.UseDatabaseErrorPage();
}
else
{
    app.UseExceptionHandler("/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseRequestLocalization();
app.UseHttpsRedirection();
app.UseStaticFiles();
//app.UseSpaStaticFiles();

app.UseRouting();

app.UseAuthentication();
app.UseIdentityServer();
app.UseAuthorization();
app.UseEndpoints(endpoints =>
{
    //endpoints.Map("Identity/Account/Manage/ChangePassword", async context =>
    //{
    //	context.Response.StatusCode = 300;
    //	await context.Response.CompleteAsync();
    //});
    endpoints.MapControllerRoute(
        name: "default",
        pattern: "{controller}/{action=Index}/{id?}");
    endpoints.MapRazorPages();
});

app.UseSpa(spa =>
{
    spa.Options.SourcePath = "ClientApp";

    if (app.Environment.IsDevelopment())
    {
        spa.UseReactDevelopmentServer(npmScript: "start");
    }
});


// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

//app.UseHttpsRedirection();
//app.UseStaticFiles();
//app.UseRouting();


//app.MapControllerRoute(
//    name: "default",
//    pattern: "{controller}/{action=Index}/{id?}");

app.MapFallbackToFile("index.html"); ;

app.Run();
