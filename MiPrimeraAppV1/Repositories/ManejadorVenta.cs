using MiPrimeraAppV1.Models;
using System.Data.SqlClient;
using System.Runtime.CompilerServices;

namespace MiPrimeraAppV1.Repositories
{
    internal static class ManejadorVenta
    {

        public static string connetionString = "Data Source=I5-6400-8GB-WIN;Initial Catalog=SistemaGestion;Integrated Security=True;Connect Timeout=30;Encrypt=False;TrustServerCertificate=False;ApplicationIntent=ReadWrite;MultiSubnetFailover=False";
        public static List<Venta> ObtenerVentas(long idUsuario)
        {
            List<Venta> ventas = new List<Venta>();

            using (SqlConnection conn = new SqlConnection(connetionString))
            {
                SqlCommand comando = new SqlCommand("SELECT * FROM Venta WHERE @IdUsuario = idUsuario", conn);
                comando.Parameters.AddWithValue("@IdUsuario", idUsuario);
                conn.Open();

                SqlDataReader reader = comando.ExecuteReader();
                if (reader.HasRows)
                {
                    while (reader.Read())
                    {
                        Venta ventaTemp = new Venta();
                        ventaTemp.Id = reader.GetInt64(0);
                        ventaTemp.Comentarios = reader.GetString(1);
                        ventaTemp.IdUsuario = reader.GetInt64(2);
                        ventas.Add(ventaTemp);
                    }
                }

                return ventas;

            }

        }

        public static void CargarVenta(long idUsuario , List<Producto> productosVendidos)
        {
            Venta venta = new Venta();
            using (SqlConnection conn = new SqlConnection(connetionString))
            {

                SqlCommand comando = new SqlCommand();
                comando.Connection = conn;
                comando.Connection.Open();

                venta.Comentarios = "";
                venta.IdUsuario = idUsuario;
                venta.Id = InsertarVenta(venta);

                foreach (Producto producto in productosVendidos)
                {
                    ProductoVendido productoVendido = new ProductoVendido();
                    productoVendido.Stock = producto.Stock;
                    productoVendido.IdProducto = producto.Id;
                    productoVendido.IdVenta = venta.Id;

                    ManejadorProductoVendido.InsertarProductoVendido( productoVendido );

                  

                    ManejadorProducto.UpdateStockProducto(productoVendido.IdProducto, productoVendido.Stock);


                }
              

            }

        }

        public static long InsertarVenta(Venta venta)
        {
            using (SqlConnection conn = new SqlConnection(connetionString))
            {
                
                SqlCommand comando = new SqlCommand();

                comando.Connection = conn;
                comando.Connection.Open();

                comando.CommandText = "INSERT INTO Venta ([Comentarios], [IdUsuario]) VALUES( @comentarios, @idUsuario)";
                comando.Parameters.AddWithValue("@comentarios", venta.Comentarios);
                comando.Parameters.AddWithValue("@idUsuario", venta.IdUsuario);               
                comando.ExecuteNonQuery();

                comando.CommandText = "SELECT @@Identity";
                long LastID = Convert.ToInt64(comando.ExecuteScalar());
                comando.Connection.Close();
                

                return LastID;

            }

          
        }



    }
}
