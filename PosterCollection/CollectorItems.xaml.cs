﻿using PosterCollection.Models;
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
    }
}