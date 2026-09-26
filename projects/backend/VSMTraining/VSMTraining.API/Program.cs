using Microsoft.EntityFrameworkCore;
using VSMTraining.API.Data;
using VSMTraining.API.Endpoints;
using VSMTraining.Infrastructure.Persistence;
using VSMTraining.Infrastructure.Runtime;
using Swashbuckle.AspNetCore.Swagger;
using VSMTraining.Infrastructure.Auth;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("Default")));

builder.Services.AddScoped<AttemptFlowService>();

builder.Services.Configure<JwtOptions>(builder.Configuration.GetSection(JwtOptions.SectionName));
builder.Services.AddScoped<PasswordHasherService>();
builder.Services.AddScoped<JwtTokenService>();
builder.Services.AddScoped<AuthCookieService>();

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.Configure<SwaggerOptions>(options => options.SerializeAsV2 = true);

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();

    using var scope = app.Services.CreateScope();
    var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
    await db.Database.MigrateAsync();
    await DemoDataSeeder.SeedAsync(db);
}

app.UseHttpsRedirection();

app.MapAttemptEndpoints();
app.MapUserEndpoints();

app.Run();
