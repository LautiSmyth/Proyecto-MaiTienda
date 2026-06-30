<%@ Page Language="C#" AutoEventWireup="true" CodeFile="Mantenimiento.aspx.cs" Inherits="Mantenimiento" %>
<!DOCTYPE html>
<html lang="es">
<head>
    <meta charset="utf-8" />
    <meta name="viewport" content="width=device-width, initial-scale=1.0" />
    <title>Sistema en Mantenimiento</title>
    <style>
        *, *::before, *::after { box-sizing: border-box; margin: 0; padding: 0; }
        body {
            font-family: 'Segoe UI', system-ui, sans-serif;
            background: #0d1117;
            color: #c9d1d9;
            height: 100vh;
            display: flex;
            align-items: center;
            justify-content: center;
        }
        .container {
            text-align: center;
            max-width: 480px;
            padding: 2rem;
        }
        .icon {
            font-size: 4rem;
            margin-bottom: 1.5rem;
            opacity: 0.7;
        }
        h1 {
            font-size: 1.75rem;
            font-weight: 600;
            color: #e6edf3;
            margin-bottom: 1rem;
        }
        p {
            font-size: 1rem;
            color: #8b949e;
            line-height: 1.6;
            margin-bottom: 0.5rem;
        }
        .overlay {
            display: none;
            position: fixed;
            inset: 0;
            background: rgba(0,0,0,0.7);
            align-items: center;
            justify-content: center;
            z-index: 100;
        }
        .overlay.visible { display: flex; }
        .modal {
            background: #161b22;
            border: 1px solid #30363d;
            border-radius: 12px;
            padding: 2rem;
            width: 360px;
            box-shadow: 0 16px 48px rgba(0,0,0,0.5);
        }
        .modal h3 {
            font-size: 1rem;
            color: #e6edf3;
            margin-bottom: 0.4rem;
        }
        .modal p {
            font-size: 0.8rem;
            color: #8b949e;
            margin-bottom: 1.2rem;
        }
        .modal input[type="password"] {
            width: 100%;
            padding: 0.65rem 0.9rem;
            background: #0d1117;
            border: 1px solid #30363d;
            border-radius: 6px;
            color: #e6edf3;
            font-size: 0.95rem;
            outline: none;
            margin-bottom: 0.8rem;
        }
        .modal input[type="password"]:focus { border-color: #58a6ff; }
        .modal-actions { display: flex; gap: 0.6rem; }
        .btn {
            flex: 1;
            padding: 0.6rem;
            border: none;
            border-radius: 6px;
            font-size: 0.9rem;
            cursor: pointer;
            font-weight: 500;
            transition: opacity 0.15s;
        }
        .btn:hover { opacity: 0.85; }
        .btn-primary { background: #238636; color: #fff; }
        .btn-secondary { background: #21262d; color: #c9d1d9; border: 1px solid #30363d; }
        .error-msg {
            display: none;
            background: #3d1f1f;
            border: 1px solid #6e2b2b;
            border-radius: 6px;
            padding: 0.5rem 0.8rem;
            font-size: 0.85rem;
            color: #f85149;
            margin-bottom: 0.8rem;
        }
        .spinner { display: none; font-size: 0.85rem; color: #8b949e; margin-bottom: 0.8rem; }
    </style>
</head>
<body>
    <div class="container">
        <div class="icon">🔒</div>
        <h1>Sistema en mantenimiento</h1>
        <p>El sistema se encuentra temporalmente fuera de servicio.</p>
        <p>Por favor, intente de nuevo más tarde.</p>
    </div>

    <div class="overlay" id="overlay">
        <div class="modal">
            <h3>Acceso de emergencia</h3>
            <p>Ingrese el passkey para acceder al panel de gestión.</p>
            <div class="error-msg" id="errorMsg">Passkey incorrecto. Intente nuevamente.</div>
            <div class="spinner" id="spinner">Verificando...</div>
            <input type="password" id="txtPasskey" placeholder="Passkey de emergencia" autocomplete="off" />
            <div class="modal-actions">
                <button class="btn btn-primary" onclick="verificar()">Verificar</button>
                <button class="btn btn-secondary" onclick="cerrarModal()">Cancelar</button>
            </div>
        </div>
    </div>

    <script src="Scripts/jquery-3.7.0.min.js"></script>
    <script>
        document.addEventListener('keydown', function (e) {
            if (e.ctrlKey && e.shiftKey && e.key === 'K') {
                e.preventDefault();
                abrirModal();
            }
        });

        function abrirModal() {
            document.getElementById('overlay').classList.add('visible');
            document.getElementById('txtPasskey').focus();
            document.getElementById('errorMsg').style.display = 'none';
            document.getElementById('txtPasskey').value = '';
        }

        function cerrarModal() {
            document.getElementById('overlay').classList.remove('visible');
        }

        function verificar() {
            var passkey = document.getElementById('txtPasskey').value;
            if (!passkey) return;

            document.getElementById('errorMsg').style.display = 'none';
            document.getElementById('spinner').style.display = 'block';

            $.ajax({
                type: 'POST',
                url: 'Mantenimiento.aspx/VerificarPasskey',
                data: JSON.stringify({ passkey: passkey }),
                contentType: 'application/json; charset=utf-8',
                dataType: 'json',
                success: function (response) {
                    document.getElementById('spinner').style.display = 'none';
                    if (response.d) {
                        window.location.href = 'GestionIntegridad.aspx';
                    } else {
                        document.getElementById('errorMsg').style.display = 'block';
                    }
                },
                error: function () {
                    document.getElementById('spinner').style.display = 'none';
                    document.getElementById('errorMsg').textContent = 'Error de comunicación. Intente nuevamente.';
                    document.getElementById('errorMsg').style.display = 'block';
                }
            });
        }

        document.addEventListener('keydown', function (e) {
            if (document.getElementById('overlay').classList.contains('visible')) {
                if (e.key === 'Enter') verificar();
                if (e.key === 'Escape') cerrarModal();
            }
        });
    </script>
</body>
</html>
