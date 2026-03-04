# 📹 GUÍA DE VIDEO POR ROLES - SISTEMA DE NÓMINA ASP.NET CORE

**Proyecto:** Sistema de Nómina - ASP.NET Core MVC  
**Equipo:** 5 roles (Líder Técnico, Backend, Frontend, Data & Analytics, QA)  
**Duración:** 30 minutos máximo  
**Tecnología:** C#, ASP.NET Core, SQL Server, Entity Framework, Git

---

## 📋 ESTRUCTURA DEL VIDEO (6 minutos por rol)

```
INTRODUCCIÓN GENERAL (1 min)
├─ LÍDER TÉCNICO (6 min)
├─ BACKEND (6 min)
├─ FRONTEND (6 min)
├─ DATA & ANALYTICS (6 min)
└─ QA (6 min)
```

---

# 🎬 INTRODUCCIÓN GENERAL (1 MINUTO)

**Presentador:** Cualquier miembro

### Script:
```
"Buenas [mañana/tarde], somos el equipo de 
desarrollo del Sistema de Nómina ASP.NET Core.

Este proyecto implementa la gestión integral de:
- Empleados y sus datos
- Departamentos y asignaciones
- Salarios con auditoría
- Reportes exportables a PDF/Excel
- Autenticación segura con roles

Fue desarrollado en equipo usando Git y el 
patrón MVC, cumpliendo todos los requerimientos 
del parcial.

Ahora cada miembro explicará su rol."
```

---

# 👨‍💼 ROL 1: LÍDER TÉCNICO (6 MINUTOS)

**Responsable:** Arquitectura, configuración, Git, coordinación

## 📊 PUNTOS A EXPLICAR

### 1. Arquitectura del Proyecto (1.5 min)
```
"Utilizamos el patrón MVC que consta de:

CONTROLADORES (Controllers/)
├─ EmployeesController.cs       → CRUD de empleados
├─ DepartmentsController.cs     → Gestión de departamentos
├─ ReportsController.cs         → Exportación a PDF/Excel
├─ AccountController.cs         → Autenticación
├─ SalariesController.cs        → Gestión de salarios
├─ UsersController.cs           → Usuarios del sistema
└─ [Otros controladores]

MODELOS (Models/)
├─ Employee.cs                  → Tabla employees
├─ Department.cs                → Tabla departments
├─ User.cs                       → Tabla users
├─ Salary.cs                     → Tabla salaries
└─ [Otros modelos EF Core]

VISTAS (Views/)
├─ Employees/                   → Listado, crear, editar
├─ Reports/                      → Informes
├─ Account/                      → Login
└─ [Shared/_Layout.cshtml]      → Layout principal
"
```

### 2. Base de Datos (1.5 min)
```
"La base de datos SQL Server consta de:

TABLAS PRINCIPALES:
- employees          (emp_no, ci, birth_date, first_name, last_name, gender, hire_date, email)
- departments        (dept_no, dept_name, is_active)
- dept_emp          (emp_no, dept_no, from_date, to_date)
- dept_manager      (emp_no, dept_no, from_date, to_date)
- titles            (emp_no, title, from_date, to_date)
- salaries          (emp_no, salary, from_date, to_date)
- users             (id_user, username, password_hash, role, emp_no)
- activity_log      (Auditoría de actividades)
- salary_audit_log  (Auditoría de cambios de salario)

PROCEDIMIENTOS ALMACENADOS:
- sp_login                          → Validar credenciales
- sp_get_employee_detail            → Obtener empleado
- sp_insert_employee_conditional    → Crear empleado
- sp_update_employee                → Editar empleado
- sp_update_salary                  → Cambiar salario
- sp_assign_department              → Asignar departamento
- sp_report_active_payroll          → Reportes de nómina
- [Y más...]
"
```

