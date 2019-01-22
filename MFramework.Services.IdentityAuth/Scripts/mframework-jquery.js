jQuery.fn.filterable = function (comparerSelector, hidingSelector, placeholder) {
    var el = $(this[0]);
    var searchInp = $('<input type="text" class="form-control" placeholder="' + placeholder + '" />');
    var searchbox = $('<div class="mb-2"></div>');
    searchbox.append(searchInp);
    el.before(searchbox);

    searchInp.keyup(function () {
        var keyword = $(this).val().toLowerCase().trim();
        var items = $(el).find(hidingSelector).hide();

        if (keyword != null && keyword != "") {
            var filtered = items.find(comparerSelector).filter(function (findex) {
                return $(this).text().toLowerCase().indexOf(keyword) > -1;
            });

            filtered.each(function (i, e) {
                $(e).parents(hidingSelector).show();
            });
        }
        else {
            items.show();
        }
    });
};