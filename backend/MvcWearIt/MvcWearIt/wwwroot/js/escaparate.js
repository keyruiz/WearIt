document.addEventListener("DOMContentLoaded", () => {
    const juegoId = document.getElementById("hdnJuegoId").value;
    const categoriaId = document.getElementById("hdnCategoriaId").value;

    const inputBuscar = document.getElementById("buscador-productos");
    let productosCache = [];

    function renderizarProductos(productos) {
        const contenedor = document.getElementById("contenedor-productos");
        const molde = document.getElementById("plantilla-producto");
        contenedor.innerHTML = '';

        const texto = (inputBuscar?.value || "").toLowerCase().trim();
        const filtrados = texto
            ? productos.filter(p => p.descripcion?.toLowerCase().includes(texto))
            : productos;

        if (filtrados.length === 0) {
            contenedor.innerHTML = `<div class="col-12 text-center text-muted my-5"><h3>No hay productos disponibles en esta sección.</h3></div>`;
            return;
        }

        filtrados.forEach(prod => {
            const clon = molde.content.cloneNode(true);

            clon.querySelector(".producto-descripcion").textContent = prod.descripcion;
            clon.querySelector(".producto-categoria").textContent = prod.categoria ? prod.categoria.descripcion : "General";
            clon.querySelector(".producto-precio").textContent = `${Number(prod.precio).toFixed(2)} €`;
            clon.querySelector(".producto-imagen").src = prod.imagen ? `/imagenes/${prod.imagen}` : "/imagenes/imagen-no-disponible.jpg";

            const botonCarrito = clon.querySelector(".boton-carrito");
            botonCarrito.onclick = (e) => {
                e.stopPropagation();

                let carrito = JSON.parse(localStorage.getItem("carrito")) || [];

                const productoExistente = carrito.find(item => item.id === prod.id);

                if (productoExistente) {
                    productoExistente.cantidad++;
                } else {
                    carrito.push({
                        id: prod.id,
                        descripcion: prod.descripcion,
                        precio: prod.precio,
                        cantidad: 1,
                        imagen: prod.imagen || "",
                        juegoId: prod.juegoId,
                        juegoNombre: prod.juego ? prod.juego.nombre : "General"
                    });
                }

                localStorage.setItem("carrito", JSON.stringify(carrito));
                actualizarContadorCarrito();
            };

            const tarjeta = clon.querySelector(".card");
            tarjeta.onclick = () => {
                window.location.href = `/Escaparate/Detalle?id=${prod.id}`;
            };

            contenedor.appendChild(clon);
        });
    }

    function cargarProductosEscaparate(juegoId, categoriaId) {
        let url = `/Escaparate/ObtenerProductos?juegoId=${juegoId}`;
        if (categoriaId) {
            url += `&id=${categoriaId}`;
        }

        fetch(url)
            .then(res => {
                if (!res.ok) throw new Error("Error al obtener los productos del escaparate");
                return res.json();
            })
            .then(productos => {
                productosCache = productos;
                renderizarProductos(productosCache);

                if (inputBuscar) {
                    inputBuscar.addEventListener("input", () => renderizarProductos(productosCache));
                }
            })
            .catch(err => console.error("Error en el fetch del escaparate:", err));
    }

    cargarProductosEscaparate(juegoId, categoriaId);
});
