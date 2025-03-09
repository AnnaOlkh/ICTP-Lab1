﻿// Please see documentation at https://learn.microsoft.com/aspnet/core/client-side/bundling-and-minification
// for details on configuring this project to bundle and minify static web assets.
/*document.getElementById("sidebarToggle").addEventListener("click", function () {
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
});*/
document.addEventListener("DOMContentLoaded", function () {
    let sidebar = document.getElementById("sidebar");
    let content = document.getElementById("content");
    let sidebarToggle = document.getElementById("sidebarToggle");

    // Імітація натискання на кнопку "гамбургер" при завантаженні сторінки
    sidebar.classList.add("show");
    content.classList.add("shift");

    // Додаємо обробник кліку для кнопки, щоб працював стандартний функціонал
    sidebarToggle.addEventListener("click", function () {
        sidebar.classList.toggle("show");
        content.classList.toggle("shift");
    });

    // Закриття сайдбару при кліку поза ним
    document.addEventListener("click", function (event) {
        if (!sidebar.contains(event.target) && event.target !== sidebarToggle) {
            sidebar.classList.remove("show");
            content.classList.remove("shift");
        }
    });
});

document.addEventListener("DOMContentLoaded", function () {
    let sidebar = document.getElementById("sidebar");
    let footer = document.querySelector("footer");

    function adjustSidebar() {
        let footerTop = footer.getBoundingClientRect().top;
        let windowHeight = window.innerHeight;
        let sidebarMaxHeight = windowHeight - 72;

        if (footerTop < windowHeight) {
            let newHeight = footerTop - 72;
            sidebar.style.height = newHeight + "px";
        } else {
            sidebar.style.height = sidebarMaxHeight + "px";
        }
    }

    window.addEventListener("scroll", adjustSidebar);
    adjustSidebar();
});