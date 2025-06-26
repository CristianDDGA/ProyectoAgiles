# 🎓 **ProyectoAgiles - Sistema de Escalafón Docente UTA**

## 📁 **ESTRUCTURA COMPLETA DEL PROYECTO**

```
proyectoAgiles/                                    # 🏠 Directorio Raíz del Proyecto
├── 📄 proyectoAgiles.slnx                        # Archivo de solución .NET
├── 📄 README.md                                   # Documentación principal
├── 📄 Sprint_2_Documentacion_Completa.md         # Documentación del sprint 2
├── 📄 .gitignore                                  # Configuración de Git
├── 📁 .git/                                       # Control de versiones Git
├── 📁 .vscode/                                    # Configuración de VS Code
│   └── settings.json                              # Configuraciones del editor
├── 📁 obj/                                        # Archivos temporales de compilación
└── 📁 InsertScript/                               # Scripts de inserción de datos
    ├── bin/ & obj/                                # Archivos de compilación
    └── InsertScript.csproj                        # Proyecto de scripts

├── 📁 ProyectoAgiles.Domain/                      # 🏛️ CAPA DE DOMINIO
│   ├── 📄 ProyectoAgiles.Domain.csproj           # Configuración del proyecto
│   ├── 📁 Entities/                               # Entidades del dominio
│   │   ├── BaseEntity.cs                         # Entidad base con propiedades comunes
│   │   ├── User.cs                               # Entidad de usuarios del sistema
│   │   ├── DITIC.cs                              # Entidad de capacitaciones DITIC
│   │   ├── EvaluacionDesempeno.cs                # Entidad de evaluaciones DAC
│   │   ├── Investigacion.cs                      # Entidad de investigaciones
│   │   ├── SolicitudEscalafon.cs                 # Entidad de solicitudes de escalafón
│   │   ├── TTHH.cs                               # Entidad de Talento Humano
│   │   ├── ExternalTeacher.cs                    # Entidad de docentes externos
│   │   └── PasswordResetToken.cs                 # Entidad de tokens de recuperación
│   ├── 📁 Enums/                                  # Enumeraciones del dominio
│   │   ├── UserType.cs                           # Tipos de usuario (Admin, TTHH, Docente)
│   │   ├── SolicitudStatus.cs                    # Estados de solicitudes
│   │   └── EvaluacionStatus.cs                   # Estados de evaluaciones
│   ├── 📁 Interfaces/                             # Interfaces del dominio
│   │   ├── IRepository.cs                        # Interfaz base de repositorio
│   │   ├── IUserRepository.cs                    # Interfaz específica de usuarios
│   │   ├── IDiticRepository.cs                   # Interfaz de capacitaciones DITIC
│   │   ├── IEvaluacionDesempenoRepository.cs     # Interfaz de evaluaciones
│   │   ├── IInvestigacionRepository.cs           # Interfaz de investigaciones
│   │   ├── ISolicitudEscalafonRepository.cs      # Interfaz de solicitudes
│   │   └── ITTHHRepository.cs                    # Interfaz de Talento Humano
│   └── 📁 bin/ & obj/                            # Archivos de compilación

├── 📁 ProyectoAgiles.Application/                 # 🧠 CAPA DE APLICACIÓN
│   ├── 📄 ProyectoAgiles.Application.csproj      # Configuración del proyecto
│   ├── 📁 DTOs/                                   # Data Transfer Objects
│   │   ├── ApiResponse.cs                        # DTO de respuesta estándar de API
│   │   ├── UserDtos.cs                           # DTOs relacionados con usuarios
│   │   ├── DiticDto.cs                           # DTOs de capacitaciones DITIC
│   │   ├── EvaluacionDesempenoDto.cs             # DTOs de evaluaciones de desempeño
│   │   ├── InvestigacionDto.cs                   # DTOs de investigaciones
│   │   ├── SolicitudEscalafonDto.cs              # DTOs de solicitudes de escalafón
│   │   ├── TeacherManagementDtos.cs              # DTOs de gestión de docentes
│   │   ├── DashboardDtos.cs                      # DTOs del dashboard
│   │   └── RequisitoEscalafonConfigDto.cs        # DTOs de configuración de requisitos
│   ├── 📁 Interfaces/                             # Interfaces de servicios
│   │   ├── IAuthService.cs                       # Servicio de autenticación
│   │   ├── IUserService.cs                       # Servicio de usuarios
│   │   ├── IDiticService.cs                      # Servicio de capacitaciones DITIC
│   │   ├── IEvaluacionDesempenoService.cs        # Servicio de evaluaciones
│   │   ├── IInvestigacionService.cs              # Servicio de investigaciones
│   │   ├── ISolicitudEscalafonService.cs         # Servicio de solicitudes
│   │   ├── ITeacherManagementService.cs          # Servicio de gestión docente
│   │   ├── IEmailService.cs                      # Servicio de correo electrónico
│   │   └── IFileService.cs                       # Servicio de manejo de archivos
│   ├── 📁 Services/                               # Implementación de servicios
│   │   ├── AuthService.cs                        # Lógica de autenticación y autorización
│   │   ├── UserService.cs                        # Lógica de gestión de usuarios
│   │   ├── DiticService.cs                       # Lógica de capacitaciones DITIC
│   │   ├── EvaluacionDesempenoService.cs         # Lógica de evaluaciones DAC
│   │   ├── InvestigacionService.cs               # Lógica de investigaciones
│   │   ├── SolicitudEscalafonService.cs          # Lógica de solicitudes de escalafón
│   │   ├── TeacherManagementService.cs           # Lógica de gestión docente
│   │   ├── EmailService.cs                       # Envío real de correos
│   │   ├── MockEmailService.cs                   # Simulador de correos (desarrollo)
│   │   ├── FileService.cs                        # Manejo de archivos PDF
│   │   └── RequisitosEscalafonService.cs         # Lógica de requisitos de escalafón
│   ├── 📁 Mappings/                               # Configuraciones de AutoMapper
│   │   ├── UserMappingProfile.cs                 # Mapeo de entidades de usuarios
│   │   ├── DiticMappingProfile.cs                # Mapeo de capacitaciones DITIC
│   │   ├── EvaluacionDesempenoMappingProfile.cs  # Mapeo de evaluaciones
│   │   ├── InvestigacionMappingProfile.cs        # Mapeo de investigaciones
│   │   └── SolicitudEscalafonMappingProfile.cs   # Mapeo de solicitudes
│   └── 📁 bin/ & obj/                            # Archivos de compilación

├── 📁 ProyectoAgiles.Infrastructure/              # 🔧 CAPA DE INFRAESTRUCTURA
│   ├── 📄 ProyectoAgiles.Infrastructure.csproj   # Configuración del proyecto
│   ├── 📁 Data/                                   # Configuración de base de datos
│   │   ├── ApplicationDbContext.cs               # Contexto principal de Entity Framework
│   │   └── DbInitializer.cs                      # Inicializador de datos semilla
│   ├── 📁 Repositories/                           # Implementación de repositorios
│   │   ├── Repository.cs                         # Repositorio base genérico
│   │   ├── UserRepository.cs                     # Repositorio de usuarios
│   │   ├── DiticRepository.cs                    # Repositorio de capacitaciones DITIC
│   │   ├── EvaluacionDesempenoRepository.cs      # Repositorio de evaluaciones
│   │   ├── InvestigacionRepository.cs            # Repositorio de investigaciones
│   │   ├── SolicitudEscalafonRepository.cs       # Repositorio de solicitudes
│   │   ├── TTHHRepository.cs                     # Repositorio de Talento Humano
│   │   ├── ExternalTeacherRepository.cs          # Repositorio de docentes externos
│   │   └── PasswordResetTokenRepository.cs       # Repositorio de tokens
│   ├── 📁 Migrations/                             # Migraciones de Entity Framework
│   │   ├── 20240601000000_InitialCreate.cs       # Migración inicial
│   │   ├── 20240615000000_AddUserFields.cs       # Agregado de campos de usuario
│   │   ├── 20240620000000_AddDiticEntity.cs      # Agregado de entidad DITIC
│   │   ├── 20240625000000_AddEvaluaciones.cs     # Agregado de evaluaciones
│   │   └── ...más migraciones                    # Otras migraciones del proyecto
│   └── 📁 bin/ & obj/                            # Archivos de compilación

├── 📁 ProyectoAgiles.Api/                         # 🌐 CAPA DE API (BACKEND)
│   ├── 📄 ProyectoAgiles.Api.csproj              # Configuración del proyecto API
│   ├── 📄 Program.cs                             # Punto de entrada y configuración
│   ├── 📄 appsettings.json                       # Configuración de producción
│   ├── 📄 appsettings.Development.json           # Configuración de desarrollo
│   ├── 📄 ProyectoAgiles.Api.http                # Archivo de pruebas HTTP
│   ├── 📄 test-api.http                          # Pruebas adicionales de API
│   ├── 📁 Controllers/                            # Controladores de API REST
│   │   ├── AuthController.cs                     # 🔐 Autenticación (7 endpoints)
│   │   ├── UsersController.cs                    # 👥 Gestión de usuarios (8 endpoints)
│   │   ├── DiticController.cs                    # 🎓 Capacitaciones DITIC (16 endpoints)
│   │   ├── InvestigacionesController.cs          # 🔬 Investigaciones (11 endpoints)
│   │   ├── EvaluacionesDesempenoController.cs    # ⭐ Evaluaciones DAC (20 endpoints)
│   │   ├── SolicitudesEscalafonController.cs     # 📋 Solicitudes escalafón (13 endpoints)
│   │   ├── TeacherManagementController.cs        # 👨‍🏫 Gestión docentes (3 endpoints)
│   │   ├── TTHHController.cs                     # 🏢 Talento Humano (3 endpoints)
│   │   └── DashboardController.cs                # 📊 Dashboard (2 endpoints)
│   ├── 📁 Properties/                             # Propiedades del proyecto
│   │   └── launchSettings.json                   # Configuración de lanzamiento
│   ├── 📁 wwwroot/                                # Archivos estáticos del API
│   │   ├── swagger-ui/                           # Personalización de Swagger
│   │   │   ├── custom.css                        # Estilos personalizados
│   │   │   └── custom.js                         # Funcionalidades personalizadas
│   │   └── uploads/                              # Archivos subidos por usuarios
│   └── 📁 bin/ & obj/                            # Archivos de compilación

└── 📁 proyectoAgiles/                             # 🎨 FRONTEND (BLAZOR WEBASSEMBLY)
    ├── 📄 proyectoAgiles.csproj                  # Configuración del proyecto frontend
    ├── 📄 Program.cs                             # Punto de entrada del frontend
    ├── 📄 App.razor                              # Componente raíz de la aplicación
    ├── 📄 _Imports.razor                         # Importaciones globales
    ├── 📁 Layout/                                 # Layouts de la aplicación
    │   ├── MainLayout.razor                      # Layout principal
    │   ├── MainLayout.razor.css                  # Estilos del layout principal
    │   ├── AuthLayout.razor                      # Layout de autenticación
    │   ├── AuthLayout.razor.css                  # Estilos del layout de auth
    │   ├── NavMenu.razor                         # Menú de navegación
    │   └── NavMenu.razor.css                     # Estilos del menú
    ├── 📁 Pages/                                  # Páginas de la aplicación
    │   ├── Home.razor/.css                       # 🏠 Página de inicio
    │   ├── Login.razor/.css                      # 🔑 Página de inicio de sesión
    │   ├── Register.razor/.css                   # 📝 Página de registro
    │   ├── ForgotPassword.razor/.css             # 🔄 Recuperación de contraseña
    │   ├── ResetPassword.razor/.css              # 🔒 Restablecimiento de contraseña
    │   ├── TeacherDashboard.razor/.css           # 👨‍🏫 Dashboard del docente
    │   ├── AdminDashboard.razor/.css             # 👑 Dashboard del administrador
    │   ├── TalentoHumano.razor/.css              # 🏢 Panel de Talento Humano
    │   ├── DireccionTalentoHumano.razor/.css     # 🎯 Dirección de TTHH
    │   ├── ManageTeachers.razor/.css             # 👥 Gestión de docentes
    │   ├── ComisionAcademicaEscalafon.razor/.css # 🏛️ Comisión Académica
    │   └── PresidenteComisionAcademica.razor/.css# 👑 Presidente de Comisión
    ├── 📁 Services/                               # Servicios del frontend
    │   ├── AuthService.cs                        # Servicio de autenticación frontend
    │   ├── UserSessionService.cs                 # Gestión de sesión de usuario
    │   └── VerificacionRequisitosEscalafonDto.cs # DTOs de verificación
    ├── 📁 Shared/                                 # Componentes compartidos
    │   └── (componentes reutilizables)           # Componentes entre páginas
    ├── 📁 Properties/                             # Propiedades del proyecto
    │   └── launchSettings.json                   # Configuración de lanzamiento
    ├── 📁 wwwroot/                                # Recursos estáticos
    │   ├── 📄 index.html                         # Página HTML principal
    │   ├── 📄 appsettings.json                   # Configuración del frontend
    │   ├── 📄 favicon.png                        # Icono de la aplicación
    │   ├── 📄 icon-192.png                       # Icono PWA 192x192
    │   ├── 📁 css/                                # Hojas de estilo
    │   │   ├── app.css                           # Estilos principales
    │   │   ├── notifications.css                 # Estilos de notificaciones
    │   │   └── proyectoAgiles.styles.css         # Estilos generados
    │   ├── 📁 js/                                 # Scripts JavaScript
    │   │   ├── file-drag-drop.js                 # Funcionalidad drag & drop
    │   │   ├── notifications.js                  # Sistema de notificaciones
    │   │   └── pdf-generator.js                  # Manejo de PDFs
    │   ├── 📁 lib/                                # Librerías externas
    │   │   └── bootstrap/                        # Framework Bootstrap
    │   └── 📁 images/                             # Imágenes de la aplicación
    └── 📁 bin/ & obj/                            # Archivos de compilación
```

