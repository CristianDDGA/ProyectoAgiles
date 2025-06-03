# Sprint Backlog: Sistema de Registro

## Historia de Usuario

**Como** usuario de la Universidad Técnica de Ambato  
**Quiero** registrarme en el sistema con mis datos personales y documento de identidad  
**Para** acceder a los servicios académicos y administrativos de la universidad

---

## Tareas del Sprint

### 1. **Desarrollo del Componente Frontend de Registro**
   - Crear interfaz de usuario con formulario completo
   - Implementar validación de campos en tiempo real
   - Diseñar layout responsivo con tema UTA

### 2. **Sistema de Carga y Validación de Archivos**
   - Implementar drag & drop para documentos
   - Validar tipos de archivo (PDF, imágenes)
   - Generar vista previa de archivos cargados

### 3. **Validación de Cédula en Tiempo Real**
   - Verificar formato de cédula ecuatoriana
   - Consultar disponibilidad en base de datos
   - Mostrar mensajes de estado inmediatos

### 4. **Desarrollo del Backend API**
   - Crear endpoints de registro
   - Implementar validaciones de negocio
   - Manejar almacenamiento de archivos

### 5. **Servicios de Autenticación**
   - Desarrollar servicio frontend AuthService
   - Implementar servicio backend de autenticación
   - Gestión de DTOs y validaciones

### 6. **Base de Datos y Migraciones**
   - Diseñar esquema de usuarios
   - Crear migraciones de Entity Framework
   - Configurar repositorios

### 7. **Styling y UX/UI**
   - Implementar diseño visual completo
   - Animaciones y efectos de interacción
   - Temas y responsive design

---

## Resumen de Tareas

**Total de tareas:** 7  
**Líneas de código:** 2,500+ líneas  
**Componentes:** 8 componentes principales  
**Tecnologías:** 6 tecnologías core  

### Detalle de Componentes:
1. **Register.razor** - Componente principal (490 líneas)
2. **Register.razor.css** - Estilos (1,600 líneas)
3. **AuthService.cs** (Frontend) - Servicio cliente (200 líneas)
4. **AuthController.cs** - API Controller (150 líneas)
5. **AuthService.cs** (Backend) - Servicio servidor (120 líneas)
6. **UserDtos.cs** - Objetos de transferencia (80 líneas)
7. **Migraciones** - Scripts de base de datos (100 líneas)
8. **Repositorios** - Acceso a datos (80 líneas)

### Tecnologías Utilizadas:
- **Blazor Server** - Framework frontend
- **ASP.NET Core Web API** - Backend
- **Entity Framework Core** - ORM
- **SQL Server** - Base de datos
- **CSS3/SCSS** - Estilos y animaciones
- **JavaScript** - Interacciones del DOM

---

## Criterios de Aceptación

### CA-01: Registro de Usuario Básico
**DADO** que soy un nuevo usuario en la página de registro  
**CUANDO** completo todos los campos obligatorios (nombre, email, contraseña, cédula)  
**ENTONCES** el sistema debe validar los datos y crear mi cuenta exitosamente

### CA-02: Validación de Campos
**DADO** que estoy llenando el formulario de registro  
**CUANDO** ingreso datos inválidos en cualquier campo  
**ENTONCES** debo ver mensajes de error específicos y claros en tiempo real

### CA-03: Verificación de Cédula
**DADO** que ingreso mi número de cédula en el formulario  
**CUANDO** el sistema valida la cédula  
**ENTONCES** debe verificar que no esté registrada previamente y mostrar el estado de disponibilidad

### CA-04: Carga de Documento de Identidad
**DADO** que necesito subir mi documento de identidad  
**CUANDO** arrastro y suelto un archivo válido o lo selecciono mediante el botón  
**ENTONCES** el sistema debe aceptar archivos PDF, JPG, PNG, GIF, BMP menores a 10MB y mostrar vista previa

### CA-05: Validación de Contraseñas
**DADO** que ingreso mi contraseña y confirmación  
**CUANDO** las contraseñas no coinciden  
**ENTONCES** debo ver un mensaje de error indicando que las contraseñas no son iguales

### CA-06: Manejo de Errores
**DADO** que ocurre un error durante el registro  
**CUANDO** hay problemas de conectividad o validación del servidor  
**ENTONCES** debo ver mensajes de error descriptivos y la opción de reintentar

### CA-07: Registro Exitoso
**DADO** que he completado correctamente el formulario de registro  
**CUANDO** envío la información y es procesada exitosamente  
**ENTONCES** debo ver un mensaje de confirmación y la opción de ir al login

### CA-08: Prevención de Duplicados
**DADO** que intento registrarme con una cédula ya existente  
**CUANDO** el sistema detecta el duplicado  
**ENTONCES** debe bloquear el registro y mostrar un mensaje de error apropiado

### CA-09: Experiencia de Usuario
**DADO** que estoy usando el formulario en cualquier dispositivo  
**CUANDO** interactúo con los elementos  
**ENTONCES** la interfaz debe ser responsive, intuitiva y mostrar feedback visual apropiado

### CA-10: Seguridad de Datos
**DADO** que envío mi información personal  
**CUANDO** los datos son transmitidos al servidor  
**ENTONCES** deben ser encriptados y almacenados de forma segura en la base de datos

---

## Definición de Terminado (DoD)

- [ ] Código desarrollado y revisado
- [ ] Pruebas unitarias implementadas
- [ ] Validaciones frontend y backend funcionando
- [ ] Interfaz responsive en múltiples dispositivos
- [ ] Documentación técnica actualizada
- [ ] Criterios de aceptación validados
- [ ] Seguridad implementada (encriptación, validaciones)
- [ ] Base de datos migrada correctamente

---

**Sprint Goal:** Entregar un sistema de registro completo y funcional que permita a los usuarios de la UTA crear cuentas de forma segura e intuitiva.

**Sprint Duration:** 2 semanas  
**Story Points:** 21 puntos
