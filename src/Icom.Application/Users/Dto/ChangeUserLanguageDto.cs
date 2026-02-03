using System.ComponentModel.DataAnnotations;

namespace Icom.Users.Dto;

public class ChangeUserLanguageDto
{
    [Required]
    public string LanguageName { get; set; }
}