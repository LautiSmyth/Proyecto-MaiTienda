# MaiTienda — Sistema de Gestión

Sistema de gestión web para tienda de componentes electrónicos y hardware gamer, desarrollado en **ASP.NET Web Forms** con arquitectura en capas y autenticación propia.

> Interfaz oscura con tipografía técnica (Orbitron + Exo 2), paleta cielo eléctrico/negro, estética de panel de control gamer.

---

## Tabla de contenidos

- [Tecnologías](#tecnologías)
- [Arquitectura](#arquitectura)
- [Estructura del proyecto](#estructura-del-proyecto)
- [Base de datos](#base-de-datos)
- [Funcionalidades implementadas](#funcionalidades-implementadas)
- [Seguridad](#seguridad)
- [Flujo de la aplicación](#flujo-de-la-aplicación)
- [Instalación y configuración](#instalación-y-configuración)
- [Módulos planificados](#módulos-planificados)

---

## Tecnologías

| Capa | Tecnología |
|---|---|
| Frontend | ASP.NET Web Forms 4.7.2, Bootstrap 3.4.1, jQuery 3.7.0 |
| Backend | C# .NET Framework 4.7.2 |
| Base de datos | SQL Server (ADO.NET puro, sin ORM) |
| Tipografía | Orbitron, Exo 2 (Google Fonts) |
| IDE | Visual Studio 2022 |

---

## Arquitectura

El proyecto implementa una arquitectura en **4 capas** con una capa de servicios transversales:

```
┌──────────────────────────────────────┐
│   Presentación  (Default / Respuesta)│  Web Forms ASPX
├──────────────────────────────────────┤
│   BLL — Business Logic Layer         │  Reglas de negocio
├──────────────────────────────────────┤
│   DAL — Data Access Layer            │  ADO.NET, SQL Server
├──────────────────────────────────────┤
│   BE  — Business Entities            │  Entidades y enums
└──────────────────────────────────────┘
              │
    ┌─────────┴──────────┐
    │      SERVICIOS      │
    │  Encriptador        │  PBKDF2 / SHA-256
    │  SessionManager     │  Singleton de sesión
    │  ServicioBitacora   │  Registro de eventos
    └────────────────────┘
```

---

## Estructura del proyecto

```
Proyecto-MaiTienda/
│
├── BE/
│   ├── BEUsuario.cs                 # Entidad usuario (Id, Nombre, Password, Estado, Perfil, IntentosFallidos)
│   ├── BEBitacora.cs                # Entidad bitácora
│   └── Enums/
│       ├── EstadoUsuario.cs         # Activo=1 | Bloqueado=2 | Inactivo=3
│       └── Perfil.cs                # WebMaster=1 | Administrador=2 | Cliente=3
│
├── BLL/
│   └── BLLUsuario.cs                # ValidarCredenciales, CerrarSesion, Insertar, ActualizarEstado
│
├── DAL/
│   ├── Acceso.cs                    # Singleton de conexión — Leer() y Escribir()
│   ├── DALUsuario.cs                # BuscarPorNombreUsuario, Insertar, Actualizar
│   └── DALBitacora.cs               # RegistrarEvento
│
├── SERVICIOS/
│   ├── Encriptador.cs               # Hash y verificación PBKDF2/SHA-256
│   ├── SessionManager.cs            # Singleton — Login / Logout
│   └── ServicioBitacora.cs          # Registro de acciones en bitácora
│
└── Proyecto MaiTienda/              # Proyecto web
    ├── Default.aspx                 # Login — interfaz oscura gamer
    ├── Default.aspx.cs
    ├── Respuesta.aspx               # Panel principal post-login
    ├── Respuesta.aspx.cs
    ├── Site.master                  # Master page Bootstrap 3
    ├── Content/
    │   ├── bootstrap.css
    │   └── Site.css
    ├── Scripts/
    │   ├── jquery-3.7.0.min.js
    │   └── bootstrap.min.js
    └── Web.config
```

---

## Base de datos

**Nombre:** `MaiTiendaDB`  
**Motor:** SQL Server  
**Cadena de conexión:**
```xml
<add name="DefaultConnection"
     connectionString="Data Source=.;Initial Catalog=MaiTiendaDB;Integrated Security=True;" />
```

### Tabla `Usuario`

| Columna | Tipo | Descripción |
|---|---|---|
| `IdUsuario` | INT PK IDENTITY | Identificador único |
| `NombreUsuario` | VARCHAR(50) | Nombre de ingreso |
| `Password` | VARCHAR(200) | Hash PBKDF2/SHA-256 |
| `Estado` | INT | 1 Activo · 2 Bloqueado · 3 Inactivo |
| `Perfil` | INT | 1 WebMaster · 2 Administrador · 3 Cliente |
| `IntentosFallidos` | INT | Contador de fallos (se bloquea al llegar a 3) |

### Tabla `Bitacora`

| Columna | Tipo | Descripción |
|---|---|---|
| `IdBitacora` | INT PK IDENTITY | Identificador único |
| `IdUsuario` | INT | Referencia al usuario |
| `NombreUsuario` | VARCHAR(50) | Nombre en el momento del evento |
| `Perfil` | VARCHAR(50) | Perfil en el momento del evento |
| `Accion` | VARCHAR(200) | Descripción del evento |
| `Fecha` | DATETIME | Timestamp del evento |
| `DVH` | NVARCHAR(255) | Dígito verificador de la fila (ver [Seguridad](#seguridad)) |

### Tabla `Pedido`

| Columna | Tipo | Descripción |
|---|---|---|
| `IdPedido` | INT PK IDENTITY | Identificador único |
| `IdUsuario` | INT FK | Usuario que compró |
| `Fecha` | DATETIME | Momento de la compra |
| `NombreCliente` / `DocumentoCliente` / `DireccionEnvio` | NVARCHAR | Datos ingresados al comprar |
| `DistanciaKm` | DECIMAL(9,2) | Distancia al negocio calculada en el servidor |
| `Subtotal` / `Envio` / `Total` | DECIMAL(18,2) | Monto de la compra, con el envío calculado en el servidor |
| `DVH` | NVARCHAR(255) | Dígito verificador de la fila |

### Tabla `LineaPedido`

| Columna | Tipo | Descripción |
|---|---|---|
| `IdLineaPedido` | INT PK IDENTITY | Identificador único |
| `IdPedido` | INT FK | Pedido al que pertenece |
| `IdProducto` | INT FK | Producto comprado |
| `Nombre` / `PrecioUnitario` | NVARCHAR / DECIMAL(18,2) | Nombre y precio del producto al momento de la compra |
| `Cantidad` | INT | Unidades compradas |

### Tabla `Factura`

| Columna | Tipo | Descripción |
|---|---|---|
| `IdFactura` | INT PK IDENTITY | Identificador único |
| `Numero` | INT UNIQUE | Número correlativo, sin saltos (se toma dentro de la transacción de la compra) |
| `IdPedido` | INT FK UNIQUE | Pedido facturado (una factura por pedido) |
| `Fecha` | DATETIME | Fecha de emisión |
| `Neto` / `IVA` / `Total` | DECIMAL(18,2) | Importe neto gravado, IVA 21% y total |
| `DVH` | NVARCHAR(255) | Dígito verificador de la fila |

### Tabla `Pago`

| Columna | Tipo | Descripción |
|---|---|---|
| `IdPago` | INT PK IDENTITY | Identificador único |
| `IdPedido` | INT FK UNIQUE | Pedido pagado (un pago por pedido) |
| `Medio` | INT | 1 Efectivo · 2 Tarjeta de débito · 3 Tarjeta de crédito · 4 Mercado Pago (simulado) |
| `Estado` | INT | 1 Pendiente de cobro · 2 Aprobado |
| `Referencia` | NVARCHAR(40) | Código del pago simulado (`SIM-XXXXXXXXXX`); vacío en efectivo |
| `Fecha` | DATETIME | Fecha del pedido |
| `Firma` | NVARCHAR(MAX) | Imagen PNG (Base64) de la firma del recuadro; vacía en efectivo |

Las tablas `Pedido`, `LineaPedido`, `Factura` y `Pago` también las crea la aplicación si no existen (`DALPedido.AsegurarEsquema`, invocado por la verificación de integridad), así que una base creada con el `db.sql` anterior se actualiza sola.

---

## Funcionalidades implementadas

### Autenticación
- Login con usuario y contraseña
- Validación de campos con `RequiredFieldValidator`
- Verificación de estado: solo usuarios **Activos** pueden ingresar
- Bloqueo automático tras **3 intentos fallidos consecutivos** (estado pasa a `Bloqueado`)
- Registro en bitácora de cada intento fallido y del login exitoso

### Gestión de sesión
- Sesión de ASP.NET por visitante: `Session["nombreUsuario"]` y `Session["perfil"]`, sin estado compartido entre usuarios
- `BLLUsuario.ValidarCredenciales(...)` devuelve el `BEUsuario` validado; cada página busca su propio usuario con `BLLUsuario.BuscarPorNombreUsuario(Session["nombreUsuario"])`
- Cierre de sesión registrado en bitácora vía `BLLUsuario.CerrarSesion(usuario)`, a nombre del usuario que cierra la sesión
- Limpieza completa: `Session.Clear()` + `Session.Abandon()`
- Protección de páginas: `Bienvenida.aspx` redirige al login si no hay sesión activa

### Panel principal
- Barra superior con nombre del sistema, usuario autenticado, perfil y botón *Cerrar sesión*
- Saludo personalizado: `Bienvenido {NombreUsuario}`
- Badge de perfil y indicador de sesión activa (punto verde animado)
- Grilla de módulos con estado *Activo* / *Próximamente*

### Bitácora
- Registro automático de: inicio de sesión, cierre de sesión, intentos fallidos y bloqueo por intentos
- Soporte para registro con usuario de sesión o usuario explícito (`ServicioBitacora`)
- El WebMaster ve en `Bitacora.aspx` las cuentas bloqueadas por intentos fallidos y puede desbloquearlas (`BLLUsuario.DesbloquearUsuario`); queda registrado en la bitácora

### Backup
- Automático cada 24 h (`Global.asax`) y manual desde `Backup.aspx`
- Se conservan los últimos **N** backups (`BackupMaximoArchivos` en `Web.config`, por defecto 7); los más viejos se borran solos (`BLLBackup`)
- Si falla el backup automático, el error queda registrado en la bitácora en vez de perderse en silencio

### Compra y pedidos
- *Finalizar compra* exige sesión iniciada y pide nombre, DNI o CUIT y dirección de entrega
- El costo de envío lo calcula el servidor (`BLLEnvio`) a partir de la ubicación; el navegador solo lo muestra
- La compra queda guardada como pedido con sus ítems y el monto (`BLLPedido.FinalizarCompra`)
- En una sola transacción se guarda el pedido, se emite la factura, se descuenta el stock, se recalculan el DVH del producto y el DVV de `Producto`, y se vacía el carrito
- *Mis compras* muestra el historial de pedidos del usuario y enlaza cada factura

### Pago (simulado)
- Al finalizar la compra se elige el medio de pago: efectivo, tarjeta de débito, tarjeta de crédito o Mercado Pago (simulado)
- **Efectivo:** no lleva firma; el pago queda *pendiente de cobro* hasta la entrega
- **Medios electrónicos:** el cliente firma en el recuadro de firma (canvas) para autorizar el pago; el pago queda *aprobado* con una referencia `SIM-…` y la firma se guarda con el pedido
- No hay ningún servicio externo, cuenta ni costo: la aprobación es una simulación (`BLLPago`) y no se cobra nada ni se piden datos de tarjeta
- La factura muestra la forma de pago, el estado, la referencia y la firma del cliente; *Mis compras* lista el medio y el estado

### Factura
- Cada compra emite una factura B con numeración correlativa (`0001-00000001`, `0001-00000002`, …)
- Los precios del catálogo ya incluyen IVA: la factura discrimina el neto y el IVA 21% sobre el total (`BLLFactura`)
- `Factura.aspx?pedido=N` muestra el comprobante con los datos del cliente, el detalle y los totales, y se puede imprimir; solo lo ve el dueño del pedido, el Administrador o el WebMaster
- Los datos del emisor (razón social, CUIT, domicilio) son valores de ejemplo en `BLLFactura`

---

## Seguridad

| Mecanismo | Detalle |
|---|---|
| Hash de contraseñas | PBKDF2 + SHA-256, salt de 16 bytes aleatorio, 100.000 iteraciones |
| Bloqueo por intentos | Tras 3 intentos fallidos el usuario queda en estado `Bloqueado` |
| Protección XSRF | Token anti-falsificación implementado en `Site.master.cs` |
| Control de estado | Solo estado `Activo` permite acceder; `Bloqueado` e `Inactivo` son rechazados |
| Protección de páginas | Verificación de sesión en `Page_Load` de cada página protegida |
| Sin Identity/OWIN | Autenticación 100% propia, sin dependencias externas de identidad |
| Passkey de emergencia | Tras 3 intentos fallidos el origen queda bloqueado 15 minutos; los accesos y los intentos quedan en la bitácora. En el login se usa con el usuario `WebMaster` |
| Desbloqueo de cuentas | El WebMaster puede reactivar una cuenta bloqueada por intentos fallidos desde `Bitacora.aspx` |
| Dígitos verificadores (DVH/DVV) | Usuario y Producto, y también Bitácora, Pedido y Factura: cada fila tiene su DVH y cada tabla su DVV; se verifican y se pueden recalcular desde `GestionIntegridad.aspx` / `Backup.aspx`. Solo Usuario y Producto bloquean todo el sitio si fallan |

---

## Flujo de la aplicación

```
[Default.aspx — Login]
        │
        │  BLLUsuario.ValidarCredenciales()
        │  ├── Usuario no existe o inactivo   → UnauthorizedAccessException
        │  ├── Password incorrecto
        │  │     ├── IntentosFallidos < 3     → incrementa contador + bitácora
        │  │     └── IntentosFallidos >= 3    → estado = Bloqueado + bitácora
        │  └── Password correcto
        │         ├── SessionManager.Login()
        │         ├── IntentosFallidos = 0
        │         ├── Bitácora: "Inicio de sesion"
        │         └── Session["nombreUsuario"] / Session["perfil"]
        │
        ▼
[Respuesta.aspx — Panel principal]
        │
        │  (página protegida — redirige si no hay sesión)
        │
        └── Clic en "Cerrar sesión"
                  │  BLLUsuario.CerrarSesion()
                  │  ├── Bitácora: "Cierre de sesión"
                  │  └── SessionManager.Logout()
                  │  Session.Clear() + Session.Abandon()
                  ▼
         [Default.aspx — Login limpio]
```

---

## Instalación y configuración

### Requisitos previos
- Visual Studio 2022
- SQL Server (local o remoto)
- .NET Framework 4.7.2

### Pasos

**1. Clonar el repositorio**
```bash
git clone https://github.com/LautiSmyth/Proyecto-MaiTienda.git
```

**2. Crear la base de datos**
```sql
CREATE DATABASE MaiTiendaDB;
USE MaiTiendaDB;

CREATE TABLE Usuario (
    IdUsuario        INT          PRIMARY KEY IDENTITY(1,1),
    NombreUsuario    VARCHAR(50)  NOT NULL,
    Password         VARCHAR(200) NOT NULL,
    Estado           INT          NOT NULL DEFAULT 1,
    Perfil           INT          NOT NULL,
    IntentosFallidos INT          NOT NULL DEFAULT 0
);

CREATE TABLE Bitacora (
    IdBitacora    INT          PRIMARY KEY IDENTITY(1,1),
    IdUsuario     INT          NOT NULL,
    NombreUsuario VARCHAR(50)  NOT NULL,
    Perfil        VARCHAR(50)  NOT NULL,
    Accion        VARCHAR(200) NOT NULL,
    Fecha         DATETIME     NOT NULL
);
```

**3. Configurar la cadena de conexión** en `GUI/ConnectionStrings.config` (no en `Web.config`: ese archivo solo apunta a este con `configSource`, así cada uno tiene la suya sin pisar la de los demás):
```xml
<add name="DefaultConnection"
     connectionString="Data Source=TU_SERVIDOR;Initial Catalog=MaiTiendaDB;Integrated Security=True;" />
```
Después de editarlo, corré una sola vez (en una terminal, parados en la carpeta del repo):
```bash
git update-index --skip-worktree GUI/ConnectionStrings.config
```
Así git deja de marcar ese archivo como modificado y nunca más se sube por error. Si en algún momento SÍ querés subir un cambio a propósito (por ejemplo, cambiar el valor por defecto para todos), primero hay que deshacerlo con `git update-index --no-skip-worktree GUI/ConnectionStrings.config`.

### Conexión local
`GUI/ConnectionStrings.config` queda en el repositorio con un valor por defecto (`(localdb)\MSSQLLocalDB`) para que el proyecto ande apenas se clona. Si tu SQL Server se llama distinto, cambiá solo ese archivo (paso 3 de arriba) y nunca `Web.config`.

**4. Abrir la solución** `Proyecto MaiTienda.sln` en Visual Studio.

**5. Restaurar paquetes NuGet** — clic derecho sobre la solución → *Restaurar paquetes NuGet*.

**6. Compilar y ejecutar** con F5 o IIS Express.

> **Nota:** el primer usuario debe insertarse con la contraseña ya hasheada. Podés usar `Encriptador.Hash("tu_password")` desde un endpoint temporal o desde la consola del depurador en Visual Studio.

---

## Módulos planificados

- [x] Autenticación con bloqueo por intentos
- [x] Bitácora de eventos
- [x] Panel principal con grilla de módulos
- [ ] Componentes — catálogo de CPU, GPU, RAM, placas, periféricos
- [ ] Clientes — registro y seguimiento de compradores
- [ ] Pedidos — control de ventas y estado de envíos
- [ ] Stock — inventario y alertas de reposición
- [ ] Reportes — estadísticas y análisis de ventas

---

## Autor

**Lautaro Smyth**  
[github.com/LautiSmyth](https://github.com/LautiSmyth)
