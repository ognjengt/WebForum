﻿webForum.controller('PodforumController', function ($scope, PodforumiFactory, $routeParams) {

    $scope.nazivPodforuma = $routeParams.naziv;
    $scope.dodavanjePopupVisible = false;
});