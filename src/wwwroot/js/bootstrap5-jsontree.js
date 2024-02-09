
class BS5JsonTreeView {
    treeData = null;
    parentId = 'tree';
    itemId = 0;

    constructor(parentId = null, treeData = null) {
        this.treeData = treeData;
        if(this.treeData==null)  this.treeData = this.getExampleData();
        if(parentId!=null) {
            this.parentId = parentId;
            this.load();
        }
    }

    setParentId(parentId) {
        this.parentId = parentId;
    }

    setDataSource(treeData) {
        this.treeData = treeData;
    }

    loadUrl(url) {
        var self = this;
        $.ajax({
            url: url,
            method: 'get',
            async: false,
            data: {
                sourceType: 'json'
            },
            success: function (response) {
                self.clear();
                self.load(response);
            }
        });
    }

    clear() {
        $('#' + this.parentId).empty();
        this.treeData = null;
        this.itemId = 0;
    }

    load(treeData = null) {
        var self = this;
        if(this.parentId==null) return;
        if(treeData!=null) this.treeData = treeData;
        if(this.treeData==null)  this.treeData = this.getExampleData();

        $('#' + this.parentId).append($('<p>', {
            id: self.parentId + "-title",
            html: self.treeData.Data.html,
            title: self.treeData.Data.title,
            style: "font-weight: bold; margin-bottom: 0;",
            class: "treeview-title"
        })); //add title to the tree
        
        this.addItem($('#' + this.parentId), this.treeData.Data); //first call to add item which passes the main parent/root.

        $('#' + this.parentId + ' :checkbox').change(function () {
            if ($(this).is(':checked')) {
                $(this).closest('li').children('ul').show();
                $(this).closest('li').find(':checkbox').prop('checked', true);
            }
        });

        $('#' + this.parentId + ' label,i').click(function(e){
            $(this).closest('li').children('ul').each(function(){
                if($(this).hasClass('treeitem-open')) {
                    $(this).hide().removeClass('treeitem-open').addClass('treeitem-closed');
                    $(this).closest('li').children('i').removeClass('fa-square-minus').addClass('fa-square-plus');
                } else {
                    $(this).show().removeClass('treeitem-closed').addClass('treeitem-open');
                    $(this).closest('li').children('i').removeClass('fa-square-plus').addClass('fa-square-minus');
                }
            });
            e.stopPropagation();
        });
    }

    addItem(parentUL, branch) {
        var self = this;
        $.each(branch.children, function(i) {
            self.itemId++;
            var item = branch.children[i]; //assign each child in variable item
            var $item = $('<li>', {        //jquery object
                id: self.parentId + "-node-" + self.itemId,
                style: "list-style: none;",
                html: item.children ? '<i class="fa-regular fa-square-minus" style="padding-right: 5px;"></i>' : '&nbsp;&nbsp;&nbsp;',
                class: "treeview-node"
            });
            $item.append($('<input>', { //add check boxes
                type: "checkbox",
                id: self.parentId + "-checkbox-" + self.itemId,
                name: self.parentId + "-checkbox-" + self.itemId,
                class: "form-check-input treeview-checkbox",
            })); //.attr("data-level", item.level));
            var style="padding-left: 5px;";
            if(item.children) {
                style += "cursor: pointer;"; //if there are children, change the cursor to pointer.
            }
            $item.append($('<label>', { //add labels to HTML. For every id, display its title.
                html: item.html,
                title: item.title,
                id: self.parentId + "-label-" + self.itemId,
                class: "form-check-label treeview-label" + (item.children ? "" : " treeview-leaf"),
                style: style
            }));
            parentUL.append($item);
            if (item.children) {
                var $ul = $('<ul>', {
                    id: self.parentId + "-ul-" + self.itemId,
                    class: "treeview-branch treeitem-open",
                    style: "padding-left: 1rem;"
                }).appendTo($item);
                self.addItem($ul, item); //recursive call to add another item if there are more children.
            }
        });
    };

    collapseAll() {
        $('#' + this.parentId + ' ul').hide().removeClass('treeitem-open').addClass('treeitem-closed');
        $('#' + this.parentId + ' i').removeClass('fa-square-minus').addClass('fa-square-plus');
    }

    expandAll() {
        $('#' + this.parentId + ' ul').show().removeClass('treeitem-closed').addClass('treeitem-open');
        $('#' + this.parentId + ' i').removeClass('fa-square-plus').addClass('fa-square-minus');
    }

    getExampleData() {
        return {
            "Data": {
                "html": "Root",
                "title": "Root",
                "level": 0,
                "children": [
                    {
                        "html": "Child 1",
                        "title": "Child 1",
                        "level": 1,
                        "children": [
                            {
                                "html": "Grandchild 1",
                                "title": "Grandchild 1",
                                "level": 2,
                                "children": [
                                    {
                                        "html": "Great-Grandchild 1",
                                        "title": "Great-Grandchild 1",
                                        "level": 3
                                    },
                                    {
                                        "html": "Great-Grandchild 2",
                                        "title": "Great-Grandchild 2",
                                        "level": 3
                                    }
                                ]
                            }
                        ]
                    },
                    {
                        "html": "Child 2",
                        "title": "Child 2",
                        "level": 1,
                        "children": [
                            {
                                "html": "Grandchild 1",
                                "title": "Grandchild 1",
                                "level": 2
                            }
                        ]
                    }
                ]
            }
        };
    }

};