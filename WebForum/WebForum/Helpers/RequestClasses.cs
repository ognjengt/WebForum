using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WebForum.Helpers
{
    // U ovaj fajl idu sve klase koje su potrebne za slanje requestova ka serveru
    public class KorisnikRequest
    {
        public string Username { get; set; }
        public string Password { get; set; }
        public string Ime { get; set; }
        public string Prezime { get; set; }
        public string Telefon { get; set; }
        public string Email { get; set; }

    }

    public class PodforumZaCuvanje
    {
        public string NazivPodforuma { get; set; }
        public string KorisnikKojiCuva { get; set; }
    }

    public class TemaZaCuvanje
    {
        public string NaslovTeme { get; set; }
        public string KorisnikKojiPrati { get; set; }
    }

    public class KomentarZaCuvanje
    {
        public string IdKomentara { get; set; }
        public string KoCuva { get; set; }
    }

    public class TemaLikeDislikeRequest
    {
        public string PunNazivTeme { get; set; }
        public string KoVrsiAkciju { get; set; }
    }
    public class KomentarLikeDislikeRequest
    {
        public string IdKomentara { get; set; }
        public string KoVrsiAkciju { get; set; }
    }
}