## 🎯 **RESUMEN COMPLETO DEL STACK TECNOLÓGICO**

---

## 🌐 **FRONTEND - BLAZOR WEBASSEMBLY**

### **Framework Principal**
- **Blazor WebAssembly** con **.NET 9.0**
- **C# 12** como lenguaje principal
- **Microsoft.AspNetCore.Components.WebAssembly** 9.0.5

### **Librerías Frontend**
| **Categoría** | **Tecnología** | **Versión** | **Propósito** |
|---------------|----------------|-------------|---------------|
| **UI Framework** | Bootstrap | 5.x | Sistema de diseño y componentes |
| **Iconos** | Font Awesome | 6.4.0 | Iconografía completa |
| **Interactividad** | JavaScript personalizado | - | Funcionalidades específicas |
| **Archivos** | Custom file handlers | - | Drag & drop, PDF handling |
| **Notificaciones** | Toast notifications | - | Sistema de notificaciones |

### **Archivos JavaScript Personalizados**
- `file-drag-drop.js` - Manejo de archivos
- `notifications.js` - Sistema de notificaciones
- `pdf-generator.js` - Generación y manejo de PDFs
- custom.js - Funcionalidades adicionales

### **Estilos CSS**
- `app.css` - Estilos principales de la aplicación
- `notifications.css` - Estilos para notificaciones
- `proyectoAgiles.styles.css` - Estilos generados automáticamente

