"use strict";

var connection = new signalR.HubConnectionBuilder().withUrl("/trigger").build();


connection.on("TriggerReceived", function (key, value) {
    var gifId = "gif_" + key;
    var soundId = "sound_" + key;
    console.info("KEY: " + key + ", VALUE: " + value);

    document.getElementById(soundId).play();
    document.getElementById(gifId).style.display = "block";

    setTimeout(function () {
        document.getElementById(gifId).style.display = "none";
    }, 5000);
    
});

async function start() {
    try {
        await connection.start();
        console.log("connected");
        location.reload();
    } catch (err) {
        console.log(err);
        setTimeout(() => start(), 5000);
    }
};

connection.onclose(async () => {
    await start();
});

connection.start().then(function () {
    
}).catch(function (err) {
    return console.error(err.toString());
});
