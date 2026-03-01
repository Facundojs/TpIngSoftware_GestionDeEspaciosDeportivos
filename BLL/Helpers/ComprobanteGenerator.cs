using System;
using System.Text;

namespace BLL.Helpers
{
    public static class ComprobanteGenerator
    {
        public static byte[] GenerarComprobantePago(DateTime fecha, string dniCliente, decimal monto, string metodo, string codigo)
        {
            var sb = new StringBuilder();
            sb.AppendLine("========================================");
            sb.AppendLine("           COMPROBANTE DE PAGO          ");
            sb.AppendLine("========================================");
            sb.AppendLine($"Fecha:        {fecha:dd/MM/yyyy HH:mm}");
            sb.AppendLine($"Cliente DNI:  {dniCliente}");
            sb.AppendLine($"Monto:        {monto:C}");
            sb.AppendLine($"Método:       {metodo}");
            if (!string.IsNullOrEmpty(codigo))
            {
                sb.AppendLine($"Código:       {codigo}");
            }
            sb.AppendLine("========================================");

            return Encoding.UTF8.GetBytes(sb.ToString());
        }

        public static byte[] GenerarComprobanteReserva(string codigoReserva, string espacio, DateTime fechaHora, decimal montoTotal, decimal adelanto, decimal saldo)
        {
            var sb = new StringBuilder();
            sb.AppendLine("========================================");
            sb.AppendLine("         COMPROBANTE DE RESERVA         ");
            sb.AppendLine("========================================");
            sb.AppendLine($"Reserva Code: {codigoReserva}");
            sb.AppendLine($"Espacio:      {espacio}");
            sb.AppendLine($"Fecha y Hora: {fechaHora:dd/MM/yyyy HH:mm}");
            sb.AppendLine("----------------------------------------");
            sb.AppendLine($"Monto Total:  {montoTotal:C}");
            sb.AppendLine($"Adelanto:     {adelanto:C}");
            sb.AppendLine($"Saldo:        {saldo:C}");
            sb.AppendLine("========================================");

            return Encoding.UTF8.GetBytes(sb.ToString());
        }
    }
}
