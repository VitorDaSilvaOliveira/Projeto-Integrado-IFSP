function previewAndUpload(input) {
    if (input.files && input.files[0]) {
        var reader = new FileReader();
        reader.onload = function (e) {
            document.getElementById('avatarPreview').src = e.target.result;
        };
        reader.readAsDataURL(input.files[0]);

        var formData = new FormData();
        formData.append('newProfilePicture', input.files[0]);

        var token = document.querySelector('input[name="__RequestVerificationToken"]').value;
        formData.append('__RequestVerificationToken', token);

        fetch(window.profilePictureUrl, {
            method: 'POST',
            body: formData,
            credentials: 'same-origin'
        })
            .then(response => response.json())
            .then(data => {
                if (!data.success) {
                    alert('Erro ao salvar imagem: ' + (data.message || 'Desconhecido'));
                }
            })
            .catch(() => alert('Erro no upload da imagem'));
    }
}