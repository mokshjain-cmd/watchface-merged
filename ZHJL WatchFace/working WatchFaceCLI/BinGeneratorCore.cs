using System;
using System.IO;
using System.Linq;
using System.Collections.Generic;
using WatchBasic;
using WatchBasic.UIBasic;
using WatchDB;
using WatchJieLi.Common;
using Microsoft.EntityFrameworkCore;
using WatchUI.CreateBin;
using WatchBasic.LZO;
using WatchBasic.Tool;

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
            
            // Step 1: Create/update watch.db database
            Console.WriteLine("Creating/updating watch.db database...");
            var watchInfo = CreateOrUpdateWatchDatabase(inputFolder, folderName);
            
            // Step 2: Use WatchJieLi's bin generation logic (same as GUI tool)
            Console.WriteLine("Generating bin file using WatchJieLi logic...");
            GenerateBinUsingWatchJieLi(inputFolder, outputFolder, watchInfo);
        }

        private static void GenerateBinUsingWatchJieLi(string inputFolder, string outputFolder, WatchInfo watchInfo)
        {
            // Load layer groups from folder structure (use true for notBitmapSource - matches GUI)
            var layergroups = inputFolder.GetLayerGroups(true).ToList();
            Console.WriteLine($"Loaded {layergroups.Count} layer groups");

            // Check unit placement (matches GUI validation)
            LayerTool.CheckUnit(layergroups, watchInfo);

            // Get watch descriptions
            var watchDescs = WatchTool.GetWatchDesc(inputFolder);
            if (watchDescs.Count == 0)
            {
                watchDescs.Add(new WatchBasic.WatchDesc { Language = "English", WatchName = $"{Path.GetFileName(inputFolder)}:Default" });
            }

            // Set language count for language-dependent layer groups (matches GUI)
            foreach (var i in layergroups)
            {
                if (LayerTool.GetLanguageCode().Contains(i.GroupCode))
                {
                    i.LanguageCount = watchDescs.Count;
                }
            }

            var watchdesc = watchDescs.First();
            var watchIdstr = CommonDefintion.GetWatchId(watchInfo, watchdesc.Language);
            var watchId = System.Text.Encoding.UTF8.GetBytes(watchIdstr).ToList();

            // Use GUI's GetBin logic
            var horbin = GetBin(layergroups, watchDescs, watchdesc, watchInfo, watchId);

            if (horbin == null)
            {
                Console.WriteLine("ERROR: Bin generation failed");
                return;
            }

            // LZO compression
            var compressedHorbin = LZOHelper.mergeDialFile(horbin);

            // Create timestamped output folder
            string timestamp = DateTime.Now.ToString("yyyy-MM-dd_HH-mm-ss");
            string outputPath = Path.Combine(outputFolder, timestamp);
            Directory.CreateDirectory(outputPath);

            // Write bin file
            string binFilePath = Path.Combine(outputPath, "hor.bin");
            File.WriteAllBytes(binFilePath, compressedHorbin);

            Console.WriteLine($"SUCCESS: Bin file generated at: {binFilePath}");
            
            // Display file size
            var fileInfo = new FileInfo(binFilePath);
            double sizeKB = fileInfo.Length / 1024.0;
            Console.WriteLine($"File size: {fileInfo.Length} bytes ({sizeKB:F2} KB)");
        }

        private static byte[] GetBin(List<LayerGroup> layergroups, List<WatchBasic.WatchDesc> watchDescs, WatchBasic.WatchDesc watchdesc, WatchInfo watchInfo, List<byte> watchId)
        {
            // This matches MainViewModel.GetBin() exactly
            BinHelpers binHelper = new BinHelpers();
            var colorInfo = new List<byte>();
            var bin = binHelper.GetBinBytes(layergroups, watchDescs.Count(), watchInfo, colorInfo, false).ToList();
            List<byte> horbin = new List<byte>();

            var watchName = System.Text.Encoding.UTF8.GetBytes(watchdesc.WatchName.Split(":")[1]);
            var len = watchName.Length;

            var arrayCount = 60 - 1 - watchName.Length;
            if (arrayCount < 0)
                throw new Exception("表盘名长度超出限制");
            var remain = Enumerable.Range(0, arrayCount).Select(i => (byte)0).ToList();
            horbin.AddRange(watchId);
            horbin.Add((byte)len);
            horbin.AddRange(watchName);
            horbin.AddRange(remain);
            horbin.Add(0); // 版本号
            horbin.AddRange(100.GetBytes32()); // 块1地址
            var firstSzie = bin.Count();
            var secondAddr = 100 + firstSzie;
            while (secondAddr % 4 != 0)
            {
                secondAddr++;
                bin.Add(0);
            } // 4K对齐
            horbin.AddRange(bin.Count().GetBytes32()); // 块1大小
            horbin.AddRange(secondAddr.GetBytes32()); // 块2地址

            // Not IsAlbum - regular watchface
            var backgroundBin = binHelper.GetBackgroundBinBytes(layergroups, 0, watchInfo, CommonDefintion.Setting);
            horbin.AddRange(backgroundBin.Count().GetBytes32()); // 块2大小
            horbin.AddRange(Enumerable.Range(0, 3).Select(i => (byte)0).ToArray()); // 保留3
            horbin.AddRange(bin);
            horbin.AddRange(backgroundBin);
            horbin.AddRange(LZOHelper.GetOther(horbin.ToArray()));

            return horbin.ToArray();
        }

        private static WatchInfo CreateOrUpdateWatchDatabase(string inputFolder, string folderName)
        {
            // Create database context (simple version with only 4 tables)
            var dataContext = new WatchDataContext(inputFolder);
            
            // Ensure database is created
            dataContext.Database.EnsureCreated();
            Console.WriteLine($"Database created/opened at: {inputFolder}\\watch.db");

            // Check if WatchInfo already exists
            var existingWatchInfo = dataContext.WatchInfos
                .Include(i => i.WatchTypes)
                .ThenInclude(t => t.WatchGroups)
                .FirstOrDefault();

            WatchInfo watchInfo;

            if (existingWatchInfo != null)
            {
                // Update watch code if folder name changed
                if (existingWatchInfo.WatchCode != folderName)
                {
                    existingWatchInfo.WatchCode = folderName;
                    existingWatchInfo.WatchName = folderName;
                    dataContext.SaveChanges();
                    Console.WriteLine("Updated existing watch info with new watch code");
                }
                watchInfo = existingWatchInfo;
            }
            else
            {
                // Create new WatchInfo
                watchInfo = new WatchInfo
                {
                    WatchName = folderName,
                    WatchCode = folderName,
                    WatchTypes = new List<WatchType>()
                };

                // Add standard watch types with WatchGroups collection
                var watchTypes = new List<WatchType>
                {
                    new WatchType { ID = 1, WatchTypeName = "Main", Align = 0, WatchGroups = new List<WatchGroup>() },
                    new WatchType { ID = 2, WatchTypeName = "Battery", Align = 0, WatchGroups = new List<WatchGroup>() },
                    new WatchType { ID = 3, WatchTypeName = "GeneralDate", Align = 0, WatchGroups = new List<WatchGroup>() },
                    new WatchType { ID = 4, WatchTypeName = "Step", Align = 0, WatchGroups = new List<WatchGroup>() },
                    new WatchType { ID = 5, WatchTypeName = "Calorie", Align = 0, WatchGroups = new List<WatchGroup>() },
                    new WatchType { ID = 6, WatchTypeName = "HeartRate", Align = 0, WatchGroups = new List<WatchGroup>() },
                    new WatchType { ID = 7, WatchTypeName = "Kwh", Align = 0, WatchGroups = new List<WatchGroup>() }
                };

                foreach (var watchType in watchTypes)
                {
                    watchInfo.WatchTypes.Add(watchType);
                }

                dataContext.WatchInfos.Add(watchInfo);
                dataContext.SaveChanges();
                Console.WriteLine("Created new watch info in database");
            }

            // Load layer groups from folder structure
            var layerGroups = inputFolder.GetLayerGroups(false).ToList();
            
            // Sync layer groups to WatchGroups table
            SyncLayerGroupsToDatabase(dataContext, watchInfo, layerGroups);

            return watchInfo;
        }

        private static void SyncLayerGroupsToDatabase(WatchDataContext dataContext, WatchInfo watchInfo, List<LayerGroup> layerGroups)
        {
            Console.WriteLine("Syncing layer groups to database (GUI-compatible mode)...");

            // ComplexCodeHead from LayerTool - only sync these categories
            var complexCodeHeads = new List<string> { "02", "03", "04", "05", "06", "07", "08", "09" };
            
            // Map folder codes to watch type names (matching GUI logic)
            var codeToTypeNameMap = new Dictionary<string, string>
            {
                { "02", "Battery" },      // 电量
                { "03", "GeneralDate" },  // 通用日期
                { "04", "GeneralDate" },  // Part of date
                { "05", "GeneralDate" },  // Part of date
                { "06", "GeneralDate" },  // Time display
                { "07", "Step" },         // 步数
                { "08", "HeartRate" },    // 心率
                { "09", "Calorie" }       // 卡路里
            };

            // Sync each complex code head category
            foreach (var codeHead in complexCodeHeads)
            {
                var showGroups = layerGroups.Where(i => 
                    i.GroupCode!.StartsWith(codeHead) && 
                    !i.GroupName!.Contains("辅助文件")
                ).ToList();

                // Special handling for "03" - include 04xx and 05xx groups
                if (codeHead == "03")
                {
                    var otherGroups = layerGroups.Where(i => 
                        (i.GroupCode!.StartsWith("04") || i.GroupCode!.StartsWith("05")) &&
                        !i.GroupName!.Contains("辅助文件")
                    ).ToList();
                    showGroups.AddRange(otherGroups);
                }

                // Special handling for "02" - include bluetooth (0102)
                if (codeHead == "02")
                {
                    var bluetoothGroups = layerGroups.Where(i => i.GroupCode!.StartsWith("0102")).ToList();
                    showGroups.AddRange(bluetoothGroups);
                }

                if (showGroups.Count > 0)
                {
                    string typeName = codeToTypeNameMap[codeHead];
                    var watchType = watchInfo.WatchTypes.FirstOrDefault(t => t.WatchTypeName == typeName);
                    
                    if (watchType != null)
                    {
                        foreach (var layerGroup in showGroups)
                        {
                            var existingGroup = watchType.WatchGroups?.FirstOrDefault(g => g.GroupCode == layerGroup.GroupCode);
                            if (existingGroup == null)
                            {
                                var watchGroup = new WatchGroup
                                {
                                    GroupCode = layerGroup.GroupCode,
                                    WatchTypeId = watchType.ID,
                                    Left = layerGroup.Left,
                                    Top = layerGroup.Top,
                                    ColorDesc = ""
                                };
                                watchType.WatchGroups?.Add(watchGroup);
                            }
                        }
                    }
                }
            }

            // Sync pointer groups (starting with "13")
            var pointerGroups = layerGroups.Where(i => i.GroupCode!.StartsWith("13")).ToList();
            if (pointerGroups.Count > 0)
            {
                // Pointers typically go under Kwh type in GUI
                var kwhType = watchInfo.WatchTypes.FirstOrDefault(t => t.WatchTypeName == "Kwh");
                if (kwhType != null)
                {
                    foreach (var layerGroup in pointerGroups)
                    {
                        var existingGroup = kwhType.WatchGroups?.FirstOrDefault(g => g.GroupCode == layerGroup.GroupCode);
                        if (existingGroup == null)
                        {
                            var watchGroup = new WatchGroup
                            {
                                GroupCode = layerGroup.GroupCode,
                                WatchTypeId = kwhType.ID,
                                Left = layerGroup.Left,
                                Top = layerGroup.Top,
                                ColorDesc = ""
                            };
                            kwhType.WatchGroups?.Add(watchGroup);
                        }
                    }
                }
            }

            dataContext.SaveChanges();
            int groupCount = dataContext.WatchGroups.Count();
            Console.WriteLine($"Synced {groupCount} watch groups to database (matching GUI behavior)");
        }
    }
}
