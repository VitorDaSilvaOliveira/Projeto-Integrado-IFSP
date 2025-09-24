SpinnerOverlay.visible = false;

$("#create-role-inline-form").submit(function (e) {
    e.preventDefault();

    var formData = $(this).serialize();

    $.ajax({
        url: window.createRoleUrl,
        type: 'POST',
        data: formData,
        success: function (response) {
            $('.dropdown.show .dropdown-toggle').dropdown('hide');
            location.reload();
        },
        error: function (xhr) {
            var errors = xhr.responseJSON?.errors || ["Erro desconhecido"];
            $("#error-message-add").text(errors[0]);
        }
    });
});
