using DAL.Factory;
using Service.Logic;
using System;
using System.Linq;
using System.Threading;

namespace BLL
{
    /// <summary>
    /// Singleton background scheduler that periodically triggers <see cref="BalanceService.CalcularSaldoMensual"/>.
    /// </summary>
    /// <remarks>
    /// Uses a double-checked lock to prevent concurrent job executions. The job fires 5 seconds
    /// after <see cref="Start"/> is called, then repeats every hour.
    /// </remarks>
    public class SchedulerService
    {
        private static SchedulerService _instance;
        private Timer _timer;
        private readonly object _lock = new object();
        private bool _isRunning = false;

        private SchedulerService() { }

        /// <summary>Returns the singleton instance, creating it lazily on first access.</summary>
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

        /// <summary>
        /// Starts the background timer if it has not already been started.
        /// Safe to call multiple times; subsequent calls are no-ops.
        /// </summary>
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
