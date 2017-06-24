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

    }
}
