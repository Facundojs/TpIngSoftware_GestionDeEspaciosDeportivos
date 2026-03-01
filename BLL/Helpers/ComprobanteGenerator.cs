using System;
using System.Text;
using System.Net;

namespace BLL.Helpers
{
    public static class ComprobanteGenerator
    {
        public static byte[] GenerarComprobantePago(DateTime fecha, string dniCliente, decimal monto, string metodo, string codigo)
        {
            var sb = new StringBuilder();
            sb.AppendLine("<html>");
            sb.AppendLine("<head><style>");
            sb.AppendLine("body { font-family: Arial, sans-serif; padding: 20px; }");
            sb.AppendLine("table { border-collapse: collapse; width: 100%; max-width: 600px; margin: auto; }");
            sb.AppendLine("th, td { border: 1px solid #ddd; padding: 8px; text-align: left; }");
            sb.AppendLine("th { background-color: #f2f2f2; }");
            sb.AppendLine(".header { text-align: center; font-size: 24px; font-weight: bold; margin-bottom: 20px; }");
            sb.AppendLine("</style></head>");
            sb.AppendLine("<body>");
            sb.AppendLine("<div class='header'>COMPROBANTE DE PAGO</div>");
            sb.AppendLine("<table>");
            sb.AppendLine($"<tr><th>Fecha</th><td>{fecha:dd/MM/yyyy HH:mm}</td></tr>");
            sb.AppendLine($"<tr><th>Cliente DNI</th><td>{WebUtility.HtmlEncode(dniCliente)}</td></tr>");
            sb.AppendLine($"<tr><th>Monto</th><td>{monto:C}</td></tr>");
            sb.AppendLine($"<tr><th>Método</th><td>{WebUtility.HtmlEncode(metodo)}</td></tr>");
            if (!string.IsNullOrEmpty(codigo))
            {
                sb.AppendLine($"<tr><th>Código</th><td>{WebUtility.HtmlEncode(codigo)}</td></tr>");
            }
            sb.AppendLine("</table>");
            sb.AppendLine("<script>window.onload = function() { window.print(); }</script>");
            sb.AppendLine("</body>");
            sb.AppendLine("</html>");

            return Encoding.UTF8.GetBytes(sb.ToString());
        }

        public static byte[] GenerarComprobanteReserva(string codigoReserva, string dniCliente, string espacio, DateTime fechaHora, decimal montoTotal, decimal adelanto, decimal saldo)
        {
            var sb = new StringBuilder();
            sb.AppendLine("<html>");
            sb.AppendLine("<head><style>");
            sb.AppendLine("body { font-family: Arial, sans-serif; padding: 20px; }");
            sb.AppendLine("table { border-collapse: collapse; width: 100%; max-width: 600px; margin: auto; }");
            sb.AppendLine("th, td { border: 1px solid #ddd; padding: 8px; text-align: left; }");
            sb.AppendLine("th { background-color: #f2f2f2; }");
            sb.AppendLine(".header { text-align: center; font-size: 24px; font-weight: bold; margin-bottom: 20px; }");
            sb.AppendLine("</style></head>");
            sb.AppendLine("<body>");
            sb.AppendLine("<div class='header'>COMPROBANTE DE RESERVA</div>");
            sb.AppendLine("<table>");
            sb.AppendLine($"<tr><th>Código Reserva</th><td>{WebUtility.HtmlEncode(codigoReserva)}</td></tr>");
            sb.AppendLine($"<tr><th>Cliente DNI</th><td>{WebUtility.HtmlEncode(dniCliente)}</td></tr>");
            sb.AppendLine($"<tr><th>Espacio</th><td>{WebUtility.HtmlEncode(espacio)}</td></tr>");
            sb.AppendLine($"<tr><th>Fecha y Hora</th><td>{fechaHora:dd/MM/yyyy HH:mm}</td></tr>");
            sb.AppendLine($"<tr><th>Monto Total</th><td>{montoTotal:C}</td></tr>");
            sb.AppendLine($"<tr><th>Adelanto</th><td>{adelanto:C}</td></tr>");
            sb.AppendLine($"<tr><th>Saldo Restante</th><td>{saldo:C}</td></tr>");
            sb.AppendLine("</table>");
            sb.AppendLine("<script>window.onload = function() { window.print(); }</script>");
            sb.AppendLine("</body>");
            sb.AppendLine("</html>");

            return Encoding.UTF8.GetBytes(sb.ToString());
        }
    }
}
