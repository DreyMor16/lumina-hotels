using System;
using System.Collections.Generic;
using EDL;
using DAL;

namespace BLL
{
    public class HabitacionBLL
    {
        private HabitacionDAL dal = new HabitacionDAL();

        public void GuardarHabitacion(Habitacion hab)
        {
            if (hab.NumHabitacion <= 0)
                throw new Exception("El número de habitación debe ser mayor a cero.");

            if (hab.IdHotel <= 0)
                throw new Exception("Debe seleccionar un hotel válido.");

            if (hab.IdCategoria <= 0)
                throw new Exception("Debe seleccionar una categoría válida.");

            if (dal.Existe(hab.NumHabitacion, hab.IdHotel))
            {
                throw new Exception($"La habitación {hab.NumHabitacion} ya está registrada en este hotel.");
            }

            dal.Insert(hab);
        }

        public List<Habitacion> ListarPorHotel(int idHotel)
        {
            if (idHotel <= 0) return new List<Habitacion>();

            return dal.GetByHotel(idHotel);
        }

        public void ActualizarHabitacion(Habitacion habitacion)
        {
            if (habitacion.NumHabitacion <= 0 || habitacion.IdHotel <= 0)
                throw new Exception("La habitación seleccionada no es válida.");

            if (habitacion.IdCategoria <= 0)
                throw new Exception("Debe seleccionar una categoría válida.");

            dal.Update(habitacion);
        }

        public void EliminarHabitacion(int numHabitacion, int idHotel)
        {
            if (numHabitacion <= 0 || idHotel <= 0)
                throw new Exception("La habitación seleccionada no es válida.");

            dal.Delete(numHabitacion, idHotel);
        }
    }
}
