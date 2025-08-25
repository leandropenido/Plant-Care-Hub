const navbarToggler = document.querySelectorAll(".navbar-toggler");
const headerNav = document.querySelector(".header-nav");
const modalBackground = document.querySelector(".modal_background");

navbarToggler.forEach((e) => {
  e.addEventListener("click", () => {
    headerNav.classList.toggle("header-nav_show");
    modalBackground.classList.toggle("header-nav_mobile");
  });
});

modalBackground.addEventListener("click", () => {
  headerNav.classList.remove("header-nav_show");
  modalBackground.classList.remove("header-nav_mobile");
});
