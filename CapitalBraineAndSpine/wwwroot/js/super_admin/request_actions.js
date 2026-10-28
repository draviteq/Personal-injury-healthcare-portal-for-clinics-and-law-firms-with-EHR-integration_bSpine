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
            // reload the page
            location.reload();
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
            $("#rejectTextArea").val('');
            $('#rejectModal').modal('hide');

            Swal.fire(
                'Request Rejected!',
                'Request rejected successfully!',
                'error',
            );

            // reload the page
            location.reload();
        },
        error: function (xhr, status, error) {
            showToast('error', "Something went Wrong!", 'bg-danger');
        }
    });
}