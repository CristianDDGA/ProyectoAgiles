# SPRINT 2 - PROCESO DE ESCALAFÓN DOCENTE

## MÓDULO DOCENTES

### Sprint Backlog:

**Como** Docente universitario de la UTA  
**Quiero** gestionar completamente mi proceso de escalafón académico desde mi dashboard personal  
**Para** poder verificar requisitos, gestionar mi información académica, crear solicitudes de ascenso y hacer seguimiento transparente de mi progreso profesional

---

### Tareas

#### 1. **Dashboard Principal de Docente (TeacherDashboard.razor)**
- Visualización de nivel académico actual con badge dinámico
- Sección de estadísticas de promoción con dashcards interactivas
- Navegación a todos los módulos de gestión académica
- Sistema de notificaciones en tiempo real

#### 2. **Sistema de Verificación de Requisitos Dinámicos**
- Modal completo de verificación por categorías
- Cálculos automáticos según nivel académico actual
- Integración con RequisitosEscalafonService
- Diagnóstico detallado con porcentajes y recomendaciones

#### 3. **Gestión de Capacitaciones DITIC**
- CRUD completo de capacitaciones con formularios avanzados
- Importación automática desde base de datos DITIC
- Validación de horas totales y pedagógicas
- Cálculo automático de cumplimiento de requisitos
- Gestión de certificados PDF

#### 4. **Gestión de Evaluaciones de Desempeño**
- Importación de evaluaciones desde sistema DAC
- Cálculo automático del 75% en últimas 4 evaluaciones
- CRUD completo con validaciones
- Visualización de puntajes y porcentajes
- Gestión de archivos de respaldo

#### 5. **Gestión de Investigaciones y Publicaciones**
- CRUD de investigaciones académicas
- Verificación automática de filiación UTA
- Gestión de PDFs de publicaciones
- Categorización por tipo y área de conocimiento
- Validación de obras relevantes

#### 6. **Sistema de Solicitudes de Escalafón**
- Creación automática de solicitudes con datos completos
- Seguimiento de estados en tiempo real
- Visualización de historial y observaciones
- Sistema de cancelación para solicitudes en revisión
- Importación de solicitudes existentes

#### 7. **Estadísticas y Resúmenes Académicos**
- Dashboard con métricas de progreso
- Resúmenes por sección (experiencia, obras, evaluaciones, capacitaciones)
- Indicadores visuales de cumplimiento
- Integración con EstadisticasDocenteResponse
- Actualización en tiempo real

**Total de tareas:** 7 módulos principales  
**Líneas de código:** ~8,500 líneas  
**Componentes:** 1 página principal + 25 componentes UI + 6 modales especializados  
**Tecnologías:** Blazor Server, C# .NET 9, Entity Framework Core, Bootstrap 5, JavaScript, CSS personalizado, PDF.js, Chart.js

---

### Criterios de aceptación

#### CA-1: Dashboard Docente Funcional
**DADO** que soy un docente autenticado en el sistema  
**CUANDO** accedo a mi dashboard en la ruta `/docente`  
**ENTONCES** veo mi nivel académico actual, tarjetas de estadísticas actualizadas, y opciones de navegación a todos los módulos de gestión 

#### CA-2: Verificación de Requisitos Automática
**DADO** que necesito conocer mi elegibilidad para ascender de nivel  
**CUANDO** hago clic en "Verificar requisitos" desde mi nivel actual  
**ENTONCES** obtengo un diagnóstico completo con porcentajes exactos, detalles por requisito, y recomendaciones específicas para cumplir faltantes 

#### CA-3: Gestión Completa de Capacitaciones DITIC
**DADO** que necesito gestionar mis capacitaciones para cumplir requisitos  
**CUANDO** accedo al módulo DITIC y realizo operaciones CRUD  
**ENTONCES** puedo importar automáticamente, crear nuevas, editar existentes, y ver en tiempo real si cumplo con las horas requeridas (totales y pedagógicas)

