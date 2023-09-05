using FluentValidation;
using Microsoft.EntityFrameworkCore;
using OpenCart.Api.Configuration;
using OpenCart.Infrastructure.Context;
using OpenCart.Models.DTOs;
using OpenCart.Models.Entities;
using OpenCart.Models.Models;
using OpenCart.Repositories.Repositories.CartItemImageRepository;
using OpenCart.Repositories.Repositories.CartItemRepository;
using OpenCart.Repositories.Repositories.UserRepository;
using OpenCart.Services.Services.CartService;
using OpenCart.Services.Services.SettingsService;
using OpenCart.Services.Services.UserService;
using OpenCart.Services.Services.Validators;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddAuthentication();

var configuration = new ConfigurationBuilder()
        .SetBasePath(Directory.GetCurrentDirectory())
        .AddJsonFile("appsettings.json", false)
        .Build();

var openCartServiceSetings = configuration.GetSection("OpenCartSettings")
    .Get<OpenCartServiceSettings>();
builder.Services.AddSingleton<IOpenCartServiceSettings>(x => openCartServiceSetings);

builder.Services.AddHttpContextAccessor();
builder.Services.AddDbContext<OpenCartDbContext>(options => options.UseSqlServer(openCartServiceSetings.ConnectionString));
builder.Services.AddScoped<IValidator<CartItemImageDto>, CartImageValidator>();
builder.Services.AddScoped<IValidator<CartItemDto>, CartValidator>();
builder.Services.AddScoped<IValidator<ApplicationUser>, UserValidator>();
builder.Services.AddScoped<ICartItemImageRepository, CartItemImageRepository>();
builder.Services.AddScoped<ICartItemRepository, CartItemRepository>();
builder.Services.AddScoped<IUserRepository, UserRepository>();
builder.Services.AddScoped<IUserService, UserService>();
builder.Services.AddScoped<ICartService, CartService>();

builder.Services.ConfigureAuthentication(openCartServiceSetings);

var app = builder.Build();

app.UseHttpLogging();

app.UseCors();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

using (var scope = app.Services.CreateScope())
{
    var dbContext = scope.ServiceProvider.GetRequiredService<OpenCartDbContext>();
    dbContext.Database.Migrate();
}
app.UseHttpsRedirection();
app.UseAuthentication();

app.UseAuthorization();

app.MapControllers();

app.Run();
