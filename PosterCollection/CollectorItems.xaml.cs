using PosterCollection.Models;
using PosterCollection.ViewModels;
using System;
using System.IO;
using System.Net.Http;
using System.Runtime.Serialization.Json;
using System.Text;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

// https://go.microsoft.com/fwlink/?LinkId=234238 上介绍了“空白页”项模板

namespace PosterCollection {
    /// <summary>
    /// 可用于自身或导航至 Frame 内部的空白页。
    /// </summary>
    public sealed partial class CollectorItems : Page
    {
        private ViewModel viewModel;
        private Star selectedItem;
        public CollectorItems()
        {
            this.InitializeComponent();
            viewModel = ViewModel.Instance;
        }

        private async void Starlist_ItemClick(object sender, ItemClickEventArgs e)
        {
            var item = (Star)e.ClickedItem;
            //点击的是电影
            if(item.type == 0)
            {
                String url = String.Format("https://api.themoviedb.org/3/movie/{0}?api_key=7888f0042a366f63289ff571b68b7ce0&append_to_response=casts", item.id);
                HttpClient client = new HttpClient();
                String Jresult = await client.GetStringAsync(url);
                DataContractJsonSerializer serializer = new DataContractJsonSerializer(typeof(MovieDetail));
                MemoryStream ms = new MemoryStream(Encoding.UTF8.GetBytes(Jresult));
                viewModel.TheMovieDetail = (MovieDetail)serializer.ReadObject(ms);
                if (viewModel.TheMovieDetail.backdrop_path != null)
                {
                    viewModel.TheMovieDetail.backdrop_path = "https://image.tmdb.org/t/p/original" + viewModel.TheMovieDetail.backdrop_path;
                }
                else
                {
                    viewModel.TheMovieDetail.backdrop_path = "Assets/defaultBackground.jpg";
                }
                if (viewModel.TheMovieDetail.poster_path != null)
                {
                    viewModel.TheMovieDetail.poster_path = "https://image.tmdb.org/t/p/w500" + viewModel.TheMovieDetail.poster_path;
                }
                else
                {
                    viewModel.TheMovieDetail.poster_path = "Assets/defaultPoster.jpg";
                }

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
                this.Frame.Navigate(typeof(DetailPage), 0);
            }
            //点击的是电视剧
            else
            {
                String url = String.Format("https://api.themoviedb.org/3/tv/{0}?api_key=7888f0042a366f63289ff571b68b7ce0&append_to_response=casts", item.id);
                HttpClient client = new HttpClient();
                String Jresult = await client.GetStringAsync(url);
                DataContractJsonSerializer serializer = new DataContractJsonSerializer(typeof(TVDetail));
                MemoryStream ms = new MemoryStream(Encoding.UTF8.GetBytes(Jresult));
                viewModel.TheTVDetail = (TVDetail)serializer.ReadObject(ms);

                if (viewModel.TheTVDetail.backdrop_path != null)
                {
                    viewModel.TheTVDetail.backdrop_path = "https://image.tmdb.org/t/p/original" + viewModel.TheTVDetail.backdrop_path;
                }
                else
                {
                    viewModel.TheTVDetail.backdrop_path = "Assets/defaultBackground.jpg";
                }
                if (viewModel.TheTVDetail.poster_path != null)
                {
                    viewModel.TheTVDetail.poster_path = "https://image.tmdb.org/t/p/w500" + viewModel.TheTVDetail.poster_path;
                }
                else
                {
                    viewModel.TheTVDetail.poster_path = "Assets/defaultPoster.jpg";
                }

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
                this.Frame.Navigate(typeof(DetailPage), 1);
            }  
        }
        //点击删除按键
        private void Delete(object sender, RoutedEventArgs e)
        {
            dynamic temp = e.OriginalSource;
            Star s= temp.DataContext;
            viewModel.DeleteStar(s.id,s.type);
        }
        //点击修改评论按键
        private async void edit(object sender, RoutedEventArgs e)
        {
            dynamic temp = e.OriginalSource;
            selectedItem = temp.DataContext;
            Comment.Text = selectedItem.comment;
           await CommentDialog.ShowAsync();

        }
        //确定修改评论
        private void ok(ContentDialog sender, ContentDialogButtonClickEventArgs args)
        {
            int id = selectedItem.id;

            viewModel.EditComment(id, Comment.Text,selectedItem.type);
            
            comment.Visibility = Visibility.Collapsed;
            selectedItem = null;
            Frame.Navigate(typeof(CollectorItems));
        }

      
    }
}
