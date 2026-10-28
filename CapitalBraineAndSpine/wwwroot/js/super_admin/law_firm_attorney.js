function GetLawfirmDetails(userId) {
    $("#myOverlay").show();
    jQuery.ajax({
        type: 'GET',
        url: "/api/lamfirmstaff/details?userId=" + userId,
        cache: false,
        contentType: false,
        processData: false,
        success: function (response) {
            $("#Id").val(response.data.id);
            $("#UserName").val(response.data.firstName);
            $("#LastName").val(response.data.lastName);
            $("#Email").val(response.data.email);
            $("#PhoneNumber").val(response.data.phoneNumber);
            $("#Address").val(response.data.address);
            $("#LawFirmName").val(response.data.city);
            $("#City").val(response.data.gender);
            $("#State").val(response.data.age);
            $("#Zip").val(response.data.zip);
            $("#myOverlay").hide();
        }, 
        error: function () {
            $("#myOverlay").hide();
            // toastr.error("Error occurs");
        }
    });
}

$("#managelawfirm").submit(function (event) {    
    if (!$("#managestaff").valid()) {
        return false;
    }
    $("#myOverlay").show();
    event.preventDefault();
    var formData = new FormData();
    formData.append("Id", $('#Id').val());
    formData.append("Email", $('#Email').val());
    formData.append("FirstName", $('#FirstName').val());
    formData.append("LastName", $('#LastName').val());
    formData.append("Address", $('#Address').val());
    formData.append("City", $('#City').val());
    formData.append("State", $('#State').val());
    formData.append("Zip", $('#Zip').val());
    formData.append("PhoneNumber", $('#PhoneNumber').val());
    formData.append("LawFirmName", $('#LawFirmName').val());

    jQuery.ajax({
        type: 'POST',
        url: "/api/lawfirmstaff/manage",
        data: formData,
        cache: false,
        contentType: false,
        processData: false,
        success: function (response) {
            $("#myOverlay").hide();
            if (response.status) {
                showToast('success', response.message, 'bg-primary');
                window.location.href = "/lawfirm";
            }
            else {
                showToast('danger', response.message, 'bg-danger');
            }
        }

    });
});