using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace TpIngSoftware_GestionDeEspaciosDeportivos
{
    static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            // Iniciar el servicio de cron job para deudas mensuales
            try
            {
                DAL.Logic.SchedulerService.Instance.Start();
            }
            catch (Exception)
            {
                // Silently fail or log to event viewer if needed, to not block UI
            }

            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(new FrmLogin());
        }
    }
}
