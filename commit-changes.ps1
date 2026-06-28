Set-Location "C:\Users\lauti\source\repos\LautiSmyth\Proyecto-MaiTienda"

# Commit 1: En-carrito feature
git add BLL/BLLCarrito.cs DAL/DALCarrito.cs GUI/Content/app.css GUI/Default.aspx GUI/Default.aspx.cs
git commit -m "feat: indicador visual 'En carrito' en tarjetas de producto

- DAL/DALCarrito.cs: nuevo metodo ObtenerIdsProductos que devuelve HashSet<int>
- BLL/BLLCarrito.cs: expone ObtenerIdsProductos y SincronizarCarrito
- GUI/Default.aspx.cs: campo _idsEnCarrito + metodo IsEnCarrito() para el Repeater
- GUI/Default.aspx: clase CSS condicional en-carrito, ribbon, boton verde si ya esta en carrito
- GUI/Content/app.css: estilos .en-carrito, .en-carrito-ribbon, .btn-en-carrito
- Para usuarios guest: JS lee localStorage y aplica la clase en-carrito en cliente"

# Commit 2: Bienvenida rename
git add GUI/Bienvenida.aspx GUI/Bienvenida.aspx.cs
git commit -m "rename: Respuesta.aspx renombrado a Bienvenida.aspx

- GUI/Bienvenida.aspx: nueva pagina de bienvenida post-login y post-compra
- GUI/Bienvenida.aspx.cs: code-behind con clase Bienvenida : Page
- Actualizado Response.Redirect en Login.aspx.cs y Carrito.aspx.cs
- Eliminados Respuesta.aspx y Respuesta.aspx.cs"

# Commit 3: Code cleanup
git add BLL/BLLUsuario.cs DAL/Acceso.cs DAL/DALBitacora.cs GUI/App_Code/BundleConfig.cs GUI/Backup.aspx.cs GUI/Carrito.aspx.cs GUI/Default.aspx.cs GUI/Login.aspx.cs GUI/Site.master.cs
git commit -m "refactor: limpieza de codigo en toda la solucion

- Eliminados todos los comentarios (preparacion para impresion)
- Eliminados imports innecesarios (using sin usar)
- DAL/Acceso.cs: removido Debug.WriteLine, catch simplificado
- DAL/DALBitacora.cs: loop simplificado con object initializer inline
- BLL/BLLUsuario.cs: removido metodo ActualizarEstado sin uso
- GUI/Site.master.cs: removido metodo Unnamed_LoggingOut (OWIN sin usar)
- GUI/Login.aspx.cs: imports reducidos al minimo necesario
- GUI/Carrito.aspx.cs: removidos comentarios de desarrollo del bloque Eliminar
- GUI/Backup.aspx.cs: removido using System.Collections.Generic innecesario
- GUI/Default.aspx.cs: removidos doc-comments y comentarios inline
- GUI/App_Code/BundleConfig.cs: removidos comentarios de URL de documentacion"

# Push
git push origin master

Write-Host "Commits y push completados." -ForegroundColor Green
git log --oneline -5
