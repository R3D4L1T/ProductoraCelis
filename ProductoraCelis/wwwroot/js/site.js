// Please see documentation at https://learn.microsoft.com/aspnet/core/client-side/bundling-and-minification
// for details on configuring this project to bundle and minify static web assets.

// Write your JavaScript code.
document.getElementById('menuToggle').addEventListener('click', function () {
    const sidebar = document.getElementById('sidebar_menu');
    const backdrop = document.getElementById('back_menu');
    sidebar.classList.toggle('active');
    backdrop.classList.toggle('active');
});
// Cerrar el menú al hacer clic en el fondo
document.getElementById('back_menu').addEventListener('click', function () {
    const sidebar = document.getElementById('sidebar_menu');
    sidebar.classList.remove('active');
    this.classList.remove('active');
});