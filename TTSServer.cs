
namespace DRGSoundPad
{

    public class TTSServer
    {
        private static String baseUrl =
            "https://dds.dui.ai/runtime/v1/synthesize?voiceId=xmamif&speed=1&volume=50&audioType=wav&text=";

        public static async Task DownloadFile(string text, string filePath)
        {
            using var client = new HttpClient();
            var response = await client.GetAsync(baseUrl + text);
            response.EnsureSuccessStatusCode();
            await using var fileStream = new FileStream(filePath, FileMode.Create);
            await response.Content.CopyToAsync(fileStream);
        }
    }
}