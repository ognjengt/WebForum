﻿using System;
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
                    dbOperater.Reader.Close();
                    sr.Close();
                    return false;
                }
            }
            dbOperater.Reader.Close();
            sr.Close();
            StreamWriter sw = dbOperater.getWriter("korisnici.txt");
            Korisnik kor = new Korisnik(k.Username, k.Password, k.Ime, k.Prezime, "Korisnik", k.Telefon, k.Email, DateTime.Now, new List<string>(), new List<string>(), new List<string>());
            sw.WriteLine(kor.Username+";"+kor.Password+";"+kor.Ime+";"+kor.Prezime+";"+kor.Uloga+";"+kor.Email+";"+kor.Telefon+";"+kor.DatumRegistracije.ToShortDateString());
            sw.Close();
            dbOperater.Writer.Close();
            
            return true;
        }

        [HttpPost]
        [ActionName("Login")]
        public Korisnik Login([FromBody]Korisnik k)
        {
            StreamReader sr = dbOperater.getReader("korisnici.txt");
            string line = "";
            while ((line = sr.ReadLine()) != null)
            {
                string[] splitter = line.Split(';');
                if (splitter[0] == k.Username)
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