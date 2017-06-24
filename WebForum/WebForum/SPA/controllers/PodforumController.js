webForum.controller('PodforumController', function ($scope, PodforumiFactory, $routeParams) {

    $scope.nazivPodforuma = $routeParams.naziv;

    function init() {
        PodforumiFactory.getPodforumByNaziv($scope.nazivPodforuma).then(function (response) {
            console.log(response.data);
        });
    }

    init();

});