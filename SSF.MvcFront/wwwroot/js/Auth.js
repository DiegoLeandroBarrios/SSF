const AuthManager = {
    login: async (email, password) => {
        // Quitamos el try-catch interno para que el error suba a la vista
        const response = await fetch('/Auth/ProcesarLogin', {
            method: 'POST',
            headers: { 'Content-Type': 'application/json' },
            body: JSON.stringify({ email, password })
        });

        const data = await response.json();

        if (!response.ok) {
            // Lanzamos el error con el mensaje de tu controlador
            throw new Error(data.mensaje || 'Error al iniciar sesión');
        }

        return data; // Retorna { redireccion: "/Home/Index" }
    },

    register: async (username, email, password, confirmPassword, roleName) => {
        const roleId = roleName === "Administrador" ? 1 : 2;

        const response = await fetch('/Auth/ProcesarRegistro', {
            method: 'POST',
            headers: { 'Content-Type': 'application/json' },
            body: JSON.stringify({
                username,
                email,
                password,
                confirmPassword,
                rolId: roleId
            })
        });

        const data = await response.json();

        if (!response.ok) {
            throw new Error(data.mensaje || 'Error al registrar');
        }

        return data; // Retorna { mensaje: "..." }
    }
};