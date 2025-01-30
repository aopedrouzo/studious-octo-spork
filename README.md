# API de Football Club Manager

Esta es mi solución para la prueba técnica de la empresa que no voy a nombrar para que no aparezca en la búsqueda de Github :)

### Requisitos Previos

- Docker
- Docker Compose

### Ejecutar la Aplicación

Para ejecutar la aplicación usando Docker Compose:

1. Clona el repositorio en tu máquina local.
2. Navega al directorio del proyecto.
3. Ejecuta el siguiente comando para iniciar la aplicación:

   ```bash
   docker-compose up --build
   ```

Este comando construirá las imágenes de Docker e iniciará los servicios definidos en el archivo `docker-compose.yml`.

Una vez que se haya iniciado la aplicación, puedes acceder a la API en `http://localhost:5000/swagger`.

### Testing

La aplicación está configurada para ejecutar los tests unitarios al ejecutar `docker-compose up`. Si fallan, la aplicación no se inicia ni se desplegará.

### Autenticación

La API utiliza JWT para la autenticación. Para poder probar sin tener que hacer login, he añadido un token pre-firmado con valor: 

```bash
eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.eyJuYW1laWQiOiJ0ZXN0LXVzZXIiLCJyb2xlIjoiQWRtaW4iLCJuYmYiOjE3MzgxNjU3NDMsImV4cCI6MjA1MzY5ODU0MywiaWF0IjoxNzM4MTY1NzQzfQ.bF6t9i8YCX0uUL34EGRR9lcYmWFYcaU1wtPOK9DNIjY
```
De todos modos, en el seeding de la base de datos se crea un usuario admin con password 'admin123'.

### Descripción de la Arquitectura

La aplicación sigue una arquitectura por capas basada en Clean Architecture:

- **Capa de Presentación**: Contiene los controladores de la API que manejan las solicitudes y respuestas HTTP.
- **Capa de Aplicación**: Incluye la lógica de negocio y las interfaces de servicio.
- **Capa de Infraestructura**: Gestiona el acceso a datos, la autenticación y los servicios externos.
- **Capa de Dominio**: Define las entidades principales y la lógica de dominio.

La aplicación utiliza Entity Framework Core para el acceso a datos y PostgreSQL como base de datos. Docker Compose se utiliza para orquestar los servicios de la aplicación y la base de datos.

## Información Adicional

- La aplicación expone Swagger UI para la documentación y prueba de la API, accesible en `http://localhost:5000/swagger` cuando se ejecuta localmente.
- La base de datos se migra automáticamente a la última versión al iniciar.

