using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WebForum.Models
{
    public class Podforum
    {
        public string Naziv { get; set; }
        public string Opis { get; set; }
        public string Ikonica { get; set; }
        public string SpisakPravila { get; set; }
        public string OdgovorniModerator { get; set; }
        public List<string> Moderatori { get; set; }

        public Podforum(string naziv, string opis, string ikonica, string spisakPravila, string odgovorniMod, List<string> moderatori)
        {
            this.Naziv = naziv;
            this.Opis = opis;
            this.Ikonica = ikonica;
            this.SpisakPravila = spisakPravila;
            this.OdgovorniModerator = odgovorniMod;
            this.Moderatori = moderatori;
        }

        public Podforum() { }
    }
}