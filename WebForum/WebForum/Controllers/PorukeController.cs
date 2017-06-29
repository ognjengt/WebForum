using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using WebForum.Helpers;
using WebForum.Models;

namespace WebForum.Controllers
{
    public class PorukeController : ApiController
    {
        DbOperater dbOperater = new DbOperater();

        [HttpPost]
        [ActionName("PosaljiPoruku")]
        public bool PosaljiPoruku([FromBody]Poruka porukaZaSlanje)
        {
            StreamWriter sw = dbOperater.getWriter("poruke.txt");
            sw.WriteLine(porukaZaSlanje.Posiljalac + ";" + porukaZaSlanje.Primalac + ";" + porukaZaSlanje.Sadrzaj + ";" + porukaZaSlanje.Procitana.ToString());
            sw.Close();
            dbOperater.Writer.Close();
            return true;
        }

    }
}
