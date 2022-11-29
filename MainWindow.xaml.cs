using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
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

namespace De_World_Launcher
{
    /// <summary>
    /// Логика взаимодействия для MainWindow.xaml
    /// </summary>

    public partial class MainWindow : Window
    {
        void Download_file(string requestString, string path)
        {
            HttpClient httpClient = new HttpClient();
            var GetTask = httpClient.GetAsync(requestString);
            GetTask.Wait(1000); // WebCommsTimeout is in milliseconds

            if (!GetTask.Result.IsSuccessStatusCode)
            {
                // write an error
                return;
            }

            using (var fs = new FileStream(path, FileMode.CreateNew))
            {
                var ResponseTask = GetTask.Result.Content.CopyToAsync(fs);
                ResponseTask.Wait(1000);
            }
            System.Threading.Thread.Sleep(200);
        }

        string game_ver;
        string ver = "0.1.1";
        WebClient client = new WebClient();
        string fullPath = Environment.CurrentDirectory;
        void setup_update(bool in_st)
        {
            string dwnl = client.DownloadString("https://raw.githubusercontent.com/Delfi1/De_Launcher/master/version.txt");

            if (dwnl.Contains(ver))
            {
                if (!in_st)
                {
                    MessageBox.Show("Версия приложения: " + ver + "\n" + "Версия приложения на сервере: " + dwnl, "Уведомление", MessageBoxButton.OK);
                }
            }
            else
            {
                if (!in_st) { MessageBox.Show("Версия приложения:" + ver + "\n" + "Версия приложения на сервере: " + dwnl, "Уведомление", MessageBoxButton.OK); }
                var result = MessageBox.Show("Обнаружена новая версия! Идет установка файлов...", "Update", MessageBoxButton.OKCancel);
                if (result == MessageBoxResult.OK)
                {
                    File.Move(fullPath + "\\DeWorld.exe", fullPath + "\\DeWorld_old.exe");
                    string requestString = @"https://github.com/Delfi1/De_Launcher/blob/master/bin/Release/De_World%20Launcher.exe?raw=true";
                    HttpClient httpClient = new HttpClient();
                    var GetTask = httpClient.GetAsync(requestString);
                    GetTask.Wait(1000); // WebCommsTimeout is in milliseconds

                    if (!GetTask.Result.IsSuccessStatusCode)
                    {
                        // write an error
                        return;
                    }

                    using (var fs = new FileStream(fullPath + "\\DeWorld.exe", FileMode.CreateNew))
                    {
                        var ResponseTask = GetTask.Result.Content.CopyToAsync(fs);
                        ResponseTask.Wait(1000);
                    }
                    System.Diagnostics.Process.Start(fullPath + "\\DeWorld.exe");
                    Application.Current.Shutdown();
                }
            }
        }
        public MainWindow()
        {
            InitializeComponent();
            if (!File.Exists(fullPath + "\\Game\\ver.txt")){
                StreamWriter sw = new StreamWriter(Environment.CurrentDirectory + "\\Game\\ver.txt");
                sw.Close();
            }

            StreamReader rd = new StreamReader(Environment.CurrentDirectory + "\\Game\\ver.txt");
            game_ver = rd.ReadLine();
            rd.Close();
            this.Title = "De:World Launcher " + ver;

            VersionText.Text = "Current version\n" + ver;

            //setup_update(true);
    
            if (File.Exists(fullPath + "\\DeWorld_old.exe"))
            {
                File.Delete(fullPath + "\\DeWorld_old.exe");
            }
        
        }
        private void Update_btn_Click(object sender, RoutedEventArgs e)
        {
            setup_update(false);
        }

        private void Launch_btn_Click(object sender, RoutedEventArgs e)
        {
            //MessageBox.Show("" + game_ver);
            if (!File.Exists(fullPath + "\\Game\\Test1.exe")){
                Download_file("https://github.com/Delfi1/Godot_Test/blob/master/Export/Test1.exe?raw=true", fullPath + "\\Game\\Test1.exe");
                Download_file("https://github.com/Delfi1/Godot_Test/blob/master/Export/Test1.pck?raw=true", fullPath + "\\Game\\Test1.pck");
            }
            else{
                if (client.DownloadString("https://raw.githubusercontent.com/Delfi1/De_Launcher/master/Game.txt").Contains(game_ver)){
                    System.Diagnostics.Process.Start(fullPath + "\\Game\\Test1.exe");
                }
                else{
                    Download_file("https://github.com/Delfi1/Godot_Test/blob/master/Export/Test1.pck?raw=true", fullPath + "\\Game\\Test1.pck");
                    game_ver = client.DownloadString("https://raw.githubusercontent.com/Delfi1/De_Launcher/master/version.txt");
                    StreamWriter sw = new StreamWriter(Environment.CurrentDirectory + "\\Game\\ver.txt");
                    sw.Write(game_ver);
                    sw.Close();
                }
            }
        }
    }
}
