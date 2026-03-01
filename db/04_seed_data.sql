-- =============================================
-- 04_SEED_DATA.SQL
-- Sistema de Nomina G2
-- Datos de prueba para desarrollo y demo
-- Clave de todos los usuarios: admin123
-- =============================================

USE nomina_db;
GO

-- ─────────────────────────────────────────────
-- EMPLOYEES
-- ─────────────────────────────────────────────
INSERT INTO employees (emp_no, ci, first_name, last_name, birth_date, gender, hire_date, email, is_active)
VALUES 
-- Equipo del proyecto
(1001, '1721456781', 'Israel',    'Arevalo',    '1990-05-15', 'M', '2020-01-10', 'i.arevalo@empresa.com',    1),
(1002, '1721456782', 'Alexandra', 'Caicedo',    '1992-08-22', 'F', '2020-01-15', 'a.caicedo@empresa.com',    1),
(1003, '1721456783', 'Diego',     'Simbana',    '1988-11-30', 'M', '2019-06-01', 'd.simbana@empresa.com',    1),
(1004, '1721456784', 'Lizbeth',   'Chacagusay', '1995-02-14', 'F', '2021-03-12', 'l.chacagusay@empresa.com', 1),
(1005, '1721456785', 'Ismael',    'Vanegas',    '1993-07-20', 'M', '2021-05-20', 'i.vanegas@empresa.com',    1),
-- Empleados adicionales de prueba
(1006, '1721456786', 'Carlos',    'Mendoza',    '1985-01-10', 'M', '2018-02-15', 'c.mendoza@test.com',       1),
(1007, '1721456787', 'Maria',     'Pazmino',    '1994-03-25', 'F', '2022-01-05', 'm.pazmino@test.com',       1),
(1008, '1721456788', 'Juan',      'Torres',     '1980-12-12', 'M', '2015-11-20', 'j.torres@test.com',        1),
(1009, '1721456789', 'Elena',     'Rojas',      '1996-06-30', 'F', '2023-04-01', 'e.rojas@test.com',         1),
(1010, '1721456790', 'Luis',      'Salgado',    '1989-09-05', 'M', '2020-08-15', 'l.salgado@test.com',       1),
(1011, '1721456791', 'Sofia',     'Vaca',       '1991-10-10', 'F', '2021-09-10', 's.vaca@test.com',          1),
(1012, '1721456792', 'Ricardo',   'Lopez',      '1987-04-18', 'M', '2017-12-01', 'r.lopez@test.com',         1),
(1013, '1721456793', 'Gabriela',  'Ortiz',      '1998-02-28', 'F', '2024-01-15', 'g.ortiz@test.com',         1),
(1014, '1721456794', 'Fernando',  'Castro',     '1982-11-11', 'M', '2016-03-14', 'f.castro@test.com',        1),
(1015, '1721456795', 'Paola',     'Moran',      '1994-05-05', 'F', '2022-07-22', 'p.moran@test.com',         1);
GO

-- ─────────────────────────────────────────────
-- DEPARTMENTS
-- ─────────────────────────────────────────────
INSERT INTO departments (dept_no, dept_name, is_active)
VALUES 
(1, 'Administracion',    1),
(2, 'Sistemas',          1),
(3, 'Recursos Humanos',  1),
(4, 'Operaciones',       1),
(5, 'Finanzas',          1);
GO

-- ─────────────────────────────────────────────
-- SALARIES
-- El trigger trg_salary_audit se activa automaticamente
-- y llena salary_audit_log con cada INSERT aqui
-- ─────────────────────────────────────────────
INSERT INTO salaries (emp_no, salary, from_date, to_date)
VALUES 
(1001, 2800, '2020-01-10', NULL),
(1002, 2500, '2020-01-15', NULL),
(1003, 3200, '2019-06-01', NULL),
(1004, 1800, '2021-03-12', NULL),
(1005, 2100, '2021-05-20', NULL),
(1006, 1500, '2018-02-15', NULL),
(1007, 1400, '2022-01-05', NULL),
(1008, 3500, '2015-11-20', NULL),
(1009, 1200, '2023-04-01', NULL),
(1010, 2000, '2020-08-15', NULL),
(1011, 1900, '2021-09-10', NULL),
(1012, 2600, '2017-12-01', NULL),
(1013, 1100, '2024-01-15', NULL),
(1014, 2900, '2016-03-14', NULL),
(1015, 1750, '2022-07-22', NULL);
GO

