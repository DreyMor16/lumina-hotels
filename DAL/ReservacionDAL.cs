using EDL;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.SqlClient;

namespace DAL
{
    public class ReservacionDAL
    {
        public int CrearReservacion(Reservacion reserva)
        {
            using (SqlConnection cnx =
                new SqlConnection(ConfigurationManager.ConnectionStrings["cnn"].ConnectionString))
            {
                cnx.Open();

                string sql = @"

                INSERT INTO Reservacion
                (cedula_cliente, monto_total)
                OUTPUT INSERTED.id_reservacion
                VALUES
                (@cedula,@monto)";

                SqlCommand cmd = new SqlCommand(sql, cnx);

                cmd.Parameters.AddWithValue("@cedula", reserva.CedulaCliente);
                cmd.Parameters.AddWithValue("@monto", reserva.MontoTotal);

                return (int)cmd.ExecuteScalar();
            }
        }

        public void CrearDetalle(Detalle_Reservacion detalle)
        {
            using (SqlConnection cnx =
                new SqlConnection(ConfigurationManager.ConnectionStrings["cnn"].ConnectionString))
            {
                cnx.Open();

                string sql = @"

                INSERT INTO Detalle_Reservacion
                (id_reservacion,num_habitacion,id_hotel,
                fecha_llegada,fecha_salida,precio_noche_aplicado)
                VALUES
                (@reserva,@habitacion,@hotel,@llegada,@salida,@precio)";

                SqlCommand cmd = new SqlCommand(sql, cnx);

                cmd.Parameters.AddWithValue("@reserva", detalle.IdReservacion);
                cmd.Parameters.AddWithValue("@habitacion", detalle.NumHabitacion);
                cmd.Parameters.AddWithValue("@hotel", detalle.IdHotel);
                cmd.Parameters.AddWithValue("@llegada", detalle.FechaLlegada);
                cmd.Parameters.AddWithValue("@salida", detalle.FechaSalida);
                cmd.Parameters.AddWithValue("@precio", detalle.PrecioNoche);

                cmd.ExecuteNonQuery();
            }
        }

        // Busca si el cliente ya tiene una reserva abierta hoy para no crear otra
        public int ObtenerReservaPendiente(string cedula)
        {
            using (SqlConnection cnx = new SqlConnection(ConfigurationManager.ConnectionStrings["cnn"].ConnectionString))
            {
                // Eliminamos la validación de la fecha 
                // para permitir pagar reservas de días anteriores.
                string sql = @"SELECT TOP 1 id_reservacion 
                       FROM Reservacion 
                       WHERE cedula_cliente = @cedula 
                       AND estado = 'Pendiente' 
                       ORDER BY id_reservacion DESC";

                SqlCommand cmd = new SqlCommand(sql, cnx);
                cmd.Parameters.AddWithValue("@cedula", cedula);
                cnx.Open();
                object result = cmd.ExecuteScalar();
                return (result != null) ? Convert.ToInt32(result) : 0;
            }
        }

        // Suma el monto de la nueva habitación al total de la reservación
        public void ActualizarMontoReserva(int idReserva, decimal montoASumar)
        {
            using (SqlConnection cnx = new SqlConnection(ConfigurationManager.ConnectionStrings["cnn"].ConnectionString))
            {
                string sql = "UPDATE Reservacion SET monto_total = monto_total + @monto WHERE id_reservacion = @id";
                SqlCommand cmd = new SqlCommand(sql, cnx);
                cmd.Parameters.AddWithValue("@monto", montoASumar);
                cmd.Parameters.AddWithValue("@id", idReserva);
                cnx.Open();
                cmd.ExecuteNonQuery();
            }
        }

