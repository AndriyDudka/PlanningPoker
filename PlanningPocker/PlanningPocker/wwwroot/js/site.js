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
            var arr = response.ClientsResponse;

            $('#card').empty();
            if (response.VoteEnabled === false) $('#vote').attr('disabled', true);
            else $('#vote').removeAttr('disabled');
            if (response.ResetShow === true) $('#reset').show();
            arr.forEach(function (socket, i, arr) {             
                var $div = $('<div>', { class: 'client panel panel-success' });
                var name = socket.Name;
                if (name.length > 10) name = name.substr(0, 10) + "...";
                $div.append($('<div>', {
                    class: 'client-name panel-heading',
                    text: name,
                    title: socket.Name
                }));
                $div.append($('<div>', {
                    class: 'client-card panel-body',
                    text: socket.Mark
                }));

                $('#card').append($div);
            });                      
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
    var name = prompt("Input your nickname", "Programmer");
} while (name === "null");

client.name = name;

client.connect(() => {
    client.sendMessage({
        name: this.name,
        status: "New Client",
        mark: ""
    })
});

$('#vote').click(() => {
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


