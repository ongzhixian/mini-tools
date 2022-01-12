"use strict";

var connection = new signalR.HubConnectionBuilder().withUrl("/chatHub").build();

//Disable the send button until connection is established.
document.getElementById("sendButton").disabled = true;

connection.on("ReceiveMessage", function (user, message) {
    var li = document.createElement("li");
    document.getElementById("messagesList").appendChild(li);
    // We can assign user-supplied strings to an element's textContent because it
    // is not interpreted as markup. If you're assigning in any other way, you 
    // should be aware of possible script injection concerns.
    li.textContent = `${user} says ${message}`;
    console.log("in ReceiveMessage");
});

connection.start().then(function () {
    document.getElementById("sendButton").disabled = false;
}).catch(function (err) {
    return console.error(err.toString());
});

document.getElementById("sendButton").addEventListener("click", function (event) {
    var user = document.getElementById("userInput").value;
    var message = document.getElementById("messageInput").value;
    connection.invoke("SendMessage", user, message).catch(function (err) {
        return console.error(err.toString());
    });
    event.preventDefault();
});

var connection2 = new signalR.HubConnectionBuilder().withUrl("/hubs/clock", {
    skipNegotiation: true,
    transport: signalR.HttpTransportType.WebSockets
}).build();

connection2.on("ShowTime", function (message) {
    console.log("in ShowTime");
    console.info(message);
});

connection2.start().then(function () {
}).catch(function (err) {
    return console.error(err.toString());
});
