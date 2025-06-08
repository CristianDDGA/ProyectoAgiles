# Sprint Backlog: Sistema de Recuperación de Contraseña

## Historia de Usuario

**Como** usuario registrado de la Universidad Técnica de Ambato que olvidó su contraseña  
**Quiero** solicitar el restablecimiento de mi contraseña mediante mi email o cédula  
**Para** recuperar el acceso a mi cuenta en el sistema académico y administrativo de la universidad

---

## Tareas del Sprint

### 1. **Desarrollo del Componente Frontend de Solicitud de Recuperación**
   - Crear interfaz de usuario para solicitar recuperación
   - Implementar formulario con validación de email/cédula
   - Diseñar layout responsive con tema UTA

### 2. **Sistema de Validación de Usuario Existente**
   - Validar formato de email y cédula ecuatoriana
   - Verificar existencia del usuario en base de datos
   - Mostrar mensajes de confirmación sin revelar información sensible

### 3. **Generación y Envío de Tokens de Recuperación**
   - Generar tokens únicos y seguros con expiración
   - Implementar servicio de envío de emails
   - Crear plantillas de email para recuperación

### 4. **Desarrollo del Componente de Restablecimiento**
   - Crear interfaz para ingresar nueva contraseña
   - Implementar validación de tokens de recuperación
   - Formulario de nueva contraseña con confirmación

### 5. **Integración con Backend API**
   - Desarrollar endpoints de solicitud de recuperación
   - Implementar endpoint de validación de token
   - Crear endpoint para establecer nueva contraseña

### 6. **Servicios de Recuperación Frontend**
   - Desarrollar ForgotPasswordService
   - Implementar manejo de estados de recuperación
   - Gestionar navegación entre pasos del proceso

### 7. **Seguridad y Experiencia de Usuario**
   - Implementar límites de solicitudes de recuperación
   - Crear feedback visual durante el proceso
   - Manejar expiración de tokens y errores

---

## Resumen de Tareas

**Total de tareas:** 7  
**Líneas de código:** 1,400+ líneas  
**Componentes:** 6 componentes principales  
**Tecnologías:** 7 tecnologías core  

### Detalle de Componentes:
1. **ForgotPassword.razor** - Componente de solicitud (220 líneas)
2. **ResetPassword.razor** - Componente de restablecimiento (250 líneas)
3. **ForgotPassword.razor.css** - Estilos de solicitud (400 líneas)
4. **ResetPassword.razor.css** - Estilos de restablecimiento (350 líneas)
5. **ForgotPasswordService.cs** - Servicio frontend (180 líneas)
6. **AuthController.cs** - Endpoints adicionales (200 líneas)

### Tecnologías Utilizadas:
- **Blazor Server** - Framework frontend
- **ASP.NET Core Web API** - Backend
- **Entity Framework Core** - ORM
- **SQL Server** - Base de datos
- **SMTP/SendGrid** - Servicio de email
- **CSS3/SCSS** - Estilos y animaciones
- **JavaScript** - Interacciones del DOM

---

## Criterios de Aceptación

### CA-01: Solicitud de Recuperación con Email
**DADO** que soy un usuario registrado en la página de recuperación de contraseña  
**CUANDO** ingreso mi email registrado y solicito recuperación  
**ENTONCES** el sistema debe enviar un email con instrucciones de restablecimiento

### CA-02: Solicitud de Recuperación con Cédula
**DADO** que soy un usuario registrado en la página de recuperación de contraseña  
**CUANDO** ingreso mi cédula registrada y solicito recuperación  
**ENTONCES** el sistema debe enviar un email al correo asociado con instrucciones

### CA-03: Validación de Usuario No Existente
**DADO** que estoy en la página de recuperación de contraseña  
**CUANDO** ingreso un email o cédula no registrados  
**ENTONCES** debo ver un mensaje genérico de confirmación sin revelar si el usuario existe

### CA-04: Validación de Formato de Datos
**DADO** que estoy completando el formulario de recuperación  
**CUANDO** ingreso datos con formato inválido  
**ENTONCES** debo ver mensajes de error específicos en tiempo real

### CA-05: Recepción de Email con Token
**DADO** que solicité recuperación de contraseña correctamente  
**CUANDO** reviso mi email  
**ENTONCES** debo recibir un mensaje con un enlace único válido por tiempo limitado

