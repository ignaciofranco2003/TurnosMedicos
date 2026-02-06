using Microsoft.EntityFrameworkCore;
using TurnosMedicos.Entities;
using TurnosMedicos.Entities.Values;

namespace TurnosMedicos.Data;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

    public DbSet<Especialidad> Especialidades { get; set; } = null!;
    public DbSet<Medico> Medicos { get; set; } = null!;
    public DbSet<ObraSocial> ObrasSociales { get; set; } = null!;
    public DbSet<Paciente> Pacientes { get; set; } = null!;
    public DbSet<Turno> Turnos { get; set; } = null!;
    public DbSet<TurnosMedicos.Auth.Entities.User> Users { get; set; } = null!;

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // Paciente
        modelBuilder.Entity<Paciente>(b =>
        {
            b.HasIndex(p => p.DNI).IsUnique();
            b.HasOne(p => p.ObraSocial)
                .WithMany(o => o.Pacientes)
                .HasForeignKey(p => p.IdObraSocial)
                .OnDelete(DeleteBehavior.SetNull);
        });

        // ObraSocial
        modelBuilder.Entity<ObraSocial>(b =>
        {
            b.HasIndex(o => o.Nombre).IsUnique();
        });

        // Medico
        modelBuilder.Entity<Medico>(b =>
        {
            b.HasIndex(m => m.Matricula).IsUnique();
            b.HasIndex(m => m.DNI).IsUnique();
        });

        // Explicit join entity MedicoEspecialidad for many-to-many (allows metadata later)
        modelBuilder.Entity<MedicoEspecialidad>(b =>
        {
            b.HasKey(me => new { me.MedicoId, me.EspecialidadId });
            b.HasOne(me => me.Medico).WithMany(m => m.MedicoEspecialidades).HasForeignKey(me => me.MedicoId);
            b.HasOne(me => me.Especialidad).WithMany(e => e.MedicoEspecialidades).HasForeignKey(me => me.EspecialidadId);
        });

        // DisponibilidadMedico -> Medico
        modelBuilder.Entity<DisponibilidadMedico>(b =>
        {
            b.HasOne(d => d.Medico)
                .WithMany(m => m.Disponibilidades)
                .HasForeignKey(d => d.MedicoId)
                .OnDelete(DeleteBehavior.Cascade);

            // Prevent duplicate availability for same day/time range for a medico
            b.HasIndex(d => new { d.MedicoId, d.DiaSemana, d.HoraDesde, d.HoraHasta }).IsUnique();
        });

        // Turno relationships
        modelBuilder.Entity<Turno>(b =>
        {
            b.HasOne(t => t.Paciente)
                .WithMany(p => p.Turnos)
                .HasForeignKey(t => t.IdPaciente)
                .OnDelete(DeleteBehavior.SetNull);

            b.HasOne(t => t.Medico)
                .WithMany(m => m.Turnos)
                .HasForeignKey(t => t.IdMedico)
                .OnDelete(DeleteBehavior.SetNull);

            // Prevent two turnos starting at same time for same medico
            b.HasIndex(t => new { t.IdMedico, t.Inicio }).IsUnique();

            // Add index to help range queries (Inicio/Fin)
            b.HasIndex(t => new { t.IdMedico, t.Inicio, t.Fin });
        });

        // Configure concurrency tokens (rowversion)
        modelBuilder.Entity<Medico>().Property(m => m.RowVersion).IsRowVersion();
        modelBuilder.Entity<Paciente>().Property(p => p.RowVersion).IsRowVersion();
        modelBuilder.Entity<ObraSocial>().Property(o => o.RowVersion).IsRowVersion();
        modelBuilder.Entity<Turno>().Property(t => t.RowVersion).IsRowVersion();
        modelBuilder.Entity<Especialidad>().Property(e => e.RowVersion).IsRowVersion();
        modelBuilder.Entity<DisponibilidadMedico>().Property(d => d.RowVersion).IsRowVersion();
    }

}