        // Para mostrar reporte de consultas
        // Método 1: Listar las habitaciones reservadas del cliente
        public List<ReservaVista> ConsultarReservasPorCliente(string cedula)
        {
            List<ReservaVista> lista = new List<ReservaVista>();

            using (SqlConnection cnx = new SqlConnection(ConfigurationManager.ConnectionStrings["cnn"].ConnectionString))
            {
                string sql = @"
        SELECT  
            r.id_reservacion, 
            r.cedula_cliente,
            d.num_habitacion, 
            d.id_hotel, 
            h.direccion AS nombre_hotel,
            c.nombre_categoria,

            STRING_AGG(ca.nombre_caracteristica, ', ') AS amenidades,

            d.fecha_llegada, 
            d.fecha_salida, 
            d.precio_noche_aplicado,
            DATEDIFF(day, d.fecha_llegada, d.fecha_salida) AS dias

        FROM Reservacion r
        INNER JOIN Detalle_Reservacion d 
            ON r.id_reservacion = d.id_reservacion

        INNER JOIN Hotel h 
            ON d.id_hotel = h.id_hotel

        INNER JOIN Habitacion ha 
            ON d.num_habitacion = ha.num_habitacion 
            AND d.id_hotel = ha.id_hotel

        INNER JOIN Categoria c 
            ON ha.id_categoria = c.id_categoria

        LEFT JOIN Detalle_Habitacion dh 
            ON ha.num_habitacion = dh.num_habitacion 
            AND ha.id_hotel = dh.id_hotel

        LEFT JOIN Caracteristica ca 
            ON dh.id_caracteristica = ca.id_caracteristica

        WHERE r.cedula_cliente = @cedula 
        AND r.estado = 'Pendiente'

        GROUP BY 
            r.id_reservacion, 
            r.cedula_cliente,
            d.num_habitacion, 
            d.id_hotel, 
            h.direccion,
            c.nombre_categoria,
            d.fecha_llegada, 
            d.fecha_salida, 
            d.precio_noche_aplicado
        ";

                SqlCommand cmd = new SqlCommand(sql, cnx);
                cmd.Parameters.AddWithValue("@cedula", cedula);

                cnx.Open();

                using (SqlDataReader reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        ReservaVista rev = new ReservaVista();

                        rev.IdReservacion = Convert.ToInt32(reader["id_reservacion"]);
                        rev.CedulaCliente = reader["cedula_cliente"].ToString();
                        rev.NumHabitacion = Convert.ToInt32(reader["num_habitacion"]);
                        rev.IdHotel = Convert.ToInt32(reader["id_hotel"]);
                        rev.NombreHotel = reader["nombre_hotel"].ToString();
                        rev.NombreCategoria = reader["nombre_categoria"].ToString();

                        rev.Amenidades = reader["amenidades"] != DBNull.Value
                            ? reader["amenidades"].ToString()
                            : "Estándar";

                        rev.FechaLlegada = Convert.ToDateTime(reader["fecha_llegada"]);
                        rev.FechaSalida = Convert.ToDateTime(reader["fecha_salida"]);
                        rev.PrecioNoche = Convert.ToDecimal(reader["precio_noche_aplicado"]);

                        int dias = Convert.ToInt32(reader["dias"]);
                        rev.TotalDias = dias <= 0 ? 1 : dias;

                        rev.SubTotal = rev.TotalDias * rev.PrecioNoche;

                        lista.Add(rev);
                    }
                }
            }

