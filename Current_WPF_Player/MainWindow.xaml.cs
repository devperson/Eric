using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace AnniPanel
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        #region Public methods
        public MainWindow()
        {
            InitializeComponent();
        }
        #endregion

        #region Private methods
        private void frameMain_Loaded(object sender, RoutedEventArgs e)
        {
            frameMain.Navigate(_loginPage);
        }
        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            _session = new AnniSession();
            _loginPage = new LoginPage(_session);
            _selectionPage = new SelectionPage(_session);
            _session.FireLoginEvent += LoginEventHandler;
        }
        private void Window_Closed(object sender, EventArgs e)
        {
            if (_session != null)
            {
                _session.Dispose();
                _session = null;
            }
        }
        private void LoginEventHandler(object sender, AnniSession.LoginEventArgs e)
        {
            if (Dispatcher.Thread == System.Threading.Thread.CurrentThread)
            {
                if (e.Status == AnniSession.LoginStatus.Success)
                {
                    frameMain.Navigate(_selectionPage);
                }
            }
            else
            {
                Dispatcher.Invoke(new AnniSession.FireLoginEventHandler(LoginEventHandler), sender, e);
            }
        }
        #endregion

        #region Private fields
        private LoginPage _loginPage;
        private SelectionPage _selectionPage;
        private AnniSession _session;
        #endregion

    }
}
