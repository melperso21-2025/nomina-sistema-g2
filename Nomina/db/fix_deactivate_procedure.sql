-- ─────────────────────────────────────────────
-- SP: sp_deactivate_employee (ACTUALIZADO)
-- Baja logica — no borra fisicamente
-- Además inactiva salarios, cargos y asignaciones vigentes
-- ─────────────────────────────────────────────
DROP PROCEDURE IF EXISTS sp_deactivate_employee;
GO

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

    -- Marcar empleado como inactivo
    UPDATE employees SET is_active = 0 WHERE emp_no = @p_emp_no;
    
    -- Cerrar salarios vigentes (to_date IS NULL)
    UPDATE salaries 
    SET to_date = CAST(GETDATE() AS DATE)
    WHERE emp_no = @p_emp_no AND to_date IS NULL;
    
    -- Cerrar cargos vigentes (to_date IS NULL)
    UPDATE titles 
    SET to_date = CAST(GETDATE() AS DATE)
    WHERE emp_no = @p_emp_no AND to_date IS NULL;
    
    -- Cerrar asignaciones de departamento vigentes
    UPDATE dept_emp 
    SET to_date = CAST(GETDATE() AS DATE)
    WHERE emp_no = @p_emp_no AND to_date IS NULL;
    
    -- Cerrar asignaciones de manager vigentes
    UPDATE dept_manager 
    SET to_date = CAST(GETDATE() AS DATE)
    WHERE emp_no = @p_emp_no AND to_date IS NULL;
    
    SET @r_message = 'SUCCESS: Employee deactivated';
END
GO
