-- MySQL dump for TurneroMedico (compatible with MySQL Workbench)
-- Run in MySQL Workbench or with: mysql -u <user> -p < dump_turnos_medicos.sql

SET FOREIGN_KEY_CHECKS = 0;

CREATE DATABASE IF NOT EXISTS `turnosmedicos` DEFAULT CHARACTER SET utf8mb4 COLLATE utf8mb4_general_ci;
USE `turnosmedicos`;

-- Drop tables if exist (in dependency order)
DROP TABLE IF EXISTS `MedicoEspecialidad`;
DROP TABLE IF EXISTS `DisponibilidadMedico`;
DROP TABLE IF EXISTS `Turnos`;
DROP TABLE IF EXISTS `Pacientes`;
DROP TABLE IF EXISTS `Medicos`;
DROP TABLE IF EXISTS `Especialidades`;
DROP TABLE IF EXISTS `ObrasSociales`;
DROP TABLE IF EXISTS `Users`;

-- ObrasSociales
CREATE TABLE `ObrasSociales` (
  `Id` INT NOT NULL AUTO_INCREMENT,
  `Nombre` VARCHAR(20) NOT NULL,
  `RowVersion` VARBINARY(8) NULL,
  PRIMARY KEY (`Id`),
  UNIQUE KEY `UX_ObrasSociales_Nombre` (`Nombre`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4;

-- Especialidades
CREATE TABLE `Especialidades` (
  `Id` INT NOT NULL AUTO_INCREMENT,
  `NombreEspecialidad` VARCHAR(255) NOT NULL,
  `RowVersion` VARBINARY(8) NULL,
  PRIMARY KEY (`Id`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4;

-- Medicos
CREATE TABLE `Medicos` (
  `Id` INT NOT NULL AUTO_INCREMENT,
  `Nombre` VARCHAR(100) NOT NULL,
  `DNI` VARCHAR(10) NOT NULL,
  `Telefono` VARCHAR(20) NOT NULL,
  `Matricula` VARCHAR(25) NOT NULL,
  `DuracionTurnoMin` INT NOT NULL,
  `RowVersion` VARBINARY(8) NULL,
  PRIMARY KEY (`Id`),
  UNIQUE KEY `UX_Medicos_Matricula` (`Matricula`),
  UNIQUE KEY `UX_Medicos_DNI` (`DNI`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4;

-- MedicoEspecialidad (join table)
CREATE TABLE `MedicoEspecialidad` (
  `MedicoId` INT NOT NULL,
  `EspecialidadId` INT NOT NULL,
  PRIMARY KEY (`MedicoId`, `EspecialidadId`),
  KEY `IX_MedicoEspecialidad_EspecialidadId` (`EspecialidadId`),
  CONSTRAINT `FK_MedicoEspecialidad_Medicos` FOREIGN KEY (`MedicoId`) REFERENCES `Medicos` (`Id`) ON DELETE CASCADE,
  CONSTRAINT `FK_MedicoEspecialidad_Especialidades` FOREIGN KEY (`EspecialidadId`) REFERENCES `Especialidades` (`Id`) ON DELETE CASCADE
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4;

-- DisponibilidadMedico
CREATE TABLE `DisponibilidadMedico` (
  `Id` INT NOT NULL AUTO_INCREMENT,
  `MedicoId` INT NOT NULL,
  `DiaSemana` INT NOT NULL,
  `HoraDesde` TIME NOT NULL,
  `HoraHasta` TIME NOT NULL,
  `RowVersion` VARBINARY(8) NULL,
  PRIMARY KEY (`Id`),
  UNIQUE KEY `UX_DisponibilidadMedico_Unique` (`MedicoId`,`DiaSemana`,`HoraDesde`,`HoraHasta`),
  CONSTRAINT `FK_DisponibilidadMedico_Medicos` FOREIGN KEY (`MedicoId`) REFERENCES `Medicos` (`Id`) ON DELETE CASCADE
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4;

-- Pacientes
CREATE TABLE `Pacientes` (
  `Id` INT NOT NULL AUTO_INCREMENT,
  `Nombre` VARCHAR(100) NOT NULL,
  `DNI` VARCHAR(10) NOT NULL,
  `Telefono` VARCHAR(20) NOT NULL,
  `TieneObraSocial` TINYINT(1) NOT NULL,
  `IdObraSocial` INT NULL,
  `RowVersion` VARBINARY(8) NULL,
  PRIMARY KEY (`Id`),
  UNIQUE KEY `UX_Pacientes_DNI` (`DNI`),
  CONSTRAINT `FK_Pacientes_ObrasSociales` FOREIGN KEY (`IdObraSocial`) REFERENCES `ObrasSociales` (`Id`) ON DELETE SET NULL
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4;

-- Turnos
CREATE TABLE `Turnos` (
  `Id` INT NOT NULL AUTO_INCREMENT,
  `IdPaciente` INT NULL,
  `IdMedico` INT NULL,
  `Inicio` DATETIME NOT NULL,
  `Fin` DATETIME NOT NULL,
  `Estado` INT NOT NULL,
  `Observaciones` VARCHAR(200) NULL,
  `RowVersion` VARBINARY(8) NULL,
  PRIMARY KEY (`Id`),
  UNIQUE KEY `UX_Turnos_Medico_Inicio` (`IdMedico`,`Inicio`),
  KEY `IX_Turnos_IdMedico_Inicio_Fin` (`IdMedico`,`Inicio`,`Fin`),
  CONSTRAINT `FK_Turnos_Pacientes` FOREIGN KEY (`IdPaciente`) REFERENCES `Pacientes` (`Id`) ON DELETE SET NULL,
  CONSTRAINT `FK_Turnos_Medicos` FOREIGN KEY (`IdMedico`) REFERENCES `Medicos` (`Id`) ON DELETE SET NULL
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4;

-- Users (auth)
CREATE TABLE `Users` (
  `Id` INT NOT NULL AUTO_INCREMENT,
  `Username` VARCHAR(100) NOT NULL,
  `PasswordHash` VARCHAR(200) NOT NULL,
  `Role` INT NOT NULL,
  PRIMARY KEY (`Id`),
  UNIQUE KEY `UX_Users_Username` (`Username`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4;

SET FOREIGN_KEY_CHECKS = 1;

-- End of dump
