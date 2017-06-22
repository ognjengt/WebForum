using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WebForum.Models
{
    public class Korisnik
    {
        public string Username { get; set; }
        public string Password { get; set; }
        public string Ime { get; set; }
        public string Prezime { get; set; }
        public string Uloga { get; set; }
        public string Telefon { get; set; }
        public string Email { get; set; }
        public DateTime DatumRegistracije { get; set; }
        public List<string> PraceniPodforumi { get; set; }
        public List<string> SnimljeneTeme { get; set; }
        public List<string> SnimljeniKomentari { get; set; }
    }
}