using Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Interfaces
{
    public interface IRepositorioService
    {
        Task<List<Repositorio>> BuscarRepositorios(string termo);
        Task<List<Repositorio>> ListarRepositoriosDoUsuario(string termo);
        Task AdicionarFavorito(Favorito favorito);
        Task<List<Favorito>> ListarFavoritos();
    }
}
