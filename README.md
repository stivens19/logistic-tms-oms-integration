# Logistic TMS to OMS Integration Service

Integración empresarial orientado a eventos para sincronizar en tiempo real el ciclo de vida de pedidos entre un TMS externo (ej. Beetrack) y el OMS interno de la compañía, implementando persistencia de estados, auditoría, almacenamiento de evidencias, resiliencia con reintentos y notificaciones multiformato a clientes finales.

---

## 1. Tecnologías y Librerías

* **Plataforma base:** .NET 10 (C#)
* **Framework Web:** ASP.NET Core Web API (Controladores REST)
* **Documentación de API:** Swashbuckle / Swagger OpenAPI
* **Canales en memoria (Buffer / Colas):** `System.Threading.Channels` para desacoplamiento asíncrono y nivelación de carga
* **Resiliencia y Reintentos:** `Polly` (`WaitAndRetryAsync` con retroceso exponencial)
* **Cliente HTTP:** `Microsoft.Extensions.Http` (`IHttpClientFactory`) para consumo de Webhooks externos
* **Configuración:** Options Pattern (`Microsoft.Extensions.Options`) con soporte para `appsettings.json` y variables de entorno

---

## 2. Artefactos del Proyecto (Estructura de la Solución)

El proyecto está diseñado bajo los principios de **Clean Architecture** (Arquitectura Limpia), separando responsabilidades en capas independientes:

```text
Scharff.Test/
├── Scharff.Test.Domain/               # Capa de Dominio (Pura, sin dependencias externas)
│   ├── Constants/
│   │   └── OrderStatus.cs             # Catálogo de estados, estados terminales y reglas de hitos
│   └── Entities/
│       ├── Order.cs                   # Entidad rica con reglas de visitas y auto-retorno
│       └── OrderHistory.cs            # Modelo inmutable de auditoría y tracking histórico
│
├── Scharff.Test.Application/          # Capa de Aplicación (Casos de uso y contratos)
│   ├── Common/
│   │   └── CustomDateTimeConverter.cs # Conversor flexible de fechas (Formatos ISO y TMS con espacios)
│   ├── Configuration/
│   │   └── IntegrationOptions.cs      # Mapeo de variables de entorno y parámetros
│   ├── DTOs/
│   │   └── TmsEventDto.cs             # Contratos de datos de entrada del Webhook
│   ├── Interfaces/
│   │   ├── IOrderRepository.cs        # Contrato de persistencia de órdenes e historial
│   │   ├── ICloudStorageService.cs    # Contrato para subida de evidencias digitales
│   │   └── IClientNotifier.cs         # Contrato para la estrategia de notificación multicliente
│   └── UseCases/
│       └── ProcessTmsEventUseCase.cs  # Orquestador del flujo y lógica de negocio
│
├── Scharff.Test.Infrastructure/       # Capa de Infraestructura (Implementaciones técnicas y mocks)
│   ├── Notifications/
│   │   ├── TiendasPeruanasNotifier.cs # Notificador con estructura personalizada (ClientCode: 01021755)
│   │   └── DefaultClientNotifier.cs   # Notificador genérico de respaldo
│   ├── Persistence/
│   │   └── InMemoryOrderRepository.cs # Almacenamiento thread-safe (ConcurrentDictionary / ConcurrentBag)
│   ├── Queues/
│   │   └── InMemoryEventQueue.cs      # Cola desacoplada basada en Channels
│   ├── Storage/
│   │   └── CloudStorageMockService.cs # Simulación de subida a Azure Blob Storage / AWS S3
│   └── Workers/
│       └── EventProcessorWorker.cs    # BackgroundService consumidor con políticas Polly
│
└── Scharff.Test.Api/                  # Capa de Entrada / Presentación
    ├── Controllers/
    │   ├── TmsWebhookController.cs    # Recepción rápida de webhooks (HTTP 202 Accepted)
    │   └── OrdersController.cs        # Consulta de estado actual e histórico de auditoría
    ├── Program.cs                     # Inyección de dependencias y configuración del pipeline HTTP
    └── appsettings.json               # Configuración base del sistema