function abrirImagenModal(rutaImagen, nombreProducto) {
    var modalImagen = document.getElementById('imagenGrande');
    var modalTitulo = document.getElementById('imagenModalLabel');

    modalImagen.src = rutaImagen;
    modalTitulo.textContent = nombreProducto;

    var modal = new bootstrap.Modal(document.getElementById('imagenModal'));
    modal.show();
}
y