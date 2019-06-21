using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Security.Policy;
using System.Text;
using System.Text.RegularExpressions;
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
using DynastyParser.Classes;

namespace DynastyParser
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private string baseUrl;
        private Scraper scraper;

        public MainWindow()
        {
            InitializeComponent();
        }

        private void ScrapeButton_OnClick(object sender, RoutedEventArgs e)
        {
            string mangaDir = UrlTextBox.Text.Split('/').Last();
            Directory.CreateDirectory(mangaDir);

            baseUrl = Regex.Split(UrlTextBox.Text, @"(?<!\/)\/(?!\/)")[0];
            scraper = new Scraper(baseUrl);

            var chapters = scraper.ScrapeChapters(UrlTextBox.Text);
            foreach (string chapter in chapters)
            {
                string chapterDir = $"{mangaDir}/{chapter.Split('/').Last()}";

                Directory.CreateDirectory(chapterDir);

                WriteLog($"Now parsing: {chapter}");
                var images = scraper.ScrapeImages(chapter);

                foreach (var image in images)
                {
                    WriteLog($"├─  Parsed image: {image}");
                    using (WebClient client = new WebClient()) 
                    {
                        client.DownloadFileAsync(new Uri(image), $"{chapterDir}/{image.Split('/').Last()}");
                    }
                }

                WriteLog("└─  Parsed all images");
                WriteLog();
            }

        }

        private void WriteLog(string text = "")
        {
            ScrapeResultsTextBox.Text += Environment.NewLine + text;
        }
    }
}
