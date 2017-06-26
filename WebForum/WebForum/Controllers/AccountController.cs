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
        DbOperater dbOperaterPodforumi = new DbOperater();
        DbOperater dbOperaterTeme = new DbOperater();
        DbOperater dbOperaterKomentari = new DbOperater();

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
            Korisnik kor = new Korisnik(k.Username, k.Password, k.Ime, k.Prezime, "Korisnik", k.Telefon, k.Email, DateTime.Now, new List<Podforum>(), new List<Tema>(), new List<Komentar>());
            sw.WriteLine(kor.Username+";"+kor.Password+";"+kor.Ime+";"+kor.Prezime+";"+kor.Uloga+";"+kor.Email+";"+kor.Telefon+";"+kor.DatumRegistracije.ToShortDateString()+";"+"nemaSnimljenihPodforuma"+";nemaSnimljenihTema"+";nemaSnimljenihKomentara");
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
            StreamReader srPodforumi = dbOperaterPodforumi.getReader("podforumi.txt");
            StreamReader srTeme = dbOperaterTeme.getReader("teme.txt");
            StreamReader srKomentari = dbOperaterKomentari.getReader("komentari.txt");

            // Todo promeni ovo u List<Podforum> , List<Tema> i List<Komentari>
            List<Podforum> listaSacuvanihPodforuma = new List<Podforum>();
            List<Tema> listaSacuvanihTema = new List<Tema>();
            List<Komentar> listaSacuvanihKomentara = new List<Komentar>();
            string line = "";
            while ((line = sr.ReadLine()) != null)
            {
                string[] splitter = line.Split(';');
                if (splitter[0] == k.Username && splitter[1] == k.Password)
                {

                    string[] podforumSplitter = splitter[8].Split('|');
                    foreach (string podforumId in podforumSplitter)
                    {
                        if (podforumId != "nemaSnimljenihPodforuma")
                        {
                            // Prodji kroz sve podforume, nadji taj sa tim imenom, napravi novi Podforum i dodaj ga u listu
                            
                            string podforumLine = "";
                            while ( (podforumLine = srPodforumi.ReadLine()) != null)
                            {
                                string[] podforumLineSplitter = podforumLine.Split(';');
                                if (podforumLineSplitter[0]== podforumId)
                                {
                                    List<string> odgovorniModeratori = new List<string>();
                                    string[] moderatoriSplitter = podforumLineSplitter[5].Split('|');
                                    foreach (string moderator in moderatoriSplitter)
                                    {
                                        odgovorniModeratori.Add(moderator);
                                    }
                                    listaSacuvanihPodforuma.Add(new Podforum(podforumLineSplitter[0],podforumLineSplitter[1],podforumLineSplitter[2],podforumLineSplitter[3],podforumLineSplitter[4], odgovorniModeratori));
                                }
                            }
                        }
                    }
                    srPodforumi.Close();
                    dbOperaterPodforumi.Reader.Close();

                    string[] temeSplitter = splitter[9].Split('|');
                    foreach (string temaId in temeSplitter)
                    {
                        if (temaId != "nemaSnimljenihTema")
                        {
                            // Prodji kroz sve teme, nadji tu sa tim imenom, napravi novu temu, od podataka i dodaj je u listu
                            
                            string temaLine = "";
                            while ((temaLine = srTeme.ReadLine()) != null)
                            {
                                string[] temaLineSplitter = temaLine.Split(';');
                                string[] podforumTema = temaId.Split('-');

                                if (temaLineSplitter[0] == podforumTema[0] && temaLineSplitter[1] == podforumTema[1])
                                {
                                    // NOTE: kada dodajem temu u listu pracenih tema , stavim da nema ni jedan komentar, posto mi ne trebaju komentari kada budem ispisivao samo teme
                                    // kada se klikne na tu temu on ga vodi i sve fino
                                    listaSacuvanihTema.Add(new Tema(temaLineSplitter[0],temaLineSplitter[1],temaLineSplitter[2],temaLineSplitter[3],temaLineSplitter[4],DateTime.Parse(temaLineSplitter[5]), Int32.Parse(temaLineSplitter[6]), Int32.Parse(temaLineSplitter[7]), new List<string>() ));
                                }
                            }
                        }
                    }
                    srTeme.Close();
                    dbOperaterTeme.Reader.Close();

                    string[] komentariSplitter = splitter[10].Split('|');
                    foreach (string komentarId in komentariSplitter)
                    {
                        if (komentarId != "nemaSnimljenihKomentara")
                        {
                            // Prodji kroz sve komentare, nadji taj sa tim id-em, napravi novi komentar, od podataka i dodaj ga u listu
                            string komentarLine = "";
                            while ((komentarLine = srKomentari.ReadLine()) != null)
                            {
                                string[] komentarLineSplitter = komentarLine.Split(';');
                                if (komentarLineSplitter[0] == komentarId)
                                {
                                    // NOTE: kada dodajem ovde komentar, necu dodavati njegove podkomentare, posto to nije bitno za tu stranicu, korisnik treba samo da ima uvid u jedan komentar
                                    listaSacuvanihKomentara.Add( new Komentar(komentarLineSplitter[0],komentarLineSplitter[1],komentarLineSplitter[2], DateTime.Parse(komentarLineSplitter[3]),komentarLineSplitter[4],new List<string>(),komentarLineSplitter[5],Int32.Parse(komentarLineSplitter[6]), Int32.Parse(komentarLineSplitter[7]), bool.Parse(komentarLineSplitter[8])));
                                }
                            }
                        }
                    }
                    srKomentari.Close();
                    dbOperaterKomentari.Reader.Close();

                    Korisnik kor = new Korisnik(splitter[0],splitter[1],splitter[2],splitter[3],splitter[4],splitter[5],splitter[6],DateTime.Parse(splitter[7]),listaSacuvanihPodforuma,listaSacuvanihTema,listaSacuvanihKomentara);
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
