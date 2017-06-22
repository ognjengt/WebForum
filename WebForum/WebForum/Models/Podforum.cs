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
    }
}