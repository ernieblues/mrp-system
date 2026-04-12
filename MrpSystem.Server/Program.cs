using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using MrpSystem.Server.Authorization;
using MrpSystem.Server.Data;
using MrpSystem.Server.Models;
using System.Text.Json.Serialization;

var builder = WebApplication.CreateBuilder(args);

// --------------------------------------------------
//          Add services to the container
// --------------------------------------------------

// Database context and Identity
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection")
    ?? throw new InvalidOperationException("Connection string 'DefaultConnection' not found.");
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(connectionString));
builder.Services.AddDatabaseDeveloperPageExceptionFilter();

// Identity (users, roles, tokens)
builder.Services.AddIdentity<ApplicationUser, ApplicationRole>(options =>
    options.SignIn.RequireConfirmedAccount = true)
    .AddEntityFrameworkStores<ApplicationDbContext>()
    .AddDefaultTokenProviders();

// Identity: override default cookie redirects to return proper API status codes
builder.Services.ConfigureApplicationCookie(options =>
{
    options.Events.OnRedirectToLogin = context =>
    {
        context.Response.StatusCode = 401; // Not logged in
        return Task.CompletedTask;
    };
    options.Events.OnRedirectToAccessDenied = context =>
    {
        context.Response.StatusCode = 403; // Forbidden
        return Task.CompletedTask;
    };
});

// Authorization handlers
builder.Services.AddScoped<IAuthorizationHandler, SystemAdministratorAuthorizationHandler>();
builder.Services.AddScoped<IAuthorizationHandler, PurchaseRequisitionAuthorizationHandler>();

// Controllers & API support
builder.Services.AddControllers()
    .AddJsonOptions(options =>
    {
        options.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter());
    });
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// --------------------------------------------------
//        Configure the HTTP request pipeline
// --------------------------------------------------

// Static files
app.UseDefaultFiles();
app.MapStaticAssets();

// Development tools
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

// Core middleware
app.UseHttpsRedirection();
app.UseAuthentication();
app.UseAuthorization();

// Endpoints
app.MapControllers();
app.MapFallbackToFile("/index.html");

// --------------------------------------------------
//      Apply pending migrations and seed data
// --------------------------------------------------
using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;
    var context = services.GetRequiredService<ApplicationDbContext>();
    context.Database.Migrate();

    var seedUserPw = builder.Configuration.GetValue<string>("SeedUserPW")
        ?? throw new InvalidOperationException("SeedUserPW not configured.");
    await SeedData.Initialize(services, seedUserPw);
}

app.Run();
