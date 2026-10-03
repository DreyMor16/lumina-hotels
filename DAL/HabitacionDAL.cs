using EDL;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Configuration;

namespace DAL
{
    public class HabitacionDAL
    {
        private string cnnString = ConfigurationManager.ConnectionStrings["cnn"].ConnectionString;

        public void Insert(Habitacion hab)
        {
            try
            {
                using (SqlConnection cnx = new SqlConnection(cnnString))
                {
                    cnx.Open();
                    const string sql = @"INSERT INTO Habitacion 
                                 (num_habitacion, id_hotel, id_categoria) 
                                 VALUES (@num, @idHotel, @idCat)";

                    using (SqlCommand cmd = new SqlCommand(sql, cnx))
                    {
                        cmd.Parameters.AddWithValue("@num", hab.NumHabitacion);
                        cmd.Parameters.AddWithValue("@idHotel", hab.IdHotel);
                        cmd.Parameters.AddWithValue("@idCat", hab.IdCategoria);

                        cmd.ExecuteNonQuery();
                    }
                }
            }
            catch (SqlException) { throw new Exception("DB_ERROR"); }
        }

        public List<Habitacion> GetByHotel(int idHotel)
        {
            List<Habitacion> lista = new List<Habitacion>();
            using (SqlConnection cnx = new SqlConnection(cnnString))
            {
                cnx.Open();
                const string sql = @"
            SELECT h.num_habitacion, 
                   h.id_hotel, 
                   h.id_categoria, 
                   cat.nombre_categoria, -- Traemos el nombre
                   ISNULL(STRING_AGG(c.nombre_caracteristica, ', '), 'Sin características') AS Caracteristicas
            FROM Habitacion h
            INNER JOIN Categoria cat ON h.id_categoria = cat.id_categoria -- Join con Categoria
            LEFT JOIN Detalle_Habitacion dh ON h.num_habitacion = dh.num_habitacion AND h.id_hotel = dh.id_hotel
            LEFT JOIN Caracteristica c ON dh.id_caracteristica = c.id_caracteristica
            WHERE h.id_hotel = @idHotel
            GROUP BY h.num_habitacion, h.id_hotel, h.id_categoria, cat.nombre_categoria"; // Agregamos al GROUP BY

                using (SqlCommand cmd = new SqlCommand(sql, cnx))
                {
                    cmd.Parameters.AddWithValue("@idHotel", idHotel);
                    SqlDataReader dr = cmd.ExecuteReader();
                    while (dr.Read())
                    {
                        lista.Add(new Habitacion
                        {
                            NumHabitacion = (int)dr["num_habitacion"],
                            IdHotel = (int)dr["id_hotel"],
                            IdCategoria = (int)dr["id_categoria"],
                            NombreCategoria = dr["nombre_categoria"].ToString(), // Mapeamos el nombre
                            CaracteristicasTexto = dr["Caracteristicas"].ToString()
                        });
                    }
                }
            }
            return lista;
        }

        public bool Existe(int numHab, int idHotel)
        {
            using (SqlConnection cnx = new SqlConnection(cnnString))
            {
                cnx.Open();
                const string sql = "SELECT COUNT(*) FROM Habitacion WHERE num_habitacion = @num AND id_hotel = @id";
                using (SqlCommand cmd = new SqlCommand(sql, cnx))
                {
                    cmd.Parameters.AddWithValue("@num", numHab);
                    cmd.Parameters.AddWithValue("@id", idHotel);
                    int cantidad = (int)cmd.ExecuteScalar();
                    return cantidad > 0;
                }
            }
        }

        public void Update(Habitacion habitacion)
        {
            using (SqlConnection cnx = new SqlConnection(cnnString))
            {
                cnx.Open();
                const string sql = @"UPDATE Habitacion
                                     SET id_categoria = @categoria
                                     WHERE num_habitacion = @numero AND id_hotel = @hotel";
                using (SqlCommand cmd = new SqlCommand(sql, cnx))
                {
                    cmd.Parameters.AddWithValue("@categoria", habitacion.IdCategoria);
                    cmd.Parameters.AddWithValue("@numero", habitacion.NumHabitacion);
                    cmd.Parameters.AddWithValue("@hotel", habitacion.IdHotel);
                    if (cmd.ExecuteNonQuery() == 0)
                        throw new Exception("La habitación indicada ya no existe.");
                }
            }
        }

        public void Delete(int numHabitacion, int idHotel)
        {
            using (SqlConnection cnx = new SqlConnection(cnnString))
            {
                cnx.Open();
                SqlTransaction transaction = cnx.BeginTransaction();
                try
                {
                    const string deleteFeatures = @"DELETE FROM Detalle_Habitacion
                                                    WHERE num_habitacion = @numero AND id_hotel = @hotel";
                    using (SqlCommand cmd = new SqlCommand(deleteFeatures, cnx, transaction))
                    {
                        cmd.Parameters.AddWithValue("@numero", numHabitacion);
                        cmd.Parameters.AddWithValue("@hotel", idHotel);
                        cmd.ExecuteNonQuery();
                    }

                    const string deleteInventory = @"DELETE FROM Inventario_Habitacion
                                                     WHERE num_habitacion = @numero AND id_hotel = @hotel";
                    using (SqlCommand cmd = new SqlCommand(deleteInventory, cnx, transaction))
                    {
                        cmd.Parameters.AddWithValue("@numero", numHabitacion);
                        cmd.Parameters.AddWithValue("@hotel", idHotel);
                        cmd.ExecuteNonQuery();
                    }

                    const string deleteRoom = @"DELETE FROM Habitacion
                                                WHERE num_habitacion = @numero AND id_hotel = @hotel";
                    using (SqlCommand cmd = new SqlCommand(deleteRoom, cnx, transaction))
                    {
                        cmd.Parameters.AddWithValue("@numero", numHabitacion);
                        cmd.Parameters.AddWithValue("@hotel", idHotel);
                        if (cmd.ExecuteNonQuery() == 0)
                            throw new Exception("La habitación indicada ya no existe.");
                    }

                    transaction.Commit();
                }
                catch (SqlException ex)
                {
                    transaction.Rollback();
                    if (ex.Number == 547)
                        throw new Exception("No se puede eliminar la habitación porque forma parte de una reservación.");
                    throw new Exception("No fue posible eliminar la habitación.");
                }
                catch
                {
                    transaction.Rollback();
                    throw;
                }
            }
        }
    }
}
