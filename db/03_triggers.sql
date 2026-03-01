-- =============================================
-- 03_TRIGGERS.SQL
-- Sistema de Nomina G2
-- Ejecutar DESPUES de 02_store_procedures.sql
-- =============================================

USE nomina_db;
GO

-- ─────────────────────────────────────────────
-- TRIGGER: trg_salary_audit
-- Se activa automaticamente AFTER INSERT en salaries
-- Registra el cambio en salary_audit_log
-- El usuario se pasa via CONTEXT_INFO desde el SP
-- El controller C# NO necesita hacer nada extra
-- ─────────────────────────────────────────────
CREATE OR ALTER TRIGGER trg_salary_audit
ON salaries
AFTER INSERT
AS
BEGIN
    SET NOCOUNT ON;

    -- Leer el usuario de sesion desde CONTEXT_INFO
    DECLARE @user_session VARCHAR(100);

    SELECT @user_session = LTRIM(RTRIM(
        REPLACE(CAST(CONTEXT_INFO() AS VARCHAR(100)), CHAR(0), '')
    ));

    -- Si no hay contexto usar el usuario del sistema
    IF @user_session IS NULL OR @user_session = ''
        SET @user_session = SYSTEM_USER;

    -- Insertar en el log de auditoria
    INSERT INTO salary_audit_log (
        emp_no,
        previous_salary,
        new_salary,
        from_date,
        user_session,
        action_date
    )
    SELECT
        i.emp_no,
        -- Salario anterior: el ultimo registro cerrado antes de este
        (
            SELECT TOP 1 s.salary
            FROM salaries s
            WHERE s.emp_no = i.emp_no
              AND s.from_date < i.from_date
            ORDER BY s.from_date DESC
        ),
        i.salary,
        i.from_date,
        @user_session,
        GETDATE()
    FROM inserted i;
END
GO

-- ─────────────────────────────────────────────
-- TRIGGER: trg_validate_min_salary
-- Valida que el salario no sea menor al minimo
-- Se activa AFTER INSERT en salaries
-- Hace ROLLBACK si el salario es menor a 470
-- ─────────────────────────────────────────────
CREATE OR ALTER TRIGGER trg_validate_min_salary
ON salaries
AFTER INSERT
AS
BEGIN
    SET NOCOUNT ON;

    IF EXISTS (SELECT 1 FROM inserted WHERE salary < 470)
    BEGIN
        RAISERROR('ERROR: Salary cannot be less than 470', 16, 1);
        ROLLBACK TRANSACTION;
    END
END
GO