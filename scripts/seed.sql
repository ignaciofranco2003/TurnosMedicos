-- Script para limpiar tablas y cargar datos de prueba
-- Ejecutar contra la base de datos configurada (MySQL)

SET FOREIGN_KEY_CHECKS = 0;

TRUNCATE TABLE Turnos;
TRUNCATE TABLE DisponibilidadMedico;
TRUNCATE TABLE MedicoEspecialidad;
TRUNCATE TABLE Pacientes;
TRUNCATE TABLE Medicos;
TRUNCATE TABLE Especialidades;
TRUNCATE TABLE ObrasSociales;
TRUNCATE TABLE Users;

SET FOREIGN_KEY_CHECKS = 1;

-- Inserta 5 obras sociales
INSERT INTO ObrasSociales (Id, Nombre) VALUES
  (1, 'OSDE'),
  (2, 'Swiss Medical'),
  (3, 'Galeno'),
  (4, 'Medicus'),
  (5, 'IOMA');

-- Inserta 6 especialidades
INSERT INTO Especialidades (Id, NombreEspecialidad) VALUES
  (1, 'Cardiología'),
  (2, 'Neurología'),
  (3, 'Pediatría'),
  (4, 'Dermatología'),
  (5, 'Ginecología'),
  (6, 'Traumatología');

-- Inserta 5 medicos (DuracionTurnoMin: 30 para todos)
INSERT INTO Medicos (Id, Nombre, DNI, Telefono, Matricula, DuracionTurnoMin) VALUES
  (1, 'Dr. Juan Pérez', '12345678', '2951-111111', 'M-1001', 30),
  (2, 'Dra. María Gómez', '22345678', '2951-222222', 'M-1002', 30),
  (3, 'Dr. Carlos Ruiz', '32345678', '2951-333333', 'M-1003', 30),
  (4, 'Dra. Laura Díaz', '42345678', '2951-444444', 'M-1004', 30),
  (5, 'Dr. Alberto Fernández', '52345678', '2951-555555', 'M-1005', 30);

-- Asignar especialidades a medicos (varias por medico)
INSERT INTO MedicoEspecialidad (MedicoId, EspecialidadId) VALUES
  (1, 1), (1, 4),                    -- Dr. Juan Pérez: Cardiología, Dermatología
  (2, 2), (2, 3),                    -- Dra. María Gómez: Neurología, Pediatría
  (3, 1), (3, 6), (3, 5),            -- Dr. Carlos Ruiz: Cardiología, Traumatología, Ginecología
  (4, 3), (4, 4),                    -- Dra. Laura Díaz: Pediatría, Dermatología
  (5, 6), (5, 2);                    -- Dr. Alberto Fernández: Traumatología, Neurología

-- Inserta 15 pacientes (algunos con obra social, otros no)
INSERT INTO Pacientes (Id, Nombre, DNI, Telefono, TieneObraSocial, IdObraSocial) VALUES
  (1,  'Ana López',          '70000001', '2999-000001', 1, 1),
  (2,  'Pedro Martínez',     '70000002', '2999-000002', 1, 2),
  (3,  'Lucía Sánchez',      '70000003', '2999-000003', 0, NULL),
  (4,  'Jorge Ramírez',      '70000004', '2999-000004', 1, 3),
  (5,  'Mariana Torres',     '70000005', '2999-000005', 1, 1),
  (6,  'Federico Díaz',      '70000006', '2999-000006', 0, NULL),
  (7,  'Camila Herrera',     '70000007', '2999-000007', 1, 4),
  (8,  'Martín Ruiz',        '70000008', '2999-000008', 1, 5),
  (9,  'Valentina Pinto',    '70000009', '2999-000009', 0, NULL),
  (10, 'Sofía García',       '70000010', '2999-000010', 1, 2),
  (11, 'Diego Blanco',       '70000011', '2999-000011', 1, 3),
  (12, 'Florencia Ortiz',    '70000012', '2999-000012', 0, NULL),
  (13, 'Nicolás Vega',       '70000013', '2999-000013', 1, 4),
  (14, 'Laura Medina',       '70000014', '2999-000014', 0, NULL),
  (15, 'Hugo Morales',       '70000015', '2999-000015', 1, 5);

