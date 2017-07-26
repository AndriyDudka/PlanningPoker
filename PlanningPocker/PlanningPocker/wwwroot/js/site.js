var WebSocketClient = (function() {
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
            var response = JSON.parse(msg.data);
            $('.grid-row1').empty();

            for (var i = 0; i < response.length; i++)
            {
                if (response[i].Mark !== "X") $('#reset').show();

                var $div = $('<div>', { class: 'client panel panel-success' });
                $div.append($('<div>', { class: 'client-name panel-heading', text: response[i].Name }));
                $div.append($('<div>', { class: 'client-card panel-body', text: response[i].Mark }));

                $('.grid-row1').append($div);   
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

do {
    var name = prompt("Input your name", "Vasia Pupkin");
} while (name === "null");

client.name = name;

client.connect(() => {
    client.sendMessage({
        name: this.name,
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


