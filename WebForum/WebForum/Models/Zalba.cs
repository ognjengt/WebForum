using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WebForum.Models
{
    public class Zalba
    {
        public string Id { get; set; }
        public string Tekst { get; set; }
        public DateTime DatumZalbe { get; set; }
        public string Entitet { get; set; }
        public string KorisnikKojiJeUlozio { get; set; }
        public string AutorZaljenogEntiteta { get; set; }
        public string TipEntiteta { get; set; }
    }
}