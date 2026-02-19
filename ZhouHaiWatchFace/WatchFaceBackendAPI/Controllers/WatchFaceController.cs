using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.IO.Compression;
using WatchControlLibrary.Model;
using WatchBin.Model;

namespace WatchFaceBackendAPI.Controllers
{
    // Extended WatchFace class that includes base64 image data
    public class WatchFaceWithImages : WatchFace
    {
        public Dictionary<string, string>? ImageData { get; set; } // imagePath -> base64Data
    }

    [ApiController]
    [Route("api/[controller]")]
    public class WatchFaceController : ControllerBase
    {
        private readonly ILogger<WatchFaceController> _logger;

        public WatchFaceController(ILogger<WatchFaceController> logger)
        {
            _logger = logger;
        }

        [HttpPost("generate-bin")]
        public async Task<IActionResult> GenerateBinFile(IFormFile zipFile)
        {
            if (zipFile == null || zipFile.Length == 0)
            {
                return BadRequest("No ZIP file uploaded");
            }

            try
            {
                // Create a temporary directory for processing
                var tempDir = Path.Combine(Path.GetTempPath(), Guid.NewGuid().ToString());
                Directory.CreateDirectory(tempDir);

                try
                {
                    // Extract ZIP file to temp directory
                    using (var stream = zipFile.OpenReadStream())
                    using (var archive = new ZipArchive(stream, ZipArchiveMode.Read))
                    {
                        archive.ExtractToDirectory(tempDir);
                    }

                    // Find the JSON file (should be the only .json file in the extracted content)
                    var jsonFiles = Directory.GetFiles(tempDir, "*.json", SearchOption.AllDirectories);
                    if (jsonFiles.Length == 0)
                    {
                        return BadRequest("No JSON file found in the ZIP archive");
                    }

                    var jsonFilePath = jsonFiles[0];
                    var projectDir = Path.GetDirectoryName(jsonFilePath);

                    _logger.LogInformation($"Processing JSON file: {jsonFilePath}");
                    _logger.LogInformation($"Project directory: {projectDir}");

                    // Set current directory to the project directory (for asset resolution)
                    var originalDir = Directory.GetCurrentDirectory();
                    Directory.SetCurrentDirectory(projectDir!);

                    try
                    {
                    // Read and parse the JSON file
                    var jsonContent = await System.IO.File.ReadAllTextAsync(jsonFilePath);
                    
                    // Log the directory contents for debugging
                    _logger.LogInformation("=== DEBUGGING DIRECTORY CONTENTS ===");
                    _logger.LogInformation($"Current directory: {Directory.GetCurrentDirectory()}");
                    _logger.LogInformation($"Project directory: {projectDir}");
                    
                    // List all files in the project directory
                    var allFiles = Directory.GetFiles(projectDir!, "*", SearchOption.AllDirectories);
                    _logger.LogInformation($"All files in project directory ({allFiles.Length} files):");
                    foreach (var file in allFiles)
                    {
                        var relativePath = Path.GetRelativePath(projectDir!, file);
                        _logger.LogInformation($"  - {relativePath}");
                    }
                    
                    // Log JSON content (first 500 chars) for debugging
                    var jsonPreview = jsonContent.Length > 500 ? jsonContent.Substring(0, 500) + "..." : jsonContent;
                    _logger.LogInformation($"JSON content preview: {jsonPreview}");
                    _logger.LogInformation("=== END DEBUGGING ===");
                    
                    // STEP 1: Parse JSON as JObject to extract ImageData without deserializing components
                    var jsonObject = JObject.Parse(jsonContent);
                    var imageDataToken = jsonObject["ImageData"];
                    
                    // STEP 2: Decode and save images to disk BEFORE deserializing components
                    if (imageDataToken != null && imageDataToken.Type == JTokenType.Object)
                    {
                        var imageDataDict = imageDataToken.ToObject<Dictionary<string, string>>();
                        if (imageDataDict != null && imageDataDict.Count > 0)
                        {
                            _logger.LogInformation($"Processing {imageDataDict.Count} base64 images");
                            
                            foreach (var imageEntry in imageDataDict)
                            {
                                var imagePath = imageEntry.Key; // "assets/image_001.png"
                                var base64Data = imageEntry.Value;
                            
                                try
                                {
                                // Decode base64 to bytes
                                var imageBytes = Convert.FromBase64String(base64Data);
                                
                                // Create the directory path
                                var fullImagePath = Path.Combine(projectDir!, imagePath);
                                var imageDir = Path.GetDirectoryName(fullImagePath);
                                if (!string.IsNullOrEmpty(imageDir))
                                {
                                    Directory.CreateDirectory(imageDir);
                                }
                                
                                // Save the image file
                                await System.IO.File.WriteAllBytesAsync(fullImagePath, imageBytes);
                                _logger.LogInformation($"Decoded and saved image: {imagePath}");
                            }
                            catch (Exception ex)
                            {
                                _logger.LogError(ex, $"Failed to decode image: {imagePath}");
                                return BadRequest($"Failed to decode image: {imagePath}");
                            }
                        }
                        }
                    }
                    
                    // STEP 3: Now deserialize the full JSON WITH TypeNameHandling
                    // Images are on disk, so DragBindPoint constructors can load them
                    _logger.LogInformation("Images saved. Now deserializing components...");
                    var settings = new JsonSerializerSettings
                    {
                        TypeNameHandling = TypeNameHandling.Auto
                    };
                    var watchFaceJson = JsonConvert.DeserializeObject<WatchFaceWithImages>(jsonContent, settings);
                    
                    if (watchFaceJson == null)
                    {
                        return BadRequest("Failed to deserialize watch face JSON with components");
                    }
                    
                    _logger.LogInformation($"Successfully deserialized watch face with {watchFaceJson.WatchStyles?.Count ?? 0} styles");
                    
                    // Create a regular WatchFace object for processing
                    var watchFace = new WatchFace
                    {
                        WatchName = watchFaceJson.WatchName,
                        WatchCode = watchFaceJson.WatchCode,
                        CornerX = watchFaceJson.CornerX,
                        CornerY = watchFaceJson.CornerY,
                        Width = watchFaceJson.Width,
                        Height = watchFaceJson.Height,
                        ThumbnailWidth = watchFaceJson.ThumbnailWidth,
                        ThumbnailHeight = watchFaceJson.ThumbnailHeight,
                        Corner = watchFaceJson.Corner,
                        DeviceType = watchFaceJson.DeviceType,
                        CreateTime = watchFaceJson.CreateTime,
                        IsAlbum = watchFaceJson.IsAlbum,
                        ColorBit = watchFaceJson.ColorBit,
                        AlbumBackground = watchFaceJson.AlbumBackground,
                        // FolderName is read-only, skip it
                        WatchStyles = watchFaceJson.WatchStyles
                    };                        if (watchFace == null)
                        {
                            return BadRequest("Failed to deserialize watch face JSON");
                        }

                        _logger.LogInformation($"Successfully parsed watch face: {watchFace.WatchName}");

                        // Generate bin file
                        var binData = BinHelper.GetBin(watchFace);

                        _logger.LogInformation($"Generated bin file with {binData.Length} bytes");

                        // Return the bin file as a download
                        var fileName = $"{watchFace.WatchCode}_{watchFace.WatchName}_{watchFace.DeviceType}.bin";
                        return File(binData, "application/octet-stream", fileName);
                    }
                    finally
                    {
                        // Restore original directory
                        Directory.SetCurrentDirectory(originalDir);
                    }
                }
                finally
                {
                    // Clean up temp directory with retry logic for file locks
                    if (Directory.Exists(tempDir))
                    {
                        await CleanupTempDirectoryAsync(tempDir);
                    }
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error generating bin file");
                return StatusCode(500, $"Error generating bin file: {ex.Message}");
            }
        }

        [HttpGet("health")]
        public IActionResult Health()
        {
            return Ok(new { status = "healthy", timestamp = DateTime.UtcNow });
        }

        private async Task CleanupTempDirectoryAsync(string tempDir)
        {
            const int maxRetries = 3;
            const int delayMs = 100;

            for (int attempt = 0; attempt < maxRetries; attempt++)
            {
                try
                {
                    // Force garbage collection to release any file handles
                    GC.Collect();
                    GC.WaitForPendingFinalizers();

                    // Try to delete individual files first
                    var files = Directory.GetFiles(tempDir, "*", SearchOption.AllDirectories);
                    foreach (var file in files)
                    {
                        try
                        {
                            System.IO.File.SetAttributes(file, FileAttributes.Normal);
                            System.IO.File.Delete(file);
                        }
                        catch
                        {
                            // Ignore individual file deletion errors
                        }
                    }

                    // Then try to delete the directory
                    Directory.Delete(tempDir, true);
                    _logger.LogInformation($"Successfully cleaned up temp directory: {tempDir}");
                    return; // Success, exit the retry loop
                }
                catch (Exception ex)
                {
                    if (attempt == maxRetries - 1)
                    {
                        _logger.LogWarning($"Failed to delete temp directory {tempDir} after {maxRetries} attempts: {ex.Message}");
                    }
                    else
                    {
                        // Wait before retrying
                        await Task.Delay(delayMs * (attempt + 1));
                    }
                }
            }
        }
    }
}