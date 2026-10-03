using EDL;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Configuration;
using System.Linq;

namespace DAL
{
    public class CaracteristicaDAL
    {
        private string cnnString = ConfigurationManager.ConnectionStrings["cnn"].ConnectionString;

        public List<Caracteristica> GetAll()
        {
            List<Caracteristica> lista = new List<Caracteristica>();
            using (SqlConnection cnx = new SqlConnection(cnnString))
            {
                cnx.Open();
                const string sql = "SELECT * FROM Caracteristica";
                using (SqlCommand cmd = new SqlCommand(sql, cnx))
                {
                    SqlDataReader dr = cmd.ExecuteReader();
                    while (dr.Read())
                    {
                        lista.Add(new Caracteristica
                        {
                            IdCaracteristica = (int)dr["id_caracteristica"],
                            NombreCaracteristica = dr["nombre_caracteristica"].ToString()
                        });
                    }
                }
            }
            return lista;
        }

        public void VincularAHabitacion(DetalleHabitacion detalle)
        {
            try
            {
                using (SqlConnection cnx = new SqlConnection(cnnString))
                {
                    cnx.Open();
                    const string sql = "INSERT INTO Detalle_Habitacion (num_habitacion, id_hotel, id_caracteristica) VALUES (@num, @hotel, @carac)";
                    using (SqlCommand cmd = new SqlCommand(sql, cnx))
                    {
                        cmd.Parameters.AddWithValue("@num", detalle.NumHabitacion);
                        cmd.Parameters.AddWithValue("@hotel", detalle.IdHotel);
                        cmd.Parameters.AddWithValue("@carac", detalle.IdCaracteristica);
                        cmd.ExecuteNonQuery();
                    }
                }
            }
            catch (SqlException) { throw new Exception("DB_ERROR"); }
        }

        public List<Caracteristica> GetByRoom(int numHabitacion, int idHotel)
        {
            List<Caracteristica> lista = new List<Caracteristica>();
            using (SqlConnection cnx = new SqlConnection(cnnString))
            {
                cnx.Open();
                const string sql = @"SELECT c.id_caracteristica, c.nombre_caracteristica
                                     FROM Detalle_Habitacion d
                                     INNER JOIN Caracteristica c ON c.id_caracteristica = d.id_caracteristica
                                     WHERE d.num_habitacion = @numero AND d.id_hotel = @hotel
                                     ORDER BY c.nombre_caracteristica";
                using (SqlCommand cmd = new SqlCommand(sql, cnx))
                {
                    cmd.Parameters.AddWithValue("@numero", numHabitacion);
                    cmd.Parameters.AddWithValue("@hotel", idHotel);
                    using (SqlDataReader dr = cmd.ExecuteReader())
                    {
                        while (dr.Read())
                        {
                            lista.Add(new Caracteristica
                            {
                                IdCaracteristica = (int)dr["id_caracteristica"],
                                NombreCaracteristica = dr["nombre_caracteristica"].ToString()
                            });
                        }
                    }
                }
            }
            return lista;
        }

        public void ReplaceForRoom(int numHabitacion, int idHotel, IEnumerable<int> caracteristicas)
        {
            int[] ids = (caracteristicas ?? Enumerable.Empty<int>()).Distinct().ToArray();
            using (SqlConnection cnx = new SqlConnection(cnnString))
            {
                cnx.Open();
                SqlTransaction transaction = cnx.BeginTransaction();
                try
                {
                    const string deleteSql = @"DELETE FROM Detalle_Habitacion
                                               WHERE num_habitacion = @numero AND id_hotel = @hotel";
                    using (SqlCommand delete = new SqlCommand(deleteSql, cnx, transaction))
                    {
                        delete.Parameters.AddWithValue("@numero", numHabitacion);
                        delete.Parameters.AddWithValue("@hotel", idHotel);
                        delete.ExecuteNonQuery();
                    }

                    const string insertSql = @"INSERT INTO Detalle_Habitacion
                                               (num_habitacion, id_hotel, id_caracteristica)
                                               VALUES (@numero, @hotel, @caracteristica)";
                    foreach (int id in ids)
                    {
                        using (SqlCommand insert = new SqlCommand(insertSql, cnx, transaction))
                        {
                            insert.Parameters.AddWithValue("@numero", numHabitacion);
                            insert.Parameters.AddWithValue("@hotel", idHotel);
                            insert.Parameters.AddWithValue("@caracteristica", id);
                            insert.ExecuteNonQuery();
                        }
                    }

                    transaction.Commit();
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
