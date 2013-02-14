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
    /// Interaction logic for SelectionPage.xaml
    /// </summary>
    public partial class SelectionPage : Page
    {
        #region Public methods
        public SelectionPage(AnniSession session)
        {
            _session = session;
            _session.FireLoginEvent += _session_FireLoginEvent;
            _session.FireQueryDirectoryEvent += _session_FireQueryDirectoryEvent;
            InitializeComponent();
            RegisterPages();
        }
        #endregion

        #region Private methods
        private void RegisterPages()
        {
            RegisterPage("application/anni-vmsdevice", new VideoDevicePage(), "vnc.png");
            RegisterPage("application/anni-video", new VideoPlaybackPage(false), "video.ico");
            RegisterPage("application/anni-group1", new DefaultInfoPage(), "building.png");
            RegisterPage("application/anni-video-live", new VideoPlaybackPage(true), "live.ico");
        }
        private void RegisterPage(string mime, Page page, string icon)
        {
            _infoPage[mime] = new FileType("", page, icon);
        }
        private void SelectFile(AnniSession.File file)
        {
            FileType fileType;
            if ((file == null) || (!_infoPage.TryGetValue(file.Mime, out fileType)))
            {
                nodeInfoFrame.Navigate(_defaultPage);
            }
            else
            {
                nodeInfoFrame.Navigate(fileType.Page);
                if (fileType.Page is INodeProperty)
                {
                    ((INodeProperty)fileType.Page).SelectNode(file);
                }
            }
        }
        private void statusBarMain_Loaded(object sender, RoutedEventArgs e)
        {
        }
        private void _session_FireLoginEvent(object sender, AnniSession.LoginEventArgs e)
        {
            if (Dispatcher.Thread == System.Threading.Thread.CurrentThread)
            {
                if (e.Status == AnniSession.LoginStatus.Success)
                {
                    labelCurrentUser.Content = e.UserName;
                    treeViewItemObjectsRoot.Items.Clear();
                    _session.BeginQueryDirectory(3, "*", treeViewItemObjectsRoot);
                }
                else
                {
                    labelCurrentUser.Content = " ";
                }
            }
            else
            {
                Dispatcher.Invoke(new AnniSession.FireLoginEventHandler(_session_FireLoginEvent), sender, e);
            }
        }
        private void treeViewItemObjectsRoot_Expanded(object sender, RoutedEventArgs e)
        {
            if (e.OriginalSource != null)
            {
                var header = ((TreeViewItem)e.OriginalSource).Header;
                if (header is AnniSession.File)
                {
                    _session.BeginQueryDirectory(((AnniSession.File)header).Id, "*",
                        e.OriginalSource);
                }
            }
        }
        private void treeViewItemObjectsRoot_Selected(object sender, RoutedEventArgs e)
        {
            if (Dispatcher.Thread == System.Threading.Thread.CurrentThread)
            {
                if (treeViewObjects.SelectedItem == null)
                {
                    SelectFile(null);
                }
                else
                {
                    var header = ((TreeViewItem)treeViewObjects.SelectedItem).Header;
                    if (header is AnniSession.File)
                    {
                        SelectFile((AnniSession.File)(((TreeViewItem)treeViewObjects.SelectedItem).Header));
                    }
                    else
                    {
                        SelectFile(null);
                    }
                }
            }
            else
            {
            }
        }
        private void _session_FireQueryDirectoryEvent(object sender, AnniSession.QueryDirectoryEventArgs e)
        {
            if (Dispatcher.Thread == System.Threading.Thread.CurrentThread)
            {
                _session.EndQueryDirectory(e.Result);
                var root = (TreeViewItem)e.Data;
                root.Items.Clear();
                foreach (var file in e.Files)
                {
                    var newItem = new TreeViewItem();
                    newItem.Header = file;
                    file.IconName = GetFileType(file.Mime).Icon;
                    root.Items.Add(newItem);
                    var lazyExpandItem = new TreeViewItem();
                    lazyExpandItem.Header = new LazyExpand();
                    newItem.Items.Add(lazyExpandItem);
                }
                if (e.Files.Length == 0)
                {
                    var lazyExpandItem = new TreeViewItem();
                    lazyExpandItem.Header = new LazyExpand();
                    root.Items.Add(lazyExpandItem);
                    root.IsExpanded = false;
                }
            }
            else
            {
                Dispatcher.Invoke(
                    new AnniSession.FireQueryDirectoryEventHandler(_session_FireQueryDirectoryEvent), sender, e);
            }
        }
        private FileType GetFileType(string mime)
        {
            FileType ft;
            if (!_infoPage.TryGetValue(mime, out ft))
            {
                ft = _defaultPage;
            }
            return ft;
        }
        #endregion

        #region Private classes
        private class LazyExpand
        {
            public override string ToString()
            {
                return "Retrieving objects...";
            }
        }
        private class FileType
        {
            public FileType(string description, Page page, string icon)
            {
                Description = description;
                Page = page;
                Icon = icon;
            }
            public string Description { get; private set; }
            public Page Page { get; private set; }
            public string Icon { get; private set; }
        }
        #endregion

        #region Private fields
        private readonly AnniSession _session;
        private readonly Dictionary<string, FileType> _infoPage = new Dictionary<string, FileType>();
        private readonly FileType _defaultPage = new FileType("", new DefaultInfoPage(), "entire_network.ico");
        #endregion

    }
}
