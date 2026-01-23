using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Service;
using Service.Domain;
using Service.Impl;
using Service.Impl.SqlServer;

namespace TpIngSoftware_GestionDeEspaciosDeportivos
{
    public partial class Form1: Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            TestLogRepository();
        }

        public void TestLogRepository()
        {
            var logRepo = new LogRepository();
            Console.WriteLine("--- Iniciando Test de LogRepository ---");

            try
            {
                // 1. Insertar logs de prueba (generamos 5 para testear paginación)
                Console.WriteLine("Insertando logs de prueba...");
                for (int i = 1; i <= 5; i++)
                {
                    logRepo.Info($"Mensaje de prueba informativo #{i}");
                    if (i % 2 == 0) // Insertamos un error cada tanto
                    {
                        logRepo.Error($"Mensaje de error de prueba #{i}", new Exception("Excepción simulada"));
                    }
                }
                Console.WriteLine("[OK] Logs insertados correctamente.");

                // 2. Testear Paginación - Página 1
                int pageSize = 10;
                var pagina1 = logRepo.GetLogs(1, pageSize);

                Console.WriteLine($"--- Verificando Página 1 (Size: {pageSize}) ---");
                foreach (var log in pagina1)
                {
                    Console.WriteLine($"[{log.Timestamp:HH:mm:ss}] {log.LogLevel}: {log.Message}...");
                }

                if (pagina1.Count == pageSize)
                {
                    Console.WriteLine("[OK] Página 1 devolvió la cantidad correcta de registros.");
                }
                else
                {
                    Console.WriteLine($"[ERROR] Se esperaban {pageSize} registros pero se obtuvieron {pagina1.Count}.");
                }

                Console.WriteLine("--- TEST DE LOGS FINALIZADO EXITOSAMENTE ---");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"--- TEST FALLIDO: {ex.Message} ---");
                Console.WriteLine(ex.StackTrace);
            }
        }
    }
}
