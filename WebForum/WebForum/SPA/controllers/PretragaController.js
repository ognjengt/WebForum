webForum.controller('PretragaController', function ($scope, $rootScope, SearchFactory) {

    function init() {
        $scope.searched = false;
        console.log('Pretraga aktivirana');
    }

    init();

    $scope.pretraziPodforume = function (pretragaPodforuma) {
        SearchFactory.pretraziPodforume(pretragaPodforuma).then(function (response) {
            console.log(response.data);
        });
    }

    $scope.pretraziTeme = function (pretragaTeme) {
        SearchFactory.pretraziTeme(pretragaTeme).then(function (response) {
            console.log(response.data);
        });
    }

    $scope.pretraziKorisnike = function (username) {
        SearchFactory.pretraziKorisnike(username).then(function (response) {
            console.log(response.data);
        });
    }

});