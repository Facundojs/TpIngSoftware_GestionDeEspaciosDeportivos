using DAL.Factory;
using Service.Logic;
using System;
using System.Linq;
using System.Threading;

namespace BLL
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
                var balanceService = new BalanceService();
                balanceService.CalcularSaldoMensual();
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
    }
}
