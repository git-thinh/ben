
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

const oauth2Client = new google.auth.OAuth2(
    _CONFIG.client_id,
    _CONFIG.client_secret,
    _CONFIG.redirect_uris[0]
);

// generate a url that asks permissions for Google+ and Google Calendar scopes
const scopes = [
    'https://www.googleapis.com/auth/drive'
    //'https://www.googleapis.com/auth/plus.me',
    //'https://www.googleapis.com/auth/calendar'
];

const url_oauthcallback = oauth2Client.generateAuthUrl({
    // 'online' (default) or 'offline' (gets refresh_token)
    access_type: 'offline',
    // If you only need one scope you can pass it as a string
    scope: scopes
});

console.log('url_oauthcallback = ', url_oauthcallback);


