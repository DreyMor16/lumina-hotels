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
    public class TelefonoClienteDAL
    {
        public void Insert(TelefonoCliente telefono)
        {
            using (SqlConnection cnx =
                new SqlConnection(ConfigurationManager.ConnectionStrings["cnn"].ConnectionString))
            {
                cnx.Open();

                const string sql =
                    "INSERT INTO Telefono_Cliente (cedula_cliente, telefono) VALUES (@cedula, @telefono)";

                using (SqlCommand cmd = new SqlCommand(sql, cnx))
                {
                    cmd.Parameters.AddWithValue("@cedula", telefono.CedulaCliente);
                    cmd.Parameters.AddWithValue("@telefono", telefono.Telefono);

                    cmd.ExecuteNonQuery();
                }
            }
        }

        public List<TelefonoCliente> GetByCliente(string cedula)
        {
            List<TelefonoCliente> lista = new List<TelefonoCliente>();

            using (SqlConnection cnx =
                new SqlConnection(ConfigurationManager.ConnectionStrings["cnn"].ConnectionString))
            {
                cnx.Open();

                const string sql =
                    "SELECT * FROM Telefono_Cliente WHERE cedula_cliente = @cedula";

                using (SqlCommand cmd = new SqlCommand(sql, cnx))
                {
                    cmd.Parameters.AddWithValue("@cedula", cedula);

                    SqlDataReader dr = cmd.ExecuteReader();

                    while (dr.Read())
                    {
                        lista.Add(new TelefonoCliente()
                        {
                            IdTelefono = Convert.ToInt32(dr["id_telefono"]),
                            CedulaCliente = dr["cedula_cliente"].ToString(),
                            Telefono = dr["telefono"].ToString()
                        });
                    }
                }
            }

            return lista;
        }

        public void DeleteByCliente(string cedula)
        {
            using (SqlConnection cnx =
                new SqlConnection(ConfigurationManager.ConnectionStrings["cnn"].ConnectionString))
            {
                cnx.Open();

                const string sql =
                    "DELETE FROM Telefono_Cliente WHERE cedula_cliente = @cedula";

                using (SqlCommand cmd = new SqlCommand(sql, cnx))
                {
                    cmd.Parameters.AddWithValue("@cedula", cedula);
                    cmd.ExecuteNonQuery();
                }
            }
        }
        public bool ExisteTelefono(string telefono)
        {
            using (SqlConnection cnx =
                new SqlConnection(ConfigurationManager.ConnectionStrings["cnn"].ConnectionString))
            {
                cnx.Open();

                const string sql =
                    "SELECT COUNT(*) FROM Telefono_Cliente WHERE telefono = @telefono";

                using (SqlCommand cmd = new SqlCommand(sql, cnx))
                {
                    cmd.Parameters.AddWithValue("@telefono", telefono);

                    int cantidad = (int)cmd.ExecuteScalar();

                    return cantidad > 0;
                }
            }
        }
        //Borrar telefono en especifico
        public void DeleteEspecifico(string cedula, string telefono)
        {
            using (SqlConnection cnx = new SqlConnection(ConfigurationManager.ConnectionStrings["cnn"].ConnectionString))
            {
                cnx.Open();
                const string sql = "DELETE FROM Telefono_Cliente WHERE cedula_cliente = @cedula AND telefono = @tel";
                using (SqlCommand cmd = new SqlCommand(sql, cnx))
                {
                    cmd.Parameters.AddWithValue("@cedula", cedula);
                    cmd.Parameters.AddWithValue("@tel", telefono);
                    cmd.ExecuteNonQuery();
                }
            }
        }
    }
}
