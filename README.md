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

---

## Funcionalidades implementadas

### Autenticación
- Login con usuario y contraseña
- Validación de campos con `RequiredFieldValidator`
- Verificación de estado: solo usuarios **Activos** pueden ingresar
- Bloqueo automático tras **3 intentos fallidos consecutivos** (estado pasa a `Bloqueado`)
- Registro en bitácora de cada intento fallido y del login exitoso

### Gestión de sesión
- `SessionManager` Singleton: almacena el `BEUsuario` autenticado en memoria de servidor
- Datos de sesión complementarios en `Session["nombreUsuario"]` y `Session["perfil"]`
- Cierre de sesión registrado en bitácora vía `BLLUsuario.CerrarSesion()`
- Limpieza completa: `Session.Clear()` + `Session.Abandon()`
- Protección de páginas: `Respuesta.aspx` redirige al login si no hay sesión activa

### Panel principal
- Barra superior con nombre del sistema, usuario autenticado, perfil y botón *Cerrar sesión*
- Saludo personalizado: `Bienvenido {NombreUsuario}`
- Badge de perfil y indicador de sesión activa (punto verde animado)
- Grilla de módulos con estado *Activo* / *Próximamente*

### Bitácora
- Registro automático de: inicio de sesión, cierre de sesión, intentos fallidos y bloqueo por intentos
- Soporte para registro con usuario de sesión o usuario explícito (`ServicioBitacora`)

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

**3. Configurar la cadena de conexión** en `Proyecto MaiTienda/Web.config`:
```xml
<add name="DefaultConnection"
     connectionString="Data Source=TU_SERVIDOR;Initial Catalog=MaiTiendaDB;Integrated Security=True;" />
```

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
