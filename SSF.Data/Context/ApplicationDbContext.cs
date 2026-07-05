using Microsoft.EntityFrameworkCore;
using SSF.Domain.Entities;

namespace SSF.Data.Context
{
    public class ApplicationDbContext : DbContext
    {
        // El constructor recibe las opciones de configuración (como el String de Conexión)
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
        {
        }

        // Declaramos los sets de datos que se transformarán en tablas
        public DbSet<Usuario> Usuarios { get; set; }
        public DbSet<Rol> Roles { get; set; }
        public DbSet<Sucursal> Sucursales { get; set; }
        public DbSet<Caja> Cajas{ get; set; }
        public DbSet<Producto> Productos { get; set; }
        public DbSet<StockSucursal> StockSucursales { get; set; }
        public DbSet<Venta> Ventas { get; set; }
        public DbSet<VentaDetalle> VentaDetalles { get; set; }
        public DbSet<VentaPago> VentaPagos { get; set; }
        public DbSet<MedioPago> MediosPago { get; set; }
        public DbSet<LogAuditoria> LogsAuditoria { get; set; }
        // Acá aplicamos la configuración avanzada con Fluent API
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // 1. Configuramos el esquema "Seguridad" para aislar estas tablas
            modelBuilder.HasDefaultSchema("Seguridad");

            // 2. Reglas para la tabla Roles
            modelBuilder.Entity<Rol>(entity =>
            {
                entity.ToTable("Roles");
                entity.HasKey(r => r.Id);
                entity.Property(r => r.Nombre).IsRequired().HasMaxLength(50);
            });

            // 3. Reglas para la tabla Usuarios
            modelBuilder.Entity<Usuario>(entity =>
            {
                entity.ToTable("Usuarios");
                entity.HasKey(u => u.Id);
                entity.Property(u => u.Username).IsRequired().HasMaxLength(50);
                entity.Property(u => u.Email).IsRequired().HasMaxLength(100);
                entity.Property(u => u.PasswordHash).IsRequired().HasMaxLength(500);

                // Configuración de la relación 1 a Muchos (Un Rol tiene muchos Usuarios)
                entity.HasOne(u => u.Rol)
                      .WithMany(r => r.Usuarios)
                      .HasForeignKey(u => u.RolId)
                      .OnDelete(DeleteBehavior.Restrict); // Evita borrados en cascada accidentales

            });

            // Reglas para la tabla Sucursales
            modelBuilder.Entity<Sucursal>(entity =>
            {
                entity.ToTable("Sucursales");
                entity.HasKey(s => s.Id);
                entity.Property(s => s.Nombre).IsRequired().HasMaxLength(100);
                entity.Property(s => s.Direccion).HasMaxLength(200); // Es opcional (?s), pero limitamos el tamaño
            });

            // Reglas para la tabla Cajas
            modelBuilder.Entity<Caja>(entity =>
            {
                entity.ToTable("Cajas");
                entity.HasKey(c => c.Id);
                entity.Property(c => c.Nombre).IsRequired().HasMaxLength(50);
                entity.Property(c => c.DeviceToken).HasMaxLength(100);

                // Configuración de la relación 1 a Muchos (Una Sucursal tiene muchas Cajas)
                entity.HasOne(c => c.Sucursal)
                      .WithMany(s => s.Cajas)
                      .HasForeignKey(c => c.SucursalId)
                      .OnDelete(DeleteBehavior.Restrict); // Si se elimina una sucursal, no borra las cajas en cascada
            });

            // Reglas para la tabla Productos
            modelBuilder.Entity<Producto>(entity =>
            {
                entity.ToTable("Productos", "Stock");
                entity.HasKey(p => p.Id);

                entity.Property(p => p.Id);
                entity.Property(p => p.CodigoBarra).HasMaxLength(50);
                entity.Property(p => p.CodigoPLU).HasMaxLength(5);
                entity.Property(p => p.Descripcion).IsRequired().HasMaxLength(150);
                entity.Property(p => p.PrecioVenta).HasColumnType("decimal(18,2)").IsRequired();
                entity.Property(p => p.ManejaPeso).HasDefaultValue(true);
                entity.Property(p => p.ProveedorTexto).HasMaxLength(100);

                entity.HasIndex(p => p.CodigoBarra).HasDatabaseName("IDX_Productos_CodigoBarra").HasFilter("[CodigoBarra] IS NOT NULL");
                entity.HasIndex(p => p.CodigoPLU).HasDatabaseName("IDX_Productos_CodigoPLU").HasFilter("[CodigoPLU] IS NOT NULL");
            });

