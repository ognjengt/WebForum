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
            
            
            string line = "";
            while ((line = sr.ReadLine()) != null)
            {
                string[] splitter = line.Split(';');
                if (splitter[0] == k.Username && splitter[1] == k.Password)
                {
                    Korisnik kor = new Korisnik(splitter[0],splitter[1],splitter[2],splitter[3],splitter[4],splitter[5],splitter[6],DateTime.Parse(splitter[7]),new List<Podforum>(), new List<Tema>(), new List<Komentar>());
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

        [ActionName("GetUserByUsername")]
        public Korisnik GetUserByUsername(string username)
        {
            StreamReader sr = dbOperater.getReader("korisnici.txt");

            string line = "";
            while ((line = sr.ReadLine()) != null)
            {
                string[] splitter = line.Split(';');
                if (splitter[0] == username)
                {
                    Korisnik kor = new Korisnik();
                    kor.Username = splitter[0];
                    kor.Ime = splitter[2];
                    kor.Prezime = splitter[3];
                    kor.Uloga = splitter[4];
                    kor.Email = splitter[5];
                    kor.Telefon = splitter[6];
                    kor.DatumRegistracije = DateTime.Parse(splitter[7]);
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

        [ActionName("GetSacuvaniPodforumi")]
        public List<Podforum> GetSacuvaniPodforumi(string username)
        {
            StreamReader sr = dbOperater.getReader("korisnici.txt");
            List<Podforum> listaSacuvanihPodforuma = new List<Podforum>();

            string line = "";
            while ( (line = sr.ReadLine()) !=null )
            {
                string[] splitter = line.Split(';');
                if (splitter[0] == username)
                {
                    string[] podforumSplitter = splitter[8].Split('|');
                    foreach (string podforumId in podforumSplitter)
                    {
                        if (podforumId != "nemaSnimljenihPodforuma")
                        {
                            // Prodji kroz sve podforume, nadji taj sa tim imenom, napravi novi Podforum i dodaj ga u listu
                            StreamReader srPodforumi = dbOperaterPodforumi.getReader("podforumi.txt");
                            string podforumLine = "";
                            while ((podforumLine = srPodforumi.ReadLine()) != null)
                            {
                                string[] podforumLineSplitter = podforumLine.Split(';');
                                if (podforumLineSplitter[0] == podforumId)
                                {
                                    List<string> odgovorniModeratori = new List<string>();
                                    string[] moderatoriSplitter = podforumLineSplitter[5].Split('|');
                                    foreach (string moderator in moderatoriSplitter)
                                    {
                                        odgovorniModeratori.Add(moderator);
                                    }
                                    listaSacuvanihPodforuma.Add(new Podforum(podforumLineSplitter[0], podforumLineSplitter[1], podforumLineSplitter[2], podforumLineSplitter[3], podforumLineSplitter[4], odgovorniModeratori));
                                    break;
                                }
                            }
                            srPodforumi.Close();
                            dbOperaterPodforumi.Reader.Close();
                        }
                    }
                }
            }
            sr.Close();
            dbOperater.Reader.Close();
            return listaSacuvanihPodforuma;
        }

        [ActionName("GetSnimljeneTeme")]
        public List<Tema> GetSnimljeneTeme(string username)
        {
            StreamReader sr = dbOperater.getReader("korisnici.txt");
            List<Tema> listaSacuvanihTema = new List<Tema>();

            string line = "";
            while ((line = sr.ReadLine()) != null)
            {
                string[] splitter = line.Split(';');
                if (splitter[0] == username)
                {
                    string[] temeSplitter = splitter[9].Split('|');
                    foreach (string temaId in temeSplitter)
                    {
                        if (temaId != "nemaSnimljenihTema")
                        {
                            // Prodji kroz sve teme, nadji tu sa tim imenom, napravi novu temu, od podataka i dodaj je u listu
                            StreamReader srTeme = dbOperaterTeme.getReader("teme.txt");

                            string temaLine = "";
                            while ((temaLine = srTeme.ReadLine()) != null)
                            {
                                string[] temaLineSplitter = temaLine.Split(';');
                                string[] podforumTema = temaId.Split('-');

                                if (temaLineSplitter[0] == podforumTema[0] && temaLineSplitter[1] == podforumTema[1])
                                {
                                    // NOTE: kada dodajem temu u listu pracenih tema , stavim da nema ni jedan komentar, posto mi ne trebaju komentari kada budem ispisivao samo teme
                                    // kada se klikne na tu temu on ga vodi i sve fino
                                    listaSacuvanihTema.Add(new Tema(temaLineSplitter[0], temaLineSplitter[1], temaLineSplitter[2], temaLineSplitter[3], temaLineSplitter[4], DateTime.Parse(temaLineSplitter[5]), Int32.Parse(temaLineSplitter[6]), Int32.Parse(temaLineSplitter[7]), new List<string>()));
                                    break; // tema nadjena pici sledeci foreach
                                } 
                            }
                            srTeme.Close();
                            dbOperaterTeme.Reader.Close();
                        }
                    }
                }
            }
            sr.Close();
            dbOperater.Reader.Close();
            return listaSacuvanihTema;
        }

        [ActionName("GetSacuvaniKomentari")]
        public List<Komentar> GetSacuvaniKomentari(string username)
        {
            StreamReader sr = dbOperater.getReader("korisnici.txt");
            List<Komentar> listaSacuvanihKomentara = new List<Komentar>();

            string line = "";
            while ((line = sr.ReadLine()) != null)
            {
                string[] splitter = line.Split(';');
                if (splitter[0] == username)
                {
                    string[] komentariSplitter = splitter[10].Split('|');
                    foreach (string komentarId in komentariSplitter)
                    {
                        if (komentarId != "nemaSnimljenihKomentara")
                        {
                            // Prodji kroz sve komentare, nadji taj sa tim id-em, napravi novi komentar, od podataka i dodaj ga u listu
                            StreamReader srKomentari = dbOperaterKomentari.getReader("komentari.txt");

                            string komentarLine = "";
                            while ((komentarLine = srKomentari.ReadLine()) != null)
                            {
                                string[] komentarLineSplitter = komentarLine.Split(';');
                                if (komentarLineSplitter[0] == komentarId && komentarLineSplitter[9] == "False")
                                {
                                    // NOTE: kada dodajem ovde komentar, necu dodavati njegove podkomentare, posto to nije bitno za tu stranicu, korisnik treba samo da ima uvid u jedan komentar
                                    listaSacuvanihKomentara.Add(new Komentar(komentarLineSplitter[0], komentarLineSplitter[1], komentarLineSplitter[2], DateTime.Parse(komentarLineSplitter[3]), komentarLineSplitter[4], new List<Komentar>(), komentarLineSplitter[5], Int32.Parse(komentarLineSplitter[6]), Int32.Parse(komentarLineSplitter[7]), bool.Parse(komentarLineSplitter[8]), bool.Parse(komentarLineSplitter[9])));
                                    break;
                                }
                            }
                            srKomentari.Close();
                            dbOperaterKomentari.Reader.Close();
                        }
                    }
                }
            }
            sr.Close();
            dbOperater.Reader.Close();
            return listaSacuvanihKomentara;
        }

        [ActionName("GetSacuvaniPodkomentari")]
        public List<Komentar> GetSacuvaniPodkomentari(string username)
        {
            StreamReader sr = dbOperater.getReader("korisnici.txt");
            List<Komentar> listaSacuvanihKomentara = new List<Komentar>();

            string line = "";
            while ((line = sr.ReadLine()) != null)
            {
                string[] splitter = line.Split(';');
                if (splitter[0] == username)
                {
                    string[] komentariSplitter = splitter[10].Split('|');
                    foreach (string komentarId in komentariSplitter)
                    {
                        if (komentarId != "nemaSnimljenihKomentara")
                        {
                            // Prodji kroz sve podkomentare, nadji taj sa tim id-em, napravi novi komentar, od podataka i dodaj ga u listu
                            StreamReader srKomentari = dbOperaterKomentari.getReader("podkomentari.txt");

                            string komentarLine = "";
                            while ((komentarLine = srKomentari.ReadLine()) != null)
                            {
                                string[] komentarLineSplitter = komentarLine.Split(';');
                                if (komentarLineSplitter[1] == komentarId && komentarLineSplitter[8] == "False")
                                {
                                    // evenutalno: prodji kroz sve teme i pogledaj da li komentarLineSplitter[9] odgovara nekoj
                                    // ako ne odgovara nijednoj, to znaci da ta tema ne postoji tj da je obrisana i nemoj dodati ovaj
                                    // podkomentar u listu

                                    Komentar podkomentar = new Komentar();
                                    podkomentar.RoditeljskiKomentar = komentarLineSplitter[0];
                                    podkomentar.Id = komentarLineSplitter[1];
                                    podkomentar.Autor = komentarLineSplitter[2];
                                    podkomentar.DatumKomentara = DateTime.Parse(komentarLineSplitter[3]);
                                    podkomentar.Tekst = komentarLineSplitter[4];
                                    podkomentar.PozitivniGlasovi = Int32.Parse(komentarLineSplitter[5]);
                                    podkomentar.NegativniGlasovi = Int32.Parse(komentarLineSplitter[6]);
                                    podkomentar.Izmenjen = bool.Parse(komentarLineSplitter[7]);
                                    podkomentar.Obrisan = bool.Parse(komentarLineSplitter[8]);
                                    podkomentar.TemaKojojPripada = komentarLineSplitter[9];
                                    listaSacuvanihKomentara.Add(podkomentar);
                                    break;
                                }
                            }
                            srKomentari.Close();
                            dbOperaterKomentari.Reader.Close();
                        }
                    }
                }
            }
            sr.Close();
            dbOperater.Reader.Close();
            return listaSacuvanihKomentara;
        }

        [HttpPost]
        [ActionName("SacuvajPodforum")]
        public bool SacuvajPodforum([FromBody]PodforumZaCuvanje pfZaCuvanje)
        {
            StreamReader sr = dbOperater.getReader("korisnici.txt");

            List<string> listaSvihKorisnika = new List<string>();
            int brojac = 0;
            int indexZaIzmenu = -1;
            string line = "";
            while ((line = sr.ReadLine()) != null)
            {
                listaSvihKorisnika.Add(line);
                brojac++;

                string[] splitter = line.Split(';');
                if (splitter[0] == pfZaCuvanje.KorisnikKojiCuva)
                {
                    indexZaIzmenu = brojac;
                }
            }
            sr.Close();
            dbOperater.Reader.Close();
            
            // splituj tu liniju koja treba da se menja tj na koju treba da se dodaje
            string[] tokeniOdabranogKorisnika = listaSvihKorisnika[indexZaIzmenu - 1].Split(';');
            // tokeniOdabranogKorisnika[8] tu se nalazi spisak pracenih podforuma
            string[] splitterProvere = tokeniOdabranogKorisnika[8].Split('|');
            // provera ukoliko korisnik vec prati postojeci podforum
            foreach (string praceniPodforum in splitterProvere)
            {
                if (praceniPodforum == pfZaCuvanje.NazivPodforuma)
                {
                    return false;
                }
            }
            // otvori bulk writer
            StreamWriter sw = dbOperater.getBulkWriter("korisnici.txt");

            // tokeniOdabranogKorisnika[8] tu se nalazi spisak pracenih podforuma
            tokeniOdabranogKorisnika[8] += "|" + pfZaCuvanje.NazivPodforuma;

            // linijaZaUpis se inicijalizuje na pocetku da je korisnicki username
            string linijaZaUpis = tokeniOdabranogKorisnika[0];
            // prodji kroz sve tokene odabranog korisnika i upisi ih u liniju, da ne pisem tokeni[0]+';'+tokeni[1] ...
            for (int i = 1; i < 11; i++)
            {
                linijaZaUpis += ";" + tokeniOdabranogKorisnika[i];
            }

            // ubaci tu izmenjenu liniju na to mesto u listiSvih
            listaSvihKorisnika[indexZaIzmenu - 1] = linijaZaUpis;
            // prepisi ceo fajl
            foreach (string korisnickaLinija in listaSvihKorisnika)
            {
                sw.WriteLine(korisnickaLinija);
            }
            sw.Close();
            dbOperater.Writer.Close();

            return true;
        }

        [HttpPost]
        [ActionName("ZapratiTemu")]
        public bool ZapratiTemu([FromBody]TemaZaCuvanje temaZaCuvanje)
        {
            StreamReader sr = dbOperater.getReader("korisnici.txt");

            List<string> listaSvihKorisnika = new List<string>();
            int brojac = 0;
            int indexZaIzmenu = -1;
            string line = "";
            while ((line = sr.ReadLine()) != null)
            {
                listaSvihKorisnika.Add(line);
                brojac++;

                string[] splitter = line.Split(';');
                if (splitter[0] == temaZaCuvanje.KorisnikKojiPrati)
                {
                    indexZaIzmenu = brojac;
                }
            }
            sr.Close();
            dbOperater.Reader.Close();

            // splituj tu liniju koja treba da se menja tj na koju treba da se dodaje
            string[] tokeniOdabranogKorisnika = listaSvihKorisnika[indexZaIzmenu - 1].Split(';');
            // tokeniOdabranogKorisnika[9] tu se nalazi spisak pracenih tema
            string[] splitterProvere = tokeniOdabranogKorisnika[9].Split('|');
            // provera ukoliko korisnik vec prati postojeci podforum
            foreach (string pracenaTema in splitterProvere)
            {
                if (pracenaTema == temaZaCuvanje.NaslovTeme)
                {
                    return false;
                }
            }
            // otvori bulk writer
            StreamWriter sw = dbOperater.getBulkWriter("korisnici.txt");

            // tokeniOdabranogKorisnika[9] tu se nalazi spisak pracenih tema
            tokeniOdabranogKorisnika[9] += "|" + temaZaCuvanje.NaslovTeme;

            // linijaZaUpis se inicijalizuje na pocetku da je korisnicki username
            string linijaZaUpis = tokeniOdabranogKorisnika[0];
            // prodji kroz sve tokene odabranog korisnika i upisi ih u liniju, da ne pisem tokeni[0]+';'+tokeni[1] ...
            for (int i = 1; i < 11; i++)
            {
                linijaZaUpis += ";" + tokeniOdabranogKorisnika[i];
            }

            // ubaci tu izmenjenu liniju na to mesto u listiSvih
            listaSvihKorisnika[indexZaIzmenu - 1] = linijaZaUpis;
            // prepisi ceo fajl
            foreach (string korisnickaLinija in listaSvihKorisnika)
            {
                sw.WriteLine(korisnickaLinija);
            }
            sw.Close();
            dbOperater.Writer.Close();

            return true;
        }

        [HttpPost]
        [ActionName("SacuvajKomentar")]
        public bool SacuvajKomentar([FromBody]KomentarZaCuvanje komentarZaCuvanje)
        {
            StreamReader sr = dbOperater.getReader("korisnici.txt");

            List<string> listaSvihKorisnika = new List<string>();
            int brojac = 0;
            int indexZaIzmenu = -1;
            string line = "";
            while ((line = sr.ReadLine()) != null)
            {
                listaSvihKorisnika.Add(line);
                brojac++;

                string[] splitter = line.Split(';');
                if (splitter[0] == komentarZaCuvanje.KoCuva)
                {
                    indexZaIzmenu = brojac;
                }
            }
            sr.Close();
            dbOperater.Reader.Close();

            // splituj tu liniju koja treba da se menja tj na koju treba da se dodaje
            string[] tokeniOdabranogKorisnika = listaSvihKorisnika[indexZaIzmenu - 1].Split(';');
            // tokeniOdabranogKorisnika[10] tu se nalazi spisak pracenih komentara
            string[] splitterProvere = tokeniOdabranogKorisnika[10].Split('|');
            // provera ukoliko korisnik vec prati postojeci podforum
            foreach (string idKomentara in splitterProvere)
            {
                if (idKomentara == komentarZaCuvanje.IdKomentara)
                {
                    return false;
                }
            }
            // otvori bulk writer
            StreamWriter sw = dbOperater.getBulkWriter("korisnici.txt");

            // tokeniOdabranogKorisnika[10] tu se nalazi spisak pracenih komentara
            tokeniOdabranogKorisnika[10] += "|" + komentarZaCuvanje.IdKomentara;

            // linijaZaUpis se inicijalizuje na pocetku da je korisnicki username
            string linijaZaUpis = tokeniOdabranogKorisnika[0];
            // prodji kroz sve tokene odabranog korisnika i upisi ih u liniju, da ne pisem tokeni[0]+';'+tokeni[1] ...
            for (int i = 1; i < 11; i++)
            {
                linijaZaUpis += ";" + tokeniOdabranogKorisnika[i];
            }

            // ubaci tu izmenjenu liniju na to mesto u listiSvih
            listaSvihKorisnika[indexZaIzmenu - 1] = linijaZaUpis;
            // prepisi ceo fajl
            foreach (string korisnickaLinija in listaSvihKorisnika)
            {
                sw.WriteLine(korisnickaLinija);
            }
            sw.Close();
            dbOperater.Writer.Close();

            return true;
        }
    }
}
