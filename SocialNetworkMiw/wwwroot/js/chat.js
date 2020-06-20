"use strict";

var connection = new signalR.HubConnectionBuilder().withUrl("/chatHub?group=" + document.getElementById("group").value).build();

document.getElementById("sendButton").disabled = true;

connection.on("ReceiveMessage", function (user, message, id) {
    var msg = message.replace(/&/g, "&amp;").replace(/</g, "&lt;").replace(/>/g, "&gt;");
    var encodedMsg = user + " says: " + msg;
    var li = document.createElement("li");
    li.textContent = encodedMsg;
    document.getElementById("messagesList").appendChild(li);
    connection.invoke("ReadMessage", id).catch(function (err) {
        return console.error(err.toString());
    });
});

connection.start().then(function () {
    document.getElementById("sendButton").disabled = false;
}).catch(function (err) {
    return console.error(err.toString());
});

document.getElementById("sendButton").addEventListener("click", function (event) {
    var message = document.getElementById("messageInput").value;
    connection.invoke("SendMessage", message).catch(function (err) {
        return console.error(err.toString());
    });
    event.preventDefault();
});