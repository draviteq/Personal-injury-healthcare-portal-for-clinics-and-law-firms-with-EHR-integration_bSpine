$(document).ready(function () {
    function hideLoader() {
        const loader = document.getElementById("loader");
        loader.classList.add("d-block")
    }

    window.addEventListener("load", hideLoader);
    GetProfileDetails();
});
function GetProfileDetails() {
    $("#myOverlay").show();
    jQuery.ajax({
        type: 'GET',
        url: "/api/profile/getmyprofile",
        cache: false,
        contentType: false,
        processData: false,
        success: function (response) {
            $("#myOverlay").hide();
            $("#Id").val(response.model.id);
            $("#FirstName").val(response.model.firstName);
            $("#LastName").val(response.model.lastName);
            $("#Email").val(response.model.email);
            $("#PhoneNumber").val(response.model.phoneNumber);
            $("#Address").val(response.model.address);
            $("#RoleName").val(response.model.roleName)
            $('#imgProfile').prop("src", response.model.profilePhotoPath);
        },
        error: function () {

        }
    });
}

$("#PhotoFile").on('change', function (e) {


    // Get the selected file
    var selectedFile = this.files[0];

    // Check if a file is selected
    if (selectedFile) {
        // Get the file name
        var fileName = selectedFile.name;

        // Get the file extension
        var extension = fileName.split('.').pop().toLowerCase();

        // Check the file extension (for example, you can check for a specific extension like "jpg")
        if (extension === "png" || extension === "jpg" || extension === "jpeg") {

        } else {
            // Display an error message for disallowed file types
            showToast('', 'Only png and jpg format are allowed', 'bg-danger');
            return;
        }
    }
    var file = e.target.files[0]; // Get the selected file

    if (file) {
        var reader = new FileReader();

        // Read the selected file as a data URL
        reader.readAsDataURL(file);

        // When the file is loaded, update the src attribute of the image element
        reader.onload = function (e) {
            $('#imgProfile').attr('src', e.target.result);
            //GetProfileDetails();
           // window.location.href = '/profile/myprofile';
        };
    }
});

$("#formMyProfile").submit(function (event) {
    if (!$("#formMyProfile").valid()) {
        return false;
    }
    $("#myOverlay").show();
    event.preventDefault();
    var files = $("#PhotoFile").prop("files");
    var formData = new FormData();
    formData.append("Id", $("#Id").val());
    formData.append("FirstName", $("#FirstName").val());
    formData.append("LastName", $("#LastName").val());
    formData.append("Email", $("#Email").val());
    formData.append("PhoneNumber", $("#PhoneNumber").val());
    formData.append("Address", $("#Address").val());
    formData.append("PhotoFile", files[0]);

    jQuery.ajax({
        type: 'POST',
        url: "/api/profile/UpdateMyProfile",
        data: formData,
        cache: false,
        contentType: false,
        processData: false,
        success: function (response) {
            $("#myOverlay").hide();
            if (response.status) {

                showToast('success', response.message, 'bg-primary');
                GetProfileDetails();
                window.location.href = '/profile/myprofile';
            } else {

                showToast('error', response.message, 'bg-danger');

            }
        }

    });

});


