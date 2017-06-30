webForum.factory('KomentariFactory', function ($http) {

    var factory = {};

    factory.ostaviKomentarNaTemu = function (podforum, naslovTeme, tekstKomentara, autor) {
       return $http.post('/api/Komentari/DodajKomentarNaTemu', {
            TemaKojojPripada: podforum + '-' + naslovTeme,
            Autor: autor,
            Tekst: tekstKomentara
        })
    }

    factory.dodajPodkomentar = function (IdRoditelja, tekstPodkomentara, autor, temaKojojPripada) {
        return $http.post('/api/Komentari/DodajPodkomentar', {
            RoditeljskiKomentar: IdRoditelja,
            TemaKojojPripada: temaKojojPripada,
            Tekst: tekstPodkomentara,
            Autor: autor
        })
    }

    factory.getKomentariZaTemu = function (podforum, tema) {
        return $http.get('/api/Komentari/GetKomentariZaTemu/?idTeme=' + podforum + '-' + tema);
    }

    factory.ThumbsUp = function (komentar, username) {
        return $http.post('/api/Komentari/ThumbsUp', {
            IdKomentara: komentar.Id,
            KoVrsiAkciju: username
        });
    }

    factory.ThumbsDown = function (komentar, username) {
        return $http.post('/api/Komentari/ThumbsDown', {
            IdKomentara: komentar.Id,
            KoVrsiAkciju: username
        });
    }

    factory.obrisiPodkomentar = function (podkomentar) {
        return $http.post('/api/Komentari/ObrisiPodkomentar', {
            Id: podkomentar.Id
        });
    }

    factory.obrisiKomentar = function (komentar) {
        return $http.post('/api/Komentari/ObrisiKomentar', {
            Id: komentar.Id
        });
    }

    factory.izmeniKomentar = function (idKomentara, tekstKomentara, prikaziDaJeIzmenjeno) {
        return $http.post('/api/Komentari/IzmeniKomentar', {
            Id: idKomentara,
            Tekst: tekstKomentara,
            Izmenjen: prikaziDaJeIzmenjeno
        });
    }

    factory.izmeniPodkomentar = function (idKomentara, tekstKomentara, prikaziDaJeIzmenjeno) {
        return $http.post('/api/Komentari/IzmeniPodkomentar', {
            Id: idKomentara,
            Tekst: tekstKomentara,
            Izmenjen: prikaziDaJeIzmenjeno
        });
    }

    factory.getLajkovaniKomentari = function (username) {
        return $http.get('/api/Komentari/GetLajkovaniKomentari?username=' + username);
    }

    factory.getLajkovaniPodkomentari = function (username) {
        return $http.get('/api/Komentari/GetLajkovaniPodkomentari?username=' + username);
    }

    factory.getDislajkovaniKomentari = function (username) {
        return $http.get('/api/Komentari/GetDislajkovaniKomentari?username=' + username);
    }

    factory.getDislajkovaniPodkomentari = function (username) {
        return $http.get('/api/Komentari/GetDislajkovaniPodkomentari?username=' + username);
    }

    return factory;

});