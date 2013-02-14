using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading;
using AnniCOMTypeLib;

using System.Net.Http;

namespace AnniPanel
{
    public class AnniSession : IDisposable
    {
        #region Public methods
        public AnniSession()
        {
            _thread.Start(this);
        }
        public void Dispose()
        {
            QueueRequest(new Request(ProcessDispose, false, null));
            _thread.Join();
        }
        public IAsyncResult BeginLogin(string userName, string password)
        {
            var request = new Request(ProcessLogin, false, new LoginData(userName, password));
            QueueRequest(request);
            return request;
        }
        public LoginStatus EndLogin(IAsyncResult result)
        {
            ProcessEnd(result);
            return ((LoginData)(((Request)result).AsyncState)).Status;
        }
        public LoginStatus Login(string userName, string password)
        {
            return EndLogin(BeginLogin(userName, password));
        }
        public IAsyncResult BeginQueryDirectory(long rootId, string query, object data)
        {
            var request = new Request(ProcessQueryDirectory, false, new QueryDirectoryData(rootId, query, data));
            QueueRequest(request);
            return request;
        }
        public File[] EndQueryDirectory(IAsyncResult result)
        {
            ProcessEnd(result);
            return ((QueryDirectoryData)(((Request)result).AsyncState)).Files;
        }
        public File[] QueryDirectory(long rootId, string query, object data)
        {
            return EndQueryDirectory(BeginQueryDirectory(rootId, query, data));
        }
        #endregion

        #region Public enums
        public enum LoginStatus
        {
            Success,
            AccessDenied,
            NetworkFailure,
            LicenseFailure,
            Exception
        }
        #endregion

        #region Public delegates
        public delegate void FireLoginEventHandler(object sender, LoginEventArgs e);
        public delegate void FireLogoutEventHandler(object sender, EventArgs e);
        public delegate void FireQueryDirectoryEventHandler(object sender, QueryDirectoryEventArgs e);
        #endregion

        #region Public events
        public event FireLoginEventHandler FireLoginEvent;
        public event FireLogoutEventHandler FireLogoutEvent;
        public event FireQueryDirectoryEventHandler FireQueryDirectoryEvent;
        #endregion

        #region Protected methods
        protected virtual void OnLogin(LoginStatus status, string userName, IAsyncResult result)
        {
            if (FireLoginEvent != null)
            {
                FireLoginEvent.Invoke(this, new LoginEventArgs(status, userName, result));
            }
        }
        protected virtual void OnLogout(IAsyncResult result)
        {
            if (FireLogoutEvent != null)
            {
                FireLogoutEvent.Invoke(this, new EventArgs());
            }
        }
        protected virtual void OnQueryDirectory(File dir, File[] files, object data, IAsyncResult result)
        {
            if (FireQueryDirectoryEvent != null)
            {
                FireQueryDirectoryEvent.Invoke(this, new QueryDirectoryEventArgs(dir, files, data, result));
            }
        }
        #endregion

        #region Private methods
        private static void ConnectionThread(object self)
        {
            ((AnniSession)self).InternalConnectionThread();
        }
        private void InternalConnectionThread()
        {
            WaitHandle[] wait = { _terminateEvent, _queueEvent };
            bool done = false;
            Request request;
            while(!done)
            {
                switch (WaitHandle.WaitAny(wait))
                {
                    case 0:
                        done = true;
                        break;
                    case 1:
                        lock (_requestQueue)
                        {
                            request = _requestQueue.Dequeue();
                            if (_requestQueue.Count < 1)
                            {
                                _queueEvent.Reset();
                            }
                        }
                        if (request != null)
                        {
                            request.Process();
                            request = null;
                        }
                        break;
                    default:
                        throw new Exception("Internal exception");
                }
            }
        }
        private void QueueRequest(Request request)
        {
            lock (_requestQueue)
            {
                _requestQueue.Enqueue(request);
                _queueEvent.Set();
            }
        }
        private static void ProcessEnd(IAsyncResult result)
        {
            Request req = (Request)result;
            req.AsyncWaitHandle.WaitOne();
            if (req.ProcessException != null)
            {
                throw req.ProcessException;
            }
        }
        private void ProcessDispose(Request request)
        {
            if (_anniSafe != null)
            {
                _anniSafe = null;
            }
            _terminateEvent.Set();
        }
        private void ProcessLogin(Request request)
        {
            var data = (LoginData)request.AsyncState;
            if (_anniSafe != null)
            {
                OnLogout(null);
                _anniSafe = null;
            }
            try
            {
                _anniSafe = new AnniSafe();
                data.Status = _statusMap[_anniSafe.Login(data.UserName, data.Password)];
            }
            catch (Exception e)
            {
                request.ProcessException = e;
                request.Complete();
                return;
            }
            request.Complete();
            OnLogin(data.Status, data.UserName, request);
        }
        private void ProcessQueryDirectory(Request request)
        {
            // Format: root=5,user=2/mydir/*
            var queryData = (QueryDirectoryData)(request.AsyncState);
            queryData.Files = MockGetFiles(queryData.RootId);
            request.Complete();
            OnQueryDirectory(new File(queryData.RootId, "AnniSafe", "application/anni-vmsdevice"),
                queryData.Files, queryData.Data, request);
        }
        private File[] MockGetFiles(long root)
        {
            List<File> files = new List<File>();
            var client = new HttpClient();
            var addr = Environment.GetCommandLineArgs();
            try
            {
                var ret = client.GetStringAsync("http://" + addr[1] + "/annisafe/dir.php?parent=" + root).Result;
                var fileLines = ret.Split('\n');
                foreach (string fileLine in fileLines)
                {
                    var rows = fileLine.Split('\t');
                    files.Add(new File(Convert.ToInt64(rows[0]), rows[2], rows[1]));
                }
            }
            catch
            {
                return files.ToArray();
            }
            return files.ToArray();
        }
        #endregion

