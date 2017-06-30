﻿using System;
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
    public class ZalbeController : ApiController
    {
        DbOperater dbOperater = new DbOperater();

        [HttpPost]
        [ActionName("PriloziZalbuNaPodforum")]
        public bool PriloziZalbuNaPodforum([FromBody]Zalba zalba)
        {
            zalba.DatumZalbe = DateTime.Now;
            zalba.Id = Guid.NewGuid().ToString();
            zalba.TipEntiteta = "Podforum";
            // treba da nadjem autora zaljenog entiteta
            StreamReader podforumiReader = dbOperater.getReader("podforumi.txt");
            string pfLine = "";
            while ((pfLine = podforumiReader.ReadLine()) != null)
            {
                string[] splitter = pfLine.Split(';');
                if (splitter[0] == zalba.Entitet)
                {
                    zalba.AutorZaljenogEntiteta = splitter[4];
                    break;
                }
            }
            podforumiReader.Close();
            dbOperater.Reader.Close();

            StreamReader sr = dbOperater.getReader("korisnici.txt");
            List<string> listaAdministratoraZaProsledjivanje = new List<string>();
            string line = "";
            while ((line = sr.ReadLine()) != null)
            {
                string[] splitter = line.Split(';');
                if (splitter[4] == "Administrator")
                {
                    listaAdministratoraZaProsledjivanje.Add(splitter[0]);
                }
            }
            sr.Close();
            dbOperater.Reader.Close();

            StreamWriter sw = dbOperater.getWriter("zalbe.txt");
            foreach (string administrator in listaAdministratoraZaProsledjivanje)
            {
                sw.WriteLine(zalba.Id + ";" + zalba.Entitet + ";" + zalba.Tekst + ";" + zalba.DatumZalbe.ToShortDateString() + ";" + zalba.KorisnikKojiJeUlozio + ";" + administrator+";"+zalba.AutorZaljenogEntiteta+";"+zalba.TipEntiteta);
            }
            sw.Close();
            dbOperater.Writer.Close();
            return true;
        }

        [HttpPost]
        [ActionName("PriloziZalbuNaTemu")]
        public bool PriloziZalbuNaTemu([FromBody]Zalba zalba)
        {
            string[] podforumTema = zalba.Entitet.Split('-');

            string podforumZaljeneTeme = podforumTema[0];
            string naslovZaljeneTeme = podforumTema[1];

            zalba.Id = Guid.NewGuid().ToString();
            zalba.DatumZalbe = DateTime.Now;
            zalba.TipEntiteta = "Tema";
            //treba da nadjem autora zaljene teme
            StreamReader temeReader = dbOperater.getReader("teme.txt");
            string temaLine = "";
            while ((temaLine = temeReader.ReadLine()) != null)
            {
                string[] splitter = temaLine.Split(';');
                if (splitter[0] == podforumZaljeneTeme && splitter[1] == naslovZaljeneTeme)
                {
                    zalba.AutorZaljenogEntiteta = splitter[3];
                    break;
                }
            }
            temeReader.Close();
            dbOperater.Reader.Close();

            // prvo nadji sve administratore da se njima prosledi
            StreamReader sr = dbOperater.getReader("korisnici.txt");
            List<string> listaAdministratoraZaProsledjivanje = new List<string>();
            string line = "";
            while ((line = sr.ReadLine()) != null)
            {
                string[] splitter = line.Split(';');
                if (splitter[4] == "Administrator")
                {
                    listaAdministratoraZaProsledjivanje.Add(splitter[0]);
                }
            }
            sr.Close();
            dbOperater.Reader.Close();

            // pa sada prodji kroz podforume i nadji odgovornog moderatora za podforum u kojem se ova tema nalazi
            StreamReader pfReader = dbOperater.getReader("podforumi.txt");
            string odgovorniModeratorPodforumaKomeTrebaDaSeProsledi = "";
            string pfLine = "";
            while ((pfLine = pfReader.ReadLine()) != null)
            {
                string[] splitter = pfLine.Split(';');
                if (splitter[0] == podforumZaljeneTeme)
                {
                    odgovorniModeratorPodforumaKomeTrebaDaSeProsledi = splitter[4];
                    break;
                }
            }
            pfReader.Close();
            dbOperater.Reader.Close();

            StreamWriter sw = dbOperater.getWriter("zalbe.txt");
            foreach (string administrator in listaAdministratoraZaProsledjivanje)
            {
                sw.WriteLine(zalba.Id + ";" + zalba.Entitet + ";" + zalba.Tekst + ";" + zalba.DatumZalbe.ToShortDateString() + ";" + zalba.KorisnikKojiJeUlozio + ";" + administrator + ";" + zalba.AutorZaljenogEntiteta + ";" + zalba.TipEntiteta);
            }
            sw.WriteLine(zalba.Id + ";" + zalba.Entitet + ";" + zalba.Tekst + ";" + zalba.DatumZalbe.ToShortDateString() + ";" + zalba.KorisnikKojiJeUlozio + ";" + odgovorniModeratorPodforumaKomeTrebaDaSeProsledi + ";" + zalba.AutorZaljenogEntiteta + ";" + zalba.TipEntiteta);
            sw.Close();
            dbOperater.Writer.Close();
            return true;
        }

        [HttpGet]
        [ActionName("GetSveZalbe")]
        public List<Zalba> GetSveZalbe(string username)
        {
            List<Zalba> listaZalbi = new List<Zalba>();
            StreamReader sr = dbOperater.getReader("zalbe.txt");

            string line = "";
            while ((line = sr.ReadLine()) != null)
            {
                string[] splitter = line.Split(';');
                if (splitter[5] == username)
                {
                    // Ako je npr neko i moderator za podforum i administrator, da mu se ne ispise 2 puta
                    bool postojiUListi = listaZalbi.Any(zalba => zalba.Id == splitter[0]);
                    if (postojiUListi)
                    {
                        continue;
                    }
                    Zalba z = new Zalba();
                    z.Id = splitter[0];
                    z.Entitet = splitter[1];
                    z.Tekst = splitter[2];
                    z.DatumZalbe = DateTime.Parse(splitter[3]);
                    z.KorisnikKojiJeUlozio = splitter[4];
                    z.AutorZaljenogEntiteta = splitter[6];
                    z.TipEntiteta = splitter[7];

                    listaZalbi.Add(z);
                }
            }
            sr.Close();
            dbOperater.Reader.Close();
            return listaZalbi;
        }

        [HttpPost]
        [ActionName("ObrisiZalbu")]
        public bool ObrisiZalbu([FromBody]Zalba zalbaZaBrisanje)
        {
            List<string> listaZalbiZaPonovniUpis = new List<string>();
            StreamReader sr = dbOperater.getReader("zalbe.txt");

            string line = "";
            while ((line = sr.ReadLine()) != null)
            {
                bool nadjena = false;
                string[] splitter = line.Split(';');

                if (splitter[0] == zalbaZaBrisanje.Id)
                {
                    nadjena = true;
                }
                if (!nadjena)
                {
                    listaZalbiZaPonovniUpis.Add(line);
                }
            }
            sr.Close();
            dbOperater.Reader.Close();

            StreamWriter sw = dbOperater.getBulkWriter("zalbe.txt");
            foreach (string zalbaLn in listaZalbiZaPonovniUpis)
            {
                sw.WriteLine(zalbaLn);
            }
            sw.Close();
            dbOperater.Reader.Close();
            return true;
        }

    }
}
