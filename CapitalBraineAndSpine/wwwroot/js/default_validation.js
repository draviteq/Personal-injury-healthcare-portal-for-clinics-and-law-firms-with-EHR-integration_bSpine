$(document).ready(function () {
    $('.form-control').keypress(function (event) {
        // Check if the Enter key was pressed (key code 13)
        if (event.which === 13) {
            // Prevent the default form submission
            event.preventDefault();
            // Trigger the form's submit event
            $(".defaultbutton").click();

        }
    });

    // Phone Number Mask
    var phoneMask = IMask(
        document.getElementById('PhoneNumber'), {
        mask: '(000) 000-0000'
    });

    // Define the ZIP code mask
    var zipCodeMask = IMask(
        document.getElementById('Zip'), {
        mask: [
            {
                mask: '00000',   // 5-digit ZIP code
            },
            {
                mask: '00000-0000',  // 5+4 ZIP+4 code
            }
        ]
    });




});