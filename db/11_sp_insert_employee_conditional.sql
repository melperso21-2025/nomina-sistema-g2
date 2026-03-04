USE nomina_db;
GO

ALTER PROCEDURE sp_insert_employee_conditional
    @p_emp_no           INT,
    @p_ci               VARCHAR(50),
    @p_first_name       VARCHAR(50),
    @p_last_name        VARCHAR(50),
    @p_birth_date       DATE,
    @p_gender           CHAR(1),
    @p_hire_date        DATE,
    @p_email            VARCHAR(100),
    @p_requires_access  BIT,
    @p_password_hash    VARBINARY(MAX) = NULL,
    @p_role             VARCHAR(50)    = NULL,
    @p_dept_no          INT,
    @p_salary           BIGINT,
    @p_title            VARCHAR(50),
    @p_user_session     VARCHAR(100),
    @r_message          VARCHAR(200) OUTPUT
AS
BEGIN
    SET NOCOUNT ON;
    BEGIN TRANSACTION;
    BEGIN TRY

        IF EXISTS (SELECT 1 FROM employees WHERE ci = @p_ci)
        BEGIN
            SET @r_message = 'ERROR: CI ya registrado';
            ROLLBACK TRANSACTION; RETURN;
        END

        IF EXISTS (SELECT 1 FROM employees WHERE email = @p_email)
        BEGIN
            SET @r_message = 'ERROR: Email ya registrado';
            ROLLBACK TRANSACTION; RETURN;
        END

        INSERT INTO employees (emp_no, ci, first_name, last_name, birth_date, gender, hire_date, email)
        VALUES (@p_emp_no, @p_ci, @p_first_name, @p_last_name, @p_birth_date, @p_gender, @p_hire_date, @p_email);

        IF @p_requires_access = 1 AND @p_password_hash IS NOT NULL
        BEGIN
            INSERT INTO users (emp_no, username, password_hash, role)
            VALUES (@p_emp_no, @p_ci, @p_password_hash, @p_role);
        END

        DECLARE @context VARBINARY(128) = CAST(@p_user_session AS VARBINARY(128));
        SET CONTEXT_INFO @context;

        INSERT INTO salaries (emp_no, salary, from_date, to_date)
        VALUES (@p_emp_no, @p_salary, @p_hire_date, NULL);

        INSERT INTO titles (emp_no, title, from_date, to_date)
        VALUES (@p_emp_no, @p_title, @p_hire_date, NULL);

        INSERT INTO dept_emp (emp_no, dept_no, from_date, to_date)
        VALUES (@p_emp_no, @p_dept_no, @p_hire_date, NULL);

        COMMIT TRANSACTION;
        SET @r_message = 'SUCCESS: Empleado registrado';

    END TRY
    BEGIN CATCH
        ROLLBACK TRANSACTION;
        SET @r_message = 'ERROR: ' + ERROR_MESSAGE();
    END CATCH
END
GO