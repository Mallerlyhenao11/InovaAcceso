using InovaAcceso.Models;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel;

namespace InovaAcceso.Data
{
    public class AppDBContext : DbContext
    {
        public AppDBContext(DbContextOptions<AppDBContext> options) : base(options)
        {

        }

        public DbSet<Cargo> Cargos { get; set; }
        public DbSet<Estado> Estados { get; set; }
        public DbSet<TipoDocumento> TipoDocumentos { get; set; }
        public DbSet<Turno> Turnos { get; set; }
        public DbSet<Persona> Personas { get; set; }
        public DbSet<Rol> Rols { get; set; }
        public DbSet<RegistroAsistencia> RegistroAsistencias { get; set; }
        public DbSet<GestionTurno> GestionTurnos { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            //base.OnModelCreating(modelBuilder);
            modelBuilder.Entity<Rol>(tb =>
            {
                tb.HasKey(col => col.IdRol);
                tb.Property(col => col.IdRol)
                .UseIdentityColumn()
                .ValueGeneratedOnAdd();

                tb.Property(col => col.NombreRol).HasMaxLength(50);
                tb.Property(col => col.FechaCreacion);
                tb.Property(col => col.FechaModificacion);
                tb.Property(col => col.ResponsableModificacion).HasMaxLength(100);

            });
            modelBuilder.Entity<Rol>().ToTable("Rol");

            modelBuilder.Entity<Cargo>(tb =>
            {
                tb.HasKey(col => col.IdCargo);
                tb.Property(col => col.IdCargo)
                .UseIdentityColumn()
                .ValueGeneratedOnAdd();

                tb.Property(col => col.NombreCargo).HasMaxLength(50);
                tb.Property(col => col.FechaCreacion);
                tb.Property(col => col.FechaModificacion);
                tb.Property(col => col.ResponsableModificacion).HasMaxLength(100);

            });
            modelBuilder.Entity<Cargo>().ToTable("Cargo");

            //La tabla estado
            modelBuilder.Entity<Estado>(tb =>
            {
                tb.HasKey(col => col.IdEstado);
                tb.Property(col => col.IdEstado)
                .UseIdentityColumn()
                .ValueGeneratedOnAdd();

                tb.Property(col => col.NombreEstado).HasMaxLength(50);
                tb.Property(col => col.FechaCreacion);
                tb.Property(col => col.FechaModificacion);
                tb.Property(col => col.ResponsableModificacion).HasMaxLength(100);

            });
            modelBuilder.Entity<Estado>().ToTable("Estado");

            //La tabla TipoDocumento
            modelBuilder.Entity<TipoDocumento>(tb =>
            {
                tb.HasKey(col => col.IdTipoDoc);
                tb.Property(col => col.IdTipoDoc)
                .UseIdentityColumn()
                .ValueGeneratedOnAdd();

                tb.Property(col => col.Documento).HasMaxLength(50);
                tb.Property(col => col.FechaCreacion);
                tb.Property(col => col.FechaModificacion);
                tb.Property(col => col.ResponsableModificacion).HasMaxLength(100);
            });
            modelBuilder.Entity<TipoDocumento>().ToTable("TipoDocumento");

            //La tabla Turno
            modelBuilder.Entity<Turno>(tb =>
            {
                tb.HasKey(col => col.IdTurno);
                tb.Property(col => col.IdTurno)
                .UseIdentityColumn()
                .ValueGeneratedOnAdd();

                tb.Property(col => col.NombreTurno).HasMaxLength(50);
                tb.Property(col => col.HoraIngreso).HasColumnType("time(0)");
                tb.Property(col => col.HoraSalida).HasColumnType("time(0)");
                tb.Property(col => col.ToleranciaMinutos);
                tb.Property(col => col.HorasTurno);
                tb.Property(col => col.FechaCreacion);
                tb.Property(col => col.FechaModificacion);
                tb.Property(col => col.ResponsableModificacion).HasMaxLength(100);
            });
            modelBuilder.Entity<Turno>().ToTable("Turno");
            // La tabla Persona
            modelBuilder.Entity<Persona>(tb =>
            {
                
                tb.HasKey(col => col.IdPersona);
                tb.Property(col => col.IdPersona)
                    .UseIdentityColumn()
                    .ValueGeneratedOnAdd();

                tb.Property(col => col.NumeroDocumento).IsRequired();
                tb.Property(col => col.PrimerNombre).IsRequired().HasMaxLength(100);
                tb.Property(col => col.SegundoNombre) .HasMaxLength(100);
                tb.Property(col => col.PrimerApellido).IsRequired().HasMaxLength(100);
                tb.Property(col => col.SegundoApellido).HasMaxLength(100);
                tb.Property(col => col.FechaNacimiento).HasColumnType("date");
                tb.Property(col => col.Edad).IsRequired();
                tb.Property(col => col.Sexo).IsRequired() .HasMaxLength(10);
                tb.Property(col => col.FechaIngreso).IsRequired().HasColumnType("date");
                tb.Property(col => col.Direccion).IsRequired().HasMaxLength(255);
                tb.Property(col => col.Telefono).IsRequired().HasMaxLength(15);
                tb.Property(col => col.Email).IsRequired().HasMaxLength(100);
                tb.Property(col => col.Contrasena).IsRequired();
                tb.Property(col => col.Restablecer).IsRequired().HasDefaultValue(false);
                tb.Property(col => col.FechaCreacion).IsRequired().HasColumnType("datetime");
                tb.Property(col => col.FechaModificacion).IsRequired().HasColumnType("datetime");
                tb.Property(col => col.ResponsableModificacion).IsRequired().HasMaxLength(100);

                // Configuración de las llaves foráneas
                tb.HasOne(col => col.Cargo)
                    .WithMany()
                    .HasForeignKey(col => col.IdCargo)
                    .OnDelete(DeleteBehavior.Cascade);

                tb.HasOne(col => col.TipoDocumento)
                    .WithMany()
                    .HasForeignKey(col => col.IdTipoDoc)
                    .OnDelete(DeleteBehavior.Cascade);
                tb.HasOne(col => col.Estado)
                    .WithMany()
                    .HasForeignKey(col => col.IdEstado)
                    .OnDelete(DeleteBehavior.Cascade);
                tb.HasOne(col => col.Rol)
                    .WithMany()
                    .HasForeignKey(col => col.IdRol)
                    .OnDelete(DeleteBehavior.Cascade);
            });


            modelBuilder.Entity<Persona>().ToTable("Persona");


            //Tabla RegistroAsistencia

            modelBuilder.Entity<RegistroAsistencia>(tb =>
            {
                tb.HasKey(col => col.IdRegistro);
                tb.Property(col => col.IdRegistro)
                    .UseIdentityColumn()
                    .ValueGeneratedOnAdd();

                tb.HasOne(col => col.Persona)
                .WithMany()
                .HasForeignKey(col => col.IdPersona)
                .OnDelete(DeleteBehavior.Cascade);

                tb.HasOne(col => col.Turno)
                .WithMany()
                .HasForeignKey(col => col.IdTurno)
                .OnDelete(DeleteBehavior.Cascade);

                tb.Property(col => col.FechaIngreso).HasColumnType("date").IsRequired();
                tb.Property(col => col.HoraIngreso).HasColumnType("time(7)").IsRequired();
                tb.Property(col => col.HoraSalida).HasColumnType("time(7)").IsRequired();
                tb.Property(col => col.LlegadaTarde).HasColumnType("bit").HasDefaultValue(false);
                tb.Property(col => col.Tardanza).IsRequired();
                tb.Property(col => col.FechaCreacion).HasColumnType("datetime");
                tb.Property(col => col.FechaModificacion).HasColumnType("datetime");
                tb.Property(col => col.ResponsableModificacion).IsRequired().HasMaxLength(100); ;


            });

            modelBuilder.Entity<RegistroAsistencia>().ToTable("RegistroAsistencia");


            //Aca la tabla Gestion Turno
            modelBuilder.Entity<GestionTurno>(tb =>
			{
				tb.HasKey(col => col.IdGestionTurno);
				tb.Property(col => col.IdGestionTurno)
                    .UseIdentityColumn()
                    .ValueGeneratedOnAdd();

                tb.HasOne(col => col.Persona)
                    .WithMany()
                    .HasForeignKey(col => col.IdPersona)
                    .OnDelete(DeleteBehavior.Cascade);

                tb.HasOne(col => col.Turno)
                    .WithMany()
                    .HasForeignKey(col => col.IdTurno)
                    .OnDelete(DeleteBehavior.Cascade);

                tb.Property(col => col.FechaInicio).HasColumnType("date").IsRequired();
                tb.Property(col => col.FechaFin).HasColumnType("date").IsRequired();
				tb.Property(col => col.FechaCreacion).HasColumnType("datetime").IsRequired();
				tb.Property(col => col.FechaModificacion).HasColumnType("datetime").IsRequired();
				tb.Property(col => col.ResponsableModificacion).HasMaxLength(50).IsRequired();
			});
			modelBuilder.Entity<GestionTurno>().ToTable("GestionTurno");



		}
	}
}
