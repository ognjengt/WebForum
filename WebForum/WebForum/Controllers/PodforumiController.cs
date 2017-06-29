using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using System.Web;
using System.Web.Hosting;
using System.Web.Http;
using WebForum.Helpers;
using WebForum.Models;

namespace WebForum.Controllers
{
    public class PodforumiController : ApiController
    {
        DbOperater dbOperater = new DbOperater();

        /// <summary>
        /// Vraca sve postojece podforume
        /// </summary>
        /// <returns></returns>
        [ActionName("GetAll")]
        public List<Podforum> GetAll()
        {
            List<Podforum> listaSvihPodforuma = new List<Podforum>();

            StreamReader sr = dbOperater.getReader("podforumi.txt");
            string line = "";
            while ( (line = sr.ReadLine()) != null )
            {
                if (line != "")
                {
                    
                    string[] splitter = line.Split(';');
                    List<string> listaModeratora = new List<string>();

                    string[] moderatorSplitter = splitter[5].Split('|');
                    foreach (string moderator in moderatorSplitter)
                    {
                        listaModeratora.Add(moderator);
                    }
                    listaSvihPodforuma.Add(new Podforum(splitter[0], splitter[1], splitter[2], splitter[3], splitter[4], listaModeratora));
                }
                
            }
            sr.Close();
            dbOperater.Reader.Close();
            return listaSvihPodforuma;
        }

        /// <summary>
        /// Vraca forum koji ima naziv koji je prosledjen
        /// </summary>
        /// <param name="naziv"></param>
        /// <returns></returns>
        [ActionName("GetPodforumByNaziv")]
        public Podforum GetPodforumByNaziv(string naziv)
        {
            StreamReader sr = dbOperater.getReader("podforumi.txt");
            string line = "";

            while ( (line = sr.ReadLine()) != null )
            {
                string[] splitter = line.Split(';');
                if (splitter[0] == naziv)
                {
                    sr.Close();
                    dbOperater.Reader.Close();
                    List<string> listaModeratora = new List<string>();

                    string[] moderatorSplitter = splitter[5].Split('|');
                    foreach (string moderator in moderatorSplitter)
                    {
                        listaModeratora.Add(moderator);
                    }
                    return new Podforum(splitter[0], splitter[1], splitter[2], splitter[3], splitter[4], listaModeratora);
                }
            }

            sr.Close();
            dbOperater.Reader.Close();
            return null;
        }

        /// <summary>
        /// Dodaje podforum sa prosledjenim parametrima u podforumi.txt
        /// </summary>
        /// <param name="p"></param>
        /// <returns></returns>
        [HttpPost]
        [ActionName("DodajPodforum")]
        public Podforum DodajPodforum([FromBody]Podforum p)
        {
            StreamReader sr = dbOperater.getReader("podforumi.txt");
            string line = "";
            while ((line = sr.ReadLine()) != null)
            {
                string[] splitter = line.Split(';');
                if (splitter[0] == p.Naziv)
                {
                    sr.Close();
                    dbOperater.Reader.Close();
                    return null;
                }
            }
            sr.Close();
            dbOperater.Reader.Close();
            StreamWriter sw = dbOperater.getWriter("podforumi.txt");
            p.Moderatori = new List<string>();
            sw.WriteLine(p.Naziv+";"+p.Opis+";"+p.Ikonica+";"+p.SpisakPravila+";"+p.OdgovorniModerator+";"+p.OdgovorniModerator); // za pocetak ima samo jedan moderator
            sw.Close();
            dbOperater.Writer.Close();

            return p;
        }

