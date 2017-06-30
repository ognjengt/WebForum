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
    public class PreporukeController : ApiController
    {

        DbOperater dbOperater = new DbOperater();

        [HttpGet]
        [ActionName("GetPreporukeZaKorisnika")]
        public List<Tema> GetPreporukeZaKorisnika(string username)
        {
            List<Tema> listaPreporucenihTema = new List<Tema>();

            StreamReader korisniciReader = dbOperater.getReader("korisnici.txt");
            List<Tema> listaPracenihTema = new List<Tema>();
            List<string> listaPracenihPodforumaString = new List<string>();

            string korLine = "";
            while ((korLine = korisniciReader.ReadLine()) != null)
            {
                string[] splitter = korLine.Split(';');
                if (splitter[0] == username)
                {
                    // uzmi mu pracene podforume i teme
                    string[] splitterPracenihPodforuma = splitter[8].Split('|');
                    string[] splitterPracenihTema = splitter[9].Split('|');

                    listaPracenihPodforumaString.AddRange(splitterPracenihPodforuma);
                    foreach (string tema in splitterPracenihTema)
                    {
                        Tema t = new Tema();
                        if (tema == "nemaSnimljenihTema")
                        {
                            continue;
                        }
                        string[] splitterPfTema = tema.Split('-');
                        string podforumKomePripada = splitterPfTema[0];
                        string naslovTeme = splitterPfTema[1];
                        t.PodforumKomePripada = podforumKomePripada;
                        t.Naslov = naslovTeme;
                        listaPracenihTema.Add(t);
                    }
                    break;
                }
            }
            korisniciReader.Close();
            dbOperater.Reader.Close();

            // prodji kroz sve teme, ukoliko se podforum od te teme nalazi u listi pracenihPodforuma i ukoliko se naslov te teme NE nalazi u listi pracenih tema, i ukoliko ta tema
            // ima vise od 5 pozitivnih glasova, parsiraj u temu i dodaj u listuPreporucenih

            StreamReader readerTema = dbOperater.getReader("teme.txt");
            string temaLine = "";
            while ((temaLine = readerTema.ReadLine()) != null)
            {
                string[] splitter = temaLine.Split(';');
                bool pratiPodforum = listaPracenihPodforumaString.Any(podforum => podforum == splitter[0]);
                if (pratiPodforum)
                {
                    // ako korisnik prati podforum u kom se ova tema nalazi
                    // proveri da li se OVA tema nalazi u listi njegovih pracenih

                    bool nalaziSeUListiPracenih = listaPracenihTema.Any(tema => tema.PodforumKomePripada == splitter[0] && tema.Naslov == splitter[1]);
                    // ukoliko korisnik nije vec sacuvao ovu temu, i ova tema ima 5 ili vise pozitivnih glasova, dodaj mu je u preporuke
                    if (!nalaziSeUListiPracenih && Int32.Parse(splitter[6]) >= 5)
                    {
                        Tema t = new Tema();
                        t.PodforumKomePripada = splitter[0];
                        t.Naslov = splitter[1];
                        t.Tip = splitter[2];
                        t.Autor = splitter[3];
                        t.Sadrzaj = splitter[4];
                        t.DatumKreiranja = DateTime.Parse(splitter[5]);
                        t.PozitivniGlasovi = Int32.Parse(splitter[6]);
                        t.NegativniGlasovi = Int32.Parse(splitter[7]);

                        listaPreporucenihTema.Add(t);
                    }
                }
            }

            readerTema.Close();
            dbOperater.Reader.Close();

            return listaPreporucenihTema;
        }
    }
}
