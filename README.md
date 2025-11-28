# 🏛️ Museo API - Documentación

![.NET](https://img.shields.io/badge/.NET-8.0%20%7C%209.0-512BD4?style=flat&logo=dotnet)
![Docker](https://img.shields.io/badge/Docker-Compose-2496ED?style=flat&logo=docker)
![PostgreSQL](https://img.shields.io/badge/PostgreSQL-16-316192?style=flat&logo=postgresql)
![Security](https://img.shields.io/badge/Security-JWT%20%2B%20Refresh%20Token-green)
![Pattern](https://img.shields.io/badge/Architecture-Repository%20Pattern-orange)

---

## 👥 Equipo de Desarrollo - Grupo 7

| Ariana Aylen Pita Vargas |
| Maria Jose Sandoval Orellana |
| Sebastian Alejandro Arce Antezana |

---

## 🏗️ Arquitectura y Decisiones de Diseño

La API ha sido construida sobre una arquitectura limpia y en capas, priorizando la mantenibilidad, escalabilidad y seguridad.

### 1. Patrón Repository y Capa de Servicios
La lógica está estrictamente separada para adherirse al principio de Responsabilidad Única (SRP).
*   **Capa de Repositorio (`Repositories/`):** Actúa como una abstracción sobre el `DbContext` de Entity Framework Core. Su única responsabilidad es la ejecución de operaciones CRUD contra la base de datos. Utiliza carga ansiosa (`Eager Loading`) mediante `Include`/`ThenInclude` para optimizar consultas que requieren datos relacionales.
*   **Capa de Servicio (`Services/`):** Orquesta la lógica de negocio. Invoca a los repositorios para obtener entidades y las transforma en DTOs, aplica validaciones complejas (ej. un usuario no puede comentar su propia obra si así se definiera) y maneja la lógica de transacciones.

### 2. Manejo Avanzado de Relaciones y Ciclos (DTOs)
Uno de los mayores desafíos en APIs con modelos de datos complejos son las referencias circulares durante la serialización JSON.
*   **Problema Identificado:** Un `Museo` tiene `Canvas`, un `Canvas` tiene un `Museo`. La serialización directa (`Json(museo)`) provoca una excepción `JsonException` por ciclo infinito.
*   **Solución Implementada:** Se ha optado por un enfoque de **proyecciones mediante Data Transfer Objects (DTOs)**. En lugar de simplemente ignorar propiedades (`[JsonIgnore]`), se crean modelos de vista específicos para cada endpoint. Esto no solo resuelve el ciclo, sino que permite modelar la respuesta de la API de forma precisa y eficiente, enviando solo los datos que el cliente necesita y ocultando la estructura interna de la base de datos.

### 3. Seguridad `Stateless` (JWT y Refresh Tokens)
La seguridad se basa en un esquema de autenticación sin estado (stateless) para garantizar escalabilidad horizontal.
*   **Access Token (JWT):** Token de corta duración (60 min) firmado con una clave secreta (HMAC-SHA256). Contiene `Claims` (como `UserId` y `Role`) que permiten al backend identificar y autorizar al usuario en cada petición.
*   **Refresh Token:** Token de larga duración (14 días), almacenado de forma segura en la base de datos y asociado al usuario. Se utiliza para generar nuevos Access Tokens sin requerir que el usuario vuelva a introducir sus credenciales.
*   **Protección Adicional:** Se implementó `Rate Limiting` en `Program.cs` para mitigar ataques de fuerza bruta en los endpoints de autenticación.

---

## 🚀 Guía de Despliegue y Ejecución

### Opción 1: Docker (Entorno de Producción Recomendado)
El proyecto está completamente contenerizado para un despliegue rápido y consistente.

1.  **Clonar Repositorio:**
    ```bash
    git clone https://github.com/Majo2505/Museo_Grupo7.git && cd Museo_Grupo7
    ```

2.  **Levantar Contenedores:**
    Este comando construirá la imagen de la API y levantará el contenedor junto a la base de datos PostgreSQL.
    ```bash
    docker-compose up -d --build
    ```

3.  **Aplicar Migraciones (Primera Vez):**
    Aunque la API se inicie, la base de datos estará vacía. Para crear las tablas, ejecuta:
    ```bash
    dotnet ef database update
    ```
    *(Asegúrate de que la ConnectionString en `appsettings.Development.json` apunte a `localhost` y al puerto `5432` si ejecutas este comando desde fuera de Docker).*

4.  **Acceso:**
    *   **Swagger UI:** `http://localhost:8080/swagger`
    *   **Base de Datos:** Accesible en `localhost:5432`

### Opción 2: Local (Entorno de Desarrollo)
1.  Verificar que .NET SDK 8.0+ y PostgreSQL estén instalados.
2.  Configurar la `ConnectionString` en `appsettings.Development.json` para que apunte a tu instancia local de Postgres.
3.  Abrir una terminal en la raíz del proyecto y ejecutar los siguientes comandos:
    ```bash
    dotnet ef database update # Crea/Actualiza el esquema de la BD
    dotnet run               # Inicia la API
    ```

---

## 📚 Referencia Completa de Endpoints

### Módulo de Autenticación (`/api/Auth`)

Este módulo gestiona el registro, inicio de sesión y renovación de tokens.

---
#### **Endpoint: `POST /api/Auth/register`**
*   **Descripción:** Registra un nuevo usuario en el sistema. La contraseña se hashea usando BCrypt. Por defecto, se le asigna el rol "Visitante".
*   **Seguridad:** Abierto (`AllowAnonymous`).
*   **Request Body (`RegisterDto`):**
    ```json
    {
      "username": "nuevo_usuario",
      "email": "usuario@ejemplo.com",
      "password": "PasswordSegura123!"
    }
    ```
*   **Response (201 Created):**
    Retorna un encabezado `Location` apuntando al recurso creado. El cuerpo está vacío.
*   **Posibles Errores:**
    *   `400 Bad Request`: Si el email ya está en uso o si los datos no cumplen las validaciones (ej. contraseña débil).

---
#### **Endpoint: `POST /api/Auth/login`**
*   **Descripción:** Autentica a un usuario con su email y contraseña. Si las credenciales son válidas, genera y devuelve un Access Token y un Refresh Token.
*   **Seguridad:** Abierto (`AllowAnonymous`).
*   **Request Body (`LoginDto`):**
    ```json
    {
      "email": "usuario@ejemplo.com",
      "password": "PasswordSegura123!"
    }
    ```
*   **Response (200 OK - `LoginResponseDto`):**
    ```json
    {
      "user": {
        "id": "a1b2c3d4-...",
        "username": "nuevo_usuario",
        "email": "usuario@ejemplo.com"
      },
      "role": "Visitante",
      "accessToken": "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9...",
      "refreshToken": "long_random_string_for_refreshing...",
      "tokenType": "Bearer",
      "expiresIn": 3600
    }
    ```
*   **Posibles Errores:**
    *   `401 Unauthorized`: Si las credenciales son incorrectas.

---
#### **Endpoint: `POST /api/Auth/refresh`**
*   **Descripción:** Permite obtener un nuevo Access Token utilizando un Refresh Token válido y no revocado.
*   **Seguridad:** Abierto (`AllowAnonymous`).
*   **Request Body (`RefreshRequestDto`):**
    ```json
    {
      "refreshToken": "long_random_string_for_refreshing..."
    }
    ```
*   **Response (200 OK - `LoginResponseDto`):**
    Devuelve la misma estructura que el endpoint de Login, con tokens renovados.
*   **Posibles Errores:**
    *   `401 Unauthorized`: Si el Refresh Token es inválido, ha expirado o ha sido revocado.

---
### Módulo de Comentarios (`/api/Comments`)

Este módulo demuestra el acceso a recursos protegidos y la extracción de identidad desde el token.

---
#### **Endpoint: `GET /api/Comments/canvas/{canvasId}`**
*   **Descripción:** Obtiene todos los comentarios asociados a una obra de arte (`Canvas`), ordenados del más reciente al más antiguo.
*   **Seguridad:** Abierto (`AllowAnonymous`).
*   **Response (200 OK):**
    ```json
    [
      {
        "id": "c1d2e3f4-...",
        "content": "¡Una obra de arte espectacular!",
        "createdAt": "2025-11-28T10:05:00Z",
        "canvasId": "ID_DEL_CANVAS",
        "userId": "ID_DEL_USUARIO",
        "username": "nuevo_usuario"
      }
    ]
    ```

---
#### **Endpoint: `POST /api/Comments`**
*   **Descripción:** Crea un nuevo comentario para una obra. El sistema extrae automáticamente el `UserId` del token JWT, garantizando que el autor sea el usuario autenticado.
*   **Seguridad:** Protegido (`[Authorize]`). Requiere un `Bearer Token` válido en el encabezado `Authorization`.
*   **Request Body (`CreateCommentDto`):**
    ```json
    {
      "content": "El juego de luces y sombras es magistral.",
      "canvasId": "a1b2c3d4-..."
    }
    ```
*   **Response (201 Created):**
    Devuelve el comentario recién creado, incluyendo el `username` obtenido de la relación con el usuario.
    ```json
    {
      "id": "f4e3d2c1-...",
      "content": "El juego de luces y sombras es magistral.",
      "createdAt": "2025-11-28T10:10:00Z",
      "canvasId": "a1b2c3d4-...",
      "userId": "ID_DEL_USUARIO_AUTENTICADO",
      "username": "nuevo_usuario"
    }
    ```
*   **Posibles Errores:**
    *   `401 Unauthorized`: Si no se proporciona un token válido.
    *   `404 Not Found`: Si el `canvasId` no existe.

---
#### **Endpoint: `PUT /api/Comments/{commentId}`**
*   **Descripción:** Actualiza el contenido de un comentario existente. Un usuario solo puede modificar sus propios comentarios.
*   **Seguridad:** Protegido (`[Authorize]`). Valida que el `UserId` del token coincida con el `UserId` del comentario.
*   **Request Body (`UpdateCommentDto`):**
    ```json
    {
      "content": "Corrección: El juego de luces y sombras es sublime."
    }
    ```
*   **Response (200 OK):** Devuelve el comentario actualizado.
*   **Posibles Errores:**
    *   `403 Forbidden`: Si el usuario intenta modificar un comentario que no le pertenece.
    *   `404 Not Found`: Si el `commentId` no existe.

---
#### **Endpoint: `DELETE /api/Comments/{commentId}`**
*   **Descripción:** Elimina un comentario. Solo el autor del comentario o un administrador pueden realizar esta acción.
*   **Seguridad:** Protegido (`[Authorize]`). Valida la autoría.
*   **Response (204 No Content):** No devuelve cuerpo si la eliminación es exitosa.
*   **Posibles Errores:**
    *   `403 Forbidden`: Si el usuario no es el autor.
    *   `404 Not Found`: Si el `commentId` no existe.

---

### Módulo de Artistas (`/api/Artist`)

Este controlador gestiona el catálogo de artistas. Destaca por el uso de proyecciones DTO para listar las obras de cada artista sin caer en recursividad.

---
#### **Endpoint: `GET /api/Artist`**
*   **Descripción:** Recupera la lista completa de artistas registrados. Incluye una proyección de los títulos de sus obras (`CanvasTitles`), evitando traer todo el objeto `Canvas` y `Museum` anidados.
*   **Seguridad:** Abierto (`AllowAnonymous`).
*   **Response (200 OK - `IEnumerable<ArtistResponseDto>`):**
    ```json
    [
      {
        "id": "e4b3c2d1-...",
        "name": "Vincent van Gogh",
        "description": "Pintor postimpresionista neerlandés.",
        "specialty": "Óleo",
        "typeOfWork": "Postimpresionismo",
        "canvasTitles": [
          "La Noche Estrellada",
          "Los Girasoles"
        ]
      }
    ]
    ```

---
#### **Endpoint: `GET /api/Artist/{id}`**
*   **Descripción:** Obtiene el detalle de un artista específico por su GUID.
*   **Seguridad:** Abierto (`AllowAnonymous`).
*   **Response (200 OK - `ArtistResponseDto`):** Misma estructura que el listado, para un solo elemento.
*   **Posibles Errores:**
    *   `404 Not Found`: Si el ID no existe.

---
#### **Endpoint: `POST /api/Artist`**
*   **Descripción:** Crea un nuevo perfil de artista en la base de datos.
*   **Seguridad:** Protegido (`[Authorize]`). Idealmente restringido a roles `Admin` (según políticas configuradas).
*   **Request Body (`CreateArtistDto`):**
    ```json
    {
      "name": "Pablo Picasso",
      "description": "Pintor y escultor español, creador del cubismo.",
      "specialty": "Pintura, Escultura",
      "typeOfWork": "Cubismo"
    }
    ```
*   **Response (201 Created):** Retorna el artista creado con su ID generado.

---

### Módulo de Obras / Lienzos (`/api/Canvas`)

Este es el núcleo relacional del sistema. Un `Canvas` conecta `Museum` (N:1) y `Artist` (N:M).

---
#### **Endpoint: `GET /api/Canvas`**
*   **Descripción:** Lista todas las obras de arte disponibles.
*   **Optimización:** Utiliza DTOs para mostrar los nombres de los artistas (`ArtistNames`) en lugar de objetos `Work` complejos, simplificando el consumo para el frontend.
*   **Seguridad:** Abierto (`AllowAnonymous`).
*   **Response (200 OK - `IEnumerable<CanvasResponseDto>`):**
    ```json
    [
      {
        "id": "a1b2c3d4-...",
        "title": "Guernica",
        "technique": "Óleo sobre lienzo",
        "dateOfEntry": "1937-06-04T00:00:00Z",
        "museumId": "GUID_DEL_MUSEO",
        "artistNames": [
          "Pablo Picasso"
        ]
      }
    ]
    ```

---
#### **Endpoint: `POST /api/Canvas`**
*   **Descripción:** Registra una nueva obra. Este endpoint es **transaccional**: crea el registro en la tabla `Canvas` y, simultáneamente, inserta los registros necesarios en la tabla intermedia `Works` para vincular a los artistas.
*   **Seguridad:** Protegido (`[Authorize]`).
*   **Request Body (`CreateCanvasDto`):**
    ```json
    {
      "title": "La Persistencia de la Memoria",
      "technique": "Óleo sobre lienzo",
      "dateOfEntry": "1931-01-01T00:00:00Z",
      "museumId": "GUID_DEL_MOMA",
      "artistIds": [
        "GUID_ARTISTA_DALI"
      ]
    }
    ```
*   **Response (201 Created):** Retorna la obra creada.
*   **Posibles Errores:**
    *   `404 Not Found`: Si el `museumId` o alguno de los `artistIds` no existen en la base de datos.

---

### Módulo de Museos (`/api/Museum`)

Gestiona las entidades principales donde se alojan las obras.

---
#### **Endpoint: `GET /api/Museum`**
*   **Descripción:** Devuelve el catálogo de museos.
*   **Proyección:** Incluye una lista anidada de las obras (`Canvas`) que posee cada museo. Para evitar ciclos, los objetos `Canvas` anidados **NO** incluyen la propiedad `Museum` de vuelta, pero **SÍ** incluyen la lista de nombres de sus artistas.
*   **Seguridad:** Abierto (`AllowAnonymous`).
*   **Response (200 OK - `IEnumerable<MuseumResponseDto>`):**
    ```json
    [
      {
        "id": "m1u2s3e4-...",
        "name": "Museo Reina Sofía",
        "description": "Museo nacional de arte del siglo XX.",
        "openingYear": 1992,
        "cityId": "GUID_MADRID",
        "canvas": [
          {
            "id": "c1a2n3v4...",
            "title": "Guernica",
            "artistNames": ["Pablo Picasso"]
          }
        ]
      }
    ]
    ```

---
#### **Endpoint: `POST /api/Museum`**
*   **Descripción:** Registra un nuevo museo en una ciudad específica.
*   **Seguridad:** Protegido (`[Authorize]`).
*   **Request Body (`CreateMuseumDto`):**
    ```json
    {
      "name": "Museo del Prado",
      "description": "Uno de los más importantes del mundo.",
      "openingYear": 1819,
      "cityId": "GUID_MADRID"
    }
    ```
*   **Response (201 Created):** Retorna el museo creado.

---

### Módulo de Ciudades (`/api/City`)

Entidad geográfica simple que agrupa a los museos.

---
#### **Endpoint: `GET /api/City`**
*   **Descripción:** Lista las ciudades registradas.
*   **Seguridad:** Abierto (`AllowAnonymous`).
*   **Response (200 OK):**
    ```json
    [
      {
        "id": "c1i2t3y4...",
        "name": "Madrid",
        "country": "España",
        "museum": {
            "id": "...",
            "name": "Museo del Prado"
        }
      }
    ]
    ```
*(Nota: El objeto `museum` anidado aquí no despliega su lista de `canvas` para mantener la respuesta ligera).*

---
#### **Endpoint: `POST /api/City`**
*   **Descripción:** Agrega una nueva ciudad al sistema.
*   **Seguridad:** Protegido (`[Authorize]`).
*   **Request Body:**
    ```json
    {
      "name": "París",
      "country": "Francia"
    }
    ```
*   **Response (201 Created):** Retorna la ciudad creada.

---

## ⚙️ Configuración y Variables de Entorno

El proyecto utiliza `DotNetEnv` para cargar configuraciones sensibles desde un archivo `.env` en la raíz, protegiendo credenciales en el control de versiones.

| Variable | Descripción | Valor por Defecto (Dev) |
| :--- | :--- | :--- |
| `POSTGRES_DB` | Nombre de la Base de Datos | `museodb` |
| `POSTGRES_USER` | Usuario de Base de Datos | `museouser` |
| `POSTGRES_PASSWORD` | Contraseña de Base de Datos | `supersecret` |
| `JWT_KEY` | Clave secreta para firma de Tokens | *(Debe ser robusta)* |
| `JWT_REFRESHDAYS` | Días de validez del Refresh Token | `14` |

---
