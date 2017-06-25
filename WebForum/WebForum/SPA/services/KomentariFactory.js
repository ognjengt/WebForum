webForum.factory('KomentariFactory', function ($http) {

    var factory = {};

    factory.ostaviKomentarNaTemu = function (podforum, naslovTeme, tekstKomentara, autor) {
       return $http.post('/api/Komentari/DodajKomentarNaTemu', {
            TemaKojojPripada: podforum + '-' + naslovTeme,
            Autor: autor,
            Tekst: tekstKomentara
        })
    }

    return factory;

});