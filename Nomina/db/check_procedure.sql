-- ═══════════════════════════════════════════════════════════════════════════
-- SCRIPT: Verificar procedimiento sp_insert_employee_conditional
-- Descripción: Valida si el procedimiento existe y está compilado correctamente
-- ═══════════════════════════════════════════════════════════════════════════

PRINT '=================================================================';
PRINT 'Verificando procedimiento sp_insert_employee_conditional';
PRINT '=================================================================';

-- Verificar si el procedimiento existe
IF EXISTS (SELECT * FROM INFORMATION_SCHEMA.ROUTINES WHERE ROUTINE_NAME = 'sp_insert_employee_conditional')
    PRINT '✓ Procedimiento EXISTE';
ELSE
BEGIN
    PRINT '✗ ERROR: Procedimiento NO EXISTE - Necesita ser creado';
    PRINT 'Ejecuta: db/fix_insert_employee_conditional.sql';
END

-- Ver la definición del procedimiento
PRINT '';
PRINT 'Mostrando definición del procedimiento:';
PRINT '=================================================================';
EXEC sp_helptext 'sp_insert_employee_conditional';
