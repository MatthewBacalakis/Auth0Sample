(()=>{
    'use strict';
    angular.module("app").component("a0sItemButtons",{
        templateUrl: 'app/items/itemButtons.html',
        controller: ItemButtonsController,
        controllerAs:'vm'
        
    });

    function ItemButtonsController(authService, a0sItemService){
        var vm = this;
        vm.buttonClicked = buttonClicked;
        vm.scope = authService.getUserItemsScope();

        function buttonClicked(method){
            a0sItemService.CallItemService(method)
            .then(setResult)
            .catch(setResult);
        }

        function setResult(result){
            if (result.status === 401){
                vm.result = result.data.Message;
            }else{
                vm.result = result.data;
            }
        }
    }
})();