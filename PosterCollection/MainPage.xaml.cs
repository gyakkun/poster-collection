using PosterCollection.Models;
using PosterCollection.ViewModels;
using System;
using System.IO;
using System.Net.Http;
using System.Runtime.Serialization.Json;
using System.Text;
using Windows.Data.Xml.Dom;
using Windows.UI.Notifications;
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
        private int page = 1;
        private string language = "";
        private string Mgenre = "";
        private string Tgenre = "";
        private string releaseYear = "";
        private string sortBy = "&sort_by=popularity.desc";

        public MainPage()
        {
            this.InitializeComponent();
            //Show the waiting ring.
            //MyProgressRing.IsActive = true;
            //MyProgressRing.Visibility = Visibility.Visible;
            viewModel = ViewModel.Instance;
            InitializeList();
            UpdateTile();
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
                    String url = String.Format("https://api.themoviedb.org/3/discover/movie?api_key=7888f0042a366f63289ff571b68b7ce0&include_adult=false{0}&page={1}{2}{3}{4}", language,page,Mgenre,releaseYear,sortBy);
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
                    String url = String.Format("https://api.themoviedb.org/3/discover/tv?api_key=7888f0042a366f63289ff571b68b7ce0&include_adult=false{0}&page={1}{2}{3}", language,page,Tgenre,sortBy);
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
                if (ListFrame.CanGoBack)
                {
                    ListFrame.GoBack();
                }
            }
            else if (Collection.IsSelected)
            {
                ListFrame.Navigate(typeof(CollectorItems));
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
            page = 1;
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
                    language = "&with_original_language=de";
                    break;
            }
            InitializeList();
        }

        private void VideoTypeComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            page = 1;
            switch (VideoTypeComboBox.SelectedIndex)
            {
                case 0:
                    if(tvGenreComboBox != null && movieGenreComboBox != null&&ReleaseYearComboBox != null)
                    {
                        tvGenreComboBox.Visibility = Visibility.Collapsed;
                        tvGenreTextBlock.Visibility = Visibility.Collapsed;
                        movieGenreComboBox.Visibility = Visibility.Visible;
                        movieGenreTextBlock.Visibility = Visibility.Visible;
                        ReleaseYearComboBox.Visibility = Visibility.Visible;
                        ReleaseYearTextBlock.Visibility = Visibility.Visible;

                    }
                    break;
                case 1:
                    if(tvGenreComboBox != null && movieGenreComboBox != null && ReleaseYearComboBox != null)
                    {
                        tvGenreComboBox.Visibility = Visibility.Visible;
                        tvGenreTextBlock.Visibility = Visibility.Visible;
                        movieGenreComboBox.Visibility = Visibility.Collapsed;
                        movieGenreTextBlock.Visibility = Visibility.Collapsed;
                        ReleaseYearComboBox.Visibility = Visibility.Collapsed;
                        ReleaseYearTextBlock.Visibility = Visibility.Collapsed;
                    }
                    break;
            }
            InitializeList();
        }

        private void movieGenreComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            page = 1;
            switch (movieGenreComboBox.SelectedIndex)
            {
                case 0:
                    Mgenre = "";
                    break;
                case 1:
                    Mgenre = "&with_genres=28";
                    break;
                case 2:
                    Mgenre = "&with_genres=12";
                    break;
                case 3:
                    Mgenre = "&with_genres=16";
                    break;
                case 4:
                    Mgenre = "&with_genres=35";
                    break;
                case 5:
                    Mgenre = "&with_genres=80";
                    break;
                case 6:
                    Mgenre = "&with_genres=99";
                    break;
                case 7:
                    Mgenre = "&with_genres=18";
                    break;
                case 8:
                    Mgenre = "&with_genres=10751";
                    break;
                case 9:
                    Mgenre = "&with_genres=14";
                    break;
                case 10:
                    Mgenre = "&with_genres=36";
                    break;
                case 11:
                    Mgenre = "&with_genres=27";
                    break;
                case 12:
                    Mgenre = "&with_genres=10402";
                    break;
                case 13:
                    Mgenre = "&with_genres=9648";
                    break;
                case 14:
                    Mgenre = "&with_genres=10749";
                    break;
                case 15:
                    Mgenre = "&with_genres=878";
                    break;
                case 16:
                    Mgenre = "&with_genres=10770";
                    break;
                case 17:
                    Mgenre = "&with_genres=53";
                    break;
                case 18:
                    Mgenre = "&with_genres=10752";
                    break;
                case 19:
                    Mgenre = "&with_genres=37";
                    break;
            }
            InitializeList();
        }

        private void PreviousAppBarButton_Click(object sender, RoutedEventArgs e)
        {
            if(page > 1)
            {
                page--;
                InitializeList();
            }
        }

        private void NextAppBarButton_Click(object sender, RoutedEventArgs e)
        {
            if(page < 1000)
            {
                page++;
                InitializeList();
            }
        }

        private void BackAppBarButton_Click(object sender, RoutedEventArgs e)
        {
            if (ListFrame.CanGoBack)
            {
                ListFrame.GoBack();
            }
        }

        private void ListFrame_Navigated(object sender, Windows.UI.Xaml.Navigation.NavigationEventArgs e)
        {
            BackAppBarButton.Visibility = !ListFrame.CurrentSourcePageType.Equals(typeof(ListPage)) ? Visibility.Visible : Visibility.Collapsed;
            pageChangePanel.Visibility = !ListFrame.CurrentSourcePageType.Equals(typeof(ListPage)) ? Visibility.Collapsed : Visibility.Visible;
            FilterSelectPanel.Visibility = !ListFrame.CurrentSourcePageType.Equals(typeof(ListPage)) ? Visibility.Collapsed : Visibility.Visible;
        }

        private void tvGenreComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            page = 1;
            switch (tvGenreComboBox.SelectedIndex)
            {
                case 0:
                    Tgenre = "";
                    break;
                case 1:
                    Tgenre = "&with_genres=10759";
                    break;
                case 2:
                    Tgenre = "&with_genres=16";
                    break;
                case 3:
                    Tgenre = "&with_genres=35";
                    break;
                case 4:
                    Tgenre = "&with_genres=80";
                    break;
                case 5:
                    Tgenre = "&with_genres=99";
                    break;
                case 6:
                    Tgenre = "&with_genres=18";
                    break;
                case 7:
                    Tgenre = "&with_genres=10751";
                    break;
                case 8:
                    Tgenre = "&with_genres=10762";
                    break;
                case 9:
                    Tgenre = "&with_genres=9648";
                    break;
                case 10:
                    Tgenre = "&with_genres=10763";
                    break;
                case 11:
                    Tgenre = "&with_genres=10764";
                    break;
                case 12:
                    Tgenre = "&with_genres=10765";
                    break;
                case 13:
                    Tgenre = "&with_genres=10766";
                    break;
                case 14:
                    Tgenre = "&with_genres=10767";
                    break;
                case 15:
                    Tgenre = "&with_genres=10768";
                    break;
                case 16:
                    Tgenre = "&with_genres=37";
                    break;
            }
            InitializeList();
        }

        private void ReleaseYearComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            page = 1;
            switch (ReleaseYearComboBox.SelectedIndex)
            {
                case 0:
                    releaseYear = "";
                    break;
                case 1:
                    releaseYear = "&primary_release_date.gte=2018-01-01&primary_release_date.lte=2018-12-31";
                    break;
                case 2:
                    releaseYear = "&primary_release_date.gte=2017-01-01&primary_release_date.lte=2017-12-31";
                    break;
                case 3:
                    releaseYear = "&primary_release_date.gte=2016-01-01&primary_release_date.lte=2016-12-31";
                    break;
                case 4:
                    releaseYear = "&primary_release_date.gte=2015-01-01&primary_release_date.lte=2015-12-31";
                    break;
                case 5:
                    releaseYear = "&primary_release_date.gte=2014-01-01&primary_release_date.lte=2014-12-31";
                    break;
                case 6:
                    releaseYear = "&primary_release_date.gte=2013-01-01&primary_release_date.lte=2013-12-31";
                    break;
                case 7:
                    releaseYear = "&primary_release_date.gte=2012-01-01&primary_release_date.lte=2012-12-31";
                    break;
                case 8:
                    releaseYear = "&primary_release_date.gte=2011-01-01&primary_release_date.lte=2011-12-31";
                    break;
                case 9:
                    releaseYear = "&primary_release_date.gte=2010-01-01&primary_release_date.lte=2010-12-31";
                    break;
                case 10:
                    releaseYear = "&primary_release_date.lte=2009-12-31";
                    break;
            }
            InitializeList();
        }

        private void SortByComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            page = 1;
            switch (SortByComboBox.SelectedIndex)
            {
                case 0:
                    sortBy = "&sort_by=popularity.desc";
                    break;
                case 1:
                    sortBy = "&sort_by=vote_average.desc";
                    break;
            }
            InitializeList();
        }
        private void UpdateTile()
        {
            
            var updater = TileUpdateManager.CreateTileUpdaterForApplication();

            
            updater.EnableNotificationQueue(true);
            updater.Clear();
            int itemCount = 0;

            //然后这里是重点：记得分3步走：
            foreach (var item in viewModel.Starlist)
            {
                //1：创建xml对象，这里看你想显示几种动态磁贴，如果想显示正方形和长方形的，那就分别设置一个动态磁贴类型即可。
                //下面这两个分别是矩形的动态磁贴，和方形的动态磁贴，具体样式，自己可以去微软官网查一查。我这里用到的是换行的文字形式。
                XmlDocument xml = new XmlDocument();
                xml.LoadXml(File.ReadAllText("tiles.xml"));


                XmlNodeList text = xml.GetElementsByTagName("text");
                ((XmlElement)text[2]).InnerText = item.title;
                ((XmlElement)text[3]).InnerText = item.comment;
                ((XmlElement)text[4]).InnerText = item.title;
                ((XmlElement)text[5]).InnerText = item.comment;
                ((XmlElement)text[6]).InnerText = item.title;
                ((XmlElement)text[7]).InnerText = item.comment;

                //3.然后用Update方法来更新这个磁贴
                updater.Update(new TileNotification(xml));

                //4.最后这里需要注意的是微软规定动态磁贴的队列数目小于5个，所以这里做出判断。
                if (itemCount++ > 5) break;
            }

        }
    }
}
