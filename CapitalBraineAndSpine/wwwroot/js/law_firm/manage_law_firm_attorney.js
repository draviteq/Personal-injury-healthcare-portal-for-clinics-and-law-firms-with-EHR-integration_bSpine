
var lawFirmUserId = '';
$(document).ready(function () {

    var attorneyUserId = '';
    if ($.url('?userId') != "" && $.url('?userId') != undefined) {
        attorneyUserId = $.url('?userId');
        $("#UserId").val(attorneyUserId);
        $("#btnSubmit").html("Update User");
        GetAttorneyDetails(attorneyUserId);
    }

    if ($.url('?lawfirmuserId') != "" && $.url('?lawfirmuserId') != undefined) {
        lawFirmUserId = $.url('?lawfirmuserId');
        if (attorneyUserId == '') {
            $("#lblHeading").html("Create Attorney");
            $("#lblHeadingTop").html("Create Attorney");
        }
        else {
            $("#lblHeading").html("Manage Attorney");
            $("#lblHeadingTop").html("Manage Attorney");
        }


    }

    if (lawFirmUserId == '' & attorneyUserId == '') {
        $("#lblHeading").html("Create Attorney");
        $("#lblHeadingTop").html("Create Attorney");
    }
    GetLawfirmDetails(lawFirmUserId);
});
function GetLawfirmDetails(lawFirmUserId) {
    $("#myOverlay").show();
    jQuery.ajax({
        type: 'GET',
        url: "/api/lawfirm/details?userId=" + lawFirmUserId,
        cache: false,
        contentType: false,
        processData: false,
        success: function (response) {
            $("#myOverlay").hide();
            if (response.status) {
                $("#lawFirmName").html(response.data.lawFirmName);
                $("#lawFirmName").attr('href', '/lawfirm/manage?userId=' + response.data.id);

            }
        },
        error: function () {
            $("#myOverlay").show();
            // toastr.error("Error occurs");
        }
    });
}

function GetAttorneyDetails(lawUserId) {
    $("#myOverlay").show();
    jQuery.ajax({
        type: 'GET',
        url: "/api/lawfirmstaff/details?userId=" + lawUserId,
        cache: false,
        contentType: false,
        processData: false,
        success: function (response) {
            $("#Id").val(response.data.id);
            $("#FirstName").val(response.data.firstName);
            $("#LastName").val(response.data.lastName);
            $("#LawFirmName").val(response.data.lawFirmName);
            $("#RoleName").val(response.data.roleName);
            $("#Email").val(response.data.email);
            $("#PhoneNumber").val(response.data.phoneNumber);
            
            $("#Address").val('');
            $("#City").val('');
            $("#State").val('');
            $("#Zip").val('');
            
        

            $("#Email").attr('disabled', 'disabled');
            //$("#RoleName").attr('disabled', 'disabled');
            $("#myOverlay").hide();
        },
        error: function () {
            // toastr.error("Error occurs");
            $("#myOverlay").hide();
        }
    });
}

$(document).on('click', '#btnSubmit', function (event) {
    if (!$("#managelawfirm").valid()) {
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
    formData.append("ParentUserId", lawFirmUserId);
    formData.append("RoleName", $('#RoleName').val());
    jQuery.ajax({
        type: 'POST',
        url: "/api/lawfirmstaff/manage",
        data: formData,
        cache: false,
        contentType: false,
        processData: false,
        success: function (response) {
            $("#myOverlay").hide();
            if (response.success) {
                showToast('success', response.message, 'bg-success');

                if ($("#inptRole").val() == 'lawfirm') {
                    setTimeout(function () { window.location.href = "/lawfirmstaff" }, 1000);
                }
                else {
                    setTimeout(function () { window.location.href = "/lawfirm/manage?userId=" + lawFirmUserId }, 1000);
                }
            }
            else {
                showToast('danger', response.message, 'bg-danger');
            }

        }

    });

});
