-- =============================================
-- 05_ACTIVITY_LOG.SQL
-- Sistema de Nomina G2
-- RF-13: Log de actividad básico
-- Ejecutar DESPUES de 03_triggers.sql
-- =============================================

USE nomina_db;
GO

-- ─────────────────────────────────────────────
-- TABLA: activity_log
-- Registra accesos y operaciones criticas
-- Poblada por RegistrarActividad() en controllers
-- ─────────────────────────────────────────────
CREATE TABLE activity_log (
    log_id       INT           IDENTITY(1,1) PRIMARY KEY,
    action_date  DATETIME      NOT NULL DEFAULT GETDATE(),
    user_session VARCHAR(100)  NOT NULL,
    module       VARCHAR(50)   NOT NULL,
    action       VARCHAR(50)   NOT NULL,
    description  VARCHAR(300)  NULL
);
GO

-- Indice para filtrar por modulo y fecha (consultas frecuentes)
CREATE INDEX idx_activity_log_module ON activity_log (module, action_date DESC);
GO
