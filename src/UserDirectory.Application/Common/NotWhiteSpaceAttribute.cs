using System.ComponentModel.DataAnnotations;

namespace UserDirectory.Application.Common;

[AttributeUsage(AttributeTargets.Property | AttributeTargets.Field | AttributeTargets.Parameter)]
public sealed class NotWhiteSpaceAttribute : ValidationAttribute
{
    public override bool IsValid(object? value) => value is string text && !string.IsNullOrWhiteSpace(text);
}
