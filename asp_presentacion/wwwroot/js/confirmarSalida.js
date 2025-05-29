document.addEventListener('DOMContentLoaded', function () {
    const backButton = document.getElementById('backButton');
    const submitButton = document.getElementById('submitButton');
    const form = document.querySelector('form');
    let formChanged = false;
    let isSubmitting = false;

    // Detectar cambios en el formulario
    if (form) {
        const initialValues = {};
        Array.from(form.elements).forEach(element => {
            if (element.name && (element.type !== 'submit' && element.type !== 'button')) {
                initialValues[element.name] = element.value;
            }
        });

        form.addEventListener('input', function () {
            formChanged = Array.from(form.elements).some(element => {
                return element.name && initialValues[element.name] !== element.value;
            });
        });
    }

    // Manejar clic en botón de atrás
    if (backButton) {
        backButton.addEventListener('click', function (e) {
            if (formChanged && !isSubmitting) {
                e.preventDefault();
                if (confirm('¿Seguro que quieres salir sin guardar los cambios?')) {
                    window.history.back();
                }
            }
        });
    }

    // Manejar envío del formulario
    if (form && submitButton) {
        form.addEventListener('submit', function () {
            isSubmitting = true;
        });
    }

    // Prevenir cierre de la página con cambios no guardados
    window.addEventListener('beforeunload', function (e) {
        if (formChanged && !isSubmitting) {
            e.preventDefault();
            e.returnValue = 'Tienes cambios sin guardar. ¿Seguro que quieres salir?';
        }
    });
});