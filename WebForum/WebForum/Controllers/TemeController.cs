using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
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
                        string[] commentSplitter = splitter[4].Split('|');
                        foreach (string komentar in commentSplitter)
                        {
                            listaKomentara.Add(komentar);
                        }

                        listaTema.Add(new Tema(splitter[0], splitter[1], splitter[2], splitter[3],splitter[4],DateTime.Parse(splitter[5]), Int32.Parse(splitter[6]), Int32.Parse(splitter[7]),listaKomentara));
                    }
                }

            }
            sr.Close();
            dbOperater.Reader.Close();
            return listaTema;
        }
    }
}
