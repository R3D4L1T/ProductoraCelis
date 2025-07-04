using Microsoft.EntityFrameworkCore;
using ProductoraCelis.Models;

namespace ProductoraCelis.Data
{
    public class pgDbContext : DbContext
    {
        public pgDbContext(DbContextOptions<pgDbContext> options) : base(options) { }

        public DbSet<Usuario> Usuario { get; set; }
        public DbSet<Clientes> Clientes { get; set; }
        public DbSet<Proveedor> Proveedor { get; set; }
        public DbSet<Producto> Productos { get; set; }
        public DbSet<Carrito> Carrito { get; set; }
        public DbSet<Compra> Compras { get; set; }
        public DbSet<Reporte> Reportes { get; set; }
        public DbSet<Pago> Pagos { get; set; }
        public DbSet<DetalleCompra> DetallesCompra { get; set; }
        public DbSet<Comprobante> Comprobantes { get; set; }


        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            //usuarios
            modelBuilder.Entity<Usuario>(tb =>
            {
                tb.HasKey(col => col.IdUsuario);
                tb.Property(col => col.IdUsuario).UseIdentityColumn().ValueGeneratedOnAdd();
                tb.Property(col => col.Nombres).HasMaxLength(50);
                tb.Property(col => col.Apellidos).HasMaxLength(50);
                tb.Property(col => col.Dni).HasMaxLength(8);
                tb.Property(col => col.Celular).HasMaxLength(9);
                tb.Property(col => col.Email).HasMaxLength(50);
                tb.Property(col => col.Password).HasMaxLength(50);
                tb.Property(col => col.Rol).HasMaxLength(20);
            });
            modelBuilder.Entity<Usuario>().ToTable("Usuarios");

            // clientes
            modelBuilder.Entity<Clientes>(tb =>
            {
                tb.HasKey(col => col.IdClientes);
                tb.Property(col => col.IdClientes).UseIdentityColumn().ValueGeneratedOnAdd();
                tb.Property(col => col.Nombres).HasMaxLength(50);
                tb.Property(col => col.Apellidos).HasMaxLength(50);
                tb.Property(col => col.Dni).HasMaxLength(8);
                tb.Property(col => col.Celular).HasMaxLength(9);
                tb.Property(col => col.Direccion).HasMaxLength(50);
                tb.Property(col => col.FechaRegistro).HasColumnType("datetime");
                tb.Property(col => col.Descripcion).HasMaxLength(100);
                tb.Property(col => col.Estado).HasMaxLength(20);
                tb.Property(col => col.Email).HasMaxLength(50);

            });
            modelBuilder.Entity<Clientes>().ToTable("Clientes_Mayoristas");


            //proveedores
            modelBuilder.Entity<Proveedor>(tb =>
            {
                tb.HasKey(col => col.IdProveedor);
                tb.Property(col => col.IdProveedor).UseIdentityColumn().ValueGeneratedOnAdd();
                tb.Property(col => col.Nombres).HasMaxLength(50);
                tb.Property(col => col.Apellidos).HasMaxLength(50);
                tb.Property(col => col.Dni).HasMaxLength(9);
                tb.Property(col => col.Celular).HasMaxLength(9);
                tb.Property(col => col.Direccion).HasMaxLength(50);
                tb.Property(col => col.FechaRegistro).HasColumnType("datetime");
                tb.Property(col => col.Descripcion).HasMaxLength(100);
                tb.Property(col => col.Tipo).HasMaxLength(100);
                tb.Property(col => col.Estado).HasMaxLength(20);
                tb.Property(col => col.Email).HasMaxLength(100);
            });
            modelBuilder.Entity<Proveedor>().ToTable("Proveedor");

            // tabla para productos
            modelBuilder.Entity<Producto>(tb =>
            {
                tb.HasKey(col => col.IdProducto);
                tb.Property(col => col.IdProducto).UseIdentityColumn().ValueGeneratedOnAdd();
                tb.Property(col => col.Nombre).IsRequired().HasMaxLength(50);
                tb.Property(col => col.Categoria).IsRequired().HasMaxLength(50);
                tb.Property(col => col.Descripcion).IsRequired().HasMaxLength(100);
                tb.Property(col => col.Stock).IsRequired().HasMaxLength(100);
                tb.Property(col => col.FechaProduccion).HasColumnType("datetime");
                tb.Property(col => col.FechaVencimiento).HasColumnType("datetime");
                tb.Property(col => col.Precio).IsRequired().HasPrecision(18, 2);
                tb.Property(col => col.Estado).HasMaxLength(20);
                tb.Property(col => col.UrlImagen).HasMaxLength(200);
            });
            modelBuilder.Entity<Producto>().ToTable("Producto");

            modelBuilder.Entity<Carrito>(tb =>
            {
                tb.HasKey(col => col.IdCarrito);
                tb.Property(col => col.IdCarrito).UseIdentityColumn().ValueGeneratedOnAdd();

                tb.Property(col => col.UsuarioId)
                  .IsRequired()
                  .HasMaxLength(450);  // Ajusta este valor según el tamaño del Id que uses

                tb.Property(col => col.IdProducto).IsRequired();
                tb.Property(col => col.Nombre).HasMaxLength(100);
                tb.Property(col => col.Categoria).HasMaxLength(100);
                tb.Property(col => col.Precio).HasColumnType("decimal(18,2)");
                tb.Property(col => col.Cantidad).IsRequired();

                tb.ToTable("Carrito");
            });


