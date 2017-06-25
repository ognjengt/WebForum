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
    public class KomentariController : ApiController
    {
        DbOperater dbOperater = new DbOperater();

        [ActionName("DodajKomentarNaTemu")]
        public Komentar DodajKomentarNaTemu([FromBody]Komentar k)
        {
            // Treba da splituje k.TemaKojojPripada po - i pita da li je splitter[0] == splitovanaTema[0] (splitovanaTema[0] je podforum u kom se tema nalazi a splitovanaTema[1] je naslov teme)
            // i trbea da pita da li je splitter[1] == splitovanaTema[1]

            string[] splitovanaTema = k.TemaKojojPripada.Split('-');
            List<string> listaSvihTema = new List<string>();
            int brojac = 0;
            int indexZaIzmenu = -1;

            StreamReader sr = dbOperater.getReader("teme.txt");
            string line = "";
            while ((line = sr.ReadLine()) != null)
            {
                listaSvihTema.Add(line);
                brojac++;

                string[] splitter = line.Split(';');
                if (splitter[0] == splitovanaTema[0] && splitter[1] == splitovanaTema[1])
                {
                    indexZaIzmenu = brojac;
                }
            }
            sr.Close();
            dbOperater.Reader.Close();
            // Upis u teme.txt tj dodavanje novog
            StreamWriter sw = dbOperater.getBulkWriter("teme.txt");


            k.Id = Guid.NewGuid().ToString();
            k.DatumKomentara = DateTime.Now;
            k.Izmenjen = false;
            k.NegativniGlasovi = 0;
            k.PozitivniGlasovi = 0;
            k.Podkomentari = new List<string>();
            k.RoditeljskiKomentar = "nemaRoditelja";

            listaSvihTema[indexZaIzmenu-1] += "|" + k.Id;

            foreach (string tema in listaSvihTema)
            {
                sw.WriteLine(tema);

            }
            sw.Close();
            dbOperater.Writer.Close();

            // Upis u komentari.txt
            StreamWriter swKomentari = dbOperater.getWriter("komentari.txt");
            swKomentari.WriteLine(k.Id + ";" + k.TemaKojojPripada + ";" + k.Autor + ";" + k.DatumKomentara.ToShortDateString() +";"+ k.Tekst + ";" + k.PozitivniGlasovi.ToString() +";"+ k.NegativniGlasovi.ToString() +";"+ k.Izmenjen.ToString());

            swKomentari.Close();
            dbOperater.Writer.Close();
            return k;
        }
    }
}
