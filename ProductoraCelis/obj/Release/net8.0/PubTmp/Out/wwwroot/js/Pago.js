document.getElementById("formPago").addEventListener("submit", function () {
    const totalElemento = document.getElementById("total");
    const inputTotal = document.getElementById("inputTotal");

    // Obtén el total del elemento visible y actualiza el campo oculto
    const totalTexto = totalElemento.innerText.replace("S/", "").trim();
    inputTotal.value = parseFloat(totalTexto).toFixed(2);
});
