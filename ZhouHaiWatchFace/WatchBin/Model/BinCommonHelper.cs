using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WatchBin.BinAnalyze;
using WatchControlLibrary;

namespace WatchBin.Model
{
    public interface Bin
    {
        byte[] GetBin();
    }

    public enum RecordType
    {
        Layout = 0x00,
        String = 0x01,
        Img = 0x02,
        ImgArray = 0x03,
        Sprite = 0x04,
        File = 0x05,
        Translation = 0x06,
        Data = 0x07,
        Slot = 0x08,
        Widget = 0x09,
        TotalCount = 0x10
    }

    public class BinCommonHelper
    {
        public static Dictionary<string, uint> UidRelationTable = new Dictionary<string, uint>();



        


        public static byte[] CommandEzip(string imagePath, string outputDir, bool isPoint, int? bit)
        {
            if (!Directory.Exists(outputDir))
            {
                Directory.CreateDirectory(outputDir);
            }
            var bitStr = "rgb565";
            if (bit == 24) 
            {
                bitStr = "rgb888";
            }

            string ezipExePath = Path.Combine(CommonHelper.AppPath, "ezip.exe");

            string command = $"/C {ezipExePath} -convert \"{imagePath}\" -{bitStr} -pal_support {(isPoint ? "-l8_support" : "")}  -binfile {(isPoint ? 1 : 2)} -outdir \"{outputDir}\"";
            
            // Create a new process
            Process process = new Process();

            // Configure process information
            process.StartInfo.FileName = "cmd.exe"; // Execute through cmd
            process.StartInfo.Arguments = $"{command}";
            process.StartInfo.RedirectStandardOutput = true;  // Redirect output stream
            process.StartInfo.RedirectStandardError = true;   // Redirect error stream
            process.StartInfo.UseShellExecute = false;  // Disable shell startup
            process.StartInfo.CreateNoWindow = true;  // Don't create new window

            // Start process
            process.Start();
            // Read output result
            string result = process.StandardOutput.ReadToEnd();
            string error = process.StandardError.ReadToEnd();
            process.WaitForExit();
            
            // Log the ezip command execution for debugging
            Console.WriteLine($"Ezip command: {command}");
            Console.WriteLine($"Ezip output: {result}");
            if (!string.IsNullOrEmpty(error))
            {
                Console.WriteLine($"Ezip error: {error}");
            }
            Console.WriteLine($"Ezip exit code: {process.ExitCode}");
            
            var expectedBinFile = $@"{outputDir}\{Path.GetFileNameWithoutExtension(imagePath)}.bin";
            Console.WriteLine($"Looking for bin file: {expectedBinFile}");
            
            if (!File.Exists(expectedBinFile))
            {
                throw new FileNotFoundException($"Ezip failed to generate bin file: {expectedBinFile}. Command: {command}, Output: {result}, Error: {error}");
            }
            
            var bytes = File.ReadAllBytes(expectedBinFile).ToList();
            bytes.InsertRange(4, Enumerable.Range(0, 4).Select(x => (byte)0));
            bytes.AddRange(Enumerable.Range(0, 4 - bytes.Count % 4).Select(x => (byte)0)); //4-byte alignment
            return bytes.ToArray();
        }

        public static byte[] CommandPointer16(string imagePath)
        {
            var image = new Bitmap(imagePath);
            List<byte> bytes = new List<byte>();
            bytes.AddRange(Enumerable.Range(0, 8).Select(x => (byte)0));
            for (int i = 0; i < image.Height; i++)
            {
                for (int j = 0; j < image.Width; j++)
                {
                    var color = image.GetPixel(j, i);
                    var r = (color.R >> 3) & 0x1F;
                    var g = (color.G >> 2) & 0x3F;
                    var b = (color.B >> 3) & 0x1F;
                    var byte1 = ((g & 0x07) << 5) | b;
                    var byte2 = (r << 3 | g >> 3);
                    bytes.Add((byte)byte1);
                    bytes.Add((byte)byte2);
                    bytes.Add((byte)color.A);

                }
            }
            return bytes.ToArray();
        }

        public static byte[] CommandPointer24(string imagePath)
        {
            var image = new Bitmap(imagePath);
            List<byte> bytes = new List<byte>();
            bytes.AddRange(Enumerable.Range(0, 8).Select(x => (byte)0));
            for (int i = 0; i < image.Height; i++)
            {
                for (int j = 0; j < image.Width; j++)
                {
                    var color = image.GetPixel(j, i);
                    bytes.Add(color.R);
                    bytes.Add(color.G);
                    bytes.Add(color.B);
                    bytes.Add(color.A);
                   
                }
            }
            return bytes.ToArray();
        }


        public static int GetQ24(int value)
        {
            return value * 256;
        }

       

        public static readonly Dictionary<string, string> Translations = new Dictionary<string, string>
        {
              {"","None" },
              { "sport_main", "Sports" },
              { "r_record", "Sports Record" },
              { "ac_metrics", "Vitality Index" },
              { "hr_main", "Heart Rate" },
              { "spo2_main", "Blood Oxygen" },
              { "sleep", "Sleep" },
              { "pressire_main", "Pressure" },
              { "breath", "Breathing Training" },
              { "women_health", "Women's Health" },
              { "pcall_contacts", "Contacts" },
              { "phone", "Phone" },
              { "alarm", "Alarm" },
              { "chronograph", "Stopwatch" },
              { "countdown", "Countdown" },
              { "world_clock", "World Clock" },
              { "barometer", "Barometer" },
              { "compass", "Compass" },
              { "weather", "Weather" },
              { "event_remaind", "Events" },
              { "media", "Music" },
              { "remote_camera", "Remote Camera" },
              { "perpetualcalendar", "Calendar" },
              { "find_phone", "Find Phone" },
              { "flashlight", "Flashlight" },
              { "watch_edit", "Watch Face Edit" },
              { "Setting", "Settings" },
        };


        public enum JumpAppStringID
        {
            sport_main, //= 0x01,
            r_record, //= 0x02,
            ac_metrics, //= 0x03,
            hr_main, //= 0x04,
            spo2_main, //= 0x05,
            sleep, //= 0x06,
            pressire_main, //= 0x07,
            breath, //= 0x08,
            women_health, //= 0x09,
            pcall_contacts, //= 0x0A,
            phone, //= 0x0B,
            alarm, //= 0x0C,
            chronograph, //= 0x0D,
            countdown, //= 0x0E,
            world_clock, //= 0x0F,
            barometer, //= 0x10,
            compass, //= 0x11,
            weather, //= 0x12,
            event_remaind, //= 0x13, // Updated value
            media, //= 0x14,
            remote_camera, //= 0x15,
            perpetualcalendar, //= 0x16,
            find_phone, //= 0x17,
            flashlight, //= 0x18,
            watch_edit, //= 0x19,
            Setting, //= 0x1A
        }


     

        public static ushort GetAppId(string appName)
        {
            var id = 0;
            var tran_App = Translations.FirstOrDefault(a => a.Value == appName).Key;
            id = (int)Enum.Parse(typeof(JumpAppStringID), tran_App, ignoreCase: false);
            return (ushort)id;
        }

        public static int GetWidgetLayoutFlag(string align)
        {
            return align switch
            {
                "flex-start" => 0,
                "center" => 1,
                "flex-end" => 2,
                _ => throw new Exception("Unknown layout type")
            };
        }

        //translate



    }
}
