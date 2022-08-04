

var connection = new signalR.HubConnectionBuilder().withUrl("/chat").build();

connection.start()


$("#sendBtn").click(function (e) {
    e.preventDefault();
    var message = $("#messageInput").val();
    var user = "user";
    connection.invoke("SendMessage", user, message).catch(function (err) {
        return console.error(err.tostring());
    });

    connection.on("ReceiveMessage", function (user, message) {
       
    });

    connection.on("Users", users => {
        users.foreach(x => {
            
        })
    });

})



