using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using WebForum.Models;

namespace WebForum.Controllers
{
    public class SearchController : ApiController
    {
        [HttpPost]
        [ActionName("PretraziPodforume")]
        public List<Podforum> PretraziPodforume([FromBody]Podforum podforumZaPretragu)
        {
            return new List<Podforum>();
        }

        [HttpPost]
        [ActionName("PretraziTeme")]
        public List<Tema> PretraziTeme([FromBody]Tema temaZaPretragu)
        {
            return new List<Tema>();
        }

        [HttpPost]
        [ActionName("PretraziKorisnike")]
        public List<Korisnik> PretraziKorisnike([FromBody]Korisnik korisnikZaPretragu)
        {
            return new List<Korisnik>();
        }
    }
}
