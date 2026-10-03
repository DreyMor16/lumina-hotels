using EDL;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Configuration;

namespace DAL
{
    public class MobiliarioDAL
    {
        private string cnnString = ConfigurationManager.ConnectionStrings["cnn"].ConnectionString;

        public void Insert(Mobiliario mueble)
        {
            try
            {
                using (SqlConnection cnx = new SqlConnection(cnnString))
                {
                    cnx.Open();
                    const string sql = "INSERT INTO Mobiliario (cod_mueble, descripcion, precio_mueble) VALUES (@cod, @desc, @precio)";
                    using (SqlCommand cmd = new SqlCommand(sql, cnx))
                    {
                        cmd.Parameters.AddWithValue("@cod", mueble.CodMueble);
                        cmd.Parameters.AddWithValue("@desc", mueble.Descripcion);
                        cmd.Parameters.AddWithValue("@precio", mueble.PrecioMueble);
                        if (cmd.ExecuteNonQuery() == 0)
                            throw new Exception("El mueble indicado ya no existe.");
                    }
                }
            }
            catch (SqlException) { throw new Exception("DB_ERROR"); }
        }

        public List<Mobiliario> GetAll()
        {
            List<Mobiliario> lista = new List<Mobiliario>();
            try
            {
                using (SqlConnection cnx = new SqlConnection(cnnString))
                {
                    cnx.Open();
                    const string sql = "SELECT * FROM Mobiliario";
                    using (SqlCommand cmd = new SqlCommand(sql, cnx))
                    {
                        SqlDataReader dr = cmd.ExecuteReader();
                        while (dr.Read())
                        {
                            lista.Add(new Mobiliario
                            {
                                CodMueble = (int)dr["cod_mueble"],
                                Descripcion = dr["descripcion"].ToString(),
                                PrecioMueble = (decimal)dr["precio_mueble"]
                            });
                        }
                    }
                }
            }
            catch (SqlException) { throw new Exception("DB_ERROR_READ"); }
            return lista;
        }

        public Mobiliario GetById(int codMueble)
        {
            Mobiliario mueble = null;
            try
            {
                using (SqlConnection cnx = new SqlConnection(cnnString))
                {
                    cnx.Open();
                    const string sql = "SELECT * FROM Mobiliario WHERE cod_mueble = @cod";
                    using (SqlCommand cmd = new SqlCommand(sql, cnx))
                    {
                        cmd.Parameters.AddWithValue("@cod", codMueble);
                        SqlDataReader dr = cmd.ExecuteReader();
                        if (dr.Read())
                        {
                            mueble = new Mobiliario
                            {
                                CodMueble = (int)dr["cod_mueble"],
                                Descripcion = dr["descripcion"].ToString(),
                                PrecioMueble = (decimal)dr["precio_mueble"]
                            };
                        }
                    }
                }
            }
            catch (SqlException) { throw new Exception("DB_ERROR_READ_ID"); }
            return mueble;
        }

        public void Update(Mobiliario mueble)
        {
            try
            {
                using (SqlConnection cnx = new SqlConnection(cnnString))
                {
                    cnx.Open();
                    const string sql = @"UPDATE Mobiliario 
                               SET descripcion = @desc, precio_mueble = @precio 
                               WHERE cod_mueble = @cod";
                    using (SqlCommand cmd = new SqlCommand(sql, cnx))
                    {
                        cmd.Parameters.AddWithValue("@cod", mueble.CodMueble);
                        cmd.Parameters.AddWithValue("@desc", mueble.Descripcion);
                        cmd.Parameters.AddWithValue("@precio", mueble.PrecioMueble);
                        if (cmd.ExecuteNonQuery() == 0)
                            throw new Exception("El mueble indicado ya no existe.");
                    }
                }
            }
            catch (SqlException) { throw new Exception("DB_ERROR_UPDATE"); }
        }

        public void Delete(int codMueble)
        {
            try
            {
                using (SqlConnection cnx = new SqlConnection(cnnString))
                {
                    cnx.Open();
                    const string sql = "DELETE FROM Mobiliario WHERE cod_mueble = @codigo";
                    using (SqlCommand cmd = new SqlCommand(sql, cnx))
                    {
                        cmd.Parameters.AddWithValue("@codigo", codMueble);
                        if (cmd.ExecuteNonQuery() == 0)
                            throw new Exception("El mueble indicado ya no existe.");
                    }
                }
            }
            catch (SqlException ex)
            {
                if (ex.Number == 547)
                    throw new Exception("No se puede eliminar el mueble porque está asignado a una habitación.");
                throw new Exception("No fue posible eliminar el mueble.");
            }
        }
    }
}
