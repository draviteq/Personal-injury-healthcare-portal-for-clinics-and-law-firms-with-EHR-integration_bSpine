//let isNavigatingWithinPage = false;

//// Detect clicks on links, buttons, or form submissions to track navigation within the page
//$(document).on('click', 'a, button, input[type="submit"]', function () {
//    isNavigatingWithinPage = true;
//});

//// Detect form submissions explicitly
//$(document).on('submit', 'form', function () {
//    isNavigatingWithinPage = true;
//});

//// Use beforeunload or unload to detect when the tab/window is closing
//$(window).on('beforeunload', function (event) {
//    // Only send the request if it's a true close event and not internal navigation
//    if (!isNavigatingWithinPage) {
//        // Delaying the unload to give the server a chance to process the request
        
//        setTimeout(function () {            
//            sendCloseRequest();
//        }, 100);  // Wait for 100ms (adjust based on your server response time)
//    }
//});

//// Function to send the AJAX request using the Fetch API (reliable and async)
//async function sendCloseRequest() {
//    const data = { message: 'Browser or tab closed by user' };
//    const url = '/account/logout';  // Adjust to your backend API

//    try {
//        // Send the request with fetch (async and non-blocking)
//        await fetch(url, {
//            method: 'POST',
//            headers: {
//                'Content-Type': 'application/json',
//            },
//            body: JSON.stringify(data),
//        });

//        console.log('Close request sent successfully!');
//    } catch (error) {
//        console.log('Error sending close request:', error);
//    }
//}
function logout() {
    var settings = {
        async: false,
        method: "POST",
        crossDomain: true,
        "global": false,
        url: '/account/logout',
        headers: {
            "content-type": "application/json"
        },
        processData: false,
    };
    $.ajax(settings)
        .done(function (response) {

            if (response.success) {
                window.location.href = "/account/login";
            }
        });
}

