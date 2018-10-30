let path = require('path')
let express = require('express')
let app = express()
let messages = [
    { text: 'tiếng việt 2', lang: 'english' },
    { text: 'tiếng việt 1', lang: 'tatar' },
    { text: 'tiếng việt 3', lang: 'swedish' }
]

let fs = require('fs')
let file_lang = path.join(__dirname, 'lang.txt')

let publicFolderName = 'public'
app.use(express.static(publicFolderName))
app.use(function (req, res, next) {
    if (!req.is('application/octet-stream')) return next()
    var data = [] // List of Buffer objects
    req.on('data', function (chunk) {
        data.push(chunk) // Append Buffer object
    })
    req.on('end', function () {
        if (data.length <= 0) return next()
        data = Buffer.concat(data) // Make one large Buffer of it
        console.log('Received buffer', data)
        req.raw = data
        next()
    })
})
let ProtoBuf = require('protobufjs')
let builder = ProtoBuf.loadProtoFile(
    path.join(__dirname, publicFolderName, 'message.proto')
)
let Message = builder.build('Message')

app.get('/api/messages', (req, res, next) => {
    var mi = messages[Math.round(Math.random() * 2)];
    let msg = new Message(mi)
    console.log('api -> ui = Origin: ', JSON.stringify(mi));
    console.log('api -> ui = Encode and decode: ', Message.decode(msg.encode().toBuffer()))
    console.log('api -> ui = Buffer we are sending: ', msg.encode().toBuffer())
    // res.end(msg.encode().toBuffer(), 'binary') // alternative
    res.send(msg.encode().toBuffer())
    // res.end(Buffer.from(msg.toArrayBuffer()), 'binary') // alternative
})

app.post('/api/messages', (req, res, next) => {
    console.log('\r\n-----------------------------------------\r\n');

    if (req.raw) {
        try {
            // Decode the Message
            var msg = Message.decode(req.raw)
            console.log('ui -> api = Received "%s" in %s', msg.text, msg.lang)


            fs.writeFile(file_lang, JSON.stringify(msg), 'utf8', function (err) { 
                console.log('WRITE SUCCESSFULLY ...'); 
            });

        } catch (err) {
            console.log('Processing failed:', err)
            next(err)
        }
    } else {
        console.log("Not binary data")
    }
})

app.all('*', (req, res) => {
    res.status(400).send('Not supported')
})

app.listen(3000)