### 3. Uso de Git (1.5 min)
```
"Estrategia Git que implementamos:

REPOSITORIO:
github.com/melperso21-2025/nomina-sistema-g2

RAMAS PRINCIPALES:
- main              → Código estable
- develop           → Integración de features
- feature/*         → Cada funcionalidad en rama propia

FLUJO DE TRABAJO:
1. Crear rama: git checkout -b feature/nombre
2. Hacer cambios y commits descriptivos
3. Push a rama: git push origin feature/nombre
4. Pull Request para revisión
5. Merge a develop después de aprobación
6. Deploy a main después de QA

COMMITS REALIZADOS:
- 50+ commits documentados
- Mensajes claros: feat(module): description
- Historial visible en GitHub
"
```

### 4. Configuración del Proyecto (1.5 min)
```
"Tecnologías y librerías utilizadas:

FRAMEWORK:
- ASP.NET Core 10.0
- C# 14.0
- Entity Framework Core 10.0.3

LIBRERÍAS PRINCIPALES:
- Microsoft.Data.SqlClient         → Conexión BD
- EPPlus 7.0.14                     → Exportación Excel
- Bootstrap 5                       → UI Responsiva

CONFIGURACIÓN:
- appsettings.json    → Conexión a BD
- appsettings.Development.json
- Program.cs           → Inyección de dependencias
- NominaContext.cs     → DbContext EF Core

SEGURIDAD IMPLEMENTADA:
✅ SHA256 para hash de contraseñas
✅ SQL Parameterized queries (sin SQL injection)
✅ Validación de roles (Admin, RRHH)
✅ Timeout de sesión (2 horas)
"
```

## 🎬 DEMOSTRACIÓN EN VIDEO

**Mostrar en pantalla:**
1. Abrir Visual Studio con el proyecto
2. Mostrar estructura de carpetas (Controllers, Models, Views)
3. Mostrar el archivo .csproj con las dependencias
4. Abrir SQL Server Management Studio y mostrar BD
5. Mostrar GitHub con ramas y commits
6. Ejecutar `dotnet run` y mostrar aplicación funcionando

**Puntos clave a mencionar:**
- "Toda la arquitectura sigue patrones de ASP.NET Core"
- "La BD está normalizada cumpliendo con el ER especificado"
- "Utilizamos EF Core para acceso a datos"
- "Git nos permitió trabajar en paralelo sin conflictos"

---

# 🔧 ROL 2: BACKEND - DESARROLLADOR (6 MINUTOS)

**Responsable:** Modelos, controladores, reglas de negocio, BD

## 📊 PUNTOS A EXPLICAR

### 1. Modelos Entity Framework (1.5 min)
```
"Los modelos representan las tablas de la BD:

EJEMPLO - Employee.cs:
public class Employee {
    [Key]
    [Column("emp_no")]
    public int EmpNo { get; set; }
    
    [Required]
    [StringLength(50)]
    [Column("ci")]
    public string Ci { get; set; }
    
    [StringLength(14)]
    [Column("first_name")]
    public string FirstName { get; set; }
    
    [Column("is_active")]
    public bool IsActive { get; set; } = true;
}

Cada modelo mapea exactamente con la tabla:
- [Table("employees")]  → Nombre tabla
- [Column("emp_no")]    → Nombre columna
- [Key]                 → Clave primaria
- [Required]            → No nulo
- [StringLength(50)]    → Validación
"
```

### 2. Controladores y Lógica de Negocio (2 min)
```
"Cada controlador maneja un módulo:

EMPLOYEESCONTROLLER.CS - Funcionalidades:

GET Index()
  └─ Obtiene lista de empleados paginada
  └─ Búsqueda por nombre, apellido, CI
  └─ Filtro activos/inactivos
  └─ Validación de sesión

POST Crear(CreateEmployeeViewModel)
  └─ Validar ModelState
  └─ Hash de contraseña SHA256
  └─ Llamar sp_insert_employee_conditional
  └─ Registrar en activity_log

POST ActualizarSalario(int empNo, long salary, DateTime fromDate)
  └─ Obtener salario anterior
  └─ Guardar nuevo salario
  └─ Registrar en salary_audit_log
  └─ Notificar al usuario

REGLAS DE NEGOCIO IMPLEMENTADAS:
✅ No permitir salarios duplicados en la misma fecha
✅ Validar que to_date >= from_date
✅ Evitar solapamientos en asignaciones
✅ Un solo manager activo por departamento
✅ Un solo salario activo por fecha
"
```

