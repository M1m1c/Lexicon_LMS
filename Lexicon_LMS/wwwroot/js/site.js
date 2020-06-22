﻿// Please see documentation at https://docs.microsoft.com/aspnet/core/client-side/bundling-and-minification
// for details on configuring this project to bundle and minify static web assets.

// Write your JavaScript code.


$(document).ready(function () {
    $('#example').dataTable({
        "order": [],
        "columnDefs": [{
            "targets": 'no-sort',
            "orderable": false,
            "searchable": false,
        }]
    });

});

$(document).ready(function () {
    $("#addComments").click(function () {
        $("#commentForm").toggle('slow');
    });
});

function RemoveForm() {
    $("#commentForm").hide();
};

function Fail(result) {
    $("#commentForm").hide();
}

