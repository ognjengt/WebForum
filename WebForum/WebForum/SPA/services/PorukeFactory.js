webForum.factory('PorukeFactory', function ($http) {

    var factory = {};

    factory.posaljiPoruku = function (poruka) {
        return $http.post('/api/Poruke/PosaljiPoruku', {
            Posiljalac: poruka.posiljalac,
            Primalac: poruka.primalac,
            Sadrzaj: poruka.sadrzaj,
            Procitana: false
        });
    }
    return factory;

});