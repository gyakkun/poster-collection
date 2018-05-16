using System;
using System.Collections.Generic;
using System.IO;
using System.Net.Http;
using Windows.Storage;
using Windows.Storage.Pickers;
using Windows.Storage.Streams;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Navigation;

// https://go.microsoft.com/fwlink/?LinkId=234238 上介绍了“空白页”项模板

namespace PosterCollection
{
    /// <summary>
    /// 可用于自身或导航至 Frame 内部的空白页。
    /// </summary>
    public sealed partial class ShowPosterPage : Page
    {
        String url;

        public ShowPosterPage()
        {
            this.InitializeComponent();
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            if (e.Parameter is String)
            {
                url = (String)e.Parameter;
                url = url.Substring(0, 26) + "/original" + url.Substring(31);
            }
        }

        private async void savePictureAppBarButton_Click(object sender, RoutedEventArgs e)
        {
            FileSavePicker fileSavePicker = new FileSavePicker();
            fileSavePicker.SuggestedStartLocation = PickerLocationId.PicturesLibrary;
            fileSavePicker.FileTypeChoices.Add("图片", new List<string>() { ".jpg", ".png", ".jpeg", ".bmp" });
            fileSavePicker.SuggestedFileName = "image";

            var outputFile = await fileSavePicker.PickSaveFileAsync();

            if (outputFile == null)
            {
                // The user cancelled the picking operation
                return;
            }
            HttpClient client = new HttpClient();
            Stream Stream = await client.GetStreamAsync(new Uri(url));
            using (IRandomAccessStream stream = await outputFile.OpenAsync(FileAccessMode.ReadWrite))
            {

                await Stream.CopyToAsync(stream.AsStreamForWrite());

            }
        }
    }
}
