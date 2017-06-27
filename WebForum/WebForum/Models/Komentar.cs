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
        public List<Komentar> Podkomentari { get; set; }
        public string Tekst { get; set; }
        public int PozitivniGlasovi { get; set; }
        public int NegativniGlasovi { get; set; }
        public bool Izmenjen { get; set; }
        public bool Obrisan { get; set; }

        public Komentar(string id, string temaKojojPripada, string autor, DateTime datum, string roditeljskiKomentar, List<Komentar> podkomentari, string tekst, int pozitivni,int negativni, bool izmenjen, bool obrisan)
        {
            this.Id = id;
            this.TemaKojojPripada = temaKojojPripada;
            this.Autor = autor;
            this.DatumKomentara = datum;
            this.RoditeljskiKomentar = roditeljskiKomentar;
            this.Podkomentari = podkomentari;
            this.Tekst = tekst;
            this.PozitivniGlasovi = pozitivni;
            this.NegativniGlasovi = negativni;
            this.Izmenjen = izmenjen;
            this.Obrisan = obrisan;
        }

        public Komentar() { }
    }
}