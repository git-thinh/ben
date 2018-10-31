
https://www.npmjs.com/package/ps-node
https://nodejs.org/api/child_process.html#child_process_child_process_execfile_file_args_options_callback

https://flaviocopes.com/node-terminate-program/
https://github.com/dcodeIO/protobuf.js/wiki/How-to-read-binary-data-in-the-browser-or-under-node.js%3F
https://github.com/dcodeIO/bytebuffer.js/issues/19
https://github.com/dcodeIO/protobuf.js/issues/123
https://github.com/dcodeIO/protobuf.js/issues/735
https://github.com/dcodeIO/protobuf.js/wiki/How-to-reverse-engineer-a-buffer-by-hand
https://github.com/improbable-eng/grpc-web/tree/master/ts/src/transports


Use NodeJs implement for WebAPI:

- Download NodeJS from address: https://nodejs.org/dist/v10.9.0/node-v10.9.0-linux-x64.tar.xz
- Copy file nodejs/node-v10.9.0-linux-x64.tar.xz to /root
- Copy file nodejs/app.js to /root/webapi/app.js
- Run command:
	tar -xf node-v10.9.0-linux-x64.tar.xz --directory /usr/local --strip-components 1
	node --version
	npm --version

# Init an empty project, Answer whatever you want, and it will create package.json
$ npm init
 
# Install grpc libraries
$ npm install --save grpc @grpc/proto-loader

-----------------------------------------------------------------------
npm install --save express
npm install --save ws
npm install --save underscore
npm install --save lodash
npm install --save grpc-caller
npm install --save grpc-client
npm install protobufjs [--save --save-prefix=~]
