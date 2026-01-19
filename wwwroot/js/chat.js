"use strict";

// Connect to SignalR Hub
var connection = window.connection = new signalR.HubConnectionBuilder()
    .withUrl("/chatHub")
    .build();

// Disable send button until connection is established
const sendButton = document.getElementById("sendButton");
if (sendButton) sendButton.disabled = true;

connection.on("ReceiveMessage", function (senderId, senderName, message, sessionId, time) {
    // Determine if message is for the current open chat window or notification
    // If widget is open, append message.
    // If admin dashboard, find correct chat pane.

    // Dispatch custom event for UI to handle
    const event = new CustomEvent('chatMessageReceived', {
        detail: { senderId, senderName, message, sessionId, time }
    });
    document.dispatchEvent(event);
});

connection.start().then(function () {
    if (sendButton) sendButton.disabled = false;
    console.log("SignalR Connected");
}).catch(function (err) {
    return console.error(err.toString());
});

// Function to send message (called by UI)
async function sendMessageToHub(receiverId, message, senderName, sessionId) {
    try {
        await connection.invoke("SendMessage", receiverId, message, senderName, sessionId);
    } catch (err) {
        console.error(err.toString());
    }
}

// Function to join group (e.g., for Admins)
async function joinChatGroup(groupName) {
    try {
        await connection.invoke("JoinGroup", groupName);
    } catch (err) {
        console.error(err.toString());
    }
}
