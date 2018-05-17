using PosterCollection.Models;
using PosterCollection.ViewModels;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Runtime.Serialization.Json;
using System.Text;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

// https://go.microsoft.com/fwlink/?LinkId=234238 上介绍了“空白页”项模板

namespace PosterCollection
{
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
            String url = String.Format("https://api.themoviedb.org/3/movie/{0}?api_key=7888f0042a366f63289ff571b68b7ce0", item.id);
            HttpClient client = new HttpClient();
            String Jresult = await client.GetStringAsync(url);
            DataContractJsonSerializer serializer = new DataContractJsonSerializer(typeof(MovieDetail));
            MemoryStream ms = new MemoryStream(Encoding.UTF8.GetBytes(Jresult));
            viewModel.TheMovieDetail = (MovieDetail)serializer.ReadObject(ms);
            this.Frame.Navigate(typeof(DetailPage), 0);
        }

        private void Delete(object sender, RoutedEventArgs e)
        {
            dynamic temp = e.OriginalSource;
            Star s= temp.DataContext;
            viewModel.DeleteStar(s.id);
        }

        private void edit(object sender, RoutedEventArgs e)
        {
            dynamic temp = e.OriginalSource;
            selectedItem = temp.DataContext;
            com.Text = selectedItem.comment;
            comment.Visibility = Visibility.Visible;

        }

        private void ok(object sender, RoutedEventArgs e)
        {
            int id = selectedItem.id;

           viewModel.EditComment(id, com.Text);
            var db = App.conn;
            using (var TodoItem = db.Prepare(App.SQL_UPDATE))
            {
                TodoItem.Bind(1, com.Text);
                TodoItem.Bind(2, id);
               
                TodoItem.Step();


            }

            comment.Visibility = Visibility.Collapsed;
            selectedItem = null;
        }
    }
}
