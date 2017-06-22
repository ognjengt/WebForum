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
}