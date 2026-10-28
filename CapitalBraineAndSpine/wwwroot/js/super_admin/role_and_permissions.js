$(document).ready(function () {
    const apiUrl = "/api/rolespermission";
    const permissionsUrl = `${apiUrl}/permissions`;
    let permissions = [];

    // Load permissions
    function loadPermissions() {
        $.get(permissionsUrl, function (data) {
            permissions = data;
            $("#permissionsList").html(
                permissions.map(
                    (perm) =>
                        `<div class="wrap-permission">
                            <input type="checkbox" class="form-check-input checkBorder" id="perm_${perm.id}" value="${perm.name}">
                            <label for="perm_${perm.id}">${perm.name}</label>
                        </div>`
                ).join("")
            );
        });
    }

    // Load roles
    function loadRoles() {
        $.get(apiUrl + "/roles/", function (data) {            
            const rows = data.map(
                (role) => `
                    <tr>
                        <td>${role.name}</td>
                        <td>${role.permissions.join(", ").replace(/,\s*/g, '<br>')}</td>
                        <td>                            
                            <i class='far fa-edit cursorPointer editRole' data-bs-toggle='tooltip' data-bs-custom-class='tooltip-primary' data-bs-placement='Right' title = 'Edit Permissions' data-id="${role.id}" ></i>                             
                                                        

                        </td>
                    </tr>`
            );
            $("#rolesTable tbody").html(rows.join(""));
        });
    }


    $('#roleModal').on('show.bs.modal', function () {
        // Reset the form when the modal is about to be shown
        $(this).find('#roleForm')[0].reset();
    });

    // Open modal for Add/Edit Role
    $("#rolesTable").on("click", ".editRole", function () {
        //$('#roleModal .modal-body').empty();
        const id = $(this).data("id");        
        if (id) {
            $.get(`${apiUrl}/roles/${id}`, function (role) {                
                $("#roleId").val(role[0].id);
                $("#roleName").text(role[0].name);
                role[0].permissions.forEach((perm) => {                    
                    $(`#perm_${permissions.find(p => p.name === perm).id}`).prop("checked", true);
                });
            });
        } else {
            $("#roleId").val("");
            $("#roleName").text("");
            $("#permissionsList input[type=checkbox]").prop("checked", false);
        }
        
        $("#roleModal").modal("show");
    });

    // Save Role
    $("#roleForm").submit(function (e) {

        e.preventDefault();
        var permissionList = $("#permissionsList input[type=checkbox]:checked")
            .map(function () { return this.value; })
            .get();
        if (permissionList.length === 0) {
            showToast('danger', "Please select permission to continue", 'bg-warning');
            return;
        } 

        
        const id = $("#roleId").val();
        const role = {
            id: id,
            name: $("#roleName").text().toLowerCase(),
            permissions: $("#permissionsList input[type=checkbox]:checked")
                .map(function () { return this.value; })
                .get()
        };
        if (id) {
            $.ajax({
                url: `${apiUrl}/update`,
                type: "POST",
                contentType: "application/json",
                data: JSON.stringify(role),
                success: function (response) {
                    if (response.status) {
                        showToast('success', response.message, 'bg-primary');
                        loadRoles();
                    }
                    else {
                        showToast('danger', response.message, 'bg-danger');
                    }
                    $("#myOverlay").hide();
                }
            });
        } else {
            $.post(apiUrl, role, loadRoles);
        }
        $("#roleModal").modal("hide");
    });


    /*
    // Delete Role
    $("#rolesTable").on("click", ".deleteRole", function () {
        const id = $(this).data("id");
        if (confirm("Are you sure you want to delete this role?")) {
            $.ajax({
                url: `${apiUrl}/${id}`,
                type: "DELETE",
                success: loadRoles
            });
        }
    });
    */
    loadRoles();
    loadPermissions();
});