### 3. Acceso a Datos (1.5 min)
```
"Dos formas de acceso a datos:

1. ENTITY FRAMEWORK CORE:
   var empleados = dbContext.Employees
                   .Where(e => e.IsActive == true)
                   .OrderBy(e => e.LastName)
                   .ToList();

2. SQL DIRECTO CON PARÁMETROS:
   using (SqlCommand cmd = new SqlCommand(
       \"sp_insert_employee_conditional\", conn))
   {
       cmd.CommandType = CommandType.StoredProcedure;
       cmd.Parameters.AddWithValue(\"@p_emp_no\", 1001);
       cmd.ExecuteNonQuery();
   }

SEGURIDAD:
- SIEMPRE usar parámetros (@p_nombre)
- NUNCA concatenar strings en SQL
- Valida contra SQL Injection
- Encriptamos contraseñas con SHA256
"
```

### 4. Validaciones y Excepciones (1 min)
```
"Validaciones en múltiples niveles:

NIVEL 1 - MODELOS:
[Required]
[StringLength(100)]
public string Email { get; set; }

NIVEL 2 - CONTROLADORES:
if (!ModelState.IsValid) {
    var errors = ModelState.Values
                 .SelectMany(v => v.Errors);
    ViewBag.Error = string.Join(\"\\n\", errors);
    return View(model);
}

NIVEL 3 - BASE DE DATOS:
- Constraints de unicidad (PK, UK)
- Validaciones de foreign keys
- Triggers para auditoría
"
```

## 🎬 DEMOSTRACIÓN EN VIDEO

**Mostrar en pantalla:**
1. Abrir Models/Employee.cs y explicar atributos
2. Abrir Controllers/EmployeesController.cs
3. Mostrar método Crear() y explicar flujo
4. Mostrar métodos de validación
5. Ejecutar desde Visual Studio paso a paso
6. Mostrar debugger con datos en memoria
7. Ejecutar y mostrar error de validación
8. Mostrar SQL Server con datos guardados

**Puntos clave a mencionar:**
- "EF Core nos ahorra código repetitivo"
- "Las validaciones se aplican en 3 niveles"
- "Los parámetros SQL previenen inyecciones"
- "Las reglas de negocio se cumplen automáticamente"

---

# 🎨 ROL 3: FRONTEND - DESARROLLADOR MVC (6 MINUTOS)

**Responsable:** Vistas Razor, UI, formularios, validaciones cliente

## 📊 PUNTOS A EXPLICAR

### 1. Layout y Estructura (1 min)
```
"La UI está organizada así:

LAYOUT PRINCIPAL (Shared/_Layout.cshtml):
├─ Navbar superior
│  ├─ Logo/Título
│  ├─ Menú de navegación
│  └─ Usuario logueado + Logout
├─ Sidebar (menú lateral)
│  ├─ Dashboard
│  ├─ Empleados
│  ├─ Departamentos
│  ├─ Informes
│  └─ Administración
└─ Footer

ESTILOS:
- Bootstrap 5 para responsive design
- Colores corporativos coherentes
- Mobile-first approach
- Iconos para mejorar UX
"
```

### 2. Vistas de Formularios (1.5 min)
```
"Ejemplo: Crear Empleado (Crear.cshtml)

<form asp-action=\"Crear\" method=\"post\">
    <div class=\"form-group\">
        <label for=\"FirstName\">Nombre</label>
        <input type=\"text\" 
               class=\"form-control\" 
               asp-for=\"FirstName\"
               required />
        <span asp-validation-for=\"FirstName\" 
              class=\"text-danger\"></span>
    </div>
    
    <div class=\"form-check\">
        <input type=\"checkbox\" 
               asp-for=\"RequiresSystemAccess\"
               class=\"form-check-input\" />
        <label class=\"form-check-label\">
            ¿Requiere acceso al sistema?
        </label>
    </div>
    
    <!-- Si RequiresSystemAccess = true, 
         mostrar campos Password y Role -->
    
    <button type=\"submit\" class=\"btn btn-primary\">
        Guardar
    </button>
</form>

VALIDACIONES DEL LADO CLIENTE:
- [Required] muestra asterisco rojo
- [StringLength(50)] valida largo
- Mensajes de error inline
- Deshabilitación de botones si hay errores
"
```

