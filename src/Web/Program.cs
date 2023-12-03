using Security.Infrastructure.Data;
using Security.Web;
using Security.Web.Endpoints;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddApplicationServices();
builder.Services.AddApplicationDbContext(builder.Configuration);
builder.Services.AddOpenIdDictDbContext(builder.Configuration);
builder.Services.AddOpenIdDictServices();

builder.Services.AddHostedService<ClientSeeder>();

// Add HealthChecks
builder.Services.AddHealthChecks()
            .AddDbContextCheck<SecurityDbContext>();

//Register TokenHandler
builder.Services.AddScoped<TokenHandler>();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    await app.InitialiseDatabaseAsync();
}
else
{
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHealthChecks("/health");
app.UseHttpsRedirection();

app.UseAuthentication();

app.UseExceptionHandler(options => { });

app.MapPost("~/token", async (HttpContext context) =>
{
    var tokenHandler = context.RequestServices.GetRequiredService<TokenHandler>();
    return await tokenHandler.Exchange(context);
});

app.Run();

public partial class Program { }
