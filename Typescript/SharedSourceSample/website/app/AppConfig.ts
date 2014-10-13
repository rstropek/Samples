/// <reference path="../lib/require.d.ts" />
/// <reference path="AppMain.ts" />

require.config({
    baseUrl: '../',
    paths: {
        'jquery': 'lib/jquery-1.9.1.min'
    }, 
    shim: { 
        jquery: {
            exports: '$'
        }
    },
    // Use urlArgs to disable require.js caching (only for debugging, remove for production)
    urlArgs: "bust=" + (new Date()).getTime()
});

require(['jquery','app/AppMain', 
     ], 
    ($, main) => {
        var appMain = new main.AppMain();
        appMain.run();
});