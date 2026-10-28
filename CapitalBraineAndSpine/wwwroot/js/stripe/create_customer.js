$(document).ready(function () {

    $("#myOverlay").show();        
    
    jQuery.ajax({
        type: 'POST',
        url: "/api/stripe/create-customer",        
        cache: false,
        contentType: false,
        processData: false,
        success: function (response) {
            $("#myOverlay").hide();
            if (response.success) {
                showToast('success', response.message, 'bg-success');                
            }
            else {
                showToast('danger', response.message, 'bg-danger');
            }
        },
        error: function (jqXHR, textStatus, errorThrown) {
            // Handle the error
            $("#myOverlay").hide();
            showToast('danger', jqXHR.responseJSON.Message, 'bg-danger');
        }

    });

});