            modelBuilder.Entity<Compra>(entity =>
            {
                entity.HasKey(c => c.IdCompra);
                entity.Property(c => c.IdCompra).UseIdentityColumn().ValueGeneratedOnAdd();

                entity.Property(c => c.FechaCompra).IsRequired();

                entity.Property(c => c.Total)
                      .HasPrecision(10, 2)
                      .IsRequired();

                entity.HasOne(c => c.UsuarioCliente)
                      .WithMany()
                      .HasForeignKey(c => c.IdUsuarioCliente)
                      .OnDelete(DeleteBehavior.Restrict);

                entity.HasOne(c => c.Pago)
                      .WithOne(p => p.Compra)
                      .HasForeignKey<Pago>(p => p.IdCompra)
                      .OnDelete(DeleteBehavior.Cascade);

                entity.HasMany(c => c.DetallesCompra)
                      .WithOne(dc => dc.Compra)
                      .HasForeignKey(dc => dc.IdCompra);

                entity.HasMany(c => c.Reportes)
                      .WithOne(r => r.Compra)
                      .HasForeignKey(r => r.IdCompra);

                entity.HasMany(c => c.Comprobantes)
                      .WithOne(cp => cp.Compra)
                      .HasForeignKey(cp => cp.IdCompra);

                entity.ToTable("Compra");
            });

            modelBuilder.Entity<Pago>(entity =>
            {
                entity.HasKey(e => e.IdPago);

                entity.Property(e => e.Monto)
                      .HasColumnType("decimal(18,2)")
                      .IsRequired();

                entity.Property(e => e.FechaPago)
                      .IsRequired();

                entity.Property(e => e.MetodoPago)
                      .HasMaxLength(50)
                      .IsRequired();

                entity.Property(e => e.NumeroTarjeta)
                      .HasMaxLength(20);

                entity.Property(e => e.NombreTitular)
                      .HasMaxLength(100);

                entity.Property(e => e.Cvv)
                      .HasMaxLength(10);

                // NO repetir relación aquí para evitar conflicto con Compra
                // La relación uno a uno ya se configuró en Compra con HasOne/WithOne
            });
            modelBuilder.Entity<Comprobante>(entity =>
            {
                entity.HasKey(e => e.IdComprobante);  // Clave primaria

                entity.Property(e => e.ArchivoPdf)
                      .HasColumnType("varbinary(max)");  // Si usas PostgreSQL, o "varbinary(max)" para SQL Server

                entity.Property(e => e.FechaGeneracion)
                      .HasColumnType("datetime")
                      .IsRequired();

                entity.HasOne(e => e.Compra)
                      .WithMany(c => c.Comprobantes)
                      .HasForeignKey(e => e.IdCompra)
                      .OnDelete(DeleteBehavior.Cascade);

                entity.ToTable("Comprobante");
            });

            modelBuilder.Entity<DetalleCompra>(entity =>
            {
                entity.HasKey(e => e.IdDetalleCompra);  // Clave primaria

                entity.Property(e => e.Cantidad).IsRequired();

                entity.Property(e => e.PrecioUnitario)
                      .HasColumnType("decimal(18,2)")
                      .IsRequired();

                entity.Property(e => e.SubTotal)
                      .HasColumnType("decimal(18,2)")
                      .IsRequired();

                entity.HasOne(e => e.Compra)
                      .WithMany(c => c.DetallesCompra)
                      .HasForeignKey(e => e.IdCompra)
                      .OnDelete(DeleteBehavior.Cascade);

                entity.HasOne(e => e.Producto)
                      .WithMany()
                      .HasForeignKey(e => e.IdProducto)
                      .OnDelete(DeleteBehavior.Restrict);

                entity.ToTable("DetalleCompra");
            });
            modelBuilder.Entity<Reporte>(entity =>
            {
                entity.HasKey(e => e.IdReporte);

                entity.HasOne(e => e.Comprobante)
                      .WithMany()  // Si Comprobante no tiene colección de Reportes
                      .HasForeignKey(e => e.IdComprobante)
                      .OnDelete(DeleteBehavior.Restrict);

                entity.HasOne(e => e.Compra)
                      .WithMany()  // Si Compra no tiene colección de Reportes, sino ponla si la tiene
                      .HasForeignKey(e => e.IdCompra)
                      .OnDelete(DeleteBehavior.Restrict);

                entity.HasOne(e => e.Producto)
                      .WithMany()  // Igual, si Producto no tiene colección de Reportes
                      .HasForeignKey(e => e.IdProducto)
                      .OnDelete(DeleteBehavior.Restrict);

                entity.Property(e => e.PrecioUnitario)
         .HasColumnType("decimal(18,2)");

                entity.Property(e => e.Subtotal)
                      .HasColumnType("decimal(18,2)");


                entity.ToTable("Reporte");
            });


        }
    }
}
