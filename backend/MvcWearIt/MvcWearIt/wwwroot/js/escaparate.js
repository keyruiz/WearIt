document.addEventListener("DOMContentLoaded", () => {
    // Leemos los IDs que C# nos dejó escondidos en el HTML
    const juegoId = document.getElementById("hdnJuegoId").value;
    const categoriaId = document.getElementById("hdnCategoriaId").value;

    cargarProductosEscaparate(juegoId, categoriaId);
});

function cargarProductosEscaparate(juegoId, categoriaId) {
    // Construimos la URL de la API. Si categoriaId está vacío, el backend devuelve todos los productos del juego
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
            const contenedor = document.getElementById("contenedor-productos");
            const molde = document.getElementById("plantilla-producto");

            contenedor.innerHTML = '';

            if (productos.length === 0) {
                contenedor.innerHTML = `<div class="col-100 text-center text-muted my-5"><h3>No hay productos disponibles en esta sección.</h3></div>`;
                return;
            }

            productos.forEach(prod => {
                const clon = molde.content.cloneNode(true);

                clon.querySelector(".producto-descripcion").textContent = prod.descripcion;
                clon.querySelector(".producto-categoria").textContent = prod.categoria ? prod.categoria.descripcion : "General";
                clon.querySelector(".producto-precio").textContent = `${prod.precio} €`;
                clon.querySelector(".producto-imagen").src = prod.imagen ? `/imagenes/${prod.imagen}` : "/imagenes/imagen-no-disponible.jpg";


                // BOTÓN AÑADIR AL CARRITO (Con localStorage)
                const botonCarrito = clon.querySelector(".boton-carrito");
                botonCarrito.onclick = (e) => {
                    e.stopPropagation(); // Evita que se abra el detalle

                    // 1. Recuperamos el carrito actual del localStorage (o creamos uno vacío si no existe)
                    let carrito = JSON.parse(localStorage.getItem("carrito")) || [];

                    // 2. Comprobamos si el producto ya está en el carrito
                    const productoExistente = carrito.find(item => item.id === prod.id);

                    if (productoExistente) {
                        productoExistente.cantidad++;
                    } else {
                        // Si es nuevo, lo metemos con cantidad 1 y guardamos los datos que necesitamos pintar
                        carrito.push({
                            id: prod.id,
                            descripcion: prod.descripcion,
                            precio: prod.precio,
                            cantidad: 1,
                            imagen: prod.imagen || ""
                        });
                    }

                    // 3. Guardamos el carrito actualizado de vuelta en el localStorage
                    localStorage.setItem("carrito", JSON.stringify(carrito));

                    actualizarContadorCarrito();
                };

                // CLIC EN LA TARJETA (Ver detalle)
                const tarjeta = clon.querySelector(".card");
                tarjeta.onclick = () => {
                    window.location.href = `/Escaparate/Detalle?id=${prod.id}`;
                };

                contenedor.appendChild(clon);
            });
        })
        .catch(err => console.error("Error en el fetch del escaparate:", err));
}

