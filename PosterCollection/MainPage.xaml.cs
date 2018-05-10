using PosterCollection.Models;
using PosterCollection.ViewModels;
using System;
using System.IO;
using System.Net.Http;
using System.Runtime.Serialization.Json;
using System.Text;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

// https://go.microsoft.com/fwlink/?LinkId=402352&clcid=0x804 上介绍了“空白页”项模板

namespace PosterCollection
{
    /// <summary>
    /// 可用于自身或导航至 Frame 内部的空白页。
    /// </summary>
    public sealed partial class MainPage : Page
    {
        private ViewModel viewModel;

        public MainPage()
        {
            this.InitializeComponent();
            viewModel = ViewModel.Instance;
        }

        private async void SearchButton_Click(object sender, RoutedEventArgs e)
        {
            if(Search.Text != "")
            {
                String url = String.Format("https://api.themoviedb.org/3/search/movie?api_key=7888f0042a366f63289ff571b68b7ce0&query={0}", Search.Text);
                HttpClient client = new HttpClient();
                String Jresult = await client.GetStringAsync(url);
                DataContractJsonSerializer serializer = new DataContractJsonSerializer(typeof(QueryList));
                MemoryStream ms = new MemoryStream(Encoding.UTF8.GetBytes(Jresult));
                QueryList queryList = (QueryList)serializer.ReadObject(ms);
                if(queryList.total_results == 0)
                {
                    await new Windows.UI.Popups.MessageDialog("Found nothing, please change the key words and try again! ").ShowAsync();
                }
                else
                {
                    foreach(var result in queryList.results)
                    {
                        result.poster_path = "https://image.tmdb.org/t/p/w500" + result.poster_path;
                        viewModel.AddResult(result);
                    }
                    this.Frame.Navigate(typeof(ListPage));
                }
            }
            else
            {
                await new Windows.UI.Popups.MessageDialog("Please enter key words first! ").ShowAsync();
            }
        }

    }
}
