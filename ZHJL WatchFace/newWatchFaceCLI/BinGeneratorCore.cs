using System;
using System.IO;
using System.Linq;
using WatchBasic;
using WatchBasic.UIBasic;
using WatchDB;
using WatchDBDIYJieLi;
using WatchJieLi.Common;

namespace WatchFaceCLI
{
    public static class BinGeneratorCore
    {
        public static void GenerateBin(string inputFolder, string outputFolder)
        {
            Console.WriteLine($"Processing watchface: {inputFolder}");
            Console.WriteLine($"Output folder: {outputFolder}");

            // Get folder name to extract watch info
            string folderName = Path.GetFileName(inputFolder);
            
            // Load layer groups from folder structure
            var layerGroups = inputFolder.GetLayerGroups(false).ToList();
            Console.WriteLine($"Loaded {layerGroups.Count} layer groups");

            // Create watch info from folder name
            var watchInfo = CreateWatchInfoFromFolder(folderName);
            int watchTypeCount = watchInfo.WatchTypes?.Count ?? 0;
            Console.WriteLine($"Found {watchTypeCount} watch descriptions");
            
            // Extract watch code components
            var parts = folderName.Split('_');
            string watchCode = folderName;
            Console.WriteLine($"Watch Code: {watchCode}");
            
            if (parts.Length >= 2)
            {
                string typeValue = parts.Length > 2 ? parts[2] : "Unknown";
                Console.WriteLine($"Shape: {parts[0]}, DPI: {parts[1]}, Type: {typeValue}");
            }

            // Generate bin file
            BinHelpers binHelper = new BinHelpers();
            var binBytes = binHelper.GetBinBytes(layerGroups, 1, watchInfo, null, false);

            // Create timestamped output folder
            string timestamp = DateTime.Now.ToString("yyyy-MM-dd_HH-mm-ss");
            string outputPath = Path.Combine(outputFolder, timestamp);
            Directory.CreateDirectory(outputPath);

            // Write bin file
            string binFilePath = Path.Combine(outputPath, "hor.bin");
            File.WriteAllBytes(binFilePath, binBytes.ToArray());

            Console.WriteLine($"SUCCESS: Bin file generated at: {binFilePath}");
            
            // Display file size
            var fileInfo = new FileInfo(binFilePath);
            double sizeKB = fileInfo.Length / 1024.0;
            Console.WriteLine($"File size: {fileInfo.Length} bytes ({sizeKB:F2} KB)");
        }

        private static WatchInfo_DIYJieLi CreateWatchInfoFromFolder(string folderName)
        {
            var watchInfo = new WatchInfo_DIYJieLi
            {
                WatchName = folderName,
                WatchCode = folderName,
                WatchTypes = new System.Collections.Generic.List<WatchType>()
            };

            // Add standard watch types
            watchInfo.WatchTypes.Add(new WatchType { ID = 1, WatchTypeName = "Main", Align = 0 });
            watchInfo.WatchTypes.Add(new WatchType { ID = 2, WatchTypeName = "Battery", Align = 0 });
            watchInfo.WatchTypes.Add(new WatchType { ID = 3, WatchTypeName = "GeneralDate", Align = 0 });
            watchInfo.WatchTypes.Add(new WatchType { ID = 4, WatchTypeName = "Step", Align = 0 });
            watchInfo.WatchTypes.Add(new WatchType { ID = 5, WatchTypeName = "Calorie", Align = 0 });
            watchInfo.WatchTypes.Add(new WatchType { ID = 6, WatchTypeName = "HeartRate", Align = 0 });
            watchInfo.WatchTypes.Add(new WatchType { ID = 7, WatchTypeName = "Kwh", Align = 0 });

            return watchInfo;
        }
    }
}
