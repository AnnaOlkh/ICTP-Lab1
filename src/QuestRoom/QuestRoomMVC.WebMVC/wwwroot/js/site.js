// Please see documentation at https://learn.microsoft.com/aspnet/core/client-side/bundling-and-minification
// for details on configuring this project to bundle and minify static web assets.
document.getElementById("sidebarToggle").addEventListener("click", function () {
    let sidebar = document.getElementById("sidebar");
    let content = document.getElementById("content");

    sidebar.classList.toggle("show");
    content.classList.toggle("shift");
});
document.addEventListener("click", function (event) {
    let sidebar = document.getElementById("sidebar");
    let sidebarToggle = document.getElementById("sidebarToggle");

    if (!sidebar.contains(event.target) && event.target !== sidebarToggle) {
        sidebar.classList.remove("show");
    }
});
document.addEventListener("DOMContentLoaded", function () {
    let sidebar = document.getElementById("sidebar");
    let footer = document.querySelector("footer");

    function adjustSidebar() {
        let footerTop = footer.getBoundingClientRect().top;
        let windowHeight = window.innerHeight;
        let sidebarMaxHeight = windowHeight - 72; // Висота без навбару

        if (footerTop < windowHeight) {
            // Футер видно -> зменшуємо сайдбар
            let newHeight = footerTop - 72; // Мінус висота навбару
            sidebar.style.height = newHeight + "px";
        } else {
            // Футер не видно -> стандартна висота
            sidebar.style.height = sidebarMaxHeight + "px";
        }
    }

    // Викликаємо при прокрутці
    window.addEventListener("scroll", adjustSidebar);
    // Викликаємо при завантаженні сторінки
    adjustSidebar();
});

// Write your JavaScript code.
