using System;
using System.Collections.Generic;
using System.IO;

namespace RIPP.Web.Chem.Upload
{
    public class Files
    {
        public List<FilesStatus> files { set; get; }
    }

    public class FilesStatus
    {
        public const string HandlerPath = "/Upload/";

        public string group { get; set; }
        public string name { get; set; }
        public string type { get; set; }
        public int size { get; set; }
        public string progress { get; set; }
        public string url { get; set; }
        public string deleteUrl { get; set; }
        public string deleteType { get; set; }
        public string error { get; set; }
        public string description { set; get; }
        public int id { set; get; }

       
    }
}