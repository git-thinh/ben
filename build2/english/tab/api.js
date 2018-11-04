
function f_api_publish_Init(selector) {
    $(selector).w2grid({
        style: 'border-left:none;border-top:none;border-bottom:none;',
        show: {
            header: false,
            columnHeaders: true
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

function f_api_data_Init(selector) {
    $(selector).w2grid({
        name: 'grid',
        style: 'border-top:none;border-bottom:none;',
        columns: [
            { field: 'fname', caption: 'First Name', size: '150px', frozen: true },
            { field: 'lname', caption: 'Last Name', size: '150px' },
            { field: 'email', caption: 'Email', size: '200px' },
            { field: 'sdate', caption: 'Start Date', size: '200px' }
        ],
        "records": [
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
