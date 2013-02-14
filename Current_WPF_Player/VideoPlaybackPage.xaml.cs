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
    /// Interaction logic for VideoPlaybackPage.xaml
    /// </summary>
    public partial class VideoPlaybackPage : Page, INodeProperty
    {
        #region Public methods
        public VideoPlaybackPage(bool isLive)
        {
            _isLive = isLive;
            InitializeComponent();
        }
        public void SelectNode(AnniSession.File file)
        {
            AnniVideo.Source = null;
            VideoNameTextBox.Text = file.FileName;
            _fileId = file.Id;
        }
        #endregion

        #region Private methods
        private void Button_Click_1(object sender, RoutedEventArgs e)
        {
            var addr = Environment.GetCommandLineArgs();
            if (_isLive)
            {
                AnniVideo.Source = new Uri("mms://" + addr[1] + ":8080");
            }
            else
            {
                AnniVideo.Source = new Uri("http://" + addr[1] + "/annisafe/index.php?file=" + _fileId);
            }
            AnniVideo.Play();
        }
        private void AnniVideo_MediaFailed(object sender, ExceptionRoutedEventArgs e)
        {
            AnniVideo.Source = null;
        }
        #endregion

        #region Private fields
        private readonly bool _isLive;
        private long _fileId;
        #endregion

    }
}
