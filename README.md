# 🏪 Sistema de Gestión Integral (SGI)
 
 Sistema que te permite manejar empleados, proveedores, productos y clientes. 

## 📋 Lo que hace el sistema

- Gestión de Empleados: Registra empleados con sus datos personales, EPS y ARL
- Gestión de Proveedores: Maneja proveedores y sus descuentos
- Gestión de Productos: Controla el inventario y stock de productos
- Gestión de Clientes: Registra clientes y sus tipos (VIP, Corporativo, etc.)

## 🚀 Cómo usarlo

1. Primero, necesitas tener MySQL instalado en tu computadora

2. Crea una base de datos llamada `dbsgi`:
```sql
CREATE DATABASE dbsgi;
```

3. Copia y pega el contenido del archivo `database/database.sql`  tu MySQL para crear las tablas

4. Copia y pega el contenido del archivo `database/insert_data.sql` y `database/procedures.sql`en  para insertar los datos iniciales

5. En el archivo `appsettings.json`, busca esta línea:
```csharp
string connStr = "server=localhost;database=dbsgi;user=root;password=1234;AllowPublicKeyRetrieval=true;SslMode=none;";
```
Y cambia:
- `user=root` por tu usuario de MySQL
- `password=1234` por tu contraseña de MySQL

6. Compila y ejecuta el programa

## 🎮 Cómo funciona

El programa tiene un menú principal donde puedes elegir qué quieres hacer:
1. Gestión de Productos
2. Gestión de Ventas
3. Gestión de Compras
4. movimientos de caja
5. Gestion de planes promocionale
6. gestión de personas 

Cada sección tiene sus propias opciones para:
- Ver todos los registros
- Agregar nuevos
- Actualizar existentes
- Eliminar registros
- Ver detalles

## ⚠️ Cosas importantes

- Asegúrate de que MySQL esté corriendo antes de ejecutar el programa
- Si te da error de conexión, revisa que el usuario y contraseña sean correctos
- Los datos iniciales son importantes, no te saltes el paso de insertar los datos del `insert_data.sql` y `database/procedures.sql`

## 🛠️ Tecnologías que usé

- C# (.NET)
- MySQL
- Visual Studio (o el IDE que prefieras)

## 📝 Notas

Este proyecto lo hice para aprender sobre:
- Programación orientada a objetos
- Conexión a bases de datos
- Manejo de excepciones
- Validación de datos
- Interfaces y repositorios

## Desarrolladores

- Fabian Andres Ortega Barragan
- Juan Pablo Pinilla Guzman