#### CA-4: Evaluaciones de Desempeño con Cálculo del 75%
**DADO** que debo cumplir el 75% en evaluaciones de desempeño  
**CUANDO** importo o gestiono mis evaluaciones DAC  
**ENTONCES** el sistema calcula automáticamente el promedio de las últimas 4 evaluaciones y me indica si cumplo el requisito mínimo 

#### CA-5: Investigaciones con Verificación de Filiación UTA
**DADO** que necesito obras relevantes con filiación UTA  
**CUANDO** gestiono mis investigaciones y publicaciones  
**ENTONCES** el sistema verifica automáticamente la filiación UTA y cuenta las obras que califican para el escalafón

#### CA-6: Solicitudes de Escalafón Automáticas
**DADO** que cumplo con todos los requisitos para ascender  
**CUANDO** creo una solicitud de escalafón desde el modal de requisitos  
**ENTONCES** la solicitud se genera automáticamente con todos mis datos académicos y se envía al flujo administrativo correspondiente

#### CA-7: Seguimiento de Solicitudes en Tiempo Real
**DADO** que he creado solicitudes de escalafón  
**CUANDO** accedo al módulo de solicitudes  
**ENTONCES** veo el estado actual, fechas de cambios, observaciones.

#### CA-8: Importación Automática de Datos Institucionales
**DADO** que tengo información académica en sistemas institucionales (DAC, DITIC)  
**CUANDO** uso las funciones de importación  
**ENTONCES** mis datos se cargan automáticamente sin duplicación, con validaciones correspondientes

#### CA-9: Estadísticas Dinámicas de Progreso
**DADO** que quiero conocer mi progreso académico  
**CUANDO** visualizo las estadísticas en mi dashboard  
**ENTONCES** veo métricas actualizadas por sección con indicadores visuales de cumplimiento de requisitos

#### CA-10: Notificaciones y Feedback Interactivo
**DADO** que realizo operaciones en el sistema  
**CUANDO** ejecuto acciones como guardar, importar, o verificar  
**ENTONCES** recibo notificaciones claras de éxito, error, o información con detalles específicos de la operación realizada

---

## FUNCIONALIDADES TÉCNICAS IMPLEMENTADAS

### Verificación de Requisitos Dinámicos:
- Integración con `RequisitosEscalafonService` para configuración por nivel
- Llamadas a API `VerificarRequisitosEscalafonDinamico`
- Cálculos en tiempo real de cumplimiento por categoría
- Configuración automática según nivel actual del docente

### Gestión de Capacitaciones DITIC:
- CRUD completo con `DiticDto` y validaciones
- Importación desde API `/api/ditic/{cedula}`
- Cálculo automático de horas totales y pedagógicas
- Validación de fechas y tipos de capacitación
- Gestión de certificados PDF con visualización

### Evaluaciones de Desempeño:
- Importación desde sistema DAC
- Cálculo del 75% en últimas 4 evaluaciones
- Validación de puntajes y fechas
- Gestión de archivos de respaldo
- Indicadores visuales de cumplimiento

### Investigaciones y Publicaciones:
- Verificación automática de filiación UTA
- Categorización por tipo y relevancia
- Gestión de PDFs de publicaciones
- Conteo automático de obras válidas
- Validación de fechas de publicación

### Sistema de Solicitudes:
- Creación automática con `CreateSolicitudEscalafonDto`
- Integración con estadísticas del docente
- Seguimiento de estados y transiciones
- Sistema de cancelación controlado
- Historial completo con observaciones

### Estadísticas y Métricas:
- Integración con `EstadisticasDocenteResponse`
- Dashcards con datos en tiempo real
- Indicadores visuales de progreso
- Resúmenes por sección académica
- Actualización automática tras cambios

Este módulo de Docentes proporciona una experiencia completa y automatizada para la gestión del escalafón académico, con todas las validaciones y cálculos necesarios integrados directamente en la interfaz de usuario.

---

## MÓDULO ADMINISTRACIÓN