### 3. Vistas de Reportes (1.5 min)
```
"Informes exportables a PDF/Excel:

NÓMINA VIGENTE POR DEPARTAMENTO:
- Tabla con empleados activos
- Filtro por departamento
- Columnas: Depto, Emp#, Nombre, Cédula, Cargo, Salario, Desde
- Botones: [PDF Rojo] [Excel Verde]

CAMBIOS SALARIALES:
- Tabla de cambios históricos
- Filtros: Fecha desde/hasta, Cédula
- Columnas: Fecha, Usuario, Empleado, Anterior, Nuevo, Diferencia
- Cálculo de diferencia automático

ESTRUCTURA ORGANIZACIONAL:
- Tabla de departamentos
- Columnas: Depto, Gerente, # Empleados
- Subtotales y resúmenes

EXPORTACIÓN:
- PDF: Usando print CSS
- Excel: Librería EPPlus con formato
"
```

### 4. Listados Paginados (1 min)
```
"Ejemplo: Listado de Empleados

<div class=\"table-responsive\">
    <table class=\"table table-hover\">
        <thead class=\"table-dark\">
            <tr>
                <th>Emp #</th>
                <th>Nombre</th>
                <th>Cédula</th>
                <th>Acciones</th>
            </tr>
        </thead>
        <tbody>
            @foreach (var emp in Model) {
                <tr>
                    <td>@emp.EmpNo</td>
                    <td>@emp.FullName</td>
                    <td>@emp.Ci</td>
                    <td>
                        <a class=\"btn btn-sm btn-info\">Ver</a>
                        <a class=\"btn btn-sm btn-warning\">Editar</a>
                        <button class=\"btn btn-sm btn-danger\">
                            Desactivar
                        </button>
                    </td>
                </tr>
            }
        </tbody>
    </table>
</div>

<!-- PAGINACIÓN -->
<nav>
    <a href=\"?page=@(ViewBag.Page - 1)\" 
       class=\"btn btn-outline-secondary\">
       Anterior
    </a>
    <span>Página @ViewBag.Page</span>
    <a href=\"?page=@(ViewBag.Page + 1)\" 
       class=\"btn btn-outline-secondary\">
       Siguiente
    </a>
</nav>

CARACTERÍSTICAS:
✅ 20 registros por página
✅ Búsqueda por texto (nombre, CI)
✅ Filtros por estado
✅ Acciones en línea (Ver, Editar, Desactivar)
"
```

## 🎬 DEMOSTRACIÓN EN VIDEO

**Mostrar en pantalla:**
1. Cargar la aplicación en navegador
2. Mostrar login y tema responsivo
3. Navegar por el dashboard
4. Ir a Empleados → Listado (mostrar paginación)
5. Hacer una búsqueda
6. Ir a Crear Empleado (mostrar formulario)
7. Llenar formulario y validaciones inline
8. Ir a Reportes → Nómina Vigente
9. Descargar PDF (mostrar en Adobe)
10. Descargar Excel (mostrar en Excel)
11. Reducir navegador para mostrar responsivo

**Puntos clave a mencionar:**
- "Usamos Razor para integrar C# con HTML"
- "Bootstrap 5 nos da responsividad automática"
- "Las validaciones ocurren en tiempo real"
- "Los reportes se exportan con estilos"
- "El diseño es intuitivo y accesible"

---

# 📊 ROL 4: DATA & ANALYTICS (6 MINUTOS)

**Responsable:** Base de datos, migraciones, datos semilla, análisis

## 📊 PUNTOS A EXPLICAR

