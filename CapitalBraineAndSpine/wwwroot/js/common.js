
function showToast(title, message, type) {
    // Remove any existing toast containers
    const existingToastContainers = document.querySelectorAll('.toast-container');
    existingToastContainers.forEach(container => container.remove());

    // Create a container for the toast
    var toastContainer = document.createElement('div');

    // Add the HTML content to the container

    toastContainer.innerHTML = `
           <div class="toast-container position-fixed top-0 end-0 p-3">
              <div id="primaryToast" class="toast colored-toast ${type}-transparent" role="alert" aria-live="assertive" aria-atomic="true">                                                 
                   <div class="toast align-items-center text-${type} border-0 fade show mb-4" role="alert" aria-live="assertive" aria-atomic="true">
                       <div class="toast-header text-${type} text-fixed-white">
                                    <img class="bd-placeholder-img rounded me-2" src="../assets/images/brand-logos/favicon.ico" alt="...">
                                    <strong class="me-auto">Capital Brain & Spine</strong>
                                    <button type="button" class="btn-close" data-bs-dismiss="toast" aria-label="Close"></button>
                                                </div>
                                    <div class="d-flex">
                                       <div class="toast-body">
                                           ${message}
                                       </div>                                   
                                    </div>
                       </div>
                 </div>
              </div>
         </div>`;

    // Append the container to the body or another element in the DOM
    document.body.appendChild(toastContainer);

    // Get the toast element
    var primarytoastExample = document.getElementById('primaryToast');

    // Initialize and show the toast
    var toast = new bootstrap.Toast(primarytoastExample);
    toast.show();
}

// Example: Call the showToast function when a button is clicked
const primaryToastButton = document.getElementById('primaryToastBtn');
if (primaryToastButton) {
    primaryToastButton.addEventListener('click', showToast);
}


// Alert with Text
async function AlertWithText(){
    Swal.fire(
        'The Internet ?',
        'That thing is still around ?',
        'question'
    )
}


//Confirmation model
function confirmationModel(title, text, icon, confirmButtonText, successTitle, successText, successIcon, onConfirm, onCancel) {
    const swalWithBootstrapButtons = Swal.mixin({
        customClass: {
            confirmButton: 'btn btn-success ms-2',
            cancelButton: 'btn btn-danger'
        },
        buttonsStyling: false
    });
    swalWithBootstrapButtons.fire({
        title: title || 'Are you sure?',
        text: text || 'You want to proceed with this action!',
        icon: icon || 'warning',
        showCancelButton: true,
        confirmButtonText: confirmButtonText || 'Yes, accept it!',
        cancelButtonText: 'No, cancel!',
        reverseButtons: true
    }).then((result) => {
        if (result.isConfirmed) {
            Swal.fire(
                successTitle,
                successText,
                successIcon
            );
            if (typeof onConfirm === 'function') {
                onConfirm();
            }

        } else if (result.dismiss === Swal.DismissReason.cancel) {
            if (typeof onCancel === 'function') {
                onCancel();
            }
        }
    });
}

// Show or hide Data Table info like Paging, Paging Info, Button and filters
function ShowHideDataTableInfo(data) {
    /*
    Check if now empty or no row record found
    var table = $(dataTableId).DataTable();
        
    var aoData = table.settings()[0].aoData; // Access the aoData array

    // Try accessing the first row data using `aoData`
    var firstCellValue = aoData[0]?.anCells[0]?.innerText || aoData[0]._aData[0];
    */

    if (data.aoData.length == 0) {                
        $(".button-container").hide();
        $(".dataTables_paginate").hide();
        $(".dataTables_info").hide();
        $(".dataTables_length").hide();
        $(".dataTables_filter").hide();
    }
    else {
        $(".button-container").show();
        $(".dataTables_paginate").show();
        $(".dataTables_info").show();
        $(".dataTables_length").show();
        $(".dataTables_filter").show();

    }
}






