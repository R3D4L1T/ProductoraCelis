fetch("/Home/ActualizarCarrito", {
    method: "POST",
    headers: {
        "Content-Type": "application/json",
    },
    body: JSON.stringify(productosSeleccionados),
}).then(response => {
    if (response.ok) {
        // Limpiar el almacenamiento local
        localStorage.removeItem("productosSeleccionados");
        window.location.href = "/Home/Carrito";
    } else {
        alert("Ocurrió un error al actualizar el carrito.");
    }
});
