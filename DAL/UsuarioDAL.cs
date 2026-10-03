using EDL;
using System;
using System.Configuration;
using System.Data.SqlClient;

namespace DAL
{
    public class UsuarioDAL
    {
        public void Insert(Usuario usuario)
        {
            try
            {
                using (SqlConnection cnx =
                    new SqlConnection(ConfigurationManager.ConnectionStrings["cnn"].ConnectionString))
                {
                    cnx.Open();

                    const string sql =
                        @"INSERT INTO Usuario
                        (username, password, id_rol, cedula_cliente)
                        VALUES
                        (@username, @password, @rol, @cedula)";

                    using (SqlCommand cmd = new SqlCommand(sql, cnx))
                    {
                        cmd.Parameters.AddWithValue("@username", usuario.Username);
                        cmd.Parameters.AddWithValue("@password", usuario.Password);
                        cmd.Parameters.AddWithValue("@rol", usuario.IdRol);
                        cmd.Parameters.AddWithValue("@cedula", usuario.CedulaCliente);

                        cmd.ExecuteNonQuery();
                    }
                }
            }
            catch (SqlException)
            {
                throw new Exception("DB_ERROR");
            }
        }

        public bool ExisteUsuario(string username)
        {
            using (SqlConnection cnx =
                new SqlConnection(ConfigurationManager.ConnectionStrings["cnn"].ConnectionString))
            {
                cnx.Open();

                const string sql =
                    "SELECT COUNT(*) FROM Usuario WHERE username = @username";

                using (SqlCommand cmd = new SqlCommand(sql, cnx))
                {
                    cmd.Parameters.AddWithValue("@username", username);

                    int cantidad = (int)cmd.ExecuteScalar();

                    return cantidad > 0;
                }
            }
        }
        public Usuario Login(string username, string password)
        {
            using (SqlConnection cnx =
                new SqlConnection(ConfigurationManager.ConnectionStrings["cnn"].ConnectionString))
            {
                cnx.Open();

                const string sql =
                "SELECT * FROM Usuario WHERE username = @username AND password = @password";

                using (SqlCommand cmd = new SqlCommand(sql, cnx))
                {
                    cmd.Parameters.AddWithValue("@username", username);
                    cmd.Parameters.AddWithValue("@password", password);

                    SqlDataReader dr = cmd.ExecuteReader();

                    if (dr.Read())
                    {
                        return new Usuario()
                        {
                            IdUsuario = Convert.ToInt32(dr["id_usuario"]),
                            Username = dr["username"].ToString(),
                            Password = dr["password"].ToString(),
                            IdRol = Convert.ToInt32(dr["id_rol"]),
                            CedulaCliente = dr["cedula_cliente"].ToString()
                        };
                    }
                }
            }

            return null;
        }

        //Actualizar mi perfil
        public Usuario GetByCedula(string cedula)
        {
            using (SqlConnection cnx = new SqlConnection(ConfigurationManager.ConnectionStrings["cnn"].ConnectionString))
            {
                cnx.Open();
                const string sql = "SELECT * FROM Usuario WHERE cedula_cliente = @cedula";
                using (SqlCommand cmd = new SqlCommand(sql, cnx))
                {
                    cmd.Parameters.AddWithValue("@cedula", cedula);
                    SqlDataReader dr = cmd.ExecuteReader();
                    if (dr.Read())
                    {
                        return new Usuario()
                        {
                            IdUsuario = Convert.ToInt32(dr["id_usuario"]),
                            Username = dr["username"].ToString(),
                            Password = dr["password"].ToString(),
                            IdRol = Convert.ToInt32(dr["id_rol"]),
                            CedulaCliente = dr["cedula_cliente"].ToString()
                        };
                    }
                }
            }
            return null;
        }

        public void Update(Usuario usuario)
        {
            using (SqlConnection cnx = new SqlConnection(ConfigurationManager.ConnectionStrings["cnn"].ConnectionString))
            {
                cnx.Open();
                const string sql = "UPDATE Usuario SET username = @username, password = @password WHERE cedula_cliente = @cedula";
                using (SqlCommand cmd = new SqlCommand(sql, cnx))
                {
                    cmd.Parameters.AddWithValue("@username", usuario.Username);
                    cmd.Parameters.AddWithValue("@password", usuario.Password);
                    cmd.Parameters.AddWithValue("@cedula", usuario.CedulaCliente);
                    cmd.ExecuteNonQuery();
                }
            }
        }
    }
}