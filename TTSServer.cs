namespace ConsoleApp1;

public class TTSServer
{
    private String url =
        "https://dds.dui.ai/runtime/v1/synthesize?voiceId=ppangf_csn&text=您好世界&speed=1&volume=50&audioType=wav";

    public async Task DownloadFile(string url, string filePath)
    {
        using var client = new HttpClient();
        // 发送 GET 请求并获取响应消息
        var response = await client.GetAsync(url);

        // 确认响应成功
        response.EnsureSuccessStatusCode();

        // 从响应消息中读取文件内容并写入到文件
        await using var fileStream = new FileStream(filePath, FileMode.Create);
        await response.Content.CopyToAsync(fileStream);
    }
}