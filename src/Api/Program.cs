using System.Reflection;
using Api.Authorization;
using Api.Data;
using Api.Models;
using Api.Services;
using FluentValidation;
using FluentValidation.AspNetCore;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Serilog;

Log.Logger = new LoggerConfiguration()
    .WriteTo.Console()
    .CreateBootstrapLogger();

try
{
    var builder = WebApplication.CreateBuilder(args);
    
    builder.Host.UseSerilog((context, services, config) =>
    {
        config.ReadFrom.Configuration(context.Configuration);
        config.ReadFrom.Services(services);
    });

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
        options.UseSqlServer(builder.Configuration.GetConnectionString("Database"));
    });
    builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
        .AddCookie(options =>
        {
            options.Cookie.Name = "sid";
            options.Cookie.HttpOnly = builder.Environment.IsProduction();
            options.LoginPath = "/api/account/unauthorized";
            options.AccessDeniedPath = "/api/account/forbidden";
        });
    builder.Services.AddAuthorization();
    builder.Services.AddAutoMapper(typeof(Program));
    builder.Services.AddValidatorsFromAssembly(Assembly.GetExecutingAssembly());
    
    builder.Services.AddSingleton<IAuthorizationHandler, ArticleAuthorizationHandler>();
    builder.Services.AddSingleton<IPasswordHasher<User>, PasswordHasher<User>>();
    
    builder.Services.AddScoped<IUserService, UserService>();
    builder.Services.AddScoped<ITagService, TagService>();
    builder.Services.AddScoped<IArticleService, ArticleService>();

    var app = builder.Build();
    
    app.UseExceptionHandler("/error");
    if (app.Environment.IsDevelopment())
    {
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