-- ─────────────────────────────────────────────
-- TITLES
-- ─────────────────────────────────────────────
INSERT INTO titles (emp_no, title, from_date, to_date)
VALUES 
(1001, 'Senior Developer',         '2020-01-10', NULL),
(1002, 'Analista de Sistemas',     '2020-01-15', NULL),
(1003, 'Jefe de IT',               '2019-06-01', NULL),
(1004, 'Asistente Administrativo', '2021-03-12', NULL),
(1005, 'Soporte Tecnico',          '2021-05-20', NULL),
(1006, 'Contador',                 '2018-02-15', NULL),
(1007, 'Secretaria',               '2022-01-05', NULL),
(1008, 'Gerente General',          '2015-11-20', NULL),
(1009, 'Pasante',                  '2023-04-01', NULL),
(1010, 'Supervisor',               '2020-08-15', NULL),
(1011, 'Reclutador',               '2021-09-10', NULL),
(1012, 'Ingeniero de Redes',       '2017-12-01', NULL),
(1013, 'Vendedor',                 '2024-01-15', NULL),
(1014, 'Project Manager',          '2016-03-14', NULL),
(1015, 'Analista Contable',        '2022-07-22', NULL);
GO

-- ─────────────────────────────────────────────
-- DEPT_EMP
-- Asignacion de empleados a departamentos
-- ─────────────────────────────────────────────
INSERT INTO dept_emp (emp_no, dept_no, from_date, to_date)
VALUES 
(1001, 2, '2020-01-10', NULL),
(1002, 2, '2020-01-15', NULL),
(1003, 2, '2019-06-01', NULL),
(1004, 1, '2021-03-12', NULL),
(1005, 2, '2021-05-20', NULL),
(1006, 5, '2018-02-15', NULL),
(1007, 1, '2022-01-05', NULL),
(1008, 1, '2015-11-20', NULL),
(1009, 4, '2023-04-01', NULL),
(1010, 4, '2020-08-15', NULL),
(1011, 3, '2021-09-10', NULL),
(1012, 2, '2017-12-01', NULL),
(1013, 4, '2024-01-15', NULL),
(1014, 2, '2016-03-14', NULL),
(1015, 5, '2022-07-22', NULL);
GO

-- ─────────────────────────────────────────────
-- DEPT_MANAGER
-- Un manager activo por departamento
-- ─────────────────────────────────────────────
INSERT INTO dept_manager (emp_no, dept_no, from_date, to_date)
VALUES 
(1003, 2, '2019-06-01', NULL),  -- Diego Simbana → Sistemas
(1008, 1, '2015-11-20', NULL),  -- Juan Torres   → Administracion
(1006, 5, '2018-02-15', NULL),  -- Carlos Mendoza → Finanzas
(1010, 4, '2020-08-15', NULL),  -- Luis Salgado  → Operaciones
(1011, 3, '2021-09-10', NULL);  -- Sofia Vaca    → Recursos Humanos
GO

-- ─────────────────────────────────────────────
-- USERS
-- Todos con clave: admin123
-- Hash SHA256 de "admin123":
-- 6CA13D52CA70C883E0F0BB101E425A89E8624DE51DB2D2392593AF6A84118090
-- Roles validos: 'Admin' o 'RRHH'
-- ─────────────────────────────────────────────
INSERT INTO users (emp_no, username, password_hash, role)
VALUES 
(1005, 'ismael.vanegas',  0x6CA13D52CA70C883E0F0BB101E425A89E8624DE51DB2D2392593AF6A84118090, 'Admin'),
(1001, 'israel.arevalo',  0x6CA13D52CA70C883E0F0BB101E425A89E8624DE51DB2D2392593AF6A84118090, 'RRHH'),
(1002, 'alexandra.cai',   0x6CA13D52CA70C883E0F0BB101E425A89E8624DE51DB2D2392593AF6A84118090, 'RRHH'),
(1003, 'diego.simbana',   0x6CA13D52CA70C883E0F0BB101E425A89E8624DE51DB2D2392593AF6A84118090, 'Admin'),
(1004, 'lizbeth.chac',    0x6CA13D52CA70C883E0F0BB101E425A89E8624DE51DB2D2392593AF6A84118090, 'RRHH');
GO

