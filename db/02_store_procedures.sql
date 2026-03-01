
-- =============================================
-- 02_STORE_PROCEDURES.SQL
-- Sistema de Nomina G2
-- Ejecutar DESPUES de 01_tablas.sql
-- =============================================

USE nomina_db;
GO

-- ─────────────────────────────────────────────
-- SP: sp_login
-- Valida credenciales del usuario
-- El hash SHA256 lo aplica el controller C# antes de llamar este SP
-- ─────────────────────────────────────────────
CREATE PROCEDURE sp_login
    @p_username      VARCHAR(100),
    @p_password_hash VARBINARY(MAX),
    @r_result        INT          OUTPUT,
    @r_role          VARCHAR(50)  OUTPUT,
    @r_emp_no        INT          OUTPUT,
    @r_full_name     VARCHAR(100) OUTPUT
AS
BEGIN
    SET NOCOUNT ON;

    IF EXISTS (
        SELECT 1 FROM users
        WHERE username = @p_username
          AND password_hash = @p_password_hash
    )
    BEGIN
        SELECT
            @r_result    = 1,
            @r_role      = u.role,
            @r_emp_no    = u.emp_no,
            @r_full_name = e.first_name + ' ' + e.last_name
        FROM users u
        INNER JOIN employees e ON u.emp_no = e.emp_no
        WHERE u.username = @p_username;
    END
    ELSE
    BEGIN
        SET @r_result    = 0;
        SET @r_role      = NULL;
        SET @r_emp_no    = NULL;
        SET @r_full_name = NULL;
    END
END
GO

-- ─────────────────────────────────────────────
-- SP: sp_list_employees
-- Lista empleados con filtros y paginacion
-- @p_filter_name: busca en nombre completo
-- @p_filter_ci: busca por cedula exacta
-- @p_dept_no: filtra por departamento (NULL = todos)
-- ─────────────────────────────────────────────
CREATE PROCEDURE sp_list_employees
    @p_filter_name  VARCHAR(100) = NULL,
    @p_filter_ci    VARCHAR(50)  = NULL,
    @p_dept_no      INT          = NULL,
    @p_page         INT          = 1,
    @p_page_size    INT          = 20,
    @r_total        INT          OUTPUT
AS
BEGIN
    SET NOCOUNT ON;

    -- Total para paginacion
    SELECT @r_total = COUNT(*)
    FROM employees e
    LEFT JOIN dept_emp de ON e.emp_no = de.emp_no AND de.to_date IS NULL
    WHERE e.is_active = 1
      AND (e.first_name + ' ' + e.last_name LIKE '%' + ISNULL(@p_filter_name, '') + '%')
      AND (e.ci = @p_filter_ci OR @p_filter_ci IS NULL)
      AND (de.dept_no = @p_dept_no OR @p_dept_no IS NULL);

    -- Resultado paginado
    SELECT
        e.emp_no,
        e.ci,
        e.first_name + ' ' + e.last_name AS full_name,
        e.email,
        e.hire_date,
        e.gender,
        d.dept_name
    FROM employees e
    LEFT JOIN dept_emp de ON e.emp_no = de.emp_no AND de.to_date IS NULL
    LEFT JOIN departments d ON de.dept_no = d.dept_no
    WHERE e.is_active = 1
      AND (e.first_name + ' ' + e.last_name LIKE '%' + ISNULL(@p_filter_name, '') + '%')
      AND (e.ci = @p_filter_ci OR @p_filter_ci IS NULL)
      AND (de.dept_no = @p_dept_no OR @p_dept_no IS NULL)
    ORDER BY e.emp_no
    OFFSET (@p_page - 1) * @p_page_size ROWS
    FETCH NEXT @p_page_size ROWS ONLY;
END
GO

-- ─────────────────────────────────────────────
-- SP: sp_get_employee_detail
-- Devuelve info completa de un empleado por emp_no
-- ─────────────────────────────────────────────
CREATE PROCEDURE sp_get_employee_detail
    @p_emp_no INT
AS
BEGIN
    SET NOCOUNT ON;

    SELECT
        e.emp_no,
        e.ci,
        e.first_name,
        e.last_name,
        e.first_name + ' ' + e.last_name AS full_name,
        e.birth_date,
        e.gender,
        e.hire_date,
        e.email,
        e.is_active,
        d.dept_name,
        de.dept_no,
        t.title,
        s.salary
    FROM employees e
    LEFT JOIN dept_emp de ON e.emp_no = de.emp_no AND de.to_date IS NULL
    LEFT JOIN departments d ON de.dept_no = d.dept_no
    LEFT JOIN titles t ON e.emp_no = t.emp_no AND t.to_date IS NULL
    LEFT JOIN salaries s ON e.emp_no = s.emp_no AND s.to_date IS NULL
    WHERE e.emp_no = @p_emp_no;
