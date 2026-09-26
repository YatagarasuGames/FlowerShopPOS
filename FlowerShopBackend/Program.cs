using FlowerShop.Application.Interfaces;
using FlowerShop.Application.Services;
using FlowerShop.Domain.Interfaces;
using FlowerShop.Infrastructure.Data;
using FlowerShop.Infrastructure.Repositories;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.HttpOverrides;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi;
using Npgsql;
using System.Text;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddControllers();

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var connectionString = builder.Configuration.GetConnectionString("DbConnection");
var dataSourceBuilder = new NpgsqlDataSourceBuilder(connectionString);
var dataSource = dataSourceBuilder.Build();

builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowVueClient", policy =>
    {
        policy.WithOrigins("http://localhost:5173", "http://127.0.0.1:5173")
              .AllowAnyHeader()
              .AllowAnyMethod()
              .AllowCredentials();
    });
});

builder.Services.AddDbContext<FlowersDbContext>(options => options.UseNpgsql(dataSource));


var jwtSettings = builder.Configuration.GetSection("JwtSettings");
var secretKey = Encoding.UTF8.GetBytes(jwtSettings["Secret"]!);
builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
})
.AddJwtBearer(options =>
{
    var jwtSettings = builder.Configuration.GetSection("JwtSettings");
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuer = true,
        ValidateAudience = true,
        ValidateLifetime = true,
        ValidateIssuerSigningKey = true,
        ValidIssuer = jwtSettings["Issuer"],
        ValidAudience = jwtSettings["Audience"],
        IssuerSigningKey = new SymmetricSecurityKey(
            Encoding.UTF8.GetBytes(jwtSettings["Secret"]!))
    };
});
builder.Services.AddAuthorization();

builder.Services.AddScoped<IUnitOfWork, UnitOfWork>();
builder.Services.AddSingleton<IPasswordHasher, PasswordHasher>();
builder.Services.AddScoped<AuthService>();
builder.Services.AddScoped<ProductsService>();
builder.Services.AddScoped<OrdersService>();
builder.Services.AddScoped<WriteOffsService>();
builder.Services.AddScoped<SuppliesService>();
builder.Services.AddScoped<AnalyticsService>();

builder.Services.AddScoped<AuditService>();
builder.Services.AddScoped<UsersService>();

builder.Services.AddHttpContextAccessor();

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
var app = builder.Build();

using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;
    var dbContext = services.GetRequiredService<FlowersDbContext>();
    var hasher = services.GetRequiredService<IPasswordHasher>();

    await dbContext.Database.MigrateAsync();
    await DatabaseSeeder.SeedAsync(dbContext, hasher);
}


if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}
app.UseForwardedHeaders(new ForwardedHeadersOptions
{
    ForwardedHeaders = ForwardedHeaders.XForwardedFor | ForwardedHeaders.XForwardedProto
});
app.UseStaticFiles();
app.UseCors("AllowVueClient");
app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();


app.Run();
