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
        DbOperater dbOperaterPodkomentari = new DbOperater();

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
            k.Obrisan = false;

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
            swKomentari.WriteLine(k.Id + ";" + k.TemaKojojPripada + ";" + k.Autor + ";" + k.DatumKomentara.ToShortDateString() +";"+k.RoditeljskiKomentar+";"+ k.Tekst + ";" + k.PozitivniGlasovi.ToString() +";"+ k.NegativniGlasovi.ToString() +";"+ k.Izmenjen.ToString()+";"+k.Obrisan.ToString()+";"+"nemaPodkomentara");

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
            List<Komentar> listaPodkomentara = new List<Komentar>();

            while ((line = sr.ReadLine()) != null)
            {
                string[] splitter = line.Split(';');
                //ovaj splitter splituje komentare
                // Daj mi taj komentar samo ako nije obrisan
                if (splitter[1] == idTeme && splitter[9] == "False")
                {
                    listaPodkomentara = new List<Komentar>();
                    string[] ideviPodkomentara = splitter[10].Split('|');
                    foreach (string idPodkomentaraUKomentarima in ideviPodkomentara)
                    {
                        if (idPodkomentaraUKomentarima != "nemaPodkomentara")
                        {
                            StreamReader readerPodkomentara = dbOperaterPodkomentari.getReader("podkomentari.txt");
                            string podkomentarLinija = "";
                            while ( (podkomentarLinija = readerPodkomentara.ReadLine()) != null )
                            {

                                string[] podkomentarTokens = podkomentarLinija.Split(';');
                                // Vrati sve podkomentare koji nisu obrisani
                                if (podkomentarTokens[1] == idPodkomentaraUKomentarima && podkomentarTokens[8] == "False")
                                {
                                    Komentar podkomentar = new Komentar();
                                    podkomentar.Id = podkomentarTokens[1];
                                    podkomentar.RoditeljskiKomentar = podkomentarTokens[0];
                                    podkomentar.Autor = podkomentarTokens[2];
                                    podkomentar.DatumKomentara = DateTime.Parse(podkomentarTokens[3]);
                                    podkomentar.Tekst = podkomentarTokens[4];
                                    podkomentar.PozitivniGlasovi = Int32.Parse(podkomentarTokens[5]);
                                    podkomentar.NegativniGlasovi = Int32.Parse(podkomentarTokens[6]);
                                    podkomentar.Izmenjen = bool.Parse(podkomentarTokens[7]);
                                    podkomentar.Obrisan = bool.Parse(podkomentarTokens[8]);
                                    podkomentar.TemaKojojPripada = podkomentarTokens[9];

                                    listaPodkomentara.Add(podkomentar);
                                }
                            }
                            readerPodkomentara.Close();
                            dbOperaterPodkomentari.Reader.Close();
                        }
                        
                    }
                    listaKomentaraZaTemu.Add(new Komentar(splitter[0],splitter[1],splitter[2],DateTime.Parse(splitter[3]),splitter[4],listaPodkomentara,splitter[5],Int32.Parse(splitter[6]),Int32.Parse(splitter[7]),bool.Parse(splitter[8]), bool.Parse(splitter[9])));
                }
            }

            sr.Close();
            dbOperater.Reader.Close();
            return listaKomentaraZaTemu;
        }

        [ActionName("DodajPodkomentar")]
        public Komentar DodajPodkomentar([FromBody]Komentar podkomentar)
        {
            List<string> listaSvihKomentara = new List<string>();
            int brojac = 0;
            int indexZaIzmenu = -1;

            StreamReader sr = dbOperater.getReader("komentari.txt");
            string line = "";
            while ((line = sr.ReadLine()) != null)
            {
                // NE zaboravi: mora proci kroz sve da bi dodao u listuSvihKomentara, kako bi mogao celu listu ponovo da upisem
                listaSvihKomentara.Add(line);
                brojac++;

                string[] splitter = line.Split(';');
                if (splitter[0] == podkomentar.RoditeljskiKomentar)
                {
                    indexZaIzmenu = brojac;
                }
            }
            sr.Close();
            dbOperater.Reader.Close();
            // Upis u komentari.txt tj dodavanje novog podkomentara na kraj
            StreamWriter sw = dbOperater.getBulkWriter("komentari.txt");

            podkomentar.Id = Guid.NewGuid().ToString();
            podkomentar.DatumKomentara = DateTime.Now;
            podkomentar.Izmenjen = false;
            podkomentar.NegativniGlasovi = 0;
            podkomentar.PozitivniGlasovi = 0;
            podkomentar.Obrisan = false;

            listaSvihKomentara[indexZaIzmenu - 1] += "|" + podkomentar.Id;

            foreach (string komentar in listaSvihKomentara)
            {
                sw.WriteLine(komentar);
            }
            sw.Close();
            dbOperater.Writer.Close();

            // Upis u podkomentari.txt
            StreamWriter swPodkomentari = dbOperater.getWriter("podkomentari.txt");
            swPodkomentari.WriteLine(podkomentar.RoditeljskiKomentar+";"+podkomentar.Id+";"+podkomentar.Autor+";"+podkomentar.DatumKomentara.ToShortDateString()+";"+podkomentar.Tekst+";"+podkomentar.PozitivniGlasovi.ToString()+";"+podkomentar.NegativniGlasovi.ToString()+";"+podkomentar.Izmenjen.ToString()+";"+podkomentar.Obrisan.ToString()+";"+podkomentar.TemaKojojPripada);

            swPodkomentari.Close();
            dbOperater.Writer.Close();
            return podkomentar;
        }

        [HttpPost]
        [ActionName("ThumbsUp")]
        public bool ThumbsUp([FromBody]KomentarLikeDislikeRequest komentarRequest)
        {
            StreamReader sr = dbOperater.getReader("lajkDislajkKomentari.txt");

            List<string> listaSvih = new List<string>();
            string line = "";
            bool changed = false;
            while ((line = sr.ReadLine()) != null)
            {
                bool isDisliked = false;

                string[] splitter = line.Split(';');
                // U slucaju da je vec lajkovao taj komentar vrati false
                if (splitter[0] == komentarRequest.KoVrsiAkciju && splitter[1] == komentarRequest.IdKomentara && splitter[2] == "like")
                {
                    sr.Close();
                    dbOperater.Reader.Close();
                    return false;
                }
                else if (splitter[0] == komentarRequest.KoVrsiAkciju && splitter[1] == komentarRequest.IdKomentara && splitter[2] == "dislike")
                {
                    isDisliked = true;
                    changed = true;
                    listaSvih.Add(komentarRequest.KoVrsiAkciju + ";" + komentarRequest.IdKomentara + ";like");

                }
                if (!isDisliked)
                {
                    listaSvih.Add(line);
                }

            }
            sr.Close();
            dbOperater.Reader.Close();

            if (!changed)
            {
                StreamWriter sw = dbOperater.getWriter("lajkDislajkKomentari.txt");

                sw.WriteLine(komentarRequest.KoVrsiAkciju + ";" + komentarRequest.IdKomentara + ";like");
                sw.Close();
                dbOperater.Writer.Close();
            }
            else
            {
                StreamWriter sw = dbOperater.getBulkWriter("lajkDislajkKomentari.txt");

                foreach (string lajkDislajk in listaSvih)
                {
                    sw.WriteLine(lajkDislajk);
                }
                sw.Close();
                dbOperater.Writer.Close();
            }
            // Nakon sto sam dodao u .txt fajl ko je lajkovao , sada nadji taj komentar i povecaj mu brojlajkovanih
            StreamReader komentariReader = dbOperater.getReader("komentari.txt");
            List<string> sviKomentari = new List<string>();

            string komentar = "";
            while ((komentar = komentariReader.ReadLine()) != null)
            {
                bool nadjena = false;

                string[] komentarTokens = komentar.Split(';');
                if (komentarTokens[0] == komentarRequest.IdKomentara)
                {
                    // nasli smo komentar kome treba povecati pozitivne glasove
                    nadjena = true;
                    int brojTrenutnoPozitivnih = Int32.Parse(komentarTokens[6]);
                    int brojTrenutnoNegativnih = Int32.Parse(komentarTokens[7]);
                    brojTrenutnoPozitivnih++;
                    if (changed)
                    {
                        brojTrenutnoNegativnih--;
                    }
                    sviKomentari.Add(komentarTokens[0]+";"+komentarTokens[1]+";"+komentarTokens[2] + ";" + komentarTokens[3] + ";" + komentarTokens[4] + ";" + komentarTokens[5] + ";" + brojTrenutnoPozitivnih.ToString() + ";" + brojTrenutnoNegativnih.ToString() + ";" + komentarTokens[8] + ";" + komentarTokens[9] + ";" + komentarTokens[10]);

                }
                if (!nadjena)
                {
                    sviKomentari.Add(komentar);
                }
            }
            komentariReader.Close();
            dbOperater.Reader.Close();

            StreamWriter komentariWriter = dbOperater.getBulkWriter("komentari.txt");
            foreach (string linijaKomentara in sviKomentari)
            {
                komentariWriter.WriteLine(linijaKomentara);
            }
            komentariWriter.Close();
            dbOperater.Writer.Close();

            // Sada sve ovo isto za podkomentare
            // Nakon sto sam dodao u .txt fajl ko je lajkovao , sada nadji taj PODKOMENTAR i povecaj mu brojlajkovanih
            StreamReader podkomentariReader = dbOperater.getReader("podkomentari.txt");
            List<string> sviPodkomentari = new List<string>();

            string podkomentar = "";
            while ((podkomentar = podkomentariReader.ReadLine()) != null)
            {
                bool nadjena = false;

                string[] podkomentarTokens = podkomentar.Split(';');
                if (podkomentarTokens[1] == komentarRequest.IdKomentara)
                {
                    // nasli smo komentar kome treba povecati pozitivne glasove
                    nadjena = true;
                    int brojTrenutnoPozitivnih = Int32.Parse(podkomentarTokens[5]);
                    int brojTrenutnoNegativnih = Int32.Parse(podkomentarTokens[6]);
                    brojTrenutnoPozitivnih++;
                    if (changed)
                    {
                        brojTrenutnoNegativnih--;
                    }
                    sviPodkomentari.Add(podkomentarTokens[0]+";"+ podkomentarTokens[1]+";"+podkomentarTokens[2]+";"+ podkomentarTokens[3]+";"+ podkomentarTokens[4]+";"+brojTrenutnoPozitivnih.ToString()+";"+brojTrenutnoNegativnih.ToString()+";"+ podkomentarTokens[7]+";"+ podkomentarTokens[8]+";"+ podkomentarTokens[9]);

                }
                if (!nadjena)
                {
                    sviPodkomentari.Add(podkomentar);
                }
            }
            podkomentariReader.Close();
            dbOperater.Reader.Close();

            StreamWriter podkomentariWriter = dbOperater.getBulkWriter("podkomentari.txt");
            foreach (string linijaKomentara in sviPodkomentari)
            {
                podkomentariWriter.WriteLine(linijaKomentara);
            }
            podkomentariWriter.Close();
            dbOperater.Writer.Close();

            return true;
        }

        [HttpPost]
        [ActionName("ThumbsDown")]
        public bool ThumbsDown([FromBody]KomentarLikeDislikeRequest komentarRequest)
        {
            StreamReader sr = dbOperater.getReader("lajkDislajkKomentari.txt");

            List<string> listaSvih = new List<string>();
            string line = "";
            bool changed = false;
            while ((line = sr.ReadLine()) != null)
            {
                bool isDisliked = false;

                string[] splitter = line.Split(';');
                // U slucaju da je vec dislajkovao taj komentar vrati false
                if (splitter[0] == komentarRequest.KoVrsiAkciju && splitter[1] == komentarRequest.IdKomentara && splitter[2] == "dislike")
                {
                    sr.Close();
                    dbOperater.Reader.Close();
                    return false;
                }
                else if (splitter[0] == komentarRequest.KoVrsiAkciju && splitter[1] == komentarRequest.IdKomentara && splitter[2] == "like")
                {
                    isDisliked = true;
                    changed = true;
                    listaSvih.Add(komentarRequest.KoVrsiAkciju + ";" + komentarRequest.IdKomentara + ";dislike");

                }
                if (!isDisliked)
                {
                    listaSvih.Add(line);
                }

            }
            sr.Close();
            dbOperater.Reader.Close();

            if (!changed)
            {
                StreamWriter sw = dbOperater.getWriter("lajkDislajkKomentari.txt");

                sw.WriteLine(komentarRequest.KoVrsiAkciju + ";" + komentarRequest.IdKomentara + ";dislike");
                sw.Close();
                dbOperater.Writer.Close();
            }
            else
            {
                StreamWriter sw = dbOperater.getBulkWriter("lajkDislajkKomentari.txt");

                foreach (string lajkDislajk in listaSvih)
                {
                    sw.WriteLine(lajkDislajk);
                }
                sw.Close();
                dbOperater.Writer.Close();
            }
            // Nakon sto sam dodao u .txt fajl ko je dislajkovao , sada nadji taj komentar i povecaj mu brojlajkovanih
            StreamReader komentariReader = dbOperater.getReader("komentari.txt");
            List<string> sviKomentari = new List<string>();

            string komentar = "";
            while ((komentar = komentariReader.ReadLine()) != null)
            {
                bool nadjena = false;

                string[] komentarTokens = komentar.Split(';');
                if (komentarTokens[0] == komentarRequest.IdKomentara)
                {
                    // nasli smo komentar kome treba povecati pozitivne glasove
                    nadjena = true;
                    int brojTrenutnoPozitivnih = Int32.Parse(komentarTokens[6]);
                    int brojTrenutnoNegativnih = Int32.Parse(komentarTokens[7]);
                    brojTrenutnoNegativnih++;
                    if (changed)
                    {
                        brojTrenutnoPozitivnih--;
                    }
                    sviKomentari.Add(komentarTokens[0] + ";" + komentarTokens[1] + ";" + komentarTokens[2] + ";" + komentarTokens[3] + ";" + komentarTokens[4] + ";" + komentarTokens[5] + ";" + brojTrenutnoPozitivnih.ToString() + ";" + brojTrenutnoNegativnih.ToString() + ";" + komentarTokens[8] + ";" + komentarTokens[9] + ";" + komentarTokens[10]);

                }
                if (!nadjena)
                {
                    sviKomentari.Add(komentar);
                }
            }
            komentariReader.Close();
            dbOperater.Reader.Close();

            StreamWriter komentariWriter = dbOperater.getBulkWriter("komentari.txt");
            foreach (string linijaKomentara in sviKomentari)
            {
                komentariWriter.WriteLine(linijaKomentara);
            }
            komentariWriter.Close();
            dbOperater.Writer.Close();

            // Sada sve ovo isto za podkomentare
            // Nakon sto sam dodao u .txt fajl ko je dislajkovao , sada nadji taj PODKOMENTAR i povecaj mu brojlajkovanih
            StreamReader podkomentariReader = dbOperater.getReader("podkomentari.txt");
            List<string> sviPodkomentari = new List<string>();

            string podkomentar = "";
            while ((podkomentar = podkomentariReader.ReadLine()) != null)
            {
                bool nadjena = false;

                string[] podkomentarTokens = podkomentar.Split(';');
                if (podkomentarTokens[1] == komentarRequest.IdKomentara)
                {
                    // nasli smo komentar kome treba povecati pozitivne glasove
                    nadjena = true;
                    int brojTrenutnoPozitivnih = Int32.Parse(podkomentarTokens[5]);
                    int brojTrenutnoNegativnih = Int32.Parse(podkomentarTokens[6]);
                    brojTrenutnoNegativnih++;
                    if (changed)
                    {
                        brojTrenutnoPozitivnih--;
                    }
                    sviPodkomentari.Add(podkomentarTokens[0] + ";" + podkomentarTokens[1] + ";" + podkomentarTokens[2] + ";" + podkomentarTokens[3] + ";" + podkomentarTokens[4] + ";" + brojTrenutnoPozitivnih.ToString() + ";" + brojTrenutnoNegativnih.ToString() + ";" + podkomentarTokens[7] + ";" + podkomentarTokens[8] + ";" + podkomentarTokens[9]);

                }
                if (!nadjena)
                {
                    sviPodkomentari.Add(podkomentar);
                }
            }
            podkomentariReader.Close();
            dbOperater.Reader.Close();

            StreamWriter podkomentariWriter = dbOperater.getBulkWriter("podkomentari.txt");
            foreach (string linijaKomentara in sviPodkomentari)
            {
                podkomentariWriter.WriteLine(linijaKomentara);
            }
            podkomentariWriter.Close();
            dbOperater.Writer.Close();

            return true;
        }

        [HttpPost]
        [ActionName("ObrisiPodkomentar")]
        public bool ObrisiPodkomentar([FromBody]Komentar podkomentar)
        {

            return true;
        }
    }
}
