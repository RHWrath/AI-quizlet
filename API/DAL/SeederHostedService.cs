using AIQuizlet.Seeding;

namespace AIQuizlet.Services
{
    public class SeederHostedService : IHostedService
    {
        private readonly IServiceProvider _serviceProvider;
        private readonly ILogger<SeederHostedService> _logger;

        public SeederHostedService(IServiceProvider serviceProvider, ILogger<SeederHostedService> logger)
        {
            _serviceProvider = serviceProvider;
            _logger = logger;
        }

        public async Task StartAsync(CancellationToken cancellationToken)
        {
            _logger.LogInformation("SeederHostedService starting...");

            using var scope = _serviceProvider.CreateScope();
            var seeder = scope.ServiceProvider.GetRequiredService<ImageSeeder>();
            await seeder.SeedAsync(clearExisting: false);

            _logger.LogInformation("SeederHostedService finished.");
        }

        public Task StopAsync(CancellationToken cancellationToken) => Task.CompletedTask;
    }
}