---

## 🔧 **BACKEND - ASP.NET CORE API**

### **Framework Principal**
- **ASP.NET Core API** con **.NET 9.0**
- **C# 12** como lenguaje principal
- **Arquitectura Clean Architecture** (Domain, Application, Infrastructure, API)

### **Base de Datos**
| **Tecnología** | **Versión** | **Propósito** |
|----------------|-------------|---------------|
| **SQL Server** | - | Base de datos principal |
| **Entity Framework Core** | 9.0.5 | ORM |
| **EF Core Design** | 9.0.5 | Herramientas de desarrollo |
| **EF Core SqlServer** | 9.0.5 | Proveedor SQL Server |

### **Documentación API**
| **Tecnología** | **Versión** | **Propósito** |
|----------------|-------------|---------------|
| **Swagger/OpenAPI** | 6.8.1 | Documentación interactiva |
| **Swashbuckle.AspNetCore** | 6.8.1 | Generación Swagger |
| **Swashbuckle Annotations** | 6.8.1 | Anotaciones mejoradas |

### **Mapeo de Objetos**
| **Tecnología** | **Versión** | **Propósito** |
|----------------|-------------|---------------|
| **AutoMapper** | 12.0.1 | Mapeo automático entre DTOs y entidades |
| **AutoMapper.Extensions** | 12.0.1 | Extensiones para DI |

