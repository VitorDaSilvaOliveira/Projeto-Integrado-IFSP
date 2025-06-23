SpinnerOverlay.visible = false
$("#create-role-inline-form").submit(function (e) {
    e.preventDefault();

    var formData = $(this).serialize();

    $.ajax({
        url: '@Url.Action("Create")',
        type: 'POST',
        data: formData,
        success: function (response) {
            $('.dropdown.show .dropdown-toggle').dropdown('hide');
            location.reload();
        },
        error: function (xhr) {
            var errorMessage = JSON.parse(xhr.responseText).errors[0];
            $("#error-message-add").text(errorMessage);
        }
    });
});