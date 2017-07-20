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
  
        

        socket.onmessage = function (msg) {          
            //var client = JSON.parse(msg.data);
            $('#cards').append($('<span>', { class: 'card', text: msg.data }));

            var count = Number(msg.data);     
           // if (count < 1)
               // $('#reset').css(display, block);
         //   paintCards(count);
        }

    }

    proto.sendMessage = function (msg) {     
        this.socket.send(msg);
    }

    paintCards = function (count) {       
        for (var i = 0; i < count; i++) {
            $('#cards').append($('<span>', { class: 'card', text: "X" }));
        }
    }

    return Client;
})();

var client = new WebSocketClient({
    uri: "ws://" + window.location.host + "/ws"
});

client.connect(function () { client.sendMessage("First!") });

$('#Vote').click(function() {
    client.sendMessage("some Message");
});
