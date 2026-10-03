using System;
using System.Collections.Generic;
using EDL;
using DAL;
using System.Data.SqlClient;

namespace BLL
{
    public class CategoriaBLL
    {
        private CategoriaDAL dal = new CategoriaDAL();

        public List<Categoria> ListarCategorias()
        {
            return dal.GetAll();
        }

        public void GuardarCategoria(Categoria cat)
        {
            if (string.IsNullOrWhiteSpace(cat.NombreCategoria))
                throw new Exception("El nombre de la categoría no puede estar vacío.");

            if (cat.PrecioActual <= 0)
                throw new Exception("El precio de la categoría debe ser mayor a cero.");

            try
            {
                dal.Insert(cat);
            }
            catch (Exception ex)
            {
                SqlException sqlEx = ex as SqlException ?? ex.InnerException as SqlException;

                if (sqlEx != null && (sqlEx.Number == 2627 || sqlEx.Number == 2601))
                {
                    throw new Exception($"Lo sentimos, el ID '{cat.IdCategoria}' ya está asignado a otra categoría.");
                }

                throw;
            }
        }

        public void ActualizarPrecio(Categoria cat)
        {
            if (cat.IdCategoria <= 0)
                throw new Exception("El código de la categoría no es válido.");

            if (string.IsNullOrWhiteSpace(cat.NombreCategoria))
                throw new Exception("El nombre de la categoría no puede estar vacío.");

            if (cat.PrecioActual <= 0)
                throw new Exception("No se puede asignar un precio de cero o negativo.");

            dal.Update(cat); 
        }

        public void EliminarCategoria(int idCategoria)
        {
            if (idCategoria <= 0)
                throw new Exception("El código de la categoría no es válido.");

            dal.Delete(idCategoria);
        }
    }
}
