// Please see documentation at https://docs.microsoft.com/aspnet/core/client-side/bundling-and-minification
// for details on configuring this project to bundle and minify static web assets.

// Write your JavaScript code.
$(document).ready(function () {
    $("#CustomerFullName").autocomplete({
        source: function (request, response) {
            $.ajax({
                url: '/Customers/Search',
                type: 'GET',
                cache: false,
                data: request,
                dataType: 'json',
                success: function (data) {
                    response($.map(data, function (item) {
                        return {
                            label: item.label,
                            value: item.id
                        }
                    }))
                }
            });
        },
        minLength: 2,
        select: function (event, ui) {
            $('#CustomerFullName').val(ui.item.label);
            $('#CustomerId').val(ui.item.value);
            return false;
        }
    });
});