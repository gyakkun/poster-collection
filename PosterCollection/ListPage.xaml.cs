using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;
using PosterCollection.ViewModels;
using PosterCollection.Models;
using System.Net.Http;
using System.Runtime.Serialization.Json;
using System.Text;

// https://go.microsoft.com/fwlink/?LinkId=234238 上介绍了“空白页”项模板

namespace PosterCollection
{
    /// <summary>
    /// 可用于自身或导航至 Frame 内部的空白页。
    /// </summary>
    public sealed partial class ListPage : Page
    {
        private ViewModel viewModel;

        public ListPage()
        {
            this.InitializeComponent();
            viewModel = ViewModel.Instance;
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            viewModel = ViewModel.Instance;
        }

        private async void GridView_MovieItemClick(object sender, ItemClickEventArgs e)
        {
            try
            {
                var item = (MovieResult)e.ClickedItem;
                String url = String.Format("https://api.themoviedb.org/3/movie/{0}?api_key=7888f0042a366f63289ff571b68b7ce0&append_to_response=casts", item.id);
                HttpClient client = new HttpClient();
                String Jresult = await client.GetStringAsync(url);
                DataContractJsonSerializer serializer = new DataContractJsonSerializer(typeof(MovieDetail));
                MemoryStream ms = new MemoryStream(Encoding.UTF8.GetBytes(Jresult));
                viewModel.TheMovieDetail = (MovieDetail)serializer.ReadObject(ms);
                this.Frame.Navigate(typeof(DetailPage), 0);
            }
            catch
            {
                await new Windows.UI.Popups.MessageDialog("Opps! This item cannot be serialized, please try another item! ").ShowAsync();
            }
            
        }

        private async void GridView_TVItemClick(object sender, ItemClickEventArgs e)
        {
            try { 
                var item = (TVResult)e.ClickedItem;
                String url = String.Format("https://api.themoviedb.org/3/tv/{0}?api_key=7888f0042a366f63289ff571b68b7ce0&append_to_response=casts", item.id);
                HttpClient client = new HttpClient();
                String Jresult = await client.GetStringAsync(url);
                DataContractJsonSerializer serializer = new DataContractJsonSerializer(typeof(TVDetail));
                MemoryStream ms = new MemoryStream(Encoding.UTF8.GetBytes(Jresult));
                viewModel.TheTVDetail = (TVDetail)serializer.ReadObject(ms);
                this.Frame.Navigate(typeof(DetailPage), 1);
            }
            catch
            {
                await new Windows.UI.Popups.MessageDialog("Opps! This item cannot be serialized, please try another item! ").ShowAsync();
            }
        }

        private async void SearchButton_Click(object sender, RoutedEventArgs e)
        {
            if (Search.Text != "")
            {
                try
                {
                    String url = String.Format("https://api.themoviedb.org/3/search/movie?api_key=7888f0042a366f63289ff571b68b7ce0&query={0}", Search.Text);
                    HttpClient client = new HttpClient();
                    String Jresult = await client.GetStringAsync(url);
                    DataContractJsonSerializer serializer = new DataContractJsonSerializer(typeof(QueryMovieList));
                    MemoryStream ms = new MemoryStream(Encoding.UTF8.GetBytes(Jresult));
                    QueryMovieList queryMovieList = (QueryMovieList)serializer.ReadObject(ms);

                    url = String.Format("https://api.themoviedb.org/3/search/tv?api_key=7888f0042a366f63289ff571b68b7ce0&query={0}", Search.Text);
                    Jresult = await client.GetStringAsync(url);
                    serializer = new DataContractJsonSerializer(typeof(QueryTVList));
                    ms = new MemoryStream(Encoding.UTF8.GetBytes(Jresult));
                    QueryTVList queryTVList = (QueryTVList)serializer.ReadObject(ms);

                    if (queryMovieList.total_results + queryTVList.total_results == 0)
                    {
                        await new Windows.UI.Popups.MessageDialog("Found nothing, please change the key words and try again! ").ShowAsync();
                    }
                    else
                    {
                        viewModel.clear();
                        foreach (var result in queryMovieList.results)
                        {
                            if (result.poster_path != null)
                            {
                                result.poster_path = "https://image.tmdb.org/t/p/w500" + result.poster_path;
                            }
                            else
                            {
                                result.poster_path = "Assets/defaultPoster.jpg";
                            }
                            viewModel.AddMovieResult(result);
                        }
                        foreach (var result in queryTVList.results)
                        {
                            if (result.poster_path != null)
                            {
                                result.poster_path = "https://image.tmdb.org/t/p/w500" + result.poster_path;
                            }
                            else
                            {
                                result.poster_path = "Assets/defaultPoster.jpg";
                            }
                            viewModel.AddTVResult(result);
                        }
                    }
                }
                catch
                {
                    await new Windows.UI.Popups.MessageDialog("Opps! Something wrong happened to the connection, please check your network and try again! ").ShowAsync();
                }
            }
            else
            {
                await new Windows.UI.Popups.MessageDialog("Please enter key words first! ").ShowAsync();
            }
        }
    }
}