### Sprint Backlog:

**Como** Administrador del sistema de escalafón académico  
**Quiero** gestionar las solicitudes de escalafón a través de un flujo administrativo estructurado con múltiples niveles de aprobación  
**Para** procesar eficientemente las promociones docentes, mantener trazabilidad completa del proceso y garantizar el cumplimiento de los requisitos institucionales

---

### Tareas

#### 1. **Tarjeta Principal de Talento Humano (TalentoHumano.razor)**
- Página principal con 3 tarjetas especializadas de navegación
- Diseño responsive con gradientes y animaciones
- Información institucional del sistema de TTHH
- Navegación directa a cada nivel administrativo

#### 2. **Presidente de Comisión Académica (PresidenteComisionAcademica.razor)**
- Dashboard de solicitudes pendientes con filtros avanzados
- Modal de detalles con información académica completa del docente
- Sistema de aprobación y rechazo con motivos documentados
- Contadores de solicitudes por estado en tiempo real
- Carga de datos académicos (investigaciones, evaluaciones, capacitaciones)

#### 3. **Dirección de Talento Humano (DireccionTalentoHumano.razor)**
- Gestión de solicitudes aprobadas por el Presidente
- Modal detallado con estadísticas de cumplimiento de requisitos
- Sistema de firma digital de solicitudes procesadas
- Rechazo administrativo con motivos específicos de TTHH
- Visualización completa de datos académicos del docente
- Filtros por estado y búsqueda por nombre/cédula

#### 4. **Comisión Académica de Escalafón (ComisionAcademicaEscalafon.razor)**
- Verificación final de solicitudes firmadas por TTHH
- Generación de informes PDF oficiales con firma institucional
- Sistema de finalización de escalafón con promoción automática
- Modal de verificación con todos los datos académicos
- Registro de auditoría completo del proceso

#### 5. **Sistema de Estados y Flujo Administrativo**
- Transiciones controladas entre estados del escalafón
- Validaciones de permisos por nivel administrativo
- Trazabilidad completa de cambios y responsables
- Notificaciones automáticas entre niveles
- Registro de fechas y observaciones por estado

#### 6. **Visualización de Datos Académicos Integrados**
- Tablas especializadas para investigaciones, evaluaciones y capacitaciones
- Indicadores visuales de cumplimiento de requisitos
- Cálculos automáticos de porcentajes y métricas
- Visualización de PDFs de certificados y documentos
- Resúmenes estadísticos por sección académica

#### 7. **Gestión de Documentación y Reportes**
- Generación automática de informes PDF oficiales
- Sistema de plantillas para documentos institucionales
- Firmas digitales y sellos de aprobación
- Archivo histórico de documentos generados
- Notificaciones por email automáticas

**Total de tareas:** 7 módulos administrativos  
**Líneas de código:** ~6,500 líneas  
**Componentes:** 4 páginas principales + 18 componentes UI + 8 modales especializados + 3 sistemas de reportes  
**Tecnologías:** Blazor Server, C# .NET 9, Entity Framework Core, Bootstrap 5, CSS Grid, JavaScript, PDF generation, Email services

---

### Criterios de aceptación

#### CA-1: Tarjeta de Talento Humano Funcional
**DADO** que soy un administrador autenticado en el sistema  
**CUANDO** accedo al módulo de Talento Humano desde `/admin/talento-humano`  
**ENTONCES** veo 3 tarjetas especializadas que me permiten navegar a Presidente de Comisión, Dirección TTHH, y Comisión Académica

#### CA-2: Presidente de Comisión - Aprobación Inicial
**DADO** que soy Presidente de la Comisión Académica  
**CUANDO** reviso solicitudes pendientes de escalafón  
**ENTONCES** puedo ver detalles completos del docente, aprobar y las solicitudes pasan al siguiente nivel del flujo 

#### CA-3: Dirección TTHH - Procesamiento y Firma
**DADO** que soy de la Dirección de Talento Humano  
**CUANDO** proceso solicitudes aprobadas por el Presidente  
**ENTONCES** puedo revisar estadísticas de cumplimiento, firmar digitalmente la solicitud

