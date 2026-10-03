using System;
using System.Collections.Generic;
using System.Linq;
using EDL;
using DAL;

namespace BLL
{
    public class HotelBLL
    {
        private HotelDAL dal = new HotelDAL();
        private InventarioHabitacionDAL dalInv = new InventarioHabitacionDAL();

        public void GuardarHotel(Hotel hotel)
        {
         
            if (hotel.IdHotel <= 0)
                throw new Exception("El código del hotel debe ser un número positivo.");

            if (string.IsNullOrEmpty(hotel.Direccion))
                throw new Exception("La dirección del hotel es obligatoria.");

            if (string.IsNullOrEmpty(hotel.Telefono))
                throw new Exception("El teléfono del hotel es obligatorio.");

            if (hotel.Telefono.Count(char.IsDigit) != 8)
                throw new Exception("El teléfono debe contener exactamente 8 dígitos.");

            try
            {
                dal.Insert(hotel);
            }
            catch (Exception ex)
            {
                if (ex.Message == "DB_ERROR")
                {
                    throw new Exception($"El ID de hotel '{hotel.IdHotel}' ya existe en el sistema.");
                }

                throw; 
            }
        }

        public List<Hotel> ListarHoteles()
        {
            return dal.GetAll();
        }

        public void ActualizarHotel(Hotel hotel)
        {
            if (hotel.IdHotel <= 0)
                throw new Exception("El código del hotel no es válido.");

            if (string.IsNullOrEmpty(hotel.Direccion))
                throw new Exception("La dirección es obligatoria para actualizar.");

            if (string.IsNullOrEmpty(hotel.Telefono))
                throw new Exception("El teléfono es obligatorio para actualizar.");

            if (hotel.Telefono.Count(char.IsDigit) != 8)
                throw new Exception("El teléfono debe contener exactamente 8 dígitos.");

            dal.Update(hotel);
        }

        public void EliminarHotel(int idHotel)
        {
            if (idHotel <= 0)
                throw new Exception("El código del hotel no es válido.");

            dal.Delete(idHotel);
        }
    }
}
