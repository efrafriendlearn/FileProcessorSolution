using FileProcessor.Core.Interfaces;
using FileProcessor.Core.Models;
using FileProcessor.Infrastructure.FileParsers;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace FileProcessor.Application.CommandHandlers
{
    public class ProcessFileCommandHandler
    {
        private readonly FileParserFactory _parserFactory;
        private readonly ILogger<ProcessFileCommandHandler> _logger;
        private readonly FileProcessorConfig _config;

        public ProcessFileCommandHandler(
            FileParserFactory parserFactory,
            ILogger<ProcessFileCommandHandler> logger,
            IOptions<FileProcessorConfig> config)
        {
            _parserFactory = parserFactory;
            _logger = logger;
            _config = config.Value;

            // Crear directorios si no existen
            InitializeDirectories();
        }

        private void InitializeDirectories()
        {
            foreach (var dirConfig in _config.ScanDirectories)
            {
                EnsureDirectoryExists(dirConfig.Path);
                EnsureDirectoryExists(dirConfig.ProcessedDirectory);
                EnsureDirectoryExists(dirConfig.ErrorDirectory);
            }
        }

        private void EnsureDirectoryExists(string path)
        {
            if (!Directory.Exists(path))
            {
                _logger.LogInformation($"Creating directory: {path}");
                Directory.CreateDirectory(path);
            }
        }

        public async Task ProcessAllDirectories()
        {
            _logger.LogInformation("Starting directory scan...");

            var tasks = new List<Task>();
            var semaphore = new SemaphoreSlim(_config.MaxParallelProcesses);

            foreach (var dirConfig in _config.ScanDirectories)
            {
                _logger.LogInformation($"Scanning directory: {dirConfig.Name} ({dirConfig.Path})");

                foreach (var pattern in dirConfig.FilePatterns)
                {
                    var files = Directory.GetFiles(dirConfig.Path, pattern);

                    foreach (var filePath in files)
                    {
                        await semaphore.WaitAsync();
                        tasks.Add(Task.Run(async () =>
                        {
                            try
                            {
                                await ProcessSingleFile(filePath, dirConfig);
                            }
                            finally
                            {
                                semaphore.Release();
                            }
                        }));
                    }
                }
            }

            await Task.WhenAll(tasks);
            _logger.LogInformation("Directory scan completed");
        }

        private async Task ProcessSingleFile(string filePath, ScanDirectoryConfig dirConfig)
        {
            var fileName = Path.GetFileName(filePath);
            _logger.LogInformation($"Processing file: {fileName}");

            try
            {
                var parser = _parserFactory.GetParser(filePath);
                var result = await parser.ParseAsync(filePath);

                if (result.Success)
                {
                    // Mover a procesados
                    var processedPath = Path.Combine(dirConfig.ProcessedDirectory, fileName);
                    File.Move(filePath, processedPath);
                    _logger.LogInformation($"Successfully processed {fileName}. Moved to: {processedPath}");
                }
                else
                {
                    // Mover a errores
                    var errorPath = Path.Combine(dirConfig.ErrorDirectory, fileName);
                    File.Move(filePath, errorPath);
                    _logger.LogError($"Failed to process {fileName}. Moved to: {errorPath}. Error: {result.Message}");
                }
            }
            catch (Exception ex)
            {
                var errorPath = Path.Combine(dirConfig.ErrorDirectory, fileName);
                File.Move(filePath, errorPath);
                _logger.LogError(ex, $"Error processing file {fileName}. Moved to: {errorPath}");
            }
        }
    }
}