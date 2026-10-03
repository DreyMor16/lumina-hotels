using EDL;
using System;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;

namespace DAL
{
    public class InventarioHabitacionDAL
    {
        private string cnnString = ConfigurationManager.ConnectionStrings["cnn"].ConnectionString;

        // Método para el reporte solicitado
        public DataTable GetReporteDetallado(int numHab, int idHotel)
        {
            DataTable dt = new DataTable();
            using (SqlConnection cnx = new SqlConnection(cnnString))
            {
                // Traemos la descripción del mueble y la cantidad
                const string sql = @"SELECT M.descripcion AS [Mueble], IH.cantidad AS [Cantidad]
                             FROM Inventario_Habitacion IH
                             INNER JOIN Mobiliario M ON IH.cod_mueble = M.cod_mueble
                             WHERE IH.num_habitacion = @num AND IH.id_hotel = @hotel";

                SqlDataAdapter da = new SqlDataAdapter(sql, cnx);
                da.SelectCommand.Parameters.AddWithValue("@num", numHab);
                da.SelectCommand.Parameters.AddWithValue("@hotel", idHotel);
                da.Fill(dt);
            }
            return dt;
        }

        public void AsignarMueble(InventarioHabitacion inv)
        {
            try
            {
                using (SqlConnection cnx = new SqlConnection(cnnString))
                {
                    cnx.Open();
                    const string sql = @"
                        IF EXISTS (SELECT 1 FROM Inventario_Habitacion
                                   WHERE num_habitacion = @num AND id_hotel = @hotel AND cod_mueble = @cod)
                            UPDATE Inventario_Habitacion
                            SET cantidad = cantidad + @cant
                            WHERE num_habitacion = @num AND id_hotel = @hotel AND cod_mueble = @cod
                        ELSE
                            INSERT INTO Inventario_Habitacion (num_habitacion, id_hotel, cod_mueble, cantidad)
                            VALUES (@num, @hotel, @cod, @cant)";
                    using (SqlCommand cmd = new SqlCommand(sql, cnx))
                    {
                        cmd.Parameters.AddWithValue("@num", inv.NumHabitacion);
                        cmd.Parameters.AddWithValue("@hotel", inv.IdHotel);
                        cmd.Parameters.AddWithValue("@cod", inv.CodMueble);
                        cmd.Parameters.AddWithValue("@cant", inv.Cantidad);
                        cmd.ExecuteNonQuery();
                    }
                }
            }
            catch (SqlException) { throw new Exception("No fue posible asignar el mueble a la habitación."); }
        }

        public DataTable GetMobiliarioPorHabitacion(int numHab, int idHotel)
        {
            DataTable dt = new DataTable();
            using (SqlConnection cnx = new SqlConnection(cnnString))
            {
                const string sql = @"SELECT M.cod_mueble, M.descripcion, IH.cantidad
                             FROM Inventario_Habitacion IH
                             INNER JOIN Mobiliario M ON IH.cod_mueble = M.cod_mueble
                             WHERE IH.num_habitacion = @num AND IH.id_hotel = @hotel";
                SqlDataAdapter da = new SqlDataAdapter(sql, cnx);
                da.SelectCommand.Parameters.AddWithValue("@num", numHab);
                da.SelectCommand.Parameters.AddWithValue("@hotel", idHotel);
                da.Fill(dt);
            }
            return dt;
        }

        public void EliminarMuebleDeHabitacion(int numHabitacion, int idHotel, int codMueble)
        {
            using (SqlConnection cnx = new SqlConnection(cnnString))
            {
                cnx.Open();
                const string sql = @"DELETE FROM Inventario_Habitacion
                                     WHERE num_habitacion = @numero
                                       AND id_hotel = @hotel
                                       AND cod_mueble = @mueble";
                using (SqlCommand cmd = new SqlCommand(sql, cnx))
                {
                    cmd.Parameters.AddWithValue("@numero", numHabitacion);
                    cmd.Parameters.AddWithValue("@hotel", idHotel);
                    cmd.Parameters.AddWithValue("@mueble", codMueble);
                    if (cmd.ExecuteNonQuery() == 0)
                        throw new Exception("La asignación de mobiliario ya no existe.");
                }
            }
        }

        public void TrasladarMueble(int idHotel, int habOrigen, int habDestino, int codMueble)
        {
            using (SqlConnection cnx = new SqlConnection(cnnString))
            {
                cnx.Open();
                SqlTransaction trans = cnx.BeginTransaction();

                try
                {
                    const string sqlExiste = @"SELECT COUNT(*) FROM Inventario_Habitacion
                                               WHERE id_hotel = @hotel AND num_habitacion = @orig AND cod_mueble = @cod";
                    using (SqlCommand cmdExiste = new SqlCommand(sqlExiste, cnx, trans))
                    {
                        cmdExiste.Parameters.AddWithValue("@hotel", idHotel);
                        cmdExiste.Parameters.AddWithValue("@orig", habOrigen);
                        cmdExiste.Parameters.AddWithValue("@cod", codMueble);
                        if ((int)cmdExiste.ExecuteScalar() == 0)
                            throw new Exception("El mueble seleccionado ya no está asignado a la habitación de origen.");
                    }

                    const string sqlUpsert = @"
                IF EXISTS (SELECT 1 FROM Inventario_Habitacion WHERE id_hotel = @hotel AND num_habitacion = @dest AND cod_mueble = @cod)
                BEGIN
                    UPDATE Inventario_Habitacion 
                    SET cantidad = cantidad + (SELECT cantidad FROM Inventario_Habitacion WHERE id_hotel = @hotel AND num_habitacion = @orig AND cod_mueble = @cod)
                    WHERE id_hotel = @hotel AND num_habitacion = @dest AND cod_mueble = @cod
                END
                ELSE
                BEGIN
                    INSERT INTO Inventario_Habitacion (num_habitacion, id_hotel, cod_mueble, cantidad)
                    SELECT @dest, @hotel, @cod, cantidad 
                    FROM Inventario_Habitacion 
                    WHERE id_hotel = @hotel AND num_habitacion = @orig AND cod_mueble = @cod
                END";

                    using (SqlCommand cmdUpsert = new SqlCommand(sqlUpsert, cnx, trans))
                    {
                        cmdUpsert.Parameters.AddWithValue("@hotel", idHotel);
                        cmdUpsert.Parameters.AddWithValue("@orig", habOrigen);
                        cmdUpsert.Parameters.AddWithValue("@dest", habDestino);
                        cmdUpsert.Parameters.AddWithValue("@cod", codMueble);
                        cmdUpsert.ExecuteNonQuery();
                    }

                    const string sqlDelete = @"DELETE FROM Inventario_Habitacion 
                                     WHERE id_hotel = @hotel AND num_habitacion = @orig AND cod_mueble = @cod";

                    using (SqlCommand cmdDelete = new SqlCommand(sqlDelete, cnx, trans))
                    {
                        cmdDelete.Parameters.AddWithValue("@hotel", idHotel);
                        cmdDelete.Parameters.AddWithValue("@orig", habOrigen);
                        cmdDelete.Parameters.AddWithValue("@cod", codMueble);
                        cmdDelete.ExecuteNonQuery();
                    }

                    trans.Commit(); 
                }
                catch (Exception ex)
                {
                    trans.Rollback(); 
                    throw new Exception("Error en el traslado: " + ex.Message);
                }
            }
        }
    }
}
