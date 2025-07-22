using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Domain.Entities;
using Xunit;

namespace TestProject
{
    public class RepositorioTests
    {
        [Theory]
        [InlineData(10, 5, 2, 10 * 3 + 5 * 2 + 2)]
        [InlineData(0, 0, 0, 0)]
        [InlineData(1, 1, 1, 6)]
        public void Deve_Calcular_Relevancia_Corretamente(int estrelas, int forks, int watchers, int esperado)
        {
            // Arrange
            var repo = new Repositorio
            {
                Nome = "Teste",
                Url = "https://github.com/hyprwm/Hyprland",
                Estrelas = estrelas,
                Forks = forks,
                Watchers = watchers
            };

            // Act
            var relevancia = repo.Relevancia;

            // Assert
            Assert.Equal(esperado, relevancia);
        }
    }
}
