webForum.controller('TemaController', function ($scope, PodforumiFactory, TemeFactory, $routeParams) {

    $scope.nazivPodforuma = $routeParams.naziv;
    $scope.nazivTeme = $routeParams.tema;

    function init() {
        $scope.tema = {};

        TemeFactory.getTemaByNaziv($scope.nazivTeme, $scope.nazivPodforuma).then(function (response) {
            $scope.tema = response.data;
            $scope.tema.DatumKreiranja = new Date($scope.tema.DatumKreiranja).toLocaleDateString();
        });
    }

    init();

    

});