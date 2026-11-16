using Microsoft.EntityFrameworkCore;
using HabitTracker.Domain;
using HabitTracker.Domain.Repositories;
using HabitTracker.Infrastructure;
using HabitTracker.Infrastructure.Repositories;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
//piesledz sql
builder.Services.AddDbContext<AppDbContext>(options => options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));
builder.Services.AddScoped<IHabitRepository, EfHabitRepository>();

builder.Services.AddOpenApi();

var app = builder.Build();
using (var scope = app.Services.CreateScope()) // pievieno sakotnejos testa datus no migracijam
{
    var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
    db.Database.Migrate();
}
using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
    db.Database.Migrate();

    if (!db.Habits.Any())
    {
        var initialHabits = TestDataGenerator.GenerateHabits();
        db.Habits.AddRange(initialHabits);
        db.SaveChanges();
    }
}

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

//app.UseHttpsRedirection(); lai lietotne lieto portu 5294
app.UseAuthorization();
app.MapControllers();

app.Run();