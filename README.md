# SendMails

Sistema de envío automatizado de correos electrónicos desarrollado en .NET 8 con arquitectura limpia (Clean Architecture).

## Descripción

SendMails es un workerbackground que procesa y envía correos electrónicos de manera automatizada desde una base de datos SQLite. El servicio consulta registros pendientes, genera correos HTML personalizados y los envía mediante SMTP.

## Arquitectura del Proyecto

```
SendMails/
├── SendMails.Domain/          # Entidades e interfaces del dominio
├── SendMails.Application/      # Servicios de lógica de negocio
├── SendMails.Infraestructure/ # Implementaciones (repositorios, email service)
└── SendMails.Worker/          # Worker background (punto de entrada)
```

## Características

- **Procesamiento automatizado**: Envío de emails en intervalos configurables
- **Plantillas HTML**: Correos personalizados con formato profesional
- **Base de datos SQLite**: Almacenamiento de contactos y estado de envíos
- **Logging**: Registro detallado de operaciones
- **Configuración flexible**: Intervalo de ejecución y cantidad de emails por ciclo

## Requisitos

- .NET 8 SDK
- Windows (para ejecución)
- Acceso a servidor SMTP para envío de correos

## Configuración

Edita el archivo `SendMails.Worker/appsettings.json`:

```json
{
  "EmailConfig": {
    "IntervaloMinutos": 60,
    "CantidadEmailsPorEjecucion": 10,
    "Smtp": {
      "Servidor": "smtp.tuservidor.com",
      "Puerto": 587,
      "Usuario": "tu@email.com",
      "Contraseña": "tu_contraseña",
      "UsarSSL": true
    }
  }
}
```

## Estructura de la Base de Datos

La tabla `Lista` contiene los siguientes campos:

| Campo | Descripción |
|-------|-------------|
| Id | Identificador único |
| Nombre | Nombre del contacto |
| Sector | Sector/Industria |
| Email | Correo electrónico |
| Web | Sitio web |
| Telefono | Teléfono de contacto |
| Direccion | Dirección |
| Ciudad | Ciudad |
| EmailEnviado | Estado del envío |
| FechaEnvio | Fecha de envío |

## Ejecución

```bash
dotnet run --project SendMails.Worker
```

## Licencia

MIT License
