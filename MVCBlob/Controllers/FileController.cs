using Microsoft.AspNetCore.Mvc;
using MVCBlob.Models;

namespace MVCBlob.Controllers
{
    public class FileController : Controller
    {
        private readonly HttpClient _apiClient;

        public FileController(IHttpClientFactory httpClientFactory)
        {
            _apiClient = httpClientFactory.CreateClient("BlobApiClient");
        }

        // GET: /Files/
        [HttpGet]
        public async Task<IActionResult> Index()
        {
            try
            {
                var files = await _apiClient.GetFromJsonAsync<List<FileRecord>>("/api/files")
                             ?? new List<FileRecord>();

                return View(files);
            }
            catch (Exception ex)
            {
                ViewBag.ErrorMessage = $"Failed to reach API: {ex.Message}";
                return View(new List<FileRecord>());
            }
        }

        // POST: /Files/Upload
        [HttpPost]
        public async Task<IActionResult> Upload(IFormFile? file)
        {
            if (file != null && file.Length > 0)
            {
                using var multipartContent = new MultipartFormDataContent();
                using var stream = file.OpenReadStream();

                var fileContent = new StreamContent(stream);
                fileContent.Headers.ContentType = new System.Net.Http.Headers.MediaTypeHeaderValue(file.ContentType);

                // "file" matches the parameter name expected by the API
                multipartContent.Add(fileContent, "file", file.FileName);

                var response = await _apiClient.PostAsync("/api/files/upload", multipartContent);

                if (!response.IsSuccessStatusCode)
                {
                    TempData["Error"] = "API rejected the file upload.";
                }
            }

            return RedirectToAction(nameof(Index));
        }
    }
}
