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

    factory.uploadImage = function (fd, imeSlike) {
        return $http.post('/api/Teme/UploadImage', fd,
        {
            transformRequest: angular.identity,
            headers: { 'Content-Type': undefined, slika: imeSlike }
        });
    }

    factory.getTemaByNaziv = function (nazivTeme, nazivPodforuma) {
        return $http.get('/api/Teme/GetTemaByNaziv/?podforum=' + nazivPodforuma + '&tema=' + nazivTeme);
    }

    factory.ThumbsUp = function (tema, username) {
        return $http.post('/api/Teme/ThumbsUp', {
            PunNazivTeme: tema.PodforumKomePripada + '-' + tema.Naslov,
            KoVrsiAkciju: username
        })
    }

    factory.ThumbsDown = function (tema, username) {
        return $http.post('/api/Teme/ThumbsDown', {
            PunNazivTeme: tema.PodforumKomePripada + '-' + tema.Naslov,
            KoVrsiAkciju: username
        })
    }

    factory.getLajkovaneTeme = function (username) {
        return $http.get('/api/Teme/GetLajkovaneTeme?username=' + username);
    }

    factory.getDislajkovaneTeme = function (username) {
        return $http.get('/api/Teme/GetDislajkovaneTeme?username=' + username);
    }

    return factory;

});