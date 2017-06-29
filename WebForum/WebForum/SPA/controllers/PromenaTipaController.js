webForum.controller('PromenaTipaController', function ($scope, $rootScope, AccountFactory) {

    function init() {
        console.log('Promena tipa inicijalizovana');

        // Uzmi sve korisnike, ali nemoj uzeti ovog korisnika
        AccountFactory.uzmiSveKorisnikeOsimMene(sessionStorage.getItem("username")).then(function (response) {
            console.log(response.data);
            $scope.korisnici = response.data;
        });
    }

    init();

    $scope.promeniTipKorisniku = function (username, tip) {
        AccountFactory.promeniTipKorisniku(username, tip).then(function (response) {
            console.log(response.data);
            alert('Uspesno promenjen tip korisniku');
        });
    }

});