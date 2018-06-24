using System;
using System.Security.Cryptography;
using System.Text;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using PosterCollection.Models;
using PosterCollection.ViewModels;

// https://go.microsoft.com/fwlink/?LinkId=234238 上介绍了“空白页”项模板

namespace PosterCollection
{
    /// <summary>
    /// 可用于自身或导航至 Frame 内部的空白页。
    /// </summary>
    public sealed partial class RegisterPage : Page
    {
        private string key = "4QrcOUm6Wau+VuBX8g+IPg==";

        public RegisterPage()
        {
            this.InitializeComponent();
        }

        private void ComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (roleCombox.SelectedIndex == 1)
            {
                keyPanel.Visibility = Visibility.Collapsed;
            }
            else
            {
                keyPanel.Visibility = Visibility.Visible;
            }
        }

        private async void confirmButton_Click(object sender, RoutedEventArgs e)
        {
            bool flag = true;
            if (roleCombox.SelectedIndex == -1 || usernameTextBox.Text == "" || confirmTextBox.Password == "" || passwordTextBox.Password == "" || (roleCombox.SelectedIndex == 0 && keyTextBox.Password == ""))
            {
                flag = false;
                await new Windows.UI.Popups.MessageDialog("请完善必填信息！").ShowAsync();
            }
            for (int i = 0; i < ViewModel.Instance.UsersList.Count; i++)
            {
                if (ViewModel.Instance.UsersList[i].Username == usernameTextBox.Text)
                {
                    flag = false;
                    await new Windows.UI.Popups.MessageDialog("用户已存在，注册失败！").ShowAsync();
                    break;
                }
            }

            if (confirmTextBox.Password != "" && passwordTextBox.Password != "" && confirmTextBox.Password != passwordTextBox.Password)
            {
                flag = false;
                await new Windows.UI.Popups.MessageDialog("两次密码输入不一致，注册失败！").ShowAsync();
            }
            if (keyTextBox.Password != "" && roleCombox.SelectedIndex == 0)
            {
                MD5 md5 = MD5.Create(); //实例化一个md5对像
                                        // 加密后是一个字节类型的数组，这里要注意编码UTF8/Unicode等的选择　
                byte[] s = md5.ComputeHash(Encoding.UTF8.GetBytes(keyTextBox.Password));

                if (key != Convert.ToBase64String(s))
                {
                    flag = false;
                    await new Windows.UI.Popups.MessageDialog("管理员密钥错误，注册失败！").ShowAsync();
                }
            }
            if (flag)
            {
                MD5 md5 = MD5.Create(); //实例化一个md5对像
                                        // 加密后是一个字节类型的数组，这里要注意编码UTF8/Unicode等的选择　
                byte[] s = md5.ComputeHash(Encoding.UTF8.GetBytes(passwordTextBox.Password));

                ViewModel.Instance.createUser(new UsersInfo(-1, usernameTextBox.Text, Convert.ToBase64String(s), emailTextBox.Text, phoneTextBox.Text, roleCombox.SelectedIndex));
                await new Windows.UI.Popups.MessageDialog("注册成功！").ShowAsync();
                this.Frame.GoBack();
            }

        }

        private void cancelButton_Click(object sender, RoutedEventArgs e)
        {
            roleCombox.SelectedIndex = -1;
            keyPanel.Visibility = Visibility.Collapsed;
            keyTextBox.Password = "";
            usernameTextBox.Text = "";
            passwordTextBox.Password = "";
            confirmTextBox.Password = "";
            emailTextBox.Text = "";
            phoneTextBox.Text = "";
        }
    }
}
