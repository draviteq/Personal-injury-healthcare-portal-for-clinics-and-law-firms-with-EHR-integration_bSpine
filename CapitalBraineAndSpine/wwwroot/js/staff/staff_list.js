$(document).ready(function () {


    if ($("#inptRole").val() == 'staff' || $("#inptRole").val() == 'superadmin')
        $("#ancAddStaff").show();
    else
        $("#ancAddStaff").hide();

    $("#myOverlay").show();
    $("#lawFirmStaffDatatable").DataTable({
        dom: '<"button-container"B>lfrtip',
        buttons: [
            {
                extend: 'excel',
                text: 'Export to Excel',
                exportOptions: {
                    columns: [1, 2, 3, 4, 5]
                },
            },
            {
                extend: 'pdf',
                text: 'Export to PDF',
                exportOptions: {
                    columns: [1, 2, 3, 4, 5]
                },
                customize: function (doc) {
                    doc.content[1].table.widths = ['15%', '20%', '10%', '35%', '15%'];
                    // Align all the table columns to the left and add left padding to the first column
                    for (var i = 0; i < doc.content[1].table.body.length; i++) {
                        for (var j = 0; j < doc.content[1].table.body[i].length; j++) {
                            doc.content[1].table.body[i][j].alignment = 'left'; // Align text to the left

                            // Add left padding to the first column only
                            if (j === 0) {
                                doc.content[1].table.body[i][j].margin = [8, 0, 0, 0]; // Left padding of 5, adjust as needed
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
        "order": [[5, 'desc']] // Default Sort
        , "ajax": {
            "url": "/api/staff/stafflist",
            "type": "POST",
            "datatype": "json"
        },
        "columnDefs": [{
            "target": 0,
            "visible": false,
            "searchable": false
        }],
        "columns": [
            { "data": "id", "name": "ID", "autoWidth": true },
            { "data": "customUserId", "name": "Staff Id", "autoWidth": true },
            {
                render: function (type, row, data) {
                    //return "<i class='far fa-edit cursorPointer' data-bs-toggle='tooltip' data-bs-custom-class='tooltip-primary' data-bs-placement='Right' title = 'Edit Law Firm' onClick=editLawFirm('" + data.id + "');></i> &nbsp;<i class='far fa-eye cursorPointer' data-bs-toggle='tooltip' data-bs-custom-class='tooltip-primary' data-bs-placement='Right' title = 'View Attorney' href='#' onClick=ViewAttorney();></i> &nbsp; <i class='fa fa-download cursorPointer' data-bs-toggle='tooltip' data-bs-custom-class='tooltip-primary' data-bs-placement='Right' title = 'View Pateint Data Download Request' href='#' onClick=ViewDownloadRequest();></i>";
                    return "<a title = 'Edit Staff' class='cursorPointer' onClick=editStaff('" + data.id + "');>" + data.firstName + ' ' + data.lastName + "</a> ";


                }
            },
            { "data": "role", "name": "Role", "autoWidth": true },
            { "data": "email", "name": "Email", "autoWidth": true },
            //{ "data": "displayDate", "name": "Added On", "autoWidth": true },
            {
                "data": "addedOn",
                "className": "text-left",
                "render": function (data, type, row) {
                    if (type === 'sort' || type === 'type') {
                        // Convert the date to a timestamp for sorting
                        return moment(row.addedOn).unix();
                    } else {
                        // Format the date for display
                        return moment(row.addedOn).format('MM/DD/YYYY');
                    }
                }
            },
            {
                render: function (type, row, data) {
                    return "<i class='far fa-edit cursorPointer' data-bs-toggle='tooltip' data-bs-custom-class='tooltip-primary' data-bs-placement='Right' title = 'Edit Staff' onClick=editStaff('" + data.id + "');></i>";
                }
            },
        ]
        , "fnDrawCallback": function (data) {
            $("#myOverlay").hide();
            // Below function written in Common.js file
            ShowHideDataTableInfo(data);
        }

    });
    // Hide the DataTable buttons (Excel, PDF, etc.)
    $('.dt-buttons').hide(); // Hide all buttons

});
function editStaff(userId) {
    window.location.href = "/staff/manage?userId=" + userId;

}
$("#updatePermission").on('click', function () {
    $("#myOverlay").show();
    $.ajax({
        type: "POST",
        url: '/api/lawFirm/updatePermissions',
        data: $('#permissionform').serialize(),
        //dataType: 'json',
        success: function () {
            $("#myOverlay").hide();
            showToast('success', "Permissions updated successfully!", 'bg-success');
        },
        error: function (xhr, status, error) {
            $("#myOverlay").hide();
            showToast('error', "Something went Wrong!", 'bg-danger');
        }
    })
});
// Trigger export to Excel when the Excel button is clicked
$('#exportExcel').click(function () {
    $("#lawFirmStaffDatatable").DataTable().button('.buttons-excel').trigger();
});

// Trigger export to PDF when the PDF button is clicked
$('#exportPDF').click(function () {
    $("#lawFirmStaffDatatable").DataTable().button('.buttons-pdf').trigger();
});
