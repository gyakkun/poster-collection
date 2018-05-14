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
        private string language;

        public MainPage()
        {
            this.InitializeComponent();
            //Show the waiting ring.
            //MyProgressRing.IsActive = true;
            //MyProgressRing.Visibility = Visibility.Visible;
            viewModel = ViewModel.Instance;
            InitializeList();


            //Kill the waiting ring.
            //MyProgressRing.IsActive = false;
            //MyProgressRing.Visibility = Visibility.Collapsed;
        }

        //private  void showProgressRing() {
        //    MyProgressRing.IsActive = true;
        //    MyProgressRing.Visibility = Visibility.Visible;
        //}

        //private  void killProgressRing() {
        //    MyProgressRing.IsActive = false;
        //    MyProgressRing.Visibility = Visibility.Collapsed;
        //}

        private async void InitializeList()
        {


            try
            {
                if (VideoTypeComboBox.SelectedIndex == 0)
                {
                    String url = String.Format("https://api.themoviedb.org/3/discover/movie?api_key=7888f0042a366f63289ff571b68b7ce0&include_adult=false{0}", language);
                    HttpClient client = new HttpClient();
                    String Jresult = await client.GetStringAsync(url);
                    DataContractJsonSerializer serializer = new DataContractJsonSerializer(typeof(QueryMovieList));
                    MemoryStream ms = new MemoryStream(Encoding.UTF8.GetBytes(Jresult));
                    QueryMovieList queryMovieList = (QueryMovieList)serializer.ReadObject(ms);
                    if (queryMovieList.total_results == 0)
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
                        ListFrame.Navigate(typeof(ListPage), 0);
                    }

                }
                else
                {
                    String url = String.Format("https://api.themoviedb.org/3/discover/tv?api_key=7888f0042a366f63289ff571b68b7ce0&include_adult=false{0}", language);
                    HttpClient client = new HttpClient();
                    String Jresult = await client.GetStringAsync(url);
                    DataContractJsonSerializer serializer = new DataContractJsonSerializer(typeof(QueryTVList));
                    MemoryStream ms = new MemoryStream(Encoding.UTF8.GetBytes(Jresult));
                    QueryTVList queryTVList = (QueryTVList)serializer.ReadObject(ms);

                    if (queryTVList.total_results == 0)
                    {
                        await new Windows.UI.Popups.MessageDialog("Found nothing, please change the key words and try again! ").ShowAsync();
                    }
                    else
                    {
                        viewModel.clear();
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
                        ListFrame.Navigate(typeof(ListPage), 1);
                    }
                }
            }
            catch
            {
                await new Windows.UI.Popups.MessageDialog("Opps! Something wrong happened to the connection, please check your network and try again! ").ShowAsync();
            }

        }

        private void HamburgerButton_Click(object sender, RoutedEventArgs e)
        {
            MySplitView.IsPaneOpen = !MySplitView.IsPaneOpen;
        }

        private void ListBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (Home.IsSelected)
            {
                TitleTextBlock.Text = "Home";
                InitializeList();
            }
            else if (Collection.IsSelected)
            {
                TitleTextBlock.Text = "Collection";
            }
        }

        private async void SearchButton_Click(object sender, RoutedEventArgs e)
        {
            if (Search.Text != "")
            {
                //Show the waiting ring.
                MyProgressRing.IsActive = true;
                MyProgressRing.Visibility = Visibility.Visible;

                try {
                    viewModel.clear();
                    for (int i = 1; i <= 5; i++)
                    {
                        String url = String.Format("https://api.themoviedb.org/3/search/movie?api_key=7888f0042a366f63289ff571b68b7ce0&query={0}&page={1}", Search.Text,i);
                        HttpClient client = new HttpClient();
                        String Jresult = await client.GetStringAsync(url);
                        DataContractJsonSerializer serializer = new DataContractJsonSerializer(typeof(QueryMovieList));
                        MemoryStream ms = new MemoryStream(Encoding.UTF8.GetBytes(Jresult));
                        QueryMovieList queryMovieList = (QueryMovieList)serializer.ReadObject(ms);

                        url = String.Format("https://api.themoviedb.org/3/search/tv?api_key=7888f0042a366f63289ff571b68b7ce0&query={0}&page={1}", Search.Text,i);
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
                            //viewModel.clear();
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
                    this.Frame.Navigate(typeof(ListPage), 2);
                }
                catch
                {
                    await new Windows.UI.Popups.MessageDialog("Opps! Something wrong happened to the connection, please check your network and try again! ").ShowAsync();
                }

                //Kill the waiting ring.
                MyProgressRing.IsActive = false;
                MyProgressRing.Visibility = Visibility.Collapsed;
            }
            else
            {
                await new Windows.UI.Popups.MessageDialog("Please enter key words first! ").ShowAsync();
            }


        }

        private void LanguageComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            switch (LanguageComboBox.SelectedIndex)
            {
                case 0:
                    language = "";
                    break;
                case 1:
                    language = "&with_original_language=en";
                    break;
                case 2:
                    language = "&with_original_language=zh";
                    break;
                case 3:
                    language = "&with_original_language=ja";
                    break;
                case 4:
                    language = "&with_original_language=ko";
                    break;
            }
            InitializeList();
        }

        private void VideoTypeComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            InitializeList();
        }

        private void movieGenreComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            InitializeList();
        }
    }
}
