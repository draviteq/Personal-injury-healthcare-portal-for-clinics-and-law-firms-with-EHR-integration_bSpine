$(document).on('mouseenter', 'table.dataTable tbody tr', function () {
    // Find the DataTable instance for the hovered row
    const table = $(this).closest('table').DataTable();

    // Remove the 'selected' class from all rows in the current table
    table.$('tr.selected').removeClass('selected');

    // Add the 'selected' class to the hovered row
    $(this).addClass('selected');
});