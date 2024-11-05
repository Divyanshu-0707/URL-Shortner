using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Threading.Tasks;
using Windows.ApplicationModel.DataTransfer;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using System.Net.Http;
using System.Threading.Tasks;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;


// The Blank Page item template is documented at https://go.microsoft.com/fwlink/?LinkId=402352&clcid=0x409

namespace URL_Shortner
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class MainPage : Page
    {
       

        public MainPage()
        {
            this.InitializeComponent();
        }

        private string GenerateShortURL(string longURL) 
        {
            var random = new Random();
            string shortCode = new string((char)random.Next(97, 122), 6);
            return $"https://short.url/{shortCode}";
        }

        private async void shortURLBTN_Click(object sender, RoutedEventArgs e)
        {
            string longURL =  urlBox.Text;
            if (string.IsNullOrEmpty(longURL) || !Uri.IsWellFormedUriString(longURL, UriKind.Absolute))
                {
                outputTextBox.Text = "Invalid URL";
                return;

            }

            UrlProgress.IsActive = true;
            UrlProgress.Visibility = Visibility.Visible;
            outputTextBox.Visibility = Visibility.Collapsed;


            string shortUrl = await GetShortUrlFromTinyUrl(longURL);
            UrlProgress.IsActive = false;
            UrlProgress.Visibility = Visibility.Collapsed;
            outputTextBox.Visibility= Visibility.Visible;
            outputTextBox.Text = shortUrl ?? "Error generating shortened URL.";
        }

        private async Task<string> GetShortUrlFromTinyUrl(string longUrl)
        {
            using (HttpClient client = new HttpClient())
            {
                try
                {
                    HttpResponseMessage response = await client.GetAsync($"https://tinyurl.com/api-create.php?url={Uri.EscapeDataString(longUrl)}");
                    if (response.IsSuccessStatusCode)
                    {
                        return await response.Content.ReadAsStringAsync();
                    }
                    else
                    {
                        return null;
                    }
                }
                catch (Exception)
                {
                    return null;
                }
            }
        }
   

        private void copytoClip_Click(object sender, RoutedEventArgs e)
        {
            if (!string.IsNullOrEmpty(outputTextBox.Text) && outputTextBox.Text != "Shortened URL will appear here")
            {
                var dataPackage = new DataPackage();
                dataPackage.SetText(outputTextBox.Text);
                Clipboard.SetContent(dataPackage);
            }
        }
    }
}

