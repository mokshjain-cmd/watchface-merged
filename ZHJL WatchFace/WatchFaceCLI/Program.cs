using System;
using System.IO;
using System.Linq;
using System.Collections.Generic;
using WatchBasic;
using WatchBasic.UIBasic;
using WatchBasic.Tool;
using WatchBasic.LZO;
using WatchJieLi.Common;
using WatchUI.CreateBin;
using WatchDB_DIY;

namespace WatchFaceCLI
{
    class Program
    {
        static int Main(string[] args)
        {
            if (args.Length < 2)
            {
                Console.WriteLine("WatchFace CLI - JieLi Bin Generator");
                Console.WriteLine("Usage: WatchFaceCLI <inputFolder> <outputFolder>");
                Console.WriteLine("");
                Console.WriteLine("Example:");
                Console.WriteLine("  WatchFaceCLI \"C:\\temp\\方形_390x450_普通_201#简约_MyWatch_001_00\" \"C:\\output\"");
                return 1;
            }

            string inputFolder = args[0];
            string outputFolder = args[1];

            if (!Directory.Exists(inputFolder))
            {
                Console.Error.WriteLine($"Error: Input folder does not exist: {inputFolder}");
                return 1;
            }

            if (!Directory.Exists(outputFolder))
            {
                Directory.CreateDirectory(outputFolder);
            }

            try
            {
                Console.WriteLine($"Processing watchface: {inputFolder}");
                Console.WriteLine($"Output folder: {outputFolder}");

                // Initialize CommonDefintion.Setting with values extracted from folder name
                string folderName = Path.GetFileName(inputFolder);
                var parts = folderName.Split('_');
                
                CommonDefintion.Setting = new WatchSetting
                {
                    ProjectName = folderName,
                    Width = parts.Length > 1 && parts[1].Contains("x") ? int.Parse(parts[1].Split('x')[0]) : 390,
                    Height = parts.Length > 1 && parts[1].Contains("x") ? int.Parse(parts[1].Split('x')[1]) : 450,
                    ThumbnailWidth = 100,
                    ThumbnailHeight = 100,
                    ThumbnailX = 0,
                    ThumbnailY = 0,
                    MaxValue = 1500
                };

                // Load layer groups from folder structure
                var layerGroups = inputFolder.GetLayerGroups(true).ToList();
                Console.WriteLine($"Loaded {layerGroups.Count} layer groups");

                // Get watch descriptions
                var watchDescs = WatchTool.GetWatchDesc(inputFolder);
                Console.WriteLine($"Found {watchDescs?.Count ?? 0} watch descriptions");

                // Create WatchInfo from folder name
                var watchInfo = new WatchInfo_DIY
                {
                    WatchCode = folderName,
                    WatchTypes = new List<WatchDB.WatchType>
                    {
                        new WatchDB.WatchType { WatchTypeName = "Kwh", Align = 0 },
                        new WatchDB.WatchType { WatchTypeName = "GeneralDate", Align = 0 },
                        new WatchDB.WatchType { WatchTypeName = "Step", Align = 0 },
                        new WatchDB.WatchType { WatchTypeName = "Calorie", Align = 0 },
                        new WatchDB.WatchType { WatchTypeName = "HeartRate", Align = 0 }
                    }
                };

                Console.WriteLine($"Watch Code: {watchInfo.WatchCode}");
                Console.WriteLine($"Shape: {watchInfo.Shape}, DPI: {watchInfo.Dpi}, Type: {watchInfo.Type}");

                // Generate bin bytes
                var binHelper = new BinHelpers();
                var colorInfo = new List<byte>();
                bool isAlbum = false;

                var binBytes = binHelper.GetBinBytes(layerGroups, watchDescs?.Count ?? 1, watchInfo, colorInfo, isAlbum);

                // Apply LZO compression
                byte[] finalBin = binBytes.ToArray();
                if (!isAlbum)
                {
                    finalBin = LZOHelper.mergeDialFile(finalBin);
                }

                // Create timestamped output folder
                string timestamp = DateTime.Now.ToString("yyyy-MM-dd_HH-mm-ss");
                string outputPath = Path.Combine(outputFolder, timestamp, "hor.bin");
                Directory.CreateDirectory(Path.GetDirectoryName(outputPath));

                // Write bin file
                File.WriteAllBytes(outputPath, finalBin);

                Console.WriteLine($"SUCCESS: Bin file generated at: {outputPath}");
                Console.WriteLine($"File size: {finalBin.Length} bytes ({finalBin.Length / 1024.0:F2} KB)");
                return 0;
            }
            catch (Exception ex)
            {
                Console.Error.WriteLine($"Error: {ex.Message}");
                Console.Error.WriteLine(ex.StackTrace);
                return 1;
            }
        }
    }
}