        /// <summary>
        /// Uploaduje sliku u /Content/img/podforumi, sa nazivom koji je izvucen iz headera, takodje se svi podaci potrebni izvlace iz headera
        /// </summary>
        /// <returns></returns>
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
                        String filePath = HostingEnvironment.MapPath("~/Content/img/podforumi");
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
        [ActionName("ObrisiPodforum")]
        public bool ObrisiPodforum([FromBody]Podforum podforumZaBrisanje)
        {
            // 1. Prodji kroz sve podforumi.txt i kada nadjes da je splitter[0] == p.Naziv preskoci ga sa dodavanjem u listu
            // 2. Prodji kroz sve teme i svaka koja ima splitter[0] == p.Naziv obrisi, tj nemoj je prepisati
            // 3. Prodji kroz komentare i svaki obrisan dodaj u listuObrisanih
            // 4. Prodji kroz podkomentare i obrisi one ciji su roditelji u listiObrisanih
            // 5. prodji kroz lajkDislajkKomentari
            // 6. Prodji kroz lajk dislajk teme

            // -------------------------------------- 1 ----------------------------------------

            StreamReader readerPodforuma = dbOperater.getReader("podforumi.txt");
            List<string> listaPodforumaZaPonovniUpis = new List<string>();

            string linijaPodforum = "";
            while ((linijaPodforum = readerPodforuma.ReadLine()) != null)
            {
                bool nadjen = false;
                string[] podforumSplitter = linijaPodforum.Split(';');
                if (podforumSplitter[0] == podforumZaBrisanje.Naziv)
                {
                    nadjen = true;
                }
                if (!nadjen)
                {
                    listaPodforumaZaPonovniUpis.Add(linijaPodforum);
                }
            }

            readerPodforuma.Close();
            dbOperater.Reader.Close();

            StreamWriter writerPodforuma = dbOperater.getBulkWriter("podforumi.txt");

            foreach (string podforumLn in listaPodforumaZaPonovniUpis)
            {
                writerPodforuma.WriteLine(podforumLn);
            }
            writerPodforuma.Close();
            dbOperater.Writer.Close();

            // -------------------------------------- 1 close ----------------------------------

            // -------------------------------------- 2 ----------------------------------------

            StreamReader readerTema = dbOperater.getReader("teme.txt");
            List<string> listaTemaZaPonovniUpis = new List<string>();

            string linijaTema = "";
            while ((linijaTema = readerTema.ReadLine()) != null)
            {
                bool nadjena = false;
                string[] temaSplitter = linijaTema.Split(';');
                if (temaSplitter[0] == podforumZaBrisanje.Naziv)
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

            // -------------------------------------- 2 close ----------------------------------

            // -------------------------------------- 3 ----------------------------------------

            StreamReader komentariReader = dbOperater.getReader("komentari.txt");
            List<string> listaKomentaraZaBrisanje = new List<string>();

            List<string> listaKomentaraZaPonovniUpis = new List<string>();

            string komentarLinija = "";

            while ((komentarLinija = komentariReader.ReadLine()) != null)
            {
                bool nadjen = false;

                string[] komentarSplitter = komentarLinija.Split(';');
                string[] podforumNaslovTemeSplitter = komentarSplitter[1].Split('-');
                string podforum = podforumNaslovTemeSplitter[0];
                string naslov = podforumNaslovTemeSplitter[1];

                if (podforum == podforumZaBrisanje.Naziv)
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

            // -------------------------------------- 3 close ----------------------------------

            // -------------------------------------- 4 ----------------------------------------

            StreamReader podkomentariReader = dbOperater.getReader("podkomentari.txt");

            List<string> listaPodkomentaraZaBrisanje = new List<string>();

            List<string> listaPodkomentaraZaPonovniUpis = new List<string>();

            string podkomentarLinija = "";
            while ((podkomentarLinija = podkomentariReader.ReadLine()) != null)
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

            // -------------------------------------- 4 close ----------------------------------

            // -------------------------------------- 5 ----------------------------------------

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
                lajkovaniDislajkovaniWriter.WriteLine(likeDislikeLn);
            }
            lajkovaniDislajkovaniWriter.Close();
            dbOperater.Reader.Close();

            // -------------------------------------- 5 close ----------------------------------

            // -------------------------------------- 6 ----------------------------------------

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

                if (podforum == podforumZaBrisanje.Naziv)
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

            // -------------------------------------- 6 close ----------------------------------

            return true;
        }

    }
}
