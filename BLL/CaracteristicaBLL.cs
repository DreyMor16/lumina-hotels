using System;
using System.Collections.Generic;
using EDL;
using DAL;

namespace BLL
{
    public class CaracteristicaBLL
    {
        private CaracteristicaDAL dal = new CaracteristicaDAL();

        public List<Caracteristica> ListarCatalogo()
        {
            // Retorna el catálogo: 1-Soleada, 2-Lavado, 3-Nevera, etc.
            return dal.GetAll();
        }

        public void AsignarCaracteristica(DetalleHabitacion detalle)
        {
            if (detalle.NumHabitacion <= 0 || detalle.IdHotel <= 0)
                throw new Exception("La habitación seleccionada no es válida.");

            if (detalle.IdCaracteristica <= 0)
                throw new Exception("Debe seleccionar una característica válida.");

            dal.VincularAHabitacion(detalle);
        }

        public List<Caracteristica> ListarPorHabitacion(int numHabitacion, int idHotel)
        {
            if (numHabitacion <= 0 || idHotel <= 0)
                return new List<Caracteristica>();

            return dal.GetByRoom(numHabitacion, idHotel);
        }

        public void ReemplazarCaracteristicas(int numHabitacion, int idHotel, IEnumerable<int> caracteristicas)
        {
            if (numHabitacion <= 0 || idHotel <= 0)
                throw new Exception("La habitación seleccionada no es válida.");

            dal.ReplaceForRoom(numHabitacion, idHotel, caracteristicas);
        }
    }
}
