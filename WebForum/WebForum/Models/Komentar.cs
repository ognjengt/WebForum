using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WebForum.Models
{
    public class Komentar
    {
        public string Id { get; set; }
        public string TemaKojojPripada { get; set; }
        public string Autor { get; set; }
        public DateTime DatumKomentara { get; set; }
        public string RoditeljskiKomentar { get; set; }
        public List<string> Podkomentari { get; set; }
        public string Tekst { get; set; }
        public int PozitivniGlasovi { get; set; }
        public int NegativniGlasovi { get; set; }
        public bool Izmenjen { get; set; }
    }
}