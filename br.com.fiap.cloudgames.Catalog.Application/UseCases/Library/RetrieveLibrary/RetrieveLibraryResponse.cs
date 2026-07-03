using System;
using System.Collections.Generic;
using System.Security.AccessControl;
using System.Text;

namespace br.com.fiap.cloudgames.Catalog.Application.UseCases.Library.RetrieveLibrary
{
    public class RetrieveLibraryResponse
    {
        public required Dictionary<string, string> OwnedGames { get; set; }
    }
}
