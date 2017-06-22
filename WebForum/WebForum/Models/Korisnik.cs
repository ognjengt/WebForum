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

        public Korisnik(string username, string password,string ime, string prezime,string uloga, string telefon, string email, DateTime datumRegistracije, List<string> praceniPodforumi, List<string> snimljeneTeme, List<string> snimljeniKomentari)
        {
            this.Username = username;
            this.Password = password;
            this.Ime = ime;
            this.Prezime = prezime;
            this.Uloga = uloga;
            this.Telefon = telefon;
            this.Email = email;
            this.DatumRegistracije = datumRegistracije;
            this.PraceniPodforumi = praceniPodforumi;
            this.SnimljeneTeme = snimljeneTeme;
            this.SnimljeniKomentari = snimljeniKomentari;
        }

    }
}