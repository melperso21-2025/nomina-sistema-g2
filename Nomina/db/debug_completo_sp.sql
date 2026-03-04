-- ═══════════════════════════════════════════════════════════════════════════
-- SCRIPT: Debug completo - Verificar qué falla en sp_insert_employee_conditional
-- Descripción: Ejecuta y muestra exactamente dónde falla
-- ═══════════════════════════════════════════════════════════════════════════

-- Primero, verificar si el procedimiento existe
PRINT '=================================================================';
PRINT 'PASO 1: Verificar que el procedimiento existe';
PRINT '=================================================================';
IF EXISTS (SELECT * FROM INFORMATION_SCHEMA.ROUTINES WHERE ROUTINE_NAME = 'sp_insert_employee_conditional')
    PRINT 'Procedimiento EXISTE - Continuando...';
ELSE
BEGIN
    PRINT 'ERROR: Procedimiento NO EXISTE';
    PRINT 'Ejecuta primero: db/fix_insert_employee_conditional.sql';
    GOTO FIN;
END

PRINT '';
PRINT '=================================================================';
PRINT 'PASO 2: Ejecutar con datos de prueba (empleado SIN usuario)';
PRINT '=================================================================';

DECLARE @message VARCHAR(200);
DECLARE @today DATE = CAST(GETDATE() AS DATE);
DECLARE @hashSimulado VARBINARY(MAX) = CONVERT(VARBINARY(MAX), 'hash_simulado');

-- Limpiar si existe
DELETE FROM dept_emp WHERE emp_no = 9999;
DELETE FROM dept_manager WHERE emp_no = 9999;
DELETE FROM titles WHERE emp_no = 9999;
DELETE FROM salaries WHERE emp_no = 9999;
DELETE FROM users WHERE emp_no = 9999;
DELETE FROM employees WHERE emp_no = 9999;

-- Ejecutar procedimiento
BEGIN TRY
    EXEC sp_insert_employee_conditional
        @p_emp_no = 9999,
        @p_ci = '9999999999',
        @p_first_name = 'Test',
        @p_last_name = 'Prueba',
        @p_birth_date = '1990-01-01',
        @p_gender = 'M',
        @p_hire_date = @today,
        @p_email = 'test.prueba@example.com',
        @p_requires_access = 0,
        @p_password_hash = NULL,
        @p_role = '',
        @p_dept_no = 1,
        @p_salary = 1000000,
        @p_title = 'Developer',
        @p_user_session = 'admin',
        @r_message = @message OUTPUT;

    PRINT 'Resultado: ' + @message;

    IF EXISTS (SELECT 1 FROM employees WHERE emp_no = 9999)
    BEGIN
        PRINT 'EXITO: Empleado 9999 CREADO';
        SELECT 'EMPLEADO' AS Tipo, emp_no, first_name, last_name, is_active FROM employees WHERE emp_no = 9999;
        SELECT 'SALARIO' AS Tipo, emp_no, salary FROM salaries WHERE emp_no = 9999;
        SELECT 'CARGO' AS Tipo, emp_no, title FROM titles WHERE emp_no = 9999;
        SELECT 'DEPARTAMENTO' AS Tipo, emp_no, dept_no FROM dept_emp WHERE emp_no = 9999;
    END
    ELSE
        PRINT 'ERROR: Empleado NO se creó';

END TRY
BEGIN CATCH
    PRINT 'ERROR CAPTURADO:';
    PRINT ERROR_MESSAGE();
END CATCH

PRINT '';
PRINT '=================================================================';
PRINT 'PASO 3: Ejecutar con datos de prueba (empleado CON usuario)';
PRINT '=================================================================';

DELETE FROM users WHERE emp_no = 8888;
DELETE FROM dept_emp WHERE emp_no = 8888;
DELETE FROM titles WHERE emp_no = 8888;
DELETE FROM salaries WHERE emp_no = 8888;
DELETE FROM employees WHERE emp_no = 8888;

BEGIN TRY
    EXEC sp_insert_employee_conditional
        @p_emp_no = 8888,
        @p_ci = '8888888888',
        @p_first_name = 'Admin',
        @p_last_name = 'Test',
        @p_birth_date = '1985-05-15',
        @p_gender = 'F',
        @p_hire_date = @today,
        @p_email = 'admin.test@example.com',
        @p_requires_access = 1,
        @p_password_hash = @hashSimulado,
        @p_role = 'Admin',
        @p_dept_no = 1,
        @p_salary = 2000000,
        @p_title = 'Manager',
        @p_user_session = 'admin',
        @r_message = @message OUTPUT;

    PRINT 'Resultado: ' + @message;

    IF EXISTS (SELECT 1 FROM employees WHERE emp_no = 8888)
    BEGIN
        PRINT 'EXITO: Empleado 8888 CREADO';
        SELECT 'EMPLEADO' AS Tipo, emp_no, first_name, last_name FROM employees WHERE emp_no = 8888;
        SELECT 'USUARIO' AS Tipo, emp_no, username, role FROM users WHERE emp_no = 8888;
    END
    ELSE
        PRINT 'ERROR: Empleado NO se creó';

END TRY
BEGIN CATCH
    PRINT 'ERROR CAPTURADO:';
    PRINT ERROR_MESSAGE();
END CATCH

PRINT '';
PRINT '=================================================================';
PRINT 'ANALISIS FINAL';
PRINT '=================================================================';
IF EXISTS (SELECT 1 FROM employees WHERE emp_no = 9999)
    PRINT 'CONCLUSION: El procedimiento FUNCIONA correctamente';
ELSE
    PRINT 'CONCLUSION: El procedimiento tiene PROBLEMAS en la ejecucion';

FIN:
PRINT 'Verificacion completada.';
