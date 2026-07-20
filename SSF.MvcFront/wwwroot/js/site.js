// Please see documentation at https://learn.microsoft.com/aspnet/core/client-side/bundling-and-minification
// for details on configuring this project to bundle and minify static web assets.

// Write your JavaScript code.
// Configuración de clases personalizadas para SweetAlert2 en todo el sistema
const MySwal = Swal.mixin({
    customClass: {
        confirmButton: 'btn custom-swal-btn-confirm',
        cancelButton: 'btn custom-swal-btn-cancel btn-danger'
    },
    buttonsStyling: false // Desactiva el estilo por defecto de SweetAlert para usar nuestro CSS
});