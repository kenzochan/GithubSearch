using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Text.Json;
using Domain.Entities;
using Domain.Interfaces;
using System.Net.Http.Json;
using Microsoft.Extensions.Logging;

namespace Infrastructure.Services
{
    // Classe que implementa a interface IRepositorioService.
    // Responsável por interagir com a API do GitHub e gerenciar os favoritos.
    public class RepositorioService : IRepositorioService
    {
        private readonly List<Favorito> _favoritos = new();
        private readonly HttpClient _httpClient;
        private readonly ILogger<RepositorioService> _logger;

        public RepositorioService(HttpClient httpClient, ILogger<RepositorioService> logger)
        {
            _httpClient = httpClient;
            _httpClient.DefaultRequestHeaders.UserAgent.ParseAdd("request");
            _logger = logger;
        }

        public async Task<List<Repositorio>> BuscarRepositorios(string termo)
        {
            _logger.LogInformation("Buscando repositórios para o termo: {termo}", termo);

            try
            {
                var response = await _httpClient.GetFromJsonAsync<JsonDocument>(
                    $"https://api.github.com/search/repositories?q={termo}");

                if (response == null)
                {
                    _logger.LogWarning("Resposta nula da API do GitHub para o termo: {termo}", termo);
                    return new List<Repositorio>();
                }

                var repos = response.RootElement.GetProperty("items").EnumerateArray()
                    .Select(repo => new Repositorio
                    {
                        Nome = repo.GetProperty("name").GetString(),
                        Url = repo.GetProperty("html_url").GetString(),
                        Estrelas = repo.GetProperty("stargazers_count").GetInt32(),
                        Forks = repo.GetProperty("forks_count").GetInt32(),
                        Watchers = repo.GetProperty("watchers_count").GetInt32()
                    }).ToList();

                return repos;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao buscar repositórios para o termo: {termo}", termo);
                throw;
            }
        }

        public async Task<List<Repositorio>> ListarRepositoriosDoUsuario(string usuario)
        {
            _logger.LogInformation("Listando repositórios para o usuário: {usuario}", usuario);

            try
            {
                _httpClient.DefaultRequestHeaders.UserAgent.ParseAdd("request");

                var response = await _httpClient.GetFromJsonAsync<List<JsonElement>>(
                    $"https://api.github.com/users/{usuario}/repos?per_page=100");

                if (response == null)
                {
                    _logger.LogWarning("Resposta nula da API do GitHub para o usuário: {usuario}", usuario);
                    return new List<Repositorio>();
                }

                var repos = response.Select(repo => new Repositorio
                {
                    Nome = repo.GetProperty("name").GetString(),
                    Url = repo.GetProperty("html_url").GetString(),
                    Estrelas = repo.GetProperty("stargazers_count").GetInt32(),
                    Forks = repo.GetProperty("forks_count").GetInt32(),
                    Watchers = repo.GetProperty("watchers_count").GetInt32()
                }).ToList();

                return repos;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao listar repositórios do usuário: {usuario}", usuario);
                throw;
            }
        }

        public async Task AdicionarFavorito(Favorito favorito)
        {
            if (!_favoritos.Any(f => f.Nome == favorito.Nome && f.Url == favorito.Url))
            {
                _favoritos.Add(favorito);
                Console.WriteLine($"✅ Adicionado: {favorito.Nome} - {favorito.Url}");

                Console.WriteLine("📋 Lista atual de favoritos:");
                foreach (var f in _favoritos)
                {
                    Console.WriteLine($"- {f.Nome} ({f.Url})");
                }
            }
            else
            {
                Console.WriteLine("Erro ao adicionar favorito");
            }
                await Task.CompletedTask;
        }

        public async Task<List<Favorito>> ListarFavoritos()
        {
            return await Task.FromResult(_favoritos);
        }
    }
}
