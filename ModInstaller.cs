using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace DRGSoundPad
{
    internal class ModInstaller
    {

        public static string GetMD5Hash(string filePath)
        {
            using (var md5 = MD5.Create())
            {
                using (var stream = File.OpenRead(filePath))
                {
                    var hash = md5.ComputeHash(stream);
                    return BitConverter.ToString(hash).Replace("-", "").ToUpperInvariant();
                }
            }
        }

        public static bool CheckModInstall()
        {
            string drgPath = GetGamePath();
            var modPath = Path.Combine(drgPath, "Mods\\DRGSoundPad\\dlls\\main.dll");
            if (!File.Exists(modPath))
            {
                return false;
            }

            var ourModPath = ".\\UE4SS\\Mods\\DRGSoundPad\\dlls\\main.dll";
            if (GetMD5Hash(ourModPath) != GetMD5Hash(modPath))
            {
                return false;
            }

            // TODO: check file md5
            return true;
        }

        private static string GetGamePath()
        {
            DriveInfo[] drives = DriveInfo.GetDrives();
            string drgPath = "";

            //TODO: check dirver c
            foreach (DriveInfo drive in drives)
            {
                drgPath = drive.Name + "SteamLibrary\\steamapps\\common\\Deep Rock Galactic\\FSD\\Binaries\\Win64\\";
                if (Directory.Exists(drgPath))
                {
                    break;
                }
            }

            return drgPath;
        }


        public static void CopyDirectory(string sourceDir, string targetDir)
        {
            DirectoryInfo dir = new DirectoryInfo(sourceDir);
            DirectoryInfo[] dirs = dir.GetDirectories();

            // If the destination directory does not exist, create it.
            if (!Directory.Exists(targetDir))
            {
                Directory.CreateDirectory(targetDir);
            }

            // Get the files in the directory and copy them to the new location.
            FileInfo[] files = dir.GetFiles();
            foreach (FileInfo file in files)
            {
                string tempPath = Path.Combine(targetDir, file.Name);
                file.CopyTo(tempPath, true);
            }

            // If copying subdirectories, copy them and their contents to the new location.
            foreach (DirectoryInfo subdir in dirs)
            {
                string tempPath = Path.Combine(targetDir, subdir.Name);
                CopyDirectory(subdir.FullName, tempPath);
            }
        }

        public static bool Install()
        {
            var drgPath = GetGamePath();
            if (string.IsNullOrEmpty(drgPath))
            {
                MessageBox.Show("无法找到游戏安装目录，请手动安装！");
                return false;
            }

            try
            {
                CopyDirectory("./UE4SS", drgPath);
            }
            catch (Exception ex)
            {
                MessageBox.Show("安装错误，请手动安装！");
                return false;
            }

            MessageBox.Show("安装成功！");
            return true;
        }


    }
}
