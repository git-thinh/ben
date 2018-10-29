/*
Use NodeJs implement for WebAPI:

- Download NodeJS from address: https://nodejs.org/dist/v10.9.0/node-v10.9.0-linux-x64.tar.xz
- Copy file nodejs/node-v10.9.0-linux-x64.tar.xz to /root
- Copy file nodejs/app.js to /root/webapi/app.js
- Run command:
	tar -xf node-v10.9.0-linux-x64.tar.xz --directory /usr/local --strip-components 1
	node --version
	npm --version


-----------------------------------------------------------------------
npm install --save express
npm install --save ws
npm install --save underscore
npm install --save lodash

*/

/////////////////////////////////////////////////////////////////////////
// VARIABLE
const _PORT = 56789; 
/////////////////////////////////////////////////////////////////////////
// CONTRACTOR
const express = require('express');
const http = require('http');
const WebSocket = require('ws');
const bodyParser = require("body-parser");
const app = express();
//const _ = require("underscore");
const _ = require('lodash');
/////////////////////////////////////////////////////////////////////////
// READ FILES
var fs = require('fs'),
    path = require('path'),
    file_domain = path.join(__dirname, 'data/domain.json');

var buffer_domain = fs.readFileSync(file_domain, 'utf8');
var _DOMAINS = JSON.parse(buffer_domain); 

/////////////////////////////////////////////////////////////////////////
// RESTFULL SERVICE: API
app.use(function (req, res, next) {
    res.header("Access-Control-Allow-Origin", "*");
    res.header("Access-Control-Allow-Methods", "POST, GET, OPTIONS");
    res.header("Access-Control-Allow-Headers", "Origin, Accept, Content-Type, Access-Control-Allow-Headers, Authorization, X-Requested-With");
    next();
});
app.use(bodyParser.urlencoded({
    extended: true
}));
app.use(bodyParser.json());

app.get('/', function (req, res) {
    var data = { "time_server": new Date().toString() };
    res.json(data);
});

app.post('/domain/add', function (req, res) {
    var json = req.body;
    console.log(json);
    //fs.writeFile(file_domain, JSON.stringify(json), 'utf8', function (err) {
    //    buffer_domain = JSON.stringify(json);
    //    console.log('WRITE SUCCESSFULLY ...');
    //    _DOMAINS = json;
    //});
    res.json(json);
    //res.json({ Ok: true });
});
/////////////////////////////////////////////////////////////////////////
// WEBSOCKET
const server = http.createServer(app);
const wss = new WebSocket.Server({ server });

wss.broadcast = function broadcast(data) {
    wss.clients.forEach(function each(client) {
        if (client.readyState === WebSocket.OPEN) {
            client.send(data);
        }
    });
};

wss.on('connection', (ws, req) => {
    ws.on('message', (message) => {
        console.log(`WS message ${message} from user`);
    });
});

const interval = setInterval(function ping() {
    var dt = new Date();
    wss.broadcast(JSON.stringify({ time: dt.toString() }));
}, 1000);
/////////////////////////////////////////////////////////////////////////
// START THE SERVER
server.listen(_PORT, function () {
    var host = server.address().address;
    var port = server.address().port;
    console.log('\r\n----> WebAPI listening at http://%s:%s \r\n\r\n', host, port);
});

//console.log('...............');

//var readline = require('readline');
//var rl = readline.createInterface(process.stdin, process.stdout);
//rl.setPrompt('guess> ');
//rl.prompt();
//rl.on('line', function (line) {
//    if (line === "right") rl.close();
//    rl.prompt();
//}).on('close', function () {
//    process.exit(0);
//});