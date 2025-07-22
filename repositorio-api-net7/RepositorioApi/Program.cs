using Domain.Entities;
using Domain.Interfaces;
using Infrastructure.Services;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll", policy =>
    {
        policy.AllowAnyOrigin()
              .AllowAnyMethod()
              .AllowAnyHeader();
    });
});
builder.Services.AddHttpClient();
builder.Services.AddSingleton<IRepositorioService, RepositorioService>();

var app = builder.Build();

app.Use(async (context, next) =>
{
    try
    {
        await next();
    }
    catch (Exception ex)
    {
        var logger = context.RequestServices.GetRequiredService<ILogger<Program>>();
        logger.LogError(ex, "Erro não tratado");

        context.Response.StatusCode = 500;
        await context.Response.WriteAsJsonAsync(new { error = "Erro interno no servidor." });
    }
});
app.UseSwagger();
app.UseSwaggerUI();
app.UseCors("AllowAll");

//requisicao para buscar repositorios, ordena por relevancia
app.MapGet("/repos/relevantes", async (string nome, IRepositorioService service) =>
{
    var repos = await service.ListarRepositoriosDoUsuario(nome);
    var ordenados = repos
        .OrderByDescending(r => r.Relevancia)
        .ToList();

    return Results.Ok(ordenados);
});

//requisicao para adicionar aos favoritos
app.MapPost("/favoritos", async (Favorito favorito, IRepositorioService service) =>
{
    Console.WriteLine("➡️ POST recebido:");
    Console.WriteLine($"Nome: {favorito.Nome}, Url: {favorito.Url}");
    await service.AdicionarFavorito(favorito);
    return Results.Ok(new { message = "Favorito adicionado." });
});

//requisicao para listar favoritos
app.MapGet("/favoritos", async (IRepositorioService service) =>
{
    var favoritos = await service.ListarFavoritos();
    return Results.Ok(favoritos);
});

app.Run();
