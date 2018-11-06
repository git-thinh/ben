var f_log = 1 ? console.log.bind(console, 'UI: ') : function () { };
function f_get(url) { var r = new XMLHttpRequest(); r.open('GET', url, false); r.send(null); if (r.status === 200) return r.responseText; return ''; }



// #region VUE: BROADCAST MESSAGE ...

var BROADCAST_COMPONENTS_VUE;
function f_broadCastVue_init() {
    var _divBroadCast = document.createElement('div');
    _divBroadCast.id = 'broadcast-xxx-4xxx-yxxx-xxxxxxxxxxxx'.replace(/[xy]/g, function (c) {
        var r = Math.random() * 16 | 0, v = c == 'x' ? r : (r & 0x3 | 0x8);
        return v.toString(16);
    });
    document.body.appendChild(_divBroadCast);
    BROADCAST_COMPONENTS_VUE = new Vue({
        el: '#' + _divBroadCast.id,
        methods: {
            f_sendMessage_toHomeUI: function (data) {
                this.$emit('BROADCAST_COMPONENTS_VUE_REG_HOMEUI', data);
            },
            f_sendMessage_toAll: function (data) {
                this.$emit('BROADCAST_COMPONENTS_VUE', data);
            },
            f_sendMessage_toSender: function (channelSender, data) {
                this.$emit(channelSender, data);
            },
        }
    });
}
f_broadCastVue_init();

function f_hui_broadcast_ComponentsVUE_Join_Event(f_callback) {
    BROADCAST_COMPONENTS_VUE.$on('BROADCAST_COMPONENTS_VUE_REG_EVENT', f_callback);
}

function f_hui_broadcast_ComponentsVUE_Join_HomeUI(f_callback) {
    BROADCAST_COMPONENTS_VUE.$on('BROADCAST_COMPONENTS_VUE_REG_HOMEUI', f_callback);
}

function f_hui_broadcast_ComponentsVUE_Join_All(f_callback) {
    BROADCAST_COMPONENTS_VUE.$on('BROADCAST_COMPONENTS_VUE', f_callback);
}

// vueCom = {id, name}
function f_hui_broadcast_ComponentsVUE_Join_Sender(f_callback, _self) {
    var channelSender;
    if (_self != null && _self._channelBroadcast != null)
        channelSender = _self._channelBroadcast.getName();
    if (channelSender != null)
        BROADCAST_COMPONENTS_VUE.$on(channelSender, f_callback);
    //console.log('channelSender = ', channelSender)
}

function f_vue_broadcast_message_Receiver(_self, msg, f_callback) {
    //console.log('VUE_COMPONENTS.' + _self._channelBroadcast.getName() + ' = ', msg);
    if (msg != null) {
        if (msg.CALL != null) {
            if (_self[msg.CALL] != null) _self[msg.CALL](msg);
            return;
        }
    }

    f_callback(msg);
}

// #endregion

var _profile;
var _app;
var _page = '<button @click="f_destroy">Destroy MAIN</button><button @click="f_main_broadCast_toComs({main: new Date()})">MAIN broadCast to COMs</button> <br><button @click="f_com1">com1: load</button> | <button @click="f_com2">com2: load</button> | <button @click="f_com3">com3: dynamic</button> | <button @click="f_com4">com4: Profile</button>  <hr> '
    + '<div id="mount-point"></div>';

