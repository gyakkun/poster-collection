using PosterCollection.Models;
using PosterCollection.Service;
using PosterCollection.ViewModels;
using System;
using System.IO;
using System.Net.Http;
using System.Runtime.Serialization.Json;
using System.Text;
using System.Xml.Linq;
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
            //载入数据库，初始化ViewModel
            viewModel = ViewModel.Instance;
            InitializeList();
            //程序开始时，载入了数据库，按照收藏的视频数据生成一次磁贴
            TileService.GenerateTiles();
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
                //如果选择的是电影类型
                if (VideoTypeComboBox.SelectedIndex == 0)
                {
                    //API请求，language是电影语言, page是搜索的页，用于支持翻页功能，翻一次页请求一次（API本身的限制只能这么做）, Mgenre电影的类型, releaseYear电影放映日期, sortBy电影以热度还是评分排序
                    String url = String.Format("https://api.themoviedb.org/3/discover/movie?api_key=7888f0042a366f63289ff571b68b7ce0&include_adult=false{0}&page={1}{2}{3}{4}", language, page, Mgenre, releaseYear, sortBy);
                    HttpClient client = new HttpClient();
                    String Jresult = await client.GetStringAsync(url);

                    //本想把这句放在最前面，跳转更快，但是ListFrame实例化需要一点时间，放在前面会报空指针的错误，只好先请求网络给程序一点时间
                    ListFrame.Navigate(typeof(ListPage), 0);

                    //序列化电影列表，通过其解析Json，Models中除了Starlist，其他类都是通过网站json2csharp.com自动生成的
                    DataContractJsonSerializer serializer = new DataContractJsonSerializer(typeof(QueryMovieList));
                    MemoryStream ms = new MemoryStream(Encoding.UTF8.GetBytes(Jresult));
                    QueryMovieList queryMovieList = (QueryMovieList)serializer.ReadObject(ms);

                    //没有这类型的电影
                    if (queryMovieList.total_results == 0)
                    {
                        await new Windows.UI.Popups.MessageDialog("Found nothing, please change the key words and try again! ").ShowAsync();
                    }
                    else
                    {
                        viewModel.clear();
                        foreach (var result in queryMovieList.results)
                        {
                            //该电影有海报
                            if (result.poster_path != null)
                            {
                                //修改为能访问的网址
                                result.poster_path = "https://image.tmdb.org/t/p/w500" + result.poster_path;
                            }
                            //没有海报
                            else
                            {
                                //使用默认海报
                                result.poster_path = "Assets/defaultPoster.jpg";
                            }
                            //添加到ViewModel
                            viewModel.AddMovieResult(result);
                        }
                    }

                }
                else
                {
                    //同理
                    String url = String.Format("https://api.themoviedb.org/3/discover/tv?api_key=7888f0042a366f63289ff571b68b7ce0&include_adult=false{0}&page={1}{2}{3}", language, page, Tgenre, sortBy);
                    HttpClient client = new HttpClient();
                    String Jresult = await client.GetStringAsync(url);

                    ListFrame.Navigate(typeof(ListPage), 1);

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
                    }
                }
            }
            catch
            {
                await new Windows.UI.Popups.MessageDialog("Opps! Something wrong happened to the connection, please check your network and try again! ").ShowAsync();
            }

        }
        //汉堡界面的开合
        private void HamburgerButton_Click(object sender, RoutedEventArgs e)
        {
            MySplitView.IsPaneOpen = !MySplitView.IsPaneOpen;
        }
        //汉堡界面的选择切换
        private void ListBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (Home.IsSelected)
            {
                //主界面
                TitleTextBlock.Text = "Home";
                InitializeList();
                
            }
            else if (Collection.IsSelected)
            {
                //收藏界面
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
                    int flag = 0;
                    for (int i = 1; i <= 5; i++)
                    {
                        //电影请求
                        String url = String.Format("https://api.themoviedb.org/3/search/movie?api_key=7888f0042a366f63289ff571b68b7ce0&query={0}&page={1}", Search.Text,i);
                        HttpClient client = new HttpClient();
                        String Jresult = await client.GetStringAsync(url);
                        DataContractJsonSerializer serializer = new DataContractJsonSerializer(typeof(QueryMovieList));
                        MemoryStream ms = new MemoryStream(Encoding.UTF8.GetBytes(Jresult));
                        QueryMovieList queryMovieList = (QueryMovieList)serializer.ReadObject(ms);
                        //电视剧请求
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
                            if(flag == 0)
                            {
                                viewModel.clear();
                                this.Frame.Navigate(typeof(ListPage), 2);
                            }
                            flag++;
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
        //语言选择改变
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
        //视频类型选择改变（电影还是电视剧）
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
        //电影类别改变
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
        //上一页
        private void PreviousAppBarButton_Click(object sender, RoutedEventArgs e)
        {
            if(page > 1)
            {
                page--;
                InitializeList();
            }
        }
        //下一页
        private void NextAppBarButton_Click(object sender, RoutedEventArgs e)
        {
            if(page < 1000)
            {
                page++;
                InitializeList();
            }
        }
        //主界面的后退键，用于在主界面点击了某个视频跳转到详情等界面
        private void BackAppBarButton_Click(object sender, RoutedEventArgs e)
        {
            if (ListFrame.CanGoBack)
            {
                ListFrame.GoBack();
            }
        }
        //监控ListFrame，以便于决定某些空间的是否可见
        private void ListFrame_Navigated(object sender, Windows.UI.Xaml.Navigation.NavigationEventArgs e)
        {
            BackAppBarButton.Visibility = !ListFrame.CurrentSourcePageType.Equals(typeof(ListPage))&&!ListFrame.CurrentSourcePageType.Equals(typeof(CollectorItems)) ? Visibility.Visible : Visibility.Collapsed;
            pageChangePanel.Visibility = !ListFrame.CurrentSourcePageType.Equals(typeof(ListPage)) ? Visibility.Collapsed : Visibility.Visible;
            FilterSelectPanel.Visibility = !ListFrame.CurrentSourcePageType.Equals(typeof(ListPage))? Visibility.Collapsed : Visibility.Visible;
        }
        //电视剧类别改变
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
        //视频上映年份改变
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
        //排序类型改变
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
        //BGM暂停
        private void pause_Click(object sender, RoutedEventArgs e)
        {
            pause.Visibility = Visibility.Collapsed;
            start.Visibility = Visibility.Visible;
            music.Pause();
        }
        //BGM开始
        private void start_Click(object sender, RoutedEventArgs e)
        {
            start.Visibility = Visibility.Collapsed;
            pause.Visibility = Visibility.Visible;
            music.Play();
        }
    }
}