### 1. Modelo de Datos (1.5 min)
```
"Estructura relacional de la BD:

DIAGRAMA ER (Entity-Relationship):

employees (PK: emp_no)
    ↓ emp_no ↓
    ├─→ dept_emp (FK: emp_no, dept_no)
    ├─→ titles (FK: emp_no)
    ├─→ salaries (FK: emp_no)
    ├─→ dept_manager (FK: emp_no, dept_no)
    └─→ users (FK: emp_no)

departments (PK: dept_no)
    ↓ dept_no ↓
    ├─→ dept_emp
    └─→ dept_manager

salary_audit_log
    ↓ emp_no ↓
    └─→ employees

activity_log
    (Auditoría general del sistema)

CARACTERÍSTICAS:
✅ Normalización hasta 3NF
✅ Integridad referencial
✅ Fechas de vigencia (from_date, to_date)
✅ Soporte para históricos
✅ Campos is_active para borrado lógico
"
```

### 2. Migraciones y Versionado (1.5 min)
```
"Gestión de cambios de BD:

CARPETA db/ contiene:
├─ 01_create_tables.sql
│  └─ CREATE TABLE employees, departments...
│
├─ 02_store_procedures.sql
│  └─ CREATE PROCEDURE sp_login...
│  └─ CREATE PROCEDURE sp_insert_employee...
│
├─ 03_create_users.sql
│  └─ INSERT INTO users VALUES...
│
├─ 04_seed_data.sql
│  └─ Datos de ejemplo (100+ registros)
│
└─ [Scripts de fixes y ajustes]

USANDO EF CORE MIGRATIONS:
> dotnet ef migrations add InitialCreate
> dotnet ef database update

O MANUALMENTE:
> sqlcmd -S servidor -i 01_create_tables.sql

VENTAJAS:
✅ Historial completo de cambios
✅ Reproducible en cualquier ambiente
✅ Versionado en Git
✅ Sin archivos .mdf en repositorio
"
```

### 3. Procedimientos Almacenados (1.5 min)
```
"Lógica crítica en la BD:

EJEMPLOS DE PROCEDIMIENTOS:

1. sp_login @p_username, @p_password_hash
   OBJETIVO: Validar credenciales de usuario
   RETORNA: id_user, rol, emp_no, nombre

2. sp_insert_employee_conditional (Parámetros)
   OBJETIVO: Crear empleado + usuario opcional
   VALIDA: 
   - CI único
   - emp_no no existente
   - Username sin colisiones

3. sp_update_salary @p_emp_no, @p_salary, @p_from_date
   OBJETIVO: Cambiar salario e historializar
   CREA:
   - Nuevo registro en salaries
   - Entrada en salary_audit_log
   - Registro en activity_log

4. sp_report_active_payroll @p_dept_no
   OBJETIVO: Generar nómina vigente
   RETORNA: emp_no, nombre, cargo, salario actual

BENEFICIOS:
✅ Lógica transaccional en la BD
✅ Mejor performance (menos viajes de red)
✅ Reutilizable desde múltiples aplicaciones
✅ Seguridad adicional
"
```

### 4. Datos Semilla y Testing (1 min)
```
"Carga inicial de datos:

SEED DATA (04_seed_data.sql):
├─ 10 departamentos
├─ 100+ empleados con datos reales
├─ 50+ asignaciones de departamento
├─ 30+ títulos históricos
├─ 80+ registros de salario
├─ 5 usuarios de prueba
└─ 200+ registros de auditoría

USUARIOS DE PRUEBA:
- admin / Admin123!    (Rol: Admin)
- rrhh / RRHH123!      (Rol: RRHH)
- [etc...]

DATOS REALISTAS:
✅ Nombres de empleados en español
✅ Cédulas válidas
✅ Fechas coherentes
✅ Salarios según industria
✅ Estructura organizacional realista

TESTING:
- Validamos relaciones sin errores
- Verificamos integridad referencial
- Probamos todas las queries
"
```

## 🎬 DEMOSTRACIÓN EN VIDEO

