webForum.factory('PreporukeFactory', function ($http) {

    var factory = {};

    factory.getPreporukeZaKorisnika = function (username) {
        return $http.get('/api/Preporuke/GetPreporukeZaKorisnika?username=' + username);
    }

    return factory;

});