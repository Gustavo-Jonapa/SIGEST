let selectedType = null;

function selectAccountType(type) {
    selectedType = type;

    // Remover selección previa
    document.getElementById('card-cliente').classList.remove('selected');
    document.getElementById('card-tecnico').classList.remove('selected');

    // Ocultar formularios
    document.getElementById('form-cliente').style.display = 'none';
    document.getElementById('tecnico-redirect').style.display = 'none';
    document.getElementById('no-selection').style.display = 'none';

    // Marcar como seleccionado
    document.getElementById('card-' + type).classList.add('selected');
    document.getElementById('radio-' + type).checked = true;

    // Mostrar el formulario/mensaje correspondiente
    if (type === 'cliente') {
        document.getElementById('form-cliente').style.display = 'block';
    } else if (type === 'tecnico') {
        document.getElementById('tecnico-redirect').style.display = 'block';
    }
}

// Validar que las contraseñas coincidan
document.addEventListener('DOMContentLoaded', function () {
    const form = document.getElementById('form-cliente');
    if (form) {
        form.addEventListener('submit', function (e) {
            const password = form.querySelector('input[name="Password"]').value;
            const confirmPassword = form.querySelector('input[name="ConfirmPassword"]').value;

            if (password !== confirmPassword) {
                e.preventDefault();
                alert('Las contraseñas no coinciden');
                return false;
            }
        });
    }
});