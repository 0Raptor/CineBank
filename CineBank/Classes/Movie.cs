using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CineBank
{
    public class Movie
    {
        public string Title { get; set; }
        public string Description { get; set; }
        public string CoverPath { get; set; }
        public string Genre { get; set; }
        public string Duration { get; set; } // screentime in h:mm:ss OR number of episodes
        public string Type { get; set; }
    }
}
