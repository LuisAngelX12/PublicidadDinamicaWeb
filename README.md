# Publicidad Dinámica Web

**Publicidad Dinámica Web** es una aplicación web desarrollada con **ASP.NET Core MVC** que permite administrar y mostrar publicidad dinámica de productos por comercio.

El sistema permite gestionar usuarios y roles, comercios, categorías, productos, anuncios e historial de precios, además de configurar la apariencia y el comportamiento de una pantalla pública de publicidad basada en slides de productos.

---

## Características principales

- Autenticación y gestión de usuarios mediante roles personalizados:
  - Administrador
  - Usuario
  - Pantalla
  - Operador
- Gestión de comercios.
- Gestión de categorías y productos.
- Gestión de anuncios asociados a productos y fechas de vigencia.
- Historial de precios para identificar variaciones:
  - Precio aumentado.
  - Precio reducido.
  - Precio sin cambios.
- Pantalla pública de publicidad con presentación de productos mediante slides.
- Configuración visual de la pantalla:
  - Fondo.
  - Colores.
  - Animaciones.
  - Duración de los slides.
  - Indicadores de variación de precios.
- Control de versión de pantalla para facilitar actualizaciones.
- Seed automático para crear roles, usuarios iniciales y comercio principal.
- Persistencia de datos mediante PostgreSQL y Entity Framework Core.
- Localización configurada para `es-AR`.
- Uso de sesiones para mantener el estado del usuario.

---

## Tecnologías

| Tecnología | Descripción |
|---|---|
| .NET 10 | Framework principal |
| ASP.NET Core MVC | Arquitectura web |
| Razor Views | Renderizado de vistas |
| Entity Framework Core | ORM para acceso a datos |
| PostgreSQL | Motor de base de datos |
| Npgsql | Proveedor de PostgreSQL para Entity Framework Core |
| DotNetEnv | Carga de variables de entorno desde `.env` |
| Bootstrap | Framework CSS |
| jQuery | Librería JavaScript |
| HTML / CSS / JavaScript | Tecnologías utilizadas en el frontend |
| Docker | Contenerización de la aplicación |

---

## Estructura del proyecto

La aplicación utiliza una estructura basada en ASP.NET Core MVC.

```text
PublicidadDinamicaWeb/
│
├── Controllers/
│   ├── AccountController
│   ├── HomeController
│   ├── ProductosController
│   ├── ComerciosController
│   ├── PublicidadController
│   └── ...
│
├── Data/
│   ├── AppDbContext
│   └── ...
│
├── Models/
│   ├── Usuario
│   ├── Rol
│   ├── Comercio
│   ├── Categoria
│   ├── Producto
│   ├── Anuncio
│   ├── HistorialPrecio
│   ├── ConfiguracionPublicidad
│   ├── VersionPantalla
│   └── ...
│
├── Views/
│   ├── Account/
│   ├── Home/
│   ├── Productos/
│   ├── Comercios/
│   ├── Publicidad/
│   │   └── Pantalla.cshtml
│   └── ...
│
├── Migrations/
│   └── Migraciones de Entity Framework Core
│
├── wwwroot/
│   ├── css/
│   ├── js/
│   ├── images/
│   └── ...
│
├── .env
├── .env.example
├── Dockerfile
├── PublicidadDinamicaWeb.csproj
├── Program.cs
└── README.md
````

### Responsabilidades principales

#### `Controllers/`

Contiene los controladores MVC encargados de recibir las solicitudes HTTP, procesar las acciones correspondientes y coordinar la interacción entre las vistas, la lógica de aplicación y la capa de datos.

#### `Data/`

Contiene la configuración del acceso a datos, incluyendo `AppDbContext` y la configuración de Entity Framework Core.

#### `Models/`

Contiene las entidades principales del dominio de la aplicación, incluyendo usuarios, roles, comercios, productos, anuncios, historial de precios y configuración de publicidad.

#### `Views/`

Contiene las vistas Razor utilizadas para representar la interfaz web administrativa y la pantalla pública de publicidad.

#### `Migrations/`

Contiene las migraciones de Entity Framework Core utilizadas para crear y actualizar la estructura de la base de datos.

#### `wwwroot/`

Contiene los recursos estáticos de la aplicación, como hojas de estilos, JavaScript, imágenes y librerías frontend.

---

## Arquitectura general

El proyecto utiliza el patrón **MVC (Model-View-Controller)** de ASP.NET Core.

El flujo general de una solicitud es:

```text
Usuario
   |
   v
Controller
   |
   v
Lógica de aplicación
   |
   v
Entity Framework Core
   |
   v
PostgreSQL
   |
   v
Controller
   |
   v
Razor View
   |
   v
