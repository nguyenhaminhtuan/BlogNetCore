using System.Reflection;
using Api.Auth;
using Api.Data;
using Api.Exceptions;
using Api.Models;
using Api.Services;
using FluentValidation;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Serilog;

var configuration = new ConfigurationBuilder()
    .SetBasePath(Directory.GetCurrentDirectory())
    .AddJsonFile("appsettings.json")
    .AddJsonFile($"appsettings.{Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT")}.json", true)
    .Build();

Log.Logger = new LoggerConfiguration()
    .ReadFrom.Configuration(configuration)
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
    builder.Services.AddControllers(options =>
    {
        options.Filters.Add<GlobalExceptionFilter>();
    });
    builder.Services.AddEndpointsApiExplorer();
    builder.Services.AddSwaggerGen(options =>
    {
        options.EnableAnnotations();
        var xmlFilename = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
        options.IncludeXmlComments(Path.Combine(AppContext.BaseDirectory, xmlFilename));
    });
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
            options.EventsType = typeof(CustomCookieAuthenticationEvents);
        });
    builder.Services.AddAuthorization(options =>
    {
        options.AddPolicy(AuthorizationPolicies.ActiveUserOnly, policy =>
            policy.RequireClaim(AdditionalClaimTypes.IsDisabled, bool.FalseString));
        options.AddPolicy(AuthorizationPolicies.VerifiedUserOnly, policy =>
            policy.RequireClaim(AdditionalClaimTypes.EmailVerified, bool.TrueString));
    });
    builder.Services.AddAutoMapper(typeof(Program));
    builder.Services.AddValidatorsFromAssembly(Assembly.GetExecutingAssembly());
    
    builder.Services.AddSingleton<IPasswordHasher<User>, PasswordHasher<User>>();
    builder.Services.AddSingleton<IAuthorizationMiddlewareResultHandler, CustomAuthorizationMiddlewareResultHandler>();
    builder.Services.AddSingleton<IAuthorizationHandler, ArticleAuthorizationHandler>();
    builder.Services.AddScoped<CustomCookieAuthenticationEvents>();

    builder.Services.AddScoped<IUserService, UserService>();
    builder.Services.AddScoped<ITagService, TagService>();
    builder.Services.AddScoped<IArticleService, ArticleService>();
    builder.Services.AddScoped<IEmailService, FakeEmailService>();
    builder.Services.AddScoped<ICommentService, CommentService>();
    builder.Services.AddScoped<IVoteService, VoteService>();

    var app = builder.Build();
    
    app.UseCors();
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