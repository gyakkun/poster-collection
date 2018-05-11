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
    public sealed partial class DetailPage : Page
    {
        private ViewModel viewModel;
        private string background;
        private MovieDetail Mdetail;
        private TVDetail Tdetail;
        public DetailPage()
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
                if (flag == 0)
                {
                    Mdetail = viewModel.TheMovieDetail;
                    if(Mdetail.backdrop_path != null)
                    {
                        background = "https://image.tmdb.org/t/p/w1280" + Mdetail.backdrop_path;
                    }
                    else
                    {
                        background = "Assets/defaultBackground.png";
                    }
                    
                }
                else
                {
                    Tdetail = viewModel.TheTVDetail;
                    if (Tdetail.backdrop_path != null)
                    {
                        background = "https://image.tmdb.org/t/p/w1280" + Tdetail.backdrop_path;
                    }
                    else
                    {
                        background = "Assets/defaultBackground.png";
                    }
                }
            }
            
        }
    }
}
