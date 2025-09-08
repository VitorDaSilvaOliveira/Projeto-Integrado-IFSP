function toggleSenha() {
    const campo = document.getElementById("senha");
    const icon = document.getElementById("toggleIcon");

    if (campo.type === "password") {
        campo.type = "text";
        icon.classList.remove("fa-eye");
        icon.classList.add("fa-eye-slash");
    } else {
        campo.type = "password";
        icon.classList.remove("fa-eye-slash");
        icon.classList.add("fa-eye");
    }
}