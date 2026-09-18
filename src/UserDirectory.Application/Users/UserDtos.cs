using UserDirectory.Application.Common;
using System.ComponentModel.DataAnnotations;

namespace UserDirectory.Application.Users;

public sealed record CreateUserRequest
{
    [Required]
    [NotWhiteSpace]
    [StringLength(100, MinimumLength = 2)]
    public required string Name { get; init; }

    [Required]
    [Range(0, 120)]
    public required int? Age { get; init; }

    [Required]
    [NotWhiteSpace]
    public required string City { get; init; }

    [Required]
    [NotWhiteSpace]
    public required string State { get; init; }

    [Required]
    [NotWhiteSpace]
    [StringLength(10, MinimumLength = 4)]
    public required string Pincode { get; init; }
}

//public sealed record CreateUserRequest(
//    [property: Required, NotWhiteSpace, StringLength(100, MinimumLength = 2)] string Name,
//    [property: Required, Range(0, 120)] int? Age,
//    [property: Required, NotWhiteSpace] string City,
//    [property: Required, NotWhiteSpace] string State,
//    [property: Required, NotWhiteSpace, StringLength(10, MinimumLength = 4)] string Pincode);

public sealed record UpdateUserRequest(
    [property: Required, NotWhiteSpace, StringLength(100, MinimumLength = 2)] string Name,
    [property: Required, Range(0, 120)] int? Age,
    [property: Required, NotWhiteSpace] string City,
    [property: Required, NotWhiteSpace] string State,
    [property: Required, NotWhiteSpace, StringLength(10, MinimumLength = 4)] string Pincode);

public sealed record UserResponse(
    int Id,
    string Name,
    int Age,
    string City,
    string State,
    string Pincode);
