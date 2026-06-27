using System;
using System.Collections.Generic;
using System.Text;

namespace br.com.fiap.cloudgames.Catalog.Application.UseCases.Game.RetrieveGame
{
    public class RetrieveGameResponse
    {
        public String Id { get; set; }
        public String Title { get; set; }
        public String Description { get; set; }
        public String Story { get; set; }
        public String Franchise { get; set; }
        public DateOnly ReleaseDate { get; set; }
        public String AgeRating { get; set; }
        public List<String> GameModes { get; set; }
        public String Publisher { get; set; }
        public List<String> Developers { get; set; }
    }
}
