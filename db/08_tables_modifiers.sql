-- =============================================
-- CREAR id_salary
-- =============================================
USE [nomina_db]
GO

-- 1. Verificar que la columna no exista
IF NOT EXISTS (
    SELECT 1 FROM INFORMATION_SCHEMA.COLUMNS 
    WHERE TABLE_NAME = 'salaries' AND COLUMN_NAME = 'id_salary'
)
BEGIN
    -- 2. Agregar la columna con IDENTITY
    ALTER TABLE [dbo].[salaries]
    ADD [id_salary] INT IDENTITY(1,1) NOT NULL
    
    -- 3. Hacer que la nueva columna sea única (si es necesario)
    ALTER TABLE [dbo].[salaries]
    ADD CONSTRAINT UQ_id_salary UNIQUE ([id_salary])
    
    PRINT 'Columna id_salary agregada exitosamente'
END
ELSE
BEGIN
    PRINT 'La columna id_salary ya existe'
END
GO


----------------------------------------------
-- actualizar sp_update_employee
-----------------------------------------------
USE [nomina_db]
GO

-- Eliminar si existe
IF OBJECT_ID('sp_update_employee', 'P') IS NOT NULL
  DROP PROCEDURE sp_update_employee
GO

-- Crear correcto (SIN concatenación)
CREATE PROCEDURE sp_update_employee
    @p_emp_no INT,
    @p_first_name VARCHAR(50),
    @p_last_name VARCHAR(50),
    @p_birth_date DATE,
    @p_gender CHAR(1),
    @p_email VARCHAR(100),
    @r_message VARCHAR(200) OUTPUT
AS
BEGIN
    BEGIN TRY
        UPDATE employees
        SET 
            first_name = @p_first_name,
            last_name = @p_last_name,
            birth_date = @p_birth_date,
            gender = @p_gender,
            email = @p_email
        WHERE emp_no = @p_emp_no

        IF @@ROWCOUNT > 0
            SET @r_message = 'SUCCESS: Employee updated successfully'
        ELSE
            SET @r_message = 'ERROR: Employee not found'
    END TRY
    BEGIN CATCH
        SET @r_message = 'ERROR: ' + ERROR_MESSAGE()
    END CATCH
END
GO


-----------------------------------------------
-- actualizar sp_get_employee_detail
-----------------------------------------------

USE [nomina_db]
GO

IF EXISTS (SELECT 1 FROM INFORMATION_SCHEMA.ROUTINES WHERE ROUTINE_NAME = 'sp_get_employee_detail')
  DROP PROCEDURE sp_get_employee_detail
GO

CREATE PROCEDURE sp_get_employee_detail
    @p_emp_no INT
AS
BEGIN
    SELECT 
        e.emp_no,                                    -- 0
        e.ci,                                         -- 1
        e.first_name + ' ' + e.last_name,           -- 2 (full_name)
        e.first_name,                                 -- 3 (FIRST, no LAST)
        e.last_name,                                  -- 4 (LAST, no FIRST)
        e.birth_date,                                 -- 5
        e.gender,                                     -- 6
        e.hire_date,                                  -- 7
        e.email,                                      -- 8
        e.is_active,                                  -- 9
        ISNULL(d.dept_name, 'Sin departamento'),     -- 10
        d.dept_no,                                    -- 11
        t.title,                                      -- 12
        s.salary                                      -- 13
    FROM employees e
    LEFT JOIN dept_emp de ON e.emp_no = de.emp_no AND de.to_date IS NULL
    LEFT JOIN departments d ON de.dept_no = d.dept_no
    LEFT JOIN titles t ON e.emp_no = t.emp_no AND t.to_date IS NULL
    LEFT JOIN salaries s ON e.emp_no = s.emp_no AND s.to_date IS NULL
    WHERE e.emp_no = @p_emp_no
END
GO


