using GateEntry.Model;
using GateEntry.Processors;
using GateEntry.Repository;
using LiveStreamingServerNet.Utilities.Mediators.Contracts;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;

namespace GateEntry.Controllers
{
    [Authorize]
    [ApiController]
    [Route("api/[controller]")]
    public class PlatesController(
        ILogger<PlatesController> logger,
        IPlateAccessRepository plateAccessRepository,
        IOptions<Settings> settings)
        : ControllerBase
    {
        [HttpPost(Name = "AddPlate")]
        public async Task<IActionResult> AddPlate([FromBody] PlateRequest plateRequest)
        {
            logger.Log(LogLevel.Information, "Adding plate: {0}", plateRequest);

            var numberPlate = GetSanitized(plateRequest.Number);

            var plate = new Plate
            {
                Number = numberPlate,
                Added = DateTime.UtcNow,
                Enabled = true
            };

            if(await plateAccessRepository.TryAdd(plate))
                return CreatedAtAction(nameof(GetPlates), plate);

            return Conflict(new { message = "Failed to add plate." });
        }

        [HttpPut("{number}/status")]
        public async Task<IActionResult> UpdatePlateStatus(string number, [FromBody] bool enabled)
        {
            var sanitizedNumber = number.ToUpperInvariant();

            var plate = await plateAccessRepository.TryGet(sanitizedNumber);

            if (plate == null)
            {
                return NotFound();
            }

            plate.Enabled = enabled;

            return Ok(plate);
        }

        [HttpDelete("{number}", Name = "DeletePlate")]
        public async Task<IActionResult> DeletePlate(string number)
        {
            var sanitized = GetSanitized(number);

            logger.Log(LogLevel.Information, "Removing plate: {0}", sanitized);

            if (await plateAccessRepository.TryDelete(sanitized))
                return NoContent();

            return NotFound();
        }

        [HttpGet(Name = "GetPlates")]
        public async Task<IActionResult> GetPlates()
        {
            var result = await plateAccessRepository.Get();

            return Ok(result.OrderBy(p => p.Added));
        }

        /*
        [HttpPost("process", Name = "FindPlates")]
        public IEnumerable<string> ProcessImage(IFormFile file)
        {
            return _plateDetector.DetectPlates(file.OpenReadStream());
        }
        */

        private string GetSanitized(string plate)
        {
            return plate.Replace(" ", "").ToUpperInvariant();
        }
    }
}
