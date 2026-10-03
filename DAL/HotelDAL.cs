using EDL;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Configuration;

namespace DAL
{
    public class HotelDAL
    {
        private string cnnString = ConfigurationManager.ConnectionStrings["cnn"].ConnectionString;

        public void Insert(Hotel hotel)
        {
            try
            {
                using (SqlConnection cnx = new SqlConnection(cnnString))
                {
                    cnx.Open();
                    const string sql = "INSERT INTO Hotel (id_hotel, direccion, telefono) VALUES (@id, @dir, @tel)";
                    using (SqlCommand cmd = new SqlCommand(sql, cnx))
                    {
                        cmd.Parameters.AddWithValue("@id", hotel.IdHotel);
                        cmd.Parameters.AddWithValue("@dir", hotel.Direccion);
                        cmd.Parameters.AddWithValue("@tel", hotel.Telefono);
                        cmd.ExecuteNonQuery();
                    }
                }
            }
            catch (SqlException) { throw new Exception("DB_ERROR"); }
        }

        public List<Hotel> GetAll()
        {
            List<Hotel> lista = new List<Hotel>();
            using (SqlConnection cnx = new SqlConnection(cnnString))
            {
                cnx.Open();
                const string sql = "SELECT * FROM Hotel";
                using (SqlCommand cmd = new SqlCommand(sql, cnx))
                {
                    SqlDataReader dr = cmd.ExecuteReader();
                    while (dr.Read())
                    {
                        lista.Add(new Hotel
                        {
                            IdHotel = (int)dr["id_hotel"],
                            Direccion = dr["direccion"].ToString(),
                            Telefono = dr["telefono"].ToString()
                        });
                    }
                }
            }
            return lista;
        }

        public void Update(Hotel hotel)
        {
            try
            {
                using (SqlConnection cnx = new SqlConnection(cnnString))
                {
                    cnx.Open();
                    const string sql = "UPDATE Hotel SET direccion = @dir, telefono = @tel WHERE id_hotel = @id";
                    using (SqlCommand cmd = new SqlCommand(sql, cnx))
                    {
                        cmd.Parameters.AddWithValue("@id", hotel.IdHotel);
                        cmd.Parameters.AddWithValue("@dir", hotel.Direccion);
                        cmd.Parameters.AddWithValue("@tel", hotel.Telefono);
                        if (cmd.ExecuteNonQuery() == 0)
                            throw new Exception("El hotel indicado ya no existe.");
                    }
                }
            }
            catch (SqlException) { throw new Exception("Error al actualizar en la base de datos."); }
        }

        public void Delete(int idHotel)
        {
            try
            {
                using (SqlConnection cnx = new SqlConnection(cnnString))
                {
                    cnx.Open();
                    const string sql = "DELETE FROM Hotel WHERE id_hotel = @id";
                    using (SqlCommand cmd = new SqlCommand(sql, cnx))
                    {
                        cmd.Parameters.AddWithValue("@id", idHotel);
                        if (cmd.ExecuteNonQuery() == 0)
                            throw new Exception("El hotel indicado ya no existe.");
                    }
                }
            }
            catch (SqlException ex)
            {
                if (ex.Number == 547)
                    throw new Exception("No se puede eliminar el hotel porque todavía tiene habitaciones asociadas.");
                throw new Exception("No fue posible eliminar el hotel.");
            }
        }
    }
}
