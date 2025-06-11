# 🛠️ COMANDOS PARA GENERAR BASE DE DATOS EN NUEVA PC

## ✅ COMANDOS PASO A PASO

### 1. 📋 Verificar Requisitos

```powershell
# Verificar que .NET 8 esté instalado
dotnet --version

# Verificar que Entity Framework CLI esté instalado
dotnet ef --version

# Si no está instalado EF CLI:
dotnet tool install --global dotnet-ef
```

### 2. 🗂️ Navegar al Proyecto

```powershell
# Cambiar al directorio del proyecto API
cd "C:\Users\nixon\OneDrive\Escritorio\proyectoAgiles\ProyectoAgiles.Api"
```

### 3. 🔧 Restaurar Dependencias

```powershell
# Restaurar paquetes NuGet
dotnet restore
```

### 4. 🏗️ Compilar el Proyecto

```powershell
# Compilar para verificar que no hay errores
dotnet build
```

### 5. 🗄️ Generar la Base de Datos

#### Opción A: Usando Entity Framework (RECOMENDADO)

```powershell
# Eliminar base de datos existente (si existe)
dotnet ef database drop --force

# Aplicar todas las migraciones
dotnet ef database update

# Verificar que las migraciones se aplicaron
dotnet ef migrations list
```

#### Opción B: Ejecutar la Aplicación (Automático)

```powershell
# Ejecutar la API (aplicará migraciones automáticamente)
dotnet run
```

### 6. ✅ Verificar la Creación

```powershell
# Verificar conexión a la base de datos
sqlcmd -S "(localdb)\mssqllocaldb" -Q "SELECT name FROM sys.databases WHERE name = 'ProyectoAgilesDB';"

# Si no tienes sqlcmd, puedes usar PowerShell:
# Verificar si LocalDB está corriendo
SqlLocalDB info
SqlLocalDB start mssqllocaldb
```

---

## 🔧 SOLUCIÓN DE PROBLEMAS

### ❌ Error: "dotnet ef command not found"

```powershell
# Instalar EF CLI globalmente
dotnet tool install --global dotnet-ef

# Verificar instalación
dotnet ef --version

# Si sigue sin funcionar, actualizar PATH o reiniciar terminal
```

### ❌ Error: "Unable to create an object of type 'ApplicationDbContext'"

```powershell
# Asegurarse de estar en el directorio correcto
cd "C:\Users\nixon\OneDrive\Escritorio\proyectoAgiles\ProyectoAgiles.Api"

# Compilar el proyecto
dotnet build

# Intentar nuevamente
dotnet ef database update
```

### ❌ Error: "A network-related or instance-specific error occurred"

```powershell
# Verificar e iniciar LocalDB
SqlLocalDB info
SqlLocalDB start mssqllocaldb
SqlLocalDB info mssqllocaldb

# Si no existe, crear la instancia
SqlLocalDB create mssqllocaldb

# Intentar conexión manual
sqlcmd -S "(localdb)\mssqllocaldb" -Q "SELECT @@VERSION;"
```

### ❌ Error: "Login failed for user"

```powershell
# Verificar cadena de conexión en appsettings.json
# Debe ser: Server=(localdb)\\mssqllocaldb;Database=ProyectoAgilesDB;Trusted_Connection=true;MultipleActiveResultSets=true

# Probar con cadena de conexión alternativa (agregar en appsettings.json):
# "Server=(localdb)\\mssqllocaldb;Database=ProyectoAgilesDB;Integrated Security=true;TrustServerCertificate=true;"
```

---

## 🎯 COMANDOS RÁPIDOS (COPY-PASTE)

Para una nueva PC, ejecuta estos comandos EN ORDEN:

```powershell
# 1. Instalar EF CLI (solo una vez)
dotnet tool install --global dotnet-ef

# 2. Navegar al proyecto
cd "C:\Users\nixon\OneDrive\Escritorio\proyectoAgiles\ProyectoAgiles.Api"

# 3. Restaurar y compilar
dotnet restore
dotnet build

# 4. Generar base de datos
dotnet ef database update

# 5. Verificar migraciones
dotnet ef migrations list

# 6. Ejecutar API para probar
dotnet run
```

---

## 📊 QUÉ SE CREA AUTOMÁTICAMENTE

✅ **Base de datos**: `ProyectoAgilesDB`  
✅ **Tablas**:
- `Users` (usuarios del sistema)
- `PasswordResetTokens` (tokens de recuperación)
- `ExternalTeachers` (docentes externos)

✅ **Usuario administrador**:
- Email: `admin@sistema.com`
- Contraseña: `Admin123!`

✅ **Datos de ejemplo**:
- 3 docentes externos de prueba

---

## 🔍 VERIFICAR RESULTADOS

```powershell
# Ver tablas creadas
sqlcmd -S "(localdb)\mssqllocaldb" -d "ProyectoAgilesDB" -Q "SELECT TABLE_NAME FROM INFORMATION_SCHEMA.TABLES;"

# Ver usuario admin
sqlcmd -S "(localdb)\mssqllocaldb" -d "ProyectoAgilesDB" -Q "SELECT Email, FirstName, LastName FROM Users WHERE UserType = 1;"

# Contar registros
sqlcmd -S "(localdb)\mssqllocaldb" -d "ProyectoAgilesDB" -Q "SELECT 'Users' as Tabla, COUNT(*) as Total FROM Users UNION SELECT 'ExternalTeachers', COUNT(*) FROM ExternalTeachers;"
```

---

## ⚡ COMANDOS DE EMERGENCIA

Si nada funciona:

```powershell
# RESET COMPLETO - Eliminar todo y empezar de nuevo
dotnet ef database drop --force
dotnet clean
dotnet restore
dotnet build
dotnet ef database update

# Si LocalDB da problemas, reiniciarlo
SqlLocalDB stop mssqllocaldb
SqlLocalDB delete mssqllocaldb
SqlLocalDB create mssqllocaldb
SqlLocalDB start mssqllocaldb

# Aplicar migraciones nuevamente
dotnet ef database update
```

---

## 🎉 ¡ÉXITO!

Si todo salió bien, deberías poder:

1. ✅ Ver la base de datos en SQL Server Object Explorer
2. ✅ Ejecutar la API en `http://localhost:5200`
3. ✅ Hacer login con `admin@sistema.com` / `Admin123!`
4. ✅ Ver los endpoints en `http://localhost:5200/api/users`

**¡Tu base de datos está lista! 🚀**
