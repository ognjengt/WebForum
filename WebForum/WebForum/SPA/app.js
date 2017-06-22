var webForum = angular.module('webForum',['ngRoute']);

webForum.config(function($routeProvider) {
	$routeProvider.when('/',
	{
		//controller: 'productsController',// inace je podeseno ng-controller atributom
		templateUrl: 'SPA/partials/home.html'
	}).when('/home',
	{
		//controller: 'shoppingCartController', // inace je podeseno ng-controller atributom
		templateUrl: 'SPA/partials/home.html'
	}).when('/login',
    {
        templateUrl: 'SPA/partials/login.html'
    })
});

