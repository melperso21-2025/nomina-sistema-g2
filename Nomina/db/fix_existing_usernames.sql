-- ═══════════════════════════════════════════════════════════════════════════
-- SCRIPT: Actualizar usernames de emp_no 1016 y 1017
-- Descripción: Convierte CI a formato nombre.apellido para los usuarios existentes
-- ═══════════════════════════════════════════════════════════════════════════

-- Verificar datos antes de actualizar
SELECT 
    u.emp_no,
    u.username AS 'Usuario Actual',
    e.first_name,
    e.last_name,
    LOWER(e.first_name + '.' + e.last_name) AS 'Nuevo Usuario'
FROM users u
INNER JOIN employees e ON u.emp_no = e.emp_no
WHERE u.emp_no IN (1016, 1017);

-- Actualizar usuario para emp_no 1016
UPDATE users 
SET username = LOWER((SELECT first_name + '.' + last_name FROM employees WHERE emp_no = 1016))
WHERE emp_no = 1016;

-- Actualizar usuario para emp_no 1017
UPDATE users 
SET username = LOWER((SELECT first_name + '.' + last_name FROM employees WHERE emp_no = 1017))
WHERE emp_no = 1017;

-- Verificar cambios realizados
SELECT 
    u.emp_no,
    u.username AS 'Usuario Actualizado',
    e.first_name,
    e.last_name,
    u.role AS 'Rol'
FROM users u
INNER JOIN employees e ON u.emp_no = e.emp_no
WHERE u.emp_no IN (1016, 1017)
ORDER BY u.emp_no;

PRINT 'Usuarios actualizados correctamente.';
PRINT 'emp_no 1016 y 1017 ahora usan formato nombre.apellido';
