using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DAL;
using EDL;

namespace BLL
{
    public class ClienteLN
    {
        private readonly ClienteDAL _clienteDAL;
        private readonly TelefonoClienteDAL _telefonoDAL;
        private readonly UsuarioDAL _usuarioDAL;
        public readonly StringBuilder stringBuilder = new StringBuilder();

        public ClienteLN()
        {
            _clienteDAL = new ClienteDAL();
            _telefonoDAL = new TelefonoClienteDAL();
            _usuarioDAL = new UsuarioDAL();
        }

        public void RegistrarCliente(Cliente cliente, List<TelefonoCliente> telefonos, Usuario usuario)
        {
            try
            {
                stringBuilder.Clear();

                if (!ValidarCliente(cliente))
                    return;

                if (string.IsNullOrEmpty(usuario.Username))
                    stringBuilder.Append("El usuario es obligatorio." + Environment.NewLine);

                if (string.IsNullOrEmpty(usuario.Password))
                    stringBuilder.Append("La contraseña es obligatoria." + Environment.NewLine);

                if (_clienteDAL.Existe(cliente.Cedula))
                {
                    stringBuilder.Append("El cliente ya existe.");
                    return;
                }

                if (_usuarioDAL.ExisteUsuario(usuario.Username))
                {
                    stringBuilder.Append("El usuario ya existe.");
                    return;
                }

                _clienteDAL.Insert(cliente);

                foreach (var tel in telefonos)
                {
                    tel.CedulaCliente = cliente.Cedula;
                    _telefonoDAL.Insert(tel);
                }

                usuario.CedulaCliente = cliente.Cedula;
                usuario.IdRol = 2;

                _usuarioDAL.Insert(usuario);
            }
            catch (Exception)
            {
                stringBuilder.Clear();
                stringBuilder.Append("Error del sistema.");
            }
        }

        public void ModificarCliente(Cliente cliente, List<TelefonoCliente> telefonos)
        {
            try
            {
                stringBuilder.Clear();

                if (!ValidarCliente(cliente))
                    return;

                _clienteDAL.Update(cliente);

                _telefonoDAL.DeleteByCliente(cliente.Cedula);

                foreach (var tel in telefonos)
                {
                    tel.CedulaCliente = cliente.Cedula;
                    _telefonoDAL.Insert(tel);
                }
            }
            catch (Exception)
            {
                stringBuilder.Clear();
                stringBuilder.Append("Error del sistema.");
            }
        }

        public Cliente TraerCliente(string cedula)
        {
            try
            {
                return _clienteDAL.GetById(cedula);
            }
            catch
            {
                stringBuilder.Clear();
                stringBuilder.Append("Error del sistema.");
                return null;
            }
        }

        public List<TelefonoCliente> TraerTelefonos(string cedula)
        {
            try
            {
                return _telefonoDAL.GetByCliente(cedula);
            }
            catch
            {
                stringBuilder.Clear();
                stringBuilder.Append("Error del sistema.");
                return new List<TelefonoCliente>();
            }
        }

        private bool ValidarCliente(Cliente cliente)
        {
            stringBuilder.Clear();

            if (string.IsNullOrEmpty(cliente.Cedula))
                stringBuilder.Append("La cédula es obligatoria." + Environment.NewLine);

            if (string.IsNullOrEmpty(cliente.Nombre))
                stringBuilder.Append("El nombre es obligatorio." + Environment.NewLine);

            return stringBuilder.Length == 0;
        }
        public bool ExisteCliente(string cedula)
        {
            try
            {
                return _clienteDAL.Existe(cedula);
            }
            catch
            {
                return false;
            }
        }
        public bool ExisteUsuario(string username)
        {
            try
            {
                return _usuarioDAL.ExisteUsuario(username);
            }
            catch
            {
                return false;
            }
        }
        public bool ExisteTelefono(string telefono)
        {
            try
            {
                return _telefonoDAL.ExisteTelefono(telefono);
            }
            catch
            {
                return false;
            }
        }

        //Actualizar mi perfil
        public Usuario TraerUsuarioPorCedula(string cedula)
        {
            return _usuarioDAL.GetByCedula(cedula);
        }

        // Este método actualiza (Cliente, Teléfonos y Usuario)
        public void ModificarPerfilCompleto(Cliente cliente, List<TelefonoCliente> telefonosNuevos, Usuario usuario)
        {
            try
            {
                stringBuilder.Clear();
                if (!ValidarCliente(cliente)) return;

         
                _clienteDAL.Update(cliente);
                _usuarioDAL.Update(usuario);

    
                // Traemos los teléfonos que existen actualmente en la BD
                List<TelefonoCliente> telefonosActuales = _telefonoDAL.GetByCliente(cliente.Cedula);

                // A. Teléfonos a ELIMINAR: Están en la BD pero NO en la lista nueva
                var aEliminar = telefonosActuales
                    .Where(act => !telefonosNuevos.Any(n => n.Telefono == act.Telefono))
                    .ToList();

                // B. Teléfonos a INSERTAR: Están en la lista nueva pero NO en la BD
                var aInsertar = telefonosNuevos
                    .Where(n => !telefonosActuales.Any(act => act.Telefono == n.Telefono))
                    .ToList();

                // Ejecutar eliminaciones
                foreach (var tel in aEliminar)
                {
                    _telefonoDAL.DeleteEspecifico(cliente.Cedula, tel.Telefono);
                }

                // Ejecutar inserciones
                foreach (var tel in aInsertar)
                {
                    tel.CedulaCliente = cliente.Cedula;
                    _telefonoDAL.Insert(tel);
                }

            }
            catch (Exception ex)
            {
                stringBuilder.Clear();
                stringBuilder.Append("Error del sistema: " + ex.Message);
            }
        }
    }

}