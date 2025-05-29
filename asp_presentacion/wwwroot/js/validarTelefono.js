document.addEventListener("DOMContentLoaded", function () {
    const telefonoInput = document.querySelector('input[name="PacienteEditable.Telefono"]');

    if (telefonoInput) {
        // Evita que se escriban letras mientras se escribe
        telefonoInput.addEventListener("keypress", function (event) {
            const charCode = event.charCode;
            if (charCode < 48 || charCode > 57) {
                event.preventDefault(); // Bloquea todo lo que no sea número (0–9)
            }
        });

        // Elimina caracteres inválidos si se pegan desde el portapapeles
        telefonoInput.addEventListener("input", function () {
            this.value = this.value.replace(/\D/g, ''); // Elimina cualquier cosa que no sea número
        });
    }
});
