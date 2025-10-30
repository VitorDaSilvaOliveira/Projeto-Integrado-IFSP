using System.ComponentModel.DataAnnotations;

namespace Estoque.Domain.Entities;

public class UserSetting
{
    [Key]
    public string UserId { get; set; }
    public bool EnableLanguageSwitch { get; set; } = true;
    public bool EnableDarkModeSwitch { get; set; } = true;
    public string PreferredLanguage { get; set; } = "pt-BR";
    public bool DarkMode { get; set; } = false;
}