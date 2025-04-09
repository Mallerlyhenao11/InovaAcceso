# InovaAcceso 🔒

## Descripción
InovaAcceso es una aplicación web desarrollada en ASP.NET Core que permite la gestión de personas en una organización. La aplicación se enfoca principalmente en el control de entrada y salida de los empleados a través de la identificación por huella digital. 🖐️

Algunas de las principales funcionalidades de la aplicación incluyen:

- ✍️ Registro y gestión de personas
- 🔑 Asignación de roles y permisos
- 📧 Envío de correos electrónicos con credenciales de acceso
- 📍 Control de entrada y salida de los empleados mediante huella digital

## Requisitos del Sistema
- ⚙️ .NET Core SDK 6.0 o superior
- 🗄️ SQL Server (o cualquier otra base de datos compatible con EF Core)
- 📧 Servicio de correo electrónico (SMTP)
- 🖱️ Lector de huellas digitales compatible con la aplicación

## Configuración del Proyecto

1. Clona el repositorio del proyecto:
   ```
   git clone https://github.com/Mallerlyhenao11/InovaAcceso.git
   ```

2. Abre el proyecto en tu IDE de preferencia (Visual Studio, Visual Studio Code, etc.).

3. Configura la cadena de conexión a la base de datos en el archivo `appsettings.json`:
   ```json
   "ConnectionStrings": {
     "DefaultConnection": "Server=localhost;Database=InovaAcceso;User Id=sa;Password=tu-contraseña;"
   }
   ```
   Reemplaza los valores de `Server`, `Database`, `User Id` y `Password` con los datos de tu entorno.

4. Configura los ajustes del servicio de correo electrónico en el archivo `appsettings.json`:
   ```json
   "EmailSettings": {
     "Host": "smtp.example.com",
     "Port": 587,
     "Username": "tu-usuario@example.com",
     "Password": "tu-contraseña"
   }
   ```
   Reemplaza los valores con los datos de tu servicio de correo electrónico.

5. Configura los ajustes del lector de huellas digitales en el archivo `appsettings.json`:
   ```json
   "FingerprintReaderSettings": {
     "DeviceId": "1234",
     "SerialPort": "COM3"
   }
   ```
   Reemplaza los valores de `DeviceId` y `SerialPort` con los datos de tu lector de huellas digitales.

6. Ejecuta las migraciones de la base de datos:
   ```
   dotnet ef database update
   ```
   Este comando creará las tablas necesarias en tu base de datos.

7. Compila y ejecuta la aplicación:
   ```
   dotnet run
   ```
   La aplicación estará disponible en `http://localhost:5000` o `http://localhost:5001` (si usas HTTPS).

## Uso de la Aplicación

Sigue estos pasos para utilizar la aplicación:

1. Accede a la aplicación en tu navegador web.
2. Inicia sesión con las credenciales de administrador:
   - Usuario: admin@example.com
   - Contraseña: admin123
3. Navega por las diferentes secciones de la aplicación:
   - 👥 Gestión de Personas
   - 🔒 Asignación de Roles y Permisos
   - 📍 Control de Entrada y Salida por Huella Digital
   - 📧 Envío de Correos Electrónicos
4. Realiza las operaciones necesarias, como agregar, editar o eliminar personas, asignar roles y permisos, y enviar correos electrónicos con credenciales de acceso.
5. Utiliza el lector de huellas digitales para registrar la entrada y salida de los empleados.

