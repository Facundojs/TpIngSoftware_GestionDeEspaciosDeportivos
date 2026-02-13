using DAL.Factory;
using Service.Logic;
using System;
using System.Linq;
using System.Threading;

namespace DAL.Logic
{
    public class SchedulerService
    {
        private static SchedulerService _instance;
        private Timer _timer;
        private readonly object _lock = new object();
        private bool _isRunning = false;

        private SchedulerService() { }

        public static SchedulerService Instance
        {
            get
            {
                if (_instance == null)
                {
                    _instance = new SchedulerService();
                }
                return _instance;
            }
        }

        public void Start()
        {
            // Initial delay 5 seconds, then check every 1 hour
            if (_timer == null)
            {
                _timer = new Timer(CheckAndRun, null, 5000, 3600000);
            }
        }

        private void CheckAndRun(object state)
        {
            if (_isRunning) return;

            lock (_lock)
            {
                if (_isRunning) return;
                _isRunning = true;
            }

            try
            {
                int currentMonth = DateTime.Now.Month;
                int currentYear = DateTime.Now.Year;

                // Check if monthly debt calculation has already run for this month via Bitacora
                bool jobRan = CheckIfJobRanInBitacora(currentMonth, currentYear);

                if (!jobRan)
                {
                    var balanceService = new BalanceService();
                    balanceService.CalcularSaldoMensual();
                }
            }
            catch (Exception ex)
            {
                // Log error via BitacoraService
                 var bitacora = new BitacoraService();
                 bitacora.Log($"Error in SchedulerService: {ex.Message}", "ERROR", ex);
            }
            finally
            {
                _isRunning = false;
            }
        }

        private bool CheckIfJobRanInBitacora(int month, int year)
        {
            try
            {
                var bitacora = new BitacoraService();
                var start = new DateTime(year, month, 1);
                var end = start.AddMonths(1).AddSeconds(-1);

                // Search for the success message logged by BalanceService
                var logs = bitacora.GetLogs(1, 1, start, end, "INFO", "Job CalcularSaldoMensual finalizado");
                return logs != null && logs.Any();
            }
            catch
            {
                return false;
            }
        }
    }
}
