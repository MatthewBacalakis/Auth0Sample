(function () {

  'use strict';

  angular
    .module('app', ['auth0.auth0', 'ui.router', 'angular-jwt'])
    .config(config);

  config.$inject = [
    '$stateProvider',
    '$locationProvider',
    '$urlRouterProvider',
    'angularAuth0Provider',
    '$httpProvider'
  ];

  function config(
    $stateProvider,
    $locationProvider,
    $urlRouterProvider,
    angularAuth0Provider,
    $httpProvider
  ) {

    //create the interceptor that will add token to outgoing requests.
    $httpProvider.interceptors.push('TokenInterceptor');

    $stateProvider
      .state('home', {
        url: '/',
        controller: 'HomeController',
        templateUrl: 'app/home/home.html',
        controllerAs: 'vm'
      })
      .state('callback', {
        url: '/callback',
        controller: 'CallbackController',
        templateUrl: 'app/callback/callback.html',
        controllerAs: 'vm'
      });;

    // Initialization for the angular-auth0 library
    angularAuth0Provider.init({
      clientID: 'FLC8nTogJJ25M5vIUFiTn76aQHli7bN2',
      domain: 'mattbtest.auth0.com',
      responseType: 'token id_token',
      audience: 'https://auth0sampleapi',
      redirectUri: 'http://localhost:3000/callback',
      scope: 'openid'
    });

    $urlRouterProvider.otherwise('/');

    $locationProvider.hashPrefix('');

    /// Comment out the line below to run the app
    // without HTML5 mode (will use hashes in routes)
    $locationProvider.html5Mode(true);
  }

})();
