document.addEventListener("DOMContentLoaded", () => {
    pintarCarrito();
});

function pintarCarrito() {
    const carrito = JSON.parse(localStorage.getItem("carrito")) || [];
    const contenedorVacio = document.getElementById("carrito-vacio");
    const contenedorDatos = document.getElementById("carrito-con-datos");
    const cuerpoTabla = document.getElementById("cuerpo-carrito");
    const moldeFila = document.getElementById("plantilla-fila-carrito");
    const txtTotal = document.getElementById("total-carrito");

    cuerpoTabla.innerHTML = '';

    if (carrito.length === 0) {
        contenedorVacio.classList.remove("d-none");
        contenedorDatos.classList.add("d-none");
        return;
    }

    contenedorVacio.classList.add("d-none");
    contenedorDatos.classList.remove("d-none");

    const grupos = {};
    carrito.forEach(item => {
        const juego = item.juegoNombre || "General";
        if (!grupos[juego]) grupos[juego] = [];
        grupos[juego].push(item);
    });

    let totalGlobal = 0;

    Object.keys(grupos).forEach(nombreJuego => {
        const items = grupos[nombreJuego];

        const filaHeader = document.createElement("tr");
        filaHeader.className = "table-info text-center fw-bold";
        filaHeader.innerHTML = `<td colspan="6">${nombreJuego}</td>`;
        cuerpoTabla.appendChild(filaHeader);

        let subtotalJuego = 0;

        items.forEach(item => {
            const precio = Number(item.precio);
            const subtotal = precio * item.cantidad;
            subtotalJuego += subtotal;

            const clon = moldeFila.content.cloneNode(true);
            clon.querySelector(".prod-descripcion").textContent = item.descripcion;
            clon.querySelector(".prod-imagen").src = item.imagen ? `/imagenes/${item.imagen}` : "/imagenes/imagen-no-disponible.jpg";
            clon.querySelector(".prod-precio").textContent = `${precio.toFixed(2)} €`;
            clon.querySelector(".prod-cantidad").textContent = item.cantidad;
            clon.querySelector(".prod-subtotal").textContent = `${subtotal.toFixed(2)} €`;

            const indiceReal = carrito.indexOf(item);
            clon.querySelector(".btn-mas").onclick = () => {
                carrito[indiceReal].cantidad++;
                actualizarYRefrescar(carrito);
            };

            clon.querySelector(".btn-menos").onclick = () => {
                if (carrito[indiceReal].cantidad > 1) {
                    carrito[indiceReal].cantidad--;
                    actualizarYRefrescar(carrito);
                }
            };

            clon.querySelector(".btn-eliminar").onclick = () => {
                carrito.splice(indiceReal, 1);
                actualizarYRefrescar(carrito);
            };

            cuerpoTabla.appendChild(clon);
        });

        const filaSubtotal = document.createElement("tr");
        filaSubtotal.className = "table-secondary fw-bold";
        filaSubtotal.innerHTML = `<td colspan="4" class="text-end">Subtotal ${nombreJuego}:</td><td class="text-end">${subtotalJuego.toFixed(2)} €</td><td></td>`;
        cuerpoTabla.appendChild(filaSubtotal);

        totalGlobal += subtotalJuego;
    });

    txtTotal.textContent = `${totalGlobal.toFixed(2)} €`;

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
            precio: Number(item.precio)
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

    const btnVaciar = document.getElementById("btn-vaciar");
    if (btnVaciar) {
        btnVaciar.onclick = () => {
            if (confirm("¿Estás seguro de que quieres vaciar el carrito?")) {
                localStorage.removeItem("carrito");
                pintarCarrito();
            }
        };
    }
}

function actualizarYRefrescar(nuevoCarrito) {
    localStorage.setItem("carrito", JSON.stringify(nuevoCarrito));
    pintarCarrito();
}
