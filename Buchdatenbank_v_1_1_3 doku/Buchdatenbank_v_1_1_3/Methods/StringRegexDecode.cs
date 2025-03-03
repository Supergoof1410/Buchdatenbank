using System.Text.RegularExpressions;
using System.Web;

namespace Buchdatenbank;

internal static class StringRegexDecode
{
    // Funktion damit überflüssige Leerzeilen entfernt werden. Die Funktion Decode
    // ist dafür zuständig die Umlaute und andere Sonderzeichen in dem Html-Code in 
    // lesbare Schrift umzuwandeln.
    internal static string DecodeHtml(string RegexConvertFormat)
    {
        string? regexResult = Regex.Replace(RegexConvertFormat, @"^\s*$[\r\n]*", string.Empty, RegexOptions.Multiline);
        var htmlDecode = HttpUtility.HtmlDecode(regexResult);
        return htmlDecode;
    }
}
