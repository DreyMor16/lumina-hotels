using EDL;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Configuration;

namespace DAL
{
    public class CategoriaDAL
    {
        private string cnnString = ConfigurationManager.ConnectionStrings["cnn"].ConnectionString;

        public List<Categoria> GetAll()
        {
            List<Categoria> lista = new List<Categoria>();
            using (SqlConnection cnx = new SqlConnection(cnnString))
            {
                cnx.Open();
                const string sql = "SELECT * FROM Categoria";
                using (SqlCommand cmd = new SqlCommand(sql, cnx))
                {
                    SqlDataReader dr = cmd.ExecuteReader();
                    while (dr.Read())
                    {
                        lista.Add(new Categoria
                        {
                            IdCategoria = (int)dr["id_categoria"],
                            NombreCategoria = dr["nombre_categoria"].ToString(),
                            PrecioActual = (decimal)dr["precio_actual"]
                        });
                    }
                }
            }
            return lista;
        }

        public void Insert(Categoria cat)
        {
            using (SqlConnection cnx = new SqlConnection(cnnString))
            {
                cnx.Open();
                const string sql = "INSERT INTO Categoria (id_categoria, nombre_categoria, precio_actual) VALUES (@id, @nom, @pre)";
                using (SqlCommand cmd = new SqlCommand(sql, cnx))
                {
                    cmd.Parameters.AddWithValue("@id", cat.IdCategoria);
                    cmd.Parameters.AddWithValue("@nom", cat.NombreCategoria);
                    cmd.Parameters.AddWithValue("@pre", cat.PrecioActual);
                    cmd.ExecuteNonQuery();
                }
            }
        }

        public void Update(Categoria categoria)
        {
            try
            {
                using (SqlConnection cnx = new SqlConnection(cnnString))
                {
                    cnx.Open();
                    const string sql = "UPDATE Categoria SET nombre_categoria = @nom, precio_actual = @pre WHERE id_categoria = @id";
                    using (SqlCommand cmd = new SqlCommand(sql, cnx))
                    {
                        cmd.Parameters.AddWithValue("@id", categoria.IdCategoria);
                        cmd.Parameters.AddWithValue("@nom", categoria.NombreCategoria);
                        cmd.Parameters.AddWithValue("@pre", categoria.PrecioActual);
                        if (cmd.ExecuteNonQuery() == 0)
                            throw new Exception("La categoría indicada ya no existe.");
                    }
                }
            }
            catch (SqlException) { throw new Exception("DB_ERROR_UPDATE"); }
        }

        public void Delete(int idCategoria)
        {
            try
            {
                using (SqlConnection cnx = new SqlConnection(cnnString))
                {
                    cnx.Open();
                    const string sql = "DELETE FROM Categoria WHERE id_categoria = @id";
                    using (SqlCommand cmd = new SqlCommand(sql, cnx))
                    {
                        cmd.Parameters.AddWithValue("@id", idCategoria);
                        if (cmd.ExecuteNonQuery() == 0)
                            throw new Exception("La categoría indicada ya no existe.");
                    }
                }
            }
            catch (SqlException ex)
            {
                if (ex.Number == 547)
                    throw new Exception("No se puede eliminar la categoría porque está asignada a una o más habitaciones.");
                throw new Exception("No fue posible eliminar la categoría.");
            }
        }
    }
}
