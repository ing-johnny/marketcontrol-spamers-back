using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace marketControlSpamers.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class FileController : ControllerBase
    {
        [HttpPost("filter")]
        public IActionResult FilterFile([FromBody] FileFilterRequest request)
        {
            if (string.IsNullOrEmpty(request.FilePath) || !System.IO.File.Exists(request.FilePath))
            {
                return BadRequest("El archivo especificado no existe.");
            }

            var filteredLines = System.IO.File.ReadLines(request.FilePath)
                                              .Where(line => line.Contains("Torre�n"))
                                              .ToList();

            var newFilePath = Path.Combine(Path.GetDirectoryName(request.FilePath), "filtered_file.txt");

            System.IO.File.WriteAllLines(newFilePath, filteredLines);

            return Ok(new { newFilePath });
        }
    }
    public class FileFilterRequest
    {
        public string FilePath { get; set; }
    }
}
