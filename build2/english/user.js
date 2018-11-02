
function f_user_toggleClick() {

};

function f_user_logoutConfirm() {
    w2confirm('Are you sure logout?')
        .yes(function () {
            console.log('YES');
        })
        .no(function () {
            console.log('NO');
        });
}

function f_user_loginOpen() {
    var fom_name = 'login_form', fom_title = 'Login System';
    if (!w2ui[fom_name]) {
        $().w2form({
            name: fom_name,
            style: 'border: 0px; background-color: transparent;',
            formHTML: f_get('view/login.html'),
            fields: [
                { field: 'username', type: 'text', required: true },
                { field: 'password', type: 'password', required: true }
            ],
            record: {
                username: 'admin',
                password: 'admin'
            },
            actions: {
                "login": function () {
                    this.validate();
                },
                "cancel": function () {
                    this.clear();
                }
            }
        });
    }
    $().w2popup('open', {
        title: fom_title,
        body: '<div id="form" style="width: 100%; height: 100%;"></div>',
        style: 'padding: 15px 0px 0px 0px',
        width: 500,
        height: 300,
        showMax: true,
        onToggle: function (event) {
            $(w2ui[fom_name].box).hide();
            event.onComplete = function () {
                $(w2ui[fom_name].box).show();
                w2ui[fom_name].resize();
            }
        },
        onOpen: function (event) {
            event.onComplete = function () {
                // specifying an onOpen handler instead is equivalent to specifying an onBeforeOpen handler, which would make this code execute too early and hence not deliver.
                $('#w2ui-popup #form').w2render(fom_name);
            }
        }
    });
}

function f_user_infoOpen() {
    var fom_name = 'user_info', fom_title = 'User Infomations';
    if (!w2ui[fom_name]) {
        $().w2form({
            name: fom_name,
            style: 'border: 0px; background-color: transparent;',
            formHTML: f_get('view/user_info.html'),
            fields: [
                { field: 'first_name', type: 'text', required: true },
                { field: 'last_name', type: 'text', required: true },
                { field: 'comments', type: 'text' },
                { field: 'address1', type: 'text', required: true },
                { field: 'address2', type: 'text' },
                { field: 'city', type: 'text', required: true },
                { field: 'state', type: 'text', required: true },
                { field: 'zip', type: 'int', required: true },
                { field: 'short_bio', type: 'text' },
                { field: 'talk_name', type: 'text', required: true },
                { field: 'description', type: 'text' }
            ],
            tabs: [
                { id: 'tab1', caption: 'Change Password' },
                { id: 'tab2', caption: 'Cloud Credentials' },
                { id: 'tab3', caption: 'General' },
                { id: 'tab4', caption: 'Address' },
                { id: 'tab5', caption: 'About' }
            ],
            actions: {
                reset: function () {
                    this.clear();
                },
                save: function () {
                    this.save();
                }
            }
        });
    }
    $().w2popup('open', {
        title: fom_title,
        body: '<div id="form" style="width: 100%; height: 100%;"></div>',
        style: 'padding: 0px 0px 0px 0px',
        width: 500,
        height: 300,
        showMax: true,
        onToggle: function (event) {
            $(w2ui[fom_name].box).hide();
            event.onComplete = function () {
                $(w2ui[fom_name].box).show();
                w2ui[fom_name].resize();
            }
        },
        onOpen: function (event) {
            event.onComplete = function () {
                // specifying an onOpen handler instead is equivalent to specifying an onBeforeOpen handler, which would make this code execute too early and hence not deliver.
                $('#w2ui-popup #form').w2render(fom_name);
                w2popup.toggle();
            }
        }
    });
}
