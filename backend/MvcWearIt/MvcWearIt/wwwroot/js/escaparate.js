function urlImagen(imagen) {
    if (!imagen) return "/imagenes/imagen-no-disponible.jpg";
    if (imagen.startsWith("http://") || imagen.startsWith("https://")) return imagen;
    return "/imagenes/" + imagen;
}

document.addEventListener("DOMContentLoaded", () => {
    const juegoId = document.getElementById("hdnJuegoId").value;
    const categoriaId = document.getElementById("hdnCategoriaId").value;

    const inputBuscar = document.getElementById("buscador-productos");
    let productosCache = [];
    let paginaActual = 1;
    const itemsPorPagina = 12;

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

        const totalPaginas = Math.ceil(filtrados.length / itemsPorPagina);
        if (paginaActual > totalPaginas) paginaActual = totalPaginas;

        const inicio = (paginaActual - 1) * itemsPorPagina;
        const fin = inicio + itemsPorPagina;
        const paginaItems = filtrados.slice(inicio, fin);

        paginaItems.forEach(prod => {
            const clon = molde.content.cloneNode(true);

            clon.querySelector(".producto-descripcion").textContent = prod.descripcion;
            clon.querySelector(".producto-categoria").textContent = prod.categoria ? prod.categoria.descripcion : "General";
            clon.querySelector(".producto-precio").textContent = `${Number(prod.precio).toFixed(2)} €`;
            clon.querySelector(".producto-imagen").src = urlImagen(prod.imagen);

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

        if (totalPaginas > 1) {
            const nav = document.createElement("div");
            nav.className = "col-12 d-flex justify-content-center mt-3";
            nav.innerHTML = `
                <nav>
                    <ul class="pagination">
                        <li class="page-item ${paginaActual <= 1 ? 'disabled' : ''}">
                            <a class="page-link" href="#" data-pagina="${paginaActual - 1}">&laquo;</a>
                        </li>
                        ${Array.from({ length: totalPaginas }, (_, i) => i + 1).map(p => `
                            <li class="page-item ${p === paginaActual ? 'active' : ''}">
                                <a class="page-link" href="#" data-pagina="${p}">${p}</a>
                            </li>
                        `).join('')}
                        <li class="page-item ${paginaActual >= totalPaginas ? 'disabled' : ''}">
                            <a class="page-link" href="#" data-pagina="${paginaActual + 1}">&raquo;</a>
                        </li>
                    </ul>
                </nav>
            `;
            nav.querySelectorAll("[data-pagina]").forEach(link => {
                link.onclick = (e) => {
                    e.preventDefault();
                    paginaActual = parseInt(link.dataset.pagina);
                    renderizarProductos(productosCache);
                };
            });
            contenedor.appendChild(nav);
        }
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
                paginaActual = 1;
                renderizarProductos(productosCache);

                if (inputBuscar) {
                    inputBuscar.addEventListener("input", () => {
                        paginaActual = 1;
                        renderizarProductos(productosCache);
                    });
                }
            })
            .catch(err => console.error("Error en el fetch del escaparate:", err));
    }

    cargarProductosEscaparate(juegoId, categoriaId);
});
