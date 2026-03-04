-- ═══════════════════════════════════════════════════════════════════════════
-- SCRIPT: Validar empleado 1019
-- Descripción: Verifica si el empleado 1019 existe y su estado
-- ═══════════════════════════════════════════════════════════════════════════

-- Verificar si existe el empleado 1019
SELECT 
    'EMPLEADO' AS Tipo,
    emp_no,
    ci,
    first_name + ' ' + last_name AS nombre_completo,
    is_active,
    hire_date
FROM employees
WHERE emp_no = 1019;

-- Verificar si tiene usuario
SELECT 
    'USUARIO' AS Tipo,
    emp_no,
    username,
    role
FROM users
WHERE emp_no = 1019;

-- Verificar si tiene salario
SELECT 
    'SALARIO' AS Tipo,
    emp_no,
    salary,
    from_date,
    to_date
FROM salaries
WHERE emp_no = 1019;

-- Verificar si tiene cargo
SELECT 
    'CARGO' AS Tipo,
    emp_no,
    title,
    from_date,
    to_date
FROM titles
WHERE emp_no = 1019;

-- Verificar si tiene departamento
SELECT 
    'DEPARTAMENTO' AS Tipo,
    emp_no,
    dept_no,
    from_date,
    to_date
FROM dept_emp
WHERE emp_no = 1019;

PRINT '=================================================================';
PRINT 'Análisis del empleado 1019:';
PRINT '=================================================================';
IF EXISTS (SELECT 1 FROM employees WHERE emp_no = 1019)
BEGIN
    PRINT 'Empleado encontrado en BD';
    IF (SELECT is_active FROM employees WHERE emp_no = 1019) = 1
        PRINT 'Estado: ACTIVO (debería aparecer en listado)';
    ELSE
        PRINT 'Estado: INACTIVO (no aparecería en listado sin filtro)';
END
ELSE
BEGIN
    PRINT 'ERROR: Empleado 1019 NO existe en la BD';
END
