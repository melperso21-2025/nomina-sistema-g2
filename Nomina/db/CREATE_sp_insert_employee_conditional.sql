-- ═══════════════════════════════════════════════════════════════════════════
-- SCRIPT: Crear sp_insert_employee_conditional (VERSIÓN CORREGIDA)
-- Descripción: Crea el procedimiento para insertar empleados con o sin usuario
--              IMPORTANTE: Garantiza is_active = 1
-- ═══════════════════════════════════════════════════════════════════════════

-- Primero, elimina si existe
IF EXISTS (SELECT * FROM INFORMATION_SCHEMA.ROUTINES WHERE ROUTINE_NAME = 'sp_insert_employee_conditional')
    DROP PROCEDURE sp_insert_employee_conditional;
GO

-- Crear el procedimiento CORRECTO
CREATE PROCEDURE sp_insert_employee_conditional
    @p_emp_no            INT,
    @p_ci                VARCHAR(50),
    @p_first_name        VARCHAR(50),
    @p_last_name         VARCHAR(50),
    @p_birth_date        DATE,
    @p_gender            CHAR(1),
    @p_hire_date         DATE,
    @p_email             VARCHAR(100),
    @p_requires_access   BIT,
    @p_password_hash     VARBINARY(MAX),
    @p_role              VARCHAR(50),
    @p_dept_no           INT,
    @p_salary            BIGINT,
    @p_title             VARCHAR(50),
    @p_user_session      VARCHAR(100),
    @r_message           VARCHAR(200) OUTPUT
AS
BEGIN
    SET NOCOUNT ON;
    BEGIN TRANSACTION;
    BEGIN TRY

        -- Validar CI único
        IF EXISTS (SELECT 1 FROM employees WHERE ci = @p_ci)
        BEGIN
            SET @r_message = 'ERROR: CI already registered';
            ROLLBACK TRANSACTION;
            RETURN;
        END

        -- Validar email único
        IF EXISTS (SELECT 1 FROM employees WHERE email = @p_email)
        BEGIN
            SET @r_message = 'ERROR: Email already registered';
            ROLLBACK TRANSACTION;
            RETURN;
        END

        -- IMPORTANTE: Insertar empleado con is_active = 1
        INSERT INTO employees (emp_no, ci, first_name, last_name, birth_date, gender, hire_date, email, is_active)
        VALUES (@p_emp_no, @p_ci, @p_first_name, @p_last_name, @p_birth_date, @p_gender, @p_hire_date, @p_email, 1);

        -- OPCIONALMENTE insertar usuario si requiere acceso al sistema
        IF @p_requires_access = 1
        BEGIN
            -- Generar username con detección de colisiones: nombre.apellido
            DECLARE @username VARCHAR(100) = LOWER(@p_first_name + '.' + @p_last_name);
            DECLARE @baseUsername VARCHAR(100) = @username;
            DECLARE @counter INT = 2;

            -- Si el username existe, agregar sufijo numérico
            WHILE EXISTS (SELECT 1 FROM users WHERE username = @username)
            BEGIN
                SET @username = @baseUsername + CAST(@counter AS VARCHAR(10));
                SET @counter = @counter + 1;
            END;

            -- Insertar usuario con username único
            INSERT INTO users (emp_no, username, password_hash, role)
            VALUES (@p_emp_no, @username, @p_password_hash, @p_role);
        END

        -- Insertar salario inicial
        DECLARE @context VARBINARY(128) = CAST(@p_user_session AS VARBINARY(128));
        SET CONTEXT_INFO @context;

        INSERT INTO salaries (emp_no, salary, from_date, to_date)
        VALUES (@p_emp_no, @p_salary, @p_hire_date, NULL);

        -- Insertar titulo/cargo
        INSERT INTO titles (emp_no, title, from_date, to_date)
        VALUES (@p_emp_no, @p_title, @p_hire_date, NULL);

        -- Asignar departamento
        INSERT INTO dept_emp (emp_no, dept_no, from_date, to_date)
        VALUES (@p_emp_no, @p_dept_no, @p_hire_date, NULL);

        COMMIT TRANSACTION;
        
        IF @p_requires_access = 1
            SET @r_message = 'SUCCESS: Employee registered with system access';
        ELSE
            SET @r_message = 'SUCCESS: Employee registered without system access';

    END TRY
    BEGIN CATCH
        ROLLBACK TRANSACTION;
        SET @r_message = 'ERROR: ' + ERROR_MESSAGE();
    END CATCH
END
GO

PRINT '✓ Procedimiento sp_insert_employee_conditional creado correctamente.';
PRINT '✓ Garantiza is_active = 1 para nuevos empleados.';
