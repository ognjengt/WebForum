webForum.factory('PodforumiFactory', function ($http) {

    var factory = {};
    
    factory.getAllPodforums = function () {
        return $http.get('/api/Podforumi/GetAll');
    }

    factory.getPodforumByNaziv = function (naziv) {
        return $http.get('/api/Podforumi/GetPodforumByNaziv?naziv=' + naziv);
    }

    factory.dodajPodforum = function (podforum) {
        return $http.post('/api/Podforumi/DodajPodforum', {
            Naziv: podforum.naziv,
            Opis: podforum.opis,
            Ikonica: podforum.ikonica,
            SpisakPravila: podforum.spisakPravila,
            OdgovorniModerator: podforum.moderator
        });
    }

    // ova metoda prima fileData i imeSlike koji salje ka serveru, takodje salje i heder slika, server uzima iz hedera naziv slike i pravi sliku sa tim nazivom
    factory.uploadImage = function (fd,imeSlike) {
        return $http.post('/api/Podforumi/UploadImage',fd,
        {
            transformRequest: angular.identity,
            headers: {'Content-Type': undefined, slika: imeSlike}
        });
    }

    return factory;

});