using System;
using System.ComponentModel.DataAnnotations;

namespace UserManagement.Web.Models.Users;

public class AddUserViewModel
{
    public long Id { get; set; }

    [StringLength(100, MinimumLength = 2, ErrorMessage = "_defaultErrorMessage")]
    [Required(ErrorMessage = "Forename is required.")]
    public string Forename { get; set; } = default!;

    [StringLength(100, MinimumLength = 1, ErrorMessage = "_defaultErrorMessage")]
    [Required(ErrorMessage = "Surname is required.")]
    public string Surname { get; set; } = default!;

    [Required(ErrorMessage = "Email is required.")]
    [EmailAddress]
    public string Email { get; set; } = default!;

    [Required(ErrorMessage = "Is active must be either true or false")]
    public bool IsActive { get; set; }

    // TODO: add date validation for minimum user age, no future dates, etc
    [Required(ErrorMessage = "Date of Birth is required.")]
    [DataType(DataType.Date, ErrorMessage = "Date of Birth must be a date")]
    public DateTime DateOfBirth { get; set; } = default!;
}
