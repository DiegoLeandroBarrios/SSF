using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SSF.Domain.Interfaces
{
    public interface IPasswordHasher
    {
        // Toma la contraseña en texto plano y devuelve el chorizo encriptado
        string HashPassword(string password);

        // Compara el texto plano contra el hash de la base de datos
        bool VerifyPassword(string password, string hashedPassword);
    }
}
