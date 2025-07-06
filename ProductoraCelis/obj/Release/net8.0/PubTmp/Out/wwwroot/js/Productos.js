const productosSeleccionados = [];
function actualizarProducto(nombre, precio, cantidad) {
    // Buscar el producto en productosSeleccionados
    const productoExistente = productosSeleccionados.find(producto => producto.nombre === nombre);

    if (productoExistente) {
        // Actualizar la cantidad si el producto ya existe
        productoExistente.cantidad = cantidad;
    } else {
        // Agregar el producto si no está en productosSeleccionados
        productosSeleccionados.push({ nombre, precio, cantidad });
    }

    // Guardar en localStorage
    localStorage.setItem("productosSeleccionados", JSON.stringify(productosSeleccionados));
}

// Función para manejar el clic en el botón de aumentar/disminuir cantidad
function manejarCantidad(event, nombre, precio) {
    const cantidadElemento = event.target.parentElement.querySelector(".cantidad");
    let cantidad = parseInt(cantidadElemento.innerText);

    if (event.target.classList.contains("btn-aumentar")) {
        cantidad++;
    } else if (event.target.classList.contains("btn-disminuir") && cantidad > 0) {
        cantidad--;
    }

    // Actualizar el elemento de cantidad en el HTML
    cantidadElemento.innerText = cantidad;

    // Actualizar o agregar el producto en productosSeleccionados
    actualizarProducto(nombre, precio, cantidad);
}

// Configuración de los eventos de cada producto
document.querySelectorAll(".combo").forEach((combo) => {
    const nombre = combo.querySelector("p").innerText;
    const precio = combo.querySelector("span").innerText;

    // Añadir eventos a los botones + y -
    combo.querySelector(".btn-aumentar").addEventListener("click", (event) => manejarCantidad(event, nombre, precio));
    combo.querySelector(".btn-disminuir").addEventListener("click", (event) => manejarCantidad(event, nombre, precio));
});

function mostrarSeccion(seccion) {
    const secciones = document.querySelectorAll('.producto');
    secciones.forEach(sec => sec.style.display = 'none');

    document.getElementById(seccion).style.display = 'block';
}