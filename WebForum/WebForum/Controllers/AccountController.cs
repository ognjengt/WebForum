using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using WebForum.Helpers;
using WebForum.Models;

namespace WebForum.Controllers
{
    public class AccountController : ApiController
    {
        DbOperater dbOperater = new DbOperater();

        /// <summary>
        /// Registruje korisnika u korisnici.txt
        /// </summary>
        /// <param name="k"></param>
        /// <returns></returns>
        [HttpPost]
        [ActionName("Register")]
        public bool Register([FromBody]KorisnikRequest k)
        {
            StreamReader sr = dbOperater.getReader("korisnici.txt");
            string line = "";
            while ( (line = sr.ReadLine()) != null )
            {
                string[] splitter = line.Split(';');
                if (splitter[0] == k.Username)
                {
                    sr.Close();
                    dbOperater.Reader.Close();
                    return false;
                }
            }
            sr.Close();
            dbOperater.Reader.Close();
            StreamWriter sw = dbOperater.getWriter("korisnici.txt");
            Korisnik kor = new Korisnik(k.Username, k.Password, k.Ime, k.Prezime, "Korisnik", k.Telefon, k.Email, DateTime.Now, new List<string>(), new List<string>(), new List<string>());
            sw.WriteLine(kor.Username+";"+kor.Password+";"+kor.Ime+";"+kor.Prezime+";"+kor.Uloga+";"+kor.Email+";"+kor.Telefon+";"+kor.DatumRegistracije.ToShortDateString());
            sw.Close();
            dbOperater.Writer.Close();
            
            return true;
        }

        /// <summary>
        /// Proverava da li su username i sifra odgovarajuci
        /// </summary>
        /// <param name="k"></param>
        /// <returns></returns>
        [HttpPost]
        [ActionName("Login")]
        public Korisnik Login([FromBody]Korisnik k)
        {
            StreamReader sr = dbOperater.getReader("korisnici.txt");
            string line = "";
            while ((line = sr.ReadLine()) != null)
            {
                string[] splitter = line.Split(';');
                if (splitter[0] == k.Username && splitter[1] == k.Password)
                {
                    Korisnik kor = new Korisnik(splitter[0],splitter[1],splitter[2],splitter[3],splitter[4],splitter[5],splitter[6],DateTime.Parse(splitter[7]),new List<string>(),new List<string>(),new List<string>());
                    kor.Password = null;
                    sr.Close();
                    dbOperater.Reader.Close();
                    return kor;
                }
            }
            sr.Close();
            dbOperater.Reader.Close();
            return null;
        }
        
    }
}
