$(function () {
    $(window).resize(forceResponsiveTables);
    forceResponsiveTables();
});

// There is a lot of field experience data, so this will always make the table responsive.
function forceResponsiveTables() {
    var windowSize = $(window).width();
    if ($('.table-responsive > .table').length > 0) {
        $.each($('.table-responsive > .table'), function (index, value) {
            var thisTable = $(value);
            var tableSize = thisTable.width();
            var parent = thisTable.parent('.table-responsive');
            // 768px is the default for bootstrap 3's responsive-table, modify if needed
            if (windowSize <= 768) {
                parent.css('width', '').css('overflow-x', '').css('overflow-y', '').css('margin-bottom', '').css('border', '');
            } else {
                if (tableSize >= windowSize) {
                    // Change the border color based on the bootstrap theme colors
                    parent.css('width', '100%').css('overflow-x', 'scroll').css('overflow-y', 'hidden').css('margin-bottom', '15px').css('border', '1px solid #DDDDDD');
                } else {
                    parent.css('width', '').css('overflow-x', '').css('overflow-y', '').css('margin-bottom', '').css('border', '');
                }
            }
        });
    }
}

