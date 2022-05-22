using Microsoft.AspNetCore.Mvc;
using PrimS.Telnet;

namespace KeeneticVpnMaster.Controllers;

[ApiController]
[Route("vpn/{mac}/[action]")]
public class VpnController : ControllerBase
{
    private readonly Configuration _configuration;

    public VpnController(Configuration configuration)
    {
        _configuration = configuration;
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
        var formattedMac = $"{mac[..2]}:{mac[2..4]}:{mac[4..6]}:{mac[6..8]}:{mac[8..10]}:{mac[10..12]}:";
        var prefix = isOn ? string.Empty : "no ";
        var command1 = $"ip hotspot host {formattedMac} permit";
        var command2 = $"{prefix}ip hotspot host {formattedMac} policy {_configuration.VpnPolicyName}";
        var command3 = "system configuration save";

        using var telnetClient = new Client(_configuration.Address, _configuration.Port, ct);
        await telnetClient.TryLoginAsync(_configuration.Login, _configuration.Password, 5000);

        await telnetClient.WriteLine(command1);
        await telnetClient.WriteLine(command2);
        await telnetClient.WriteLine(command3);
    }
}
