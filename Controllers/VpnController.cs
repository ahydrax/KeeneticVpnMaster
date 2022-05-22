using Microsoft.AspNetCore.Mvc;
using PrimS.Telnet;

namespace KeeneticVpnMaster.Controllers;

[ApiController]
[Route("vpn/{mac}/[action]")]
public class VpnController : ControllerBase
{
    private readonly Configuration _configuration;
    private readonly ILogger<VpnController> _logger;

    public VpnController(Configuration configuration, ILogger<VpnController> logger)
    {
        _configuration = configuration;
        _logger = logger;
    }

    [HttpPost]
    public async Task<IActionResult> On(string mac, CancellationToken ct)
    {
        await TurnVpn(mac, true, ct);
        return Ok();
    }

    [HttpPost]
    public async Task<IActionResult> Off(string mac, CancellationToken ct)
    {
        await TurnVpn(mac, false, ct);
        return Ok();
    }

    private async Task TurnVpn(string mac, bool isOn, CancellationToken ct)
    {
        _logger.LogInformation("Attempting to turn vpn enabled {state} for {mac}", isOn, mac);
        
        var prefix = isOn ? string.Empty : "no ";
        var postfix = isOn ? _configuration.VpnPolicyName : string.Empty; 
        var command1 = $"ip hotspot host {mac} permit";
        var command2 = $"{prefix}ip hotspot host {mac} policy {postfix}";
        
        using var telnetClient = new Client(_configuration.Address, _configuration.Port, ct);

        //await telnetClient.TerminatedReadAsync(":");
        await telnetClient.TryLoginAsync(_configuration.Login, _configuration.Password, 5000, ":");
        
        
        await telnetClient.TerminatedReadAsync(":");
        await telnetClient.WriteLine(_configuration.Login);
        
        await telnetClient.TerminatedReadAsync(":");
        await telnetClient.WriteLine(_configuration.Password);
        
        await telnetClient.TerminatedReadAsync(">");
        await telnetClient.WriteLine(command1);
        await telnetClient.TerminatedReadAsync(">");
        await telnetClient.WriteLine(command2);
        await telnetClient.TerminatedReadAsync(">");
        await telnetClient.WriteLine("system configuration save");
        await telnetClient.TerminatedReadAsync(">");
        await telnetClient.WriteLine("exit");
    }
}
