
using System;
using System.IO;
using System.Net.Http;
using System.Threading.Tasks;

namespace DRGSoundPad
{

    public class TTSServer
    {
        public static String DefaultUrl =
            "https://dds.dui.ai/runtime/v1/synthesize?voiceId=xmamif&speed=8&volume=100&audioType=wav&text=";


        public static async Task DownloadFile(string text, string filePath)
        {
            try
            {
                using var client = new HttpClient();
                var response = await client.GetAsync(DefaultUrl + text);
                response.EnsureSuccessStatusCode();
                await using var fileStream = new FileStream(filePath, FileMode.Create);
                await response.Content.CopyToAsync(fileStream);
            }
            catch (Exception ex)
            {
                Console.Write(ex.ToString());
            }
        }
    }
}