using Microsoft.Extensions.DependencyInjection;

namespace Auction.Services
{
    public class WorkerService: BackgroundService

    {
        private readonly IServiceScopeFactory _serviceScopeFactory;

        public WorkerService(IServiceScopeFactory serviceScopeFactory)
        {
            _serviceScopeFactory = serviceScopeFactory;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                using (var scope = _serviceScopeFactory.CreateScope())
                {
                    var check = scope.ServiceProvider.GetRequiredService<ICheckService>();
                    check.checkStart();
                }

                Console.WriteLine("test");
                await Task.Delay(TimeSpan.FromSeconds(Convert.ToDouble(3)),
                    stoppingToken);
            }
        }
    }
}
