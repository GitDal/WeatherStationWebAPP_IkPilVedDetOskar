var connection = new signalR.HubConnectionBuilder().withUrl("/updateHub").withAutomaticReconnect().build();

connection.on('NewData', function (message) {
    console.log(message);
    var li = document.createElement("li");
    li.textContent = message.date + " " + message.name + " - " +
        "Temperature: " + message.temperature + " - " +
        "Pressure: " + message.pressure + " - " +
        "Humidity: " + message.humidity;
    document.getElementById("messagesList").appendChild(li);
});

connection.start().then(function () {
    console.log("Connected");
}).catch(function (err) {
    console.error(err.toString());
});
