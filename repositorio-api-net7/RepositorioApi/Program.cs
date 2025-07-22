using Application.DTO;
using Domain.Entities;
using Domain.Interfaces;
using Infrastructure.Services;

var builder = WebApplication.CreateBuilder(args);

//serviços
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

//middleware de requisição
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



// GET Para requisição dos repositórios
app.MapGet("/repos/relevantes", async (string nome, IRepositorioService service) =>
{
    // O serviço agora retorna uma lista de RepositorioResponseDTO já ordenada
    var repositoriosDto = await service.ListarRepositoriosDoUsuario(nome);
    return Results.Ok(repositoriosDto);
});

//POST para adicionar favoritos
app.MapPost("/favoritos", async (AdicionarFavoritoDTO favoritoDto, IRepositorioService service) =>
{
    Console.WriteLine("➡️ POST recebido:");
    Console.WriteLine($"Nome: {favoritoDto.Nome}, Url: {favoritoDto.Url}");
    await service.AdicionarFavorito(favoritoDto);
    return Results.Ok(new { message = "Favorito adicionado." });
});

//Get para listar favoritos
app.MapGet("/favoritos", async (IRepositorioService service) =>
{
    var favoritosDto = await service.ListarFavoritos();
    return Results.Ok(favoritosDto);
});

app.Run();