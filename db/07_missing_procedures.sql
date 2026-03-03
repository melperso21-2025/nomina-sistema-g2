-- =============================================
-- 07_MISSING_PROCEDURES.SQL
-- Sistema de Nomina G2
-- Procedimientos almacenados faltantes
-- Ejecutar en SSMS sobre nomina_db
-- =============================================

USE nomina_db;
GO

-- ─────────────────────────────────────────────
-- SP: sp_insert_department
-- Inserta un nuevo departamento
-- ─────────────────────────────────────────────
CREATE PROCEDURE sp_insert_department
    @p_dept_no   VARCHAR(50),
    @p_dept_name VARCHAR(50),
    @r_message   VARCHAR(200) OUTPUT
AS
BEGIN
    SET NOCOUNT ON;

    IF EXISTS (SELECT 1 FROM departments WHERE dept_no = @p_dept_no)
    BEGIN
        SET @r_message = 'ERROR: Ya existe un departamento con ese numero.';
        RETURN;
    END

    IF EXISTS (SELECT 1 FROM departments WHERE dept_name = @p_dept_name)
    BEGIN
        SET @r_message = 'ERROR: Ya existe un departamento con ese nombre.';
        RETURN;
    END

    INSERT INTO departments (dept_no, dept_name, is_active)
    VALUES (@p_dept_no, @p_dept_name, 1);

    SET @r_message = 'SUCCESS: Departamento creado correctamente.';
END
GO

-- ─────────────────────────────────────────────
-- SP: sp_update_department
-- Actualiza el nombre de un departamento existente
-- ─────────────────────────────────────────────
CREATE PROCEDURE sp_update_department
    @p_dept_no   VARCHAR(50),
    @p_dept_name VARCHAR(50),
    @r_message   VARCHAR(200) OUTPUT
AS
BEGIN
    SET NOCOUNT ON;

    IF NOT EXISTS (SELECT 1 FROM departments WHERE dept_no = @p_dept_no)
    BEGIN
        SET @r_message = 'ERROR: Departamento no encontrado.';
        RETURN;
    END

    IF EXISTS (SELECT 1 FROM departments WHERE dept_name = @p_dept_name AND dept_no <> @p_dept_no)
    BEGIN
        SET @r_message = 'ERROR: Ya existe otro departamento con ese nombre.';
        RETURN;
    END

    UPDATE departments
    SET dept_name = @p_dept_name
    WHERE dept_no = @p_dept_no;

    SET @r_message = 'SUCCESS: Departamento actualizado correctamente.';
END
GO

-- ─────────────────────────────────────────────
-- SP: sp_insert_salary
-- Registra un nuevo salario para un empleado
-- Activa el trigger trg_salary_audit automaticamente
-- ─────────────────────────────────────────────
CREATE PROCEDURE sp_insert_salary
    @p_emp_no       INT,
    @p_salary       BIGINT,
    @p_from_date    DATE,
    @p_to_date      DATE         = NULL,
    @p_user_session VARCHAR(100),
    @r_message      VARCHAR(200) OUTPUT
AS
BEGIN
    SET NOCOUNT ON;
    BEGIN TRANSACTION;
    BEGIN TRY

        IF NOT EXISTS (SELECT 1 FROM employees WHERE emp_no = @p_emp_no AND is_active = 1)
        BEGIN
            SET @r_message = 'ERROR: Empleado no encontrado o inactivo.';
            ROLLBACK TRANSACTION;
            RETURN;
        END

        IF EXISTS (
            SELECT 1 FROM salaries
            WHERE emp_no = @p_emp_no
              AND to_date IS NULL
              AND from_date >= @p_from_date
        )
        BEGIN
            SET @r_message = 'ERROR: La fecha se solapa con un salario vigente existente.';
            ROLLBACK TRANSACTION;
            RETURN;
        END

        -- Cerrar salario vigente si existe
        UPDATE salaries
        SET to_date = @p_from_date
        WHERE emp_no = @p_emp_no AND to_date IS NULL;

        -- Pasar usuario al contexto para el trigger de auditoria
        DECLARE @context VARBINARY(128) = CAST(@p_user_session AS VARBINARY(128));
        SET CONTEXT_INFO @context;

        INSERT INTO salaries (emp_no, salary, from_date, to_date)
        VALUES (@p_emp_no, @p_salary, @p_from_date, @p_to_date);

        COMMIT TRANSACTION;
        SET @r_message = 'SUCCESS: Salario registrado correctamente.';

    END TRY
    BEGIN CATCH
        ROLLBACK TRANSACTION;
        SET @r_message = 'ERROR: ' + ERROR_MESSAGE();
    END CATCH
END
GO
