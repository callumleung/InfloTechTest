using System;
using System.ComponentModel.DataAnnotations;

namespace UserManagement.Web.Models.Users;

public class AddUserViewModel
{
    [Required(ErrorMessage = "Forename is required.")]
    public string Forename { get; set; } = default!;

    [Required(ErrorMessage = "Surname is required.")]
    public string Surname { get; set; } = default!;

    [Required(ErrorMessage = "Email is required.")]
    [EmailAddress]
    public string Email { get; set; } = default!;

    [Required(ErrorMessage = "Date of Birth is required.")]
    [DataType(DataType.Date)]
    public DateTime DateOfBirth { get; set; } = default!;
}
