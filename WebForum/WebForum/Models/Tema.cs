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
        public List<string> Komentari { get; set; }
        public string Sadrzaj { get; set; }
        public DateTime DatumKreiranja { get; set; }
        public int PozitivniGlasovi { get; set; }
        public int NegativniGlasovi { get; set; }

        public Tema(string podforumKomePripada,string naslov, string tip, string autor, string sadrzaj, DateTime datumKreiranja, int pozitivniG, int negativniG,List<string> komentari)
        {
            this.PodforumKomePripada = podforumKomePripada;
            this.Naslov = naslov;
            this.Tip = tip;
            this.Autor = autor;
            this.Komentari = komentari;
            this.Sadrzaj = sadrzaj;
            this.DatumKreiranja = datumKreiranja;
            this.PozitivniGlasovi = pozitivniG;
            this.NegativniGlasovi = negativniG;
        }

        public Tema() { }
    }
}