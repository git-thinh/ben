
function f_api_publish_Init() {
    $('#tab_content').w2grid({
        style: 'border-left:none;border-right:none;border-top:none;border-bottom:none;',
        show: {
            header: false,
            toolbar: false,
            toolbarSearch: false,
            toolbarInput: false,
            searchAll: false,
            toolbarColumns: false,
            toolbarReload: false,
            lineNumbers: true,
            selectColumn: false,
            expandColumn: false,
            footer: true
        },
        name: 'grid2',
        columns: [
            { field: 'name', caption: 'Name', size: '100px', style: 'background-color: #efefef; border-bottom: 1px solid white; padding-right: 5px;', attr: "align=right" },
            { field: 'value', caption: 'Value', size: '100%' }
        ],
        records: [
            { recid: 0, name: 'Key', value: 1 },
            { recid: 1, name: 'Port', value: 'Mr Thinh' },
            { recid: 2, name: 'Name', value: 'Master' },
            { recid: 3, name: 'Description', value: 'thinhifis@gmail.com' },
            { recid: 4, name: 'Date Create', value: '1/1/2018' }
        ]
    });
}

function f_api_setting_Init() {

}

function f_api_data_Init() {
    $('#tab_toolbar').w2toolbar({
        name: 'tab_toolbar',
        style: 'background-color:#fff;border:none;',
        tooltip: 'bottom',
        items: [
            {
                type: 'menu', id: 'menu_action', text: 'Data', icon: 'fa fa-bars', items: [
                    { id: 'api_publish', text: 'Publish', icon: 'fa fa-bolt' },
                    { id: 'api_setting', text: 'Setting', icon: 'icon-basic-settings' },
                    { id: 'api_data', text: 'Data', icon: 'icon-basic-server2', count: 5 },
                    { id: 'api_filter', text: 'Filter', icon: 'icon-basic-pin1' },
                    { id: 'api_menu', text: 'Menu', icon: 'icon-basic-share' },
                    { id: 'api_permission', text: 'Permission', icon: 'icon-basic-key', disabled: true },
                ]
            },
            {
                type: 'html', id: 'item5', tooltip: 'Search',
                html: function (item) {
                    var html =
                        '<div style="padding: 3px 10px;">' +
                        //' CUSTOM:' +
                        '    <input size="10" onchange="var el = w2ui.toolbar.set(\'item5\', { value: this.value });" ' +
                        '         style="padding: 3px; border-radius: 2px; border: 1px solid silver" value="' + (item.value || '') + '"/>' +
                        '</div>';
                    return html;
                }
            },
            //{ type: 'break' },
            //{
            //    type: 'menu-radio', id: 'item2', icon: 'fa fa-bars', tooltip: 'Select Actions',
            //    text: function (item) {
            //        var text = item.selected;
            //        var el = this.get('item2:' + item.selected);
            //        return el.text;
            //    },
            //    selected: 'id3',
            //    items: [
            //        { id: 'id1', text: 'Action 1', icon: 'fa fa-camera' },
            //        { id: 'id2', text: 'Action 2', icon: 'fa fa-picture-o' },
            //        { id: 'id3', text: 'Action 3', icon: 'fa fa-glass', count: 12 }
            //    ]
            //},
            { type: 'break' },
            {
                type: 'menu-check', id: 'item3', text: '', icon: 'fa fa-filter', tooltip: 'Select filters',
                selected: ['id3', 'id4'],
                onRefresh: function (event) {
                    event.item.count = event.item.selected.length;
                },
                items: [
                    { id: 'id1', text: 'Item 1', icon: 'fa fa-camera' },
                    { id: 'id2', text: 'Item 2', icon: 'fa fa-picture-o' },
                    { id: 'id3', text: 'Item 3', icon: 'fa fa-glass', count: 12 },
                    { text: '--' },
                    { id: 'id4', text: 'Item 4', icon: 'fa fa-glass' }
                ]
            },
            { type: 'break' },
            { id: 'tb_color', type: 'color' },
            { id: 'tb_text_color', type: 'text-color', transparent: false },
            { type: 'spacer' },
            {
                type: 'drop', id: 'item4', text: '', icon: 'fa fa-comment-o', tooltip: 'Notification Messages', count: 12,
                html: '<div style="padding: 10px; line-height: 1.5">You can put any HTML in the drop down.<br>Include tables, images, etc.</div>'
            },
            { type: 'button', id: 'item6', text: '', icon: 'fa fa-trash-o' },
            { type: 'button', id: 'item7', text: '', icon: 'fa fa-floppy-o' },
        ],
        onClick: function (event) {
            console.log('Selected:', event.item.selected);
            if (event.color == null) return;
            event.done(function () {
                console.log(event.item.type, ':', event.item.color);
            });
        },
    });

    //w2ui.toolbar.on('*', function (event) {
    //    console.log('EVENT: ' + event.type + ' TARGET: ' + event.target, event);
    //});

    $('#tab_content').w2grid({
        name: 'grid',
        style: 'border-left:none;border-top:1px solid #ccc;border-bottom:none;border-right:none;',
        show: {
            header: false,
            toolbar: false,
            toolbarSearch: false,
            toolbarInput: false,
            searchAll: false,
            toolbarColumns: false,
            toolbarReload: false,
            lineNumbers: true,
            selectColumn: false,
            expandColumn: false,
            footer: true
        },
        columns: [
            { field: 'fname', caption: 'First Name', size: '150px', frozen: true },
            { field: 'lname', caption: 'Last Name', size: '150px' },
            { field: 'email', caption: 'Email', size: '200px' },
            { field: 'sdate', caption: 'Start Date', size: '200px' }
        ],
        records: [
            { "recid": 1, "fname": "John", "lname": "Doe", "email": "jdoe@gmail.com", "sdate": "4/3/2012" },
            { "recid": 2, "fname": "Stuart", "lname": "Motzart", "email": "jdoe@gmail.com", "sdate": "4/3/2012" },
            { "recid": 3, "fname": "Jin", "lname": "Franson", "email": "jdoe@gmail.com", "sdate": "4/3/2012" },
            { "recid": 4, "fname": "Susan", "lname": "Ottie", "email": "jdoe@gmail.com", "sdate": "4/3/2012" },
            { "recid": 5, "fname": "Kelly", "lname": "Silver", "email": "jdoe@gmail.com", "sdate": "4/3/2012" },
            { "recid": 6, "fname": "Francis", "lname": "Gatos", "email": "jdoe@gmail.com", "sdate": "4/3/2012" },
            { "recid": 7, "fname": "Mark", "lname": "Welldo", "email": "jdoe@gmail.com", "sdate": "4/3/2012" },
            { "recid": 8, "fname": "Thomas", "lname": "Bahh", "email": "jdoe@gmail.com", "sdate": "4/3/2012" },
            { "recid": 9, "fname": "Sergei", "lname": "Rachmaninov", "email": "jdoe@gmail.com", "sdate": "4/3/2012" }
        ]
    });
}

function f_api_filter_Init() {

}

function f_api_menu_Init() {

}

function f_api_permission_Init() {

}