END
GO

-- ─────────────────────────────────────────────
-- SP: sp_insert_full_employee
-- Registra un empleado nuevo con usuario, salario y dept
-- Todo en una sola transaccion
-- ─────────────────────────────────────────────
CREATE PROCEDURE sp_insert_full_employee
    @p_emp_no         INT,
    @p_ci             VARCHAR(50),
    @p_first_name     VARCHAR(50),
    @p_last_name      VARCHAR(50),
    @p_birth_date     DATE,
    @p_gender         CHAR(1),
    @p_hire_date      DATE,
    @p_email          VARCHAR(100),
    @p_password_hash  VARBINARY(MAX),
    @p_role           VARCHAR(50),
    @p_dept_no        INT,
    @p_salary         BIGINT,
    @p_title          VARCHAR(50),
    @p_user_session   VARCHAR(100),
    @r_message        VARCHAR(200) OUTPUT
AS
BEGIN
    SET NOCOUNT ON;
    BEGIN TRANSACTION;
    BEGIN TRY

        -- Validar CI unico
        IF EXISTS (SELECT 1 FROM employees WHERE ci = @p_ci)
        BEGIN
            SET @r_message = 'ERROR: CI already registered';
            ROLLBACK TRANSACTION;
            RETURN;
        END

        -- Validar email unico
        IF EXISTS (SELECT 1 FROM employees WHERE email = @p_email)
        BEGIN
            SET @r_message = 'ERROR: Email already registered';
            ROLLBACK TRANSACTION;
            RETURN;
        END

        -- Insertar empleado
        INSERT INTO employees (emp_no, ci, first_name, last_name, birth_date, gender, hire_date, email)
        VALUES (@p_emp_no, @p_ci, @p_first_name, @p_last_name, @p_birth_date, @p_gender, @p_hire_date, @p_email);

        -- Insertar usuario
        INSERT INTO users (emp_no, username, password_hash, role)
        VALUES (@p_emp_no, @p_ci, @p_password_hash, @p_role);

        -- Insertar salario inicial (activa el trigger de auditoria)
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
        SET @r_message = 'SUCCESS: Employee registered';

    END TRY
    BEGIN CATCH
        ROLLBACK TRANSACTION;
        SET @r_message = 'ERROR: ' + ERROR_MESSAGE();
    END CATCH
END
GO

-- ─────────────────────────────────────────────
-- SP: sp_update_employee
-- Edita datos personales de un empleado existente
-- ─────────────────────────────────────────────
CREATE PROCEDURE sp_update_employee
    @p_emp_no     INT,
    @p_first_name VARCHAR(50),
    @p_last_name  VARCHAR(50),
    @p_birth_date DATE,
    @p_gender     CHAR(1),
    @p_email      VARCHAR(100),
    @r_message    VARCHAR(200) OUTPUT
AS
BEGIN
    SET NOCOUNT ON;

    -- Validar email unico (excluyendo el mismo empleado)
    IF EXISTS (SELECT 1 FROM employees WHERE email = @p_email AND emp_no <> @p_emp_no)
    BEGIN
        SET @r_message = 'ERROR: Email already registered';
        RETURN;
    END

    UPDATE employees
    SET first_name = @p_first_name,
        last_name  = @p_last_name,
        birth_date = @p_birth_date,
        gender     = @p_gender,
        email      = @p_email
    WHERE emp_no = @p_emp_no;

    SET @r_message = 'SUCCESS: Employee updated';
END
GO

-- ─────────────────────────────────────────────
-- SP: sp_deactivate_employee
-- Baja logica — no borra fisicamente
-- ─────────────────────────────────────────────
CREATE PROCEDURE sp_deactivate_employee
    @p_emp_no  INT,
    @r_message VARCHAR(200) OUTPUT
AS
BEGIN
    SET NOCOUNT ON;

    IF NOT EXISTS (SELECT 1 FROM employees WHERE emp_no = @p_emp_no)
    BEGIN
        SET @r_message = 'ERROR: Employee not found';
        RETURN;
    END

    UPDATE employees SET is_active = 0 WHERE emp_no = @p_emp_no;
    SET @r_message = 'SUCCESS: Employee deactivated';
END
GO

-- ─────────────────────────────────────────────
-- SP: sp_update_salary
-- Cierra salario vigente e inserta el nuevo
-- El trigger trg_salary_audit se activa automaticamente
-- ─────────────────────────────────────────────
CREATE PROCEDURE sp_update_salary
    @p_emp_no       INT,
    @p_salary       BIGINT,
    @p_from_date    DATE,
    @p_user_session VARCHAR(100),
    @r_message      VARCHAR(200) OUTPUT,
    @r_prev_salary  BIGINT       OUTPUT
