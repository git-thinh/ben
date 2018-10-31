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
/////////////////////////////////////////////////////////////////////////

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
const _CONFIG = {
    "client_id": "962642037870-okjgtts7hrlh9912q48j11sbvj7jrohr.apps.googleusercontent.com",
    "project_id": "inspiring-orb-165801",
    "auth_uri": "https://accounts.google.com/o/oauth2/auth",
    "token_uri": "https://www.googleapis.com/oauth2/v3/token",
    "auth_provider_x509_cert_url": "https://www.googleapis.com/oauth2/v1/certs",
    "client_secret": "fW5BVRvfmDsbeSsTiD7GCtpL",
    "redirect_uris": ["http://localhost:56789/NODEJS_DRIVER_OAUTH_CLIENTID_CALLBACK"]
};

const { google } = require('googleapis');

const _OPEN_AUTH_CLIENT = new google.auth.OAuth2(_CONFIG.client_id, _CONFIG.client_secret, _CONFIG.redirect_uris[0]);

//// generate a url that asks permissions for Google+ and Google Calendar scopes
//const scopes = [
//    'https://www.googleapis.com/auth/drive'
//    //'https://www.googleapis.com/auth/plus.me',
//    //'https://www.googleapis.com/auth/calendar'
//];

// If modifying these scopes, delete token.json.
//const SCOPES = ['https://www.googleapis.com/auth/drive.metadata.readonly'];
const SCOPES = ['https://www.googleapis.com/auth/drive'];
const TOKEN_PATH = 'google_driver_token.json';

//const url_oauthcallback = _OPEN_AUTH_CLIENT.generateAuthUrl({
//    // 'online' (default) or 'offline' (gets refresh_token)
//    access_type: 'offline',
//    // If you only need one scope you can pass it as a string
//    scope: scopes
//});
//console.log('url_oauthcallback = ', url_oauthcallback);

app.get('/google-driver', function (req, res) {

    //// Check if we have previously stored a token.
    //fs.readFile(TOKEN_PATH, (err, token) => {
    //    if (err) {
    const url_oauthcallback = _OPEN_AUTH_CLIENT.generateAuthUrl({
        // 'online' (default) or 'offline' (gets refresh_token)
        access_type: 'offline',
        // If you only need one scope you can pass it as a string
        scope: SCOPES
    });
    console.log('url_oauthcallback = ', url_oauthcallback);
    res.redirect(url_oauthcallback);
    //    } else {
    //        var _token = JSON.parse(token);
    //        _OPEN_AUTH_CLIENT.setCredentials(_token);            
    //        f_goo_driver_listFiles();
    //        res.json(_token);
    //    }
    //});
});

app.get('/google-driver-files', function (req, res) {
    f_goo_driver_listFiles();
    res.json({  url: 'http://localhost:56789/google-driver' });
});

app.get('/NODEJS_DRIVER_OAUTH_CLIENTID_CALLBACK', function (req, res) {
    var code = req.query.code;
    var scope = req.query.scope;

    console.log(JSON.stringify(req.query));

    _OPEN_AUTH_CLIENT.getToken(code, (err, token) => {
        if (err) return console.error('Error retrieving access token', err);

        _OPEN_AUTH_CLIENT.setCredentials(token);

        // Store the token to disk for later program executions
        fs.writeFile(TOKEN_PATH, JSON.stringify(token), (err) => {
            if (err) console.error(err);
            console.log('Token stored to', TOKEN_PATH);
        });

        //res.redirect('http://localhost:56789/google-driver-files');
    });

    res.json({ code: code, scope: scope, url: 'http://localhost:56789/google-driver-files', time: new Date().toString() });
});

/**
 * Lists the names and IDs of up to 10 files.
 * @param {google.auth.OAuth2} _OPEN_AUTH_CLIENT An authorized OAuth2 client.
 */
function f_goo_driver_listFiles() {
    const drive = google.drive({ version: 'v3', _OPEN_AUTH_CLIENT });
    drive.files.list({
        pageSize: 10,
        fields: 'nextPageToken, files(id, name)',
    }, (err, res) => {
        if (err) return console.log('The API returned an error: ' + err);
        const files = res.data.files;
        if (files.length) {
            console.log('Files:');
            files.map((file) => {
                console.log(`${file.name} (${file.id})`);
            });
        } else {
            console.log('No files found.');
        }
    });
}

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