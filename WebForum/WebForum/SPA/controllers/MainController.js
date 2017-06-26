webForum.controller('MainController', function ($scope, $location, $rootScope) {
 
    // Kontroler koji je zaduzen za Index.cshtml, svaki put kada se bilo koja stranica ucita, proverava se cookie

    if (document.cookie !== "") {
        //console.log(document.cookie);
        var cookieInfo = document.cookie.substring(9, document.cookie.length);
        var parsed = JSON.parse(cookieInfo);
        sessionStorage.setItem("username", parsed.username);
        sessionStorage.setItem("uloga", parsed.uloga);
        sessionStorage.setItem("imePrezime", parsed.imePrezime);
        sessionStorage.setItem("praceniPodforumi", JSON.stringify(parsed.praceniPodforumi));
        sessionStorage.setItem("snimljeneTeme", JSON.stringify(parsed.snimljeneTeme));
        sessionStorage.setItem("snimljeniKomentari", JSON.stringify(parsed.snimljeniKomentari));

        $rootScope.ulogovan = true;
        $rootScope.korisnik = {
            username: sessionStorage.getItem("username"),
            uloga: sessionStorage.getItem("uloga"),
            imePrezime: sessionStorage.getItem("imePrezime"),
            praceniPodforumi: JSON.parse(sessionStorage.getItem("praceniPodforumi")),
            snimljeneTeme: JSON.parse(sessionStorage.getItem("snimljeneTeme")),
            snimljeniKomentar: JSON.parse(sessionStorage.getItem("snimljeniKomentari"))
        };
    } else {
        $rootScope.ulogovan = false;
    }

    $scope.checkActive = function (path) {
        return ($location.path().substr(0, path.length) === path) ? 'active' : '';
    }


    $scope.Logout = function () {
        document.cookie = 'korisnik=;expires=Thu, 01 Jan 1970 00:00:01 GMT;';
        sessionStorage.clear();
        $rootScope.ulogovan = false;
        $rootScope.korisnik = {};
        document.location.href = "#!/login";
    }

});