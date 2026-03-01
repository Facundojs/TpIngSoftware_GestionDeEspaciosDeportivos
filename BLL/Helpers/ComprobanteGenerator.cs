using System;
using System.Text;
using System.Net;

namespace BLL.Helpers
{
    public static class ComprobanteGenerator
    {
        private const string HtmlHeader = @"
            <html>
            <head>
                <style>
                    body { font-family: Arial, sans-serif; padding: 20px; }
                    table { border-collapse: collapse; width: 100%; max-width: 600px; margin: auto; }
                    th, td { border: 1px solid #ddd; padding: 8px; text-align: left; }
                    th { background-color: #f2f2f2; }
                    .header { text-align: center; font-size: 24px; font-weight: bold; margin-bottom: 20px; }
                </style>
            </head>
            <body>";

        private const string HtmlFooter = @"
                <script>window.onload = function() { window.print(); }</script>
            </body>
            </html>";

        private const string TemplatePago = HtmlHeader + @"
            <div class='header'>COMPROBANTE DE PAGO</div>
            <table>
                <tr><th>Fecha</th><td>{{Fecha}}</td></tr>
                <tr><th>Cliente DNI</th><td>{{Dni}}</td></tr>
                <tr><th>Monto</th><td>{{Monto}}</td></tr>
                <tr><th>Método</th><td>{{Metodo}}</td></tr>
                {{CodigoRow}}
            </table>" + HtmlFooter;

        private const string TemplateReserva = HtmlHeader + @"
            <div class='header'>COMPROBANTE DE RESERVA</div>
            <table>
                <tr><th>Código Reserva</th><td>{{CodigoReserva}}</td></tr>
                <tr><th>Cliente DNI</th><td>{{Dni}}</td></tr>
                <tr><th>Espacio</th><td>{{Espacio}}</td></tr>
                <tr><th>Fecha y Hora</th><td>{{FechaHora}}</td></tr>
                <tr><th>Monto Total</th><td>{{MontoTotal}}</td></tr>
                <tr><th>Adelanto</th><td>{{Adelanto}}</td></tr>
                <tr><th>Saldo Restante</th><td>{{Saldo}}</td></tr>
            </table>" + HtmlFooter;

        public static byte[] GenerarComprobantePago(DateTime fecha, string dniCliente, decimal monto, string metodo, string codigo)
        {
            string codigoRow = !string.IsNullOrEmpty(codigo)
                ? $"<tr><th>Código</th><td>{WebUtility.HtmlEncode(codigo)}</td></tr>"
                : string.Empty;

            string html = TemplatePago
                .Replace("{{Fecha}}", fecha.ToString("dd/MM/yyyy HH:mm"))
                .Replace("{{Dni}}", WebUtility.HtmlEncode(dniCliente))
                .Replace("{{Monto}}", monto.ToString("C"))
                .Replace("{{Metodo}}", WebUtility.HtmlEncode(metodo))
                .Replace("{{CodigoRow}}", codigoRow);

            return Encoding.UTF8.GetBytes(html);
        }

        public static byte[] GenerarComprobanteReserva(string codigoReserva, string dniCliente, string espacio, DateTime fechaHora, decimal montoTotal, decimal adelanto, decimal saldo)
        {
            string html = TemplateReserva
                .Replace("{{CodigoReserva}}", WebUtility.HtmlEncode(codigoReserva))
                .Replace("{{Dni}}", WebUtility.HtmlEncode(dniCliente))
                .Replace("{{Espacio}}", WebUtility.HtmlEncode(espacio))
                .Replace("{{FechaHora}}", fechaHora.ToString("dd/MM/yyyy HH:mm"))
                .Replace("{{MontoTotal}}", montoTotal.ToString("C"))
                .Replace("{{Adelanto}}", adelanto.ToString("C"))
                .Replace("{{Saldo}}", saldo.ToString("C"));

            return Encoding.UTF8.GetBytes(html);
        }
    }
}