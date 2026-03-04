// ============================================================
// RF-11: Pruebas Unitarias — Regla de Solapamiento de Fechas
// Espeja la lógica de sp_update_salary:
//   WHERE to_date IS NULL AND from_date >= @p_from_date
// ============================================================

using Nomina.Models;

namespace Nomina.Tests
{
    // ─────────────────────────────────────────────────────────────
    // Clase auxiliar que replica la regla del SP en C# puro.
    // Permite probar la lógica sin base de datos ni HttpContext.
    // ─────────────────────────────────────────────────────────────
    public static class SalaryDateOverlapRule
    {
        /// <summary>
        /// Retorna true si <paramref name="newFromDate"/> genera solapamiento
        /// con algún salario vigente del empleado.
        /// Regla: existe un registro con to_date IS NULL y from_date >= newFromDate.
        /// </summary>
        public static bool HasOverlap(
            IEnumerable<SalaryListItem> registros,
            int empNo,
            DateTime newFromDate)
        {
            return registros.Any(s =>
                s.EmpNo == empNo &&
                s.ToDate == null &&
                s.FromDate >= newFromDate);
        }
    }

    // ─────────────────────────────────────────────────────────────
    // Suite de pruebas RF-11
    // ─────────────────────────────────────────────────────────────
    public class UnitTest1
    {
        // ── Dato base reutilizado por todos los tests ─────────────
        private const int EmpNo = 101;
        private static readonly DateTime FechaVigente = new(2020, 1, 1);

        /// <summary>Crea un salario vigente (to_date = null) para los Arrange.</summary>
        private static SalaryListItem CrearSalarioVigente(int empNo, DateTime fromDate) => new()
        {
            IdSalario = 1,
            EmpNo = empNo,
            FullName = "Juan Pérez",
            Salary = 1_000_000m,
            FromDate = fromDate,
            ToDate = null  // NULL = vigente, igual que en la BD
        };

        // ── [CASO 1] from_date IGUAL al vigente → BLOQUEADO ──────
        [Fact]
        public void RF11_FromDateIgualAlVigente_DebeDetectarSolapamiento()
        {
            // Arrange
            var salarios = new List<SalaryListItem> { CrearSalarioVigente(EmpNo, FechaVigente) };
            var nuevaFecha = FechaVigente; // misma fecha

            // Act
            bool solapamiento = SalaryDateOverlapRule.HasOverlap(salarios, EmpNo, nuevaFecha);

            // Assert
            Assert.True(solapamiento,
                "RF-11: Debe bloquear cuando from_date es igual a la fecha del salario vigente.");
        }

        // ── [CASO 2] from_date ANTERIOR al vigente → BLOQUEADO ───
        [Fact]
        public void RF11_FromDateAnteriorAlVigente_DebeDetectarSolapamiento()
        {
            // Arrange
            var salarios = new List<SalaryListItem> { CrearSalarioVigente(EmpNo, FechaVigente) };
            var nuevaFecha = new DateTime(2019, 6, 1); // anterior al vigente

            // Act
            bool solapamiento = SalaryDateOverlapRule.HasOverlap(salarios, EmpNo, nuevaFecha);

            // Assert
            Assert.True(solapamiento,
                "RF-11: Debe bloquear cuando from_date es anterior a la fecha del salario vigente.");
        }

        // ── [CASO 3] from_date POSTERIOR al vigente → PERMITIDO ──
        [Fact]
        public void RF11_FromDatePosteriorAlVigente_NoDebeSolapar()
        {
            // Arrange
            var salarios = new List<SalaryListItem> { CrearSalarioVigente(EmpNo, FechaVigente) };
            var nuevaFecha = new DateTime(2024, 1, 1); // posterior → camino feliz

            // Act
            bool solapamiento = SalaryDateOverlapRule.HasOverlap(salarios, EmpNo, nuevaFecha);

            // Assert
            Assert.False(solapamiento,
                "RF-11: Debe permitir el registro cuando from_date es posterior al salario vigente.");
        }

        // ── [CASO 4] emp_no DIFERENTE → la regla no aplica ───────
        [Fact]
        public void RF11_EmpleadoDiferente_NoDebeSolapar()
        {
            // Arrange: la lista solo tiene un salario del emp 101
            var salarios = new List<SalaryListItem> { CrearSalarioVigente(EmpNo, FechaVigente) };
            var otroEmpNo = 202; // empleado distinto
            var nuevaFecha = new DateTime(2019, 1, 1); // fecha que solaparía para el emp 101

            // Act
            bool solapamiento = SalaryDateOverlapRule.HasOverlap(salarios, otroEmpNo, nuevaFecha);

            // Assert
            Assert.False(solapamiento,
                "RF-11: No debe evaluar solapamiento en registros de otro empleado.");
        }
    }
}