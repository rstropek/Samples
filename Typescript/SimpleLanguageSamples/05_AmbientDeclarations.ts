/// <reference path="05_jquery.d.ts" />

$(document.body).ready(function(){
  alert("Loaded");
  $("a").click(function(event) {
    alert("The link no longer took you to timecockpit.com");
    event.preventDefault();
  });
});
