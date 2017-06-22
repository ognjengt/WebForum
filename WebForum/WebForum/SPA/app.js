var webForum = angular.module('webForum',['ngRoute']);

webForum.config(function ($routeProvider, $locationProvider) {
    $routeProvider.when('/',
	{
	    controller: 'HomeController',
	    templateUrl: 'SPA/partials/home.html'
	}).when('/home',
	{
	    controller: 'HomeController',
	    templateUrl: 'SPA/partials/home.html'
	}).when('/login',
    {
        controller: 'LoginController',
        templateUrl: 'SPA/partials/login.html'
    })

});
