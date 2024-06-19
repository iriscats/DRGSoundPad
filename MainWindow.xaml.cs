using Microsoft.VisualBasic.Devices;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace DRGSoundPad
{
    public partial class MainWindow : Window
    {

        private const string WavSaveDir = "Sound";
        private int _vbDevice = 0;
        private object _lockObject = new object();


        public MainWindow()
        {
            InitializeComponent();

            _vbDevice = SpeakServer.GetOutputDeviceID("CABLE Input (VB-Audio Virtual C");
            this.TB_TTS_TextBox.Text = TTSServer.DefaultUrl;
        }

        public static string ComputeMD5Hash(string input)
        {
            using (MD5 md5Hash = MD5.Create())
            {
                byte[] data = md5Hash.ComputeHash(Encoding.UTF8.GetBytes(input));
                StringBuilder sBuilder = new StringBuilder();
                for (int i = 0; i < data.Length; i++)
                {
                    sBuilder.Append(data[i].ToString("x2"));
                }
                return sBuilder.ToString();
            }
        }

        void CheckAndCreateWavSaveDir(string path)
        {
            if (!System.IO.Directory.Exists(path))
            {
                System.IO.Directory.CreateDirectory(path);
            }
        }

        async void PlaySound(DRGMessage message)
        {

            string audioName = ComputeMD5Hash(message.msg) + ".wav";
            string audioPath = System.IO.Path.Combine(WavSaveDir, audioName);
            if (!File.Exists(audioPath))
            {
                await TTSServer.DownloadFile(message.msg, audioPath);
            }

            lock (_lockObject)
            {
                SpeakServer.PlayAudioToSpecificDevice(audioPath, _vbDevice, false, 1);
                SpeakServer.PlayAudioex(audioPath, 0, 1);
            }

            Dispatcher.Invoke(() =>
            {
                this.TB_Log.Text += $"[{message.player}]{message.msg}\n";
            });

        }

        private void CheckEnv()
        {
            bool vb = SpeakServer.checkVB();
            if (vb)
            {
                this.TB_VB.Text = "已启用";
                this.B_VB.Visibility = Visibility.Hidden;
            }
            else
            {
                this.TB_VB.Text = "未启用";
                this.B_VB.Visibility = Visibility.Visible;
            }


        }

        private void CheckMod()
        {
            bool mod = ModInstaller.CheckModInstall();
            if (mod)
            {
                this.TB_Mod.Text = "已启用";
                this.B_Mod.Visibility = Visibility.Hidden;
            }
            else
            {
                this.TB_Mod.Text = "未启用";
                this.B_Mod.Visibility = Visibility.Visible;
            }

        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            CheckAndCreateWavSaveDir(WavSaveDir);
            CheckEnv();
            CheckMod();

            this.CB_OutputDDevice.ItemsSource = SpeakServer.GetOutputAudioDeviceNames();

            var mq = new MessageQueue();
            mq.SetCallBack(PlaySound);

            Thread thread = new Thread(new ThreadStart(mq.MainLoop));
            thread.Start();

        }

        private void TB_TTS_TextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            TextBox? textBox = sender! as TextBox;
            if (textBox != null)
            {
                TTSServer.DefaultUrl = textBox.Text;
            }
        }

        private void B_Mod_Click(object sender, RoutedEventArgs e)
        {
            if (ModInstaller.Install())
            {
                CheckMod();
            }
        }

        private void B_VB_Click(object sender, RoutedEventArgs e)
        {
            string appName = AppDomain.CurrentDomain.BaseDirectory;
            appName += "\\VBCABLE\\Setup.exe";
            if (File.Exists(appName))
            {
                ProcessStartInfo startInfo = new ProcessStartInfo();
                startInfo.FileName = appName; // 指定外部exe的完整路径
                startInfo.UseShellExecute = true;
                startInfo.Verb = "runas"; // 设置runas使得程序以管理员权限运行

                try
                {
                    Process process = Process.Start(startInfo);
                    process.WaitForExit(); // 如果需要等待程序执行完成可以使用这个方法
                }
                catch (Exception ex)
                {
                    Console.WriteLine("无法以管理员权限启动程序: " + ex.Message);
                }

            }
            else
            {
                MessageBox.Show("安装程序不存在！");
            }
        }
    }
}
