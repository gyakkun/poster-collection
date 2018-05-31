using PosterCollection.Models;
using PosterCollection.Service;
using PosterCollection.ViewModels;
using System;
using Windows.ApplicationModel.DataTransfer;
using Windows.Storage.Streams;
using Windows.UI.Popups;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Media.Imaging;
using Windows.UI.Xaml.Navigation;

// https://go.microsoft.com/fwlink/?LinkId=234238 上介绍了“空白页”项模板

namespace PosterCollection
{
    /// <summary>
    /// 可用于自身或导航至 Frame 内部的空白页。
    /// </summary>
    public sealed partial class DetailPage : Page {
        private ViewModel viewModel;
        private string background;
        private MovieDetail Mdetail;
        private TVDetail Tdetail;
        private int flag;


        private Star mystar;
        public DetailPage() {
            this.InitializeComponent();
            viewModel = ViewModel.Instance;

        }

        protected override void OnNavigatedTo(NavigationEventArgs e) {
            viewModel = ViewModel.Instance;
            
            if (e.Parameter is int) {
                flag = (int)e.Parameter;
                
                if (flag == 0) {

                    int id = viewModel.TheMovieDetail.id;

                    for (int i = 0; i < viewModel.Starlist.Count; i++)
                    {
                        if (viewModel.Starlist[i].id == id)
                        {
                            collect.Visibility = Visibility.Collapsed;
                            collected.Visibility = Visibility.Visible;
                            break;
                        }
                    }
                    tvDetailGrid.Visibility = Visibility.Collapsed;
                    tvPosterImage.Visibility = Visibility.Collapsed;

                    Mdetail = viewModel.TheMovieDetail;

                    background = Mdetail.backdrop_path;

                    productionCompaniesTextBlock.Text = "";
                    foreach (var PCompany in Mdetail.production_companies) {
                        productionCompaniesTextBlock.Text += PCompany.name + "\n";
                    }
                    if (productionCompaniesTextBlock.Text == "") {
                        productionCompaniesTextBlock.Text = "Unknown";
                    }
                    productionCountriesTextBlock.Text = "";
                    foreach (var PCountries in Mdetail.production_countries) {
                        productionCountriesTextBlock.Text += PCountries.name + "\n";
                    }
                    if (productionCountriesTextBlock.Text == "") {
                        productionCountriesTextBlock.Text = "Unknown";
                    }
                    genresTextBlock.Text = "";
                    foreach (var genre in Mdetail.genres) {
                        genresTextBlock.Text += " | " + genre.name;
                    }
                    if (genresTextBlock.Text != "") {
                        genresTextBlock.Text = genresTextBlock.Text.Substring(3);
                    }
                    else {
                        genresTextBlock.Text = "Unkonwn";
                    }
                    spokenLanguageTextBlock.Text = "";
                    foreach (var language in Mdetail.spoken_languages) {
                        spokenLanguageTextBlock.Text += " | " + language.name;
                    }
                    if (spokenLanguageTextBlock.Text != "") {
                        spokenLanguageTextBlock.Text = spokenLanguageTextBlock.Text.Substring(3);
                    }
                    else {
                        spokenLanguageTextBlock.Text = Mdetail.original_language;
                    }
                    scoreTextBlock.Text = Mdetail.vote_average + " points / " + Mdetail.vote_count + " participants";
                    if (Mdetail.revenue != 0) {
                        revenueTextBlock.Text = Mdetail.revenue + " dollars";
                    }
                    else {
                        revenueTextBlock.Text = "Unknown";
                    }
                    if (Mdetail.budget != 0) {
                        budgetTextBlock.Text = Mdetail.budget + " dollars";
                    }
                    else {
                        budgetTextBlock.Text = "Unknown";
                    }
                    runtimeTextBlock.Text = Mdetail.runtime + " minutes";

                }
                else if (flag == 1) {

                    int id = viewModel.TheTVDetail.id;

                    for (int i = 0; i < viewModel.Starlist.Count; i++)
                    {
                        if (viewModel.Starlist[i].id == id)
                        {
                            collect.Visibility = Visibility.Collapsed;
                            collected.Visibility = Visibility.Visible;
                            break;
                        }
                    }

                    movieDetailGrid.Visibility = Visibility.Collapsed;
                    moviePosterImage.Visibility = Visibility.Collapsed;

                    Tdetail = viewModel.TheTVDetail;

                    background = Tdetail.backdrop_path;

                    originalCountriesText.Text = "";
                    foreach (var country in Tdetail.origin_country) {
                        originalCountriesText.Text += country + "\n";
                    }
                    if (originalCountriesText.Text == "") {
                        originalCountriesText.Text = "Unknown";
                    }
                    productionCompaniesText.Text = "";
                    foreach (var PCompany in Tdetail.production_companies) {
                        productionCompaniesText.Text += PCompany.name + "\n";
                    }
                    if (productionCompaniesText.Text == "") {
                        productionCompaniesText.Text = "Unknown";
                    }
                    genresTextBlock.Text = "";
                    foreach (var genre in Tdetail.genres) {
                        genresText.Text += " | " + genre.name;
                    }
                    if (genresText.Text != "") {
                        genresText.Text = genresText.Text.Substring(3);
                    }
                    else {
                        genresText.Text = "Unkonwn";
                    }
                    LanguageText.Text = "";
                    foreach (var language in Tdetail.languages) {
                        LanguageText.Text += " | " + language;
                    }
                    if (LanguageText.Text != "") {
                        LanguageText.Text = LanguageText.Text.Substring(3);
                    }
                    else {
                        LanguageText.Text = Tdetail.original_language;
                    }
                    scoreText.Text = Tdetail.vote_average + " points / " + Tdetail.vote_count + " participants";

                    createText.Text = "";
                    foreach (var creator in Tdetail.created_by) {
                        createText.Text += creator.name + "\n";
                    }
                    if (createText.Text == "") {
                        createText.Text = "Unknown";
                    }
                }
            }
            DataTransferManager.GetForCurrentView().DataRequested += OnShareDataRequested;
        }

