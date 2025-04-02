using FileProcessor.Application.CommandHandlers;
using FileProcessor.Core.Models;
using FileProcessor.Core.Interfaces;
using FileProcessor.Infrastructure.FileParsers;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;
using FileProcessor.Core.Services;
using FileProcessor.Infrastructure.Data.Prosa;
using FileProcessor.Core.Interfaces.Prosa;


// Configuración
var configuration = new ConfigurationBuilder()
    .SetBasePath(Directory.GetCurrentDirectory())
    .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
    .Build();

// Configuración del servicio de DI
var services = new ServiceCollection();

// Configurar logging
services.AddLogging(configure =>
    configure.AddConsole()
            .AddConfiguration(configuration.GetSection("Logging"))
            .SetMinimumLevel(LogLevel.Information));

services.AddSingleton<IConfiguration>(configuration);
services.AddTransient<IArchivosProcesadosRepository, ArchivosProcesadosRepository>();
services.AddTransient<IDetailEMVRepository, DetailEMVRepository>();
services.AddTransient<IDetailRepository, DetailRepository>();
services.AddTransient<IHeaderRepository, HeaderRepository>();
services.AddTransient<ITrailerRepository, TrailerRepository>();

services.AddTransient<ProsaPosteoService>();

// Configuración
services.Configure<FileProcessorConfig>(configuration.GetSection("FileProcessorConfig"));

// Registrar parsers
services.AddTransient<IFileParser, TextFileParser>();
services.AddTransient<IFileParser, CsvFileParser>();
services.AddTransient<IFileParser, ExcelFileParser>();
services.AddTransient<ProsaTextFileParser>();
services.AddTransient<TextFileParser>();

// Registrar factory y command handler
services.AddSingleton<FileParserFactory>();
services.AddTransient<ProcessFileCommandHandler>();

// Construir el proveedor de servicios
var serviceProvider = services.BuildServiceProvider();

// Obtener logger
var logger = serviceProvider.GetRequiredService<ILogger<Program>>();
var config = serviceProvider.GetRequiredService<IOptions<FileProcessorConfig>>().Value;

logger.LogInformation("File Processor Service started");
logger.LogInformation($"Scan interval: {config.ScanIntervalMinutes} minutes");
logger.LogInformation($"Max parallel processes: {config.MaxParallelProcesses}");

try
{
    var handler = serviceProvider.GetRequiredService<ProcessFileCommandHandler>();

    // Ejecución continua
    while (true)
    {
        var nextScan = DateTime.Now.AddMinutes(config.ScanIntervalMinutes);

        await handler.ProcessAllDirectories();

        logger.LogInformation($"Next scan at: {nextScan}");
        await Task.Delay(nextScan - DateTime.Now);
    }
}
catch (Exception ex)
{
    logger.LogCritical(ex, "Service stopped due to error");
}
finally
{
    logger.LogInformation("File Processor Service stopped");
    Console.ReadKey();
}