Usuario
```

La aplicación utiliza **Dependency Injection** de ASP.NET Core para registrar y resolver las dependencias necesarias durante la ejecución.

---

## Modelo de datos

Las principales entidades o tablas utilizadas por el sistema son:

* `Usuarios`
* `Roles`
* `UsuarioRol`
* `Comercios`
* `Categorias`
* `Productos`
* `HistorialPrecios`
* `Anuncios`
* `ConfiguracionPublicidad`
* `VersionPantalla`

### Relaciones principales

* Un usuario puede tener uno o varios roles.
* Un comercio puede tener categorías y productos.
* Los productos pueden tener historial de precios.
* Los productos pueden asociarse a anuncios.
* Cada comercio puede tener una configuración para su pantalla de publicidad.
* `VersionPantalla` permite controlar la versión almacenada de la configuración de la pantalla.

---

## Endpoints principales

| Ruta                                   | Descripción                                        |
| -------------------------------------- | -------------------------------------------------- |
| `/Account/Login`                       | Inicio de sesión                                   |
| `/Home/Index`                          | Dashboard principal                                |
| `/Publicidad/Pantalla?comercioId={id}` | Pantalla pública de publicidad                     |
| `/Publicidad/VersionPantalla`          | Consulta la versión almacenada en la base de datos |

### Pantalla pública

La pantalla pública de publicidad está disponible en:

```text
/Publicidad/Pantalla
```

También puede recibir un comercio específico:

```text
/Publicidad/Pantalla?comercioId={id}
```

Cuando no se proporciona `comercioId`, la aplicación utiliza la configuración correspondiente al comportamiento definido para la pantalla.

---

# Configuración

## Requisitos previos

Para ejecutar el proyecto localmente se requiere:

* .NET 10 SDK
* PostgreSQL
* Visual Studio 2022 con soporte para desarrollo ASP.NET Core, si se utiliza Visual Studio.
* Docker, opcional.

---

## Variables de entorno

La aplicación utiliza `DotNetEnv` para cargar variables de entorno desde un archivo `.env`.

Para ejecutar el proyecto localmente, crea un archivo `.env` en la raíz del proyecto.

Ejemplo:

```env
DB_HOST=localhost
DB_PORT=5432
DB_NAME=publicidad_dinamica
DB_USER=postgres
DB_PASSWORD=tu_password

ADMIN_EMAIL=admin@publicidad.com
ADMIN_PASSWORD=Admin123

PANTALLA_EMAIL=pantalla@publicidad.com
PANTALLA_PASSWORD=Pantalla123
```

> Los valores anteriores son únicamente ejemplos para desarrollo local. No utilices contraseñas de producción dentro del repositorio.

El archivo `.env` utilizado para desarrollo local contiene configuración de la base de datos local y usuarios seed.

---

## Ejecución local

### 1. Clonar el repositorio

```bash
git clone https://github.com/LuisAngelX12/PublicidadDinamicaWeb.git
```

Entrar al directorio:

```bash
cd PublicidadDinamicaWeb
```

### 2. Configurar las variables de entorno

Crea el archivo:

```text
.env
```

en la raíz del proyecto y configura las variables necesarias.

Si el repositorio contiene `.env.example`, puedes utilizarlo como plantilla.

En Linux o macOS:

```bash
cp .env.example .env
```

En Windows PowerShell:

```powershell
Copy-Item .env.example .env
```

Después modifica los valores de acuerdo con tu instalación local de PostgreSQL.

---

## Configuración de PostgreSQL

Asegúrate de tener PostgreSQL ejecutándose localmente y de que las credenciales configuradas en `.env` sean correctas.

Ejemplo:

```env
DB_HOST=localhost
DB_PORT=5432
DB_NAME=publicidad_dinamica
DB_USER=postgres
DB_PASSWORD=tu_password
```

La aplicación utiliza **Entity Framework Core + Npgsql** para conectarse a PostgreSQL.

---

## Migraciones de base de datos

La aplicación ejecuta las migraciones de Entity Framework Core durante el arranque mediante:

```text
context.Database.Migrate()
```

Por lo tanto, al iniciar la aplicación se aplicarán las migraciones pendientes.

También es posible utilizar Entity Framework Core CLI.

Si no tienes instalado `dotnet-ef`:

```bash
dotnet tool install --global dotnet-ef
```

Después ejecuta:

```bash
dotnet ef database update --project PublicidadDinamicaWeb.csproj
```

---

## Seed de datos iniciales

Durante el arranque de la aplicación se ejecuta el proceso de inicialización de datos.

El seed crea los elementos necesarios cuando no existen, incluyendo:

* Roles iniciales.
* Usuario administrador.
* Usuario de pantalla.
* Comercio principal.

Las credenciales utilizadas por los usuarios seed se obtienen de las variables configuradas en `.env`.

Ejemplo:

```env
ADMIN_EMAIL=admin@publicidad.com
ADMIN_PASSWORD=Admin123

