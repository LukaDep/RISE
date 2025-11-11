using Renci.SshNet;

namespace Rise.Server.Services;

public class SshTunnelService : IHostedService, IDisposable
{
    private readonly ILogger<SshTunnelService> _logger;
    private readonly IConfiguration _configuration;
    private SshClient? _sshClient;
    private ForwardedPortLocal? _forwardedPort;
    private readonly TaskCompletionSource<bool> _readyTaskCompletionSource = new();

    public Task ReadyTask => _readyTaskCompletionSource.Task;

    public SshTunnelService(ILogger<SshTunnelService> logger, IConfiguration configuration)
    {
        _logger = logger;
        _configuration = configuration;
    }

    public async Task StartAsync(CancellationToken cancellationToken)
    {
        try
        {
            var host = _configuration["SshTunnel:Host"] ?? "vichogent.be";
            var port = int.Parse(_configuration["SshTunnel:Port"] ?? "41233");
            var username = _configuration["SshTunnel:Username"] ?? "vicuser";
            var privateKeyPath = _configuration["SshTunnel:PrivateKeyPath"];
            var password = _configuration["SshTunnel:Password"];
            var localPort = uint.Parse(_configuration["SshTunnel:LocalPort"] ?? "3306");
            var remoteHost = _configuration["SshTunnel:RemoteHost"] ?? "localhost";
            var remotePort = uint.Parse(_configuration["SshTunnel:RemotePort"] ?? "3306");

            _logger.LogInformation("Establishing SSH tunnel to {Host}:{Port}", host, port);

            // Use private key if provided, otherwise use password
            if (!string.IsNullOrWhiteSpace(privateKeyPath))
            {
                if (!File.Exists(privateKeyPath))
                {
                    throw new FileNotFoundException($"Private key file not found: {privateKeyPath}");
                }

                var privateKeyFile = new PrivateKeyFile(privateKeyPath, password);
                _sshClient = new SshClient(host, port, username, privateKeyFile);
            }
            else if (!string.IsNullOrWhiteSpace(password))
            {
                _sshClient = new SshClient(host, port, username, password);
            }
            else
            {
                throw new InvalidOperationException("Either Password or PrivateKeyPath must be configured for SSH authentication");
            }
            _sshClient.Connect();

            _forwardedPort = new ForwardedPortLocal("127.0.0.1", localPort, remoteHost, remotePort);
            _sshClient.AddForwardedPort(_forwardedPort);
            _forwardedPort.Start();

            _logger.LogInformation("SSH tunnel established successfully. Local port {LocalPort} forwarded to {RemoteHost}:{RemotePort}",
                localPort, remoteHost, remotePort);

            _readyTaskCompletionSource.TrySetResult(true);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to establish SSH tunnel");
            _readyTaskCompletionSource.TrySetException(ex);
            throw;
        }
    }

    public Task StopAsync(CancellationToken cancellationToken)
    {
        try
        {
            _forwardedPort?.Stop();
            _sshClient?.Disconnect();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error while closing SSH tunnel");
        }

        return Task.CompletedTask;
    }

    public void Dispose()
    {
        _forwardedPort?.Dispose();
        _sshClient?.Dispose();
    }
}
