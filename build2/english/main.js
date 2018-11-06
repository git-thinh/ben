var f_log = 1 ? console.log.bind(console, 'UI: ') : function () { };
function f_get(url) { var r = new XMLHttpRequest(); r.open('GET', url, false); r.send(null); if (r.status === 200) return r.responseText; return ''; }

var _profile;
var _app;
var _page = '<button @click="f_destroy">Destroy MAIN</button> <br><button @click="f_com1">com1: load</button> | <button @click="f_com2">com2: load</button> | <button @click="f_com3">com3: dynamic</button> | <button @click="f_com4">com4: Profile</button>  <hr> '
    + '<div id="mount-point"></div>';

var _mixin = {
    created: function () {
        f_log('Class Base component created ... data = ', JSON.stringify(this.$data));
    },
    methods: {
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
                template: '<p>{{firstName}} {{lastName}} aka {{alias}}</p><button @click="f_destroy">Destroy Profile</button> <br>  ' + _page
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
                    }
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
    data: {
        currentView: 'home'
    },
    created: function () {
        f_log('VUE created ...');
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
        f_destroy: function () {
            this.$destroy();
        }
    }
});

