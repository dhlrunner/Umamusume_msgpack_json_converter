using MessagePack;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace umamusume_msgpack_json_converter
{
    class Program
    {
        static void Main(string[] args)
        {
            byte[] file = File.ReadAllBytes(args[0]);
            byte[] newfile = null;
            using (BinaryReader ms = new BinaryReader(new MemoryStream(file)))
            {
                byte[] magic = { 0xA6, 0, 0, 0 };
                if (ms.ReadBytes(4).SequenceEqual(magic)) //check if binary data is included
                {
                    ms.BaseStream.Seek(170, 0);
                    newfile = ms.ReadBytes((int)ms.BaseStream.Length - 170); //cut 170 bytes of binary data to decode a msgpack
                }
                else
                {
                    newfile = file;
                }
            }
            string json = MessagePackSerializer.ConvertToJson(newfile);
            File.WriteAllText(Path.GetDirectoryName(args[0]) + "/" + Path.GetFileNameWithoutExtension(args[0]) + ".json", json);
            try
            {
                JObject data = JObject.Parse(json);
                JArray race = (JArray)data["data"]["race_result_array"];
                Console.WriteLine("race_scenario array data found..");
                for (int i = 0; i < race.Count; i++)
                {
                    JToken a = race[i];
                    byte[] raw = Convert.FromBase64String(a["race_scenario"].ToString());
                    string outfilename = Path.GetDirectoryName(args[0]) + "/" + Path.GetFileNameWithoutExtension(args[0]) +
                        "_racescenario" + i.ToString() + ".bin";
                    File.WriteAllBytes(outfilename, Decompress(raw));
                    ConvertRaceScenarioBinarytoJson(outfilename);
                }
            }
            catch
            {
                try
                {
                    JObject data = JObject.Parse(json);
                    byte[] raw = Convert.FromBase64String(data["data"]["race_scenario"].ToString());
                    string outfilename = Path.GetDirectoryName(args[0]) + "/" + Path.GetFileNameWithoutExtension(args[0]) +
                            "_racescenario.bin";
                    Console.WriteLine("race_scenario data found..");
                    File.WriteAllBytes(outfilename, Decompress(raw));
                    ConvertRaceScenarioBinarytoJson(outfilename);
                }
                catch
                {
                    Console.WriteLine("No race_scenario data");
                }
            }
        }
        static byte[] Decompress(byte[] gzip)
        {
            using (GZipStream stream = new GZipStream(new MemoryStream(gzip),
                CompressionMode.Decompress))
            {
                const int size = 4096;
                byte[] buffer = new byte[size];
                using (MemoryStream memory = new MemoryStream())
                {
                    int count = 0;
                    do
                    {
                        count = stream.Read(buffer, 0, size);
                        if (count > 0)
                        {
                            memory.Write(buffer, 0, count);
                        }
                    }
                    while (count > 0);
                    return memory.ToArray();
                }
            }
        }
        static void ConvertRaceScenarioBinarytoJson(string inputfilepath)
        {
            ProcessStartInfo processInfo = new ProcessStartInfo();
            processInfo.FileName = "race_data_parser.exe";
            processInfo.Arguments = inputfilepath;
            processInfo.WorkingDirectory = AppDomain.CurrentDomain.BaseDirectory;
            processInfo.CreateNoWindow = true;
            processInfo.UseShellExecute = false;
            Process process = new Process();
            process.StartInfo = processInfo;
            process.Start();
            process.WaitForExit();
            process.Close();
        }
    }
}
