$(document).ready(function () {

    var lawUserId = '';
    if ($.url('?userid') != "" && $.url('?userid') != undefined) {
        lawUserId = $.url('?userid');
    }
    $("#myOverlay").show();
    $("#lawFirmDatatable").DataTable({
        dom: '<"button-container"B>lfrtip',
        buttons: [
            {
                extend: 'excel',
                text: 'Export to Excel',
                exportOptions: {
                    columns: [1, 2, 3, 4, 5, 6, 7]
                },
            },
            {
                extend: 'pdf',
                text: 'Export to PDF',
                exportOptions: {
                    columns: [1, 2, 3, 4, 5, 6, 7]
                },  
                customize: function (doc) {  
                    doc.pageOrientation = 'landscape'; // Set the page orientation to landscape
                    doc.content[1].table.widths = ['10%', '18%', '12%', '12%', '20%', '16%','12%'];
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
        "ajax": {
            "url": "/api/lawfirm/getLawFirmDetails",
            "type": "POST",
            "datatype": "json"
        },
        "columnDefs": [{
            "target": 0,
            "visible": false,
            "searchable": false
        }
        ],        
        "order": [[8, 'desc']] // Default Sort
        ,"columns": [
            { "data": "id", "name": "ID", "autoWidth": true },            
            { "data": "customUserId", "name": "Id", "autoWidth": true },
            

            {
                render: function (type, row, data) {
                    //return "<i class='far fa-edit cursorPointer'  title = 'Edit Law Firm' onClick=editLawFirm('" + data.id + "');></i> &nbsp;<i class='far fa-eye cursorPointer'  title = 'View Attorney' href='#' onClick=ViewAttorney();></i> &nbsp; <i class='fa fa-download cursorPointer'  title = 'View Pateint Data Download Request' href='#' onClick=ViewDownloadRequest();></i>";
                    return "<a title = 'Edit Law Firm' class='cursorPointer' onClick=editLawFirm('" + data.id + "');>"+data.lawFirmName+"</a> ";


                }
            },

            { "data": "firstName", "name": "Owner First Name", "autoWidth": true },
            { "data": "lastName", "name": "Owner Last Name", "autoWidth": true },
            { "data": "email", "name": "Email", "autoWidth": true },
            { "data": "phoneNumber", "name": "Contact Number", "autoWidth": true },
            { "data": "downloadRequest", "name": "Data Download Requests", "autoWidth": true },

            {
                "data": "createdDate",
                "className": "text-left",
                "render": function (data, type, row) {                    
                    if (type === 'sort' || type === 'type') {
                        // Convert the date to a timestamp for sorting
                        return moment(row.createdDate).unix();
                    } else {                        
                        // Format the date for display
                        return moment(row.createdDate).format('MM/DD/YYYY');
                    }
                }
            }, 

            //{ "data": "displayDate", "name": "Created On", "autoWidth": true },
            {
                "orderable": false
                ,render: function (type, row, data) {
                    //return "<i class='far fa-edit cursorPointer'  title = 'Edit Law Firm' onClick=editLawFirm('" + data.id + "');></i> &nbsp;<i class='far fa-eye cursorPointer'  title = 'View Attorney' href='#' onClick=ViewAttorney();></i> &nbsp; <i class='fa fa-download cursorPointer'  title = 'View Pateint Data Download Request' href='#' onClick=ViewDownloadRequest();></i>";
                    return "<i class='far fa-edit cursorPointer'  title = 'Edit Law Firm' onClick=editLawFirm('" + data.id + "');></i> ";


                }
            }
            
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

// Trigger export to Excel when the Excel button is clicked
$('#exportExcel').click(function () {
    $("#lawFirmDatatable").DataTable().button('.buttons-excel').trigger();
});

// Trigger export to PDF when the PDF button is clicked
$('#exportPDF').click(function () {
    $("#lawFirmDatatable").DataTable().button('.buttons-pdf').trigger();
});

function editLawFirm(userId) {
    window.location.href = "/lawfirm/manage?userId=" + userId;

}
function ViewAttorney() {
    window.location.href = "/lawfirmStaff";
}

function ViewDownloadRequest() {
    window.location.href = "/dataRequest/Index";
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
