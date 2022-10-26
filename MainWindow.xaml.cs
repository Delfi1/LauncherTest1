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
        string ver = "0.0.4";
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
                MessageBox.Show("Обнаружена новая версия! Идет установка файлов...", "Update", MessageBoxButton.OK);
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
                System.Windows.Application.Current.Shutdown();
            }
        }
        public MainWindow()
        {
            InitializeComponent();

            this.Title = "De:World Launcher " + ver;

            VersionText.Text = "Current version\n" + ver;

            setup_update(true);
    
            if (File.Exists(fullPath + "\\DeWorld_old.exe"))
            {
                File.Delete(fullPath + "\\DeWorld_old.exe");
            }
        
        }
        private void Update_btn_Click(object sender, RoutedEventArgs e)
        {
            setup_update(false);
        }
    }
}