        #region Public classes
        public class SessionException : Exception
        {
            public SessionException()
            {
            }
        }
        public class LoginEventArgs : EventArgs
        {
            internal LoginEventArgs(LoginStatus status, string userName, IAsyncResult result)
            {
                Status = status;
                UserName = userName;
                Result = result;
            }
            public LoginStatus Status { get; private set; }
            public string UserName { get; private set; }
            public IAsyncResult Result { get; private set; }
        }
        public class QueryDirectoryEventArgs : EventArgs
        {
            internal QueryDirectoryEventArgs(File dir, File[] files, object data, IAsyncResult result)
            {
                Directory = dir;
                Files = files;
                Data = data;
                Result = result;
            }
            public File Directory { get; private set; }
            public File[] Files { get; private set; }
            public object Data { get; private set; }
            public IAsyncResult Result { get; private set; }
        }
        public class File
        {
            internal File(long id, string fileName, string mime)
            {
                Id = id;
                FileName = fileName;
                Mime = mime;
                IconName = "entire_network.ico";
            }
            public long Id { get; private set; }
            public string FileName { get; private set; }
            public string Mime { get; private set; }
            public override string ToString()
            {
                return FileName;
            }
            public string IconSource { get; private set; }
            public string IconName
            {
                set
                {
                    IconSource = "/AnniPanel;component/Images/" + value;
                }
            }
        }
        #endregion

        #region Private delegates
        private delegate void ProcessRequestDelegate(Request request);
        #endregion

        #region Private classes
        private class Request : IAsyncResult
        {
            public Request(ProcessRequestDelegate process, bool isComplete, object asyncState)
            {
                _proc = process;
                _isAsyncComplete = isComplete;
                _asyncState = asyncState;
            }
            public object AsyncState
            {
                get { return _asyncState; }
            }

            public WaitHandle AsyncWaitHandle
            {
                get { return _completeEvent; }
            }

            public bool CompletedSynchronously
            {
                get { return _isAsyncComplete; }
            }

            public bool IsCompleted
            {
                get { return _isComplete; }
            }

            public void Process()
            {
                _proc.Invoke(this);
                _isComplete = true;
                _completeEvent.Set();
            }

            public void Complete()
            {
                _completeEvent.Set();
            }

            public Exception ProcessException { get; set; }

            private readonly ProcessRequestDelegate _proc;
            private readonly bool _isAsyncComplete;
            private bool _isComplete = false;
            private readonly ManualResetEvent _completeEvent = new ManualResetEvent(false);
            private object _asyncState;
        }
        private class LoginData
        {
            public LoginData(string userName, string password)
            {
                UserName = userName;
                Password = password;
                Status = LoginStatus.NetworkFailure;
            }
            public string UserName { get; private set; }
            public string Password { get; private set; }
            public LoginStatus Status { get; set; }
        }
        private class QueryDirectoryData
        {
            public QueryDirectoryData(long rootId, string query, object data)
            {
                RootId = rootId;
                Query = query;
                Data = data;
            }
            public long RootId { get; private set; }
            public string Query { get; private set; }
            public File[] Files { get; set; }
            public object Data { get; private set; }
        }
        #endregion

        #region Private fields
        private AnniCOMTypeLib.IAnniSafe _anniSafe;
        private readonly Thread _thread = new Thread(
            new ParameterizedThreadStart(ConnectionThread));
        private readonly Queue<Request> _requestQueue = new Queue<Request>();
        private readonly ManualResetEvent _terminateEvent = new ManualResetEvent(false);
        private readonly ManualResetEvent _queueEvent = new ManualResetEvent(false);
        private readonly Dictionary<AnniCOMTypeLib.LoginStatus, LoginStatus> _statusMap =
            new Dictionary<AnniCOMTypeLib.LoginStatus, LoginStatus>()
            {
                {AnniCOMTypeLib.LoginStatus.LoginStatusSucceeded, LoginStatus.Success},
                {AnniCOMTypeLib.LoginStatus.LoginStatusAccessDenied, LoginStatus.AccessDenied},
                {AnniCOMTypeLib.LoginStatus.LoginStatusCommunicationLoss, LoginStatus.NetworkFailure},
                {AnniCOMTypeLib.LoginStatus.LoginStatusException, LoginStatus.Exception}
            };
        #endregion

    }
}