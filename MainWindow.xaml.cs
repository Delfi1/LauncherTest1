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
        async void Download_file(string requestString, string path)
        {
            HttpClient httpClient = new HttpClient();
            var GetTask = httpClient.GetAsync(requestString);
            await Task.Delay(1000); // WebCommsTimeout is in milliseconds

            if (!GetTask.Result.IsSuccessStatusCode)
            {
                // write an error
                return;
            }

            using (var fs = new FileStream(path, FileMode.CreateNew))
            {
                var ResponseTask = GetTask.Result.Content.CopyToAsync(fs);
                await Task.Delay(1000);
            }
            await Task.Delay(200);
        }

        string game_ver;
        void Update_UI(){
            StreamReader rd = new StreamReader(Environment.CurrentDirectory + "\\Game\\ver.txt");
            game_ver = rd.ReadLine();
            rd.Close();
            VersionGameText.Text = "Game version:\n" + game_ver;
        }

        string ver = "0.2.4";
        WebClient client = new WebClient();
        string fullPath = Environment.CurrentDirectory;
        async void setup_update(bool in_st)
        {
            Uri SUi = new Uri("https://raw.githubusercontent.com/Delfi1/De_Launcher/master/version.txt");
            string dwnl = client.DownloadString(SUi);
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
                var result = MessageBox.Show("Обнаружена новая версия! Установить?", "Update", MessageBoxButton.YesNo);
                if (result == MessageBoxResult.Yes)
                {
                    File.Move(fullPath + "\\DeWorld.exe", fullPath + "\\DeWorld_old.exe");
                    string requestString = @"https://github.com/Delfi1/De_Launcher/blob/master/bin/Release/De_World%20Launcher.exe?raw=true";
                    Download_file(requestString, fullPath + "\\DeWorld.exe");
                    System.Diagnostics.Process.Start(fullPath + "\\DeWorld.exe");
                    await Task.Delay(100);
                    Application.Current.Shutdown();
                }
            }
        }
        
        async void Check(){
            setup_update(true);
            await Task.Delay(60000);
        }

        public MainWindow()
        {
            InitializeComponent();

            if(!Directory.Exists(fullPath + "\\Game")){
                Directory.CreateDirectory(fullPath + "\\Game");
            }

            if (!File.Exists(fullPath + "\\Game\\ver.txt")){
                StreamWriter sw = new StreamWriter(Environment.CurrentDirectory + "\\Game\\ver.txt");
                sw.Close();
                VersionGameText.Text = "\nDownload game:";
            }
            else{
                Update_UI();
            }

            this.Title = "De:World Launcher " + ver;

            VersionText.Text = "Current version\n" + ver;
    
            if (File.Exists(fullPath + "\\DeWorld_old.exe"))
            {
                File.Delete(fullPath + "\\DeWorld_old.exe");
            }
            Check();
        }
        private void Update_btn_Click(object sender, RoutedEventArgs e)
        {
            setup_update(false);
        }

        private void Launch_btn_Click(object sender, RoutedEventArgs e)
        {
            StreamWriter sw2 = new StreamWriter(fullPath + "\\Game\\ver.txt");
            sw2.WriteLine(client.DownloadString("https://raw.githubusercontent.com/Delfi1/De_Launcher/master/Game.txt"));
            sw2.Close();
            Update_UI();
            //MessageBox.Show("" + game_ver);
            if (!File.Exists(fullPath + "\\Game\\Test1.exe")){
                Download_file("https://github.com/Delfi1/Godot_Test/blob/master/Export/Test1.exe?raw=true", fullPath + "\\Game\\Test1.exe");
                Download_file("https://github.com/Delfi1/Godot_Test/blob/master/Export/Test1.pck?raw=true", fullPath + "\\Game\\Test1.pck");
                File.Delete(fullPath + "\\Game\\ver.txt");
                System.Diagnostics.Process.Start(fullPath + "\\Game\\Test1.exe");
                StreamWriter sw3 = new StreamWriter(fullPath + "\\Game\\ver.txt");
                sw3.WriteLine(client.DownloadString("https://raw.githubusercontent.com/Delfi1/De_Launcher/master/Game.txt"));
                sw3.Close();
            }
            else{
                if (game_ver.Contains(client.DownloadString("https://raw.githubusercontent.com/Delfi1/De_Launcher/master/Game.txt"))){
                    System.Diagnostics.Process.Start(fullPath + "\\Game\\Test1.exe");
                }
                else{
                    File.Delete(fullPath + "\\Game\\Test1.pck");
                    File.Delete(fullPath + "\\Game\\ver.txt");
                    Download_file("https://github.com/Delfi1/Godot_Test/blob/master/Export/Test1.pck?raw=true", fullPath + "\\Game\\Test1.pck");
                    StreamWriter sw4 = new StreamWriter(fullPath + "\\Game\\ver.txt");
                    sw4.WriteLine(client.DownloadString("https://raw.githubusercontent.com/Delfi1/De_Launcher/master/Game.txt"));
                    sw4.Close();
                    System.Diagnostics.Process.Start(fullPath + "\\Game\\Test1.exe");
                }
            }
        }
    }
}
