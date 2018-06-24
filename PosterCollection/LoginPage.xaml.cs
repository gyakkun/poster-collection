using System;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using System.Security.Cryptography;
using System.Text;
using PosterCollection.ViewModels;
using PosterCollection.Models;

// https://go.microsoft.com/fwlink/?LinkId=234238 上介绍了“空白页”项模板

namespace PosterCollection
{
    /// <summary>
    /// 可用于自身或导航至 Frame 内部的空白页。
    /// </summary>
    public sealed partial class LoginPage : Page
    {
        public LoginPage()
        {
            this.InitializeComponent();
        }

        private async void loginButton_Click(object sender, RoutedEventArgs e)
        {
            bool flag = false;
            for(int i = 0; i < ViewModel.Instance.UsersList.Count; i++)
            {
                if(usernameTextBox.Text == ViewModel.Instance.UsersList[i].Username)
                {
                    flag = true;
                    MD5 md5 = MD5.Create(); //实例化一个md5对像
                                            // 加密后是一个字节类型的数组，这里要注意编码UTF8/Unicode等的选择　
                    byte[] s = md5.ComputeHash(Encoding.UTF8.GetBytes(passwordBox.Password));

                    if(ViewModel.Instance.UsersList[i].Password == Convert.ToBase64String(s))
                    {
                        await new Windows.UI.Popups.MessageDialog("登录成功！").ShowAsync();
                        this.Frame.Navigate(typeof(MainPage), ViewModel.Instance.UsersList[i]);
                    }
                    else
                    {
                        await new Windows.UI.Popups.MessageDialog("密码错误，登录失败！").ShowAsync();
                    }
                }
            }
            if (!flag)
            {
                if(typeComboBox.SelectedIndex == 0)
                {
                    await new Windows.UI.Popups.MessageDialog("管理员账号不存在，登录失败！").ShowAsync();
                }
                else
                {
                    await new Windows.UI.Popups.MessageDialog("用户账号不存在，登录失败！").ShowAsync();
                }
            }
        }

        private void registerButton_Click(object sender, RoutedEventArgs e)
        {
            this.Frame.Navigate(typeof(RegisterPage));
        }
    }
}
