using System.Globalization;
using System.Text.RegularExpressions;

namespace KeeneticVpnMaster;

public class MacRouteConstraint : IRouteConstraint
{
    public bool Match(HttpContext? httpContext, IRouter? route, string routeKey, RouteValueDictionary values,
        RouteDirection routeDirection)
    {
        if (values.TryGetValue(routeKey, out var rawValue))
        {
            var value = Convert.ToString(rawValue, CultureInfo.InvariantCulture);
            return ValidateMac(value);
        }

        return false;
    }

    private static bool ValidateMac(string? mac)
    {
        if (string.IsNullOrWhiteSpace(mac)) return false;

        mac = mac.ToLowerInvariant().Replace(":", string.Empty).Replace("-", string.Empty);
        return Regex.IsMatch(mac, "^[a-f0-9]{12}$");
    }
}
