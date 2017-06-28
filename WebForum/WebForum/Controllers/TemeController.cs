using System;
using System.Collections.Generic;
using System.Drawing;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Hosting;
using System.Web.Http;
using WebForum.Helpers;
using WebForum.Models;

namespace WebForum.Controllers
{
    public class TemeController : ApiController
    {
        DbOperater dbOperater = new DbOperater();

        /// <summary>
        /// Vraca sve teme koje pripadaju prosledjenom podforumu
        /// </summary>
        /// <param name="podforum"></param>
        /// <returns></returns>
        [ActionName("GetTemeZaPodforum")]
        public List<Tema> GetTemeZaPodforum(string podforum)
        {
            List<Tema> listaTema = new List<Tema>();
            StreamReader sr = dbOperater.getReader("teme.txt");
            string line = "";
            while ((line = sr.ReadLine()) != null)
            {
                if (line != "")
                {

                    string[] splitter = line.Split(';');
                    List<string> listaKomentara = new List<string>();
                    
                    if (splitter[0] == podforum)
                    {
                        string[] commentSplitter = splitter[8].Split('|');
                        foreach (string komentar in commentSplitter)
                        {
                            if (komentar != "nePostoje")
                            {
                                listaKomentara.Add(komentar);
                            }
                            
                        }

                        listaTema.Add(new Tema(splitter[0], splitter[1], splitter[2], splitter[3],splitter[4],DateTime.Parse(splitter[5]), Int32.Parse(splitter[6]), Int32.Parse(splitter[7]),listaKomentara));
                    }
                }

            }
            sr.Close();
            dbOperater.Reader.Close();
            return listaTema;
        }

        /// <summary>
        /// Proverava da li tema sa prosledjenim naslovom i podforumom u koji pripada vec postoji
        /// </summary>
        /// <param name="t"></param>
        /// <returns></returns>
        [HttpPost]
        [ActionName("DodajTemu")]
        public Tema DodajTemu(Tema t)
        {
            StreamReader sr = dbOperater.getReader("teme.txt");
            string line = "";
            while ((line = sr.ReadLine()) != null)
            {
                string[] splitter = line.Split(';');
                if (splitter[0] == t.PodforumKomePripada && splitter[1] == t.Naslov)
                {
                    sr.Close();
                    dbOperater.Reader.Close();
                    return null;
                }
            }
            sr.Close();
            dbOperater.Reader.Close();
            StreamWriter sw = dbOperater.getWriter("teme.txt");

            t.Komentari = new List<string>();
            t.DatumKreiranja = DateTime.Now;
            t.PozitivniGlasovi = 0;
            t.NegativniGlasovi = 0;

            sw.WriteLine(t.PodforumKomePripada + ";" + t.Naslov + ";" + t.Tip + ";" + t.Autor + ";" + t.Sadrzaj + ";" + t.DatumKreiranja.ToShortDateString() + ";" + t.PozitivniGlasovi.ToString() + ";" + t.NegativniGlasovi.ToString() + ";" + "nePostoje");
            sw.Close();
            dbOperater.Writer.Close();
            return t;
        }

        [ActionName("GetTemaByNaziv")]
        public Tema GetTemaByNaziv(string podforum, string tema)
        {
            StreamReader sr = dbOperater.getReader("teme.txt");
            string line = "";

            List<string> listaKomentara = new List<string>();

            while ((line = sr.ReadLine()) != null)
            {
                string[] splitter = line.Split(';');
                if (splitter[0] == podforum && splitter[1] == tema)
                {
                    string[] komentarSplitter = splitter[8].Split('|');
                    foreach (string komentar in komentarSplitter)
                    {
                        if (komentar != "nePostoje")
                        {
                            listaKomentara.Add(komentar);
                        }
                    }
                    sr.Close();
                    dbOperater.Reader.Close();
                    return new Tema(splitter[0],splitter[1],splitter[2],splitter[3],splitter[4],DateTime.Parse(splitter[5]),Int32.Parse(splitter[6]),Int32.Parse(splitter[7]),listaKomentara);
                }
            }

            sr.Close();
            dbOperater.Reader.Close();
            return null;
        }

