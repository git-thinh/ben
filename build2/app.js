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
// Serve static files from the 'public' folder.
app.use(express.static('public'));

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
app.get('/exit_node', (req, res) => {
    res.send('shutdown server...!');
    process.kill(process.pid, 'SIGTERM');
});
server.listen(_PORT, function () {
    var host = server.address().address;
    var port = server.address().port;
    console.log('\r\n----> WebAPI listening at http://%s:%s \r\n\r\n', host, port);
});
process.on('SIGTERM', () => {
    server.close(() => {
        console.log('Process terminated');
        process.exit();
    })
});