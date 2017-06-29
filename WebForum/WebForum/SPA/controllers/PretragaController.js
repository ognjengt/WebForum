webForum.controller('PretragaController', function ($scope, $rootScope, SearchFactory) {

    function init() {
        $scope.searched = false;
        
        $scope.rezultatiPretragePodforumi = null;
        $scope.rezultatiPretrageTeme = null;
        $scope.rezultatiPretrageKorisnika = null;

        console.log('Pretraga aktivirana');
    }

    init();

    $scope.pretraziPodforume = function (pretragaPodforuma) {
        if (pretragaPodforuma.naziv == "") {
            pretragaPodforuma.naziv = null;
        }
        if (pretragaPodforuma.opis == "") {
            pretragaPodforuma.opis = null;
        }
        if (pretragaPodforuma.moderator == "") {
            pretragaPodforuma.moderator = null;
        }

        SearchFactory.pretraziPodforume(pretragaPodforuma).then(function (response) {
            console.log(response.data);
            $scope.rezultatiPretragePodforumi = response.data;
        });
    }

    $scope.pretraziTeme = function (pretragaTeme) {

        if (pretragaTeme.podforum == "") {
            pretragaTeme.podforum = null;
        }
        if (pretragaTeme.naslov == "") {
            pretragaTeme.naslov = null;
        }
        if (pretragaTeme.sadrzaj == "") {
            pretragaTeme.sadrzaj = null;
        }
        if (pretragaTeme.autor == "") {
            pretragaTeme.autor = null;
        }

        SearchFactory.pretraziTeme(pretragaTeme).then(function (response) {
            console.log(response.data);
            $scope.rezultatiPretrageTeme = response.data;
        });
    }

    $scope.pretraziKorisnike = function (username) {

        if (username == "") {
            username = null;
        }

        SearchFactory.pretraziKorisnike(username).then(function (response) {
            console.log(response.data);
            $scope.rezultatiPretrageKorisnika = response.data;
        });
    }

});