            return lista;
        }
        // Método 2: Cancelar una tarjeta (Eliminar detalle y restar monto)
        public void CancelarDetalleReserva(int idReservacion, int numHabitacion, int idHotel, decimal montoARestar, DateTime fechaLlegada, DateTime fechaSalida)
        {
            using (SqlConnection cnx = new SqlConnection(ConfigurationManager.ConnectionStrings["cnn"].ConnectionString))
            {
                cnx.Open();
                SqlTransaction trx = cnx.BeginTransaction();

                try
                {
                    // ELIMINAR SOLO UNA RESERVA ESPECÍFICA (CON FECHAS)
                    string sqlDelete = @"DELETE FROM Detalle_Reservacion 
                                 WHERE id_reservacion = @id 
                                 AND num_habitacion = @num 
                                 AND id_hotel = @hotel
                                 AND fecha_llegada = @llegada
                                 AND fecha_salida = @salida";

                    SqlCommand cmdDel = new SqlCommand(sqlDelete, cnx, trx);
                    cmdDel.Parameters.AddWithValue("@id", idReservacion);
                    cmdDel.Parameters.AddWithValue("@num", numHabitacion);
                    cmdDel.Parameters.AddWithValue("@hotel", idHotel);
                    cmdDel.Parameters.AddWithValue("@llegada", fechaLlegada);
                    cmdDel.Parameters.AddWithValue("@salida", fechaSalida);

                    int filas = cmdDel.ExecuteNonQuery();

                    if (filas == 0)
                        throw new Exception("No se encontró la reservación específica.");

                    //ACTUALIZAR MONTO
                    string sqlUpdate = @"UPDATE Reservacion 
                                 SET monto_total = CASE 
                                     WHEN monto_total - @monto < 0 THEN 0
                                     ELSE monto_total - @monto 
                                 END
                                 WHERE id_reservacion = @id";

                    SqlCommand cmdUpd = new SqlCommand(sqlUpdate, cnx, trx);
                    cmdUpd.Parameters.AddWithValue("@monto", montoARestar);
                    cmdUpd.Parameters.AddWithValue("@id", idReservacion);
                    cmdUpd.ExecuteNonQuery();

                    // VERIFICAR SI QUEDAN DETALLES
                    string sqlCheck = @"SELECT COUNT(*) 
                                FROM Detalle_Reservacion 
                                WHERE id_reservacion = @id";

                    SqlCommand cmdCheck = new SqlCommand(sqlCheck, cnx, trx);
                    cmdCheck.Parameters.AddWithValue("@id", idReservacion);

                    int cantidad = Convert.ToInt32(cmdCheck.ExecuteScalar());

                    // SI NO QUEDA NADA → BORRAR RESERVA
                    if (cantidad == 0)
                    {
                        string sqlDeleteReserva = @"DELETE FROM Reservacion 
                                            WHERE id_reservacion = @id";

                        SqlCommand cmdDelete = new SqlCommand(sqlDeleteReserva, cnx, trx);
                        cmdDelete.Parameters.AddWithValue("@id", idReservacion);
                        cmdDelete.ExecuteNonQuery();
                    }

                    trx.Commit();
                }
                catch (Exception ex)
                {
                    trx.Rollback();
                    throw new Exception("Error al cancelar la habitación: " + ex.Message);
                }
            }
        }
        public void FinalizarPago(int idReservacion, string metodoPago)
        {
            using (SqlConnection cnx = new SqlConnection(ConfigurationManager.ConnectionStrings["cnn"].ConnectionString))
            {
                // Cambiamos el estado a 'Pagada' y asignamos el método elegido
                string sql = "UPDATE Reservacion SET estado = 'Pagada', metodo_pago = @metodo WHERE id_reservacion = @id";
                SqlCommand cmd = new SqlCommand(sql, cnx);
                cmd.Parameters.AddWithValue("@metodo", metodoPago);
                cmd.Parameters.AddWithValue("@id", idReservacion);
                cnx.Open();
                cmd.ExecuteNonQuery();
            }
        }

        //Para traer la info del pdf
        public List<ReservaVista> ConsultarReservasPorId(int idReserva)
        {
            List<ReservaVista> lista = new List<ReservaVista>();

            using (SqlConnection cnx = new SqlConnection(ConfigurationManager.ConnectionStrings["cnn"].ConnectionString))
            {
                string sql = @"
        SELECT  
            r.id_reservacion, 
            r.cedula_cliente,
            d.num_habitacion, 
            d.id_hotel, 
            h.direccion AS nombre_hotel, 
            c.nombre_categoria,            

            STRING_AGG(ca.nombre_caracteristica, ', ') AS amenidades,

            d.fecha_llegada, 
            d.fecha_salida, 
            d.precio_noche_aplicado,
            DATEDIFF(day, d.fecha_llegada, d.fecha_salida) AS dias

        FROM Reservacion r
        INNER JOIN Detalle_Reservacion d 
            ON r.id_reservacion = d.id_reservacion

        INNER JOIN Hotel h 
            ON d.id_hotel = h.id_hotel

        INNER JOIN Habitacion ha 
            ON d.num_habitacion = ha.num_habitacion 
            AND d.id_hotel = ha.id_hotel

        INNER JOIN Categoria c 
            ON ha.id_categoria = c.id_categoria

        LEFT JOIN Detalle_Habitacion dh 
            ON ha.num_habitacion = dh.num_habitacion 
            AND ha.id_hotel = dh.id_hotel

        LEFT JOIN Caracteristica ca 
            ON dh.id_caracteristica = ca.id_caracteristica

        WHERE r.id_reservacion = @id

        GROUP BY 
            r.id_reservacion, 
            r.cedula_cliente,
            d.num_habitacion, 
            d.id_hotel, 
            h.direccion,
            c.nombre_categoria,
            d.fecha_llegada, 
            d.fecha_salida, 
            d.precio_noche_aplicado
        ";

                SqlCommand cmd = new SqlCommand(sql, cnx);
                cmd.Parameters.AddWithValue("@id", idReserva);

                cnx.Open();

                using (SqlDataReader reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        ReservaVista rev = new ReservaVista();

                        rev.IdReservacion = Convert.ToInt32(reader["id_reservacion"]);
                        rev.CedulaCliente = reader["cedula_cliente"].ToString();
                        rev.NumHabitacion = Convert.ToInt32(reader["num_habitacion"]);
                        rev.IdHotel = Convert.ToInt32(reader["id_hotel"]);
                        rev.NombreHotel = reader["nombre_hotel"].ToString();
                        rev.NombreCategoria = reader["nombre_categoria"].ToString();

                        rev.Amenidades = reader["amenidades"] != DBNull.Value
                            ? reader["amenidades"].ToString()
                            : "Servicios básicos incluidos";

                        rev.FechaLlegada = Convert.ToDateTime(reader["fecha_llegada"]);
                        rev.FechaSalida = Convert.ToDateTime(reader["fecha_salida"]);
                        rev.PrecioNoche = Convert.ToDecimal(reader["precio_noche_aplicado"]);

                        int dias = Convert.ToInt32(reader["dias"]);
                        rev.TotalDias = dias <= 0 ? 1 : dias;

                        rev.SubTotal = rev.TotalDias * rev.PrecioNoche;

                        lista.Add(rev);
                    }
                }
            }

