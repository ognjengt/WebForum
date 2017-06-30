webForum.controller('PromenaTipaController', function ($scope, $rootScope, $window, AccountFactory) {

    if (sessionStorage.getItem("uloga") != 'Administrator') {
        alert('Niste autorizovani da pregledate ovu stranicu.');
        $window.location.href = "#!/podforumi";
    }

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