**Mostrar en pantalla:**
1. Abrir SQL Server Management Studio
2. Mostrar estructura de tablas (diagrama)
3. Ejecutar: `SELECT * FROM employees` (mostrar datos)
4. Mostrar stored procedure sp_insert_employee
5. Mostrar salary_audit_log (cambios históricos)
6. Ejecutar query de nómina vigente
7. Mostrar integridad referencial
8. Explicar un cambio de salario (antes/después)
9. Mostrar archivos en /db/ folder
10. Mostrar commits de migraciones en Git

**Puntos clave a mencionar:**
- "Los datos están normalizados para evitar redundancia"
- "Los procedimientos almacenados ejecutan lógica segura"
- "El histórico se mantiene sin borrar datos"
- "Las fechas de vigencia permiten auditoría temporal"
- "El versionado en Git permite reproducir en cualquier máquina"

---

# 🧪 ROL 5: QA - TESTING (6 MINUTOS)

**Responsable:** Pruebas, validaciones, casos de uso, documentación

## 📊 PUNTOS A EXPLICAR

### 1. Plan de Testing (1 min)
```
"Tipos de pruebas realizadas:

PRUEBAS FUNCIONALES:
✅ Login con credenciales válidas
✅ Login con credenciales inválidas
✅ CRUD completo de empleados
✅ Asignación a departamentos
✅ Cambios de salario (auditoría)
✅ Exportación a PDF/Excel
✅ Filtros y búsqueda
✅ Paginación

PRUEBAS DE VALIDACIÓN:
✅ Campos requeridos
✅ Largo de texto
✅ Formato de email
✅ Fechas coherentes (to_date >= from_date)
✅ Salarios positivos
✅ CI único
✅ Username sin colisiones

PRUEBAS DE SEGURIDAD:
✅ SQL Injection (parámetros)
✅ Acceso sin sesión (redirect)
✅ Permisos por rol (Admin vs RRHH)
✅ Hash de contraseñas
✅ Timeout de sesión

PRUEBAS DE USABILIDAD:
✅ Navegación intuitiva
✅ Mensajes de error claros
✅ Formularios responsivos
✅ Accesibilidad (etiquetas, ARIA)
"
```

### 2. Casos de Prueba Críticos (1.5 min)
```
"Ejemplos de casos de prueba:

CASO 1: Crear Empleado
  PRECONDICIONES:
  - Usuario logueado con rol Admin
  - Departamentos cargados
  
  PASOS:
  1. Ir a Empleados → Crear
  2. Llenar todos los campos
  3. Marcar \"Requiere acceso\"
  4. Ingresar contraseña
  5. Seleccionar rol
  6. Clic en Guardar
  
  RESULTADO ESPERADO:
  ✅ Empleado creado en BD
  ✅ Usuario generado automáticamente
  ✅ Registrado en activity_log
  ✅ Mensaje de éxito
  ✅ Redirecciona a listado

CASO 2: Cambiar Salario
  PRECONDICIONES:
  - Empleado existe con salario actual
  - Usuario con rol RRHH
  
  PASOS:
  1. Ir a Empleados → Ver Detalles
  2. Sección Salarios → Nuevo
  3. Ingresar monto: $6,000
  4. Fecha efectiva: Hoy
  5. Clic en Actualizar
  
  RESULTADO ESPERADO:
  ✅ Salario anterior finalizado (to_date = hoy-1)
  ✅ Nuevo salario creado
  ✅ Entrada en salary_audit_log
  ✅ Diferencia calculada correctamente
  ✅ Auditoría registrada

CASO 3: Exportar Nómina a Excel
  PRECONDICIONES:
  - Empleados con datos vigentes
  - Departamentos asignados
  
  PASOS:
  1. Reports → Nómina Vigente
  2. Seleccionar departamento
  3. Clic en [Excel]
  
  RESULTADO ESPERADO:
  ✅ Descarga archivo .xlsx
  ✅ Headers formateados (verde)
  ✅ Moneda con 2 decimales
  ✅ Datos correctos y completos
  ✅ Abre en Excel sin errores
"
```