            return lista;
        }

        //historial para reservas
        // Método para la pantalla "Reserva Actual" (Pagadas y que aún no terminan)
        public List<ReservaVista> ConsultarReservasActualesPagadas(string cedula)
        {
            List<ReservaVista> lista = new List<ReservaVista>();

            using (SqlConnection cnx = new SqlConnection(ConfigurationManager.ConnectionStrings["cnn"].ConnectionString))
            {
                string sql = @"
                    SELECT  
                        r.id_reservacion, 
                        r.cedula_cliente, 
                        d.num_habitacion, 
                        d.id_hotel, 
                        h.direccion AS nombre_hotel, 
                        c.nombre_categoria,            

                        STRING_AGG(ca.nombre_caracteristica, ', ') AS amenidades,

                        d.fecha_llegada, 
                        d.fecha_salida, 
                        d.precio_noche_aplicado,
                        DATEDIFF(day, d.fecha_llegada, d.fecha_salida) AS dias

                    FROM Reservacion r
                    INNER JOIN Detalle_Reservacion d 
                        ON r.id_reservacion = d.id_reservacion
                    INNER JOIN Hotel h 
                        ON d.id_hotel = h.id_hotel
                    INNER JOIN Habitacion ha 
                        ON d.num_habitacion = ha.num_habitacion 
                        AND d.id_hotel = ha.id_hotel
                    INNER JOIN Categoria c 
                        ON ha.id_categoria = c.id_categoria

                    LEFT JOIN Detalle_Habitacion dh 
                        ON ha.num_habitacion = dh.num_habitacion 
                        AND ha.id_hotel = dh.id_hotel
                    LEFT JOIN Caracteristica ca 
                        ON dh.id_caracteristica = ca.id_caracteristica

                    WHERE r.cedula_cliente = @cedula 
                    AND r.estado = 'Pagada' 
                    AND d.fecha_salida >= CAST(GETDATE() AS DATE)

                    GROUP BY 
                        r.id_reservacion, 
                        r.cedula_cliente, 
                        d.num_habitacion, 
                        d.id_hotel, 
                        h.direccion, 
                        c.nombre_categoria,
                        d.fecha_llegada, 
                        d.fecha_salida, 
                        d.precio_noche_aplicado

                    ORDER BY d.fecha_llegada ASC
                    ";

                SqlCommand cmd = new SqlCommand(sql, cnx);
                cmd.Parameters.AddWithValue("@cedula", cedula);
                cnx.Open();

                using (SqlDataReader reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        ReservaVista rev = new ReservaVista
                        {
                            IdReservacion = Convert.ToInt32(reader["id_reservacion"]),
                            CedulaCliente = reader["cedula_cliente"].ToString(),
                            NumHabitacion = Convert.ToInt32(reader["num_habitacion"]),
                            IdHotel = Convert.ToInt32(reader["id_hotel"]),
                            NombreHotel = reader["nombre_hotel"].ToString(),
                            NombreCategoria = reader["nombre_categoria"].ToString(),

                            Amenidades = reader["amenidades"] != DBNull.Value
                                ? reader["amenidades"].ToString()
                                : "Estándar",

                            FechaLlegada = Convert.ToDateTime(reader["fecha_llegada"]),
                            FechaSalida = Convert.ToDateTime(reader["fecha_salida"]),
                            PrecioNoche = Convert.ToDecimal(reader["precio_noche_aplicado"])
                        };

                        int dias = Convert.ToInt32(reader["dias"]);
                        rev.TotalDias = dias <= 0 ? 1 : dias;
                        rev.SubTotal = rev.TotalDias * rev.PrecioNoche;

                        lista.Add(rev);
                    }
                }
            }

            return lista;
        }
        // Método para el "Historial" (Pagadas y que ya pasaron la fecha de salida)
        public List<ReservaVista> ConsultarHistorialReservas(string cedula)
        {
            List<ReservaVista> lista = new List<ReservaVista>();

            using (SqlConnection cnx = new SqlConnection(ConfigurationManager.ConnectionStrings["cnn"].ConnectionString))
            {
                string sql = @"
                        SELECT  
                            r.id_reservacion, 
                            r.cedula_cliente,
                            d.num_habitacion, 
                            d.id_hotel, 
                            h.direccion AS nombre_hotel, 
                            c.nombre_categoria,            

                            STRING_AGG(ca.nombre_caracteristica, ', ') AS amenidades,

                            d.fecha_llegada, 
                            d.fecha_salida, 
                            d.precio_noche_aplicado,
                            DATEDIFF(day, d.fecha_llegada, d.fecha_salida) AS dias

                        FROM Reservacion r
                        INNER JOIN Detalle_Reservacion d 
                            ON r.id_reservacion = d.id_reservacion
                        INNER JOIN Hotel h 
                            ON d.id_hotel = h.id_hotel
                        INNER JOIN Habitacion ha 
                            ON d.num_habitacion = ha.num_habitacion 
                            AND d.id_hotel = ha.id_hotel
                        INNER JOIN Categoria c 
                            ON ha.id_categoria = c.id_categoria

                        LEFT JOIN Detalle_Habitacion dh 
                            ON ha.num_habitacion = dh.num_habitacion 
                            AND ha.id_hotel = dh.id_hotel
                        LEFT JOIN Caracteristica ca 
                            ON dh.id_caracteristica = ca.id_caracteristica

                        WHERE r.cedula_cliente = @cedula 
                          AND r.estado = 'Pagada' 
                          AND d.fecha_salida < CAST(GETDATE() AS DATE)

                        GROUP BY 
                            r.id_reservacion, 
                            r.cedula_cliente,
                            d.num_habitacion, 
                            d.id_hotel, 
                            h.direccion, 
                            c.nombre_categoria,
                            d.fecha_llegada, 
                            d.fecha_salida, 
                            d.precio_noche_aplicado

                        ORDER BY d.fecha_llegada DESC
                        ";

                SqlCommand cmd = new SqlCommand(sql, cnx);
                cmd.Parameters.AddWithValue("@cedula", cedula);
                cnx.Open();

                using (SqlDataReader reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        ReservaVista rev = new ReservaVista();

                        rev.IdReservacion = Convert.ToInt32(reader["id_reservacion"]);
                        rev.CedulaCliente = reader["cedula_cliente"].ToString();
                        rev.NumHabitacion = Convert.ToInt32(reader["num_habitacion"]);
                        rev.IdHotel = Convert.ToInt32(reader["id_hotel"]);
                        rev.NombreHotel = reader["nombre_hotel"].ToString();
                        rev.NombreCategoria = reader["nombre_categoria"].ToString();

                        rev.Amenidades = reader["amenidades"] != DBNull.Value
                            ? reader["amenidades"].ToString()
                            : "Servicios básicos incluidos";

                        rev.FechaLlegada = Convert.ToDateTime(reader["fecha_llegada"]);
                        rev.FechaSalida = Convert.ToDateTime(reader["fecha_salida"]);
                        rev.PrecioNoche = Convert.ToDecimal(reader["precio_noche_aplicado"]);

                        int dias = Convert.ToInt32(reader["dias"]);
                        rev.TotalDias = dias <= 0 ? 1 : dias;
                        rev.SubTotal = rev.TotalDias * rev.PrecioNoche;

                        lista.Add(rev);
                    }
                }
            }

            return lista;
        }


        public List<ReservaVista> ConsultarReservasPagadas(string cedula)
        {
            List<ReservaVista> lista = new List<ReservaVista>();
            using (SqlConnection cnx = new SqlConnection(ConfigurationManager.ConnectionStrings["cnn"].ConnectionString))
            {
          
                string sql = @"
            SELECT  
                r.id_reservacion, 
                r.cedula_cliente,
                d.num_habitacion, 
                d.id_hotel, 
                h.direccion AS nombre_hotel, 
                c.nombre_categoria,             
                ca.nombre_caracteristica AS amenidades, 
                d.fecha_llegada, 
                d.fecha_salida, 
                d.precio_noche_aplicado,
                DATEDIFF(day, d.fecha_llegada, d.fecha_salida) AS dias
            FROM Reservacion r
            INNER JOIN Detalle_Reservacion d ON r.id_reservacion = d.id_reservacion
            INNER JOIN Hotel h ON d.id_hotel = h.id_hotel
            INNER JOIN Habitacion ha ON d.num_habitacion = ha.num_habitacion AND d.id_hotel = ha.id_hotel
            INNER JOIN Categoria c ON ha.id_categoria = c.id_categoria
            LEFT JOIN Detalle_Habitacion dh ON ha.num_habitacion = dh.num_habitacion AND ha.id_hotel = dh.id_hotel
            LEFT JOIN Caracteristica ca ON dh.id_caracteristica = ca.id_caracteristica
            WHERE r.cedula_cliente = @cedula 
              AND r.estado = 'Pagada' 
            ORDER BY d.fecha_llegada DESC";

                SqlCommand cmd = new SqlCommand(sql, cnx);
                cmd.Parameters.AddWithValue("@cedula", cedula);
                cnx.Open();

                using (SqlDataReader reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        ReservaVista rev = new ReservaVista();
                        rev.IdReservacion = Convert.ToInt32(reader["id_reservacion"]);
                        rev.CedulaCliente = reader["cedula_cliente"].ToString();
                        rev.NumHabitacion = Convert.ToInt32(reader["num_habitacion"]);
                        rev.IdHotel = Convert.ToInt32(reader["id_hotel"]);
                        rev.NombreHotel = reader["nombre_hotel"].ToString();
                        rev.NombreCategoria = reader["nombre_categoria"].ToString();

                        rev.Amenidades = reader["amenidades"] != DBNull.Value
                                         ? reader["amenidades"].ToString()
                                         : "Servicios básicos incluidos";

                        rev.FechaLlegada = Convert.ToDateTime(reader["fecha_llegada"]);
                        rev.FechaSalida = Convert.ToDateTime(reader["fecha_salida"]);
                        rev.PrecioNoche = Convert.ToDecimal(reader["precio_noche_aplicado"]);

                        int dias = Convert.ToInt32(reader["dias"]);
                        rev.TotalDias = dias <= 0 ? 1 : dias;
                        rev.SubTotal = rev.TotalDias * rev.PrecioNoche;

                        lista.Add(rev);
                    }
                }
            }
            return lista;
        }
        public bool ExisteCliente(string cedula)
        {
            using (SqlConnection cnx = new SqlConnection(ConfigurationManager.ConnectionStrings["cnn"].ConnectionString))
            {
                string sql = "SELECT COUNT(*) FROM Cliente WHERE cedula = @cedula";

                SqlCommand cmd = new SqlCommand(sql, cnx);
                cmd.Parameters.AddWithValue("@cedula", cedula);

                cnx.Open();
                int count = (int)cmd.ExecuteScalar();

                return count > 0;
            }
        }

        //check in-chack out
        public List<ReservaVista> ObtenerReservasPorCedulaHoy(string cedula)
        {
            List<ReservaVista> lista = new List<ReservaVista>();

            using (SqlConnection cnx = new SqlConnection(ConfigurationManager.ConnectionStrings["cnn"].ConnectionString))
            {
                string sql = @"
        SELECT  
            r.id_reservacion, 
            r.cedula_cliente,
            d.num_habitacion, 
            d.id_hotel, 
            h.direccion AS nombre_hotel,
            c.nombre_categoria,
            ISNULL(ca.nombre_caracteristica, 'Básico') AS amenidades,
            d.fecha_llegada, 
            d.fecha_salida, 
            d.precio_noche_aplicado,
            DATEDIFF(day, d.fecha_llegada, d.fecha_salida) AS dias,
            ISNULL(d.check_in, 0) AS check_in,
            ISNULL(d.check_out, 0) AS check_out
        FROM Detalle_Reservacion d
        INNER JOIN Reservacion r ON r.id_reservacion = d.id_reservacion
        INNER JOIN Hotel h ON d.id_hotel = h.id_hotel
        INNER JOIN Habitacion ha ON d.num_habitacion = ha.num_habitacion AND d.id_hotel = ha.id_hotel
        INNER JOIN Categoria c ON ha.id_categoria = c.id_categoria
        LEFT JOIN Detalle_Habitacion dh ON ha.num_habitacion = dh.num_habitacion AND ha.id_hotel = dh.id_hotel
        LEFT JOIN Caracteristica ca ON dh.id_caracteristica = ca.id_caracteristica
        WHERE r.cedula_cliente = @cedula
        AND (
            CAST(d.fecha_llegada AS DATE) = CAST(GETDATE() AS DATE)
            OR CAST(d.fecha_salida AS DATE) = CAST(GETDATE() AS DATE)
        )";

                SqlCommand cmd = new SqlCommand(sql, cnx);
                cmd.Parameters.AddWithValue("@cedula", cedula);

                cnx.Open();

                using (SqlDataReader dr = cmd.ExecuteReader())
                {
                    while (dr.Read())
                    {
                        int dias = Convert.ToInt32(dr["dias"]);
                        if (dias <= 0) dias = 1; 

                        decimal precio = Convert.ToDecimal(dr["precio_noche_aplicado"]);

                        lista.Add(new ReservaVista
                        {
                            IdReservacion = Convert.ToInt32(dr["id_reservacion"]),
                            CedulaCliente = dr["cedula_cliente"].ToString(),
                            NumHabitacion = Convert.ToInt32(dr["num_habitacion"]),
                            IdHotel = Convert.ToInt32(dr["id_hotel"]),
                            NombreHotel = dr["nombre_hotel"].ToString(),
                            NombreCategoria = dr["nombre_categoria"].ToString(),
                            Amenidades = dr["amenidades"].ToString(),

                            FechaLlegada = Convert.ToDateTime(dr["fecha_llegada"]),
                            FechaSalida = Convert.ToDateTime(dr["fecha_salida"]),

                            PrecioNoche = precio,
                            TotalDias = dias,
                            SubTotal = precio * dias,

                            CheckIn = Convert.ToInt32(dr["check_in"]) == 1,
                            CheckOut = Convert.ToInt32(dr["check_out"]) == 1
                        });
                    }
                }
            }

            return lista;
        }
        public void MarcarCheckIn(int idReserva, int numHabitacion, int idHotel, bool estado)
        {
            using (SqlConnection cnx = new SqlConnection(ConfigurationManager.ConnectionStrings["cnn"].ConnectionString))
            {
                // Usamos @estado para que guarde 1 (true) o 0 (false)
                string sql = @"
            UPDATE Detalle_Reservacion 
            SET check_in = @estado 
            WHERE id_reservacion = @id 
            AND num_habitacion = @num 
            AND id_hotel = @hotel";

                SqlCommand cmd = new SqlCommand(sql, cnx);
                cmd.Parameters.AddWithValue("@id", idReserva);
                cmd.Parameters.AddWithValue("@num", numHabitacion);
                cmd.Parameters.AddWithValue("@hotel", idHotel);
                cmd.Parameters.AddWithValue("@estado", estado ? 1 : 0);

                cnx.Open();
                cmd.ExecuteNonQuery();
            }
        }

        public void MarcarCheckOut(int idReserva, int numHabitacion, int idHotel, bool estado)
        {
            using (SqlConnection cnx = new SqlConnection(ConfigurationManager.ConnectionStrings["cnn"].ConnectionString))
            {
                string sql = @"
            UPDATE Detalle_Reservacion 
            SET check_out = @estado 
            WHERE id_reservacion = @id 
            AND num_habitacion = @num 
            AND id_hotel = @hotel";

                SqlCommand cmd = new SqlCommand(sql, cnx);
                cmd.Parameters.AddWithValue("@id", idReserva);
                cmd.Parameters.AddWithValue("@num", numHabitacion);
                cmd.Parameters.AddWithValue("@hotel", idHotel);
                cmd.Parameters.AddWithValue("@estado", estado ? 1 : 0);

                cnx.Open();
                cmd.ExecuteNonQuery();
            }
        }
    }
}