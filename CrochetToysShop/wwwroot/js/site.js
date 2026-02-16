// Please see documentation at https://learn.microsoft.com/aspnet/core/client-side/bundling-and-minification
// for details on configuring this project to bundle and minify static web assets.

// Highlight active nav link
document.addEventListener("DOMContentLoaded", function () {

    // Active nav link
    const links = document.querySelectorAll(".navbar-nav .nav-link");
    const current = window.location.pathname.toLowerCase();
    links.forEach(function (link) {
        const href = link.getAttribute("href")?.toLowerCase();
        if (href && href !== "/" && current.startsWith(href)) {
            link.classList.add("active", "fw-semibold");
        }
    });

    // Auto-dismiss success/error alerts след 4 секунди
    setTimeout(function () {
        document.querySelectorAll(".alert-success, .alert-danger").forEach(function (el) {
            el.style.transition = "opacity 0.5s";
            el.style.opacity = "0";
            setTimeout(function () { el.remove(); }, 500);
        });
    }, 4000);

});
// Auto-dismiss alerts
document.addEventListener("DOMContentLoaded", function () {
    setTimeout(function () {
        document.querySelectorAll(".alert-success, .alert-danger").forEach(function (el) {
            el.style.transition = "opacity 0.5s";
            el.style.opacity = "0";
            setTimeout(() => el.remove(), 500);
        });
    }, 4000);
});