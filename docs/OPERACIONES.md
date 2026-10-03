# Recorrido funcional de Lúmina Hotels

Esta galería documenta el sistema en ejecución con información sintética de demostración. Las capturas recorren las funciones disponibles para huésped y administración, incluidos los estados posteriores a actualización, pago, retiro y eliminación.

> Los comprobantes PDF de la reserva actual y del historial fueron descargados y validados. El formulario de cambio de contraseña se comprobó visualmente sin modificar la credencial de demostración.

## 1. Experiencia pública y registro

| Inicio público | Acceso unificado |
| --- | --- |
| ![Inicio público](screenshots/operations/01-inicio-publico.jpg) | ![Inicio de sesión unificado](screenshots/operations/02-login-unificado.jpg) |

| Registro con teléfono | Registro completado |
| --- | --- |
| ![Registro del cliente](screenshots/operations/03-registro-cliente-telefono.jpg) | ![Registro exitoso](screenshots/operations/04-registro-exitoso.jpg) |

## 2. Portal del huésped

### Inicio y reservación

| Portal del cliente | Búsqueda de habitaciones |
| --- | --- |
| ![Portal del cliente](screenshots/operations/05-portal-cliente.jpg) | ![Buscar habitaciones](screenshots/operations/06-cliente-buscar-habitaciones.jpg) |

| Reserva creada | Resumen pendiente |
| --- | --- |
| ![Reserva creada](screenshots/operations/07-cliente-reserva-creada.jpg) | ![Reserva pendiente](screenshots/operations/08-cliente-reservas-pendientes.jpg) |

### Pago y comprobante

| Pago confirmado | Comprobante PDF |
| --- | --- |
| ![Pago confirmado](screenshots/operations/09-cliente-pago-confirmado.jpg) | ![Comprobante PDF](screenshots/operations/10-cliente-comprobante-pdf.jpg) |

### Próximas estancias

| Próximas estancias | Aplicar filtros |
| --- | --- |
| ![Próximas estancias](screenshots/operations/11-cliente-estancias-proximas.jpg) | ![Filtrar próximas estancias](screenshots/operations/12-cliente-estancias-filtrar.jpg) |

| Limpiar filtros |
| --- |
| ![Limpiar filtros](screenshots/operations/13-cliente-estancias-limpiar-filtros.jpg) |

### Historial

| Historial de estancias | Historial filtrado |
| --- | --- |
| ![Historial](screenshots/operations/14-cliente-historial-estancias.jpg) | ![Historial filtrado](screenshots/operations/15-cliente-historial-filtrar.jpg) |

El botón **Descargar comprobante** del historial generó correctamente `Comprobante_Reserva_13.pdf`.

### Perfil y seguridad

| Perfil | Datos actualizados |
| --- | --- |
| ![Perfil del cliente](screenshots/operations/16-cliente-perfil.jpg) | ![Perfil actualizado](screenshots/operations/17-cliente-perfil-actualizar.jpg) |

| Formulario de contraseña | Teléfono preparado para retirar |
| --- | --- |
| ![Cambio de contraseña](screenshots/operations/18-cliente-cambio-contrasena.jpg) | ![Teléfono seleccionado](screenshots/operations/20-cliente-telefono-eliminar-preparado.jpg) |

### Cancelación, limpieza y cierre de sesión

| Reserva pendiente preparada | Reserva retirada |
| --- | --- |
| ![Reserva preparada para cancelar](screenshots/operations/19-cliente-cancelar-reserva-preparada.jpg) | ![Reserva cancelada](screenshots/operations/21-cliente-reserva-cancelada.jpg) |

| Teléfono retirado | Sesión cerrada |
| --- | --- |
| ![Teléfono retirado](screenshots/operations/22-cliente-telefono-eliminado.jpg) | ![Cierre de sesión](screenshots/operations/23-cliente-cerrar-sesion.jpg) |

## 3. Centro de operaciones

### Resumen

![Panel administrativo](screenshots/operations/14-panel-administrativo.jpg)

### Hoteles

