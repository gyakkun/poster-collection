using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using PosterCollection.Models;
using PosterCollection.ViewModels;
using System.Collections.ObjectModel;
using System;
using System.Security.Cryptography;
using System.Text;
using Windows.UI.Xaml.Navigation;

// https://go.microsoft.com/fwlink/?LinkId=234238 上介绍了“空白页”项模板

namespace PosterCollection
{
    /// <summary>
    /// 可用于自身或导航至 Frame 内部的空白页。
    /// </summary>
    public sealed partial class UserManagePage : Page
    {
        private ObservableCollection<UsersInfo> users = new ObservableCollection<UsersInfo>();
        private UsersInfo user;
        private bool canChange = false;
        private UsersInfo temp;
        private int index;

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            if(e.Parameter is UsersInfo)
            {
                user = ((UsersInfo)e.Parameter);
            }
            if(user.Role == 1)
            {
                users.Clear();
                for (int i = 0; i < ViewModel.Instance.UsersList.Count; i++)
                {
                    if (ViewModel.Instance.UsersList[i].Username == user.Username)
                    {
                        users.Add(ViewModel.Instance.UsersList[i]);

                    }

                }
                AddPanel.Visibility = Visibility.Collapsed;
                SearchPanel.Visibility = Visibility.Collapsed;
                confirmButton.Content = "修改";
                usernameTextBlock.Text = user.Username;
                passwordTextBlock.Password = user.Password;
                emailTextBlock.Text = user.Email;
                phoneTextBlock.Text = user.Phone;
                roleTextBlock.Text = temp.Role == 0 ? "管理员" : "用户";
            }
        }


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
            for(int i = 0; i < users.Count; i++)
            {
                if(users[i].Username == Search.Text)
                {
                    users.RemoveAt(i);
                    users.Insert(1,users[i]);
                }
            }
        }

        private void ListView_ItemClick(object sender, ItemClickEventArgs e)
        {
            temp = (UsersInfo)e.ClickedItem;
            if(temp.Username == user.Username||temp.Role == 1)
            {
                canChange = true;
            }
            else
            {
                canChange = false;
            }
            index = users.IndexOf(temp);
            usernameTextBlock.Text = temp.Username;
            roleTextBlock.Text = temp.Role == 0 ? "管理员" : "用户";
            passwordTextBlock.Password = temp.Password;
            emailTextBlock.Text = temp.Email;
            phoneTextBlock.Text = temp.Phone;
            confirmButton.Content = "修改";
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
                await new Windows.UI.Popups.MessageDialog("用户已被删除。").ShowAsync();
            }
        }

        private async void AddButton_Click(object sender, RoutedEventArgs e)
        {
            usernameTextBlock.Text = "";
            roleTextBlock.Text = "用户";
            passwordTextBlock.Password = "";
            emailTextBlock.Text = "";
            phoneTextBlock.Text = "";
            confirmButton.Content = "创建";
            await new Windows.UI.Popups.MessageDialog("请填写添加的用户信息，邮箱，电话可选，用户名，密码必填。").ShowAsync();
        }

        private void cancelButton_Click(object sender, RoutedEventArgs e)
        {
            passwordTextBlock.Password = "";
            usernameTextBlock.Text = "";
            roleTextBlock.Text = "";
            emailTextBlock.Text = "";
            phoneTextBlock.Text = "";
            confirmButton.Content = "确定";
        }

        private async void confirmButton_Click(object sender, RoutedEventArgs e)
        {
            bool flag = true;
            if((string)confirmButton.Content == "创建")
            {
                if (usernameTextBlock.Text != "" && passwordTextBlock.Password != "")
                {
                    for (int i = 0; i < ViewModel.Instance.UsersList.Count; i++)
                    {
                        if (ViewModel.Instance.UsersList[i].Username == usernameTextBlock.Text)
                        {
                        flag = false;
                        await new Windows.UI.Popups.MessageDialog("用户名已存在，创建失败。").ShowAsync();
                        }
                    }
                    if (flag)
                    {
                    
                        MD5 md5 = MD5.Create(); //实例化一个md5对像
                                                // 加密后是一个字节类型的数组，这里要注意编码UTF8/Unicode等的选择　
                        byte[] s = md5.ComputeHash(Encoding.UTF8.GetBytes(passwordTextBlock.Password));

                        ViewModel.Instance.createUser(new UsersInfo(-1, usernameTextBlock.Text, Convert.ToBase64String(s), emailTextBlock.Text, phoneTextBlock.Text, 1));
                        users.Add(new UsersInfo(-1, usernameTextBlock.Text, Convert.ToBase64String(s), emailTextBlock.Text, phoneTextBlock.Text, 1));
                        await new Windows.UI.Popups.MessageDialog("用户创建成功。").ShowAsync();
                    
                    }
                }
                else
                {
                    await new Windows.UI.Popups.MessageDialog("请填写用户名，密码信息。").ShowAsync();
                }
            }
            else
            {
                if (usernameTextBlock.Text != "" && passwordTextBlock.Password != "")
                {
                    for (int i = 0; i < ViewModel.Instance.UsersList.Count; i++)
                    {
                        if (ViewModel.Instance.UsersList[i].Username == usernameTextBlock.Text && usernameTextBlock.Text != temp.Username)
                        {
                            
                            await new Windows.UI.Popups.MessageDialog("用户名已存在，修改失败。").ShowAsync();
                            return;
                        }
                    }
                    if (canChange)
                    {
                        if(passwordTextBlock.Password != temp.Password)
                        {
                            MD5 md5 = MD5.Create(); //实例化一个md5对像
                                                    // 加密后是一个字节类型的数组，这里要注意编码UTF8/Unicode等的选择　
                            byte[] s = md5.ComputeHash(Encoding.UTF8.GetBytes(passwordTextBlock.Password));


                            users.RemoveAt(index);
                            users.Insert(index, new UsersInfo(temp.Id, usernameTextBlock.Text, Convert.ToBase64String(s), emailTextBlock.Text, phoneTextBlock.Text, temp.Role));
                            ViewModel.Instance.updateUser(new UsersInfo(temp.Id, usernameTextBlock.Text, Convert.ToBase64String(s), emailTextBlock.Text, phoneTextBlock.Text, temp.Role));
                        }
                        else
                        {
                            users.RemoveAt(index);
                            users.Insert(index, new UsersInfo(temp.Id, usernameTextBlock.Text, passwordTextBlock.Password, emailTextBlock.Text, phoneTextBlock.Text, temp.Role));
                            ViewModel.Instance.updateUser(new UsersInfo(temp.Id, usernameTextBlock.Text, passwordTextBlock.Password, emailTextBlock.Text, phoneTextBlock.Text, temp.Role));
                        }
                        await new Windows.UI.Popups.MessageDialog("用户信息修改成功。").ShowAsync();
                    }
                    else
                    {
                        await new Windows.UI.Popups.MessageDialog("你没有权限修改该用户信息。").ShowAsync();
                    }
                }
                else
                {
                    await new Windows.UI.Popups.MessageDialog("请填写用户名，密码信息。").ShowAsync();
                }
            }
        }
    }
}
