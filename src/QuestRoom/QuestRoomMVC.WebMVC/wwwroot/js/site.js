document.addEventListener("DOMContentLoaded", function () {
    const sidebar = document.getElementById("sidebar");
    const sidebarToggle = document.getElementById("sidebarToggle");
    sidebar.classList.add("show");

    sidebarToggle.addEventListener("click", function () {
        sidebar.classList.toggle("show");
    });

    document.addEventListener("click", function (event) {
        const isClickInside = sidebar.contains(event.target);
        const isToggleClick = sidebarToggle.contains(event.target);

        if (!isClickInside && !isToggleClick) {
            sidebar.classList.remove("show");
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