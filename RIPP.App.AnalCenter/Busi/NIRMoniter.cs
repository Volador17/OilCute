using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Threading;
namespace RIPP.App.AnalCenter.Busi
{
    public class NIRMoniter
    {
        private string _folder =Path.Combine( AppDomain.CurrentDomain.BaseDirectory,"spec");
        private DateTime _workTime = DateTime.Now;
        private int _interval = 1000;
        private Thread _thread = null;
        private bool _running = false;

        public event EventHandler<NIRMonitorHandler> OnFoundSpecs;

        public NIRMoniter(string folder = null, int interval = 1000)
        {
            if (!string.IsNullOrWhiteSpace(folder))
                this._folder = folder;
            if (interval > 50)
                this._interval = interval;
        }

        public void Start()
        {
            if (_thread == null)
                _thread = new Thread(new ThreadStart(_worker));
            if (_thread.ThreadState == ThreadState.Aborted)
                _thread = new Thread(new ThreadStart(_worker));
            if (_thread.ThreadState == ThreadState.Unstarted)
            {
                _thread.IsBackground = true;
                _thread.Start();
                _running = true;
            }
            this._workTime = DateTime.Now;
        }

        public void Stop()
        {
            try
            {
                _running = false;
                _thread.Abort();
                _thread.Join();
            }
            catch { }
        }


        private void _worker()
        {
            while (true)
            {
                if (!_running)
                {
                    Thread.Sleep(1000);
                    continue;
                }
                LinkedList<FileInfo> ret = new LinkedList<FileInfo>();

                DirectoryInfo di = new DirectoryInfo(_folder);
                if (!di.Exists)
                    di.Create();

                FileInfo[] files =("*.txt|*.csv").Split('|').SelectMany(f => di.GetFiles(f)).ToArray();

                var newfile = files.Where(f => f.LastWriteTime > this._workTime).OrderByDescending(f => f.LastWriteTime).ToArray();

                this._workTime = DateTime.Now;

                if (newfile.Length > 0 && this.OnFoundSpecs != null)
                {
                    this.OnFoundSpecs.BeginInvoke(this, new NIRMonitorHandler()
                    {
                        LastTime = DateTime.Now,
                        NewFile = newfile
                    }, null, null);
                }

                Thread.Sleep(this._interval);

            }

        }
    }

    public class NIRMonitorHandler : EventArgs
    {
        public DateTime LastTime { set; get; }

        public FileInfo[] NewFile { set; get; }
    }
}
