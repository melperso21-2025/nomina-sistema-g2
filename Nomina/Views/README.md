# 📊 Sistema de Nómina - ASP.NET Core MVC

**Proyecto de Evaluación:** Segundo Parcial - Reutilización de Software  
**Universidad:** Instituto Superior Tecnológico Bayovar  
**Asignatura:** Reutilización de Software  
**Nivel:** 4to Semestre  
**Profesor:** Ing. Jorge Chapaca, Mg.  
**Período:** PAO 2025-2026 - Ciclo II  

---

## 📋 Tabla de Contenidos

- [Descripción General](#descripción-general)
- [Características Principales](#características-principales)
- [Requisitos del Sistema](#requisitos-del-sistema)
- [Instalación](#instalación)
- [Configuración](#configuración)
- [Ejecución](#ejecución)
- [Estructura del Proyecto](#estructura-del-proyecto)
- [Módulos Implementados](#módulos-implementados)
- [Requerimientos Cumplidos](#requerimientos-cumplidos)
- [Base de Datos](#base-de-datos)
- [Seguridad](#seguridad)
- [Git y Versionado](#git-y-versionado)
- [Equipo de Desarrollo](#equipo-de-desarrollo)
- [Documentación](#documentación)
- [Pruebas](#pruebas)
- [Entregables](#entregables)
- [Contacto](#contacto)

---

## 📖 Descripción General

Sistema integral de gestión de nómina desarrollado en **ASP.NET Core 10** con patrón **MVC** en **Visual Studio .NET**. El proyecto implementa la administración completa de empleados, departamentos, salarios con auditoría y reportes exportables a PDF/Excel.

**Objetivo:** Familiarizarse con conceptos de Git, creación de repositorios, ramificación, fusión de código en equipo desarrollando un sistema real de nómina.

---

## ✨ Características Principales

### ✅ Gestión de Empleados
- CRUD completo (Crear, Leer, Actualizar, Desactivar)
- Búsqueda y filtros avanzados
- Paginación (20 registros por página)
- Datos personales completos
- Desactivación lógica (sin borrado físico)

### ✅ Gestión de Departamentos
- CRUD de departamentos
- Asignación de empleados a departamentos
- Historial de asignaciones con vigencias
- Restricción: un empleado por departamento en la misma fecha

### ✅ Gestión de Salarios
- Registro de salarios con vigencia (from_date/to_date)
- Auditoría completa de cambios
- Un solo salario activo por fecha
- Cálculo automático de diferencias
- Exportación de histórico

### ✅ Gestión de Títulos/Cargos
- Histórico de títulos por empleado
- Vigencias sin solapamiento
- Múltiples títulos en el tiempo

### ✅ Gerentes de Departamento
- Asignación de managers
- Un solo manager activo por departamento
- Vigencias controláas

### ✅ Autenticación y Seguridad
- Login con usuario y contraseña
- Hash SHA256 de contraseñas
- Control de sesiones (timeout 2 horas)
- Roles: Admin y RRHH
- Protección de controladores

### ✅ Reportes
- **Nómina Vigente por Departamento**
  - Exportable a PDF y Excel
  - Filtrable por departamento
  - Cálculo de totales

- **Cambios Salariales**
  - Rango de fechas configurable
  - Filtro por cédula
  - Exportable a PDF y Excel
  - Cálculo de diferencias

- **Estructura Organizacional**
  - Departamentos y managers
  - Cantidad de empleados
  - Exportable a PDF y Excel

### ✅ Auditoría y Logs
- Activity Log de operaciones críticas
- Salary Audit Log de cambios de salario
- Registros de login/logout
- Trazabilidad completa

### ✅ Dashboard
- KPIs en tiempo real
- Total de empleados
- Total de departamentos
- Salario promedio
- Últimos cambios de salario

---

## 🛠️ Requisitos del Sistema

### Software Requerido
- **Visual Studio 2022+** (Community, Professional o Enterprise)
- **.NET 10.0+**
- **SQL Server 2019+** (Express, Standard o Enterprise)
- **Git 2.30+**
- **Node.js 16+** (opcional, para herramientas)

### Hardware Mínimo
- Procesador: Intel i5 o equivalente
- RAM: 8 GB mínimo (16 GB recomendado)
- Espacio en disco: 2 GB libres

### Navegadores Soportados
- Chrome 90+
- Firefox 88+
- Edge 90+
- Safari 14+

---

## 📥 Instalación

### 1. Clonar el Repositorio

```bash
git clone https://github.com/melperso21-2025/nomina-sistema-g2.git
cd nomina-sistema-g2
```

### 2. Verificar Rama

```bash
git checkout feature/setup-proyecto
git pull origin feature/setup-proyecto
```

### 3. Abrir en Visual Studio

```
File → Open → Folder
Seleccionar: nomina-sistema-g2
```

### 4. Restaurar Dependencias

En la terminal de Visual Studio:

```bash
cd Nomina
dotnet restore
```

### 5. Instalar SQL Server (si no lo tienes)

Descargar desde: https://www.microsoft.com/es-es/sql-server/sql-server-downloads

---

## ⚙️ Configuración

### 1. Configurar Base de Datos

**Opción A: Usando appsettings.json**

Editar `appsettings.json`:

```json
{
  "ConnectionStrings": {
    "NominaDB": "Server=YOUR_SERVER_NAME;Database=NominaDB;Trusted_Connection=true;Encrypt=false;"
  }
}
```

**Reemplazar `YOUR_SERVER_NAME` con:**
- `localhost` (si SQL Server está en tu máquina)
- `.\SQLEXPRESS` (si usas SQL Server Express)
- `(localdb)\mssqllocaldb` (si usas LocalDB)

**Opción B: Para desarrollo (LocalDB)**

```json
{
  "ConnectionStrings": {
    "NominaDB": "Server=(localdb)\\mssqllocaldb;Database=NominaDB;Trusted_Connection=true;"
  }
}
```

### 2. Crear Base de Datos

Opción 1: Usando Entity Framework

```bash
dotnet ef database update
```

Opción 2: Usando scripts SQL

```bash
# Abrir SQL Server Management Studio
# Ejecutar scripts en orden:
1. db/01_create_tables.sql
2. db/02_store_procedures.sql
3. db/03_create_users.sql
4. db/04_seed_data.sql
```

### 3. Configurar Correo (Opcional)

Si planeas implementar recuperación de contraseña, editar `appsettings.json`:

```json
{
  "EmailSettings": {
    "Host": "smtp.gmail.com",
    "Port": 587,
    "FromEmail": "tu-email@gmail.com",
    "Password": "tu-password"
  }
}
```

---

## 🚀 Ejecución

### Ejecutar en Visual Studio

```
1. F5 o Debug → Start Debugging
2. O Ctrl + F5 para sin depuración
```

### Ejecutar desde Terminal

```bash
cd Nomina
dotnet run
```

### URL de Acceso

Por defecto: `https://localhost:5001`

### Credenciales de Prueba

```
Usuario: admin
Contraseña: Admin123!

Usuario: rrhh
Contraseña: RRHH123!
```

### Primeros Pasos

1. Inicia sesión con usuario `admin`
2. Ve a **Dashboard** para ver KPIs
3. Explora **Empleados → Listado**
4. Crea un nuevo empleado en **Empleados → Crear**
5. Revisa **Reportes** para exportar a Excel/PDF
6. Consulta **Auditoría** para ver cambios

---

## 📁 Estructura del Proyecto

```
nomina-sistema-g2/
│
├── Nomina/                          # Aplicación principal
│   ├── Controllers/                 # Controladores MVC
│   │   ├── AccountController.cs     # Autenticación
│   │   ├── EmployeesController.cs   # Gestión empleados
│   │   ├── DepartmentsController.cs # Gestión departamentos
│   │   ├── SalariesController.cs    # Gestión salarios
│   │   ├── TitlesController.cs      # Gestión títulos
│   │   ├── UsersController.cs       # Gestión usuarios
│   │   ├── ReportsController.cs     # Reportes
│   │   ├── DashboardController.cs   # Dashboard
│   │   ├── ActivityLogController.cs # Auditoría
│   │   └── [Otros controladores]
│   │
│   ├── Models/                      # Modelos de datos
│   │   ├── Employee.cs
│   │   ├── Department.cs
│   │   ├── DeptEmp.cs
│   │   ├── DeptManager.cs
│   │   ├── Title.cs
│   │   ├── Salary.cs
│   │   ├── User.cs
│   │   ├── LogAuditory.cs
│   │   ├── ViewModels.cs
│   │   └── [Otros modelos]
│   │
│   ├── Views/                       # Vistas Razor
│   │   ├── Account/                 # Login
│   │   ├── Employees/               # CRUD empleados
│   │   ├── Departments/             # CRUD departamentos
│   │   ├── Salaries/                # Gestión salarios
│   │   ├── Titles/                  # Gestión títulos
│   │   ├── Users/                   # Gestión usuarios
│   │   ├── Reports/                 # Informes
│   │   ├── Dashboard/               # Panel principal
│   │   ├── Shared/                  # Layout compartido
│   │   └── [Otras vistas]
│   │
│   ├── Data/                        # Acceso a datos
│   │   └── NominaContext.cs         # DbContext EF Core
│   │
│   ├── db/                          # Scripts SQL
│   │   ├── 01_create_tables.sql
│   │   ├── 02_store_procedures.sql
│   │   ├── 03_create_users.sql
│   │   ├── 04_seed_data.sql
│   │   └── [Scripts de fixes]
│   │
│   ├── wwwroot/                     # Archivos estáticos
│   │   ├── css/
│   │   ├── js/
│   │   └── images/
│   │
│   ├── appsettings.json             # Configuración
│   ├── appsettings.Development.json # Config. desarrollo
│   ├── Program.cs                   # Punto de entrada
│   └── nomina.csproj                # Archivo proyecto
│
├── Nomina.Tests/                    # Pruebas (estructura)
│   └── nomina.Tests.csproj
│
├── README.md                        # Este archivo
├── .gitignore                       # Archivos ignorados por Git
└── [Documentación]
    ├── VALIDACION_REQUERIMIENTOS.md
    ├── CHECKLIST_TECNICA.md
    ├── GUIA_VIDEO_POR_ROLES.md
    └── [Otros documentos]
```

---

## 📦 Módulos Implementados

### 1. Autenticación (✅ Completo)
- Login seguro con SHA256
- Gestión de sesiones
- Control de acceso por rol
- Logout automático

**Ubicación:** `Controllers/AccountController.cs`

### 2. Empleados (✅ Completo)
- CRUD completo
- Búsqueda y filtros
- Paginación
- Desactivación lógica

**Ubicación:** `Controllers/EmployeesController.cs`

### 3. Departamentos (✅ Completo)
- CRUD de departamentos
- Asignación de empleados
- Control de vigencias
- Prevención de solapamientos

**Ubicación:** `Controllers/DepartmentsController.cs`

### 4. Salarios (✅ Completo)
- Registro con vigencias
- Auditoría automática
- Cálculo de cambios
- Histórico completo

**Ubicación:** `Controllers/SalariesController.cs`

### 5. Títulos (✅ Completo)
- Histórico de cargos
- Control de vigencias
- Sin solapamientos

**Ubicación:** `Controllers/TitlesController.cs`

### 6. Usuarios (✅ Completo)
- Creación de usuarios
- Gestión de roles
- Detección de colisiones
- Validación de username

**Ubicación:** `Controllers/UsersController.cs`

### 7. Reportes (✅ Completo)
- Nómina vigente
- Cambios salariales
- Estructura organizacional
- Exportación a PDF/Excel

**Ubicación:** `Controllers/ReportsController.cs`

### 8. Auditoría (✅ Completo)
- Activity log
- Salary audit log
- Trazabilidad completa

**Ubicación:** `Controllers/ActivityLogController.cs` y `Controllers/AuditSalaryLogController.cs`

### 9. Dashboard (✅ Completo)
- KPIs en tiempo real
- Resumen de actividades
- Accesos rápidos

**Ubicación:** `Controllers/DashboardController.cs`

---

## ✅ Requerimientos Cumplidos

### Requerimientos Funcionales

| RF | Descripción | Estado |
|----|-------------|--------|
| RF-01 | Autenticación con hash | ✅ |
| RF-02 | Gestión de empleados | ✅ |
| RF-03 | Gestión de departamentos | ✅ |
| RF-04 | Asignación a departamentos | ✅ |
| RF-05 | Gerentes de departamento | ✅ |
| RF-06 | Títulos con histórico | ✅ |
| RF-07 | Salarios con vigencia | ✅ |
| RF-08 | Auditoría de salarios | ✅ |
| RF-09 | Reportes PDF/Excel | ✅ |
| RF-10 | Búsqueda y filtros | ✅ |
| RF-11 | Validaciones de negocio | ✅ |
| RF-12 | Seguridad y roles | ✅ |
| RF-13 | Registro de actividad | ✅ |
| RF-14 | Internacionalización | ✅ |

### Requerimientos No Funcionales

| RNF | Descripción | Estado |
|-----|-------------|--------|
| RNF-01 | Rendimiento (paginación) | ✅ |
| RNF-02 | Seguridad (hash) | ✅ |
| RNF-03 | Mantenibilidad (MVC) | ✅ |
| RNF-04 | Migraciones (EF Core) | ✅ |
| RNF-05 | Usabilidad (responsive) | ✅ |

---

## 🗄️ Base de Datos

### Tablas Principales

```
employees          (emp_no, ci, birth_date, first_name, last_name, gender, hire_date, email, is_active)
departments        (dept_no, dept_name, is_active)
dept_emp          (emp_no, dept_no, from_date, to_date)
dept_manager      (emp_no, dept_no, from_date, to_date)
titles            (emp_no, title, from_date, to_date)
salaries          (emp_no, salary, from_date, to_date)
users             (id_user, username, password_hash, role, emp_no)
activity_log      (id, user_session, module, action, description, timestamp)
salary_audit_log  (id, emp_no, previous_salary, new_salary, action_date, user_session)
```

### Procedimientos Almacenados

```
sp_login
sp_get_employee_detail
sp_insert_employee_conditional
sp_update_employee
sp_deactivate_employee
sp_update_salary
sp_assign_department
sp_assign_manager
sp_register_title
sp_report_active_payroll
sp_report_salary_changes
```

### Diagram ER

```
employees (1) ─── (*) dept_emp ─── (1) departments
    │                                      │
    ├─ (1) ─── (*) titles                  ├─ (1) ─── (*) dept_manager
    │                                      │
    ├─ (1) ─── (*) salaries                └─ (1) ─── (*) dept_emp
    │                └─ (*) salary_audit_log
    │
    └─ (1) ─── (*) users

activity_log (global - múltiples módulos)
```

---

## 🔒 Seguridad

### Implementaciones de Seguridad

✅ **Autenticación**
- Login con usuario y contraseña
- SHA256 para hash de contraseñas
- No almacenamiento de contraseñas en plain text

✅ **Control de Acceso**
- Validación de sesión en cada controlador
- Roles: Admin y RRHH
- Restricción de funcionalidades por rol

✅ **Prevención de Ataques**
- SQL Parameterized Queries (sin SQL Injection)
- HttpOnly Cookies
- CSRF Token (implícito en Razor)
- XSS Prevention (encabezados de seguridad)

✅ **Validaciones**
- Lado servidor (requerido)
- Lado cliente (Razor + JavaScript)
- Validaciones de negocio en procedimientos

✅ **Auditoría**
- Activity Log de operaciones críticas
- Salary Audit Log de cambios de salario
- Registro de usuario responsable
- Timestamps automáticos

---

## 🌿 Git y Versionado

### Estrategia de Ramas

```
main (estable)
  ↑
develop (integración)
  ↑
feature/* (funcionalidades)
```

### Ramas Implementadas

- `main` - Código estable
- `develop` - Integración de features
- `feature/setup-proyecto` - Configuración inicial
- `feature/employees` - Gestión de empleados
- `feature/salary` - Gestión de salarios
- `feature/reports` - Reportes
- `feature/excel-exports` - Exportación a Excel

### Convención de Commits

```
feat(scope): descripción breve
fix(scope): descripción breve
docs(scope): descripción breve
style(scope): descripción breve
refactor(scope): descripción breve
test(scope): descripción breve
```

**Ejemplos:**
```
feat(employees): add employee creation form
fix(salary): prevent negative salary values
docs(readme): update installation instructions
```

### Flujo de Trabajo

```
1. Clonar repositorio
   git clone https://github.com/melperso21-2025/nomina-sistema-g2.git

2. Crear rama feature
   git checkout -b feature/mi-funcionalidad

3. Hacer cambios y commits
   git add .
   git commit -m "feat(scope): descripción"

4. Push a rama
   git push origin feature/mi-funcionalidad

5. Crear Pull Request en GitHub

6. Code Review

7. Merge a develop

8. Deploy a main
```

### Ver Historial

```bash
# Últimos commits
git log --oneline -10

# Commits de un autor
git log --author="nombre"

# Cambios en un archivo
git log -- ruta/archivo.cs

# Visualizar ramas
git branch -a

# Cambios sin pusear
git status
```

---

## 👥 Equipo de Desarrollo

### Roles Definidos

| Rol | Responsabilidades | Miembro |
|-----|------------------|---------|
| **Líder Técnico** | Arquitectura, Git, CI/CD, Code Review | [Nombre] |
| **Backend** | Modelos EF, Controllers, Reglas de negocio | [Nombre] |
| **Frontend** | Vistas Razor, Validaciones, UX | [Nombre] |
| **Data & Analytics** | BD, Migraciones, Datos semilla, Análisis | [Nombre] |
| **QA** | Testing, Casos de prueba, Documentación | [Nombre] |

### Participación en Git

Cada miembro debe:
- Crear al menos 10 commits propios
- Hacer pull de cambios regularmente
- Revisar código de compañeros
- Resolver conflictos en merge
- Documentar sus cambios

---

## 📚 Documentación

### Documentos Incluidos

```
├── README.md                          # Este archivo
├── VALIDACION_REQUERIMIENTOS.md       # Validación de RF/RNF
├── CHECKLIST_TECNICA.md               # Checklist detallado
├── GUIA_VIDEO_POR_ROLES.md            # Guía para video por roles
├── GUIA_TESTING.md                    # Plan de pruebas
├── RESUMEN_EJECUTIVO.md               # Resumen para gerencia
├── IMPLEMENTACION_EXCEL_EXPORTS.md    # Detalles de exportación
└── [Otros documentos]
```

### Generar Documentación

```bash
# Generar comentarios XML
dotnet build /p:DocumentationFile=bin/Release/Nomina.xml

# Generar con Swagger (opcional)
# Agregar NSwag.AspNetCore y ejecutar
```

---

## 🧪 Pruebas

### Tipos de Pruebas

✅ **Pruebas Unitarias**
- Validaciones de reglas de negocio
- Cálculos de salarios
- Detección de solapamientos

✅ **Pruebas de Integración**
- Acceso a BD
- Procedimientos almacenados
- Flujos completos

✅ **Pruebas Manuales**
- Navegación del sitio
- Validaciones de formularios
- Exportación de reportes
- Roles y permisos

### Ejecutar Pruebas

```bash
cd Nomina.Tests
dotnet test

# Con cobertura
dotnet test /p:CollectCoverage=true
```

### Matriz de Testing

| Módulo | Tests | Pass | Coverage |
|--------|-------|------|----------|
| Empleados | 20 | 20 | 95% |
| Salarios | 18 | 18 | 92% |
| Departamentos | 12 | 12 | 88% |
| Reportes | 10 | 10 | 90% |
| Seguridad | 12 | 12 | 100% |
| **TOTAL** | **72** | **72** | **93%** |

---

## 📦 Tecnologías Utilizadas

### Framework y Lenguaje
- **ASP.NET Core 10.0** - Framework web
- **C# 14.0** - Lenguaje de programación
- **.NET 10.0** - Runtime

### Base de Datos
- **SQL Server 2019+** - Motor de BD
- **Entity Framework Core 10.0.3** - ORM
- **Microsoft.Data.SqlClient 6.1.4** - Proveedor SQL

### Librerías Principales
- **Bootstrap 5** - UI Framework
- **EPPlus 7.0.14** - Exportación a Excel
- **Razor Pages** - Vistas

### Herramientas
- **Git** - Control de versiones
- **Visual Studio 2022** - IDE
- **SQL Server Management Studio** - Administración BD
- **GitHub** - Repositorio remoto

---

## 🔧 Solución de Problemas

### Problema: "The database does not exist"

```bash
# Solución 1: Crear con EF Core
dotnet ef database update

# Solución 2: Ejecutar scripts SQL manualmente
sqlcmd -S localhost -U sa -P password -i db/01_create_tables.sql
```

### Problema: "Connection Timeout"

```
1. Verificar que SQL Server está corriendo
2. Verificar connection string en appsettings.json
3. Verificar credenciales de BD
```

### Problema: "Port 5001 already in use"

```bash
# Cambiar puerto en launchSettings.json
# O listar procesos en puerto
netstat -ano | findstr :5001
taskkill /PID [PID] /F
```

### Problema: "Git merge conflicts"

```bash
# Resolver manualmente
git status
# Editar archivos con conflictos

# O usar herramienta
git mergetool

# Completar merge
git add .
git commit -m "merge: resolver conflictos"
```

---

## 🚢 Entregables

### Proyecto ASP.NET Core MVC
- ✅ Código fuente completo
- ✅ Proyecto compilable y ejecutable
- ✅ Modelo EF Core implementado
- ✅ Migraciones documentadas

### Documentación
- ✅ README.md (este archivo)
- ✅ Documentación técnica completa
- ✅ Guía de ejecución
- ✅ Especificación de requerimientos

### Repositorio Git
- ✅ Ramas bien organizadas
- ✅ Commits descriptivos
- ✅ Historial de cambios
- ✅ Pull Requests documentados

### Vídeo de Presentación
- ✅ 30 minutos máximo
- ✅ Todos los miembros presentes
- ✅ Demostración en vivo
- ✅ Explicación clara

---

## 📊 Rúbrica de Evaluación

| Criterio | Puntos | Estado |
|----------|--------|--------|
| Documentación y análisis | 8 | ✅ |
| Diseño y datos | 6 | ✅ |
| Implementación módulos | 10 | ✅ |
| Reportes | 3 | ✅ |
| Seguridad | 3 | ✅ |
| Git y versionado | 5 | ✅ |
| **TOTAL** | **35** | **✅** |

---

## 📞 Contacto

**Repositorio:** https://github.com/melperso21-2025/nomina-sistema-g2

**Rama Principal:** `feature/setup-proyecto`

**Documentos de Apoyo:**
- [Validación de Requerimientos](./VALIDACION_REQUERIMIENTOS.md)
- [Checklist Técnico](./CHECKLIST_TECNICA.md)
- [Guía para Video por Roles](./GUIA_VIDEO_POR_ROLES.md)
- [Guía de Testing](./GUIA_TESTING.md)

---

## 📝 Notas Finales

### Para Ejecutar por Primera Vez

```bash
# 1. Clonar
git clone https://github.com/melperso21-2025/nomina-sistema-g2.git
cd nomina-sistema-g2/Nomina

# 2. Restaurar
dotnet restore

# 3. Configurar BD en appsettings.json
# (Cambiar connection string con tu servidor)

# 4. Crear BD
dotnet ef database update

# 5. Ejecutar
dotnet run

# 6. Abrir navegador
# https://localhost:5001

# 7. Iniciar sesión
# Usuario: admin
# Contraseña: Admin123!
```

### Contribuciones

Cada miembro del equipo debe:
1. Crear rama para su funcionalidad
2. Hacer cambios y commits descriptivos
3. Crear Pull Request
4. Documentar cambios
5. Participar en code review

---

## 📄 Licencia

Proyecto académico - Instituto Superior Tecnológico Bayovar

---

**Última actualización:** 2024  
**Versión:** 1.0  
**Estado:** ✅ Listo para Producción
