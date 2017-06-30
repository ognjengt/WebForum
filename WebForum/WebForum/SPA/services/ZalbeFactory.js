webForum.factory('ZalbeFactory', function ($http) {

    var factory = {};

    factory.getSveZalbe = function (username) {
        return $http.get('/api/Zalbe/GetSveZalbe?username=' + username);
    }

    factory.priloziZalbuNaPodforum = function (zalba) {
        return $http.post('/api/Zalbe/PriloziZalbuNaPodforum', {
            Tekst: zalba.tekst,
            Entitet: zalba.entitet,
            KorisnikKojiJeUlozio: zalba.korisnikKojiJeUlozio
        });
    }

    factory.priloziZalbuNaTemu = function (zalba) {
        return $http.post('/api/Zalbe/PriloziZalbuNaTemu', {
            Tekst: zalba.tekst,
            Entitet: zalba.entitet,
            KorisnikKojiJeUlozio: zalba.korisnikKojiJeUlozio
        })
    }


    factory.obrisiZalbu = function (zalba) {
        return $http.post('/api/Zalbe/ObrisiZalbu', {
            Id: zalba.Id
        });
    }

    

    return factory;

});