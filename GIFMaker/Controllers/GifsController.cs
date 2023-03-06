using GIFMaker.Contracts;
using GIFMaker.Exceptions;
using GIFMaker.Models;
using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;

namespace GIFMaker.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [ResponseCache(Duration = 30, VaryByHeader = "cookie")]
    public class GifsController : ControllerBase
    {
        private readonly IGifRepository _gifRepository;
        private readonly IGifGenerator _gifGenerator;

        public GifsController(IGifRepository gifRepository, IGifGenerator gifGenerator)
        {
            _gifRepository = gifRepository ?? throw new ArgumentNullException(nameof(gifRepository));
            _gifGenerator = gifGenerator ?? throw new ArgumentNullException(nameof(gifGenerator));
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<string>>> GetWhereNameContains([FromQuery, Required(AllowEmptyStrings = false)] string name)
        {
            var gifs = await _gifRepository.GetWhereNameContainsAsync(name);
            return Ok(gifs.Select(x => x.Name));
        }

        [HttpGet("{name}")]
        public async Task<ActionResult> GetByName([Required(AllowEmptyStrings = false)]string name)
        {
            var gif = await _gifRepository.GetByNameAsync(name);
            if (gif == null)
            {
                return NotFound();
            }

            return File(gif.FileBytes, "image/gif");
        }

        [HttpPost]
        public async Task<ActionResult> CreateGif([FromForm]GifCreation gifCreation)
        {
            try
            {
                var gif = await _gifGenerator.GenerateAsync(gifCreation.Name, gifCreation.ImageFiles.Select(x => x.OpenReadStream()));
                await _gifRepository.InsertGifAsync(gif);
                return CreatedAtAction(nameof(GetByName), new { name = gif.Name }, new { name = gif.Name});
            }
            catch (InvalidFileException ex)
            {
                return BadRequest(ex.Message);
            }
            catch (GifNameTakenException ex)
            {
                return Conflict(ex.Message);
            }

        }
    }
}
