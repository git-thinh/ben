﻿function f_spreadsheet_Init(selector_grid_spreadsheet) {
    var _config = {
        name: 'grid_spreadsheet',
        selectType: 'cell',
        show: {
            footer: true,
            lineNumbers: true
        },
        onColumnClick: function (event) {
            this.selectNone();
            var column = this.getColumn(event.field, true);
            var sel = [];
            for (var i = 0; i < this.total; i++) {
                sel.push({ recid: this.records[i].recid, column: column });
            }
            this.select.apply(this, sel);
            event.preventDefault();
        },
        onSelectionExtend: function (event) {
            event.onComplete = function () {
                console.log('Original:', event.originalRange[0], '-', event.originalRange[1]);
                console.log('New:', event.newRange[0], '-', event.newRange[1]);
                w2alert('Selection expanded (see more details in the console).');
            }
        },
        onDelete: function (event) {
            event.force = true;
        }
    };

    //$(function () {
    // initialization
    $().w2grid(_config);

    // create columns
    var tmp = 'abcdefghijklmnoprst';
    var values = {};
    for (var i = 0; i < tmp.length; i++) {
        w2ui.grid.columns.push({
            field: tmp[i],
            caption: '<div style="text-align: center">' + tmp[i].toUpperCase() + '</div>',
            size: '15%',
            resizable: true,
            sortable: true,
            editable: { type: 'd' },
            render: function (record, index, col_index) {
                var field = this.columns[col_index].field;
                var val = record[field];
                var style = '';
                if (record.changed && record.changes[field]) val = record.changes[field];
                if (record.style && record.style[col_index]) style = record.style[col_index];
                return '<div style="' + style + '; padding: 6px 3px; height: ' + this.recordHeight + 'px">' + val + '</div>';
            }
        });
        values[tmp[i]] = '';
    }
    // insert  records
    for (var i = 0; i < 100; i++) {
        w2ui.grid.records.push($.extend({ recid: i + 1 }, values));
    }
    w2ui.grid.total = w2ui.grid.records.length;
    w2ui.grid.buffered = w2ui.grid.total;
    // init some values
    var rec = w2ui.grid.records;
    $.extend(rec[2], { c: 1, d: 67, f: 'some', g: 'text' });
    $.extend(rec[3], { c: 2, d: 35, f: 'more', g: 'text' });
    $.extend(rec[4], { c: 3, d: 17 });
    $.extend(rec[5], { c: 4, d: 84 });
    $.extend(rec[6], { c: 5, d: 14 });
    // display grid
    //$('#main').w2render('grid_spreadsheet');
    $(selector_grid_spreadsheet).w2render('grid_spreadsheet');
    //});


    /**
    *  This extands grid with a function that already part in 1.4.nightly
    */

    w2ui.grid.refreshRow = function (recid) {
        var tr = $('#grid_' + this.name + '_rec_' + w2utils.escapeId(recid));
        if (tr.length != 0) {
            var ind = this.get(recid, true);
            var line = tr.attr('line');
            // if it is searched, find index in search array
            var url = (typeof this.url != 'object' ? this.url : this.url.get);
            if (this.searchData.length > 0 && !url) for (var s in this.last.searchIds) if (this.last.searchIds[s] == ind) ind = s;
            $(tr).replaceWith(this.getRecordHTML(ind, line));
        }

    }
}

function mark() {
    w2ui.grid.addRange({
        name: 'range',
        range: [{ "recid": 3, "column": 2 }, { "recid": 7, "column": 3 }],
        style: "border: 2px dotted green; background-color: rgba(100,400,100,0.2)"
    });
}


function cellFormat(flag) {
    var style = '';
    if (flag) {
        w2ui.grid.get(3).style = { 5: 'color: white; background-color: #8BC386', 6: 'color: black; background-color: #BFE6FF' };
        w2ui.grid.get(4).style = { 5: 'color: white; background-color: #8BC386', 6: 'color: black; background-color: #BFE6FF' };
    } else {
        w2ui.grid.get(3).style = { 5: '', 6: '' };
        w2ui.grid.get(4).style = { 5: '', 6: '' };
    }
    w2ui.grid.refreshRow(3);
    w2ui.grid.refreshRow(4);
}