        [HttpPost]
        [ActionName("UploadImage")]
        public HttpResponseMessage UploadImage()
        {
            var result = new HttpResponseMessage(HttpStatusCode.OK);
            if (Request.Content.IsMimeMultipartContent())
            {
                Request.Content.ReadAsMultipartAsync<MultipartMemoryStreamProvider>(new MultipartMemoryStreamProvider()).ContinueWith((task) =>
                {

                    MultipartMemoryStreamProvider provider = task.Result;
                    foreach (HttpContent content in provider.Contents)
                    {
                        Stream stream = content.ReadAsStreamAsync().Result;
                        Image image = Image.FromStream(stream);
                        var testName = content.Headers.ContentDisposition.Name;
                        String filePath = HostingEnvironment.MapPath("~/Content/img/teme");
                        string slika = Request.Headers.GetValues("slika").First();
                        string[] spliter = slika.Split('.');
                        string imeSlike = spliter[0];
                        string ekstenzija = spliter[1];
                        String fileName = slika + "." + ekstenzija;
                        String fullPath = Path.Combine(filePath, fileName);
                        image.Save(fullPath);
                    }
                });
                return result;
            }
            else
            {
                throw new HttpResponseException(Request.CreateResponse(HttpStatusCode.NotAcceptable, "This request is not properly formatted"));
            }
        }

