var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

var summaries = new[]
{
    "Freezing", "Bracing", "Chilly", "Cool", "Mild", "Warm", "Balmy", "Hot", "Sweltering", "Scorching"
};

app.MapGet("/weatherforecast", () =>
{
    var forecast = Enumerable.Range(1, 5).Select(index =>
        new WeatherForecast
        (
            DateOnly.FromDateTime(DateTime.Now.AddDays(index)),
            Random.Shared.Next(-20, 55),
            summaries[Random.Shared.Next(summaries.Length)]
        ))
        .ToArray();
    return forecast;
})
.WithName("GetWeatherForecast")
.WithOpenApi();

//app.MapPost("/upload", async (IFormFile arquivo) =>
//{
//    await arquivo.CopyToAsync(File.OpenWrite($@"{DateTime.Now.Ticks}.txt"));
//});

app.MapPost("/uploadArquivo", async (IFormFile arquivo) =>
{
    string tempfile = CreateTempfilePath();
    using var stream = File.OpenWrite(tempfile);
    await arquivo.CopyToAsync(stream);

    return Results.Ok("Arquivo enviado com sucesso");

    //await arquivo.CopyToAsync(File.OpenWrite($@"{DateTime.Now.Ticks}.txt"));
});

app.MapPost("/uploadArquivos", async (IFormFileCollection arquivos) =>
{
    foreach(var file in arquivos)
    {
        string tempfile = CreateTempfilePath();
        using var stream = File.OpenWrite(tempfile);
        await file.CopyToAsync(stream);
    }

    return Results.Ok("Arquivos enviados com sucesso");
});

//criao nome do arquivo usando Ticks do DateTime e criar a pasta temp/uploads no servidor para salvar os arquivos enviados.
static string CreateTempfilePath()
{
    var filename = $@"{DateTime.Now.Ticks}.tmp";
    var directoryPath = Path.Combine("temp", "uploads");

    if (!Directory.Exists(directoryPath))
        Directory.CreateDirectory(directoryPath);

    return Path.Combine(directoryPath, filename);
}

app.Run();

internal record WeatherForecast(DateOnly Date, int TemperatureC, string? Summary)
{
    public int TemperatureF => 32 + (int)(TemperatureC / 0.5556);
}
