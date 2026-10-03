using EDL;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DAL
{
    public class HabitacionDisponibleDAL
    {
        public List<HabitacionDisponible> BuscarHabitaciones(DateTime llegada, DateTime salida, int? categoria, int? idHotel)
        {
            List<HabitacionDisponible> lista = new List<HabitacionDisponible>();

            DateTime fechaBusquedaLlegada = llegada.Date;
            DateTime fechaBusquedaSalida = salida.Date;

            // Validación: No permitir búsquedas en fechas pasadas
            if (fechaBusquedaLlegada < DateTime.Today)
            {
                return lista;
            }

            using (SqlConnection cnx = new SqlConnection(ConfigurationManager.ConnectionStrings["cnn"].ConnectionString))
            {
                string sql = @"
            SELECT h.num_habitacion, h.id_hotel, hot.direccion, c.id_categoria, c.nombre_categoria, c.precio_actual,
            ISNULL((SELECT STRING_AGG(car.nombre_caracteristica, ', ') 
              FROM Detalle_Habitacion dh 
              INNER JOIN Caracteristica car ON dh.id_caracteristica = car.id_caracteristica
              WHERE dh.num_habitacion = h.num_habitacion AND dh.id_hotel = h.id_hotel), 'Estándar') as Caracteristicas
            FROM Habitacion h
            INNER JOIN Hotel hot ON h.id_hotel = hot.id_hotel
            INNER JOIN Categoria c ON h.id_categoria = c.id_categoria
            WHERE NOT EXISTS (
                SELECT 1 FROM Detalle_Reservacion dr
                WHERE dr.num_habitacion = h.num_habitacion 
                AND dr.id_hotel = h.id_hotel
                -- LÓGICA DE COLISIÓN (Excluye solo si hay choque real):
                -- Si alguien sale el 18 y yo llego el 18, (18 < 18) es FALSO.
                -- Por lo tanto, NO hay colisión y la habitación SÍ aparece.
                AND (@llegada < CAST(dr.fecha_salida AS DATE) 
                     AND @salida > CAST(dr.fecha_llegada AS DATE))
            )";

                if (categoria.HasValue && categoria > 0) sql += " AND c.id_categoria = @categoria";
                if (idHotel.HasValue && idHotel > 0) sql += " AND h.id_hotel = @idHotel";

                SqlCommand cmd = new SqlCommand(sql, cnx);

                cmd.Parameters.Add("@llegada", System.Data.SqlDbType.Date).Value = fechaBusquedaLlegada;
                cmd.Parameters.Add("@salida", System.Data.SqlDbType.Date).Value = fechaBusquedaSalida;

                if (categoria.HasValue && categoria > 0) cmd.Parameters.AddWithValue("@categoria", categoria);
                if (idHotel.HasValue && idHotel > 0) cmd.Parameters.AddWithValue("@idHotel", idHotel);

                cnx.Open();
                using (SqlDataReader dr = cmd.ExecuteReader())
                {
                    while (dr.Read())
                    {
                        lista.Add(new HabitacionDisponible
                        {
                            NumHabitacion = Convert.ToInt32(dr["num_habitacion"]),
                            IdHotel = Convert.ToInt32(dr["id_hotel"]),
                            DireccionHotel = dr["direccion"].ToString(),
                            IdCategoria = Convert.ToInt32(dr["id_categoria"]),
                            NombreCategoria = dr["nombre_categoria"].ToString(),
                            Precio = Convert.ToDecimal(dr["precio_actual"]),
                            Caracteristicas = dr["Caracteristicas"].ToString()
                        });
                    }
                }
            }
            return lista;
        }
        public List<Hotel> ListarHoteles()
        {
            List<Hotel> lista = new List<Hotel>();
            using (SqlConnection cnx = new SqlConnection(ConfigurationManager.ConnectionStrings["cnn"].ConnectionString))
            {
                string sql = "SELECT id_hotel, direccion FROM Hotel";
                SqlCommand cmd = new SqlCommand(sql, cnx);
                cnx.Open();
                SqlDataReader dr = cmd.ExecuteReader();
                while (dr.Read())
                {
                    lista.Add(new Hotel { IdHotel = (int)dr["id_hotel"], Direccion = dr["direccion"].ToString() });
                }
            }
            return lista;
        }

        public List<Categoria> ListarCategorias()
        {
            List<Categoria> lista = new List<Categoria>();
            using (SqlConnection cnx = new SqlConnection(ConfigurationManager.ConnectionStrings["cnn"].ConnectionString))
            {
                string sql = "SELECT id_categoria, nombre_categoria FROM Categoria";
                SqlCommand cmd = new SqlCommand(sql, cnx);
                cnx.Open();
                SqlDataReader dr = cmd.ExecuteReader();
                while (dr.Read())
                {
                    lista.Add(new Categoria { IdCategoria = (int)dr["id_categoria"], NombreCategoria = dr["nombre_categoria"].ToString() });
                }
            }
            return lista;
        }
    }
}
