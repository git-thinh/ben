/////////////////////////////////////////////////////////////////////////
// VARIABLE
const _PORT = 61422;
/////////////////////////////////////////////////////////////////////////
// CONTRACTOR
const fetch = require('node-fetch');
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
//app.use(express.static('public'));
/////////////////////////////////////////////////////////////////////////
const _URL = {
    get_token: 'http://localhost:' + _PORT + '/get_token',
    get_files: 'http://localhost:' + _PORT + '/get_files'
};

app.get('/', function (req, res) {
    var code = req.query.code;
    var scope = req.query.scope;

    if (code == null) {
        const _OPEN_AUTH_CLIENT = new google.auth.OAuth2(_CONFIG.CLIENT_ID, _CONFIG.CLIENT_SECRET, _CONFIG.REDIRECT_URI);
        const url_oauthcallback = _OPEN_AUTH_CLIENT.generateAuthUrl({
            access_type: 'offline',
            scope: _CONFIG.SCOPES
        });
        res.redirect(url_oauthcallback);
    } else {
        _CONFIG.CODE_AUTH_CLIENT = code;
        res.redirect('/get_token');
    }
});

const _fet_GET_USER_INFO_OR_CREATE_NEW_IF_NOT_EXIST = async (req, res, next) => {
    console.log('GET_TOKEN: BEGIN ...')
    req.data = await fetch('http://localhost:3399/GET_USER_INFO_OR_CREATE_NEW_IF_NOT_EXIST', { method: 'POST', body: JSON.stringify(_CONFIG) }).then(res => res.json());
    console.log('GET_TOKEN: END ...')
    next();
};
app.get('/get_token', _fet_GET_USER_INFO_OR_CREATE_NEW_IF_NOT_EXIST, ({ data }, res) => {
    res.json({
        config: _CONFIG,
        service: data,
        urls: _URL,
        time: new Date().toString()
    });
});

const _fet_GET_RETRIEVE_ALL_FILES = async (req, res, next) => {
    req.data = await fetch('http://localhost:3399/GET_RETRIEVE_ALL_FILES').then(res => res.json());
    req.data.urls = _URL;
    next();
};
app.get('/get_files', _fet_GET_RETRIEVE_ALL_FILES, ({ data }, res) => res.json(data));
/////////////////////////////////////////////////////////////////////////
const _CONFIG = {
    CLIENT_ID: "962642037870-u8gg10odivorhmn19o3qqq9hlbmtras3.apps.googleusercontent.com",
    CLIENT_SECRET: "x8FbmONjiC3GN7lSPvyPwgG2",
    SCOPES: [
        "https://www.googleapis.com/auth/drive.file",
        "https://www.googleapis.com/auth/userinfo.email",
        "https://www.googleapis.com/auth/userinfo.profile",
        "https://www.googleapis.com/auth/drive.install"
        //'https://www.googleapis.com/auth/drive'
        //'https://www.googleapis.com/auth/plus.me',
        //'https://www.googleapis.com/auth/calendar'
        //'https://www.googleapis.com/auth/drive.metadata.readonly'
    ],
    REDIRECT_URI: "http://localhost:61422/",
    CODE_AUTH_CLIENT: '',
}
const { google } = require('googleapis');
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