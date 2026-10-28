
"use strict";
const stripe = Stripe("stripe-key");
var lawFirmUserId = '';
$(document).ready(function () {

    $("#myOverlay").show();
    if ($.url('?userId') != "" && $.url('?userId') != undefined) {
        lawFirmUserId = $.url('?userId');
    }
    var role = $("#LoggedUserRole").val();
    if (role == "lawfirm") {
        $("#dataRequestDatatable").DataTable({
            /* dom: 'Blfrtip',*/
            dom: '<"button-container"B>lfrtip',
            buttons: [
                {
                    extend: 'excel',
                    text: '<i class="bi bi-upload me-2"></i> Export to Excel',
                    className: 'btn btn-outline-light btn-wave waves-effect waves-light me-2 mt-2',
                    exportOptions: {
                        columns: [0, 2, 3, 4, 5]
                    },
                },
                {
                    extend: 'pdf',
                    text: '<i class="bi bi-upload me-2"></i> Export to PDF',
                    className: 'btn btn-outline-light btn-wave waves-effect waves-light mt-2',
                    exportOptions: {
                        columns: [0, 2, 3, 4, 5]
                    },
                    customize: function (doc) {
                        doc.content[1].table.widths = ['5%', '15%', '20%', '20%', '15%', '15%','10%'];
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
            initComplete: function () {
                // Remove the default DataTables button classes
                $('.dt-button').removeClass('dt-button buttons-html5 buttons-excel buttons-pdf');
            },
            "lengthMenu": [[10, 25, 50, 100, 500, -1], [10, 25, 50, 100, 500, "All"]],
            "bLengthChange": true,
            "processing": true,
            "serverSide": true,
            "filter": true,
            "autoWidth": false,
            "ajax": {
                "url": "/api/datarequest/getdatarequestdetails",
                "type": "POST",
                "datatype": "json",
                "data": function (d) {
                    // Add any extra parameters to send to the server                
                    d.lawfirmUserId = lawFirmUserId;

                }

            }
            , "columnDefs": [{
                "target": 1,
                "visible": false,
                "searchable": false
            }],

            "columns": [
                { "data": "id", "name": "Request Id", "autoWidth": true },
                { "data": "lawFirmName", "name": "Law Firm Name", "autoWidth": true },
                { "data": "patientName", "name": "Patient Name", "autoWidth": true },

                { "data": "createdDate", "name": "Created Date", "autoWidth": true },
                { "data": "lastUpdated", "name": "Last Updated", "autoWidth": true },
                { "data": "statusName", "name": "Status", "autoWidth": true },
                { "data": "paymentStatusName", "name": "Payment Status", "autoWidth": true },
                {
                    "orderable": false,
                    render: function (type, row, data) {
                        if (data.paymentStatusId == 2) {
                            return "<button class='btn btn-outline-warning btn-sm btn-wave'  title = 'View patient data from Tebra' onclick=ViewData('" + data.tebraPatientId + "'); >View Data</button> <button Class='btn btn-outline-success btn-sm btn-wave ms-3'  id='ancDownload' data-patientId='" + data.tebraPatientId + "' title='Download patient details to PDF'  onclick='DownloadPatientPDF(event)'>Download PDF</button>";
                        }
                        else if (data.statusName == "Accepted" || data.statusName == "Accepted and Paid") {

                            return "<button Class='btn btn-outline-success btn-sm btn-wave ms-3'  id='ancMakePayment' data-id='" + data.id + "' data-userId='" + data.userId + "' title='Make payment to access patient details'  onclick='MakePayment(event)'>Make Payment</button>";
                        }
                        else if (data.statusName == "Rejected") {
                            return "<button class='btn btn-outline-primary btn-sm btn-wave'  title = 'View Request Rejection Reason' data-rejectionDate='" + data.lastUpdated + "' data-arg='" + data.reason + "' onclick='ViewReason(event);'>View Reason</button>";
                        }
                        else {
                            return "<label class='form-label text-primary'  title = 'Request pending from Admin Side'); >Pending for approval</label>";
                        }
                    }
                },

            ]
            , "fnDrawCallback": function (data) {

                // Below function written in Common.js file
                ShowHideDataTableInfo(data);
                $("#myOverlay").hide();
            }
        });
    }
    else {
        $("#dataRequestDatatable").DataTable({
            /* dom: 'Blfrtip',*/
            dom: '<"button-container"B>lfrtip',
            buttons: [
                {
                    extend: 'excel',
                    text: '<i class="bi bi-upload me-2"></i> Export to Excel',
                    className: 'btn btn-outline-light btn-wave waves-effect waves-light me-2',
                    exportOptions: {
                        columns: [0, 1, 2, 3, 4, 5]
                    },
                },
                {
                    extend: 'pdf',
                    text: '<i class="bi bi-upload me-2"></i> Export to PDF',
                    className: 'btn btn-outline-light btn-wave waves-effect waves-light',
                    exportOptions: {
                        columns: [0, 1, 2, 3, 4, 5]
                    },
                    customize: function (doc) {
                        doc.content[1].table.widths = ['10%', '20%', '20%', '20%', '15%', '15%'];
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
            initComplete: function () {
                // Remove the default DataTables button classes
                $('.dt-button').removeClass('dt-button buttons-html5 buttons-excel buttons-pdf');
            },
            "lengthMenu": [[10, 25, 50, 100, 500, -1], [10, 25, 50, 100, 500, "All"]],
            "bLengthChange": true,
            "processing": true,
            "serverSide": true,
            "filter": true,
            "autoWidth": false,
            "ajax": {
                "url": "/api/datarequest/getdatarequestdetails",
                "type": "POST",
                "datatype": "json",
                "data": function (d) {
                    // Add any extra parameters to send to the server                
                    d.lawfirmUserId = lawFirmUserId;

                }
            },

            "columns": [
                { "data": "id", "name": "Request Id", "autoWidth": true },
                //{ "data": "lawFirmName", "name": "Law Firm Name", "autoWidth": true },
                {

                    render: function (type, row, data) {
                        return "<a title='Edit Law Firm' class='cursorPointer' href='/lawfirm/manage?userId=" + data.lawFirmUserId + "'>" + data.lawFirmName + "</a>";
                    }

                },

                { "data": "patientName", "name": "Patient Name", "autoWidth": true },
                { "data": "createdDate", "name": "Created Date", "autoWidth": true },
                { "data": "lastUpdated", "name": "Last Updated", "autoWidth": true },
                { "data": "statusName", "name": "Status", "autoWidth": true },
                {
                    "orderable": false,
                    render: function (type, row, data) {
                        if (data.userRole == "superadmin" || data.userRole == "staff" || data.userRole == "admin") {
                            if (data.statusName == "Accepted" || data.statusName == "Accepted and Paid") {
                                return "<button class='btn btn-outline-warning btn-sm btn-wave'  title = 'View patient data from Tebra' onclick=ViewData('" + data.tebraPatientId + "'); >View Data</button>  <button Class='btn btn-outline-success btn-sm btn-wave ms-3'  id='ancDownload' data-patientId='" + data.tebraPatientId + "' title='Download patient details to PDF'  onclick='DownloadPatientPDF(event)'><i class='fas fa-file-pdf fa-2x'></i> Download</button>"

                            }
                            else if (data.statusName == "Rejected") {
                                return "<button class='btn btn-outline-warning btn-sm btn-wave'  title = 'View patient data from Tebra' onclick=ViewData('" + data.tebraPatientId + "'); >View Data</button>&nbsp;<button  class='btn btn-outline-primary btn-sm btn-wave ms-3'  title = 'View Request Rejection Reason' data-rejectionDate='" + data.lastUpdated + "' data-arg='" + data.reason + "' onclick=ViewReason(event); >View Reason</button>";
                            }
                            else {
                                return "<button  class='btn btn-outline-warning btn-sm btn-wave'  title = 'View patient data from Tebra' onclick=ViewData('" + data.tebraPatientId + "'); >View Data</button>&nbsp;<button  class='btn btn-outline-success btn-sm btn-wave ms-3'  title = 'Accept Data Download Request' onclick=AcceptRequest('" + data.id + "'); >Accept</a> &nbsp; &nbsp; <button class='btn btn-outline-danger btn-sm btn-wave ms-3'  title = 'Reject Data Download Request' onclick=RejectRequest('" + data.id + "'); >Reject</button>";
                            }
                        }
                        else {
                            if (data.statusName == "Accepted" || data.statusName == "Accepted and Paid") {
                                return "<button class='btn btn-outline-warning btn-sm btn-wave'  title = 'View patient data from Tebra' onclick=ViewData('" + data.tebraPatientId + "'); >View Data</button>  <button Class='btn btn-outline-success btn-sm btn-wave ms-3' title='Download patient details to PDF'  id='ancDownload' data-patientId='" + data.tebraPatientId + "'  onclick='DownloadPatientPDF(event)'>Download PDF</button>";
                            }
                            else if (data.statusName == "Rejected") {
                                return "<button  class='btn btn-outline-primary btn-sm btn-wave'  title = 'View Request Rejection Reason' data-rejectionDate='" + data.lastUpdated + "' data-arg='" + data.reason + "' onclick='ViewReason(event);' >View Reason</button>";
                            }
                            else {
                                return "<label class='form-label text-primary'  title = 'Request pending from Admin Side'); >Pending for approval</label>";
                            }
                        }
                    }
                },

            ]
            , "fnDrawCallback": function (data) {

                // Below function written in Common.js file
                ShowHideDataTableInfo(data);
                $("#myOverlay").hide();
            }
        });
    }


});
function ViewReason(event) {
    var reason = event.currentTarget.attributes["data-arg"].value;
    var rejectionDate = event.currentTarget.attributes["data-rejectionDate"].value;
    $("#reason").text(reason);
    $("#rejectionDate").text(rejectionDate);
    $('#reasonModal').modal('show');
}
function ViewData(id) {
    window.location.href = '/patient/details?patientid=' + id;
}
function AcceptRequest(id) {
    $("#acceptRequestId").val(id);
    confirmationModel(
        'Are you sure',
        'You want to accept this data download request !',
        'warning',
        'Yes, accept it!',
        'Accepted!',
        'Your data download request has been accepted.',
        'success',
        function () {
            OnAcceptClick();
        }
    );

}


function OnAcceptClick() {
    var requestId = $("#acceptRequestId").val();
    $.ajax({
        url: '/api/datarequest/acceptrequest',
        method: 'GET', // or 'POST', 'PUT', etc.
        data: { requestId: requestId }, // Optional data to send
        success: function (response) {
            $('#dataRequestDatatable').DataTable().ajax.reload();
            //  showToast('success', "Request accepted successfully!", 'bg-success');
        },
        error: function (xhr, status, error) {
            //  showToast('error', "Something went Wrong!", 'bg-danger');
        }
    });
}
function RejectRequest(id) {
    $("#rejectRequestId").val(id);
    $('#rejectModal').modal('show');

}


function OnRejectClick() {
    var requestId = $("#rejectRequestId").val();
    var rejectReason = $("#rejectTextArea").val();
    if ($.trim(rejectReason) == "") {
        showToast('error', "Please enter valid reason!", 'bg-danger');
        return false;
    }
    $.ajax({
        url: '/api/datarequest/rejectrequest',
        method: 'GET',
        data:
        {
            requestId: requestId,
            rejectReason: rejectReason
        },
        success: function (response) {
            $('#dataRequestDatatable').DataTable().ajax.reload();
            $("#rejectTextArea").val('');
            $('#rejectModal').modal('hide');

            Swal.fire(
                'Request Rejected!',
                'Request rejected successfully!',
                'error',
            );

            // showToast('success', "Request rejected successfully!", 'bg-success');
        },
        error: function (xhr, status, error) {
            showToast('error', "Something went Wrong!", 'bg-danger');
        }
    });
}

function DownloadPatientPDF(event) {
    var url = "/api/patient/exporttopdf";
    $("#myOverlay").show();
    var tebraPatientId = event.currentTarget.attributes["data-patientId"].value;
    $.ajax({
        url: url,
        method: 'GET', // or 'POST', 'PUT', etc.
        data: { patientId: tebraPatientId }, // Optional data to send
        xhrFields: {
            responseType: 'blob' // Treat the response as binary data
        },
        success: function (response, status, xhr) {

            $("#myOverlay").hide();

            // Get the filename from the Content-Disposition header
            var disposition = xhr.getResponseHeader('Content-Disposition');
            var fileName = "PatientDetails.pdf"; // Default filename
            if (disposition && disposition.indexOf('filename=') !== -1) {
                var matches = /filename[^;=\n]*=((['"]).*?\2|[^;\n]*)/.exec(disposition);
                if (matches != null && matches[1]) {
                    fileName = matches[1].replace(/['"]/g, ''); // Remove quotes if present
                }
            }

            // Remove unnecessary content from filename
            fileName = fileName.replace(/(PatientDetails).*\.pdf$/, "$1.pdf");
            var cleanedFile = fileName.replace(/(PatientDetails).*\.pdf$/, "$1.pdf");
            var blob = new Blob([response], { type: 'application/pdf' }); // Ensure correct MIME type
            var url = window.URL.createObjectURL(blob);

            // Create a temporary <a> element
            var a = document.createElement('a');
            a.href = url;
            a.download = cleanedFile; // Set the filename for download
            document.body.appendChild(a);
            a.click(); // Trigger the download
            document.body.removeChild(a); // Clean up
            window.URL.revokeObjectURL(url); // Free up memory

        },
        error: function (xhr, status, error) {
            $("#myOverlay").hide();
        }
    });
}

async function MakePayment(event) {
    try {
        // Show the loader before making the API call
        $("#myOverlay").show();

        var requestId = event.currentTarget.attributes["data-id"].value;
        var userId = event.currentTarget.attributes["data-userId"].value;

        // Create a FormData object
        const form_data = new FormData();
        form_data.append("requestId", requestId);
        form_data.append("userId", userId);

        // Send the POST request
        const response = await fetch("/api/stripe/create-session", { method: "POST", body: form_data });

        // Parse the JSON response
        const session = await response.json();

        // Check for errors in the session
        if (!session.status) {
            showToast('error', session.message || "An unknown error occurred.", 'bg-danger');
            $("#myOverlay").hide();
            return;
        }

        // Get sessionId from the response
        var sessionId = session.data.sessionId;

        // Proceed to Stripe checkout
        const { error } = await stripe.redirectToCheckout({ sessionId });

        if (error) {
            showToast('error', error.message || "An unknown error occurred.", 'bg-danger');
            $("#myOverlay").hide();
            return;
        }
    } catch (err) {
        // Handle unexpected errors
        //console.error("Payment Error:", err);
        showToast('error', err.message || "An unexpected error occurred.", 'bg-danger');
        $("#myOverlay").hide();
    } finally {
        // Hide the loader after the API call completes
        $("#myOverlay").hide();
    }
}