using System;
using System.IO;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;
using PosterCollection.ViewModels;
using PosterCollection.Models;
using System.Net.Http;
using System.Runtime.Serialization.Json;
using System.Text;
using Windows.UI.Xaml.Media.Imaging;

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
            if (e.Parameter is int)
            {
                int flag = (int)e.Parameter;
                //主界面ListFrame电影类导航到该界面，只显示电影
                if (flag == 0)
                {
                    searchGrid.Visibility = Visibility.Collapsed;
                    TVPanel.Visibility = Visibility.Collapsed;
                    movieTextBlock.Visibility = Visibility.Collapsed;
                }
                //主界面ListFrame电视剧类导航到该界面，只显示电视剧
                else if(flag == 1)
                {
                    searchGrid.Visibility = Visibility.Collapsed;
                    MoviePanel.Visibility = Visibility.Collapsed;
                    tvTextBlock.Visibility = Visibility.Collapsed;
                }
                //搜索导航到该界面，都要显示
                else if(flag == 2)
                {
                    ImageBrush background = new ImageBrush();
                    background.ImageSource = new BitmapImage(new Uri(this.BaseUri, "Assets/defaultBackground.jpg"));
                    listGrid.Background = background;
                }
            }
        }
        //点击了一个电影
        private async void GridView_MovieItemClick(object sender, ItemClickEventArgs e)
        {
            try
            {
                var item = (MovieResult)e.ClickedItem;
                //API访问该项的详细信息，剩下的请参看主界面的注释
                String url = String.Format("https://api.themoviedb.org/3/movie/{0}?api_key=7888f0042a366f63289ff571b68b7ce0&append_to_response=casts", item.id);
                HttpClient client = new HttpClient();
                String Jresult = await client.GetStringAsync(url);
                DataContractJsonSerializer serializer = new DataContractJsonSerializer(typeof(MovieDetail));
                MemoryStream ms = new MemoryStream(Encoding.UTF8.GetBytes(Jresult));
                viewModel.TheMovieDetail = (MovieDetail)serializer.ReadObject(ms);
                //跳转到详情页面
                this.Frame.Navigate(typeof(DetailPage), 0);
                //电影背景改为可访问网址或默认背景
                if (viewModel.TheMovieDetail.backdrop_path != null)
                {
                    viewModel.TheMovieDetail.backdrop_path = "https://image.tmdb.org/t/p/original" + viewModel.TheMovieDetail.backdrop_path;
                }
                else
                {
                    viewModel.TheMovieDetail.backdrop_path = "Assets/defaultBackground.jpg";
                }
                //电影海报改为可访问网址或默认海报
                if (viewModel.TheMovieDetail.poster_path != null)
                {
                    viewModel.TheMovieDetail.poster_path = "https://image.tmdb.org/t/p/w500" + viewModel.TheMovieDetail.poster_path;
                }
                else
                {
                    viewModel.TheMovieDetail.poster_path = "Assets/defaultPoster.jpg";
                }
                //演员头像改为可访问网址或默认头像
                foreach (var cast in viewModel.TheMovieDetail.casts.cast)
                {
                    if (cast.profile_path != null)
                    {
                        cast.profile_path = "https://image.tmdb.org/t/p/w500" + cast.profile_path;
                    }
                    else
                    {
                        cast.profile_path = "Assets/defaultPhoto.jpg";
                    }
                }
                
            }
            catch
            {
                await new Windows.UI.Popups.MessageDialog("Opps! This item cannot be serialized, please try another item! ").ShowAsync();
            }
            
        }
        //点击一个电视剧
        private async void GridView_TVItemClick(object sender, ItemClickEventArgs e)
        {
            try { 
                //同理
                var item = (TVResult)e.ClickedItem;
                String url = String.Format("https://api.themoviedb.org/3/tv/{0}?api_key=7888f0042a366f63289ff571b68b7ce0&append_to_response=casts", item.id);
                HttpClient client = new HttpClient();
                String Jresult = await client.GetStringAsync(url);
                DataContractJsonSerializer serializer = new DataContractJsonSerializer(typeof(TVDetail));
                MemoryStream ms = new MemoryStream(Encoding.UTF8.GetBytes(Jresult));
                viewModel.TheTVDetail = (TVDetail)serializer.ReadObject(ms);
                this.Frame.Navigate(typeof(DetailPage), 1);
                //背景
                if (viewModel.TheTVDetail.backdrop_path != null)
                {
                    viewModel.TheTVDetail.backdrop_path = "https://image.tmdb.org/t/p/original" + viewModel.TheTVDetail.backdrop_path;
                }
                else
                {
                    viewModel.TheTVDetail.backdrop_path = "Assets/defaultBackground.jpg";
                }
                //海报
                if (viewModel.TheTVDetail.poster_path != null)
                {
                    viewModel.TheTVDetail.poster_path = "https://image.tmdb.org/t/p/w500" + viewModel.TheTVDetail.poster_path;
                }
                else
                {
                    viewModel.TheTVDetail.poster_path = "Assets/defaultPoster.jpg";
                }
                //每一季的海报
                foreach (var season in viewModel.TheTVDetail.seasons)
                {
                    if (season.poster_path != null)
                    {
                        season.poster_path = "https://image.tmdb.org/t/p/w500" + season.poster_path;
                    }
                    else
                    {
                        season.poster_path = "Assets/defaultPoster.jpg";
                    }
                }
                
            }
            catch
            {
                await new Windows.UI.Popups.MessageDialog("Opps! This item cannot be serialized, please try another item! ").ShowAsync();
            }
        }
                
        //关键词搜索
        private async void SearchButton_Click(object sender, RoutedEventArgs e)
        {
            if (Search.Text != "")
            {
                ProgressRingInListPage.IsActive = true;
                ProgressRingInListPage.Visibility = Visibility.Visible;
                try
                {
                    int tmp = 0;
                    
                    for (int i = 1; i <= 5; i++)
                    {
                        //请求电影
                        String url = String.Format("https://api.themoviedb.org/3/search/movie?api_key=7888f0042a366f63289ff571b68b7ce0&query={0}&page={1}", Search.Text,i);
                        HttpClient client = new HttpClient();
                        String Jresult = await client.GetStringAsync(url);
                        DataContractJsonSerializer serializer = new DataContractJsonSerializer(typeof(QueryMovieList));
                        MemoryStream ms = new MemoryStream(Encoding.UTF8.GetBytes(Jresult));
                        QueryMovieList queryMovieList = (QueryMovieList)serializer.ReadObject(ms);
                        //请求电视剧
                        url = String.Format("https://api.themoviedb.org/3/search/tv?api_key=7888f0042a366f63289ff571b68b7ce0&query={0}&page={1}", Search.Text,i);
                        Jresult = await client.GetStringAsync(url);
                        serializer = new DataContractJsonSerializer(typeof(QueryTVList));
                        ms = new MemoryStream(Encoding.UTF8.GetBytes(Jresult));
                        QueryTVList queryTVList = (QueryTVList)serializer.ReadObject(ms);

                        if (queryMovieList.total_results + queryTVList.total_results == 0)
                        {
                            await new Windows.UI.Popups.MessageDialog("Found nothing, please change the key words and try again! ").ShowAsync();
                            break;
                        }
                        else
                        {
                            if(tmp == 0)
                            {
                                viewModel.clear();
                            }
                            tmp ++;
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
                }
                catch
                {
                    await new Windows.UI.Popups.MessageDialog("Opps! Something wrong happened to the connection, please check your network and try again! ").ShowAsync();
                }
                
                //Kill the waiting ring.
                ProgressRingInListPage.IsActive = false;
                ProgressRingInListPage.Visibility = Visibility.Collapsed;

            }
            else
            {
                await new Windows.UI.Popups.MessageDialog("Please enter key words first! ").ShowAsync();
            }
        }
    }
}
