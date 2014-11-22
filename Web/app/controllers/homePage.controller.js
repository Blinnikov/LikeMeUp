(function() {
    'use strict';

    angular
        .module('app')
        .controller('HomePageController', HomePageController);

    HomePageController.$inject = [];

    function HomePageController() {
        var vm = this;
        vm.text = 'Greetings from ng controler!';
    }
})();