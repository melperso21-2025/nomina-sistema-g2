-- ═══════════════════════════════════════════════════════════════════════════
-- SCRIPT: Debug sp_insert_employee_conditional
-- Descripción: Ejecuta el procedimiento con parámetros de prueba para ver el error
-- ═══════════════════════════════════════════════════════════════════════════

DECLARE @message VARCHAR(200);

-- Ejecutar el procedimiento con los datos del empleado que intentaste crear
EXEC sp_insert_employee_conditional
    @p_emp_no = 1019,
    @p_ci = '1111111111',  -- Reemplaza con el CI que usaste
    @p_first_name = 'Test',  -- Reemplaza con el nombre que usaste
    @p_last_name = 'Empleado',  -- Reemplaza con el apellido que usaste
    @p_birth_date = '1990-01-01',  -- Reemplaza con la fecha que usaste
    @p_gender = 'M',  -- Reemplaza con M o F
    @p_hire_date = '2024-01-01',  -- Reemplaza con la fecha que usaste
    @p_email = 'test@example.com',  -- Reemplaza con el email que usaste
    @p_requires_access = 0,  -- Sin usuario
    @p_password_hash = NULL,
    @p_role = '',
    @p_dept_no = 1,  -- Reemplaza con el ID del departamento
    @p_salary = 1000000,  -- Reemplaza con el salario
    @p_title = 'Junior',  -- Reemplaza con el cargo
    @p_user_session = 'admin',
    @r_message = @message OUTPUT;

-- Mostrar el mensaje de respuesta
PRINT 'Mensaje del procedimiento:';
PRINT @message;

-- Verificar si se creó el empleado
PRINT '';
PRINT '=================================================================';
PRINT 'Verificación después de ejecutar el procedimiento:';
PRINT '=================================================================';

IF EXISTS (SELECT 1 FROM employees WHERE emp_no = 1019)
    PRINT 'Empleado 1019 CREADO correctamente';
ELSE
    PRINT 'ERROR: Empleado 1019 NO se creó';
