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

    factory.getAllUserMessages = function (username) {
        return $http.get('/api/Poruke/GetAllUserMessages?username=' + username);
    }

    factory.markirajKaoProcitano = function (poruka) {
        return $http.post('/api/Poruke/MarkirajKaoProcitano?id=' + poruka.Id);
    }

    return factory;

});