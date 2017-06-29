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
            porukaZaSlanje.Id = Guid.NewGuid().ToString();
            sw.WriteLine(porukaZaSlanje.Id + ";" + porukaZaSlanje.Posiljalac + ";" + porukaZaSlanje.Primalac + ";" + porukaZaSlanje.Sadrzaj + ";" + porukaZaSlanje.Procitana.ToString());
            sw.Close();
            dbOperater.Writer.Close();
            return true;
        }

        [HttpGet]
        [ActionName("GetAllUserMessages")]
        public List<Poruka> GetAllUserMessages(string username)
        {
            StreamReader sr = dbOperater.getReader("poruke.txt");
            List<Poruka> listaPoruka = new List<Poruka>();

            string line = "";
            while ((line = sr.ReadLine()) != null)
            {
                string[] splitter = line.Split(';');
                if (splitter[2] == username)
                {
                    Poruka p = new Poruka();
                    p.Id = splitter[0];
                    p.Posiljalac = splitter[1];
                    p.Primalac = splitter[2];
                    p.Sadrzaj = splitter[3];
                    p.Procitana = bool.Parse(splitter[4]);

                    listaPoruka.Add(p);
                }
            }
            sr.Close();
            dbOperater.Reader.Close();

            return listaPoruka;
        }

        [HttpPost]
        [ActionName("MarkirajKaoProcitano")]
        public bool MarkirajKaoProcitano(string id)
        {
            StreamReader sr = dbOperater.getReader("poruke.txt");
            List<string> listaPorukaZaPonovniUpis = new List<string>();

            string line = "";
            while ((line = sr.ReadLine()) != null)
            {
                bool nadjena = false;
                string[] splitter = line.Split(';');
                if (splitter[0] == id)
                {
                    nadjena = true;
                    listaPorukaZaPonovniUpis.Add(splitter[0]+";"+splitter[1]+";"+splitter[2]+";"+splitter[3]+";"+"True");
                }
                if (!nadjena)
                {
                    listaPorukaZaPonovniUpis.Add(line);
                }
            }
            sr.Close();
            dbOperater.Reader.Close();

            StreamWriter sw = dbOperater.getBulkWriter("poruke.txt");
            foreach (string poruka in listaPorukaZaPonovniUpis)
            {
                sw.WriteLine(poruka);
            }
            sw.Close();
            dbOperater.Writer.Close();

            return true;
        }

    }
}
