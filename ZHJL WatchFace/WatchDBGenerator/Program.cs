using System;
using System.IO;
using System.Linq;
using System.Collections.Generic;
using WatchBasic;
using WatchBasic.UIBasic;
using WatchDB;
using Microsoft.EntityFrameworkCore;

namespace WatchDBGenerator
{
    class Program
    {
        static void Main(string[] args)
        {
            // Initialize SQLite
            SQLitePCL.Batteries.Init();

            if (args.Length < 1)
            {
                Console.WriteLine("Usage: WatchDBGenerator <watchfaceFolder>");
                return;
            }

            string watchfaceFolder = args[0];

            if (!Directory.Exists(watchfaceFolder))
            {
                Console.WriteLine($"Error: Folder does not exist: {watchfaceFolder}");
                return;
            }

            try
            {
                string folderName = Path.GetFileName(watchfaceFolder);
                Console.WriteLine($"Generating watch.db for: {folderName}");
                
                CreateOrUpdateWatchDatabase(watchfaceFolder, folderName);
                
                Console.WriteLine($"SUCCESS: watch.db created at: {watchfaceFolder}\\watch.db");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"ERROR: {ex.Message}");
                Console.WriteLine($"Stack trace: {ex.StackTrace}");
                Environment.Exit(1);
            }
        }

        private static void CreateOrUpdateWatchDatabase(string inputFolder, string folderName)
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
