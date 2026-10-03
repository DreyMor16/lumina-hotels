using EDL;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Configuration;

namespace DAL
{
    public class ClienteDAL
    {
        public void Insert(Cliente cliente)
        {
            try
            {
                using (SqlConnection cnx =
                    new SqlConnection(ConfigurationManager.ConnectionStrings["cnn"].ConnectionString))
                {
                    cnx.Open();

                    const string sql =
                        "INSERT INTO Cliente (cedula, nombre) VALUES (@cedula, @nombre)";

                    using (SqlCommand cmd = new SqlCommand(sql, cnx))
                    {
                        cmd.Parameters.AddWithValue("@cedula", cliente.Cedula);
                        cmd.Parameters.AddWithValue("@nombre", cliente.Nombre);

                        cmd.ExecuteNonQuery();
                    }
                }
            }
            catch (SqlException)
            {
                throw new Exception("DB_ERROR");
            }
        }

        public void Update(Cliente cliente)
        {
            try
            {
                using (SqlConnection cnx =
                    new SqlConnection(ConfigurationManager.ConnectionStrings["cnn"].ConnectionString))
                {
                    cnx.Open();

                    const string sql =
                        "UPDATE Cliente SET nombre = @nombre WHERE cedula = @cedula";

                    using (SqlCommand cmd = new SqlCommand(sql, cnx))
                    {
                        cmd.Parameters.AddWithValue("@cedula", cliente.Cedula);
                        cmd.Parameters.AddWithValue("@nombre", cliente.Nombre);

                        cmd.ExecuteNonQuery();
                    }
                }
            }
            catch (SqlException)
            {
                throw new Exception("DB_ERROR");
            }
        }

        public Cliente GetById(string cedula)
        {
            try
            {
                using (SqlConnection cnx =
                    new SqlConnection(ConfigurationManager.ConnectionStrings["cnn"].ConnectionString))
                {
                    cnx.Open();

                    const string sql =
                        "SELECT * FROM Cliente WHERE cedula = @cedula";

                    using (SqlCommand cmd = new SqlCommand(sql, cnx))
                    {
                        cmd.Parameters.AddWithValue("@cedula", cedula);

                        SqlDataReader dr = cmd.ExecuteReader();

                        if (dr.Read())
                        {
                            return new Cliente()
                            {
                                Cedula = dr["cedula"].ToString(),
                                Nombre = dr["nombre"].ToString()
                            };
                        }
                    }
                }

                return null;
            }
            catch (SqlException)
            {
                throw new Exception("DB_ERROR");
            }
        }

        public bool Existe(string cedula)
        {
            using (SqlConnection cnx =
                new SqlConnection(ConfigurationManager.ConnectionStrings["cnn"].ConnectionString))
            {
                cnx.Open();

                const string sql =
                    "SELECT COUNT(*) FROM Cliente WHERE cedula = @cedula";

                using (SqlCommand cmd = new SqlCommand(sql, cnx))
                {
                    cmd.Parameters.AddWithValue("@cedula", cedula);

                    int cantidad = (int)cmd.ExecuteScalar();
                    return cantidad > 0;
                }
            }
        }
    }
}