PANTALLA_EMAIL=pantalla@publicidad.com
PANTALLA_PASSWORD=Pantalla123
```

> Estas credenciales son únicamente ejemplos para desarrollo local. En ambientes de producción deben utilizarse credenciales seguras y un mecanismo adecuado de gestión de secretos.

---

## Ejecutar la aplicación

### Visual Studio 2022

Abre la solución en Visual Studio 2022 y ejecuta el proyecto utilizando el perfil de ejecución correspondiente.

También puede utilizarse el perfil de Docker si el entorno está configurado para trabajar con contenedores.

### Línea de comandos

Desde la raíz del proyecto:

```bash
dotnet run --project PublicidadDinamicaWeb.csproj
```

Una vez iniciada la aplicación, utiliza la URL indicada por ASP.NET Core en la consola o por el perfil de ejecución configurado en Visual Studio.

---

# Docker

El proyecto incluye un `Dockerfile` para construir y ejecutar la aplicación dentro de un contenedor.

## Construir la imagen

Desde la raíz del repositorio:

```bash
docker build -t publicidad-dinamica:latest .
```

## Ejecutar el contenedor

Utilizando el archivo `.env`:

```bash
docker run --env-file .env -p 8080:8080 --name publicidad-dinamica publicidad-dinamica:latest
```

La aplicación expone los puertos:

```text
8080
8081
```

El mapeo de puertos puede modificarse según las necesidades del entorno.

Por ejemplo:

```bash
docker run --env-file .env -p 8080:8080 publicidad-dinamica:latest
```

> Asegúrate de que el archivo `.env` exista en la ubicación desde donde ejecutas el comando y que contenga las variables requeridas por la aplicación.

---

# Personalización de la pantalla de publicidad

La pantalla pública se encuentra en:

```text
/Views/Publicidad/Pantalla.cshtml
```

La configuración se obtiene mediante `ConfiguracionPublicidad`.

Entre los elementos configurables se encuentran:

* Tipo de fondo.
* Color de fondo.
* Imagen de fondo.
* Colores para las variaciones de precio.
* Tipo de animación.
* Duración de las animaciones.
* Duración de los slides.
* Indicadores de aumento o reducción de precio.
* Visualización del precio anterior.

Los recursos gráficos se sirven desde:

```text
wwwroot/images
```

donde se encuentran imágenes relacionadas con productos, comercios y posibles fondos de pantalla.

---

# Control de versión de pantalla

El proyecto cuenta con una entidad `VersionPantalla` que permite almacenar y consultar la versión de la pantalla desde la base de datos.

El endpoint:

```text
/Publicidad/VersionPantalla
```

permite consultar la versión almacenada.

Este mecanismo puede utilizarse para detectar cambios o actualizaciones de configuración de la pantalla.

---

# Seguridad y buenas prácticas

## Variables de entorno

No se deben almacenar credenciales de producción directamente en el repositorio.

Para entornos de producción se recomienda utilizar:

* Variables de entorno del servidor.
* Secret managers.
* Secretos administrados por el proveedor de infraestructura.

## Credenciales de desarrollo

Las credenciales incluidas como ejemplo en este README son únicamente para desarrollo local.

Se recomienda utilizar credenciales diferentes y seguras en cualquier entorno que no sea local.

## Autorización

El proyecto utiliza roles personalizados para controlar el acceso a determinadas funcionalidades.

Los roles principales son:

* `Admin`
* `Usuario`
* `Pantalla`
* `Operador`

---

# Solución de problemas

## No se puede conectar a PostgreSQL

Verifica:

1. Que PostgreSQL esté ejecutándose.
2. Que `DB_HOST` sea correcto.
3. Que `DB_PORT` corresponda al puerto configurado.
4. Que la base de datos exista o que el usuario tenga permisos para crearla.
5. Que `DB_USER` y `DB_PASSWORD` sean correctos.
6. Que el archivo `.env` se encuentre en la ubicación esperada.

## Las migraciones no se aplican

Comprueba que:

* El proyecto pueda conectarse correctamente a PostgreSQL.
* Las migraciones estén presentes en el proyecto.
* Entity Framework Core esté correctamente configurado.

También puedes ejecutar:

```bash
dotnet ef database update --project PublicidadDinamicaWeb.csproj
```
o

```bash
Update-database // En la Consola del Administrador de paquetes
```

## El puerto ya está ocupado

Si el puerto `8080` está siendo utilizado por otro proceso, cambia el puerto del host:

```bash
docker run --env-file .env -p 8082:8080 --name publicidad-dinamica publicidad-dinamica:latest
```

En este caso, la aplicación seguirá escuchando en el puerto `8080` dentro del contenedor, mientras que será accesible desde el puerto `8082` del equipo local.

# Licencia

Actualmente, el repositorio no contiene un archivo `LICENSE` explícito.

Si el proyecto va a distribuirse públicamente, se recomienda agregar una licencia que defina las condiciones de uso, modificación y distribución.

---

# Repositorio

Repositorio oficial:

[https://github.com/LuisAngelX12/PublicidadDinamicaWeb](https://github.com/LuisAngelX12/PublicidadDinamicaWeb)

---

# Autor

**Luis Ángel Hernández Monge**

---

> Este README está orientado principalmente al desarrollo y ejecución local del proyecto. Las configuraciones y credenciales mostradas son ejemplos y deben adaptarse al entorno donde se despliegue la aplicación.

```
```
