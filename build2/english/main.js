var f_log = 1 ? console.log.bind(console, 'UI: ') : function () { };
function f_get(url) { var r = new XMLHttpRequest(); r.open('GET', url, false); r.send(null); if (r.status === 200) return r.responseText; return ''; }

// #region [ LAYOUT ]

var _config = {
    layout: {
        name: 'layout',
        padding: 0,
        panels: [
            { type: 'left', size: 200, resizable: true, minSize: 120, style: 'overflow: hidden;' },
            {
                type: 'main', overflow: 'hidden',
                style: 'background-color: white; border: 1px solid silver; border-top: 0px; padding: 0px;',
                tabs: {
                    active: 'tab0',
                    tabs: [{ id: 'tab0', caption: '<i class="icon-basic-home"></i>' }],
                    onClick: function (event) {
                        //w2ui.layout.html('main', '<div id=tab_view><div id=tab_content> Active tab: ' + event.target + '</div><div id=tab_sidebar>00</div></div>');
                        f_log('Active tab: ' + event.target);
                    },
                    onClose: function (event) {
                        this.click('tab0');
                    }
                }
            },
            //{ type: 'right', size: 200, resizable: true, style: '', content: 'right' }
        ]
    },
    sidebar: {
        name: 'sidebar',
        flatButton: false,
        nodes: [
            { id: 'level-1-2', text: 'Level 1.2', icon: 'fa fa-coffee', selected: true },
            { id: 'level-1-3', text: 'Level 1.3', icon: 'fa fa-comment-o' }
            //{
            //    id: 'level-1', text: 'Level 1', img: 'icon-folder', expanded: true, group: true, groupShowHide: false,
            //    nodes: [{ id: 'level-1-1', text: 'Level 1.1', icon: 'fa fa-home' },
            //    { id: 'level-1-2', text: 'Level 1.2', icon: 'fa fa-coffee', selected: true },
            //    { id: 'level-1-3', text: 'Level 1.3', icon: 'fa fa-comment-o' }
            //    ]
            //},
            //{
            //    id: 'level-2', text: 'Level 2', img: 'icon-folder', group: true,
            //    nodes: [{ id: 'level-2-1', text: 'Level 2.1', icon: 'fa fa-star-o' },
            //    { id: 'level-2-2', text: 'Level 2.2', icon: 'fa fa-star-o' },
            //    { id: 'level-2-3', text: 'Level 2.3', icon: 'fa fa-star-o' }
            //    ]
            //}
        ],
        onFlat: function (event) {
            $('#sidebar').css('width', (event.goFlat ? '35px' : '200px'));
        }
    }
};

$(function () {
    // initialization
    $('#main').w2layout(_config.layout);
    //w2ui.layout.content('left', $().w2sidebar(_config.sidebar));

    var tree_htm = f_get('view/tree.html');
    w2ui.layout.content('left', tree_htm);
    setTimeout(f_tree_Init, 100);

    //w2ui.layout.content('right', $().w2sidebar(_config.sidebar));
});

function f_tree_Init() {
    $('#tree').jstree().on("changed.jstree", function (e, data) {
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
        //w2ui.layout.html('main', 'New tab added');
        w2ui.layout.html('main', '<div id=tab_view><div id=tab_content> Add new tab: ' + tab_name + '</div><div id=tab_sidebar>00</div></div>');
        setTimeout(function () {
            $('#tab_sidebar').w2sidebar(_config.sidebar);
        }, 100);
    }
}

// #endregion
