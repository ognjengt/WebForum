webForum.controller('LoginController', function ($scope, AccountFactory, $window, $rootScope) {

    $scope.korisnik = {};

    if ($rootScope.ulogovan == true) {
        $window.location.href = "#!/pocetna";
    }

    function init() {
        console.log('Login controller inicijalizovan');
    };

    init();

    $scope.RegistrujKorisnika = function (korisnik) {
        if (korisnik.pwd != korisnik.pwd2) {
            alert("Sifre se ne poklapaju. Molimo vas potvrdite sifru.");
        }
        else {
            AccountFactory.RegistrujKorisnika(korisnik).then(function (response) {
                if (response.data == true) {
                    console.log(response.data);
                    $rootScope.uspesnoRegistrovan = "Uspesno ste se registrovali, molimo vas da se ulogujete";
                    $window.location.href = "#!/login";
                }
                else {
                    alert("Korisnik sa tim korisnickim imenom vec postoji, molimo vas odaberite drugo");
                }
            })
        }
    };

    $scope.Login = function (korisnik) {
        AccountFactory.Login(korisnik).then(function (response) {
            if (response.data == null) {
                alert("Korisnik ne postoji");
            }
            else {
                document.cookie = "korisnik=" + JSON.stringify({ username: response.data.Username, uloga: response.data.Uloga, imePrezime: response.data.Ime+" "+response.data.Prezime }) + ";expires=Thu, 01 Jan 2019 00:00:01 GMT;";
                sessionStorage.setItem("username", response.data.Username);
                sessionStorage.setItem("uloga", response.data.Uloga);
                sessionStorage.setItem("imePrezime", response.data.Ime + " " + response.data.Prezime);
                $rootScope.ulogovan = true;
                $rootScope.korisnik = {
                    username: sessionStorage.getItem("username"),
                    uloga: sessionStorage.getItem("uloga"),
                    imePrezime: sessionStorage.getItem("imePrezime")
                };
                document.location.href = "#!/";
            }
        });
    }



});