### **Seguridad**
| **Tecnología** | **Versión** | **Propósito** |
|----------------|-------------|---------------|
| **BCrypt.Net-Next** | 4.0.3 | Hashing de contraseñas |
| **JWT** | - | Tokens de autenticación |

---

## 🏗️ **ARQUITECTURA Y PATRONES**

### **Arquitectura Clean Architecture**
```
📁 ProyectoAgiles.Domain/          // Entidades y reglas de negocio
📁 ProyectoAgiles.Application/     // Casos de uso y servicios
📁 ProyectoAgiles.Infrastructure/  // Acceso a datos y servicios externos
📁 ProyectoAgiles.Api/            // Controladores y endpoints
📁 proyectoAgiles/                // Frontend Blazor WebAssembly
```

### **Patrones Implementados**
- **Repository Pattern** - Acceso a datos
- **Dependency Injection** - Inyección de dependencias
- **DTO Pattern** - Transfer Objects
- **CQRS Pattern** - Separación comando/consulta
- **Unit of Work** - Manejo de transacciones

---

## 🔧 **HERRAMIENTAS DE DESARROLLO**

### **Desarrollo y Build**
- **.NET 9.0 SDK**
- **Visual Studio 2024** / **VS Code**
- **Entity Framework Core Tools**
- **Swagger UI** personalizado