        [HttpPost]
        [ActionName("ThumbsUp")]
        public bool ThumbsUp([FromBody]TemaLikeDislikeRequest temaRequest)
        {
            StreamReader sr = dbOperater.getReader("lajkDislajkTeme.txt");

            List<string> listaSvih = new List<string>();

            string[] temaRequestSplit = temaRequest.PunNazivTeme.Split('-');
            string podforumKomePripada = temaRequestSplit[0];
            string naslovTeme = temaRequestSplit[1];

            string line = "";
            bool changed = false;
            while ((line = sr.ReadLine()) != null)
            {
                bool isDisliked = false;

                string[] splitter = line.Split(';');
                // U slucaju da je vec lajkovao tu temu vrati false
                if (splitter[0] == temaRequest.KoVrsiAkciju && splitter[1] == temaRequest.PunNazivTeme && splitter[2] == "like")
                {
                    sr.Close();
                    dbOperater.Reader.Close();
                    return false;
                }
                else if (splitter[0] == temaRequest.KoVrsiAkciju && splitter[1] == temaRequest.PunNazivTeme && splitter[2] == "dislike")
                {
                    isDisliked = true;
                    changed = true;
                    listaSvih.Add(temaRequest.KoVrsiAkciju + ";" + temaRequest.PunNazivTeme + ";like");

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
                StreamWriter sw = dbOperater.getWriter("lajkDislajkTeme.txt");

                sw.WriteLine(temaRequest.KoVrsiAkciju + ";" + temaRequest.PunNazivTeme + ";like");
                sw.Close();
                dbOperater.Writer.Close();
            }
            else
            {
                StreamWriter sw = dbOperater.getBulkWriter("lajkDislajkTeme.txt");

                foreach (string lajkDislajk in listaSvih)
                {
                    sw.WriteLine(lajkDislajk);
                }
                sw.Close();
                dbOperater.Writer.Close();
            }
            // Nakon sto sam dodao u .txt fajl ko je lajkovao , sada nadji tu temu i povecaj joj brojlajkovanih
            StreamReader temeReader = dbOperater.getReader("teme.txt");
            List<string> sveTeme = new List<string>();

            string tema = "";
            while ((tema = temeReader.ReadLine()) != null)
            {
                bool nadjena = false;

                string[] temaTokens = tema.Split(';');
                if (temaTokens[0] == podforumKomePripada && temaTokens[1] == naslovTeme)
                {
                    // nasli smo temu kojoj treba povecati pozitivne glasove
                    nadjena = true;
                    int brojTrenutnoPozitivnih = Int32.Parse(temaTokens[6]);
                    int brojTrenutnoNegativnih = Int32.Parse(temaTokens[7]);
                    brojTrenutnoPozitivnih++;
                    if (changed)
                    {
                        brojTrenutnoNegativnih--;
                    }
                    sveTeme.Add(temaTokens[0] + ";" + temaTokens[1] + ";" + temaTokens[2] + ";" + temaTokens[3] + ";" + temaTokens[4] + ";" + temaTokens[5] + ";" + brojTrenutnoPozitivnih.ToString() + ";" + brojTrenutnoNegativnih.ToString() + ";" + temaTokens[8]);

                }
                if (!nadjena)
                {
                    sveTeme.Add(tema);
                }
            }
            temeReader.Close();
            dbOperater.Reader.Close();

            StreamWriter temeWriter = dbOperater.getBulkWriter("teme.txt");
            foreach (string linijaTeme in sveTeme)
            {
                temeWriter.WriteLine(linijaTeme);
            }
            temeWriter.Close();
            dbOperater.Writer.Close();

            return true;

        }

        [HttpPost]
        [ActionName("ThumbsDown")]
        public bool ThumbsDown([FromBody]TemaLikeDislikeRequest temaRequest)
        {
            StreamReader sr = dbOperater.getReader("lajkDislajkTeme.txt");

            List<string> listaSvih = new List<string>();

            string[] temaRequestSplit = temaRequest.PunNazivTeme.Split('-');
            string podforumKomePripada = temaRequestSplit[0];
            string naslovTeme = temaRequestSplit[1];

            string line = "";
            bool changed = false;
            while ((line = sr.ReadLine()) != null)
            {
                bool isLiked = false;

                string[] splitter = line.Split(';');
                // U slucaju da je vec dislajkovao tu temu vrati false
                if (splitter[0] == temaRequest.KoVrsiAkciju && splitter[1] == temaRequest.PunNazivTeme && splitter[2] == "dislike")
                {
                    sr.Close();
                    dbOperater.Reader.Close();
                    return false;
                }
                else if (splitter[0] == temaRequest.KoVrsiAkciju && splitter[1] == temaRequest.PunNazivTeme && splitter[2] == "like")
                {
                    isLiked = true;
                    changed = true;
                    listaSvih.Add(temaRequest.KoVrsiAkciju + ";" + temaRequest.PunNazivTeme + ";dislike");

                }
                if (!isLiked)
                {
                    listaSvih.Add(line);
                }

            }
            sr.Close();
            dbOperater.Reader.Close();

            if (!changed)
            {
                StreamWriter sw = dbOperater.getWriter("lajkDislajkTeme.txt");

                sw.WriteLine(temaRequest.KoVrsiAkciju + ";" + temaRequest.PunNazivTeme + ";dislike");
                sw.Close();
                dbOperater.Writer.Close();
            }
            else
            {
                StreamWriter sw = dbOperater.getBulkWriter("lajkDislajkTeme.txt");

                foreach (string lajkDislajk in listaSvih)
                {
                    sw.WriteLine(lajkDislajk);
                }
                sw.Close();
                dbOperater.Writer.Close();
            }
            // Nakon sto sam dodao u .txt fajl ko je dislajkovao , sada nadji tu temu i povecaj joj brojDislajkovanih
            StreamReader temeReader = dbOperater.getReader("teme.txt");
            List<string> sveTeme = new List<string>();

            string tema = "";
            while ((tema = temeReader.ReadLine()) != null)
            {
                bool nadjena = false;

                string[] temaTokens = tema.Split(';');
                if (temaTokens[0] == podforumKomePripada && temaTokens[1] == naslovTeme)
                {
                    // nasli smo temu kojoj treba povecati negativne glasove
                    nadjena = true;
                    int brojTrenutnoPozitivnih = Int32.Parse(temaTokens[6]);
                    int brojTrenutnoNegativnih = Int32.Parse(temaTokens[7]);
                    brojTrenutnoNegativnih++;
                    if (changed)
                    {
                        brojTrenutnoPozitivnih--;
                    }
                    sveTeme.Add(temaTokens[0] + ";" + temaTokens[1] + ";" + temaTokens[2] + ";" + temaTokens[3] + ";" + temaTokens[4] + ";" + temaTokens[5] + ";" + brojTrenutnoPozitivnih.ToString() + ";" + brojTrenutnoNegativnih.ToString() + ";" + temaTokens[8]);

                }
                if (!nadjena)
                {
                    sveTeme.Add(tema);
                }
            }
            temeReader.Close();
            dbOperater.Reader.Close();

            StreamWriter temeWriter = dbOperater.getBulkWriter("teme.txt");
            foreach (string linijaTeme in sveTeme)
            {
                temeWriter.WriteLine(linijaTeme);
            }
            temeWriter.Close();
            dbOperater.Writer.Close();

            return true;
        }

        [ActionName("GetLajkovaneTeme")]
        public List<string> GetLajkovaneTeme(string username)
        {
            List<string> listaLajkovanihTema = new List<string>();

            StreamReader sr = dbOperater.getReader("lajkDislajkTeme.txt");

            string line = "";
            while ((line = sr.ReadLine()) != null)
            {
                string[] splitter = line.Split(';');
                if (splitter[0] == username && splitter[2] == "like")
                {
                    listaLajkovanihTema.Add(splitter[1]);
                }
            }
            sr.Close();
            dbOperater.Reader.Close();

            return listaLajkovanihTema;
        }

        [ActionName("GetDislajkovaneTeme")]
        public List<string> GetDislajkovaneTeme(string username)
        {
            List<string> listaDislajkovanihTema = new List<string>();

            StreamReader sr = dbOperater.getReader("lajkDislajkTeme.txt");

            string line = "";
            while ((line = sr.ReadLine()) != null)
            {
                string[] splitter = line.Split(';');
                if (splitter[0] == username && splitter[2] == "dislike")
                {
                    listaDislajkovanihTema.Add(splitter[1]);
                }
            }
            sr.Close();
            dbOperater.Reader.Close();

            return listaDislajkovanihTema;
        }

        [HttpPost]
        [ActionName("ObrisiTemu")]
        public bool ObrisiTemu([FromBody]Tema temaZaBrisanje)
        {
            // 1. Prodji kroz sve teme.txt i kada nadjes da je splitter[0] == temaZaBrisanje.PodforumKomePripada i splitter[1] == temaZaBrisanje.Naslov tu nemoj dodati
            // 2. Prodji kroz sve komentare, i svaki koji u sebi sadrzi tu temu obrisi ga, tj prepisi komentare.txt a nemoj dodati te koji sadrze tu temu
            // 3. Za svaki komentar koji ne sadrzi tu temu dodaj njegov id u neku listu stringova, zatim prodji kroz sve podkomentari.txt i obrisi sve podkomentare ciji je splitter[0] == id-em iz liste obrisanih komentara, i takodje te obrisane ideve dodaj u jos neku listuObrisanihPodkomentara
            // 4. Prodji kroz lajkDislajkKomentari i obrisi svaki koji sadrzi neki id ili iz listeObrisanihKomentara ili iz listeObrisanihPodkomentara
            // 5. Prodji kroz lajkDislajkTeme i obrisi svaku temu ciji je splitter[1] == temaZaBrisanje.PodforumKomePripada-tema.Naslov
            // 6. Prodji kroz korisnici.txt i skloni iz splittera[9] sve nepotrebne teme i iz splittera[10] sve nepotrebne komentare koji se nalaze u listiObrisanihKomentara i listiObrisanihPodkomentara

            // -------------------------------- 1 ---------------------------------
            StreamReader readerTema = dbOperater.getReader("teme.txt");
            List<string> listaTemaZaPonovniUpis = new List<string>();

            string linijaTema = "";
            while ( (linijaTema = readerTema.ReadLine()) != null )
            {
                bool nadjena = false;
                string[] temaSplitter = linijaTema.Split(';');
                if (temaSplitter[0] == temaZaBrisanje.PodforumKomePripada && temaSplitter[1] == temaZaBrisanje.Naslov)
                {
                    nadjena = true;
                }
                if (!nadjena)
                {
                    listaTemaZaPonovniUpis.Add(linijaTema);
                }
            }

            readerTema.Close();
            dbOperater.Reader.Close();

            StreamWriter writerTema = dbOperater.getBulkWriter("teme.txt");

            foreach (string temaLn in listaTemaZaPonovniUpis)
            {
                writerTema.WriteLine(temaLn);
            }
            writerTema.Close();
            dbOperater.Writer.Close();

            // ------------------------------------------ 1 close ---------------------------------------------

            // ------------------------------------------ 2 ---------------------------------------------------

            StreamReader komentariReader = dbOperater.getReader("komentari.txt");
            List<string> listaKomentaraZaBrisanje = new List<string>();

            List<string> listaKomentaraZaPonovniUpis = new List<string>();

            string komentarLinija = "";

            while ( (komentarLinija = komentariReader.ReadLine()) != null )
            {
                bool nadjen = false;

                string[] komentarSplitter = komentarLinija.Split(';');
                string[] podforumNaslovTemeSplitter = komentarSplitter[1].Split('-');
                string podforum = podforumNaslovTemeSplitter[0];
                string naslov = podforumNaslovTemeSplitter[1];

                if (podforum == temaZaBrisanje.PodforumKomePripada && naslov == temaZaBrisanje.Naslov)
                {
                    listaKomentaraZaBrisanje.Add(komentarSplitter[0]);
                    nadjen = true;
                }
                if (!nadjen)
                {
                    listaKomentaraZaPonovniUpis.Add(komentarLinija);
                }
            }
            komentariReader.Close();
            dbOperater.Reader.Close();

            StreamWriter writerKomentara = dbOperater.getBulkWriter("komentari.txt");

            foreach (string komentarLn in listaKomentaraZaPonovniUpis)
            {
                writerKomentara.WriteLine(komentarLn);
            }
            writerKomentara.Close();
            dbOperater.Writer.Close();

            // ------------------------------------------ 2 close ---------------------------------------------

            // ------------------------------------------ 3 ---------------------------------------------------

            StreamReader podkomentariReader = dbOperater.getReader("podkomentari.txt");

            List<string> listaPodkomentaraZaBrisanje = new List<string>();

            List<string> listaPodkomentaraZaPonovniUpis = new List<string>();

            string podkomentarLinija = "";
            while ( (podkomentarLinija = podkomentariReader.ReadLine()) != null )
            {
                bool nadjen = false;
                string[] podkomentarSplitter = podkomentarLinija.Split(';');
                foreach (string idRoditelja in listaKomentaraZaBrisanje)
                {
                    if (podkomentarSplitter[0] == idRoditelja)
                    {
                        nadjen = true;
                        listaPodkomentaraZaBrisanje.Add(podkomentarSplitter[1]);
                    }
                }
                if (!nadjen)
                {
                    listaPodkomentaraZaPonovniUpis.Add(podkomentarLinija);
                }
            }

            podkomentariReader.Close();
            dbOperater.Reader.Close();

            StreamWriter podkomentariWriter = dbOperater.getBulkWriter("podkomentari.txt");
            foreach (string podkomentarLn in listaPodkomentaraZaPonovniUpis)
            {
                podkomentariWriter.WriteLine(podkomentarLn);
            }
            podkomentariWriter.Close();
            dbOperater.Writer.Close();

            // ------------------------------------------ 3 close ---------------------------------------------

            // ------------------------------------------ 4 ---------------------------------------------------

            StreamReader lajkDislajkKomentariReader = dbOperater.getReader("lajkDislajkKomentari.txt");
            List<string> listaLajkovanihDislajkovanihKomentaraZaPonovniUpis = new List<string>();

            string likeDislikeComLine = "";
            while ((likeDislikeComLine = lajkDislajkKomentariReader.ReadLine()) != null)
            {
                bool nadjen = false;
                string[] likeDislikeSplitter = likeDislikeComLine.Split(';');
                foreach (string idKomentara in listaKomentaraZaBrisanje)
                {
                    if (likeDislikeSplitter[1] == idKomentara)
                    {
                        nadjen = true;
                    }
                }
                if (!nadjen)
                {
                    listaLajkovanihDislajkovanihKomentaraZaPonovniUpis.Add(likeDislikeComLine);
                }
            }
            lajkDislajkKomentariReader.Close();
            dbOperater.Reader.Close();

            StreamWriter lajkovaniDislajkovaniWriter = dbOperater.getBulkWriter("lajkDislajkKomentari.txt");
            foreach (string likeDislikeLn in listaLajkovanihDislajkovanihKomentaraZaPonovniUpis)
            {
                lajkovaniDislajkovaniWriter.Write(likeDislikeLn);
            }
            lajkovaniDislajkovaniWriter.Close();
            dbOperater.Reader.Close();


            // ------------------------------------------ 4 close ---------------------------------------------

            // ------------------------------------------ 5 ---------------------------------------------------
            StreamReader lajkDislajkTemeReader = dbOperater.getReader("lajkDislajkTeme.txt");

            List<string> listaLajkovanihDislajkovanihTemaZaPonovniUpis = new List<string>();

            string likeDislikeTemeLinija = "";
            while ((likeDislikeTemeLinija = lajkDislajkTemeReader.ReadLine()) != null)
            {
                bool nadjen = false;

                string[] likeDislikeTemeLineSplitter = likeDislikeTemeLinija.Split(';');
                string[] podforumNazivSplitter = likeDislikeTemeLineSplitter[1].Split('-');
                string podforum = podforumNazivSplitter[0];
                string nazivTeme = podforumNazivSplitter[1];

                if (podforum == temaZaBrisanje.PodforumKomePripada && nazivTeme == temaZaBrisanje.Naslov)
                {
                    nadjen = true;
                }
                if (!nadjen)
                {
                    listaLajkovanihDislajkovanihTemaZaPonovniUpis.Add(likeDislikeTemeLinija);
                }
            }
            lajkDislajkTemeReader.Close();
            dbOperater.Reader.Close();

            StreamWriter writerLikeDislikeTeme = dbOperater.getBulkWriter("lajkDislajkTeme.txt");
            foreach (string temaLn in listaLajkovanihDislajkovanihTemaZaPonovniUpis)
            {
                writerLikeDislikeTeme.WriteLine(temaLn);
            }
            writerLikeDislikeTeme.Close();
            dbOperater.Writer.Close();

            // ------------------------------------------ 5 close ---------------------------------------------

            return true;
        }

        [HttpPost]
        [ActionName("IzmeniTemu")]
        public bool IzmeniTemu([FromBody]Tema temaZaIzmenu)
        {
            StreamReader sr = dbOperater.getReader("teme.txt");
            List<string> temeZaDodavanje = new List<string>();

            string line = "";
            while ((line = sr.ReadLine()) != null)
            {
                bool nadjen = false;
                string[] splitter = line.Split(';');
                if (splitter[0] == temaZaIzmenu.PodforumKomePripada && splitter[1] == temaZaIzmenu.Naslov)
                {
                    nadjen = true;
                    temeZaDodavanje.Add(splitter[0] + ";" + splitter[1] + ";" + splitter[2] + ";" + splitter[3] + ";" + temaZaIzmenu.Sadrzaj + ";" + splitter[5] + ";" + splitter[6] + ";" + splitter[7] + ";" + splitter[8]);
                }
                if (!nadjen)
                {
                    temeZaDodavanje.Add(line);
                }
            }
            sr.Close();
            dbOperater.Reader.Close();

            StreamWriter sw = dbOperater.getBulkWriter("teme.txt");
            foreach (string temaLn in temeZaDodavanje)
            {
                sw.WriteLine(temaLn);
            }
            sw.Close();
            dbOperater.Writer.Close();
            return true;
        }
    }
}
