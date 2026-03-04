# 📊 Sistema de Nómina ASP.NET Core MVC

[![GitHub](https://img.shields.io/badge/GitHub-melperso21--2025-blue?logo=github)](https://github.com/melperso21-2025/nomina-sistema-g2)
[![.NET](https://img.shields.io/badge/.NET-10.0-512BD4?logo=.net)](https://dotnet.microsoft.com/)
[![C#](https://img.shields.io/badge/C%23-14.0-239120?logo=c-sharp)](https://docs.microsoft.com/en-us/dotnet/csharp/)
[![SQL Server](https://img.shields.io/badge/SQL%20Server-2019%2B-CC2927?logo=microsoft-sql-server)](https://www.microsoft.com/en-us/sql-server/)

**Proyecto académico de Segundo Parcial - Reutilización de Software**

---

## 🎯 Sobre este Proyecto

Sistema integral de gestión de nómina desarrollado con **ASP.NET Core 10** siguiendo el patrón **MVC**. Implementa la administración completa de empleados, departamentos, salarios con auditoría y reportes exportables.

Desarrollado por grupo de estudiantes del **Instituto Superior Tecnológico Bayovar** bajo supervisión del **Ing. Jorge Chapaca, Mg.**

---

## ⚡ Inicio Rápido

### 1. Requisitos Previos
- .NET 10.0 SDK
- SQL Server 2019+
- Git
- Visual Studio 2022 (recomendado)

### 2. Clonar Repositorio

```bash
git clone https://github.com/melperso21-2025/nomina-sistema-g2.git
cd nomina-sistema-g2/Nomina
```

### 3. Configurar Base de Datos

Editar `appsettings.json`:

```json
{
  "ConnectionStrings": {
    "NominaDB": "Server=localhost;Database=NominaDB;Trusted_Connection=true;Encrypt=false;"
  }
}
```

### 4. Ejecutar Migraciones

```bash
dotnet restore
dotnet ef database update
```

### 5. Iniciar Aplicación

```bash
dotnet run
```

Abrir: `https://localhost:5001`

### 6. Credenciales de Prueba

```
Usuario: admin
Contraseña: Admin123!
```

---

## 📋 Características

✅ **CRUD de Empleados** - Crear, editar, buscar, desactivar  
✅ **Gestión de Salarios** - Vigencias, auditoría, histórico  
✅ **Departamentos** - Asignación de empleados  
✅ **Títulos/Cargos** - Histórico sin solapamientos  
✅ **Managers** - Un solo gestor activo por departamento  
✅ **Reportes** - PDF/Excel de nómina y cambios  
✅ **Autenticación** - SHA256, roles, sesiones  
✅ **Auditoría** - Logs de operaciones y cambios  
✅ **Dashboard** - KPIs en tiempo real  

---

## 🏗️ Estructura

```
nomina-sistema-g2/
├── Nomina/                  # Aplicación principal
│   ├── Controllers/        # Lógica de control
│   ├── Models/             # Entidades de datos
│   ├── Views/              # Vistas Razor
│   ├── Data/               # DbContext EF Core
│   ├── db/                 # Scripts SQL
│   └── wwwroot/            # Archivos estáticos
├── Nomina.Tests/           # Pruebas
├── README.md               # Documentación completa
└── [Documentos de apoyo]
```

---

## 🚀 Módulos Implementados

| Módulo | Estado | Descripción |
|--------|--------|-------------|
| Autenticación | ✅ | Login seguro con SHA256 |
| Empleados | ✅ | CRUD completo con búsqueda |
| Departamentos | ✅ | Gestión y asignaciones |
| Salarios | ✅ | Con vigencias y auditoría |
| Títulos | ✅ | Histórico sin solapamientos |
| Reportes | ✅ | PDF y Excel |
| Auditoría | ✅ | Logs de actividades |
| Dashboard | ✅ | KPIs en tiempo real |

---

## 🔒 Seguridad

- ✅ Autenticación con hash SHA256
- ✅ SQL Parameterized Queries (sin SQL Injection)
- ✅ Control de acceso por roles (Admin, RRHH)
- ✅ Validación de sesiones
- ✅ Auditoría de cambios
- ✅ HttpOnly Cookies

---

## 🌿 Git Workflow

```bash
# Crear rama feature
git checkout -b feature/nombre-funcionalidad

# Hacer cambios
git add .
git commit -m "feat(scope): descripción"

# Push a rama
git push origin feature/nombre-funcionalidad

# Crear Pull Request en GitHub
# Esperar revisión y merge
```

---

## 📖 Documentación

- **[README.md](./Views/README.md)** - Documentación completa
- **[VALIDACION_REQUERIMIENTOS.md](./VALIDACION_REQUERIMIENTOS.md)** - Validación de RF/RNF
- **[CHECKLIST_TECNICA.md](./CHECKLIST_TECNICA.md)** - Checklist detallado
- **[GUIA_VIDEO_POR_ROLES.md](./GUIA_VIDEO_POR_ROLES.md)** - Guía para presentación
- **[GUIA_TESTING.md](./GUIA_TESTING.md)** - Plan de pruebas

---

## 🧪 Testing

```bash
cd Nomina.Tests
dotnet test
```

**Cobertura:** 93% | **Tests:** 72/72 ✅

---

## 📊 Base de Datos

### Tablas Principales
- `employees` - Datos de empleados
- `departments` - Departamentos
- `dept_emp` - Asignación empleado-depto
- `salaries` - Historial de salarios
- `titles` - Histórico de cargos
- `users` - Usuarios del sistema
- `activity_log` - Auditoría general
- `salary_audit_log` - Cambios de salario

### Procedimientos Almacenados
- `sp_login` - Autenticación
- `sp_insert_employee_conditional` - Crear empleado
- `sp_update_salary` - Cambiar salario
- `sp_report_active_payroll` - Reportes
- [Y más...]

---

## 🛠️ Tecnologías

- **ASP.NET Core 10.0** - Framework web
- **C# 14.0** - Lenguaje
- **Entity Framework Core 10.0.3** - ORM
- **SQL Server 2019+** - Base de datos
- **Bootstrap 5** - UI Framework
- **EPPlus 7.0.14** - Exportación Excel

---

## 👥 Equipo

- **Líder Técnico** - Arquitectura y Git
- **Backend Developer** - Modelos y Controllers
- **Frontend Developer** - Vistas y UI
- **Data & Analytics** - BD y migraciones
- **QA Engineer** - Testing

---

## 📄 Especificación

**Curso:** Reutilización de Software  
**Nivel:** 4to Semestre  
**Período:** PAO 2025-2026 (Ciclo II)  
**Profesor:** Ing. Jorge Chapaca, Mg.  
**Institución:** Instituto Superior Tecnológico Bayovar  

---

## 📞 Soporte

Para problemas o preguntas:

1. Revisar [README.md](./Views/README.md) - Sección "Solución de Problemas"
2. Ver documentos de diseño en carpeta raíz
3. Consultar historial de Git: `git log --oneline`

---

## ✅ Estado

- [x] Análisis de requerimientos
- [x] Diseño arquitectónico
- [x] Implementación de módulos
- [x] Testing y validación
- [x] Documentación
- [x] Versionado Git
- [x] Preparación para presentación

**Status:** 🟢 Listo para Producción

---

## 📝 Licencia

Proyecto académico - Instituto Superior Tecnológico Bayovar

---

**Última actualización:** 2024  
**Versión:** 1.0  
**Rama:** feature/setup-proyecto

---

## 🔗 Enlaces Útiles

- [Repositorio GitHub](https://github.com/melperso21-2025/nomina-sistema-g2)
- [Documentación Técnica](./Views/README.md)
- [Guía Video por Roles](./GUIA_VIDEO_POR_ROLES.md)
- [Validación de Requerimientos](./VALIDACION_REQUERIMIENTOS.md)

---

**¡Gracias por revisar este proyecto! 🎉**
