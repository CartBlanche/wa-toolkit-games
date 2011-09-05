var io = require('socket.io').listen(8080);
var http = require('http');

io.sockets.on('connection', function (socket) {
  socket.on('command', function (command) {
    socket.broadcast.to(socket.room).emit('command', command);
    console.log('Sending command', command);
    var message = { name: 'command', args: [ command ] };
    var text = JSON.stringify(message);
    broadcast(text+'\r\n', socket);
  });
  
  socket.on('join', function(room) {
	socket.room = room;
	socket.join(room);
	console.log('Joining', room);
  });

  socket.on('disconnect', function () {
    console.log('user disconnected');
  });
});

var net = require('net');

var clients = [];

var server = net.createServer(function (c) {
    console.log('client connected');
    c.setEncoding('ascii');
    addClient(c);
	
	var state = null;

    c.on('data', function (data) {
        //console.log("Received", data);

        var isEmpty = /^\s*$/;
        var parts = data.split("\n");
        var len = parts.length;
		
		if (len>0 && state != null) {
			parts[0] = state + parts[0];
			state = null;
		}
		
		if (len>0 && data.length > 0 && data[data.length-1] != '\n') {
			state = parts[len-1];
			parts[len-1] = '';
		}
		
        for (var i=0; i<len; i++) {
            // ignore empty strings
            if (isEmpty.test(parts[i])) { continue; }
            var message = null;

            try {
			//	console.log("Parsing", parts[i]);
                message = JSON.parse(parts[i]);
            }
            catch (err) {
                console.log("Error Processing Data", err, parts[i]);
                continue;
            }

            if (message != null && message.name != null && message.args != null) {
                if (message.name == 'join') {
                    c.room = message.args[0];
                 	console.log('Joining', c.room);
                    return;          
                }

                if (c.room != null) {
                    console.log("Broadcast", "Room " + c.room);
                    broadcast(parts[i]+'\r\n', c);
                    io.sockets.in(c.room).emit(message.name, message.args[0]);
                }
            }
        }
    });

    c.on('close', function () {
        console.log('client disconnected');
        removeClient(c);
    });
});

function addClient(client)
{
	clients.push(client);
}

function removeClient(client)
{
    for (var n in clients) {
	if (clients[n] == client) {
	    clients.splice(n,1);
	    return;
	}
    }
}

function broadcast(data, sender)
{
    for (var n in clients) {
        var client = clients[n];

    	if (client == sender)
	    	continue;

        if (client.room !== sender.room)
            continue;

	    try {
		    client.write(data);
	    }
	    catch (err) {
		    console.log("Error Sending Message");
    	}	
    }
}

server.listen(8124);