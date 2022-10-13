using System.Net;
using BlogNetCore.Authorization;
using BlogNetCore.Config;
using BlogNetCore.Data;
using BlogNetCore.Models;
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

    builder.Services.AddRouting(options =>
    {
        options.LowercaseUrls = true;
        options.LowercaseQueryStrings = true;
    });
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
            options.Events.OnRedirectToLogin = async (context) =>
            {
                await Results.Problem(
                        title: "Unauthorized request.",
                        statusCode: (int)HttpStatusCode.Unauthorized)
                    .ExecuteAsync(context.HttpContext);
            };
            options.Events.OnRedirectToAccessDenied = async (context) =>
            {
                await Results.Problem(
                        title: "Access denied.",
                        statusCode: (int)HttpStatusCode.Forbidden)
                    .ExecuteAsync(context.HttpContext);
            };
        });
    builder.Services.AddAuthorization((options) =>
    {
        var requireAuthenticatedPolicy = new AuthorizationPolicyBuilder()
            .RequireAuthenticatedUser()
            .Build();
        options.DefaultPolicy = requireAuthenticatedPolicy;
        options.FallbackPolicy = requireAuthenticatedPolicy;
    });
    builder.Services.AddSingleton<IAuthorizationHandler, ArticleAuthorizationHandler>();
    builder.Services.AddSingleton<IPasswordHasher<User>, PasswordHasher<User>>();
    builder.Services.AddAutoMapper(typeof(AutoMapperProfile).Assembly);
    
    // Services
    builder.Services.AddScoped<IUserService, UserService>();
    builder.Services.AddScoped<ITagService, TagService>();
    builder.Services.AddScoped<IArticleService, ArticleService>();

    var app = builder.Build();
    
    app.UseExceptionHandler("/error");

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