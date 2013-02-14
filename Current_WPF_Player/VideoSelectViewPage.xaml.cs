using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Net;
using System.Text;
using System.Timers;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using Microsoft.Win32;

namespace AnniSafeVideo
{
    /// <summary>
    /// Interaction logic for VideoSelectViewPage.xaml
    /// </summary>
    public partial class VideoSelectViewPage : Page
    {
        // Members goes here
        Timer _internalTimer;
        const int sliderTimerValue = 1000;

        public VideoSelectViewPage()
        {
            InitializeComponent();

            // Initialize and fill UI members
            InitUI();
            // Members initialization
            _internalTimer = new Timer(sliderTimerValue);
            _internalTimer.Elapsed += new ElapsedEventHandler(onTimerElapsed);
            _internalTimer.AutoReset = false;
            _internalTimer.Enabled = false;
        }

        /// <summary>
        /// Fills UI controls on this page
        /// </summary>
        private void InitUI() 
        {
            int numberOfDays = 5;
            for (int i = 0; i < numberOfDays; i++)
            {
                // Get current date
                DateTime tmpDate = DateTime.Now.Date;
                tmpDate = tmpDate.AddDays(-i);
                // Create combo box items and fill combo box
                ComboBoxItem tmpDateComboItem = new ComboBoxItem();
                tmpDateComboItem.Content = tmpDate.ToString();
                comboScreenShotDate.Items.Add(tmpDateComboItem);
            }
        }
        
        public void DisplayObjectView(MainWindow mainWindow, AnniObject obj)
        {
            _mainWindow = mainWindow;
            _obj = obj;
            labelCameraName.Content = _obj.FileInfo.Name;
        }

        /// <summary>
        /// Starts timer 
        /// </summary>
        private void StartTimer()
        {
            if (_internalTimer == null)
                return;

            _internalTimer.Enabled = true;
            _internalTimer.Start();
        }

        #region Download
        private void downloadVideo()
        {
            String strURL = "http://www.realmadrid.ru/images/gallery/originals/13012013-os-rm-0-0-all.jpg";
            String fileName = @"c:\tmpTest.jpg";

            WebClient webClient = new WebClient();
            webClient.DownloadFileCompleted += new AsyncCompletedEventHandler(downloadCompleted);
            webClient.DownloadProgressChanged += new DownloadProgressChangedEventHandler(progressChanged);
            webClient.DownloadFileAsync(new Uri(strURL), fileName);    
        }

        private void progressChanged(object sender, DownloadProgressChangedEventArgs e)
        {
            // Update progress
            downloadProgressBar.Value = e.ProgressPercentage;
        }

        /// <summary>
        /// This function is called when all progress bars are at 100%. At this point 
        /// we need to update UI controls to show Save and Play buttons
        /// </summary>
        private void downloadCompleted(object sender, AsyncCompletedEventArgs e)
        {
            playButton.Visibility = System.Windows.Visibility.Visible;
            saveButton.Visibility = System.Windows.Visibility.Visible;
        }
        #endregion

        private MainWindow _mainWindow;
        private AnniObject _obj;

        #region events
        private void onDateChanged(object sender, SelectionChangedEventArgs e)
        {
            // Change visibility for screen shot controls
            gridScreenShots.Visibility = System.Windows.Visibility.Visible;

            // TODO: Update UI with new selected content
        }

        private void onDownloadButtonClicked(object sender, RoutedEventArgs e)
        {
            // Show progress bars
            gridDownloadBar.Visibility = System.Windows.Visibility.Visible;
            // Download vidom from server
            downloadVideo();

            // TODO: Start download process
            // JIRA: https://annigan.com:8443/browse/ANNISAFEII-6?focusedCommentId=10304#comment-10304
        }

        private void onPlayButtonClicked(object sender, RoutedEventArgs e)
        {
            // TODO:
            // JIRA: https://annigan.com:8443/browse/ANNISAFEII-7
        }

        private void onSaveButtonClicked(object sender, RoutedEventArgs e)
        {
            SaveFileDialog saveDialog = new SaveFileDialog();
            saveDialog.Filter = "Show All Files (*.*)|*.*";
            saveDialog.FileName = "Untitled";
            if (saveDialog.ShowDialog() == true)
            {
                
            }

            // TODO:
            // JIRA: https://annigan.com:8443/browse/ANNISAFEII-8
        }
        
        private void onStartSliderValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            // Update time for slider
            double value = e.NewValue;

            // Get position in seconds
            double seconds = value * 863.99; // 864 = 86400 / 100

            DateTime time = new DateTime(1974, 7, 10, 0, 0, 0);
            time = time.AddSeconds(seconds);
            String strTime = time.TimeOfDay.ToString();
            string[] list = strTime.Split('.');
            startTimeLabel.Content = list[0];

            // Start timer
            StartTimer();
        }

        private void onEndSliderValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            // Update time for slider
            double value = e.NewValue;

            // Get position in seconds
            double seconds = value * 863.99; // 864 = 86400 / 100

            DateTime time = new DateTime(1974, 7, 10, 0, 0, 0);
            time = time.AddSeconds(seconds);
            String strTime = time.TimeOfDay.ToString();
            string[] list = strTime.Split('.');
            endTimeLabel.Content = list[0];

            // Start timer
            StartTimer();
        }
        
        private void onTimerElapsed(object sender, ElapsedEventArgs e)
        {
            // TODO: 
        }
        #endregion
    }
}
