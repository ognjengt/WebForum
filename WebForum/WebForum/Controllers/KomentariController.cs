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

        [HttpPost]
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
            k.Podkomentari = new List<Komentar>();
            k.RoditeljskiKomentar = "nemaRoditelja";

            // Prvo ako ova tema nema komentar tj ako joj je spliter listaSvihTema[indexZaIzmenu-1][8] == 'nePostoje', obrisi to nePostoje

            listaSvihTema[indexZaIzmenu-1] += "|" + k.Id;

            foreach (string tema in listaSvihTema)
            {
                sw.WriteLine(tema);

            }
            sw.Close();
            dbOperater.Writer.Close();

            // Upis u komentari.txt
            StreamWriter swKomentari = dbOperater.getWriter("komentari.txt");
            swKomentari.WriteLine(k.Id + ";" + k.TemaKojojPripada + ";" + k.Autor + ";" + k.DatumKomentara.ToShortDateString() +";"+k.RoditeljskiKomentar+";"+ k.Tekst + ";" + k.PozitivniGlasovi.ToString() +";"+ k.NegativniGlasovi.ToString() +";"+ k.Izmenjen.ToString());

            swKomentari.Close();
            dbOperater.Writer.Close();
            return k;
        }

        [ActionName("GetKomentariZaTemu")]
        public List<Komentar> GetKomentariZaTemu(string idTeme)
        {
            StreamReader sr = dbOperater.getReader("komentari.txt");
            string line = "";

            List<Komentar> listaKomentaraZaTemu = new List<Komentar>();

            while ((line = sr.ReadLine()) != null)
            {
                string[] splitter = line.Split(';');
                if (splitter[1] == idTeme)
                {
                    listaKomentaraZaTemu.Add(new Komentar(splitter[0],splitter[1],splitter[2],DateTime.Parse(splitter[3]),splitter[4],new List<Komentar>(),splitter[5],Int32.Parse(splitter[6]),Int32.Parse(splitter[7]),bool.Parse(splitter[8])));
                }
            }

            sr.Close();
            dbOperater.Reader.Close();
            return listaKomentaraZaTemu;
        }
    }
}