            // Reglas para la tabla StockSucursal
            modelBuilder.Entity<StockSucursal>(entity =>
            {
                entity.ToTable("StockSucursal", "Stock");
                entity.HasKey(ss => ss.Id);

                entity.Property(ss => ss.Id);

                // Configuramos los 3 decimales indispensables para pesar el fiambre
                entity.Property(ss => ss.Cantidad)
                      .HasColumnType("decimal(18,3)")
                      .HasDefaultValue(0.000m)
                      .IsRequired();

                // Mapeo de claves foráneas a nombres de SQL
                entity.Property(ss => ss.ProductoId).HasColumnName("IdProducto");
                entity.Property(ss => ss.SucursalId).HasColumnName("IdSucursal");

                // Relación con Productos
                entity.HasOne(ss => ss.Producto)
                      .WithMany() // Un producto puede estar en el stock de muchas sucursales
                      .HasForeignKey(ss => ss.ProductoId)
                      .OnDelete(DeleteBehavior.Restrict);

                // Relación con Sucursales
                entity.HasOne(ss => ss.Sucursal)
                      .WithMany() // Una sucursal tiene muchos registros de stock
                      .HasForeignKey(ss => ss.SucursalId)
                      .OnDelete(DeleteBehavior.Restrict);

                // Creamos un índice único compuesto. 
                // Evita que por error haya dos filas de "IdProducto 5 en IdSucursal 1". El stock de un artículo por local debe ser único.
                entity.HasIndex(ss => new { ss.ProductoId, ss.SucursalId }).IsUnique();

                // Índices condicionales (Filtered Indexes) para búsquedas en mostrador
                entity.HasIndex(ss => new { ss.SucursalId, ss.ProductoId }).HasDatabaseName("IDX_StockSucursal_Combinado");
            });

            // Reglas para la tabla Ventas
            modelBuilder.Entity<Venta>(entity =>
            {
                entity.ToTable("Ventas", "Facturacion"); // Esquema Facturacion
                entity.HasKey(v => v.Id);

                entity.Property(v => v.Id);
                entity.Property(v => v.Fecha).IsRequired();
                entity.Property(v => v.Total).HasColumnType("decimal(18,2)").IsRequired();
                entity.Property(v => v.RutaPDF).IsRequired().HasMaxLength(255);

                // Mapeo de nombres de columnas extranjeras de tu script
                entity.Property(v => v.CajaId).HasColumnName("IdCaja");
                entity.Property(v => v.UsuarioId).HasColumnName("IdUsuario");

                // Relación con Cajas (viven en esquema Seguridad)
                entity.HasOne(v => v.Caja)
                      .WithMany()
                      .HasForeignKey(v => v.CajaId)
                      .OnDelete(DeleteBehavior.Restrict);

                // Relación con Usuarios (viven en esquema Seguridad)
                entity.HasOne(v => v.Usuario)
                      .WithMany()
                      .HasForeignKey(v => v.UsuarioId)
                      .OnDelete(DeleteBehavior.Restrict);

                // Índice compuesto para acelerar el filtro de stock por local
                entity.HasIndex(v => new { v.CajaId, v.UsuarioId }).HasDatabaseName("IDX_Ventas_CajaUsuario");
            });

            // Reglas para la tabla VentaDetalles
            modelBuilder.Entity<VentaDetalle>(entity =>
            {
                entity.ToTable("VentaDetalles", "Facturacion");
                entity.HasKey(vd => vd.Id);

                entity.Property(vd => vd.Id);

                // Mantenemos la precisión clave de 3 decimales para pesar el fiambre
                entity.Property(vd => vd.Cantidad).HasColumnType("decimal(18,3)").IsRequired();
                entity.Property(vd => vd.PrecioUnitario).HasColumnType("decimal(18,2)").IsRequired();
                entity.Property(vd => vd.Subtotal).HasColumnType("decimal(18,2)").IsRequired();

                // Mapeo de claves foráneas
                entity.Property(vd => vd.VentaId).HasColumnName("IdVenta");
                entity.Property(vd => vd.ProductoId).HasColumnName("IdProducto");

                // Relación con Ventas (Si se borra la venta, se eliminan sus renglones en cascada)
                entity.HasOne(vd => vd.Venta)
                      .WithMany(vd => vd.Detalles)
                      .HasForeignKey(vd => vd.VentaId)
                      .OnDelete(DeleteBehavior.Cascade);

                // Relación con Productos (viven en esquema Stock)
                entity.HasOne(vd => vd.Producto)
                      .WithMany()
                      .HasForeignKey(vd => vd.ProductoId)
                      .OnDelete(DeleteBehavior.Restrict);

                // Acelera la carga de renglones en el PDF o pantalla
                entity.HasIndex(vd => new { vd.VentaId, vd.ProductoId }).HasDatabaseName("IDX_VentaDetalles_VentaProducto");
            });

