$(document).ready(function () {
    var role = $("#LoggedUserRole").val();
    if (role == 'superadmin')
        $("#divFileterActivityLogs").removeClass("d-none");

    ActivityLog(false);
});

/*
$('#toggleswitchInfo').change(function () {    
    if (this.checked) {
        ActivityLog(true);
    } else {
        ActivityLog(false);
    }
});
*/

$('input[name="flexRadioDefault"]').on('change', function () {
    // Get the associated label text for the selected radio button
    var selectedLabel = $(this).closest('label').find('.radio-label-span').text().trim();

    // Call ActivityLog based on the label value
    if (selectedLabel === 'Internal') {
        ActivityLog(true);
    } else {
        ActivityLog(false);
    }
});

function ActivityLog(isinternal) {

    // If DataTable is already initialized, destroy it first
    if ($.fn.DataTable.isDataTable('#activityLogDatatable')) {
        $('#activityLogDatatable').DataTable().destroy();
    }

    $("#myOverlay").show();
    $("#activityLogDatatable").DataTable({
        dom: '<"button-container"B>lfrtip',
        buttons: [
            {
                extend: 'excel',
                text: 'Export to Excel',
                exportOptions: {
                    columns: [0, 1, 2, 3, 4, 5, 6]
                },
            },
            {
                extend: 'pdf',
                text: 'Export to PDF',
                exportOptions: {
                    columns: [0, 1, 2, 3, 4, 5, 6],
                    format: {
                        body: function (data, row, column, node) {
                            // Ensure data is a string before replacing <br /> tags                            
                            return String(data).replace(/<br\s*\/?>/g, '\r');
                        }
                    }
                },
                customize: function (doc) {
                    doc.pageOrientation = 'landscape'; // Set the page orientation to landscape
                    doc.content[1].table.widths = ['4%', '10%', '20%', '12%', '8%', '36%', '10%'];

                    // Align all table columns to the left
                    for (var i = 0; i < doc.content[1].table.body.length; i++) {
                        for (var j = 0; j < doc.content[1].table.body[i].length; j++) {
                            let cell = doc.content[1].table.body[i][j];

                            // Ensure alignment for all cells
                            cell.alignment = 'left';

                            // Add left padding to the first column
                            if (j === 0) {
                                cell.margin = [4, 0, 4, 0]; // Left padding
                            }

                            // If the cell contains a stack of text, split by newlines and render each piece separately
                            if (cell.stack) {

                                // Split the text and apply individual styling for each part
                                let text = cell.stack.join('\n').split('\n');
                                cell.stack = text.map(function (textElement) {
                                    return {
                                        text: textElement,
                                        fontSize: 10,           // Smaller font size
                                        lineHeight: 0.2,        // Reduced line height
                                        margin: [0, 0, 0, 1]    // Reduced bottom margin
                                    };
                                });
                            }
                        }
                    }
                }
            }
        ],
        "lengthMenu": [[10, 25, 50, 100, 500, -1], [10, 25, 50, 100, 500, "All"]],
        "bLengthChange": true,
        "processing": true,
        "serverSide": true,
        "filter": true,
        "ajax": {
            "url": "/api/auditlog/list",
            "type": "POST",
            "datatype": "json",
            "data": function (d) {
                // Add any extra parameters to send to the server                
                d.isinternal = isinternal;

            }
        },
        "order": [[0, 'desc']],
        "columnDefs": [{
            "width": 400,
            "targets": 5
        }],
        "columns": [
            { "data": "id", "name": "Id", "autoWidth": true },

            {
                "orderable": false,
                render: function (data, type, row) {
                    if (row.createByUserName == "You")
                        return row.createByUserName;
                    else
                        return "" + row.createByUserName + "<br />(" + row.createdByRole + ")";
                }
            },
            { "data": "userName", "name": "User Name", "autoWidth": true },
            { "data": "userRole", "name": "User Role", "autoWidth": true },
            { "data": "activity", "name": "Activity", "autoWidth": true },
            { "data": "changeDetails", "name": "Changes", "autoWidth": false },
            { "data": "displayDate", "name": "Time Stamp", "autoWidth": true },
        ]
        , "fnDrawCallback": function (data) {
            $("#myOverlay").hide();
            // Below function written in Common.js file
            ShowHideDataTableInfo(data);
        }
    });
    // Hide the DataTable buttons (Excel, PDF, etc.)
    $('.dt-buttons').hide(); // Hide all buttons
}
// Trigger export to Excel when the Excel button is clicked
$('#exportExcel').click(function () {
    $("#activityLogDatatable").DataTable().button('.buttons-excel').trigger();
});

// Trigger export to PDF when the PDF button is clicked
$('#exportPDF').click(function () {
    $("#activityLogDatatable").DataTable().button('.buttons-pdf').trigger();
});

