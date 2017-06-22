webForum.controller('HomeController', function ($scope) {
    $scope.pozdrav = "Deste brate da li ovaj kontroler radi";

    function init() {
        console.log("Inicijalizovan home controller");
    }

    init();

});