webForum.factory('SearchFactory', function ($http) {

    var factory = {};
    
    factory.pretraziPodforume = function (pretragaPodforuma) {
        return $http.post('/api/Search/PretraziPodforume', {
            Naziv: pretragaPodforuma.naziv,
            Opis: pretragaPodforuma.opis,
            OdgovorniModerator: pretragaPodforuma.moderator
        });
    }

    factory.pretraziTeme = function (pretragaTeme) {
        return $http.post('/api/Search/PretraziTeme', {
            Naslov: pretragaTeme.naslov,
            Sadrzaj: pretragaTeme.sadrzaj,
            Autor: pretragaTeme.autor,
            PodforumKomePripada: pretragaTeme.podforum
        });
    }

    factory.pretraziKorisnike = function (username) {
        return $http.post('/api/Search/PretraziKorisnike', {
            Username: username
        });
    }

    return factory;

});