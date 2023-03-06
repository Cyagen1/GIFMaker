using GIFMaker.Configuration;
using GIFMaker.Contracts;
using GIFMaker.Services;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// Configuration
builder.Services.AddSingleton(builder.Configuration.GetSection(nameof(FileStorageGifRepositorySettings)).Get<FileStorageGifRepositorySettings>());
builder.Services.AddSingleton(builder.Configuration.GetSection(nameof(GifGeneratorSettings)).Get<GifGeneratorSettings>());


// Services
builder.Services.AddScoped<IGifGenerator, MagickGifGenerator>();
builder.Services.AddScoped<IGifRepository, FileStorageGifRepository>();

// Caching
builder.Services.AddResponseCaching();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseResponseCaching();

app.UseAuthorization();

app.MapControllers();

app.Run();
