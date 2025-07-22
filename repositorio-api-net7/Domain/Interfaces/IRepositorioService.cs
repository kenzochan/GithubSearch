using Application.DTO; // Adicionar o using para os DTOs
using Domain.Entities;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Domain.Interfaces
{
    public interface IRepositorioService
    {
        Task<List<Repositorio>> BuscarRepositorios(string termo);

        Task<List<RepositorioResponseDTO>> ListarRepositoriosDoUsuario(string termo);

        Task AdicionarFavorito(AdicionarFavoritoDTO favoritoDto);

        Task<List<FavoritoDTO>> ListarFavoritos();
    }
}