-- Limpiar datos existentes (en orden inverso a las dependencias)
DELETE FROM MovCaja;
DELETE FROM TipoMovCaja;
DELETE FROM PlanProductos;
DELETE FROM Planes;
DELETE FROM Detalle_Venta;
DELETE FROM Venta;
DELETE FROM Cliente;
DELETE FROM Facturacion;
DELETE FROM Detalle_Compra;
DELETE FROM Compras;
DELETE FROM Producto_Proveedor;
DELETE FROM Proveedor;
DELETE FROM Productos;
DELETE FROM Tercero_Telefonos;
DELETE FROM Empleado;
DELETE FROM Terceros;
DELETE FROM TipoDocumento;
DELETE FROM TipoTercero;
DELETE FROM EPS;
DELETE FROM ARL;
DELETE FROM Ciudad;
DELETE FROM Region;
DELETE FROM Pais;

-- Insertar países
INSERT INTO Pais (id, nombre) VALUES 
(1, 'Colombia'),
(2, 'México'),
(3, 'Argentina'),
(4, 'Chile'),
(5, 'Perú'),
(6, 'Ecuador'),
(7, 'Venezuela'),
(8, 'Brasil'),
(9, 'Uruguay'),
(10, 'Paraguay'),
(11, 'Bolivia'),
(12, 'Panamá'),
(13, 'Costa Rica'),
(14, 'Guatemala'),
(15, 'Honduras'),
(16, 'El Salvador'),
(17, 'Nicaragua'),
(18, 'República Dominicana'),
(19, 'Puerto Rico'),
(20, 'Cuba');

-- Insertar regiones
INSERT INTO Region (id, nombre, pais_id) VALUES 
(1, 'Antioquia', 1),
(2, 'Cundinamarca', 1),
(3, 'Valle del Cauca', 1),
(4, 'Atlántico', 1),
(5, 'Bolívar', 1),
(6, 'Santander', 1),
(7, 'Boyacá', 1),
(8, 'Cauca', 1),
(9, 'Nariño', 1),
(10, 'Córdoba', 1),
(11, 'Magdalena', 1),
(12, 'Cesar', 1),
(13, 'La Guajira', 1),
(14, 'Meta', 1),
(15, 'Casanare', 1),
(16, 'Arauca', 1),
(17, 'Putumayo', 1),
(18, 'Amazonas', 1),
(19, 'Guainía', 1),
(20, 'Vaupés', 1);

-- Insertar ciudades
INSERT INTO Ciudad (id, nombre, region_id) VALUES 
(1, 'Medellín', 1),
(2, 'Bogotá', 2),
(3, 'Cali', 3),
(4, 'Barranquilla', 4),
(5, 'Cartagena', 5),
(6, 'Bucaramanga', 6),
(7, 'Tunja', 7),
(8, 'Popayán', 8),
(9, 'Pasto', 9),
(10, 'Montería', 10),
(11, 'Santa Marta', 11),
(12, 'Valledupar', 12),
(13, 'Riohacha', 13),
(14, 'Villavicencio', 14),
(15, 'Yopal', 15),
(16, 'Arauca', 16),
(17, 'Mocoa', 17),
(18, 'Leticia', 18),
(19, 'Inírida', 19),
(20, 'Mitú', 20);

-- Insertar tipos de documento
INSERT INTO TipoDocumento (id, descripcion) VALUES 
(1, 'Cédula de Ciudadanía'),
(2, 'Cédula de Extranjería'),
(3, 'Pasaporte'),
(4, 'Tarjeta de Identidad'),
(5, 'NIT'),
(6, 'RUT'),
(7, 'DNI'),
(8, 'Carné de Identidad'),
(9, 'Cédula de Residencia'),
(10, 'Permiso de Trabajo'),
(11, 'Visa'),
(12, 'Permiso Especial'),
(13, 'Documento de Identidad'),
(14, 'Carné de Extranjería'),
(15, 'Documento Nacional'),
(16, 'Cédula de Identidad'),
(17, 'Documento Único'),
(18, 'Carné de Identificación'),
(19, 'Documento de Residencia'),
(20, 'Permiso de Permanencia');

-- Insertar tipos de tercero
INSERT INTO TipoTercero (id, descripcion) VALUES 
(1, 'Cliente'),
(2, 'Empleado'),
(3, 'Proveedor'),
(4, 'Cliente VIP'),
(5, 'Cliente Corporativo'),
(6, 'Proveedor Premium'),
(7, 'Proveedor Local'),
(8, 'Empleado Temporal'),
(9, 'Empleado Contratista'),
(10, 'Cliente Mayorista'),
(11, 'Cliente Minorista'),
(12, 'Proveedor Internacional'),
(13, 'Proveedor Nacional'),
(14, 'Empleado Administrativo'),
(15, 'Empleado Operativo'),
(16, 'Cliente Frecuente'),
(17, 'Cliente Nuevo'),
(18, 'Proveedor Exclusivo'),
(19, 'Empleado Directivo'),
(20, 'Cliente Especial');

-- Insertar EPS
INSERT INTO EPS (id, nombre) VALUES 
(1, 'Sura'),
(2, 'Nueva EPS'),
(3, 'Sanitas'),
(4, 'Coomeva'),
(5, 'Compensar'),
(6, 'Famisanar'),
(7, 'Salud Total'),
(8, 'Medimás'),
(9, 'Aliansalud'),
(10, 'Comfenalco'),
(11, 'Cafesalud'),
(12, 'Colsubsidio'),
(13, 'SOS'),
(14, 'Capital Salud'),
(15, 'Savia Salud'),
(16, 'Ambuq'),
(17, 'Asmet Salud'),
(18, 'Coosalud'),
(19, 'Mutual Ser'),
(20, 'Saludvida');

-- Insertar ARL
INSERT INTO ARL (id, nombre) VALUES 
(1, 'Sura'),
(2, 'Colmena'),
(3, 'Positiva'),
(4, 'La Equidad'),
(5, 'Seguros Bolívar'),
(6, 'Mapfre'),
(7, 'Allianz'),
(8, 'Axa Colpatria'),
(9, 'Liberty'),
(10, 'QBE'),
(11, 'HDI'),
(12, 'Chubb'),
(13, 'Zurich'),
(14, 'Aseguradora Solidaria'),
(15, 'Seguros del Estado'),
(16, 'Previsora'),
(17, 'Alfa'),
(18, 'Mundial'),
(19, 'Seguros Generales'),
(20, 'Seguros Atlas'); 