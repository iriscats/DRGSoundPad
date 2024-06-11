using Microsoft.Win32;
using NAudio.Wave;

namespace ConsoleApp1;

public class SpeakServer
{
    public static bool checkVB()
    {
        bool isVBCableInstalled = false; // 初始化VB-Cable安装状态为假（未安装）
        // 打开注册表中的卸载信息部分
        RegistryKey key = Registry.LocalMachine.OpenSubKey(@"SOFTWARE\Microsoft\Windows\CurrentVersion\Uninstall");
        if (key != null) // 确保key不为null
        {
            foreach (string subkeyName in key.GetSubKeyNames()) // 遍历所有子键
            {
                RegistryKey subkey = key.OpenSubKey(subkeyName);
                string displayName = subkey.GetValue("DisplayName") as string;
                if (!string.IsNullOrEmpty(displayName) && displayName.Contains("CABLE")) // 检查显示名称是否包含"CABLE"
                {
                    isVBCableInstalled = true; // 如果找到，更新安装状态为真（已安装）
                    break; // 退出循环
                }
            }
        }

        return isVBCableInstalled;
    }

    public static string[] GetOutputAudioDeviceNames()
    {
        int waveOutDevices = WaveOut.DeviceCount;
        string[] outputDeviceNames = new string[waveOutDevices];
        for (int i = 0; i < waveOutDevices; i++)
        {
            WaveOutCapabilities deviceInfo = WaveOut.GetCapabilities(i);
            outputDeviceNames[i] = deviceInfo.ProductName;
        }

        return outputDeviceNames;
    }

    public static int GetOutputDeviceID(string deviceName)
    {
        for (int deviceId = 0; deviceId < WaveOut.DeviceCount; deviceId++)
        {
            var capabilities = WaveOut.GetCapabilities(deviceId);
            if (capabilities.ProductName.Contains(deviceName, StringComparison.OrdinalIgnoreCase))
            {
                return deviceId;
            }
        }

        return -1; // 没有找到匹配的设备
    }


    private static bool ignoreNextPlayAttempt = false;

    public static void PlayAudioToSpecificDevice(string audioFilePath, int deviceNumber, bool stopCurrent, float volume,
        bool rplay, string raudioFilePath, int rdeviceNumber, float rvolume)
    {
        try
        {
            if (stopCurrent && currentOutputDevice != null)
            {
                currentOutputDevice.Stop();
                currentOutputDevice.Dispose();
                currentAudioFile.Dispose();
                currentOutputDeviceEX.Stop();
                currentOutputDeviceEX.Dispose();
                currentAudioFileEX?.Dispose();
                currentOutputDevice = null;
                currentAudioFile = null;
                return;
            }
            else if (currentOutputDevice == null)
            {
                var audioFile = new AudioFileReader(audioFilePath);
                var outputDevice = new WaveOutEvent { DeviceNumber = deviceNumber };
                outputDevice.Volume = Math.Max(0, Math.Min(1, volume));
                int totalMilliseconds = (int)audioFile.TotalTime.TotalMilliseconds;
                if (rplay)
                {
                    PlayAudioex(raudioFilePath, rdeviceNumber, rvolume);
                    ignoreNextPlayAttempt = false;
                }

                outputDevice.PlaybackStopped += (sender, e) =>
                {
                    outputDevice.Dispose();
                    audioFile.Dispose();
                    ignoreNextPlayAttempt = false;
                    currentOutputDevice = null;
                    currentAudioFile = null;
                };
                outputDevice.Init(audioFile);
                outputDevice.Play();
                ignoreNextPlayAttempt = false;
                currentOutputDevice = outputDevice;
                currentAudioFile = audioFile;
            }

            ignoreNextPlayAttempt = false;
        }
        catch (Exception ex)
        {
            MessageBox.Show($"播放音频时出错: {ex.Message}");
        }

        ignoreNextPlayAttempt = false;
    }
}