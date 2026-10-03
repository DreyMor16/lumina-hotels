using DAL;
using EDL;
using System;
using System.Data;

namespace BLL
{
    public class InventarioHabitacionBLL
    {
        private InventarioHabitacionDAL dal = new InventarioHabitacionDAL();

        public void AsignarMuebleAHabitacion(InventarioHabitacion inv)
        {
            if (inv.IdHotel <= 0 || inv.NumHabitacion <= 0)
                throw new Exception("La información de la habitación no es válida.");

            if (inv.CodMueble <= 0)
                throw new Exception("Debe seleccionar un mueble válido del catálogo.");

            if (inv.Cantidad <= 0)
                throw new Exception("La cantidad de muebles debe ser al menos 1.");

            dal.AsignarMueble(inv);
        }

        public DataTable ObtenerReporteTabular(int numHab, int idHotel)
        {
            if (numHab <= 0 || idHotel <= 0)
                throw new Exception("Selección de hotel o habitación no válida.");

            return dal.GetReporteDetallado(numHab, idHotel);
        }

        public DataTable ListarMueblesDeHabitacion(int numHab, int idHotel)
        {
            return dal.GetMobiliarioPorHabitacion(numHab, idHotel);
        }

        public void TrasladarMueble(int idHotel, int habOrigen, int habDestino, int codMueble)
        {
            if (idHotel <= 0 || habOrigen <= 0 || habDestino <= 0 || codMueble <= 0)
                throw new Exception("Complete correctamente todos los datos del traslado.");

            if (habOrigen == habDestino)
                throw new Exception("La habitación de destino debe ser diferente a la de origen.");

            dal.TrasladarMueble(idHotel, habOrigen, habDestino, codMueble);
        }

        public void EliminarMuebleDeHabitacion(int numHabitacion, int idHotel, int codMueble)
        {
            if (numHabitacion <= 0 || idHotel <= 0 || codMueble <= 0)
                throw new Exception("La asignación seleccionada no es válida.");

            dal.EliminarMuebleDeHabitacion(numHabitacion, idHotel, codMueble);
        }
    }
}
