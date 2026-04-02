document.addEventListener("DOMContentLoaded", () => {

    function esColorClaro(hex) {
        hex = hex.replace("#", "");

        const r = parseInt(hex.substring(0, 2), 16);
        const g = parseInt(hex.substring(2, 4), 16);
        const b = parseInt(hex.substring(4, 6), 16);

        const luminancia = (0.299 * r + 0.587 * g + 0.114 * b);
        return luminancia > 186;
    }

    let slides = [];
    let index = 0;
    let intervalo = null;

    const DURACION = window.configPublicidad?.tiempoPorSlide ?? 7000;

    function iniciarCarrusel() {

        if (intervalo) clearInterval(intervalo);

        slides = document.querySelectorAll(".slide");
        if (slides.length === 0) return;

        index = 0;
        slides.forEach(s => s.classList.remove("active"));
        slides[index].classList.add("active");

        intervalo = setInterval(() => {
            slides[index].classList.remove("active");
            index = (index + 1) % slides.length;
            slides[index].classList.add("active");
        }, DURACION);
    }

    iniciarCarrusel();

    // ===============================
    // VERIFICAR CAMBIOS EN SERVIDOR
    // ===============================

    let versionLocal = null;

    function chequearCambios() {
        fetch("/Publicidad/VersionPantalla", { cache: "no-store" })
            .then(r => r.text())
            .then(v => {
                if (versionLocal === null) {
                    versionLocal = v;
                    return;
                }
                if (versionLocal !== v) location.reload();
            });
    }

    setInterval(chequearCambios, 3000);

})