var grpc = require('grpc');
var protoLoader = require("@grpc/proto-loader");
const REMOTE_SERVER = "localhost:50051";

//Load the protobuf
var protoDescriptor = grpc.loadPackageDefinition(
  protoLoader.loadSync("helloworld.proto", {
      keepCase: true,
      longs: String,
      enums: String,
      defaults: true,
      oneofs: true
  })
);

var client = new protoDescriptor.helloworld.Greeter(REMOTE_SERVER, grpc.credentials.createInsecure());

//console.log(client);

//client.sayHello({ name: 'Mr Thinh' }, function (error, response) {
//    if (error)
//        console.log('Error: ', error);
//    else
//        console.log('loginresponse:', response.message);
//});

let channel = client.sayHello({ name: '12345' });
channel.on("data", onData);
//When server send a message
function onData(message) {
    console.log('====> ', message);
}

//function main() {
//    var client = new api_proto.InternalService('192.168.1.80:8080',
//                                       grpc.credentials.createInsecure());
//    var body = JSON.stringify({ username: "admin", password: "password" })

//    client.Login({}, function (err, response) {
//        console.log('loginresponse:', response.message);
//    });
//}

//main();

//////var booksProto = grpc.load('books.proto');

//////var client = new booksProto.books.BookService(REMOTE_SERVER, grpc.credentials.createInsecure());
////var client = new booksProto.books.BookService(REMOTE_SERVER, grpc.credentials.createSsl());
//////var client = new helloProto.Greeter(REMOTE_SERVER, grpc.credentials.createInsecure());

////console.log('Start ....');

////function printResponse(error, response) {
////    if (error)
////        console.log('Error: ', error);
////    else
////        console.log(response);
////}

////function listBooks() {
////    client.list({}, function (error, books) {
////        printResponse(error, books);
////    });
////}

////listBooks();