### CA-06: Validación de Token Válido
**DADO** que accedo al enlace de recuperación desde mi email  
**CUANDO** el token es válido y no ha expirado  
**ENTONCES** debo ser redirigido al formulario de nueva contraseña

### CA-07: Validación de Token Expirado
**DADO** que accedo al enlace de recuperación desde mi email  
**CUANDO** el token ha expirado  
**ENTONCES** debo ver un mensaje de error y opción para solicitar nuevo enlace

### CA-08: Validación de Token Inválido
**DADO** que accedo a un enlace de recuperación manipulado  
**CUANDO** el token es inválido o no existe  
**ENTONCES** debo ver un mensaje de error de seguridad y ser redirigido al login

### CA-09: Establecimiento de Nueva Contraseña
**DADO** que estoy en el formulario de nueva contraseña con token válido  
**CUANDO** ingreso y confirmo mi nueva contraseña segura  
**ENTONCES** el sistema debe actualizar mi contraseña y confirmar el cambio

### CA-10: Validación de Contraseñas
**DADO** que estoy estableciendo mi nueva contraseña  
**CUANDO** las contraseñas no coinciden o no cumplen criterios de seguridad  
**ENTONCES** debo ver mensajes de error específicos para cada problema

### CA-11: Confirmación de Cambio Exitoso
**DADO** que he establecido exitosamente mi nueva contraseña  
**CUANDO** el proceso se completa  
**ENTONCES** debo ver confirmación del cambio y ser redirigido al login

### CA-12: Límite de Solicitudes
**DADO** que he solicitado recuperación múltiples veces  
**CUANDO** supero el límite permitido en un período de tiempo  
**ENTONCES** debo ver un mensaje indicando que espere antes de solicitar nuevamente

### CA-13: Invalidación de Tokens Usados
**DADO** que he usado exitosamente un token de recuperación  
**CUANDO** intento usar el mismo token nuevamente  
**ENTONCES** el token debe estar invalidado y mostrar mensaje de error

### CA-14: Navegación y Enlaces
**DADO** que estoy en cualquier página del proceso de recuperación  
**CUANDO** necesito volver al login o solicitar ayuda  
**ENTONCES** debo tener enlaces claros y funcionales de navegación

### CA-15: Experiencia Responsive
**DADO** que accedo al proceso de recuperación desde cualquier dispositivo  
**CUANDO** interactúo con los formularios  
**ENTONCES** la interfaz debe adaptarse correctamente y ser completamente funcional

### CA-16: Feedback Visual Durante Procesamiento
**DADO** que estoy enviando cualquier formulario del proceso  
**CUANDO** el sistema está procesando mi solicitud  
**ENTONCES** debo ver indicadores de carga y botones deshabilitados apropiadamente

### CA-17: Seguridad del Email
**DADO** que se envía un email de recuperación  
**CUANDO** el email es generado  
**ENTONCES** debe contener información mínima necesaria sin datos sensibles del usuario

### CA-18: Manejo de Errores de Email
**DADO** que hay problemas con el servicio de email  
**CUANDO** no se puede enviar el email de recuperación  
**ENTONCES** debo ver un mensaje de error apropiado con instrucciones alternativas

---

## Definición de Terminado (DoD)

- [ ] Código desarrollado y revisado
- [ ] Pruebas unitarias implementadas
- [ ] Validaciones frontend y backend funcionando
- [ ] Servicio de email configurado y probado
- [ ] Interfaz responsive en múltiples dispositivos
- [ ] Documentación técnica actualizada
- [ ] Criterios de aceptación validados
- [ ] Seguridad implementada (tokens seguros, limitación de solicitudes)
- [ ] Manejo adecuado de errores y excepciones
- [ ] Plantillas de email diseñadas y probadas
- [ ] Navegación entre componentes funcionando
- [ ] Validación de expiración de tokens
- [ ] Logs de seguridad implementados

---

**Sprint Goal:** Entregar un sistema completo y seguro de recuperación de contraseñas que permita a los usuarios de la UTA restablecer su acceso de forma intuitiva y protegida.

**Sprint Duration:** 2 semanas  
**Story Points:** 18 puntos
