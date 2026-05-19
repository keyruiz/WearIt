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

                tarjetaClonada.querySelector(".nombre-juego").textContent = juego.nombre;

                const tarjetaElemento = tarjetaClonada.querySelector(".tarjeta-juego");

                tarjetaElemento.onclick = () => {
                    window.location.href = `/Escaparate?juegoId=${juego.id}`;
                };

                contenedor.appendChild(tarjetaClonada);
            });
        })
        .catch(error => console.error("Error al conectar con el backend:", error));
}