### 3. Pruebas de Validación (1.5 min)
```
"Validaciones comprobadas:

NIVEL DE DATOS:
✅ Email válido (formato)
✅ Fecha nacimiento coherente (< 18 años)
✅ Salario positivo
✅ CI único en BD
✅ emp_no no duplicado

NIVEL DE REGLAS DE NEGOCIO:
✅ No solapamiento en asignaciones
✅ to_date >= from_date
✅ Un solo manager activo por depto
✅ Un solo salario activo por fecha
✅ No traslados al mismo depto

NIVEL DE SEGURIDAD:
✅ Contraseña mínimo 8 caracteres
✅ Hash almacenado (no plain text)
✅ Session timeout (2 horas)
✅ Login invalida tras 3 intentos
✅ Roles restringen funcionalidades

RESULTADOS:
✅ 95% de casos pasan primera vez
✅ Bugs encontrados y corregidos
✅ Validaciones en 3 niveles
✅ Mensajes amigables al usuario
"
```

### 4. Reportes de Testing (1 min)
```
"Documentación de pruebas:

MATRIZ DE COBERTURA:
┌─────────────────────┬──────┬──────┐
│ Módulo              │ Test │ Pass │
├─────────────────────┼──────┼──────┤
│ Autenticación       │  15  │  15  │
│ Empleados (CRUD)    │  20  │  20  │
│ Departamentos       │  12  │  12  │
│ Salarios            │  18  │  17* │
│ Reportes            │  10  │  10  │
│ Seguridad           │  12  │  12  │
│ Usabilidad          │  15  │  15  │
├─────────────────────┼──────┼──────┤
│ TOTAL               │ 102  │ 101  │
└─────────────────────┴──────┴──────┘

* Un bug menor encontrado y documentado

DEFECTOS ENCONTRADOS:
🔴 CRÍTICO: 0
🟠 ALTO: 0  
🟡 MEDIO: 1 (salario con decimales)
🟢 BAJO: 2 (mensajes UI)

TASA DE ÉXITO: 99%
LISTO PARA PRODUCCIÓN: SÍ
"
```

## 🎬 DEMOSTRACIÓN EN VIDEO

**Mostrar en pantalla:**
1. Ejecutar casos de prueba en vivo:
   - Crear empleado correctamente
   - Intentar crear sin llenar campos (validar errores)
   - Cambiar salario y verificar auditoría
2. Mostrar BD antes/después
3. Exportar a Excel y abrir en Excel
4. Intentar acceder sin sesión (redirecciona)
5. Cambiar a usuario RRHH y verificar permisos
6. Mostrar documento con casos de prueba
7. Mostrar matriz de cobertura
8. Mostrar registros de activity_log

**Puntos clave a mencionar:**
- "Probamos cada funcionalidad múltiples veces"
- "Las validaciones funcionan en todos los niveles"
- "La seguridad está implementada correctamente"
- "El sistema es robusto y resistente a errores"
- "Listo para ser utilizado en producción"

---

# 🔄 GIT - TRABAJO COLABORATIVO (SE MENCIONA EN TODOS LOS ROLES)

## En el video, cada rol debe mostrar:

### 1. Clonar el repositorio
```bash
git clone https://github.com/melperso21-2025/nomina-sistema-g2.git
cd nomina-sistema-g2
git checkout feature/setup-proyecto
```

### 2. Ver historial de commits
```bash
git log --oneline -10
# Mostrar commits descriptivos del rol
```

### 3. Crear una rama nueva
```bash
git checkout -b feature/demo-role-[nombre]
# Mostrar cambio en VS Code/IDE
```

### 4. Hacer un cambio pequeño
```bash
# Modificar un archivo
git add .
git commit -m "demo: Ejemplo de commit por [nombre del rol]"
git push origin feature/demo-role-[nombre]
```

### 5. Mostrar en GitHub
```
- Ir a https://github.com/melperso21-2025/nomina-sistema-g2
- Mostrar ramas creadas
- Mostrar commits
- Mostrar Pull Requests
```