            // Reglas para la tabla MedioPagos
            modelBuilder.Entity<MedioPago>(entity =>
            {
                entity.ToTable("MediosPagos", "Facturacion");
                entity.HasKey(mp => mp.Id);

                entity.Property(mp => mp.Id);
                entity.Property(mp => mp.NombreMedio).IsRequired().HasMaxLength(50);
                entity.Property(mp => mp.Activo).HasDefaultValue(true);

                //Precargamos los medios de pago estándar de Argentina
                entity.HasData(
                    new MedioPago { Id = 1, NombreMedio = "Efectivo", Activo = true },
                    new MedioPago { Id = 2, NombreMedio = "Tarjeta de Débito", Activo = true },
                    new MedioPago { Id = 3, NombreMedio = "Tarjeta de Crédito", Activo = true },
                    new MedioPago { Id = 4, NombreMedio = "Mercado Pago (QR/Transferencia)", Activo = true }
                );
            });

            // Reglas para la tabla VentaPagos
            modelBuilder.Entity<VentaPago>(entity =>
            {
                entity.ToTable("VentaPagos", "Facturacion");
                entity.HasKey(vp => vp.Id);

                entity.Property(vp => vp.Id);
                entity.Property(vp => vp.Monto).HasColumnType("decimal(18,2)").IsRequired();
                entity.Property(vp => vp.NroReferencia).HasMaxLength(50); // Mismo tamaño que tu script

                entity.Property(vp => vp.VentaId).HasColumnName("IdVenta");
                entity.Property(vp => vp.MedioPagoId).HasColumnName("IdMedioPago");

                // Relación bidireccional con Ventas (Si se elimina la venta, se eliminan sus pagos en cascada)
                entity.HasOne(vp => vp.Venta)
                      .WithMany(v => v.Pagos)
                      .HasForeignKey(vp => vp.VentaId)
                      .OnDelete(DeleteBehavior.Cascade);

                // Relación unidireccional con MediosPago (El medio de pago no necesita saber qué facturas se pagaron con él)
                entity.HasOne(vp => vp.MedioPago)
                      .WithMany()
                      .HasForeignKey(vp => vp.MedioPagoId)
                      .OnDelete(DeleteBehavior.Restrict);

                // Agiliza el cierre de caja por medio de pago (Efectivo, MP, etc)
                entity.HasIndex(vp => new { vp.VentaId, vp.MedioPagoId }).HasDatabaseName("IDX_VentaPagos_VentaMedio");
            });

            modelBuilder.Entity<LogAuditoria>(entity =>
            {
                // Cambiamos esto para especificar el esquema 'Seguridad'
                entity.ToTable("LogsAuditoria", "Seguridad");

                entity.HasKey(e => e.Id);

                entity.Property(e => e.Accion).HasMaxLength(100).IsRequired();
                entity.Property(e => e.Detalle).IsRequired();
                entity.Property(e => e.IpDireccion).HasMaxLength(45).IsRequired();

                // Relaciones (quedan igual)
                entity.HasOne(d => d.Usuario)
                      .WithMany()
                      .HasForeignKey(d => d.IdUsuario)
                      .OnDelete(DeleteBehavior.Restrict);

                entity.HasOne(d => d.Sucursal)
                      .WithMany()
                      .HasForeignKey(d => d.IdSucursal)
                      .OnDelete(DeleteBehavior.Restrict);
            });

            //INYECCIÓN DE DATOS INICIALES (Data Seeding)
            modelBuilder.Entity<Rol>().HasData(
                new Rol { Id = 1, Nombre = "Administrador" },
                new Rol { Id = 2, Nombre = "Cajero" }
            );

        }
    }
}