### **Control de Versiones**
- **Git** (archivos .git*)
- **Migraciones EF Core** automáticas

---

## 📦 **PAQUETES NUGET COMPLETOS**

### **Backend (API)**
```xml
- Microsoft.AspNetCore.OpenApi (9.0.5)
- Microsoft.EntityFrameworkCore (9.0.5)
- Microsoft.EntityFrameworkCore.Design (9.0.5)
- Microsoft.EntityFrameworkCore.SqlServer (9.0.5)
- AutoMapper.Extensions.Microsoft.DependencyInjection (12.0.1)
- Swashbuckle.AspNetCore (6.8.1)
- Swashbuckle.AspNetCore.Annotations (6.8.1)
```

### **Application Layer**
```xml
- AutoMapper (12.0.1)
- BCrypt.Net-Next (4.0.3)
- Microsoft.AspNetCore.Hosting.Abstractions (2.3.0)
- Microsoft.Extensions.Configuration.Abstractions (9.0.5)
```

### **Infrastructure Layer**
```xml
- BCrypt.Net-Next (4.0.3)
- Microsoft.EntityFrameworkCore.Design (9.0.5)
- Microsoft.EntityFrameworkCore.SqlServer (9.0.5)
```

### **Frontend (Blazor)**
```xml
- Microsoft.AspNetCore.Components.WebAssembly (9.0.5)
- Microsoft.AspNetCore.Components.WebAssembly.DevServer (9.0.5)
```

