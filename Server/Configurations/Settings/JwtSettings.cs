using System.ComponentModel.DataAnnotations;

// ReSharper disable once CheckNamespace
namespace Microsoft.Extensions.Configuration;

public sealed class JwtSettings
{
    public const string SectionName = "Jwt";

    [Required] public string Key { get; set; } = default!;
    [Required] public string Issuer { get; set; } = default!;
    [Required] public string Audience { get; set; } = default!;
}