#### CA-4: Comisión Académica - Finalización del Escalafón
**DADO** que soy de la Comisión Académica de Escalafón  
**CUANDO** verifico solicitudes firmadas por TTHH  
**ENTONCES** puedo procesar para que el Presidente de la Comisión Académica notifique mediante correo

#### CA-5: Flujo de Estados Controlado
**DADO** que una solicitud progresa por el flujo administrativo  
**CUANDO** cada nivel realiza su acción correspondiente  
**ENTONCES** la solicitud transiciona automáticamente al siguiente estado con trazabilidad completa: Pendiente → Aprobado → Procesado → Verificado → Finalizado

#### CA-6: Visualización Completa de Datos Académicos
**DADO** que necesito revisar la información académica de un docente  
**CUANDO** abro el modal de detalles en cualquier nivel administrativo  
**ENTONCES** veo tablas especializadas con investigaciones, evaluaciones, capacitaciones, y estadísticas de cumplimiento de requisitos

#### CA-7: Generación Automática de Reportes PDF
**DADO** que una solicitud es aprobada finalmente por la Comisión Académica  
**CUANDO** se completa el proceso de escalafón  
**ENTONCES** se genera automáticamente un informe PDF oficial 

#### CA-8: Filtros y Búsqueda Avanzada
**DADO** que manejo múltiples solicitudes en cada nivel administrativo  
**CUANDO** utilizo los filtros por estado o búsqueda por nombre/cédula  
**ENTONCES** puedo localizar rápidamente solicitudes específicas y optimizar mi flujo de trabajo administrativo

#### CA-9: Notificaciones y Trazabilidad Completa
**DADO** que se realizan cambios de estado en las solicitudes  
**CUANDO** cualquier nivel administrativo toma una acción  
**ENTONCES** se registran automáticamente fechas, responsables, observaciones.

---

## FUNCIONALIDADES TÉCNICAS IMPLEMENTADAS - ADMINISTRACIÓN

### Tarjeta de Talento Humano:
- Navegación centralizada con diseño de tarjetas interactivas
- Información institucional del sistema TTHH
- Validación de roles y permisos administrativos
- Diseño responsive con CSS Grid y animaciones

### Presidente de Comisión Académica:
- Dashboard con contadores dinámicos de solicitudes
- Modal de detalles con carga asíncrona de datos académicos
- Sistema de aprobación/rechazo con validaciones
- Integración con APIs de estadísticas del docente
- Filtros avanzados por estado y búsqueda

### Dirección de Talento Humano:
- Procesamiento de solicitudes con firma digital
- Visualización de estadísticas de cumplimiento de requisitos
- Sistema de rechazo específico con motivos de TTHH
- Tablas especializadas para datos académicos
- Indicadores visuales de cumplimiento por categoría

### Comisión Académica de Escalafón:
- Verificación final con generación de reportes PDF
- Sistema de finalización con promoción automática del docente
- Plantillas institucionales para documentos oficiales
- Integración con servicios de email para notificaciones
- Registro de auditoría completo del proceso

### Sistema de Estados y Transiciones:
- Máquina de estados con validaciones de flujo
- Trazabilidad completa con timestamps y responsables
- Validación de permisos por nivel administrativo
- Notificaciones automáticas entre niveles
- Registro histórico de todos los cambios

### Visualización de Datos Integrados:
- Componentes reutilizables para tablas académicas
- Cálculo automático de estadísticas y porcentajes
- Integración con `EstadisticasDocenteResponse`
- Visualización de PDFs con PDF.js
- Indicadores visuales de cumplimiento (✓/✗)

### Gestión de Documentación:
- Generación de PDFs con plantillas HTML/CSS
- Sistema de firmas digitales institucionales
- Archivo automático de documentos generados
- Integración con sistema de archivos del servidor
- Notificaciones por email con adjuntos