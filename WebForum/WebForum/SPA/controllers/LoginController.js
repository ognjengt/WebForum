webForum.controller('LoginController', function ($scope, AccountFactory, $window, $rootScope) {

    $scope.korisnik = {};

    if ($rootScope.ulogovan == true) {
        $window.location.href = "#!/podforumi";
    }

    function init() {
        console.log('Login controller inicijalizovan');
    };

    init();

    $scope.RegistrujKorisnika = function (korisnik) {
        if (korisnik.username == null || korisnik.username == "") {
            alert('Popunite polje za korisnicko ime!');
            return;
        }
        else if (korisnik.ime == null || korisnik.ime == "") {
            alert('Popunite polje za ime!');
            return;
        }
        else if (korisnik.prezime == null || korisnik.prezime == "") {
            alert('Popunite polje za prezime!');
            return;
        }
        else if (korisnik.kontaktTelefon == null || korisnik.kontaktTelefon == "") {
            alert('Popunite polje za telefon!');
            return;
        }
        else if (korisnik.kontaktTelefon.match(/[a-z]/i)) {
            alert('Telefon ne sme da sadrzi karaktere a-z');
            return;
        }
        else if (korisnik.email == null || korisnik.email == "") {
            alert('Popunite polje za email!');
            return;
        }
        else if (!korisnik.email.includes('@')) {
            alert('Email nije validan, molimo vas upisite validan email');
            return;
        }
        else if (korisnik.pwd == null || korisnik.pwd == "") {
            alert('Popunite polje za sifru!');
            return;
        }
        else if (korisnik.pwd2 == null || korisnik.pwd2 == "") {
            alert('Popunite polje za potvrdu sifre!');
            return;
        }
        else if (korisnik.pwd != korisnik.pwd2) {
            alert("Sifre se ne poklapaju. Molimo vas potvrdite sifru.");
            return;
        }
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
    };

    $scope.Login = function (korisnik) {
        AccountFactory.Login(korisnik).then(function (response) {
            if (response.data == null) {
                alert("Korisnik sa tim korisnickim imenom i sifrom ne postoji");
            }
            else {
                console.log(response);
                document.cookie = "korisnik=" + JSON.stringify({
                    username: response.data.Username,
                    uloga: response.data.Uloga,
                    imePrezime: response.data.Ime + " " + response.data.Prezime
                }) + ";expires=Thu, 01 Jan 2019 00:00:01 GMT;";
                sessionStorage.setItem("username", response.data.Username);
                sessionStorage.setItem("uloga", response.data.Uloga);
                sessionStorage.setItem("imePrezime", response.data.Ime + " " + response.data.Prezime);

                $rootScope.ulogovan = true;
                $rootScope.korisnik = {
                    username: sessionStorage.getItem("username"),
                    uloga: sessionStorage.getItem("uloga"),
                    imePrezime: sessionStorage.getItem("imePrezime")
                };
                $window.location.href = "#!/";
            }
        });
    }



});