---

## 🌍 **CONFIGURACIÓN Y DEPLOYMENT**

### **Configuración**
- **appsettings.json** - Configuración del API
- **appsettings.Development.json** - Configuración de desarrollo
- **Program.cs** - Configuración de startup
- **CORS** configurado para múltiples puertos

### **Features Habilitadas**
- **Nullable Reference Types** habilitado
- **Implicit Usings** habilitado
- **Documentación XML** automática
- **Hot Reload** en desarrollo

---

## 📊 **ESTADÍSTICAS DEL PROYECTO**

| **Categoría** | **Cantidad** | **Detalle** |
|---------------|--------------|-------------|
| **Controladores** | 9 | APIs REST |
| **Endpoints** | 83+ | Rutas API completas |
| **Entidades** | 10+ | Modelos de dominio |
| **Repositorios** | 8+ | Acceso a datos |
| **Servicios** | 10+ | Lógica de negocio |
| **DTOs** | 50+ | Transfer Objects |
| **Migraciones** | 5+ | Base de datos |

---

## 🎯 **CARACTERÍSTICAS ESPECIALES**

### **Frontend Avanzado**
- ✅ **Single Page Application (SPA)**
- ✅ **Componentes reutilizables**
- ✅ **Gestión de estado avanzada**
- ✅ **Manejo de archivos PDF**
- ✅ **Sistema de notificaciones**
- ✅ **Diseño responsivo**

### **Backend Robusto**
- ✅ **API REST completa**
- ✅ **Documentación Swagger automática**
- ✅ **Arquitectura escalable**
- ✅ **Patrones de diseño**
- ✅ **Seguridad implementada**
- ✅ **Manejo de archivos**

### **Integración**
- ✅ **CORS configurado**
- ✅ **HttpClient para comunicación**
- ✅ **Manejo de errores**
- ✅ **Logging integrado**

Tu proyecto utiliza un **stack tecnológico moderno y completo** con **.NET 9.0**, implementando las mejores prácticas de desarrollo con **Clean Architecture**, **patrones de diseño** y una **experiencia de usuario rica** con **Blazor WebAssembly**.



## 📊 **RESUMEN TOTAL DE APIs EN EL PROYECTO**

### **Total de Controladores: 9**
### **Total de Endpoints: ~75+**

---

## 🔐 **1. AuthController** (`/api/Auth`)
- `POST /api/Auth/register` - Registrar nuevo usuario
- `POST /api/Auth/login` - Iniciar sesión
- `GET /api/Auth/user/{id}` - Obtener usuario por ID
- `GET /api/Auth/check-email/{email}` - Verificar si email existe
- `GET /api/Auth/check-cedula/{cedula}` - Verificar si cédula existe
- `POST /api/Auth/forgot-password` - Recuperar contraseña
- `POST /api/Auth/reset-password` - Restablecer contraseña

**Total: 7 endpoints**

---

## 👥 **2. UsersController** (`/api/Users`)
- `GET /api/Users` - Obtener todos los usuarios
- `GET /api/Users/{id}` - Obtener usuario por ID
- `PUT /api/Users/{id}` - Actualizar usuario
- `DELETE /api/Users/{id}` - Eliminar usuario
- `PATCH /api/Users/{id}/toggle-status` - Alternar estado de usuario
- `POST /api/Users/{id}/subir-nivel` - Subir nivel de usuario
- `GET /api/Users/cedula/{cedula}` - Obtener usuario por cédula
- `POST /api/Users/cedula/{cedula}/subir-nivel` - Subir nivel por cédula

**Total: 8 endpoints**

