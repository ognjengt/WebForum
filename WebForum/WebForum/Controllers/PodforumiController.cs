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
    public class PodforumiController : ApiController
    {
        DbOperater dbOperater = new DbOperater();

        [ActionName("GetAll")]
        public List<Podforum> GetAll()
        {
            List<Podforum> listaSvihPodforuma = new List<Podforum>();

            StreamReader sr = dbOperater.getReader("podforumi.txt");
            string line = "";

            while ( (line = sr.ReadLine()) != null )
            {
                string[] splitter = line.Split(';');
                listaSvihPodforuma.Add(new Podforum(splitter[0], splitter[1], splitter[2], splitter[3], splitter[4], new List<string>()));
            }
            sr.Close();
            dbOperater.Reader.Close();
            return listaSvihPodforuma;
        }
    }
}
