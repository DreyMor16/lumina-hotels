using System;
using System.Collections.Generic;
using EDL;
using DAL;

namespace BLL
{
    public class MobiliarioBLL
    {
        private MobiliarioDAL dal = new MobiliarioDAL();

        public void RegistrarMueble(Mobiliario mueble)
        {
            if (mueble.CodMueble <= 0)
                throw new Exception("El código de mueble debe ser un número positivo.");

            if (string.IsNullOrWhiteSpace(mueble.Descripcion))
                throw new Exception("La descripción del mueble es obligatoria.");

            if (mueble.PrecioMueble < 0)
                throw new Exception("El precio no puede ser negativo.");


            try
            {
                dal.Insert(mueble);
            }
            catch (Exception ex)
            {
                if (ex.Message == "DB_ERROR")
                {
                    throw new Exception($"El ID de mobiliario '{mueble.CodMueble}' ya existe en el sistema.");
                }

                throw;
            }
        }

        public List<Mobiliario> ListarCatalogo()
        {
            return dal.GetAll();
        }

        public Mobiliario BuscarPorCodigo(int codigo)
        {
            if (codigo <= 0) return null;
            return dal.GetById(codigo);
        }

        public void ActualizarMueble(Mobiliario mueble)
        {
            if (mueble.CodMueble <= 0)
                throw new Exception("El código del mueble no es válido.");

            if (string.IsNullOrWhiteSpace(mueble.Descripcion))
                throw new Exception("La descripción es obligatoria para actualizar.");

            if (mueble.PrecioMueble < 0)
                throw new Exception("El precio no puede ser negativo.");

            dal.Update(mueble);
        }

        public void EliminarMueble(int codigo)
        {
            if (codigo <= 0)
                throw new Exception("El código del mueble no es válido.");

            dal.Delete(codigo);
        }
    }
}
