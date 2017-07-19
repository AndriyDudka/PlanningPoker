var WebSocketClient = (function() {
    function Client(settings) {
        this.uri = settings.uri;
    }

    var proto = Client.prototype;

    proto.connect = function (callback) {
        var uri = this.uri;
        var socket = this.socket = new WebSocket(uri);

        socket.onopen = function() {
            $('#card').append($('<p>', { text: "connected" }));
            callback();
        }

        socket.onmessage = function(msg) {
            $('#card').append($('<p>', { text: msg.data }));
        }
    }

    proto.sendMessage = function (msg) {     
        this.socket.send(msg);
    }

    return Client;
})();

var client = new WebSocketClient({
    uri: "ws://" + window.location.host + "/ws"
});

client.connect(function(){client.sendMessage("Hello. Its me!")});