-- Inserta 30 turnos distribuidos entre los 5 medicos
-- Se garantiza que para cada medico no haya inicio duplicado (índice único sobre IdMedico + Inicio)
-- Fecha base: 2026-02-20

INSERT INTO Turnos (Id, IdPaciente, IdMedico, Inicio, Fin, Estado, Observaciones) VALUES
  (1,  1,  1, '2026-02-20 09:00:00', '2026-02-20 09:30:00', 1, 'Primera consulta'),
  (2,  2,  1, '2026-02-20 09:30:00', '2026-02-20 10:00:00', 0, NULL),
  (3,  3,  1, '2026-02-20 10:00:00', '2026-02-20 10:30:00', 0, NULL),
  (4,  4,  1, '2026-02-21 09:00:00', '2026-02-21 09:30:00', 3, 'Atendido'),
  (5,  5,  1, '2026-02-21 09:30:00', '2026-02-21 10:00:00', 2, 'Cancelado por paciente'),

  (6,  6,  2, '2026-02-20 09:00:00', '2026-02-20 09:30:00', 1, NULL),
  (7,  7,  2, '2026-02-20 09:30:00', '2026-02-20 10:00:00', 0, NULL),
  (8,  8,  2, '2026-02-20 10:00:00', '2026-02-20 10:30:00', 4, 'Paciente ausente'),
  (9,  9,  2, '2026-02-21 09:00:00', '2026-02-21 09:30:00', 0, NULL),
  (10, 10,  2, '2026-02-21 09:30:00', '2026-02-21 10:00:00', 0, NULL),

  (11, 11,  3, '2026-02-20 11:00:00', '2026-02-20 11:30:00', 1, 'Control'),
  (12, 12,  3, '2026-02-20 11:30:00', '2026-02-20 12:00:00', 0, NULL),
  (13, 13,  3, '2026-02-21 11:00:00', '2026-02-21 11:30:00', 0, NULL),
  (14, 14,  3, '2026-02-21 11:30:00', '2026-02-21 12:00:00', 2, 'Paciente canceló'),
  (15, 15,  3, '2026-02-22 11:00:00', '2026-02-22 11:30:00', 0, NULL),

  (16,  1,  4, '2026-02-20 14:00:00', '2026-02-20 14:30:00', 1, NULL),
  (17,  2,  4, '2026-02-20 14:30:00', '2026-02-20 15:00:00', 0, NULL),
  (18,  3,  4, '2026-02-21 14:00:00', '2026-02-21 14:30:00', 0, NULL),
  (19,  4,  4, '2026-02-21 14:30:00', '2026-02-21 15:00:00', 3, 'Atendido'),
  (20,  5,  4, '2026-02-22 14:00:00', '2026-02-22 14:30:00', 0, NULL),

  (21,  6,  5, '2026-02-20 16:00:00', '2026-02-20 16:30:00', 1, NULL),
  (22,  7,  5, '2026-02-20 16:30:00', '2026-02-20 17:00:00', 0, NULL),
  (23,  8,  5, '2026-02-21 16:00:00', '2026-02-21 16:30:00', 0, NULL),
  (24,  9,  5, '2026-02-21 16:30:00', '2026-02-21 17:00:00', 2, 'Cancelado por clínica'),
  (25, 10,  5, '2026-02-22 16:00:00', '2026-02-22 16:30:00', 0, NULL),

  (26, 11,  1, '2026-02-22 09:00:00', '2026-02-22 09:30:00', 0, NULL),
  (27, 12,  2, '2026-02-22 09:30:00', '2026-02-22 10:00:00', 0, NULL),
  (28, 13,  3, '2026-02-23 11:00:00', '2026-02-23 11:30:00', 0, NULL),
  (29, 14,  4, '2026-02-23 14:00:00', '2026-02-23 14:30:00', 0, NULL),
  (30, 15,  5, '2026-02-23 16:00:00', '2026-02-23 16:30:00', 0, NULL);

-- Ajustar AUTO_INCREMENT para futuras inserciones
ALTER TABLE ObrasSociales AUTO_INCREMENT = 6;
ALTER TABLE Especialidades AUTO_INCREMENT = 7;
ALTER TABLE Medicos AUTO_INCREMENT = 6;
ALTER TABLE Pacientes AUTO_INCREMENT = 16;
ALTER TABLE Turnos AUTO_INCREMENT = 31;

-- Fin del script
