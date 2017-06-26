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

            sw.WriteLine();
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
    }
}
