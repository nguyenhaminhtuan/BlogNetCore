using BlogNetCore.Data;
using BlogNetCore.Models;
using BlogNetCore.Common.Exceptions;
using BlogNetCore.Services;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Serilog;

var builder = WebApplication.CreateBuilder(args);

Log.Logger = new LoggerConfiguration()
    .ReadFrom.Configuration(builder.Configuration)
    .CreateBootstrapLogger();

try
{
    builder.Host.UseSerilog();

    builder.Services.AddControllers();
    builder.Services.AddEndpointsApiExplorer();
    builder.Services.AddSwaggerGen();
    builder.Services.AddDbContext<ApplicationDbContext>(options =>
    {
        options.UseSqlServer(builder.Configuration.GetConnectionString("SqlServer"));
    });
    builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
        .AddCookie(options =>
        {
            options.Cookie.Name = "sid";
            options.Cookie.HttpOnly = builder.Environment.IsProduction();
            options.Events.OnRedirectToLogin = _ => throw new AuthenticationException();
            options.Events.OnRedirectToAccessDenied = _ => throw new AuthorizationException();
        });
    builder.Services.AddAuthorization((options) =>
    {
        var requireAuthenticatedPolicy = new AuthorizationPolicyBuilder()
            .RequireAuthenticatedUser()
            .Build();
        options.DefaultPolicy = requireAuthenticatedPolicy;
        options.FallbackPolicy = requireAuthenticatedPolicy;
    });
    builder.Services.AddSingleton<IPasswordHasher<User>, PasswordHasher<User>>();
    builder.Services.AddScoped<IUserService, UserService>();

    var app = builder.Build();

    if (app.Environment.IsDevelopment())
    {
        using (var scope = app.Services.CreateScope())
        {
            var services = scope.ServiceProvider;
            var context = services.GetRequiredService<ApplicationDbContext>();
            context.Database.Migrate();
        }

        app.UseSwagger();
        app.UseSwaggerUI();
    }

    app.UseAuthentication();
    app.UseAuthorization();
    app.MapControllers();

    app.Run();
}
catch (Exception ex)
{
    Log.Fatal(ex,"An unhandled exception occured during bootstrapping");
}
finally
{
    Log.CloseAndFlush();
}