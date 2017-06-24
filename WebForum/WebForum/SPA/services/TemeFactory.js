webForum.factory('TemeFactory', function ($http) {

    var factory = {};
    
    factory.getTemeZaPodforum = function (podforum) {
        return $http.get('/api/Teme/GetTemeZaPodforum/?podforum=' + podforum);
    }

    return factory;

});