---

## 🎓 **3. DiticController** (`/api/Ditic`)
- `GET /api/Ditic` - Obtener todas las capacitaciones
- `GET /api/Ditic/{id}` - Obtener capacitación por ID
- `GET /api/Ditic/cedula/{cedula}` - Obtener capacitaciones por cédula
- `GET /api/Ditic/cedula/{cedula}/last-three-years` - Capacitaciones últimos 3 años
- `POST /api/Ditic` - Crear capacitación
- `POST /api/Ditic/with-pdf` - Crear capacitación con PDF
- `PUT /api/Ditic/{id}` - Actualizar capacitación
- `DELETE /api/Ditic/{id}` - Eliminar capacitación
- `GET /api/Ditic/verify-requirement/{cedula}` - Verificar requisitos
- `GET /api/Ditic/summary/{cedula}` - Resumen de capacitaciones
- `GET /api/Ditic/statistics/{cedula}` - Estadísticas de capacitaciones
- `GET /api/Ditic/{id}/certificate` - Descargar certificado
- `PUT /api/Ditic/{id}/certificate` - Actualizar certificado
- `DELETE /api/Ditic/{id}/certificate` - Eliminar certificado
- `POST /api/Ditic/import/{cedula}` - Importar desde sistema externo
- `GET /api/Ditic/search` - Buscar capacitaciones

**Total: 16 endpoints**

---

## 🔬 **4. InvestigacionesController** (`/api/Investigaciones`)
- `GET /api/Investigaciones` - Obtener todas las investigaciones
- `GET /api/Investigaciones/{id}` - Obtener investigación por ID
- `GET /api/Investigaciones/cedula/{cedula}` - Obtener por cédula
- `GET /api/Investigaciones/tipo/{tipo}` - Obtener por tipo
- `GET /api/Investigaciones/campo/{campoConocimiento}` - Obtener por campo
- `POST /api/Investigaciones` - Crear investigación
- `POST /api/Investigaciones/with-pdf` - Crear con PDF
- `PUT /api/Investigaciones/{id}` - Actualizar investigación
- `PUT /api/Investigaciones/{id}/with-pdf` - Actualizar con PDF
- `DELETE /api/Investigaciones/{id}` - Eliminar investigación
- `GET /api/Investigaciones/{id}/pdf` - Obtener PDF

**Total: 11 endpoints**

---

## ⭐ **5. EvaluacionesDesempenoController** (`/api/EvaluacionesDesempeno`)
- `GET /api/EvaluacionesDesempeno` - Obtener todas las evaluaciones
- `GET /api/EvaluacionesDesempeno/{id}` - Obtener por ID
- `GET /api/EvaluacionesDesempeno/cedula/{cedula}` - Obtener por cédula
- `GET /api/EvaluacionesDesempeno/cedula/{cedula}/ultimas-cuatro` - Últimas 4 evaluaciones
- `GET /api/EvaluacionesDesempeno/periodo/{periodoAcademico}` - Por período académico
- `GET /api/EvaluacionesDesempeno/anio/{anio}` - Por año
- `GET /api/EvaluacionesDesempeno/anio/{anio}/semestre/{semestre}` - Por año y semestre
- `POST /api/EvaluacionesDesempeno` - Crear evaluación
- `POST /api/EvaluacionesDesempeno/with-pdf` - Crear con PDF
- `PUT /api/EvaluacionesDesempeno/{id}` - Actualizar evaluación
- `PUT /api/EvaluacionesDesempeno/{id}/with-pdf` - Actualizar con PDF
- `DELETE /api/EvaluacionesDesempeno/{id}` - Eliminar evaluación
- `GET /api/EvaluacionesDesempeno/resumen/{cedula}` - Resumen de evaluaciones
- `GET /api/EvaluacionesDesempeno/verificar-requisito-75/{cedula}` - Verificar requisito 75%
- `GET /api/EvaluacionesDesempeno/que-alcanzan-75` - Evaluaciones que alcanzan 75%
- `GET /api/EvaluacionesDesempeno/cedula/{cedula}/que-alcanzan-75` - Por cédula que alcanzan 75%
- `GET /api/EvaluacionesDesempeno/{id}/pdf` - Obtener PDF
- `GET /api/EvaluacionesDesempeno/estadisticas-generales` - Estadísticas generales
- `GET /api/EvaluacionesDesempeno/existe-periodo/{cedula}/{periodoAcademico}` - Verificar período
- `GET /api/EvaluacionesDesempeno/estadisticas-docente/{cedula}` - Estadísticas del docente

