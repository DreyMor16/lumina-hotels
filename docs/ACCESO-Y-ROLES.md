# Acceso y roles

Lúmina Hotels utiliza un único inicio de sesión para huéspedes y administradores. La autorización depende del campo `id_rol` almacenado en la tabla `Usuario`.

## Flujo de acceso

1. `Login.aspx` recibe el usuario y la contraseña.
2. `UsuarioBLL.Login` aplica la lógica de negocio y consulta los datos mediante la capa DAL.
3. Si las credenciales son correctas, se guardan en sesión la cédula, el nombre de usuario y el rol.
4. El rol determina el destino:
   - **Rol 1:** `AdminDashboard.aspx`.
   - **Rol 2:** `PerfilCliente.aspx`.
5. `Admin.Master` protege todas las páginas administrativas y `Site.Master` protege el portal del huésped.

## Accesos locales

La base de datos de desarrollo contiene dos perfiles con propósitos diferentes:

| Usuario | Rol | Uso |
| --- | ---: | --- |
| `admin` | 1 | Centro de operaciones |
| `dreymor` | 2 | Portal del huésped |

La contraseña inicial del administrador está definida en `DB_Hotel.sql`. Debe cambiarse antes de desplegar el proyecto en un entorno público.

## Páginas principales

- `Default.aspx`: portada pública.
- `Login.aspx`: acceso único para todas las cuentas. El destino se decide automáticamente después de validar las credenciales.
- `AdminDashboard.aspx`: catálogo y operación.
- `AdminReservas.aspx`: reservaciones, llegadas y salidas.

## Nota de seguridad

El proyecto académico conserva el mecanismo original de contraseñas. Para producción se recomienda almacenar hashes con sal y aplicar bloqueo temporal después de varios intentos fallidos.
