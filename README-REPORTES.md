# Reporte de Ascensos Docentes - UTA

## Descripción
Esta sección permite la visualización, análisis y gestión de los ascensos docentes en la Universidad Técnica de Ambato. Incluye estadísticas, gráficos interactivos, filtros avanzados, vista previa en modal y descarga de fichas en PDF, todo con la paleta institucional.

---

## Instalaciones y dependencias necesarias

### 1. Blazorise y Blazorise.Charts
- **Blazorise**: Framework de componentes UI para Blazor.
- **Blazorise.Charts**: Para gráficos estadísticos (barras, líneas, etc.).

Instalación por consola en el proyecto Blazor WebAssembly:

```
dotnet add package Blazorise.Bootstrap --version 1.4.0
```
```
dotnet add package Blazorise.Charts --version 1.4.0
```

> Asegúrate de tener la versión compatible con .NET 9.

### 2. Chart.js
- **Chart.js**: Librería JS para renderizar los gráficos.
- Se descarga manualmente la versión 3.9.1 y se coloca en `wwwroot/js/chart.js`.

### 3. Referencias en `index.html`
Agrega en `<head>`:
```html
<script src="js/chart.js"></script>
<script src="_content/Blazorise.Charts/charts.js"></script>
```

Y al final del `<body>`:
```html
<script src="_content/Blazorise.Charts/chart.js/chart.js"></script>
```

### 4. Bootstrap y FontAwesome
- Bootstrap para estilos generales.
- FontAwesome para iconos.

Ya referenciados en el proyecto:
```html
<link rel="stylesheet" href="lib/bootstrap/dist/css/bootstrap.min.css" />
<link rel="stylesheet" href="https://cdnjs.cloudflare.com/ajax/libs/font-awesome/6.4.0/css/all.min.css" />
```

---

## Uso y funcionalidades
- **Estadísticas**: Tarjetas con totales y promedios.
- **Gráfico de barras**: Visualización de ascensos por estado.
- **Filtros**: Por tipo de promoción, ascenso y año.
- **Tabla**: Listado detallado de solicitudes.
- **Vista previa**: Modal con ficha detallada.
- **Descarga PDF**: Generación de ficha en PDF con formato institucional.

---

## Personalización
- Paleta de colores: blanco, plomo (`#722f37`), rojo institucional (`#b02a37`).
- Todos los estilos están en `Pages/ReportesEstadisticas.razor.css`.
- Los scripts de modal y PDF están en `wwwroot/index.html`.

---

## Requisitos
- .NET 9
- Blazor WebAssembly
- Permitir ventanas emergentes para la descarga de PDF.

---

## Contacto
Para dudas o mejoras, contactar al equipo de desarrollo UTA.
