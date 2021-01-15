using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace HomePage.Models
{
    public class JsonFile
    {
        public string FilePath { get; set; }
        public string FileName { get; set; }
        public string FileSize { get; set; }
        public string FileFormat { get; set; }
        public string DownloadDate { get; set; }

    }
}