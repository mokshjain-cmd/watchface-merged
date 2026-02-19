using System;
using System.IO;
using WatchJieLi;

namespace WatchFaceCLI
{
    class Program
    {
        static void Main(string[] args)
        {
            if (args.Length < 2)
            {
                Console.WriteLine("Usage: WatchFaceCLI <inputFolder> <outputFolder>");
                return;
            }
            string inputFolder = args[0];
            string outputFolder = args[1];

            if (!Directory.Exists(inputFolder))
            {
                Console.WriteLine($"Input folder does not exist: {inputFolder}");
                return;
            }
            if (!Directory.Exists(outputFolder))
            {
                Directory.CreateDirectory(outputFolder);
            }

            try
            {
                // Initialize CommonDefintion.Setting with default or sample values
                WatchBasic.CommonDefintion.Setting = new WatchBasic.UIBasic.WatchSetting
                {
                    ProjectName = "DefaultProject",
                    Width = 240,
                    Height = 280,
                    ThumbnailWidth = 100,
                    ThumbnailHeight = 100,
                    ThumbnailX = 0,
                    ThumbnailY = 0,
                    MaxValue = 1500
                };
                BinGeneratorCore.GenerateBin(inputFolder, outputFolder);
                Console.WriteLine("Bin generation completed successfully.");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error: {ex.Message}");
            }
        }
    }
}
