# Base de Datos con Entity Framework - Proyecto Ágiles

## ✅ Implementación Completada

Se ha creado exitosamente una base de datos utilizando Entity Framework Core con las siguientes características:

### 🏗️ Arquitectura Implementada
- **Clean Architecture** con separación de capas:
  - `ProyectoAgiles.Domain`: Entidades y interfaces
  - `ProyectoAgiles.Application`: Servicios y DTOs
  - `ProyectoAgiles.Infrastructure`: Repositorios y DbContext
  - `ProyectoAgiles.Api`: Controladores y configuración

### 👥 Sistema de Usuarios
- **2 Tipos de Usuario**: Admin y Docente
- **Registro** y **Login** implementados
- **Hashing de contraseñas** con BCrypt
- **Validaciones** de datos
- **Soft Delete** implementado

### 🗄️ Base de Datos
- **SQL Server LocalDB** configurado
- **Migraciones** aplicadas correctamente
- **Usuario administrador** creado por defecto:
  - Email: `admin@sistema.com`
  - Contraseña: `Admin123!`
  - Tipo: Admin

### 🛡️ Seguridad
- Contraseñas hasheadas con BCrypt
- Validación de emails únicos
- Validaciones de entrada robustas
- Filtro global para soft delete

### 🌐 API REST
- **Endpoints de autenticación**:
  - `POST /api/auth/register` - Registro de usuarios
  - `POST /api/auth/login` - Inicio de sesión
  - `GET /api/auth/user/{id}` - Obtener usuario por ID
  - `GET /api/auth/check-email/{email}` - Verificar email

- **Endpoints de gestión de usuarios**:
  - `GET /api/users` - Listar todos los usuarios
  - `GET /api/users/{id}` - Obtener usuario específico
  - `PUT /api/users/{id}` - Actualizar usuario
  - `DELETE /api/users/{id}` - Eliminar usuario (soft delete)
  - `PATCH /api/users/{id}/toggle-status` - Activar/desactivar usuario

### 📊 Estructura de la Base de Datos

#### Tabla Users
```sql
- Id (int, PK, Identity)
- FirstName (nvarchar(100), required)
- LastName (nvarchar(100), required)
- Email (nvarchar(255), required, unique)
- PasswordHash (nvarchar(255), required)
- UserType (int, required) -- 1=Admin, 2=Docente
- PhoneNumber (nvarchar(20), optional)
- IsActive (bit, default true)
- CreatedAt (datetime2, required)
- UpdatedAt (datetime2, optional)
- IsDeleted (bit, default false)
```

### 🚀 Cómo Usar

1. **La API está ejecutándose en**: `http://localhost:5200`

2. **Para probar los endpoints**, usa el archivo `test-api.http` creado

3. **Usuario administrador por defecto**:
   - Email: `admin@sistema.com`
   - Contraseña: `Admin123!`

4. **Para registrar un docente**, usa el endpoint de registro con `userType: 2`

### 🔧 Configuración Adicional

- **Cadena de conexión**: Configurada en `appsettings.json`
- **CORS**: Configurado para aplicación Blazor
- **Migraciones**: Aplicadas automáticamente en desarrollo
- **Logging**: Configurado para desarrollo

### 📝 Próximos Pasos Sugeridos

1. Implementar autenticación JWT para sesiones
2. Agregar roles y permisos más granulares
3. Implementar validaciones adicionales
4. Agregar logs de auditoría
5. Implementar paginación para listados
6. Agregar filtros de búsqueda

¡La base de datos y API están listas para ser utilizadas! 🎉
