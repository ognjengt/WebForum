webForum.factory('ZalbeFactory', function ($http) {

    var factory = {};

    factory.priloziZalbuNaPodforum = function (zalba) {
        return $http.post('/api/Zalbe/PriloziZalbuNaPodforum', {
            Tekst: zalba.tekst,
            Entitet: zalba.entitet,
            KorisnikKojiJeUlozio: zalba.korisnikKojiJeUlozio
        });
    }

    return factory;

});