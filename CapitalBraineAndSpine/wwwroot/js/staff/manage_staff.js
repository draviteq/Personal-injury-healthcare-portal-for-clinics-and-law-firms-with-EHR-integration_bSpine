
$(document).ready(function () {

    var lawUserId = '';
    if ($.url('?userId') != "" && $.url('?userId') != undefined) {
        lawUserId = $.url('?userId');

        $("#UserId").val(lawUserId);
        $("#btnSubmit").html("Update Staff");
        
        $("#lblHeadingTop").html("Staff");
        $("#lblHeading").html("Manage Staff");
    }
    else {
        $("#btnSubmit").html("Create Staff");
        $("#lblHeading").html(" Staff");
        $("#lblHeadingTop").html("Create Staff");
    }

    GetStaffDetails(lawUserId);
});
function GetStaffDetails(lawUserId) {
    $("#myOverlay").show();
    jQuery.ajax({
        type: 'GET',
        url: "/api/staff/details?userId=" + lawUserId,
        cache: false,
        contentType: false,
        processData: false,
        success: function (response) {
            $("#myOverlay").hide();
            $("#Id").val(response.data.id);
            $("#FirstName").val(response.data.firstName);
            $("#LastName").val(response.data.lastName); 
            $("#RoleName").val(response.data.roleName);          
            $("#Email").val(response.data.email);
            $("#PhoneNumber").val(response.data.phoneNumber);
            /*
            $("#Address").val(response.data.address);
            $("#City").val(response.data.city);
            $("#State").val(response.data.state);
            $("#Zip").val(response.data.zip);
            */

            $("#Address").val('');
            $("#City").val('');
            $("#State").val('');
            $("#Zip").val('');

            $("#Email").attr('disabled', 'disabled');
            //$("#RoleName").attr('disabled', 'disabled');

        },
        error: function () {
            // toastr.error("Error occurs");
            $("#myOverlay").hide();
        }
    });
}

$(document).on('click', '#btnSubmit', function (event) {
    if (!$("#managestaff").valid()) {
        return false;
    }

    $("#myOverlay").show();
    event.preventDefault();
    var formData = new FormData();
    formData.append("Id", $('#UserId').val());
    formData.append("Email", $('#Email').val());
    formData.append("FirstName", $('#FirstName').val());
    formData.append("LastName", $('#LastName').val());
    formData.append("Address", $('#Address').val());
    formData.append("City", $('#City').val());
    formData.append("State", $('#State').val());
    formData.append("Zip", $('#Zip').val());
    formData.append("PhoneNumber", $('#PhoneNumber').val());
    formData.append("RoleName", $('#RoleName').val());


    jQuery.ajax({
        type: 'POST',
        url: "/api/staff/manage",
        data: formData,
        cache: false,
        contentType: false,
        processData: false,
        success: function (response) {

            $("#myOverlay").hide();
            if (response.success) {
                showToast('success', response.message, 'bg-success');
                setTimeout(function () { window.location.href = "/staff"; }, 1000);
            }
            else {
                showToast('danger', response.message, 'bg-danger');
            }

        }

    });

});
