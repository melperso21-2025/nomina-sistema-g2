-- ═══════════════════════════════════════════════════════════════════════════
-- SCRIPT: Cerrar salarios, cargos y asignaciones del empleado 1016
-- Descripción: Inactiva los registros vigentes del empleado 1016
-- ═══════════════════════════════════════════════════════════════════════════

-- Mostrar los registros antes de actualizar
SELECT 'SALARIOS VIGENTES' AS Tipo, emp_no, salary, from_date, to_date FROM salaries WHERE emp_no = 1016 AND to_date IS NULL;
SELECT 'CARGOS VIGENTES' AS Tipo, emp_no, title, from_date, to_date FROM titles WHERE emp_no = 1016 AND to_date IS NULL;
SELECT 'DEPARTAMENTOS VIGENTES' AS Tipo, emp_no, dept_no, from_date, to_date FROM dept_emp WHERE emp_no = 1016 AND to_date IS NULL;
SELECT 'MANAGERS VIGENTES' AS Tipo, emp_no, dept_no, from_date, to_date FROM dept_manager WHERE emp_no = 1016 AND to_date IS NULL;

-- Cerrar salarios vigentes
UPDATE salaries 
SET to_date = CAST(GETDATE() AS DATE)
WHERE emp_no = 1016 AND to_date IS NULL;

-- Cerrar cargos vigentes
UPDATE titles 
SET to_date = CAST(GETDATE() AS DATE)
WHERE emp_no = 1016 AND to_date IS NULL;

-- Cerrar asignaciones de departamento vigentes
UPDATE dept_emp 
SET to_date = CAST(GETDATE() AS DATE)
WHERE emp_no = 1016 AND to_date IS NULL;

-- Cerrar asignaciones de manager vigentes
UPDATE dept_manager 
SET to_date = CAST(GETDATE() AS DATE)
WHERE emp_no = 1016 AND to_date IS NULL;

-- Verificar los cambios
PRINT '═══════════════════════════════════════════════════════════════════════════';
PRINT 'CAMBIOS REALIZADOS AL EMPLEADO 1016:';
PRINT '═══════════════════════════════════════════════════════════════════════════';

SELECT 'SALARIOS (AHORA INACTIVOS)' AS Tipo, emp_no, salary, from_date, to_date FROM salaries WHERE emp_no = 1016;
SELECT 'CARGOS (AHORA INACTIVOS)' AS Tipo, emp_no, title, from_date, to_date FROM titles WHERE emp_no = 1016;
SELECT 'DEPARTAMENTOS (AHORA INACTIVOS)' AS Tipo, emp_no, dept_no, from_date, to_date FROM dept_emp WHERE emp_no = 1016;
SELECT 'MANAGERS (AHORA INACTIVOS)' AS Tipo, emp_no, dept_no, from_date, to_date FROM dept_manager WHERE emp_no = 1016;

PRINT '═══════════════════════════════════════════════════════════════════════════';
PRINT 'ESTADO DEL EMPLEADO 1016:';
PRINT '═══════════════════════════════════════════════════════════════════════════';
SELECT emp_no, first_name, last_name, is_active, email FROM employees WHERE emp_no = 1016;
