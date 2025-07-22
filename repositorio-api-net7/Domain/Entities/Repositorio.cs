using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Entities
{
    public class Repositorio
    {
        public string Nome { get; set; }
        public string Url { get; set; }
        public int Estrelas { get; set; }
        public int Forks { get; set; }
        public int Watchers { get; set; }

        public int Relevancia => Estrelas * 2 + Forks * 3 + Watchers; 
        /*Quantidades de Forks indica que muitas pessoas se envolveram a fundo com o repsitorio: peso 3
         Estrelas indicam a popularidade: peso 2
        Quanto mais watchers significa diretamente que o repositorio possui relevancia,*/
    }
}
