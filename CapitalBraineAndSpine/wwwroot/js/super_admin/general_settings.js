
$(document).ready(function () {
    GetDetailsGeneralSettings();

});

//Change button handle
$("#ChangeButton").click(function () {
    var btn = $(this);
    var input = $("#settingValue");

    if (btn.val() === "Change") {
        input.prop("disabled", false);
        $('#ChangeButton').hide();
        $('#SaveButton').removeAttr("hidden");
    }
});

function GetDetailsGeneralSettings() {
    $("#myOverlay").show();
    $.ajax({
        type: "GET",
        url: "/Settings/GetGeneralSettings",
        data: "model",
        success: function (data) {
            $("#myOverlay").hide();
            if (data.status) {
                $('#Id').val(data.data.id);
                $('#SettingType').val(data.data.settingType);
                $('#settingValue').val(data.data.settingValue);
            }
            else {
                var input = $("#settingValue");
                input.prop("disabled", false);
                $('#ChangeButton').hide();
                $('#SaveButton').removeAttr("hidden");
            }
        }

    });
}
//Inser data in general settings
$("#GeneralSettings").submit(function (event) {
    if (!$("#GeneralSettings").valid()) {
        return false;
    }
    $("#myOverlay").show();
    event.preventDefault();
    var formData = new FormData();
    formData.append("Id", $('#Id').val());
    formData.append("SettingType", $('#SettingType').val());
    formData.append("settingValue", $('#settingValue').val());

    jQuery.ajax({
        type: 'POST',
        url: "/Settings/GeneralSettings",
        data: formData,
        cache: false,
        contentType: false,
        processData: false,
        success: function (response) {
            $("#myOverlay").hide();
            if (response.status) {
                showToast('success', response.message, 'bg-primary');
            }
            else {
                showToast('danger', response.message, 'bg-danger');
            }
        }

    });
});

// Remove any non-numeric characters
$('#settingValue').on('input', function () {
    this.value = this.value.replace(/[^0-9]/g, '');
});
