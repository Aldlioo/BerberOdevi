using Microsoft.AspNetCore.Mvc;
using kaufor.Models;
using System.IO;
using System.Threading.Tasks;

namespace kaufor.Controllers
{
    public class HaircutController : Controller
    {
        // GET: Display the Upload Page
        public IActionResult Upload()
        {
            return View();
        }

        // POST: Handle Image Upload and AI Processing
        [HttpPost]
        public async Task<IActionResult> AnalyzeImage(ImageUploadViewModel model)
        {
            if (ModelState.IsValid)
            {
                if (model.Image != null && model.Image.Length > 0)
                {
                    // Save the uploaded file to a local directory
                    var filePath = Path.Combine("wwwroot/uploads", model.Image.FileName);

                    using (var stream = new FileStream(filePath, FileMode.Create))
                    {
                        await model.Image.CopyToAsync(stream);
                    }

                    // Placeholder for calling the AI API
                    var aiResult = await CallAIForHaircutSuggestion(filePath);

                    // Pass the result to the view
                    ViewBag.Suggestion = aiResult;
                    return View("Result");
                }
            }

            return View("Upload");
        }

        // Dummy method for AI processing (to be replaced with actual API call)
        private async Task<string> CallAIForHaircutSuggestion(string imagePath)
        {
            var apiUrl = "https://api-inference.huggingface.co/models/stabilityai/stable-diffusion-2-inpainting\r\n"; // Use this as the API URL
            var apiKey = "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"; // Your Hugging Face API key

            using (var client = new HttpClient())
            {
                client.DefaultRequestHeaders.Add("Authorization", $"Bearer {apiKey}");

                var form = new MultipartFormDataContent();
                form.Add(new ByteArrayContent(System.IO.File.ReadAllBytes(imagePath)), "file", Path.GetFileName(imagePath));

                var response = await client.PostAsync(apiUrl, form);

                if (response.IsSuccessStatusCode)
                {
                    var resultImageBytes = await response.Content.ReadAsByteArrayAsync();
                    var outputPath = Path.Combine("wwwroot/generated-images", "result.png");
                    await System.IO.File.WriteAllBytesAsync(outputPath, resultImageBytes);

                    return "/generated-images/result.png"; // Return path to the saved image
                }
                else
                {
                    var error = await response.Content.ReadAsStringAsync();
                    throw new Exception($"AI API Error: {error}");
                }
            }
        }

    }

}