var _mixin = {
    created: function () {
        f_log('Class Base component created ... data = ', JSON.stringify(this.$data));
    },
    events: {
        BROADCAST_TO_COMS: 'f_receiver_messageFromVue'
    },
    methods: {
        f_main_broadCast_toComs: function (m) {
            f_log('----> Main send broadCast to COMs: ', m);
            if (_profile) _profile.$emit('BROADCAST_TO_COMS', m);
        },
        f_receiver_messageFromVue: function (m) {
            f_log('#####> COM received broadCast message from VUE: ', m);
        },
        f_broadCast_toVue: function (m) {
            f_log('----> component send broadcast to vue: ', m);
            //this.$dispatch('BROADCAST_TO_VUE', m);
            _app.$emit('BROADCAST_TO_VUE', m);
        },
        f_destroy: function () {
            this.$destroy();
        },
        f_com3: function () {
            f_log('f_com3 -> dynamic component -> destroy ...');

            //this.destroy();

            var temp = '<div><h1> Test com3: <br>{{ msg }} </h1></div>' + _page;

            this.msg = 'COM3: ' + new Date().toString();

            this.$el.innerHTML = temp;
            //$(this.$el).append(temp);

            this.$compile(this.$el);
        },
        f_com4: function () {
            f_log('f_com4');

            // create reusable constructor
            var Profile = Vue.extend({
                template: '<p>{{firstName}} {{lastName}} aka {{alias}}</p><button @click="f_destroy">Destroy Profile</button> <button @click="f_broadCast_toVue({time: new Date().toString()})">Profile send f_broadCast_toVue </button><br>  ' + _page
            });

            // create an instance of Profile
            var profile = new Profile({
                mixins: [_mixin],
                data: {
                    firstName: 'Walter',
                    lastName: 'White',
                    alias: 'Heisenberg'
                },
                ready: function () {
                    f_log('my-component-profile ready');
                },
                destroyed: function () {
                    f_log('my-component-profile destroyed');
                },
                methods: {
                    f_destroy: function () {
                        f_log('destroy _profile ...');
                        _profile.$destroy();
                    },
                    // #region [ SCREEN: BROADCAST ]

                    f_broadcas_Register: function () {
                        var _self = this;
                        this._channelBroadcast = { id: this._uid, name: 'BROADCAST_COMPONENTS_VUE_REG_HOMEUI', getName: function () { return this.name; } };
                        f_hui_broadcast_ComponentsVUE_Join_All(function (msg) { f_vue_broadcast_message_Receiver(_self, msg, _self.f_com_process_Message_Receiver); });
                        f_hui_broadcast_ComponentsVUE_Join_Sender(function (msg) { f_vue_broadcast_message_Receiver(_self, msg, _self.f_com_process_Message_Receiver); }, _self);
                        f_hui_broadcast_ComponentsVUE_Join_Event(function (msg) { f_vue_broadcast_message_Receiver(_self, msg, _self.f_com_even_Message_Receiver); }, _self);
                    },

                    f_com_process_Message_Receiver: function (msg) {
                        var _self = this;
                        //////f_log_hui('HOMEUI -> RECEIVER: ', msg);
                        var key = msg.KEY, data = msg.DATA;
                        switch (key) {
                            case 'WIDGET_OVERLAP':
                                _self.f_hui_process_wigetOverlap(data);
                                break;
                            case 'API_WAITING':
                                _self.f_hui_process_apiWating(data);
                                break;
                        }
                    },

                    // #endregion
                }
            });

            // mount it on an element
            _profile = profile.$mount('#mount-point');

        },
        f_com1: function () {
            f_log('f_com1');
            _app.currentView = 'com1';
        },
        f_com2: function () {
            f_log('f_com2');
            _app.currentView = 'com2';
        }
    }
};

_app = new Vue({
    el: '#app',
    replace: false,
    data: {
        currentView: 'home'
    },
    created: function () {
        f_log('VUE created ...');
    },
    events: {
        BROADCAST_TO_VUE: 'f_receiver_messageFromComponents'
    },
    components: {
        com1: {
            mixins: [_mixin],
            template: '<div>this com1: {{msg}} || <div v-if="true"><button @click="rerender">re-render</button>' + _page + '</div></div>',
            ready: function () {
                f_log('my-component1 ready');
            },
            destroyed: function () {
                f_log('my-component1 destroyed');
            }
        },
        com2: {
            mixins: [_mixin],
            template: '<div>this com2: {{msg}} || <div v-if="true"><button @click="rerender">re-render</button>' + _page + '</div></div>',
            ready: function () {
                f_log('my-component2 ready');
            },
            destroyed: function () {
                f_log('my-component2 destroyed');
            }
        },
        home: {
            mixins: [_mixin],
            data: function () {
                return {
                    show: true,
                    msg: ''
                };
            },
            template: '<div>A custom component: {{msg}} || <div v-if="show"><button @click="rerender">re-render</button>' + _page + '</div></div>',
            activate: function (done) {
                f_log('begin contractor ....');

                var self = this;
                setTimeout(function () {
                    f_log('complete contractor ....');

                    self.msg = new Date().toString();
                    done();
                }, 300);
            },
            created: function () {
                f_log('Component created ...');
            },
            ready: function () {
                f_log('my-component-home ready');
            },
            destroyed: function () {
                f_log('my-component-home destroyed');
            },
            methods: {
                done: function () {
                    f_log('DONE: ...');
                },
                rerender: function () {
                    this.msg = new Date().toString();

                    this.show = false;

                    this.$nextTick(function () {
                        this.show = true;

                        f_log('re-render start');

                        this.$nextTick(function () {
                            f_log('re-render end');
                        });
                    });
                }
            }
        }



    },
    methods: {
        f_receiver_messageFromComponents: function (m) {
            f_log('====> BROADCAST_TO_VUE = ', m);
        },
        f_destroy: function () {
            this.$destroy();
        }
    }
});

