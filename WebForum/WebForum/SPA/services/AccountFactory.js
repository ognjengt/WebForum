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

    factory.getSacuvaneTeme = function (username) {
        return $http.get('/api/Account/GetSnimljeneTeme?username=' + username);
    }

    return factory;

});