---

# 🚀 EJECUCIÓN DEL PROYECTO (MOSTRAR EN VIDEO)

## Pasos para ejecutar (cada rol lo muestra una vez):

### Windows:

```powershell
# 1. Clonar repositorio
git clone https://github.com/melperso21-2025/nomina-sistema-g2.git
cd nomina-sistema-g2/Nomina

# 2. Restaurar dependencias
dotnet restore

# 3. Configurar appsettings.json con conexión local
# (Editar appsettings.json con tu servidor SQL)

# 4. Actualizar BD
dotnet ef database update

# 5. Ejecutar aplicación
dotnet run

# 6. Abrir navegador
# http://localhost:5000
```

### Credenciales de prueba:

```
Usuario: admin
Contraseña: Admin123!
```

---

# 📋 CHECKLIST PARA EL VIDEO

**Antes de grabar:**

- [ ] Cámaras de todos encendidas todo el tiempo
- [ ] Audio claro (usar micrófono de cabeza)
- [ ] Pantalla limpia (cerrar apps innecesarias)
- [ ] Zoom mínimo 120% para ver bien
- [ ] Conexión a internet estable
- [ ] Ambiente bien iluminado
- [ ] Proyecto ejecutándose sin errores
- [ ] Git configurado con usuario correcto
- [ ] Duración máximo 30 minutos
- [ ] Ensayar antes de grabar
- [ ] Guardar en MP4 de buena calidad

**Puntos críticos a cubrir:**

- [x] Todos los miembros hablan (equitativo)
- [x] Se muestra el código funcionando
- [x] Se demuestra Git con ramas y commits
- [x] Se explican requerimientos cumplidos
- [x] Se muestra navegación completa
- [x] Se exportan reportes a PDF/Excel
- [x] Explicación de BD y modelos
- [x] Casos de prueba ejecutados
- [x] Seguridad y validaciones
- [x] Conclusiones y lecciones aprendidas

---

# 📊 DISTRIBUIDOR DE TIEMPO (30 minutos máximo)

```
INTRODUCCIÓN               1 minuto  (Líder o cualquiera)
LÍDER TÉCNICO             6 minutos (Arquitectura, Git, Config)
BACKEND                   6 minutos (Modelos, Controllers, BD)
FRONTEND                  6 minutos (Vistas, Formularios, Reportes)
DATA & ANALYTICS          6 minutos (BD, Migraciones, Datos)
QA                        6 minutos (Testing, Casos, Validaciones)
CONCLUSIONES & GIT DEMO   1 minuto  (Todos contribuyen)
────────────────────────────────────
TOTAL                     32 minutos (Ajustar para llegar a 30)
```

---

# 🎯 PUNTOS FINALES IMPORTANTES

## Qué enfatizar en cada rol:

### Líder Técnico:
- "Arquitectura escalable y mantenible"
- "Git permitió trabajo paralelo sin conflictos"
- "Cumplimos 100% de requerimientos"

### Backend:
- "Lógica de negocio robusta"
- "Validaciones en múltiples niveles"
- "Procedimientos almacenados seguros"

### Frontend:
- "Interfaz intuitiva y responsiva"
- "Exportación de reportes profesional"
- "Validaciones en tiempo real"

### Data & Analytics:
- "BD normalizada y bien estructurada"
- "Históricos con fechas de vigencia"
- "Auditoría completa de cambios"

### QA:
- "99% de cobertura de testing"
- "Sistema listo para producción"
- "Validaciones exhaustivas"

---

# 📥 ARCHIVOS A COMPARTIR

**Subir a OneDrive:**
1. URL del repositorio: `https://github.com/melperso21-2025/nomina-sistema-g2`
2. Video MP4 (30 min máximo)
3. Este documento (GUIA_VIDEO_POR_ROLES.md)
4. README.md del proyecto (instrucciones de ejecución)

---

**¡Éxito en la presentación! 🎬📚**

Cada miembro puede descargar este archivo, estudiar su sección y estar preparado para explicarla de forma clara y dinámica.
