# Guía Práctica: Creación de un Proyecto Web API en .NET 10
**Curso:** Desarrollo de Servicios Web | **Sesión 07**

Esta guía explica dos métodos para crear un proyecto Web API REST en ASP.NET Core:
1. **Método A: Mediante comandos CLI de `dotnet`** (rápido y recomendado para desarrollo ágil).
2. **Método B: Creación 100% Manual desde cero** (recomendado para entender la anatomía real sin "magia" de plantillas).

---

## 🛠️ Método A: Creación con Comandos de .NET CLI

Abre una terminal (PowerShell o Bash) en la carpeta donde deseas ubicar el proyecto:

### 1. Crear el proyecto Web API
```bash
# Opción 1: Plantilla Web API clásica con controladores
dotnet new webapi -n Sesion_07 --use-controllers

# Opción 2: Plantilla Web mínima vacía (la más limpia)
dotnet new web -n Sesion_07
```

### 2. Agregar el proyecto a la solución existente
```bash
# Desde la raíz donde se encuentra AdoptaMasAPI.sln:
dotnet sln AdoptaMasAPI.sln add Sesiones/Sesion_07/Sesion_07.csproj
```

### 3. Compilar y ejecutar
```bash
cd Sesiones/Sesion_07
dotnet run
```

---

## 🏗️ Método B: Creación 100% Manual (Sin Plantillas)

Para entender exactamente qué compone una Web API, puedes crear los archivos a mano sin usar ninguna plantilla:

### 1. Crear las carpetas base
```bash
mkdir Sesion_07
cd Sesion_07
mkdir Controllers Services Middlewares Models Properties
```

### 2. Crear el archivo de proyecto (`Sesion_07.csproj`)
Crea un archivo llamado `Sesion_07.csproj` con este contenido exacto:
```xml
<Project Sdk="Microsoft.NET.Sdk.Web">
  <PropertyGroup>
    <TargetFramework>net10.0</TargetFramework>
    <Nullable>enable</Nullable>
    <ImplicitUsings>enable</ImplicitUsings>
  </PropertyGroup>
</Project>
```
> **Nota clave:** Al especificar `Sdk="Microsoft.NET.Sdk.Web"`, .NET importa automáticamente Kestrel, Inyección de Dependencias, Routing y JSON. No necesitas instalar ningún paquete NuGet externo.

### 3. Configurar el puerto local (`Properties/launchSettings.json`)
Crea el archivo `Properties/launchSettings.json`:
```json
{
  "profiles": {
    "http": {
      "commandName": "Project",
      "dotnetRunMessages": true,
      "launchBrowser": false,
      "applicationUrl": "http://localhost:5100",
      "environmentVariables": {
        "ASPNETCORE_ENVIRONMENT": "Development"
      }
    }
  }
}
```

### 4. Crear el punto de entrada (`Program.cs`)
Crea el archivo `Program.cs` con el mínimo necesario:
```csharp
using Sesion_07.Middlewares;
using Sesion_07.Services;

var builder = WebApplication.CreateBuilder(args);

// 1. Contenedor de Inversión de Control (IoC)
builder.Services.AddControllers();
builder.Services.AddSingleton<IProductoServicio, ProductoServicioEnMemoria>();

var app = builder.Build();

// 2. Pipeline de Middlewares
app.UseMiddleware<ManejadorExcepcionesMiddleware>();

// 3. Mapeo de Controladores
app.MapControllers();

app.Run();
```

---

## 📁 Estructura Final del Proyecto

```text
Sesion_07/
├── Controllers/
│   └── ProductosController.cs            <-- Rutas y endpoints HTTP con ActionResult<T>
├── Middlewares/
│   └── ManejadorExcepcionesMiddleware.cs <-- Captura global RFC 7807 (ProblemDetails)
├── Models/
│   └── ProductoDtos.cs                   <-- DTOs de transferencia
├── Properties/
│   └── launchSettings.json               <-- Puerto Kestrel (:5100) y perfiles
├── Services/
│   └── ProductoServicio.cs               <-- Lógica desacoplada (IProductoServicio)
├── Program.cs                            <-- Punto de entrada unificado
└── Sesion_07.csproj                      <-- Definición del proyecto con SDK Web
```

---

## 🚀 Verificación y Pruebas

Ejecuta el proyecto con:
```bash
dotnet run
```

Prueba los endpoints en tu navegador o cliente HTTP:
* **📖 Documentación Interactiva Scalar:** `http://localhost:5100/scalar/v1`
* **Especificación OpenAPI (JSON):** `http://localhost:5100/openapi/v1.json`
* **200 OK (Catálogo completo):** `GET http://localhost:5100/api/productos`
* **200 OK (Producto por ID):** `GET http://localhost:5100/api/productos/101`
* **404 Not Found (ID inexistente):** `GET http://localhost:5100/api/productos/999`
* **500 Error RFC 7807 (ProblemDetails):** `GET http://localhost:5100/api/productos/simular-error-500`
