using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WebForum.Models
{
    public class Tema
    {
        public string PodforumKomePripada { get; set; }
        public string Naslov { get; set; }
        public string Tip { get; set; }
        public string Autor { get; set; }
        // Komentari
        public string Sadrzaj { get; set; }
        public DateTime DatumKreiranja { get; set; }
        public int PozitivniGlasovi { get; set; }
        public int NegativniGlasovi { get; set; }
    }
}