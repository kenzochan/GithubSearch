using Xunit;
using Moq;
using System.Net;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using Infrastructure.Services;
using Microsoft.Extensions.Logging;
using System.Text;
using System.Collections.Generic;
using Domain.Entities;
using System.Net.Http.Headers;
using System;
using System.Linq;
using Moq.Protected;
using Application.DTO;

public class RepositorioServiceTests
{
    // Método auxiliar que cria uma instância do RepositorioService com um HttpClient simulado (mock)
    private RepositorioService CriarServiceComMockHttp(string respostaJson)
    {
        // Cria uma simulação (mock) do handler de mensagens HTTP
        var handlerMock = new Mock<HttpMessageHandler>();

        // Define o comportamento simulado para o método SendAsync (protegido)
        handlerMock
            .Protected()
            .Setup<Task<HttpResponseMessage>>(
                "SendAsync",
                ItExpr.IsAny<HttpRequestMessage>(),
                ItExpr.IsAny<CancellationToken>()
            )
            .ReturnsAsync((HttpRequestMessage request, CancellationToken token) =>
            {
                // Retorna uma resposta HTTP falsa com o conteúdo JSON passado como parâmetro
                var response = new HttpResponseMessage(HttpStatusCode.OK)
                {
                    Content = new StringContent(respostaJson, Encoding.UTF8, "application/json")
                };
                return response;
            });

        // Cria o HttpClient com o handler simulado (mockado)
        var httpClient = new HttpClient(handlerMock.Object);
        httpClient.DefaultRequestHeaders.UserAgent.Add(new ProductInfoHeaderValue("test", "1.0"));

        // Cria uma simulação (mock) do logger
        var loggerMock = new Mock<ILogger<RepositorioService>>();

        // Retorna a instância de RepositorioService com as dependências mockadas
        return new RepositorioService(httpClient, loggerMock.Object);
    }

    // Teste para verificar se o método BuscarRepositorios retorna corretamente os dados do GitHub
    [Fact]
    public async Task BuscarRepositorios_DeveRetornarLista_DeRepositorios()
    {
        // Arrange: define uma resposta JSON falsa simulando a resposta da API do GitHub
        string fakeJson = @"
        {
          ""items"": [
            {
              ""name"": ""Repo1"",
              ""html_url"": ""https://github.com/user/Repo1"",
              ""stargazers_count"": 100,
              ""forks_count"": 50,
              ""watchers_count"": 70
            }
          ]
        }";

        var service = CriarServiceComMockHttp(fakeJson);

        // Act: executa o método que será testado
        var resultado = await service.BuscarRepositorios("teste");

        // Assert: verifica se os resultados retornados estão corretos
        Assert.Single(resultado);
        Assert.Equal("Repo1", resultado[0].Nome);
        Assert.Equal("https://github.com/user/Repo1", resultado[0].Url);
        Assert.Equal(100, resultado[0].Estrelas);
    }

    // Teste para verificar se um favorito adicionado aparece na lista de favoritos
    [Fact]
    public async Task ListarFavoritos_DeveRetornarFavoritos_Adicionados()
    {
        // Arrange: cria o service com HttpClient "vazio", já que não será usado neste teste
        var loggerMock = new Mock<ILogger<RepositorioService>>();
        var httpClient = new HttpClient(); // não será usado nesse teste

        var service = new RepositorioService(httpClient, loggerMock.Object);

        var favorito = new AdicionarFavoritoDTO
        {
            Nome = "teste1",
            Url = "https://github.com/teste1"
        };

        // Act: adiciona um favorito e lista os favoritos
        await service.AdicionarFavorito(favorito);
        var favoritos = await service.ListarFavoritos();

        // Assert: verifica se o favorito foi adicionado corretamente
        Assert.Single(favoritos);
        Assert.Equal(favorito.Nome, favoritos[0].Nome);
        Assert.Equal(favorito.Url, favoritos[0].Url);
    }

    // Teste para garantir que favoritos duplicados não sejam adicionados
    [Fact]
    public async Task AdicionarFavorito_NaoAdicionaDuplicado()
    {
        // Arrange: configura o service
        var loggerMock = new Mock<ILogger<RepositorioService>>();
        var httpClient = new HttpClient();

        var service = new RepositorioService(httpClient, loggerMock.Object);

        var favorito = new AdicionarFavoritoDTO
        {
            Nome = "teste1",
            Url = "https://github.com/teste1"
        };

        // Act: adiciona o mesmo favorito duas vezes
        await service.AdicionarFavorito(favorito);
        await service.AdicionarFavorito(favorito); // tenta duplicar
        var favoritos = await service.ListarFavoritos();

        // Assert: deve existir apenas um favorito na lista
        Assert.Single(favoritos); // deve haver apenas 1
    }
}
