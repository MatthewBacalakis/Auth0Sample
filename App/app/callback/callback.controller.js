(function () {

  'use strict';

  angular
    .module('app')
    .controller('CallbackController', callbackController);

  callbackController.$inject = ['$state'];
  function callbackController($state) {
    console.log($state);
  }

})();