| Crear hotel | Actualizar hotel | Eliminar hotel |
| --- | --- | --- |
| ![Crear hotel](screenshots/operations/15-admin-hoteles-crear.jpg) | ![Actualizar hotel](screenshots/operations/16-admin-hoteles-actualizar.jpg) | ![Eliminar hotel](screenshots/operations/37-admin-hotel-eliminar.jpg) |

### Categorías

| Crear categoría | Actualizar categoría | Eliminar categoría |
| --- | --- | --- |
| ![Crear categoría](screenshots/operations/17-admin-categorias-crear.jpg) | ![Actualizar categoría](screenshots/operations/18-admin-categorias-actualizar.jpg) | ![Eliminar categoría](screenshots/operations/36-admin-categoria-eliminar.jpg) |

### Habitaciones y características

| Crear habitación | Actualizar habitación |
| --- | --- |
| ![Crear habitación](screenshots/operations/19-admin-habitaciones-crear-caracteristicas.jpg) | ![Actualizar habitación](screenshots/operations/20-admin-habitaciones-actualizar.jpg) |

| Eliminar una habitación | Limpieza de habitaciones |
| --- | --- |
| ![Eliminar habitación 902](screenshots/operations/33-admin-habitacion-902-eliminada.jpg) | ![Eliminar habitaciones temporales](screenshots/operations/34-admin-habitaciones-eliminar.jpg) |

### Catálogo de mobiliario

| Crear pieza | Actualizar pieza | Eliminar pieza |
| --- | --- | --- |
| ![Crear mobiliario](screenshots/operations/21-admin-mobiliario-crear.jpg) | ![Actualizar mobiliario](screenshots/operations/22-admin-mobiliario-actualizar.jpg) | ![Eliminar mobiliario](screenshots/operations/35-admin-mobiliario-eliminar.jpg) |

### Inventario por habitación

| Asignar y consultar | Trasladar lote | Retirar pieza |
| --- | --- | --- |
| ![Asignar mobiliario](screenshots/operations/23-admin-inventario-asignar-reporte.jpg) | ![Trasladar mobiliario](screenshots/operations/24-admin-inventario-trasladar.jpg) | ![Retirar mobiliario](screenshots/operations/32-admin-inventario-retirar.jpg) |

### Reservaciones y recepción

| Buscar disponibilidad | Reserva asistida |
| --- | --- |
| ![Buscar disponibilidad](screenshots/operations/25-admin-reservas-buscar-disponibilidad.jpg) | ![Reserva asistida](screenshots/operations/26-admin-reserva-asistida.jpg) |

| Consultar recepción | Check-in | Check-out |
| --- | --- | --- |
| ![Consultar recepción](screenshots/operations/27-admin-recepcion-consultar.jpg) | ![Check-in](screenshots/operations/28-admin-check-in.jpg) | ![Check-out](screenshots/operations/29-admin-check-out.jpg) |

| Paginación de 10 registros | Reserva pendiente para prueba |
| --- | --- |
| ![Paginación](screenshots/operations/30-admin-paginacion-10-registros.jpg) | ![Reserva pendiente administrativa](screenshots/operations/31-admin-reserva-pendiente-cancelacion.jpg) |

### Cierre de sesión

![Cierre de sesión administrativo](screenshots/operations/38-admin-cerrar-sesion.jpg)

## 4. Resultado de la verificación

| Área | Resultado |
| --- | --- |
| Autenticación y roles | Acceso y redirección correctos para cliente y administrador |
| Reservaciones | Creación, retiro, pago y comprobantes PDF verificados |
| Estancias | Próximas, historial, filtros y limpieza de filtros verificados |
| Perfil | Lectura, actualización y teléfonos verificados |
| Administración | Altas, actualizaciones y bajas verificadas |
| Inventario | Asignación, consulta, traslado y retiro verificados |
| Recepción | Consulta, check-in y check-out verificados |
| Paginación | Listados limitados a 10 elementos por página |
| Limpieza | Hotel, categoría, habitaciones, mobiliario, inventario y teléfono temporales quedaron en 0 registros |
