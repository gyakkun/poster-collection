using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using PosterCollection.Models;
using PosterCollection.ViewModels;
using System.Collections.ObjectModel;
using System;

// https://go.microsoft.com/fwlink/?LinkId=234238 上介绍了“空白页”项模板

namespace PosterCollection
{
    /// <summary>
    /// 可用于自身或导航至 Frame 内部的空白页。
    /// </summary>
    public sealed partial class UserManagePage : Page
    {
        private ObservableCollection<UsersInfo> users = new ObservableCollection<UsersInfo>();

        public UserManagePage()
        {
            this.InitializeComponent();
            initialUsersList();
        }

        public void initialUsersList()
        {
            for (int i = 0; i < ViewModel.Instance.UsersList.Count; i++)
            {
                if (ViewModel.Instance.UsersList[i].Role == 0)
                {
                    users.Add(ViewModel.Instance.UsersList[i]);
                }
                
            }
            for (int i = 0; i < ViewModel.Instance.UsersList.Count; i++)
            {
                if (ViewModel.Instance.UsersList[i].Role == 1)
                {
                    users.Add(ViewModel.Instance.UsersList[i]);
                }

            }
        }

        private void SearchButton_Click(object sender, RoutedEventArgs e)
        {
            users.Clear();
            for(int i = 0; i < ViewModel.Instance.UsersList.Count; i++)
            {
                if(ViewModel.Instance.UsersList[i].Username == Search.Text)
                {
                    users.Add(ViewModel.Instance.UsersList[i]);
                }
            }
        }

        private void ListView_ItemClick(object sender, ItemClickEventArgs e)
        {
            UsersInfo user = (UsersInfo)e.ClickedItem;
            usernameTextBlock.Text = user.Username;
            roleTextBlock.Text = user.Role == 0 ? "管理员" : "用户";
            emailTextBlock.Text = user.Email;
            phoneTextBlock.Text = user.Phone;
        }

        private async void AppBarButton_Click(object sender, RoutedEventArgs e)
        {
            dynamic temp = e.OriginalSource;
           
            UsersInfo user = (UsersInfo)temp.DataContext;
            if (user.Role == 0)
            {
                await new Windows.UI.Popups.MessageDialog("对不起，你没有权限删除该用户。").ShowAsync();
            }
            else
            {

                users.Remove(user);

                ViewModel.Instance.deleteUser(user.Id);
                await new Windows.UI.Popups.MessageDialog("用户已被删除").ShowAsync();
            }
        }

        private void AddButton_Click(object sender, RoutedEventArgs e)
        {

        }

        private void cancelButton_Click(object sender, RoutedEventArgs e)
        {

        }

        private void confirmButton_Click(object sender, RoutedEventArgs e)
        {

        }
    }
}
