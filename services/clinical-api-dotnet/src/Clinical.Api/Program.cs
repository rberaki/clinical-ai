using Clinical.Application;
using Clinical.Infrastructure.Persistence;
using MediatR;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddMediatR(typeof(AssemblyMarker).Assembly);

builder.Services.AddDbContext<ClinicalDbContext>(options =>
{
    var connectionString =
        builder.Configuration.GetConnectionString("ClinicalDb")
        ?? builder.Configuration["ConnectionStrings:ClinicalDb"]
        ?? throw new InvalidOperationException("Connection string 'ClinicalDb' is missing.");

    options.UseNpgsql(connectionString);
});

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.UseAuthorization();
app.MapControllers();

app.Run();
