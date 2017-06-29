webForum.factory('AccountFactory', function ($http) {

    var factory = {};
    factory.RegistrujKorisnika = function (korisnik) {
        return $http.post('/api/Account/Register', {
            Username: korisnik.username,
            Password: korisnik.pwd,
            Ime: korisnik.ime,
            Prezime: korisnik.prezime,
            Telefon: korisnik.kontaktTelefon,
            Email: korisnik.email
        });
    }

    factory.Login = function (korisnik) {
        return $http.post('/api/Account/Login', {
            Username: korisnik.username,
            Password: korisnik.password
        });
    }

    factory.sacuvajPodforum = function (nazivPodforuma, username) {
        return $http.post('/api/Account/SacuvajPodforum', {
            NazivPodforuma: nazivPodforuma,
            KorisnikKojiCuva: username
        });
    }

    factory.zapratiTemu = function (naslovTeme, username) {
        return $http.post('/api/Account/ZapratiTemu', {
            NaslovTeme: naslovTeme,
            KorisnikKojiPrati: username
        })
    }

    factory.sacuvajKomentar = function (idKomentara, username) {
        return $http.post('/api/Account/SacuvajKomentar', {
            IdKomentara: idKomentara,
            KoCuva: username
        });
    }

    factory.getUserByUsername = function (username) {
        return $http.get('/api/Account/GetUserByUsername?username=' + username);
    }

    factory.getSacuvaniPodforumi = function (username) {
        return $http.get('/api/Account/GetSacuvaniPodforumi?username=' + username);
    }

    factory.getSacuvaneTeme = function (username) {
        return $http.get('/api/Account/GetSnimljeneTeme?username=' + username);
    }

    factory.getSacuvaniKomentari = function (username) {
        return $http.get('/api/Account/GetSacuvaniKomentari?username=' + username);
    }

    factory.getSacuvaniPodkomentari = function (username) {
        return $http.get('/api/Account/GetSacuvaniPodkomentari?username=' + username);
    }

    factory.uzmiSveKorisnikeOsimMene = function (username) {
        return $http.get('/api/Account/UzmiSveKorisnikeOsimMene?username=' + username);
    }

    factory.promeniTipKorisniku = function (username, tip) {
        return $http.post('/api/Account/PromeniTipKorisniku', {
            Username: username,
            Uloga: tip
        });
    }

    return factory;

});