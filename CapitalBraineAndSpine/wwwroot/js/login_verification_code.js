
$(document).ready(function () {
    $('.code-input').keypress(function (event) {        
        // Check if the Enter key was pressed (key code 13)
        if (event.which === 13) {
            // Prevent the default form submission
            event.preventDefault();            
            // Trigger the form's submit event                        
            $("#btnValidateOtp").click();
            
        }
    });


//    $("#ReturnUrl").val(returnUrl);
    DisplayExpirationTime();
    $("#ancResendOtp").removeAttr("href");
    $('#ancResendOtp').off('click');    
    $("#ancResendOtp").css('cursor', 'default').css('text-decoration', 'none');

});
const inputs = document.getElementById("inputs");
if (inputs != null) {
    inputs.addEventListener("input", function (e) {
        const target = e.target;
        const val = target.value;

        if (isNaN(val)) {
            target.value = "";
            return;
        }

        if (val != "") {
            const next = target.nextElementSibling;
            if (next) {
                next.focus();
            }
        }
    });

    inputs.addEventListener("keyup", function (e) {
        const target = e.target;
        const key = e.key.toLowerCase();

        if (key == "backspace" || key == "delete") {
            target.value = "";
            const prev = target.previousElementSibling;
            if (prev) {
                prev.focus();
            }
            return;
        }
    });
}
(function (window, document) {    
    const form = document.querySelector('.otp_form')

    if (form == null)
        return;

    const inputs = form.querySelectorAll('.code-input')
    const KEYBOARDS = {
        backspace: 8,
        arrowLeft: 37,
        arrowRight: 39,
    }

    function handleInput(e) {
        const input = e.target
        const nextInput = input.nextElementSibling
        if (nextInput && input.value) {
            nextInput.focus()
            if (nextInput.value) {
                nextInput.select()
            }
        }
    }

    function handlePaste(e) {
        e.preventDefault()
        const paste = e.clipboardData.getData('text')
        inputs.forEach((input, i) => {
            input.value = paste[i] || ''
        })
    }

    function handleBackspace(e) {
        const input = e.target
        if (input.value) {
            input.value = ''
            return
        }

        input.previousElementSibling.focus()
    }

    function handleArrowLeft(e) {
        const previousInput = e.target.previousElementSibling
        if (!previousInput) return
        previousInput.focus()
    }

    function handleArrowRight(e) {
        const nextInput = e.target.nextElementSibling
        if (!nextInput) return
        nextInput.focus()
    }

    form.addEventListener('input', handleInput)
    inputs[0].addEventListener('paste', handlePaste)

    inputs.forEach(input => {
        input.addEventListener('focus', e => {
            setTimeout(() => {
                e.target.select()
            }, 0)
        })

        input.addEventListener('keydown', e => {
            switch (e.keyCode) {
                case KEYBOARDS.backspace:
                    handleBackspace(e)
                    break
                case KEYBOARDS.arrowLeft:
                    handleArrowLeft(e)
                    break
                case KEYBOARDS.arrowRight:
                    handleArrowRight(e)
                    break
                default:
            }
        })
    })

})(window, document);


$(document).on("click", "#btnValidateOtp", function (e) {
    e.preventDefault();
    ValidateOTP();
});
function ValidateOTP() {

    let verificationOtp = '',
        otp_inputs = document.querySelectorAll('.code-input');


    // Get OTP number
    otp_inputs.forEach(element => {
        verificationOtp += $(element).val()
    });

    // CHeck if OTP is valid or not
    var inValidOtp = $.isNumeric(verificationOtp);

    if (verificationOtp.length == 0 || !inValidOtp) {

        if (!inValidOtp)
            $('#spnOTPValidate').text('Please enter valid Otp');

        $("#spnOTPValidate").removeAttr("style");
        $("#spnOTPValidate").css("display", "block");
        $("#spnOTPValidate").removeClass("is-valid");
        $("#spnOTPValidate").addClass("is-invalid");

        return;
    }
    else { $("#spnOTPValidate").css("display:none;"); }


    // Validate OTP in backend
    var form_data = {
        otp: parseInt(verificationOtp),
        userId: $("#hdnuserId").val()
    };
    $.post('/account/validateotp', form_data).done((response) => {        
        if (response.message != "") {
            $('#spnOTPValidate').text(`${response.message}`);
            $("#spnOTPValidate").removeAttr("style");
            $("#spnOTPValidate").css("display", "block");
            $("#spnOTPValidate").removeClass("is-valid");
            $("#spnOTPValidate").addClass("is-invalid");
        }
        else {
            // Redirect to page from where user come from
            /*
            if ($("#hdnreturnurl").val())
                $(location).attr('href', $("#hdnreturnurl").val());
            else
                $(location).attr('href', "/admin/dashboard");
                */
            $(location).attr('href', response.url);
        }

        // To do.....
        // This will be changes according to the role..

    })
}
function DisplayExpirationTime() {

    $(".firstInput").focus();

    const otpExpirationMinutes = 1; // Set the OTP expiration time in minutes
    let timerInterval;

    // Function to start the timer
    function startTimer(duration) {
        let timer = duration * 60;
        timerInterval = setInterval(function () {
            const minutes = Math.floor(timer / 60);
            const seconds = timer % 60;

            // Display the remaining time in the #timer element
            $('#otp-expiration').text(`OTP will expire in:  ${minutes}:${seconds.toString().padStart(2, '0')}`);


            // When OTP expire
            if (--timer < 0) {
                {                    
                    clearInterval(timerInterval);
                    $("#otp-expiration").removeAttr("style");
                    $("#otp-expiration").css("display", "none");

                    $("#divResendOTP").removeAttr("style");
                    $("#divResendOTP").css("display", "block");

                    // Enable Resned Otp option
                    $("#ancResendOtp").removeAttr("style");
                    $("#ancResendOtp").css('cursor', 'pointer').css('text-decoration', 'underline');
                    if (!$(this).hasClass("disabled")) {
                        $(this).attr("href");
                    }

                    $('#ancResendOtp').on('click', function (e) {
                                                

                        $.get('/account/ResendOtp?userId=' + $("#hdnuserId").val()).done((response) => {
                            
                            // Disable Resend Otp option                       
                            $("#ancResendOtp").removeAttr("href");
                            $('#ancResendOtp').off('click');
                            $("#ancResendOtp").css('cursor', 'default').css('text-decoration', 'none');


                            // Display Otp expire time
                            $("#otp-expiration").removeAttr("style");
                            $("#otp-expiration").css("display", "block");

                            // Wil use swwet alert instead of this
                            //$('#spnOTPValidate').text('Otp has been send on your email');

                            $('#spnOTPValidate').text('');

                            swal("", "Otp has been re-send on your email!", "success");

                            // Show expire timer
                            DisplayExpirationTime();
                        })
                    });


                }
            }
        }, 1000);
    }

    // Start the timer when the page loads
    startTimer(otpExpirationMinutes);

}
$(document).on("keypress", "form", function (event) {
    if (event.keyCode === 13) {
        event.preventDefault();
        //alert("You have successfully submitted the form!");
        $(this).submit();
    }
});