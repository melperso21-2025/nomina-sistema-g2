-- ═══════════════════════════════════════════════════════════════════════════
-- SCRIPT: Limpiar datos de prueba y resetear secuencia
-- Descripción: Elimina empleados 9999, 8888 y deja la BD lista para continuar
-- ═══════════════════════════════════════════════════════════════════════════

PRINT '=================================================================';
PRINT 'LIMPIANDO DATOS DE PRUEBA';
PRINT '=================================================================';

-- Limpiar empleado 9999
PRINT 'Eliminando empleado 9999...';
DELETE FROM salary_audit_log WHERE emp_no = 9999;
DELETE FROM dept_emp WHERE emp_no = 9999;
DELETE FROM dept_manager WHERE emp_no = 9999;
DELETE FROM titles WHERE emp_no = 9999;
DELETE FROM salaries WHERE emp_no = 9999;
DELETE FROM users WHERE emp_no = 9999;
DELETE FROM employees WHERE emp_no = 9999;

-- Limpiar empleado 8888
PRINT 'Eliminando empleado 8888...';
DELETE FROM salary_audit_log WHERE emp_no = 8888;
DELETE FROM users WHERE emp_no = 8888;
DELETE FROM dept_emp WHERE emp_no = 8888;
DELETE FROM dept_manager WHERE emp_no = 8888;
DELETE FROM titles WHERE emp_no = 8888;
DELETE FROM salaries WHERE emp_no = 8888;
DELETE FROM employees WHERE emp_no = 8888;

PRINT '';
PRINT '=================================================================';
PRINT 'VERIFICACIÓN POST-LIMPIEZA';
PRINT '=================================================================';

-- Verificar que fueron eliminados
IF NOT EXISTS (SELECT 1 FROM employees WHERE emp_no IN (9999, 8888))
    PRINT 'Empleados de prueba eliminados correctamente';
ELSE
    PRINT 'ERROR: Aún hay registros de prueba';

PRINT '';
PRINT '=================================================================';
PRINT 'ESTADO ACTUAL DE LA SECUENCIA';
PRINT '=================================================================';

DECLARE @maxEmpNo INT = (SELECT ISNULL(MAX(emp_no), 0) FROM employees);
PRINT 'Máximo emp_no actual: ' + CAST(@maxEmpNo AS VARCHAR(10));
PRINT 'Próximo emp_no será: ' + CAST(@maxEmpNo + 1 AS VARCHAR(10));

PRINT '';
PRINT '=================================================================';
PRINT 'LIMPIEZA COMPLETADA';
PRINT '=================================================================';
PRINT 'La BD está lista para continuar creando empleados normalmente.';
