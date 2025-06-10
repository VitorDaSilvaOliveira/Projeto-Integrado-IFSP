using Microsoft.AspNetCore.Identity;

namespace Estoque.Infrastructure.Services;

public class IdentityErrorDescriberPtBr : IdentityErrorDescriber
{
    public override IdentityError PasswordTooShort(int length) =>
        new() { Code = nameof(PasswordTooShort), Description = $"A senha deve ter no mínimo {length} caracteres." };

    public override IdentityError PasswordRequiresNonAlphanumeric() =>
        new()
        {
            Code = nameof(PasswordRequiresNonAlphanumeric),
            Description = "A senha deve conter ao menos um caractere não alfanumérico."
        };

    public override IdentityError PasswordRequiresDigit() =>
        new()
        {
            Code = nameof(PasswordRequiresDigit), Description = "A senha deve conter ao menos um dígito ('0'-'9')."
        };

    public override IdentityError PasswordRequiresUpper() =>
        new()
        {
            Code = nameof(PasswordRequiresUpper),
            Description = "A senha deve conter ao menos uma letra maiúscula ('A'-'Z')."
        };
}