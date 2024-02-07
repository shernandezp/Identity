using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using OpenIddict.Server.AspNetCore;
using Security.Infrastructure;
using Security.Web;
using Security.Web.Endpoints;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();

builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
                .AddCookie(CookieAuthenticationDefaults.AuthenticationScheme,
                    options => options.LoginPath = "/login");

// Add services to the container.
builder.Services.AddApplicationServices();
builder.Services.AddApplicationDbContext(builder.Configuration);
builder.Services.AddOpenIdDictDbContext(builder.Configuration);
builder.Services.AddOpenIdDictServices(builder.Configuration);

builder.Services.AddRazorPages();
builder.Services.AddHostedService<ClientSeeder>();

// Add HealthChecks
builder.Services.AddHealthChecks()
            .AddDbContextCheck<SecurityDbContext>();

//Register Handlers
builder.Services.AddScoped<AuthorizationHandler>();
builder.Services.AddScoped<TokenHandler>();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHealthChecks("/health");
app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();
app.UseAuthentication();
app.UseAuthorization();

app.UseExceptionHandler(options => { });

app.MapGet("~/authorize", async (HttpContext context) =>
{
    var tokenHandler = context.RequestServices.GetRequiredService<AuthorizationHandler>();
    await tokenHandler.Authorize(context);
});

app.MapPost("~/token", async (HttpContext context) =>
{
    var tokenHandler = context.RequestServices.GetRequiredService<TokenHandler>();
    return await tokenHandler.Exchange(context);
});

app.MapGet("~/logout", async (HttpContext context) =>
{
    await context.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
    await context.SignOutAsync(OpenIddictServerAspNetCoreDefaults.AuthenticationScheme, new AuthenticationProperties
    {
        RedirectUri = "/Home"
    });
    context.Response.Redirect("/Home");
});

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Login}/{action=Index}/{id?}");

app.Run();
