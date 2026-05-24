document.addEventListener("DOMContentLoaded", () => {
    cargarJuegosPortada();
});

function cargarJuegosPortada() {
    fetch('/Juegos/ObtenerTodos')
        .then(response => {
            if (!response.ok) {
                throw new Error("No se pudo obtener la lista de juegos");
            }
            return response.json(); 
        })
        .then(juegos => {
            const contenedor = document.getElementById("contenedor-juegos");
            const molde = document.getElementById("plantilla-juego");

            contenedor.innerHTML = '';

            juegos.forEach(juego => {
                const tarjetaClonada = molde.content.cloneNode(true);

                // 1. Ponemos el nombre del juego
                tarjetaClonada.querySelector(".nombre-juego").textContent = juego.nombre;

                // 2. Buscamos la etiqueta img y le encasquetamos la url que viene del servidor
                // (JavaScript en los navegadores es case-sensitive: asegúrate de si es juego.imagenPortada o juego.imagen_portada)
                const imgElemento = tarjetaClonada.querySelector(".img-portada-fondo");
                if (juego.imagenPortada) {
                    imgElemento.src = juego.imagenPortada;
                }

                // 3. Configurar clic
                const tarjetaElemento = tarjetaClonada.querySelector(".tarjeta-juego");
                tarjetaElemento.onclick = () => {
                    window.location.href = `/Escaparate?juegoId=${juego.id}`;
                };

                contenedor.appendChild(tarjetaClonada);
            });
        })
        .catch(error => console.error("Error al conectar con el backend:", error));
}