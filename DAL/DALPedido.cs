using BE;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Linq;

namespace DAL
{
    public class DALPedido
    {
        private readonly Acceso _acceso = Acceso.GetInstance();

        public void AsegurarEsquema()
        {
            _acceso.Escribir(@"
                IF OBJECT_ID(N'[dbo].[Pedido]', N'U') IS NULL
                CREATE TABLE [dbo].[Pedido](
                    [IdPedido] [int] IDENTITY(1,1) NOT NULL,
                    [IdUsuario] [int] NOT NULL,
                    [Fecha] [datetime] NOT NULL,
                    [NombreCliente] [nvarchar](100) NOT NULL,
                    [DocumentoCliente] [nvarchar](20) NOT NULL,
                    [DireccionEnvio] [nvarchar](200) NOT NULL,
                    [DistanciaKm] [decimal](9, 2) NOT NULL,
                    [Subtotal] [decimal](18, 2) NOT NULL,
                    [Envio] [decimal](18, 2) NOT NULL,
                    [Total] [decimal](18, 2) NOT NULL,
                    CONSTRAINT [PK_Pedido] PRIMARY KEY CLUSTERED ([IdPedido] ASC),
                    CONSTRAINT [FK_Pedido_Usuario] FOREIGN KEY ([IdUsuario]) REFERENCES [dbo].[Usuario] ([IdUsuario])
                )", null);

            _acceso.Escribir(@"
                IF OBJECT_ID(N'[dbo].[LineaPedido]', N'U') IS NULL
                CREATE TABLE [dbo].[LineaPedido](
                    [IdLineaPedido] [int] IDENTITY(1,1) NOT NULL,
                    [IdPedido] [int] NOT NULL,
                    [IdProducto] [int] NOT NULL,
                    [Nombre] [nvarchar](100) NOT NULL,
                    [PrecioUnitario] [decimal](18, 2) NOT NULL,
                    [Cantidad] [int] NOT NULL,
                    CONSTRAINT [PK_LineaPedido] PRIMARY KEY CLUSTERED ([IdLineaPedido] ASC),
                    CONSTRAINT [FK_LineaPedido_Pedido] FOREIGN KEY ([IdPedido]) REFERENCES [dbo].[Pedido] ([IdPedido]),
                    CONSTRAINT [FK_LineaPedido_Producto] FOREIGN KEY ([IdProducto]) REFERENCES [dbo].[Producto] ([IdProducto])
                )", null);
        }

        public void RegistrarPedido(BEPedido pedido, List<BEProducto> productosActualizados, string dvvProducto)
        {
            using (var conexion = new SqlConnection(ConfigurationManager.ConnectionStrings["DefaultConnection"].ConnectionString))
            {
                conexion.Open();
                // Si una escritura falla, Dispose revierte el pedido, el stock y el carrito juntos.
                using (var transaccion = conexion.BeginTransaction())
                {
                    using (var cmd = new SqlCommand(@"
                        INSERT INTO Pedido (IdUsuario, Fecha, NombreCliente, DocumentoCliente, DireccionEnvio, DistanciaKm, Subtotal, Envio, Total)
                        VALUES (@IdUsuario, @Fecha, @NombreCliente, @DocumentoCliente, @DireccionEnvio, @DistanciaKm, @Subtotal, @Envio, @Total);
                        SELECT CAST(SCOPE_IDENTITY() AS INT);", conexion, transaccion))
                    {
                        cmd.Parameters.AddRange(new SqlParameter[]
                        {
                            new SqlParameter("@IdUsuario", pedido.IdUsuario),
                            new SqlParameter("@Fecha", pedido.Fecha),
                            new SqlParameter("@NombreCliente", pedido.NombreCliente),
                            new SqlParameter("@DocumentoCliente", pedido.DocumentoCliente),
                            new SqlParameter("@DireccionEnvio", pedido.DireccionEnvio),
                            new SqlParameter("@DistanciaKm", pedido.DistanciaKm),
                            new SqlParameter("@Subtotal", pedido.Subtotal),
                            new SqlParameter("@Envio", pedido.Envio),
                            new SqlParameter("@Total", pedido.Total)
                        });
                        pedido.IdPedido = Convert.ToInt32(cmd.ExecuteScalar());
                    }

                    foreach (BELineaPedido linea in pedido.Lineas)
                    {
                        using (var cmd = new SqlCommand(
                            "INSERT INTO LineaPedido (IdPedido, IdProducto, Nombre, PrecioUnitario, Cantidad) VALUES (@IdPedido, @IdProducto, @Nombre, @PrecioUnitario, @Cantidad)",
                            conexion, transaccion))
                        {
                            cmd.Parameters.AddRange(new SqlParameter[]
                            {
                                new SqlParameter("@IdPedido", pedido.IdPedido),
                                new SqlParameter("@IdProducto", linea.IdProducto),
                                new SqlParameter("@Nombre", linea.Nombre),
                                new SqlParameter("@PrecioUnitario", linea.PrecioUnitario),
                                new SqlParameter("@Cantidad", linea.Cantidad)
                            });
                            cmd.ExecuteNonQuery();
                        }

                        BEProducto producto = productosActualizados.First(p => p.IdProducto == linea.IdProducto);
                        using (var cmd = new SqlCommand(
                            "UPDATE Producto SET Stock = @Stock, DVH = @DVH WHERE IdProducto = @IdProducto AND Stock = @StockAnterior",
                            conexion, transaccion))
                        {
                            cmd.Parameters.AddRange(new SqlParameter[]
                            {
                                new SqlParameter("@Stock", producto.Stock),
                                new SqlParameter("@DVH", producto.DVH),
                                new SqlParameter("@IdProducto", producto.IdProducto),
                                new SqlParameter("@StockAnterior", producto.Stock + linea.Cantidad)
                            });
                            if (cmd.ExecuteNonQuery() != 1)
                                throw new InvalidOperationException("El stock de " + linea.Nombre + " cambió mientras se procesaba la compra.");
                        }
                    }

                    using (var cmd = new SqlCommand(@"
                        MERGE INTO DVV AS target
                        USING (SELECT @Tabla AS Tabla) AS source
                        ON (target.Tabla = source.Tabla)
                        WHEN MATCHED THEN
                            UPDATE SET DVV = @DVV
                        WHEN NOT MATCHED THEN
                            INSERT (Tabla, DVV) VALUES (source.Tabla, @DVV);", conexion, transaccion))
                    {
                        cmd.Parameters.AddRange(new SqlParameter[]
                        {
                            new SqlParameter("@Tabla", "Producto"),
                            new SqlParameter("@DVV", dvvProducto)
                        });
                        cmd.ExecuteNonQuery();
                    }

                    using (var cmd = new SqlCommand("DELETE FROM ItemCarrito WHERE IdUsuario = @IdUsuario", conexion, transaccion))
                    {
                        cmd.Parameters.Add(new SqlParameter("@IdUsuario", pedido.IdUsuario));
                        cmd.ExecuteNonQuery();
                    }

                    transaccion.Commit();
                }
            }
        }

        public List<BEPedido> ObtenerPorUsuario(int idUsuario)
        {
            DataTable dtPedidos = _acceso.Leer(@"
                SELECT IdPedido, IdUsuario, Fecha, NombreCliente, DocumentoCliente, DireccionEnvio, DistanciaKm, Subtotal, Envio, Total
                FROM Pedido
                WHERE IdUsuario = @IdUsuario
                ORDER BY Fecha DESC, IdPedido DESC",
                new SqlParameter[] { new SqlParameter("@IdUsuario", idUsuario) });

            DataTable dtLineas = _acceso.Leer(@"
                SELECT lp.IdPedido, lp.IdProducto, lp.Nombre, lp.PrecioUnitario, lp.Cantidad
                FROM LineaPedido lp
                INNER JOIN Pedido p ON lp.IdPedido = p.IdPedido
                WHERE p.IdUsuario = @IdUsuario
                ORDER BY lp.IdLineaPedido",
                new SqlParameter[] { new SqlParameter("@IdUsuario", idUsuario) });

            List<BEPedido> pedidos = new List<BEPedido>();
            foreach (DataRow row in dtPedidos.Rows)
            {
                BEPedido pedido = MapearPedido(row);
                foreach (DataRow fila in dtLineas.Select("IdPedido = " + pedido.IdPedido))
                    pedido.Lineas.Add(MapearLinea(fila));
                pedidos.Add(pedido);
            }
            return pedidos;
        }

        private static BEPedido MapearPedido(DataRow row)
        {
            return new BEPedido
            {
                IdPedido = Convert.ToInt32(row["IdPedido"]),
                IdUsuario = Convert.ToInt32(row["IdUsuario"]),
                Fecha = Convert.ToDateTime(row["Fecha"]),
                NombreCliente = row["NombreCliente"].ToString(),
                DocumentoCliente = row["DocumentoCliente"].ToString(),
                DireccionEnvio = row["DireccionEnvio"].ToString(),
                DistanciaKm = Convert.ToDecimal(row["DistanciaKm"]),
                Subtotal = Convert.ToDecimal(row["Subtotal"]),
                Envio = Convert.ToDecimal(row["Envio"]),
                Total = Convert.ToDecimal(row["Total"])
            };
        }

        private static BELineaPedido MapearLinea(DataRow row)
        {
            return new BELineaPedido
            {
                IdProducto = Convert.ToInt32(row["IdProducto"]),
                Nombre = row["Nombre"].ToString(),
                PrecioUnitario = Convert.ToDecimal(row["PrecioUnitario"]),
                Cantidad = Convert.ToInt32(row["Cantidad"])
            };
        }
    }
}
