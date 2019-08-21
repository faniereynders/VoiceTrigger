"use strict";

var connection = new signalR.HubConnectionBuilder().withUrl("/trigger").build();


connection.on("TriggerReceived", function (key, value) {
    console.info("KEY: " + key + ", VALUE: " + value);

    document.getElementById(key).play();
    document.getElementById("gif").style.display = "block";

    setTimeout(function () {
        document.getElementById("gif").style.display = "none";
    }, 5000);
    
});

connection.start().then(function () {
    
}).catch(function (err) {
    return console.error(err.toString());
});
