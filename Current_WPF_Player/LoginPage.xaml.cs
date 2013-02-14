using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Threading;

namespace AnniPanel
{
    /// <summary>
    /// Interaction logic for Page1.xaml
    /// </summary>
    public partial class LoginPage : Page
    {
        #region Public methods
        public LoginPage(AnniSession session)
        {
            InitializeComponent();
            textBoxUserName.Focus();
            _session = session;
        }

        public void Clear()
        {
        }

        #endregion

        #region Private methods
        private void buttonLogin_Click(object sender, RoutedEventArgs e)
        {
            _session.BeginLogin(textBoxUserName.Text, passwordBoxPassword.Password);
        }

        private void textBoxUserName_TextChanged(object sender, TextChangedEventArgs e)
        {
            TestForEnableButton();
        }

        private void passwordBoxPassword_PasswordChanged(object sender, RoutedEventArgs e)
        {
            TestForEnableButton();
        }

        private void TestForEnableButton()
        {
            if ((textBoxUserName.Text.Length > 0) && (passwordBoxPassword.Password.Length > 0))
            {
                buttonLogin.IsEnabled = true;
            }
            else
            {
                buttonLogin.IsEnabled = false;
            }
        }

        private void LoginComplete(IAsyncResult result)
        {
        }

        private void buttonOk_Click(object sender, RoutedEventArgs e)
        {
            groupBoxProcessing.Visibility = System.Windows.Visibility.Collapsed;
            groupBoxLogin.Visibility = System.Windows.Visibility.Visible;
        }

        private void comboBoxDomain_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {

        }

        #endregion

        #region Private fields
        private readonly AnniSession _session;
        #endregion
    }
}
