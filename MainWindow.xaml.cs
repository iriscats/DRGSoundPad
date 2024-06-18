using Microsoft.VisualBasic.Devices;
using System;
using System.Collections.Generic;
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

        private int vbDevice = 0;
        private const string WavSaveDir = "Sound";
        public MainWindow()
        {
            InitializeComponent();

            vbDevice = SpeakServer.GetOutputDeviceID("CABLE Input (VB-Audio Virtual C");
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

        async void PlaySound(string text)
        {
            string audioName = ComputeMD5Hash(text) + ".wav";
            string audioPath = System.IO.Path.Combine(WavSaveDir, audioName);
            if (!File.Exists(audioPath))
            {
                await TTSServer.DownloadFile(text, audioPath);
            }
            SpeakServer.PlayAudioToSpecificDevice(audioPath, vbDevice, false, 100, false);
            SpeakServer.PlayAudioex(audioPath, 0, 1);

            Dispatcher.Invoke(() =>
            {
                this.TB_Log.Text += $"Recv: {text}\n";
            });

        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            CheckAndCreateWavSaveDir(WavSaveDir);

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
    }
}
