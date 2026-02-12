using System;
using System.Timers;
using System.Threading.Tasks;

namespace Service.Logic
{
    public class CronService
    {
        private static Timer _timer;
        private static BalanceService _balanceService;

        public static void Start()
        {
            _balanceService = new BalanceService();

            // Run initially in background to catch up if missed
            Task.Run(() =>
            {
                try
                {
                    // Idempotent call to ensure debt for current month exists
                    _balanceService.CalcularSaldoMensual();
                }
                catch (Exception ex)
                {
                    System.Diagnostics.Debug.WriteLine($"Error in initial BalanceService run: {ex.Message}");
                }
            });

            _timer = new Timer(60000); // Check every minute
            _timer.Elapsed += OnTimedEvent;
            _timer.AutoReset = true;
            _timer.Enabled = true;
        }

        private static void OnTimedEvent(Object source, ElapsedEventArgs e)
        {
            var now = DateTime.Now;
            // Execute at beginning of month 00:00
            // Logic: Day == 1, Hour == 0, Minute == 0
            if (now.Day == 1 && now.Hour == 0 && now.Minute == 0)
            {
                try
                {
                    _balanceService.CalcularSaldoMensual();
                }
                catch (Exception ex)
                {
                    System.Diagnostics.Debug.WriteLine($"Error in scheduled BalanceService run: {ex.Message}");
                }
            }
        }
    }
}
