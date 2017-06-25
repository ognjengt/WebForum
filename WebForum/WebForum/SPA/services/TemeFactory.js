webForum.factory('TemeFactory', function ($http) {

    var factory = {};
    
    factory.getTemeZaPodforum = function (podforum) {
        return $http.get('/api/Teme/GetTemeZaPodforum/?podforum=' + podforum);
    }

    factory.dodajTemu = function (tema) {
        return $http.post('/api/Teme/DodajTemu', {
            PodforumKomePripada: tema.podforumKomePripada,
            Naslov: tema.naslov,
            Tip: tema.tip,
            Sadrzaj: tema.sadrzaj,
            Autor: tema.autor
        });
    }

    factory.dodajTemuSaSlikom = function (tema) {

    }

    factory.getTemaByNaziv = function (nazivTeme, nazivPodforuma) {
        return $http.get('/api/Teme/GetTemaByNaziv/?podforum=' + nazivPodforuma + '&tema=' + nazivTeme);
    }

    return factory;

});