AS
BEGIN
    SET NOCOUNT ON;
    BEGIN TRANSACTION;
    BEGIN TRY

        -- Validar empleado
        IF NOT EXISTS (SELECT 1 FROM employees WHERE emp_no = @p_emp_no AND is_active = 1)
        BEGIN
            SET @r_message = 'ERROR: Employee not found';
            ROLLBACK TRANSACTION;
            RETURN;
        END

        -- Validar que from_date no se solape
        IF EXISTS (
            SELECT 1 FROM salaries
            WHERE emp_no = @p_emp_no
              AND to_date IS NULL
              AND from_date >= @p_from_date
        )
        BEGIN
            SET @r_message = 'ERROR: Date overlaps existing salary record';
            ROLLBACK TRANSACTION;
            RETURN;
        END

        -- Obtener salario anterior
        SELECT @r_prev_salary = salary
        FROM salaries
        WHERE emp_no = @p_emp_no AND to_date IS NULL;

        -- Cerrar salario vigente
        UPDATE salaries
        SET to_date = @p_from_date
        WHERE emp_no = @p_emp_no AND to_date IS NULL;

        -- Pasar usuario al contexto para el trigger
        DECLARE @context VARBINARY(128) = CAST(@p_user_session AS VARBINARY(128));
        SET CONTEXT_INFO @context;

        -- Insertar nuevo salario (dispara el trigger)
        INSERT INTO salaries (emp_no, salary, from_date, to_date)
        VALUES (@p_emp_no, @p_salary, @p_from_date, NULL);

        COMMIT TRANSACTION;
        SET @r_message = 'SUCCESS: Salary updated';

    END TRY
    BEGIN CATCH
        ROLLBACK TRANSACTION;
        SET @r_message = 'ERROR: ' + ERROR_MESSAGE();
    END CATCH
END
GO

-- ─────────────────────────────────────────────
-- SP: sp_assign_department
-- Asigna empleado a un departamento con vigencia
-- Cierra asignacion anterior automaticamente
-- ─────────────────────────────────────────────
CREATE PROCEDURE sp_assign_department
    @p_emp_no    INT,
    @p_dept_no   INT,
    @p_from_date DATE,
    @p_to_date   DATE = NULL,
    @r_message   VARCHAR(200) OUTPUT
AS
BEGIN
    SET NOCOUNT ON;
    BEGIN TRANSACTION;
    BEGIN TRY

        -- Validar solapamiento
        IF EXISTS (
            SELECT 1 FROM dept_emp
            WHERE emp_no = @p_emp_no
              AND to_date IS NULL
              AND from_date >= @p_from_date
        )
        BEGIN
            SET @r_message = 'ERROR: Date overlaps existing department assignment';
            ROLLBACK TRANSACTION;
            RETURN;
        END

        -- Cerrar asignacion vigente
        UPDATE dept_emp
        SET to_date = @p_from_date
        WHERE emp_no = @p_emp_no AND to_date IS NULL;

        -- Nueva asignacion
        INSERT INTO dept_emp (emp_no, dept_no, from_date, to_date)
        VALUES (@p_emp_no, @p_dept_no, @p_from_date, @p_to_date);

        COMMIT TRANSACTION;
        SET @r_message = 'SUCCESS: Department assigned';

    END TRY
    BEGIN CATCH
        ROLLBACK TRANSACTION;
        SET @r_message = 'ERROR: ' + ERROR_MESSAGE();
    END CATCH
END
GO

-- ─────────────────────────────────────────────
-- SP: sp_assign_manager
-- Asigna manager a un departamento
-- Valida que solo haya un manager activo por dept
-- ─────────────────────────────────────────────
CREATE PROCEDURE sp_assign_manager
    @p_emp_no    INT,
    @p_dept_no   INT,
    @p_from_date DATE,
    @p_to_date   DATE = NULL,
    @r_message   VARCHAR(200) OUTPUT
AS
BEGIN
    SET NOCOUNT ON;
    BEGIN TRANSACTION;
    BEGIN TRY

        -- Validar un solo manager activo por departamento
        IF EXISTS (
            SELECT 1 FROM dept_manager
            WHERE dept_no = @p_dept_no AND to_date IS NULL
        )
        BEGIN
            -- Cerrar el manager vigente
            UPDATE dept_manager
            SET to_date = @p_from_date
            WHERE dept_no = @p_dept_no AND to_date IS NULL;
        END

        -- Insertar nuevo manager
        INSERT INTO dept_manager (emp_no, dept_no, from_date, to_date)
        VALUES (@p_emp_no, @p_dept_no, @p_from_date, @p_to_date);

        COMMIT TRANSACTION;
        SET @r_message = 'SUCCESS: Manager assigned';

    END TRY
    BEGIN CATCH
        ROLLBACK TRANSACTION;
        SET @r_message = 'ERROR: ' + ERROR_MESSAGE();
    END CATCH
