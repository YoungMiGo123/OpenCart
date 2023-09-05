using Microsoft.EntityFrameworkCore;
using OpenCart.Api.Configuration;
using OpenCart.Infrastructure.Context;
using OpenCart.Services.Services.SettingsService;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddAuthentication();
builder.Services.AddHttpContextAccessor();

var configuration = new ConfigurationBuilder()
        .SetBasePath(Directory.GetCurrentDirectory())
        .AddJsonFile("appsettings.json", false)
        .Build();

var openCartServiceSetings = configuration.GetSection("OpenCartSettings")
    .Get<OpenCartServiceSettings>();

builder.Services.AddSingleton<IOpenCartServiceSettings>(x => openCartServiceSetings);
builder.Services.ConfigureAuthentication(openCartServiceSetings);
builder.Services.ConfigureDatabase(openCartServiceSetings);
builder.Services.ConfigureServices(openCartServiceSetings);
builder.ConfigureLogging(openCartServiceSetings);

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
