// Change Password Ajax call on form submit
$("#formchangepassword").submit(function (event) {
    if (!$("#formchangepassword").valid()) {
        return false;
    }
    $("#myOverlay").show();
    event.preventDefault();
    var formData = new FormData();
    formData.append("CurrentPassword", $("#CurrentPassword").val());
    formData.append("NewPassword", $("#NewPassword").val());
    formData.append("ConfirmPassword", $("#ConfirmPassword").val());

    jQuery.ajax({
        type: 'POST',
        url: "/api/Profile/ChangePassword",
        data: formData,
        cache: false,
        contentType: false,
        processData: false,
        success: function (response) {
            $("#myOverlay").hide();
            if (!response.success && response.validationerror) {
                showToast('error', response.errors[0].description, 'bg-danger');
            }
            else {
                if (response.success) {
                    showToast('success', response.result, 'bg-primary');
                    GetProfileDetails();
                }
                else {
                    showToast('error', response.result, 'bg-danger');
                }
            }
        },
        error: function () {

        }
    });
});