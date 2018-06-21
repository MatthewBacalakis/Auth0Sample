(() => {        
    'use strict';
    angular.module('app').factory('TokenInterceptor',TokenInterceptor);

    TokenInterceptor.$inject = ["authService"];
        
    function TokenInterceptor(authService) {
        //when token available adds an Authorization header to each output request
        //with value: Bearer access_token
        var interceptor = { request: request };

        function request(config) {
            if (authService.isAuthenticated()){
                config.headers.authorization = 'Bearer ' + localStorage.getItem('access_token');
            }
            return config;
        }
        
        return interceptor;
    }
}
)();