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

    return factory;

});