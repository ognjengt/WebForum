webForum.factory('AccountFactory', function ($http) {

    var factory = {};
    factory.RegistrujKorisnika = function(korisnik) {
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

    return factory;

});