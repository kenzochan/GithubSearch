using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.DTO
{
    public class RepositorioResponseDTO
    {
        public string Nome { get; set; }
        public string Url { get; set; }
        public int Estrelas { get; set; }
        public int Forks { get; set; }
        public int Watchers { get; set; }
    }
}
