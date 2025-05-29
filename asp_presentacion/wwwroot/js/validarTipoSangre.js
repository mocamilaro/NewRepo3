document.addEventListener("DOMContentLoaded", function () {
    const tipoSangreInput = document.querySelector("[asp-for='PacienteEditable.TipoSang']");
    const validos = ["A+", "A-", "B+", "B-", "AB+", "AB-", "O+", "O-"];
    const errorSpan = document.querySelector("[data-valmsg-for='PacienteEditable.TipoSang']") ||
        document.querySelector(".text-danger[asp-validation-for='PacienteEditable.TipoSang']");

    if (tipoSangreInput) {
        // Validación en tiempo real mientras escribe
        tipoSangreInput.addEventListener("input", function () {
            let valor = this.value.toUpperCase().replace(/[^ABO+-]/g, '');

            // Limitar a 3 caracteres máximo (para AB+/-)
            if (valor.length > 3) {
                valor = valor.substring(0, 3);
            }

            // Autoformatear cuando se escribe el tipo
            if (valor.length === 1 && /[ABO]/.test(valor)) {
                this.value = valor;
            } else if (valor.length === 2 && /[ABO][+-]/.test(valor)) {
                this.value = valor;
            } else if (valor.length === 3 && /AB[+-]/.test(valor)) {
                this.value = valor;
            } else {
                this.value = valor;
            }

            // Mostrar/ocultar error en tiempo real
            if (errorSpan) {
                const currentValue = this.value.trim().toUpperCase();
                if (currentValue && !validos.includes(currentValue)) {
                    errorSpan.textContent = "Tipo de sangre inválido. Valores permitidos: A+, A-, B+, B-, AB+, AB-, O+, O-.";
                    errorSpan.style.display = 'block';
                } else {
                    errorSpan.textContent = '';
                    errorSpan.style.display = 'none';
                }
            }
        });

        // Validación al perder el foco
        tipoSangreInput.addEventListener("blur", function () {
            const valor = this.value.trim().toUpperCase();
            if (valor && !validos.includes(valor) {
                if (errorSpan) {
                    errorSpan.textContent = "Tipo de sangre inválido. Valores permitidos: A+, A-, B+, B-, AB+, AB-, O+, O-.";
                    errorSpan.style.display = 'block';
                }
            }
        });

        // Validación al enviar el formulario
        const form = tipoSangreInput.closest("form");
        form.addEventListener("submit", function (e) {
            const valor = tipoSangreInput.value.trim().toUpperCase();
            if (valor && !validos.includes(valor)) {
                if (errorSpan) {
                    errorSpan.textContent = "Tipo de sangre inválido. Valores permitidos: A+, A-, B+, B-, AB+, AB-, O+, O-.";
                    errorSpan.style.display = 'block';
                }
                tipoSangreInput.focus();
                e.preventDefault();
            }
        });
    }
});