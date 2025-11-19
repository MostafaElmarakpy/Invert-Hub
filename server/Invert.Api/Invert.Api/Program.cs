
using Invert.Api.Data;
using Invert.Api.Entities;
using Invert.Api.Mappings;
using Invert.Api.Repositories.Implementation;
using Invert.Api.Repositories.Interface;
using Invert.Api.Services.Implementation;
using Invert.Api.Services.Interface;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.FileProviders;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddOpenApi();
builder.Services.AddSwaggerGen();

builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo
    {
        Title = "Dummy App",
        Version = "v1"
    });
    c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme()
    {
        Name = "Authorization",
        Type = SecuritySchemeType.ApiKey,
        Scheme = "Bearer",
        BearerFormat = "JWT",
        In = ParameterLocation.Header,
        Description = "Enter 'Bearer' [space] and then your valid token.\r\nExample: \"Bearer eyJhbGciOiJIUzI1NiIsInR5cCI...\""
    });
    c.AddSecurityRequirement(new OpenApiSecurityRequirement
{
    {
        new OpenApiSecurityScheme
        {
            Reference = new OpenApiReference
            {
                Type = ReferenceType.SecurityScheme,
                Id = "Bearer"
            }
        },
        new string[] {}
    }
});
});

// ✅ Configure Cloudinary Settings
// builder.Services.Configure<CloudinarySettings>(
//     builder.Configuration.GetSection("CloudinarySettings"));

// // ✅ Register Cloudinary as Singleton
//builder.Services.AddSingleton<Cloudinary>(sp =>
//{
//    var settings = sp.GetRequiredService<IOptions<CloudinarySettings>>().Value;

//    var account = new Account(
//        settings.CloudName,
//        settings.ApiKey,
//        settings.ApiSecret
//    );

//    return new Cloudinary(account);
//});

builder.Services.AddMemoryCache();

builder.Services.AddLogging(config =>
{
    config.AddConsole();
    config.AddDebug();
});

builder.Services.AddDbContext<ApplicationDbContext>(option =>
option.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

// builder.Services.AddAutoMapper(typeof(AutoMapperProfile));

builder.Services.AddAutoMapper(typeof(AutoMapperProfile).Assembly);
builder.Services.AddScoped<IUnitOfWork, UnitOfWork>();
builder.Services.AddScoped(typeof(IGenaricRepository<>), typeof(GenaricRepository<>));
builder.Services.AddScoped<IProjectRepository, ProjectRepository>();
builder.Services.AddScoped<IUserRepository, UserRepository>();
builder.Services.AddScoped<IProjectService, ProjectService>();
builder.Services.AddScoped<IAuthService, AuthService>();
builder.Services.AddScoped<IArticleRepository, ArticleRepository>();
builder.Services.AddScoped<IArticleService, ArticleService>();
builder.Services.AddScoped<IJobRepository, JobRepository>();
builder.Services.AddScoped<IJobService, JobService>();
builder.Services.AddScoped<IAdminService, AdminService>();
builder.Services.AddScoped<IUserService, UserService>();

builder.Services.AddScoped<INotificationService, NotificationService>();

// Identity (ensure AppUser type and ApplicationDbContext are correct)
builder.Services.AddIdentity<AppUser, IdentityRole>(options =>
{
    options.Password.RequireDigit = true;
    options.Password.RequiredLength = 8;
})
.AddEntityFrameworkStores<ApplicationDbContext>()
.AddDefaultTokenProviders();



builder.Services.AddCors(options =>
  options.AddPolicy("AllowReactDev", policy =>
    policy.WithOrigins("http://localhost:3000", "https://localhost:3000")
          .AllowAnyHeader()
          .AllowAnyMethod()
          .AllowCredentials()
  )
);

// Configure JWT auth
var jwtCfg = builder.Configuration.GetSection("Jwt");
var key = Encoding.UTF8.GetBytes(jwtCfg["Key"] ?? throw new InvalidOperationException("Jwt:Key missing"));

builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
})

.AddJwtBearer(options =>
{
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuer = true,
        ValidateAudience = true,
        ValidateIssuerSigningKey = true,
        ValidIssuer = jwtCfg["Issuer"],
        ValidAudience = jwtCfg["Audience"],
        IssuerSigningKey = new SymmetricSecurityKey(key),
        ClockSkew = TimeSpan.FromMinutes(1)
    };
});

builder.Services.AddControllers();

var app = builder.Build();

// app.UseMiddleware<ExceptionMiddleware>();
// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseStaticFiles(new StaticFileOptions
{
    FileProvider = new PhysicalFileProvider(
        Path.Combine(builder.Environment.ContentRootPath, "wwwroot", "uploads")),
    RequestPath = "/uploads"
});

app.UseRouting();

app.UseCors("AllowReactDev");
app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();
