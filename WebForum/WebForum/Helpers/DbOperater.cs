using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;

namespace WebForum.Helpers
{
    public class DbOperater
    {
        public FileStream Writer { get; set; }
        public FileStream Reader { get; set; }
        public StreamWriter getWriter(string filename)
        {
            var dataFile = HttpContext.Current.Server.MapPath("~/App_Data/"+filename);
            Writer = new FileStream(dataFile, FileMode.Append, FileAccess.Write);
            return new StreamWriter(Writer);
        }

        public StreamReader getReader(string filename)
        {
            var dataFile = HttpContext.Current.Server.MapPath("~/App_Data/"+filename);
            Reader = new FileStream(dataFile, FileMode.Open);
            return new StreamReader(Reader);
        }

    }
}