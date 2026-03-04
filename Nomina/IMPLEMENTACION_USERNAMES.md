-- ═══════════════════════════════════════════════════════════════════════════
-- RESUMEN: Manejo de Usernames y Colisiones
-- ═══════════════════════════════════════════════════════════════════════════

/*
PROBLEMA IDENTIFICADO:
- Usuarios 0908930332 y 0908930333 tienen CI como username
- Falta control de colisiones cuando dos empleados tienen el mismo nombre

SOLUCIÓN IMPLEMENTADA:

1. CORREGIR USUARIOS EXISTENTES
   ├─ Script: db/fix_existing_usernames.sql
   ├─ Acción: Actualiza emp_no 1016 y 1017 a formato nombre.apellido
   └─ Resultado: 0908930332 → nombre.apellido, 0908930333 → nombre.apellido

2. PREVENIR COLISIONES EN NUEVOS EMPLEADOS
   ├─ Script: db/sp_insert_with_collision_detection.sql
   ├─ Lógica: 
   │  ├─ Intenta crear: juan.perez
   │  ├─ Si existe, crea: juan.perez2
   │  ├─ Si existe, crea: juan.perez3
   │  └─ Y así sucesivamente...
   └─ Método: Loop While en procedimiento almacenado

3. VALIDACIÓN EN CREACIÓN MANUAL
   ├─ Controlador: UsersController.cs (método Crear POST)
   ├─ Validaciones:
   │  ├─ Username no puede estar vacío
   │  ├─ Solo letras minúsculas, números y puntos
   │  ├─ Debe ser único en la tabla users
   │  └─ Si existe, sugiere alternativa (nombre.apellido2)
   └─ Mensaje: "Intenta con otro (ej: nombre.apellido2)"

4. MEJORA DE VISTA DE CREACIÓN
   ├─ Archivo: Views/Users/Crear.cshtml
   ├─ Mejoras:
   │  ├─ Auto-genera username al seleccionar empleado
   │  ├─ Valida formato en tiempo real (minúsculas, puntos, números)
   │  ├─ Muestra sugerencia si hay error
   │  └─ Permite editar si es necesario
   └─ User Experience: Más intuitiva y segura

═══════════════════════════════════════════════════════════════════════════

PASOS PARA IMPLEMENTAR:

1. EJECUTAR SCRIPTS EN BD (SQL Server)
   
   a) Primero ejecuta (DEBE SER PRIMERO - corrige existentes):
      → db/fix_existing_usernames.sql
      
   b) Luego ejecuta (mejora procedimiento para futuros):
      → db/sp_insert_with_collision_detection.sql

2. RECARGAR APLICACIÓN
   → Hot reload en Visual Studio
   → Verifica que compila sin errores

3. PROBAR CREACIÓN DE USUARIO
   → Vuelve a la vista de crear usuario
   → Selecciona empleado
   → Observa cómo se auto-genera el username
   → Si lo modificas, valida formato automáticamente

═══════════════════════════════════════════════════════════════════════════

ESCENARIOS DE COLISIÓN MANEJADOS:

Escenario 1: Nombres únicos
├─ Empleado: Juan Pérez
├─ Username generado: juan.perez
└─ Resultado: ✅ SIN CONFLICTO

Escenario 2: Dos Juan Pérez (mismo nombre)
├─ Empleado 1: Juan Pérez → juan.perez
├─ Empleado 2: Juan Pérez → juan.perez2 (automático)
└─ Resultado: ✅ MANEJADO POR PROCEDIMIENTO

Escenario 3: Tres Juan Pérez (raro pero posible)
├─ Empleado 1: Juan Pérez → juan.perez
├─ Empleado 2: Juan Pérez → juan.perez2
├─ Empleado 3: Juan Pérez → juan.perez3
└─ Resultado: ✅ ESCALABLE

Escenario 4: Usuario intenta crear manual duplicado
├─ Intenta crear: juan.perez (ya existe)
├─ Validación: ❌ ERROR
├─ Sugerencia: "Intenta con otro (ej: juan.perez2)"
└─ Resultado: ✅ USUARIO DEBE ELEGIR OTRO

═══════════════════════════════════════════════════════════════════════════

CAMBIOS DE CÓDIGO:

✅ Controllers/UsersController.cs
   - Agregó validación de formato (regex)
   - Mensaje mejorado para colisiones
   
✅ Views/Users/Crear.cshtml
   - Auto-generación de username
   - Validación en tiempo real
   - Mejor UX

✅ db/sp_insert_with_collision_detection.sql (NUEVO)
   - Procedimiento mejorado con loop de detección
   - Genera sufijos numéricos automáticamente
   - Retorna el username generado en mensaje

✅ db/fix_existing_usernames.sql (NUEVO)
   - Script para corregir 1016 y 1017
   - Usa LOWER(first_name + '.' + last_name)

═══════════════════════════════════════════════════════════════════════════
*/
