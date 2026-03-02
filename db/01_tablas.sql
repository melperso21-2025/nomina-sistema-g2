<<<<<<< HEAD
﻿-- =============================================
=======
-- =============================================
>>>>>>> develop
-- 01_TABLAS.SQL
-- Sistema de Nomina G2
-- Todas las tablas en ingles, formato snake_case
-- Ejecutar primero antes que cualquier otro script
-- =============================================

IF NOT EXISTS (SELECT * FROM sys.databases WHERE name = 'nomina_db')
BEGIN
    CREATE DATABASE nomina_db;
END
GO

USE nomina_db;
GO

-- ─────────────────────────────────────────────
-- TABLA: employees
-- Datos personales de cada empleado
-- ─────────────────────────────────────────────
CREATE TABLE employees (
    emp_no       INT          PRIMARY KEY,
    ci           VARCHAR(50)  NOT NULL UNIQUE,
    first_name   VARCHAR(50)  NOT NULL,
    last_name    VARCHAR(50)  NOT NULL,
    birth_date   DATE         NOT NULL,
    gender       CHAR(1)      NOT NULL CHECK (gender IN ('M', 'F')),
    hire_date    DATE         NOT NULL,
    email        VARCHAR(100) NOT NULL UNIQUE,
    is_active    BIT          NOT NULL DEFAULT 1
);
GO

-- ─────────────────────────────────────────────
-- TABLA: departments
-- Departamentos de la empresa
-- ─────────────────────────────────────────────
CREATE TABLE departments (
    dept_no    INT          PRIMARY KEY,
    dept_name  VARCHAR(50)  NOT NULL UNIQUE,
    is_active  BIT          NOT NULL DEFAULT 1
);
GO

-- ─────────────────────────────────────────────
-- TABLA: salaries
-- Historial de salarios por empleado
-- to_date NULL = salario vigente
-- ─────────────────────────────────────────────
CREATE TABLE salaries (
    emp_no     INT    NOT NULL,
    salary     BIGINT NOT NULL,
    from_date  DATE   NOT NULL,
    to_date    DATE   NULL,
    PRIMARY KEY (emp_no, from_date),
    CONSTRAINT fk_salaries_employees
        FOREIGN KEY (emp_no) REFERENCES employees(emp_no)
);
GO

-- ─────────────────────────────────────────────
-- TABLA: titles
-- Historial de cargos por empleado
-- to_date NULL = cargo vigente
-- ─────────────────────────────────────────────
CREATE TABLE titles (
    emp_no     INT         NOT NULL,
    title      VARCHAR(50) NOT NULL,
    from_date  DATE        NOT NULL,
    to_date    DATE        NULL,
    PRIMARY KEY (emp_no, from_date),
    CONSTRAINT fk_titles_employees
        FOREIGN KEY (emp_no) REFERENCES employees(emp_no)
);
GO

-- ─────────────────────────────────────────────
-- TABLA: dept_emp
-- Asignacion de empleados a departamentos
-- to_date NULL = asignacion vigente
-- ─────────────────────────────────────────────
CREATE TABLE dept_emp (
    emp_no     INT  NOT NULL,
    dept_no    INT  NOT NULL,
    from_date  DATE NOT NULL,
    to_date    DATE NULL,
    PRIMARY KEY (emp_no, dept_no, from_date),
    CONSTRAINT fk_dept_emp_employees
        FOREIGN KEY (emp_no) REFERENCES employees(emp_no),
    CONSTRAINT fk_dept_emp_departments
        FOREIGN KEY (dept_no) REFERENCES departments(dept_no)
);
GO

-- ─────────────────────────────────────────────
-- TABLA: dept_manager
-- Gerentes asignados por departamento
-- Solo un manager activo por departamento (validado en SP)
-- to_date NULL = manager vigente
-- ─────────────────────────────────────────────
CREATE TABLE dept_manager (
    emp_no     INT  NOT NULL,
    dept_no    INT  NOT NULL,
    from_date  DATE NOT NULL,
    to_date    DATE NULL,
    PRIMARY KEY (emp_no, dept_no, from_date),
    CONSTRAINT fk_dept_manager_employees
        FOREIGN KEY (emp_no) REFERENCES employees(emp_no),
    CONSTRAINT fk_dept_manager_departments
        FOREIGN KEY (dept_no) REFERENCES departments(dept_no)
);
GO

-- ─────────────────────────────────────────────
-- TABLA: users
-- Credenciales de acceso al sistema
-- password_hash: SHA256 aplicado en el backend C#
-- ─────────────────────────────────────────────
CREATE TABLE users (
    emp_no         INT           NOT NULL PRIMARY KEY,
    username       VARCHAR(100)  NOT NULL UNIQUE,
    password_hash  VARBINARY(MAX) NOT NULL,
    role           VARCHAR(50)   NOT NULL CHECK (role IN ('Admin', 'RRHH')),
    CONSTRAINT fk_users_employees
        FOREIGN KEY (emp_no) REFERENCES employees(emp_no)
);
GO

-- ─────────────────────────────────────────────
-- TABLA: salary_audit_log
-- Registro automatico de cambios salariales
-- Poblado por el trigger trg_salary_audit
-- ─────────────────────────────────────────────
CREATE TABLE salary_audit_log (
    log_id          INT           IDENTITY(1,1) PRIMARY KEY,
    emp_no          INT           NOT NULL,
    previous_salary BIGINT        NULL,
    new_salary      BIGINT        NOT NULL,
    from_date       DATE          NOT NULL,
    user_session    VARCHAR(100)  NOT NULL,
    action_date     DATETIME      NOT NULL DEFAULT GETDATE(),
    CONSTRAINT fk_audit_log_employees
        FOREIGN KEY (emp_no) REFERENCES employees(emp_no)
);
<<<<<<< HEAD
GO
=======
GO
>>>>>>> develop
