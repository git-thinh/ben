function f_get(url) { var r = new XMLHttpRequest(); r.open('GET', url, false); r.send(null); if (r.status === 200) return r.responseText; return ''; }

var _config = {
    layout: {
        name: 'layout',
        padding: 0,
        panels: [
            { type: 'left', size: 200, resizable: true, minSize: 120, style: 'background-color: #edf1f6;overflow: hidden;' },
            {
                type: 'main', overflow: 'hidden',
                style: 'background-color: white; border: 1px solid silver; border-top: 0px; padding: 10px;',
                tabs: {
                    active: 'tab0',
                    tabs: [{ id: 'tab0', caption: '<i class="icon-basic-home"></i>' }],
                    onClick: function (event) {
                        w2ui.layout.html('main', 'Active tab: ' + event.target);
                    },
                    onClose: function (event) {
                        this.click('tab0');
                    }
                }
            }
        ]
    }
};

$(function () {
    // initialization
    $('#main').w2layout(_config.layout);
    //w2ui.layout.content('left', $().w2sidebar(_config.sidebar));

    var tree_htm = f_get('tree.html');
    w2ui.layout.content('left', tree_htm);
    setTimeout(f_tree_Init, 100);
});

function f_tree_Init() {
    $('#tree').jstree()
        .on("changed.jstree", function (e, data) {
            if (data.selected.length) {
                var node = data.instance.get_node(data.selected[0]), text = node.text;
                console.log('The selected node is: ' + text, node);
                f_tab_AddNew(node);
            }
        });
}

function f_tab_AddNew(node) {
    var tab_name = node.text
    var tabs = w2ui.layout_main_tabs;
    if (tabs.get(tab_name)) {
        tabs.select(tab_name);
        w2ui.layout.html('main', 'Tab Selected');
    } else {
        tabs.add({ id: tab_name, caption: tab_name, closable: true });
        w2ui.layout.html('main', 'New tab added');
    }
}