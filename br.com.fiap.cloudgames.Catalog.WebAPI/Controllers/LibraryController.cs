using br.com.fiap.cloudgames.Catalog.Application.UseCases.Library.RetrieveLibrary;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace br.com.fiap.cloudgames.Catalog.WebAPI.Controllers
{
    [Route("api/[controller]")]
    public class LibraryController : Controller
    {
        private readonly RetrieveLibraryUseCase _retrieveLibraryUseCase;
        private const string BASIC_ROLE = "user";

        public LibraryController(RetrieveLibraryUseCase retrieveLibraryUseCase)
        {
            _retrieveLibraryUseCase = retrieveLibraryUseCase;
        }
        
        [Authorize(Roles = BASIC_ROLE)]
        [HttpGet]
        public async Task<IActionResult> Get()
        {
            var result = await _retrieveLibraryUseCase.ExecuteAsync();
            return Ok(result);
        }
    }
}