        private void OnShareDataRequested(DataTransferManager sender, DataRequestedEventArgs args) {

            DataRequest request = args.Request;

            var deferral = args.Request.GetDeferral();

            try
            {
                if(flag == 0)
                {
                    request.Data.Properties.Title = Mdetail.title;
                    request.Data.Properties.Description = Mdetail.overview;
                    // 设置文本
                    request.Data.SetText(Mdetail.title + '\n' + Mdetail.overview + '\n');

                    //添加图片
                    
                    RandomAccessStreamReference imageRASR = RandomAccessStreamReference.CreateFromUri(((BitmapImage)moviePosterImage.Source).UriSource);
                    // 建议用Thumbnail分享文件
                    request.Data.Properties.Thumbnail = imageRASR;
                    request.Data.SetBitmap(imageRASR);

                }
                else if(flag == 1)
                {
                    request.Data.Properties.Title = Tdetail.name;
                    request.Data.Properties.Description = Tdetail.overview;
                    // 设置文本
                    request.Data.SetText(Tdetail.name + '\n' + Tdetail.overview + '\n');

                    //添加图片

                    RandomAccessStreamReference imageRASR = RandomAccessStreamReference.CreateFromUri(((BitmapImage)tvPosterImage.Source).UriSource);
                    // 建议用Thumbnail分享文件
                    request.Data.Properties.Thumbnail = imageRASR;
                    request.Data.SetBitmap(imageRASR);
                }
                

            }
            finally
            {
                deferral.Complete();
            }
        }


        private async void seasonsGridView_ItemClick(object sender, ItemClickEventArgs e) {
            var item = (Season)e.ClickedItem;
            string message = "Season Name:\t" + item.name + "\n\n" +
                             "Episode count:\t" + item.episode_count + "\n\n" +
                             "Air Date:\t" + item.air_date + "\n\n";

            if (item.overview != null && item.overview != "") {
                message += "Overview:\n\n" + item.overview + "\n";
            }
            await new Windows.UI.Popups.MessageDialog(message).ShowAsync();
        }

        private async void starGridView_ItemClick(object sender, ItemClickEventArgs e) {
            var item = (Cast)e.ClickedItem;
            string message = "Name:\t\t" + item.name + "\n\n";
            message += "Character:\t" + item.character + "\n";

            await new Windows.UI.Popups.MessageDialog(message).ShowAsync();
        }

        private async void collect_Click(object sender, RoutedEventArgs e) {

            if(flag == 0)
            {
                mystar = new Star(Mdetail.id, Mdetail.title, background,Mdetail.poster_path, "",0);

            }
            else
            {
                mystar = new Star(Tdetail.id, Tdetail.name, background,Tdetail.poster_path, "", 1);
                
            }
            viewModel.AddStar(mystar);
            TileService.GenerateTiles();

            var dialog = new MessageDialog("Mark");
            dialog.Commands.Add(new UICommand("Ok", cmd => { }, commandId: 0));
            var result = await dialog.ShowAsync();
            collect.Visibility = Visibility.Collapsed;
            collected.Visibility = Visibility.Visible;
        }

        private async void collected_Click(object sender, RoutedEventArgs e)
        {
            if (flag == 0)
            {
                mystar = new Star(Mdetail.id, Mdetail.title, background, Mdetail.poster_path, "", 0);

            }
            else
            {
                mystar = new Star(Tdetail.id, Tdetail.name, background, Tdetail.poster_path, "", 1);

            }

            viewModel.DeleteStar(mystar.id,mystar.type);
            var dialog = new MessageDialog("Unmark");
            dialog.Commands.Add(new UICommand("Ok", cmd => { }, commandId: 0));
            var result = await dialog.ShowAsync();
            collect.Visibility = Visibility.Visible;
            collected.Visibility = Visibility.Collapsed;
        }

        private void PostersBrowserAppBarButton_Click(object sender, RoutedEventArgs e) {
            if (flag == 0) {
                String url = String.Format("https://api.themoviedb.org/3/movie/{0}/images?api_key=7888f0042a366f63289ff571b68b7ce0", Mdetail.id);
                this.Frame.Navigate(typeof(PosterBrowserPage), url);
            }
            else {
                String url = String.Format("https://api.themoviedb.org/3/tv/{0}/images?api_key=7888f0042a366f63289ff571b68b7ce0", Tdetail.id);
                this.Frame.Navigate(typeof(PosterBrowserPage), url);
            }
        }

        private void shareWithFriends_Click(object sender, RoutedEventArgs e) {
            var s = sender as FrameworkElement;
            //var item = (Models.MovieDetail)s.DataContext;
            //App.title = item.title;
            //App.poster_path = item.poster_path;
            DataTransferManager.ShowShareUI();
        }
    }
}