END
GO

-- ─────────────────────────────────────────────
-- SP: sp_register_title
-- Registra un nuevo cargo/titulo para un empleado
-- Valida solapamiento de fechas
-- ─────────────────────────────────────────────
CREATE PROCEDURE sp_register_title
    @p_emp_no    INT,
    @p_title     VARCHAR(50),
    @p_from_date DATE,
    @p_to_date   DATE = NULL,
    @r_message   VARCHAR(200) OUTPUT
AS
BEGIN
    SET NOCOUNT ON;
    BEGIN TRANSACTION;
    BEGIN TRY

        -- Validar solapamiento
        IF EXISTS (
            SELECT 1 FROM titles
            WHERE emp_no = @p_emp_no
              AND to_date IS NULL
              AND from_date >= @p_from_date
        )
        BEGIN
            SET @r_message = 'ERROR: Date overlaps existing title record';
            ROLLBACK TRANSACTION;
            RETURN;
        END

        -- Cerrar titulo vigente
        UPDATE titles
        SET to_date = @p_from_date
        WHERE emp_no = @p_emp_no AND to_date IS NULL;

        -- Nuevo titulo
        INSERT INTO titles (emp_no, title, from_date, to_date)
        VALUES (@p_emp_no, @p_title, @p_from_date, @p_to_date);

        COMMIT TRANSACTION;
        SET @r_message = 'SUCCESS: Title registered';

    END TRY
    BEGIN CATCH
        ROLLBACK TRANSACTION;
        SET @r_message = 'ERROR: ' + ERROR_MESSAGE();
    END CATCH
END
GO

-- ─────────────────────────────────────────────
-- SP: sp_report_active_payroll
-- Nomina vigente por departamento
-- ─────────────────────────────────────────────
CREATE PROCEDURE sp_report_active_payroll
    @p_dept_no INT = NULL
AS
BEGIN
    SET NOCOUNT ON;

    SELECT
        d.dept_name,
        e.emp_no,
        e.first_name + ' ' + e.last_name AS full_name,
        e.ci,
        t.title,
        s.salary,
        s.from_date AS salary_since
    FROM employees e
    INNER JOIN dept_emp de ON e.emp_no = de.emp_no AND de.to_date IS NULL
    INNER JOIN departments d ON de.dept_no = d.dept_no
    LEFT JOIN  titles t ON e.emp_no = t.emp_no AND t.to_date IS NULL
    INNER JOIN salaries s ON e.emp_no = s.emp_no AND s.to_date IS NULL
    WHERE e.is_active = 1
      AND (de.dept_no = @p_dept_no OR @p_dept_no IS NULL)
    ORDER BY d.dept_name, e.last_name;
END
GO

-- ─────────────────────────────────────────────
-- SP: sp_report_salary_changes
-- Cambios salariales en un rango de fechas
-- ─────────────────────────────────────────────
CREATE PROCEDURE sp_report_salary_changes
    @p_date_from DATE = NULL,
    @p_date_to   DATE = NULL,
    @p_ci        VARCHAR(50) = NULL
AS
BEGIN
    SET NOCOUNT ON;

    SELECT
        sal.log_id,
        sal.action_date,
        sal.user_session,
        e.first_name + ' ' + e.last_name AS full_name,
        e.ci,
        sal.previous_salary,
        sal.new_salary,
        sal.from_date
    FROM salary_audit_log sal
    INNER JOIN employees e ON sal.emp_no = e.emp_no
    WHERE (sal.action_date >= @p_date_from OR @p_date_from IS NULL)
      AND (sal.action_date <= @p_date_to   OR @p_date_to   IS NULL)
      AND (e.ci = @p_ci                    OR @p_ci        IS NULL)
    ORDER BY sal.action_date DESC;
END
GO

-- ─────────────────────────────────────────────
-- SP: sp_dashboard_stats
-- Estadisticas para el dashboard principal
-- ─────────────────────────────────────────────
CREATE PROCEDURE sp_dashboard_stats
AS
BEGIN
    SET NOCOUNT ON;

    -- Total empleados activos
    SELECT COUNT(*) AS total_employees
    FROM employees WHERE is_active = 1;

    -- Total departamentos activos
    SELECT COUNT(*) AS total_departments
    FROM departments WHERE is_active = 1;

    -- Salario promedio vigente
    SELECT ISNULL(AVG(salary), 0) AS avg_salary
    FROM salaries WHERE to_date IS NULL;

    -- Ultimos 5 cambios salariales
    SELECT TOP 5
        sal.action_date,
        e.first_name + ' ' + e.last_name AS full_name,
        sal.previous_salary,
        sal.new_salary,
        sal.user_session
    FROM salary_audit_log sal
    INNER JOIN employees e ON sal.emp_no = e.emp_no
    ORDER BY sal.action_date DESC;
END
GO
