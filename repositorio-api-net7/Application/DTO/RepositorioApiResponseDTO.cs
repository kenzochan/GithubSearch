using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.DTO
{
    public class RepositorioApiResponseDTO
    {
        public string Name { get; set; }
        public string Html_Url { get; set; }
        public int Stars_Count { get; set; }
        public int Forks_Count { get; set; }
        public int Watchers_Count { get; set; }
    }
}
