
namespace DRGSoundPad
{

    public class TTSServer
    {
        private static String baseUrl =
            "https://dds.dui.ai/runtime/v1/synthesize?voiceId=xmamif&speed=1&volume=50&audioType=wav&text=";

        public static async Task DownloadFile(string text, string filePath)
        {
            using var client = new HttpClient();
            // 发送 GET 请求并获取响应消息
            var response = await client.GetAsync(baseUrl + text);

            // 确认响应成功
            response.EnsureSuccessStatusCode();

            // 从响应消息中读取文件内容并写入到文件
            await using var fileStream = new FileStream(filePath, FileMode.Create);
            await response.Content.CopyToAsync(fileStream);
        }


    }
}