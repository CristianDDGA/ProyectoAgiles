# Sprint Backlog: Sistema de Login

## Historia de Usuario

**Como** usuario registrado de la Universidad Técnica de Ambato  
**Quiero** iniciar sesión en el sistema con mis credenciales (email/cédula y contraseña)  
**Para** acceder a los servicios académicos y administrativos de la universidad de forma segura

---

## Tareas del Sprint

### 1. **Desarrollo del Componente Frontend de Login**
   - Crear interfaz de usuario con formulario de autenticación
   - Implementar validación de campos en tiempo real
   - Diseñar layout responsive con tema UTA

### 2. **Sistema de Validación de Credenciales**
   - Validar formato de email y cédula ecuatoriana
   - Implementar validación de contraseña
   - Mostrar mensajes de error específicos

### 3. **Funcionalidad "Recordar Sesión"**
   - Implementar checkbox "Mantener sesión iniciada"
   - Gestionar tokens de autenticación persistentes
   - Configurar expiración de sesiones

### 4. **Integración con Backend API**
   - Consumir endpoints de autenticación
   - Manejar respuestas de login exitoso/fallido
   - Gestionar tokens JWT y almacenamiento seguro

### 5. **Servicios de Autenticación Frontend**
   - Actualizar AuthService con métodos de login
   - Implementar manejo de estado de usuario
   - Gestionar redirecciones post-login

### 6. ****
   - Implementar protección contra ataques de fuerza bruta
   - Manejar intentos fallidos de login
   - Mostrar mensajes de error apropiados

### 7. **Navegación y Experiencia de Usuario**
   - Implementar redirección automática después del login
   - Enlaces a registro y recuperación de contraseña
   - Feedback visual durante el proceso de autenticación

---

## Resumen de Tareas

**Total de tareas:** 7  
**Líneas de código:** 1,200+ líneas  
**Componentes:** 5 componentes principales  
**Tecnologías:** 6 tecnologías core  

### Detalle de Componentes:
1. **Login.razor** - Componente principal (280 líneas)
2. **Login.razor.css** - Estilos (650 líneas)
3. **AuthService.cs** (Frontend) - Métodos de login (150 líneas)
4. **AuthController.cs** - Endpoints de login (120 líneas)
5. **Validaciones y Seguridad** - Lógica adicional (100 líneas)

### Tecnologías Utilizadas:
- **Blazor Server** - Framework frontend
- **ASP.NET Core Web API** - Backend
- **Entity Framework Core** - ORM
- **SQL Server** - Base de datos
- **CSS3/SCSS** - Estilos y animaciones
- **JavaScript** - Interacciones del DOM

---

## Criterios de Aceptación

### CA-01: Login Básico con Email
**DADO** que soy un usuario registrado en la página de login  
**CUANDO** ingreso mi email y contraseña correctos  
**ENTONCES** el sistema debe autenticarme y redirigirme al dashboard principal

### CA-02: Login con Cédula
**DADO** que soy un usuario registrado en la página de login  
**CUANDO** ingreso mi cédula y contraseña correctos  
**ENTONCES** el sistema debe autenticarme exitosamente permitiendo el acceso

### CA-03: Validación de Credenciales Incorrectas
**DADO** que estoy en la página de login  
**CUANDO** ingreso credenciales incorrectas (email/cédula o contraseña)  
**ENTONCES** debo ver un mensaje de error claro sin revelar qué dato específico es incorrecto

### CA-04: Validación de Campos Vacíos
**DADO** que estoy en el formulario de login  
**CUANDO** intento enviar el formulario con campos vacíos  
**ENTONCES** debo ver mensajes de validación indicando los campos requeridos

### CA-05: Validación de Formato de Email
**DADO** que ingreso un email en el campo de usuario  
**CUANDO** el formato del email es inválido  
**ENTONCES** debo ver un mensaje de error de formato en tiempo real

### CA-06: Validación de Formato de Cédula
**DADO** que ingreso una cédula en el campo de usuario  
**CUANDO** el formato de la cédula ecuatoriana es inválido  
**ENTONCES** debo ver un mensaje de error de formato específico

### CA-07: Funcionalidad "Recordar Sesión"
**DADO** que marco la opción "Mantener sesión iniciada"  
**CUANDO** cierro y reabro el navegador  
**ENTONCES** mi sesión debe permanecer activa sin necesidad de login nuevamente

### CA-08: Protección contra Fuerza Bruta
**DADO** que fallo el login múltiples veces consecutivas  
**CUANDO** supero el límite de intentos permitidos  
**ENTONCES** mi cuenta debe bloquearse temporalmente con mensaje informativo

### CA-09: Enlaces de Navegación
**DADO** que estoy en la página de login  
**CUANDO** necesito registrarme u olvidé mi contraseña  
**ENTONCES** debo tener enlaces visibles y funcionales a "Crear cuenta" y "¿Olvidaste tu contraseña?"

### CA-10: Experiencia Responsive
**DADO** que accedo al login desde cualquier dispositivo  
**CUANDO** interactúo con el formulario  
**ENTONCES** la interfaz debe adaptarse correctamente y ser completamente funcional

### CA-11: Feedback Visual
**DADO** que estoy enviando el formulario de login  
**CUANDO** el sistema está procesando mi solicitud  
**ENTONCES** debo ver un indicador de carga y el botón debe deshabilitarse temporalmente

### CA-12: Redirección Post-Login
**DADO** que me autentico exitosamente  
**CUANDO** el login es completado  
**ENTONCES** debo ser redirigido automáticamente a la página principal o la URL que intentaba acceder

### CA-13: Manejo de Sesiones Expiradas
**DADO** que mi sesión ha expirado  
**CUANDO** intento acceder a una página protegida  
**ENTONCES** debo ser redirigido al login con un mensaje informativo sobre la expiración

### CA-14: Seguridad de Contraseñas
**DADO** que ingreso mi contraseña  
**CUANDO** escribo en el campo de contraseña  
**ENTONCES** los caracteres deben estar ocultos con la opción de mostrar/ocultar contraseña

---

## Definición de Terminado (DoD)

- [ ] Código desarrollado y revisado
- [ ] Pruebas unitarias implementadas
- [ ] Validaciones frontend y backend funcionando
- [ ] Interfaz responsive en múltiples dispositivos
- [ ] Documentación técnica actualizada
- [ ] Criterios de aceptación validados
- [ ] Seguridad implementada (tokens JWT, protección contra ataques)
- [ ] Manejo adecuado de errores y excepciones
- [ ] Enlaces de navegación funcionando correctamente
- [ ] Funcionalidad de "recordar sesión" probada

---

**Sprint Goal:** Entregar un sistema de login completo, seguro y funcional que permita a los usuarios registrados de la UTA acceder al sistema de forma intuitiva y protegida.

**Sprint Duration:** 1.5 semanas  
**Story Points:** 13 puntos
