using System;
using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Net;
using System.Security.Policy;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using AdonisUI;
using DynastyParser.Classes;

namespace DynastyParser
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private string _baseUrl;
        private Scraper _scraper;
        private bool _isDark;

        public MainWindow()
        {
            InitializeComponent();

            _isDark = (bool) Properties.Settings.Default["darkmode"];
            ToggleButton button = DarkModeToggleButton;
            button.Content = _isDark ? "Off" : "On";
            ResourceLocator.SetColorScheme(Application.Current.Resources, _isDark ? ResourceLocator.LightColorScheme : ResourceLocator.DarkColorScheme);
        }

        private void ScrapeButton_OnClick(object sender, RoutedEventArgs e)
        {
            bool isUrl = Uri.TryCreate(UrlTextBox.Text, UriKind.Absolute, out var uriResult) 
                          && (uriResult.Scheme == Uri.UriSchemeHttp || uriResult.Scheme == Uri.UriSchemeHttps);
            if (!isUrl) return;

            // Set base download directory and create it
            string mangaDir = UrlTextBox.Text.Split('/').Last();
            Directory.CreateDirectory(mangaDir);

            // Set base URL and scraper
            _baseUrl = Regex.Split(UrlTextBox.Text, @"(?<!\/)\/(?!\/)")[0];
            _scraper = new Scraper(_baseUrl);

            // Scrape chapters
            var chapters = _scraper.ScrapeChapters(UrlTextBox.Text);

            // Start parsing all chapters for pages
            foreach (string chapter in chapters)
            {
                // Set base chapter download directory and create it
                string chapterDir = $"{mangaDir}/{chapter.Split('/').Last()}";
                Directory.CreateDirectory(chapterDir);

                WriteLog($"Now parsing: {chapter}");

                // Scrape images of the chapter
                var images = _scraper.ScrapeImages(chapter);

                // Start parsing all images
                foreach (var image in images)
                {
                    // Grab the file extension
                    string ext = image.Split('.').Last();

                    // If the url with original extension does not return 200 OK, change the extension
                    // Swap extensions between .png and .jpg
                    bool isNotOk = Helpers.CheckUrlResponse(image) != HttpStatusCode.OK;
                    if (isNotOk) ext = ext == "png" ? "jpg" : "png";

                    WriteLog($"├─  Parsed image: {image.Split('.').SkipLast(1).Join(".")}.{ext}");

                    // Download image and save it
                    using (WebClient client = new WebClient())
                    {
                        string directory = $"{chapterDir}/{image.Split('/').Last().Split('.')[0]}.{ext}";

                        client.DownloadFileAsync(new Uri($"{image.Split('.').SkipLast(1).Join(".")}.{ext}"), directory);
                    }
                }

                WriteLog("└─  Parsed all images");
                WriteLog();
            }
        }

        private void OnDropImages(object sender, DragEventArgs e)
        {
            if (!e.Data.GetDataPresent(DataFormats.FileDrop)) return;

            string[] files = (string[])e.Data.GetData(DataFormats.FileDrop);

            ImageGrid.ItemsSource = (files ?? throw new InvalidOperationException()).OrderBy(q=>q).ToArray();

            DragHereText.Visibility = Visibility.Collapsed;
        }

        private void GlueButton_OnClick(object sender, RoutedEventArgs e)
        {
            var imagesList = ImageGrid.SelectedItems;
            string[] images = new string[imagesList.Count];
            imagesList.CopyTo(images, 0);

            string saveLocation = images[0].Split('\\').SkipLast(1).Join("\\");

            Bitmap finalImage = Glue.GlueImages(images);
            finalImage.Save($"{saveLocation}\\glued.png", ImageFormat.Png);
        }

        private void GlueAllButton_OnClick(object sender, RoutedEventArgs e)
        {
            var imagesList = ImageGrid.Items;
            string[] images = new string[imagesList.Count];
            imagesList.CopyTo(images, 0);

            string saveLocation = images[0].Split('\\').SkipLast(1).Join("\\");

            Bitmap finalImage = Glue.GlueImages(images);
            finalImage.Save($"{saveLocation}\\glued.png", ImageFormat.Png);
        }

        private void DarkModeToggleButton_OnClick(object sender, RoutedEventArgs e)
        {
            ToggleButton button = (ToggleButton) sender;
            button.Content = _isDark ? "Off" : "On";

            ResourceLocator.SetColorScheme(Application.Current.Resources, _isDark ? ResourceLocator.LightColorScheme : ResourceLocator.DarkColorScheme);

            _isDark = !_isDark;
            Properties.Settings.Default["darkmode"] = _isDark;
            Properties.Settings.Default.Save();
        }
        

        private void WriteLog(string text = "")
        {
            ScrapeResultsTextBox.Text += Environment.NewLine + text;
        }
    }
}
