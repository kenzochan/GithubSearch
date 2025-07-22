using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using System.Net.Http.Json;
using Domain.Entities;
using Domain.Interfaces;
using Application.DTO; // Adicionar o using para os DTOs

namespace Infrastructure.Services
{
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

        //busca repositórios
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

        // lISTA REPOSITORIOS
        public async Task<List<RepositorioResponseDTO>> ListarRepositoriosDoUsuario(string usuario)
        {
            _logger.LogInformation("Listando repositórios para o usuário: {usuario}", usuario);
            try
            {
                var response = await _httpClient.GetFromJsonAsync<List<JsonElement>>(
                    $"https://api.github.com/users/{usuario}/repos?per_page=100");

                if (response == null)
                {
                    _logger.LogWarning("Resposta nula da API do GitHub para o usuário: {usuario}", usuario);
                    return new List<RepositorioResponseDTO>();
                }

                // Mapeia a resposta da API para a entidade do domínio e depois para o DTO
                var repos = response.Select(repo => new Repositorio
                {
                    Nome = repo.GetProperty("name").GetString(),
                    Url = repo.GetProperty("html_url").GetString(),
                    Estrelas = repo.GetProperty("stargazers_count").GetInt32(),
                    Forks = repo.GetProperty("forks_count").GetInt32(),
                    Watchers = repo.GetProperty("watchers_count").GetInt32()
                })
                // Ordena pela relevância
                .OrderByDescending(r => r.Relevancia)
                // Mapeia a entidade para o DTO 
                .Select(r => new RepositorioResponseDTO
                {
                    Nome = r.Nome,
                    Url = r.Url,
                    Estrelas = r.Estrelas,
                    Forks = r.Forks,
                    Watchers = r.Watchers
                }).ToList();

                return repos;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao listar repositórios do usuário: {usuario}", usuario);
                throw;
            }
        }

        //Adiciona um repositório ao favoritos
        public async Task AdicionarFavorito(AdicionarFavoritoDTO favoritoDto)
        {
            var favorito = new Favorito
            {
                Nome = favoritoDto.Nome,
                Url = favoritoDto.Url
            };

            if (!_favoritos.Any(f => f.Nome == favorito.Nome && f.Url == favorito.Url))
            {
                _favoritos.Add(favorito);
                Console.WriteLine($"Adicionado: {favorito.Nome} - {favorito.Url}");
            }
            else
            {
                Console.WriteLine("Erro ao adicionar favorito: item já existe.");
            }
            await Task.CompletedTask;
        }

        // Lista os repositórios favoritos
        public async Task<List<FavoritoDTO>> ListarFavoritos()
        {
            var favoritosDto = _favoritos.Select(f => new FavoritoDTO
            {
                Nome = f.Nome,
                Url = f.Url
            }).ToList();

            return await Task.FromResult(favoritosDto);
        }
    }
}