webForum.controller('PodforumController', function ($scope, PodforumiFactory, $routeParams) {

    $scope.nazivPodforuma = $routeParams.naziv;

    function init() {
        PodforumiFactory.getPodforumByNaziv($scope.nazivPodforuma).then(function (response) {
            $scope.podforum = response.data;
            if ($scope.podforum.Ikonica == null || $scope.podforum.Ikonica == "" || $scope.podforum.Ikonica.includes('.gif')) {
                $scope.podforum.Ikonica = "noimage.png";
            } else {
                var spliter = $scope.podforum.Ikonica.split('.');
                $scope.podforum.Ikonica = $scope.podforum.Ikonica + "." + spliter[1];
            }

            
        });
    }

    init();

});