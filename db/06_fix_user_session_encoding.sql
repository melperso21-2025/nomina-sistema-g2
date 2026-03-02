-- =============================================
-- 06_FIX_user_session_encoding.SQL
-- Sistema de Nomina G2
-- Fix: RTRIM en user_session de sp_report_salary_changes
-- Corrige los signos raros (◆◆◆) en columna Usuario
-- del Reporte de Cambios Salariales
-- =============================================

USE nomina_db;
GO

ALTER PROCEDURE sp_report_salary_changes
    @p_date_from DATE = NULL,
    @p_date_to   DATE = NULL,
    @p_ci        VARCHAR(50) = NULL
AS
BEGIN
    SET NOCOUNT ON;

    SELECT
        sal.log_id,
        sal.action_date,
        RTRIM(CONVERT(NVARCHAR(200), sal.user_session)) AS user_session,
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