**Total: 20 endpoints**

---

## 📋 **6. SolicitudesEscalafonController** (`/api/SolicitudesEscalafon`)
- `GET /api/SolicitudesEscalafon` - Obtener todas las solicitudes
- `GET /api/SolicitudesEscalafon/{id}` - Obtener por ID
- `GET /api/SolicitudesEscalafon/cedula/{cedula}` - Obtener por cédula
- `GET /api/SolicitudesEscalafon/status/{status}` - Obtener por estado
- `GET /api/SolicitudesEscalafon/pending-count` - Contar pendientes
- `GET /api/SolicitudesEscalafon/pending-count-alt` - Contar pendientes (alternativo)
- `POST /api/SolicitudesEscalafon` - Crear solicitud
- `PUT /api/SolicitudesEscalafon/update-status` - Actualizar estado
- `PUT /api/SolicitudesEscalafon/{id}/update-status` - Actualizar estado por ID
- `DELETE /api/SolicitudesEscalafon/{id}` - Eliminar solicitud
- `GET /api/SolicitudesEscalafon/existe-pendiente/{cedula}` - Verificar pendientes
- `POST /api/SolicitudesEscalafon/{id}/notificar-aprobacion` - Notificar aprobación
- `POST /api/SolicitudesEscalafon/{id}/finalizar` - Finalizar escalafón

**Total: 13 endpoints**

---

## 👨‍🏫 **7. TeacherManagementController** (`/api/TeacherManagement`)
- `POST /api/TeacherManagement/validate-teacher` - Validar docente por cédula
- `POST /api/TeacherManagement/register-teacher` - Registrar docente
- `GET /api/TeacherManagement/external-teachers` - Obtener docentes externos

**Total: 3 endpoints**

---

## 🏢 **8. TTHHController** (`/api/TTHH`)
- `GET /api/TTHH/cedula/{cedula}` - Obtener por cédula
- `GET /api/TTHH` - Obtener todos
- `POST /api/TTHH` - Crear registro TTHH

**Total: 3 endpoints**

---

## 📊 **9. DashboardController** (`/api/Dashboard`)
- `GET /api/Dashboard/stats` - Obtener estadísticas del dashboard
- `GET /api/Dashboard/recent-activities` - Obtener actividades recientes

**Total: 2 endpoints**

---

## 🎯 **RESUMEN FINAL**

| **Controlador** | **Endpoints** | **Funcionalidad Principal** |
|-----------------|---------------|------------------------------|
| AuthController | 7 | Autenticación y autorización |
| UsersController | 8 | Gestión de usuarios |
| DiticController | 16 | Capacitaciones DITIC |
| InvestigacionesController | 11 | Gestión de investigaciones |
| EvaluacionesDesempenoController | 20 | Evaluaciones DAC |
| SolicitudesEscalafonController | 13 | Solicitudes de escalafón |
| TeacherManagementController | 3 | Gestión de docentes |
| TTHHController | 3 | Talento Humano |
| DashboardController | 2 | Dashboard y estadísticas |

### **📈 TOTAL: 83 ENDPOINTS**

Tu proyecto tiene una **API muy completa** con 83 endpoints distribuidos en 9 controladores, cubriendo todas las funcionalidades del sistema académico de escalafón docente.