﻿var WebSocketClient = (function() {
    function Client(settings) {
        this.uri = settings.uri;
    }

    var proto = Client.prototype;

    proto.connect = function (callback) {
        var uri = this.uri;
        var socket = this.socket = new WebSocket(uri);

        socket.onopen = function () {
            
           callback();
        }
  
        socket.onclose = function () {         
            var str = JSON.stringify({
                status: "Close",
                mark: ""
            });
            //чомусь передається пуста строка!
            this.send(str);
        }

        socket.onmessage = function (msg) {   
            if (msg.data === "clean") {
                $('#cards').empty();
            } else {
                if (msg.data !== "X") $('#reset').show();
                $('#cards').append($('<span>', { class: 'card', text: msg.data }));   
            }
        
        }
    }

    proto.sendMessage = function (msg) {
        var str = JSON.stringify(msg);
        this.socket.send(str);
    }

    return Client;
})();

var client = new WebSocketClient({
    uri: "ws://" + window.location.host + "/ws"
});

client.connect(() => {
    client.sendMessage({
        status: "New Client",
        mark: ""
    })
});

$('#Vote').click(() => {
    var mark = $('#mark option:selected').text();
    client.sendMessage({
        status: "Vote",
        mark: mark
    });
});

$('#reset').click(() => {
    client.sendMessage({
        status: "Reset",
        mark: ""
    });
});


