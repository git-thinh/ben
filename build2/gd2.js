const google = require('googleapis');
const fs = require('fs');
const config = {
    "client_id": "962642037870-ksd2s4spaj7rooh3r9iu5ujkp8qtie9q.apps.googleusercontent.com",
    "project_id": "inspiring-orb-165801",
    "auth_uri": "https://accounts.google.com/o/oauth2/auth",
    "token_uri": "https://www.googleapis.com/oauth2/v3/token",
    "auth_provider_x509_cert_url": "https://www.googleapis.com/oauth2/v1/certs",
    "client_secret": "rCrUiTM3W3iWowytIPr6_WJ1",
    "redirect_uris": ["urn:ietf:wg:oauth:2.0:oob", "http://localhost"]
};
const drive = google.drive('v3');
const targetFolderId = "123456789"


const jwtClient = new google.auth.JWT(
    config.client_email,
    null,
    config.private_key,
    ['https://www.googleapis.com/auth/drive'],
    null
);

jwtClient.authorize((authErr) => {
    if (authErr) {
        console.log(authErr);
        return;
    }


    const fileMetadata = {
        name: './file.txt',
        parents: [targetFolderId]
    };

    const media = {
        mimeType: 'application/vnd.ms-excel',
        body: fs.createReadStream('./file.txt')
    };

    drive.files.create({
        auth: jwtClient,
        resource: fileMetadata,
        media,
        fields: 'id'
    }, (err, file) => {
        if (err) {
            console.log(err);
            return;
        }
        console.log('Uploaded File Id: ', file.data.id);
    });
});