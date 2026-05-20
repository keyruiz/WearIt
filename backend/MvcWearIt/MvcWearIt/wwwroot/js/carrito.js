document.addEventListener("DOMContentLoaded", () => {
    pintarCarrito();
});

function pintarCarrito() {
    const carrito = JSON.parse(localStorage.getItem("carrito")) || [];
    const contenedorVacio = document.getElementById("carrito-vacio");
    const contenedorDatos = document.getElementById("carrito-con-datos");
    const cuerpoTabla = document.getElementById("cuerpo-carrito");
    const molde = document.getElementById("plantilla-fila-carrito");
    const txtTotal = document.getElementById("total-carrito");

    cuerpoTabla.innerHTML = '';

    if (carrito.length === 0) {
        contenedorVacio.classList.remove("d-none");
        contenedorDatos.classList.add("d-none");
        return;
    }

    contenedorVacio.classList.add("d-none");
    contenedorDatos.classList.remove("d-none");

    let total = 0;

    carrito.forEach((item, indice) => {
        const subtotal = item.precio * item.cantidad;
        total += subtotal;

        const clon = molde.content.cloneNode(true);
        clon.querySelector(".prod-descripcion").textContent = item.descripcion;
        clon.querySelector(".prod-imagen").src = item.imagen ? `/imagenes/${item.imagen}` : "/imagenes/imagen-no-disponible.jpg";
        clon.querySelector(".prod-precio").textContent = `${item.precio.toFixed(2)} €`;
        clon.querySelector(".prod-cantidad").textContent = item.cantidad;
        clon.querySelector(".prod-subtotal").textContent = `${subtotal.toFixed(2)} €`;

        clon.querySelector(".btn-mas").onclick = () => {
            carrito[indice].cantidad++;
            actualizarYRefrescar(carrito);
        };

        clon.querySelector(".btn-menos").onclick = () => {
            if (carrito[indice].cantidad > 1) {
                carrito[indice].submit;
                carrito[indice].cantidad--;
                actualizarYRefrescar(carrito);
            }
        };

        clon.querySelector(".btn-eliminar").onclick = () => {
            carrito.splice(indice, 1);
            actualizarYRefrescar(carrito);
        };

        cuerpoTabla.appendChild(clon);
    });

    txtTotal.textContent = `${total.toFixed(2)} €`;

    // CONTROL DE SEGURIDAD PARA ANÓNIMOS
    const btnConfirmar = document.getElementById("btn-confirmar");
    btnConfirmar.onclick = async () => {
        const estaAutenticado = btnConfirmar.getAttribute("data-autenticado") === "true";

        if (!estaAutenticado) {
            alert("Para procesar la compra necesitas registrarte o iniciar sesión.");
            window.location.href = "/Identity/Account/Register";
            return;
        }

        const items = carrito.map(item => ({
            productoId: item.id,
            cantidad: item.cantidad,
            descripcion: item.descripcion,
            precio: item.precio
        }));

        try {
            const res = await fetch("/Carrito/ConfirmarPedido", {
                method: "POST",
                headers: { "Content-Type": "application/json" },
                body: JSON.stringify(items)
            });
            const data = await res.json();

            if (data.exito) {
                alert("¡Pedido confirmado correctamente!");
                localStorage.removeItem("carrito");
                window.location.href = "/Juegos/Index";
            } else {
                alert("Error: " + data.mensaje);
            }
        } catch (err) {
            alert("Error al conectar con el servidor.");
            console.error(err);
        }
    };
}

function actualizarYRefrescar(nuevoCarrito) {
    localStorage.setItem("carrito", JSON.stringify(nuevoCarrito));
    pintarCarrito();
}