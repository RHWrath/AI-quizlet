using Services; // Zorg dat dit erbij staat
using AI_quizelet_API.Seeding;
using AI_quizelet_API.Service; // Zorg dat dit erbij staat

var builder = WebApplication.CreateBuilder(args);

// Voeg controllers toe
builder.Services.AddControllers();

// Voeg MongoDbSettings toe als singleton
builder.Services.AddSingleton(new MongoDbSettings()); // default instellingen uit Services/MongoDbSettings.cs

// Voeg je services toe via DI
builder.Services.AddScoped<ImageService>();
builder.Services.AddScoped<MusicService>();
builder.Services.AddScoped<PlayerService>();

// Add seeder services
builder.Services.AddTransient<ImageSeeder>();
builder.Services.AddHostedService<SeederHostedService>();

// Swagger/OpenAPI
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

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
