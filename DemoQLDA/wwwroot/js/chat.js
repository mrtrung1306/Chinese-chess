"use strict";
var connection = new signalR.HubConnectionBuilder().withUrl("/chatHub").build();
let lastMessageTime = 0;
let continuousMessagesCount = 0;

//Disable the send button until connection is established.
document.getElementById("sendButton").disabled = true;

//connection.on("ReceiveMessage", function (user, message) {
//    var li = document.createElement("li");
//    document.getElementById("messagesList").appendChild(li);
//    // We can assign user-supplied strings to an element's textContent because it
//    // is not interpreted as markup. If you're assigning in any other way, you 
//    // should be aware of possible script injection concerns.
//    li.textContent = `${user} says ${message}`; 
//});
function addMessageToChat(user, message) {
    let ul = document.getElementById("messagesList");
    let li = document.createElement("li");
    li.setAttribute("data-time", Date.now());
    li.textContent = `${user} says: ${message}`;
    ul.appendChild(li);

    let messages = ul.getElementsByTagName("li");
    if (messages.length > 5) {
        for (let i = 0; i < messages.length - 5; i++) {
            ul.removeChild(messages[i]);
        }
    }
}

connection.on("ReceiveMessage", function (user, message) {
    addMessageToChat(user, message);
});

//connection.on("ShowErrorMessage", (errorMessage) => {
//    // Hiển thị thông báo lên giao diện của người dùng
//    alert(errorMessage); // Hoặc sử dụng một cách khác để hiển thị thông báo
//});
connection.start().then(function () {
    document.getElementById("sendButton").disabled = false;
    let roomId = localStorage.getItem('roomId');
    let user = "system";
    let message = "đã tham gia phòng chat";
    connection.invoke("SendToGroup", roomId, user, message);
   /* connection.invoke("GetConnectionId").then(function (connectionId) { console.log(connectionId) })*/
}).catch(function (err) {
    return console.error(err.toString());
});

//document.getElementById("sendButton").addEventListener("click", function (event) {
//    var user = document.getElementById("userInput").value;
//    var message = document.getElementById("MyText").value;
//    connection.invoke("SendMessage", user, message).catch(function (err) {
//        return console.error(err.toString());
//    });
//    event.preventDefault();
//});



document.getElementById("sendButton").addEventListener("click", function (event) {
   
    let currentTime = Date.now();
    if (currentTime - lastMessageTime >= 10000) {
        continuousMessagesCount = 1;
        let roomId = localStorage.getItem('roomId')
        let user = document.getElementById("userInput").value;
        let message = document.getElementById("MyText").value;
        connection.invoke("SendToGroup", roomId, user, message).catch(function (err) {
            return console.error(err.toString());
        });
        lastMessageTime = currentTime;
    }
    else {
        continuousMessagesCount++;
        if (continuousMessagesCount > 5) {
            console.log("Bạn đã gửi quá nhiều tin nhắn liên tục trong khoảng thời gian chờ đợi.");
            return;
        }
        console.log("Bạn chỉ có thể gửi tin nhắn sau mỗi 10 giây.");
    }
    event.preventDefault();
});
