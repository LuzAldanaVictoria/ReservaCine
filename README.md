# Grupo 3 - Reserva de cine

### Integrantes
> - Do Nascimento, María Florencia	<mflorenciadn@gmail.com>
> - Propatto y Cafaro, Federico	<volumenxero@gmail.com>
> - Victoria, Luz Aldana	<luzaldanavictoria@gmail.com>

### Idea
Se trata de un sistema que permita a usuarios ingresar y hacer una reserva de una función de cine para una película proyectada en una sala en un horario en particular.
 
### Modelos
 - Usuario 
 - Reserva 
 - Función 
 - Película 
 - Sala 
 - Género
 
### Consideraciones mínimas
 - Un Usuario tiene  
	 - Una colección de Reservas 
	 - Una Fecha de alta 
	 - Un Nombre 
	 - Un Apellido 
	 - Una Fecha de nacimiento (sugerencia, puede utilizarse para restringir el acceso a películas donde no le alcance su edad para visualizarla) 
 
 - Una Reserva tiene: 
	 - Un Usuario que la realizó 
	 - Una fecha de alta 
	 - Un costo total (basado en la cantidad de butacas reservadas) 
	 - Una Función 
	 - La cantidad de butacas reservadas 
 
 - Una película tiene: 
	 - Una colección de funciones 
	 - Un nombre 
	 - Un género 
 
 - Una Sala tiene: 
	 - Un Nombre 
	 - La capacidad total en butacas de la sala 
	 - Una colección de funciones disponibles. 
 
 - Una Función tiene: 
	 - Una Sala 
	 - Una Película 
	 - Una colección de reservas. 
	 - La capacidad disponible de butacas (se modifica con cada reserva que se confirme). 
 
 - El Género tiene: 
	 - Una descripción (terror, drama, comedia, etc.) 
	 - Una colección de películas 

 
##### Se requiere que 
 - Se puedan cargar y administrar   
	- Usuarios
	- Salas 
	- Películas 
	- Funciones 
	- Géneros 
 - Los Usuarios pueden buscar Funciones disponibles. 
	 - Pueden buscar las funciones disponibles filtrando por día o por película. 
	 - Si filtran por película deben aparecer todas las funciones disponibles para dicha película. 
	 - Si filtran por día deben aparecer todas las funciones del día para todas las películas del cine. 
	 - Las funciones que se muestran deben estar deshabilitadas y mostrar un cartel de "agotadas" en caso que no haya más butacas disponibles para dicha función. 
	 - Pueden seleccionar una función de la lista, indicar la cantidad de butacas a reservar y proceder a realizar la reserva (se validará si la cantidad de butacas a reservar está disponible en la función seleccionada). 
 - Los Usuarios pueden hacer una Reserva. 
	 - Se selecciona una función de la lista y se dice cuántas butacas se desea reservar para dicha función. 
	 - La reserva a realizar debe indicar el costo total de la misma antes de confirmarse. 
 - Los Usuarios pueden ver sus reservas activas (reservas de una función que aún no se haya proyectado, o sea, futuras). 
 - Las Funciones de una Película 
	 - Se visualizan las funciones que son futuras solamente (activas). 
	 - Siempre tienen la cantidad de butacas disponibles en la sala 
		 - Si se actualiza la cantidad total de butacas de una sala se debe actualizar la cantidad de butacas disponible de todas las funciones futuras a proyectarse en dicha sala. 
		 - Si se hace una reserva para una función se deben actualizar la cantidad de butacas disponibles en dicha función considerando la cantidad de butacas reservadas. 
 - Las Salas pueden modificar la cantidad de butacas disponibles y eso debe actualizar la cantidad de butacas de todas las funciones a proyectarse en dicha sala. 
