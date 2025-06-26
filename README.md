# 🎓 **ProyectoAgiles - Sistema de Escalafón Docente UTA**

## 🚀 **GUÍA DE INSTALACIÓN Y CONFIGURACIÓN**

### **📋 REQUISITOS PREVIOS**

Antes de comenzar, asegúrate de tener instalado:

| **Software** | **Versión Mínima** | **Descarga** | **Propósito** |
|--------------|-------------------|--------------|---------------|
| **.NET SDK** | 9.0 | [Descargar](https://dotnet.microsoft.com/download) | Framework principal |
| **SQL Server** | 2019+ | [Descargar](https://www.microsoft.com/sql-server/sql-server-downloads) | Base de datos |
| **SQL Server Management Studio** | Última | [Descargar](https://aka.ms/ssmsfullsetup) | Gestión de BD (opcional) |
| **Visual Studio** | 2022+ | [Descargar](https://visualstudio.microsoft.com/) | IDE recomendado |

**Alternativas:**
- **Visual Studio Code** + Extensiones C#
- **SQL Server LocalDB** (incluido con Visual Studio)
- **Azure Data Studio** para gestión de BD

### **✅ VERIFICACIÓN DE ARCHIVOS DEL PROYECTO**

Antes de comenzar, verifica que tengas **TODOS** estos archivos y carpetas:

```
proyectoAgiles/                           # 📁 Carpeta principal del proyecto
├── proyectoAgiles.slnx                   # ✅ OBLIGATORIO - Archivo de solución
├── README.md                             # ✅ OBLIGATORIO - Este archivo
├── ProyectoAgiles.Api/                   # ✅ OBLIGATORIO - Proyecto API Backend
│   ├── ProyectoAgiles.Api.csproj         # ✅ OBLIGATORIO
│   ├── Program.cs                        # ✅ OBLIGATORIO
│   ├── appsettings.json                  # ✅ OBLIGATORIO
│   └── Controllers/                      # ✅ OBLIGATORIO
├── proyectoAgiles/                       # ✅ OBLIGATORIO - Proyecto Frontend Blazor
│   ├── proyectoAgiles.csproj             # ✅ OBLIGATORIO
│   ├── Program.cs                        # ✅ OBLIGATORIO
│   ├── App.razor                         # ✅ OBLIGATORIO
│   └── wwwroot/                          # ✅ OBLIGATORIO
├── ProyectoAgiles.Application/           # ✅ OBLIGATORIO - Capa de aplicación
│   └── ProyectoAgiles.Application.csproj # ✅ OBLIGATORIO
├── ProyectoAgiles.Domain/                # ✅ OBLIGATORIO - Capa de dominio
│   └── ProyectoAgiles.Domain.csproj      # ✅ OBLIGATORIO
└── ProyectoAgiles.Infrastructure/        # ✅ OBLIGATORIO - Capa de infraestructura
    └── ProyectoAgiles.Infrastructure.csproj # ✅ OBLIGATORIO
```

**🚨 IMPORTANTE:** Si falta alguno de estos archivos/carpetas, el proyecto NO funcionará.

---

### **📥 PASO 1: PREPARAR EL PROYECTO**

Si tienes la carpeta completa del proyecto:

```bash
# Extraer/copiar la carpeta del proyecto a tu ubicación deseada
# Navegar al directorio del proyecto
cd ruta/hacia/proyectoAgiles

# Verificar que tienes todos los archivos necesarios
dir  # En Windows
ls   # En Linux/macOS
```

**Archivos y carpetas que DEBES tener:**
- ✅ `proyectoAgiles.slnx` (archivo de solución)
- ✅ `ProyectoAgiles.Api/` (proyecto backend)
- ✅ `proyectoAgiles/` (proyecto frontend)
- ✅ `ProyectoAgiles.Application/`
- ✅ `ProyectoAgiles.Domain/`
- ✅ `ProyectoAgiles.Infrastructure/`

---

### **🗃️ PASO 2: CONFIGURAR BASE DE DATOS**

#### **Opción A: SQL Server LocalDB (Recomendado para desarrollo)**
```bash
# Verificar si LocalDB está disponible
sqllocaldb info

# Si no está disponible, instalar SQL Server Express LocalDB
```

#### **Opción B: SQL Server Completo**
1. Instalar SQL Server
2. Crear una nueva base de datos llamada `ProyectoAgilesDB`
3. Actualizar la cadena de conexión en `appsettings.json`

#### **Configuración de Cadena de Conexión**
Editar `ProyectoAgiles.Api/appsettings.json`:

```json
{
  "ConnectionStrings": {
    // Para LocalDB (por defecto)
    "DefaultConnection": "Server=(localdb)\\mssqllocaldb;Database=ProyectoAgilesDB;Trusted_Connection=true;MultipleActiveResultSets=true"
    
    // Para SQL Server completo (opcional)
    // "DefaultConnection": "Server=localhost;Database=ProyectoAgilesDB;Trusted_Connection=true;MultipleActiveResultSets=true"
    
    // Para SQL Server con autenticación (opcional)
    // "DefaultConnection": "Server=localhost;Database=ProyectoAgilesDB;User Id=tu_usuario;Password=tu_password;MultipleActiveResultSets=true"
  }
}
```

---

### **🔧 PASO 3: RESTAURAR DEPENDENCIAS**

```bash
# Navegar al directorio raíz del proyecto (donde está el archivo .slnx)
cd proyectoAgiles

# Restaurar dependencias de toda la solución
dotnet restore

# Si hay errores, restaurar cada proyecto individualmente:
dotnet restore ProyectoAgiles.Domain/ProyectoAgiles.Domain.csproj
dotnet restore ProyectoAgiles.Application/ProyectoAgiles.Application.csproj
dotnet restore ProyectoAgiles.Infrastructure/ProyectoAgiles.Infrastructure.csproj
dotnet restore ProyectoAgiles.Api/ProyectoAgiles.Api.csproj
dotnet restore proyectoAgiles/proyectoAgiles.csproj

# Compilar toda la solución para verificar dependencias
dotnet build
```

---

### **🗂️ PASO 4: EJECUTAR MIGRACIONES**

```bash
# Navegar al proyecto de API
cd ProyectoAgiles.Api

# Verificar migraciones disponibles
dotnet ef migrations list

# Aplicar migraciones a la base de datos
dotnet ef database update

# Si hay problemas, recrear la base de datos
dotnet ef database drop
dotnet ef database update
```

**Si `dotnet ef` no está instalado:**
```bash
dotnet tool install --global dotnet-ef
```

---

### **⚙️ PASO 5: CONFIGURAR APLICACIONES**

#### **Backend API - Puerto 5200**
Verificar `ProyectoAgiles.Api/Properties/launchSettings.json`:
```json
{
  "profiles": {
    "http": {
      "applicationUrl": "http://localhost:5200"
    }
  }
}
```

#### **Frontend Blazor - Puerto 5043**
Verificar `proyectoAgiles/Properties/launchSettings.json`:
```json
{
  "profiles": {
    "http": {
      "applicationUrl": "http://localhost:5043"
    }
  }
}
```

Verificar `proyectoAgiles/wwwroot/appsettings.json`:
```json
{
  "ApiSettings": {
    "BaseUrl": "http://localhost:5200"
  }
}
```

---

### **🚀 PASO 6: EJECUTAR EL PROYECTO**

#### **Opción A: Ejecutar desde Visual Studio**
1. Abrir `proyectoAgiles.slnx` en Visual Studio
2. Establecer múltiples proyectos de inicio:
   - `ProyectoAgiles.Api`
   - `proyectoAgiles`
3. Presionar `F5` o clic en "Iniciar"

#### **Opción B: Ejecutar desde línea de comandos**

**Terminal 1 - Backend API:**
```bash
cd ProyectoAgiles.Api
dotnet run
```

**Terminal 2 - Frontend Blazor:**
```bash
cd proyectoAgiles
dotnet run
```

#### **Opción C: Ejecutar ambos con un comando**
```bash
# Desde el directorio raíz
dotnet run --project ProyectoAgiles.Api &
dotnet run --project proyectoAgiles
```

---

### **🌐 PASO 7: VERIFICAR INSTALACIÓN**

Una vez ejecutado, verifica que las aplicaciones estén funcionando:

| **Aplicación** | **URL** | **Descripción** |
|----------------|---------|-----------------|
| **Frontend** | http://localhost:5043 | Aplicación Blazor WebAssembly |
| **API** | http://localhost:5200 | API REST Backend |
| **Swagger** | http://localhost:5200/swagger | Documentación de API |

### **✅ URLs de Verificación:**
- **Página de inicio:** http://localhost:5043
- **API Health Check:** http://localhost:5200/api/Dashboard/stats
- **Swagger UI:** http://localhost:5200/swagger/index.html

---

### **👤 PASO 8: DATOS INICIALES**

El sistema incluye datos semilla para comenzar:

#### **Usuario Administrador por Defecto:**
- **Email:** `admin@uta.edu.ec`
- **Contraseña:** `Admin123!`
- **Tipo:** Administrador

#### **Usuario de Prueba:**
- **Email:** `docente@uta.edu.ec`
- **Contraseña:** `Docente123!`
- **Tipo:** Docente

**⚠️ Importante:** Cambiar estas credenciales en producción.

---

### **🛠️ CONFIGURACIONES ADICIONALES**

#### **Configuración de Email (Opcional)**
En `ProyectoAgiles.Api/appsettings.json`:
```json
{
  "EmailSettings": {
    "SmtpHost": "smtp.gmail.com",
    "SmtpPort": 587,
    "SmtpUsername": "tu_email@gmail.com",
    "SmtpPassword": "tu_app_password",
    "EnableSsl": true,
    "FromName": "Sistema UTA",
    "FromEmail": "tu_email@gmail.com"
  }
}
```

#### **Configuración de CORS**
Ya está configurado para desarrollo local en:
- `http://localhost:5043` (Frontend)
- `http://localhost:5022` (Admin)

---

### **🐛 SOLUCIÓN DE PROBLEMAS COMUNES**

#### **❌ Error: "No se puede encontrar el archivo .slnx"**
```bash
# Verificar que estás en el directorio correcto
dir proyectoAgiles.slnx  # Windows
ls proyectoAgiles.slnx   # Linux/macOS

# Si no existe, buscar archivos .sln
dir *.sln*
```

#### **❌ Error: "No se puede restaurar el paquete"**
```bash
# Limpiar cache de NuGet y restaurar
dotnet nuget locals all --clear
dotnet clean
dotnet restore
dotnet build
```

#### **❌ Error de Base de Datos**
```bash
# Recrear base de datos completamente
dotnet ef database drop --force -p ProyectoAgiles.Infrastructure -s ProyectoAgiles.Api
dotnet ef database update -p ProyectoAgiles.Infrastructure -s ProyectoAgiles.Api
```

#### **❌ Error de Dependencias**
```bash
# Restaurar dependencias paso a paso
dotnet clean
dotnet restore ProyectoAgiles.Domain/
dotnet restore ProyectoAgiles.Application/
dotnet restore ProyectoAgiles.Infrastructure/
dotnet restore ProyectoAgiles.Api/
dotnet restore proyectoAgiles/
dotnet build
```

#### **❌ Error de Puertos Ocupados**
- **Puerto 5043 ocupado:** Cambiar en `proyectoAgiles/Properties/launchSettings.json`
- **Puerto 5200 ocupado:** Cambiar en `ProyectoAgiles.Api/Properties/launchSettings.json`
- **También actualizar:** `proyectoAgiles/wwwroot/appsettings.json`

#### **❌ Error de CORS**
- Verificar que el frontend use la URL correcta del backend
- Verificar configuración de CORS en `ProyectoAgiles.Api/Program.cs`

#### **❌ Error de Entity Framework**
```bash
# Instalar/actualizar herramientas EF globalmente
dotnet tool install --global dotnet-ef
dotnet tool update --global dotnet-ef

# Verificar instalación
dotnet ef --version
```

#### **❌ Error: "SDK de .NET no encontrado"**
```bash
# Verificar versión de .NET instalada
dotnet --version

# Descargar .NET 9.0 SDK si no está instalado
# https://dotnet.microsoft.com/download
```

#### **❌ Problemas de Compilación**
```bash
# Verificar que todos los proyectos compilen individualmente
dotnet build ProyectoAgiles.Domain/
dotnet build ProyectoAgiles.Application/
dotnet build ProyectoAgiles.Infrastructure/
dotnet build ProyectoAgiles.Api/
dotnet build proyectoAgiles/
```

---

### **🆘 SI NADA FUNCIONA - REINICIO COMPLETO**

```bash
# 1. Limpiar todo
dotnet clean
rm -rf bin/ obj/  # Linux/macOS
rmdir /s bin obj  # Windows

# 2. Reinstalar herramientas
dotnet tool uninstall --global dotnet-ef
dotnet tool install --global dotnet-ef

# 3. Restaurar desde cero
dotnet restore
dotnet build

# 4. Recrear base de datos
dotnet ef database drop --force -p ProyectoAgiles.Infrastructure -s ProyectoAgiles.Api
dotnet ef database update -p ProyectoAgiles.Infrastructure -s ProyectoAgiles.Api

# 5. Ejecutar
dotnet run --project ProyectoAgiles.Api &
dotnet run --project proyectoAgiles
```

---

## **✅ ¡LISTO PARA DESARROLLAR!**

Si todos los pasos anteriores se completaron exitosamente, tendrás:

- ✅ **Frontend funcionando** en http://localhost:5043
- ✅ **API funcionando** en http://localhost:5200
- ✅ **Base de datos configurada** y con migraciones aplicadas
- ✅ **Swagger disponible** para probar la API
- ✅ **Usuarios de prueba** para hacer login

### **🎯 CHECKLIST FINAL DE VERIFICACIÓN**

Marca cada elemento cuando esté funcionando:

- [ ] **Proyecto compilado:** `dotnet build` ejecuta sin errores
- [ ] **Base de datos creada:** Migraciones aplicadas correctamente
- [ ] **Backend funcionando:** http://localhost:5200/swagger abre correctamente
- [ ] **Frontend funcionando:** http://localhost:5043 carga la aplicación
- [ ] **Login funcional:** Puedes hacer login con `admin@uta.edu.ec` / `Admin123!`
- [ ] **API conectada:** El frontend puede comunicarse con el backend

### **📞 ¿NECESITAS AYUDA?**

Si tienes problemas que no están en la sección de troubleshooting:

1. **Verificar logs:** Revisar la consola donde ejecutaste los comandos
2. **Verificar archivos:** Asegúrate de tener todos los archivos requeridos
3. **Revisar versiones:** Verificar que tienes .NET 9.0 SDK instalado
4. **Revisar puertos:** Asegúrate de que los puertos 5043 y 5200 estén libres

**🎉 ¡El sistema está listo para ser usado y desarrollado!**

---

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

### **📚 COMANDOS ÚTILES PARA DESARROLLO LOCAL**

```bash
# ===== VERIFICACIÓN INICIAL =====
# Verificar versión de .NET
dotnet --version

# Verificar estructura del proyecto
dotnet sln list

# Verificar que todos los proyectos están en la solución
dotnet sln proyectoAgiles.slnx list

# ===== COMPILACIÓN =====
# Compilar toda la solución
dotnet build

# Compilar en modo Release
dotnet build --configuration Release

# Limpiar archivos de compilación
dotnet clean

# ===== DEPENDENCIAS =====
# Ver dependencias de un proyecto
dotnet list ProyectoAgiles.Api/ package

# Actualizar paquetes NuGet
dotnet list package --outdated
dotnet add package [NombrePaquete] --version [Version]

# ===== BASE DE DATOS =====
# Ver todas las migraciones
dotnet ef migrations list -p ProyectoAgiles.Infrastructure -s ProyectoAgiles.Api

# Crear nueva migración
dotnet ef migrations add NombreMigracion -p ProyectoAgiles.Infrastructure -s ProyectoAgiles.Api

# Aplicar migraciones
dotnet ef database update -p ProyectoAgiles.Infrastructure -s ProyectoAgiles.Api

# Ver SQL que se ejecutará
dotnet ef migrations script -p ProyectoAgiles.Infrastructure -s ProyectoAgiles.Api

# Eliminar base de datos
dotnet ef database drop -p ProyectoAgiles.Infrastructure -s ProyectoAgiles.Api

# ===== EJECUCIÓN =====
# Ejecutar backend solamente
dotnet run --project ProyectoAgiles.Api

# Ejecutar frontend solamente
dotnet run --project proyectoAgiles

# Ejecutar en modo watch (recarga automática)
dotnet watch run --project ProyectoAgiles.Api
dotnet watch run --project proyectoAgiles

# ===== PRUEBAS (si existen) =====
# Ejecutar todas las pruebas
dotnet test

# Ejecutar pruebas con detalles
dotnet test --verbosity normal

# ===== PUBLICACIÓN =====
# Publicar backend para producción
dotnet publish ProyectoAgiles.Api/ --configuration Release --output ./publish/api

# Publicar frontend para producción
dotnet publish proyectoAgiles/ --configuration Release --output ./publish/web
```