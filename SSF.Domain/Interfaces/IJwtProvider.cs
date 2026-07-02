using SSF.Domain.Entities;

namespace SSF.Domain.Interfaces
{
    public interface IJwtProvider
    {
        // Recibe el usuario, mira su Rol, y devuelve el string del token firmado
        string Generate(Usuario usuario);
    }
}