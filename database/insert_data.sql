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

-- Insertar terceros
INSERT INTO Terceros (id, nombre, apellidos, email) VALUES 
('T001', 'Juan', 'Pérez', 'juan.perez@example.com'),
('T002', 'María', 'Gómez', 'maria.gomez@example.com'),
('T003', 'Carlos', 'López', 'carlos.lopez@example.com');

-- Insertar productos
INSERT INTO Producto (id, nombre, stock, stockMin, stockMax, createdAt, updatedAt, barcode) VALUES 
('P001', 'Producto A', 100, 10, 200, NOW(), NOW(), '1234567890123'),
('P002', 'Producto B', 50, 5, 100, NOW(), NOW(), '1234567890456'),
('P003', 'Producto C', 75, 15, 150, NOW(), NOW(), '1234567890789');

-- Insertar proveedores
INSERT INTO Proveedor (id, tercero_id, fecha_ingreso, descuento) VALUES 
(1, 'T001', '2025-01-01', 10.5),
(2, 'T002', '2025-02-01', 15.0);

-- Insertar compras
INSERT INTO Compras (id, tercero_prov_id, fecha, tercero_empl_id, doc_compra) VALUES 
(1, 'T001', '2025-03-01', 'T003', 'DOC001'),
(2, 'T002', '2025-03-15', 'T003', 'DOC002');

-- Insertar detalles de compra
INSERT INTO Detalle_Compra (id, fecha, producto_id, cantidad, valor, compra_id, entrada_salida) VALUES 
(1, '2025-03-01', 'P001', 10, 100.00, 1, 'entrada'),
(2, '2025-03-15', 'P002', 5, 50.00, 2, 'entrada');

-- Insertar facturación
INSERT INTO Facturacion (id, fecha_resolucion, num_inicio, num_final, fact_adicional) VALUES 
(1, '2025-01-01', 1000, 2000, 'Adicional 1'),
(2, '2025-02-01', 2001, 3000, 'Adicional 2');

-- Insertar clientes
INSERT INTO Cliente (id, tercero_id, fecha_nac, fecha_compra) VALUES 
(1, 'T001', '1990-01-01', '2025-03-01'),
(2, 'T002', '1985-05-15', '2025-03-15');

-- Insertar ventas
INSERT INTO Venta (id, fecha, tercero_empl_id, cliente_id, fact_id) VALUES 
(1, '2025-03-10', 'T003', 1, 1),
(2, '2025-03-20', 'T003', 2, 2);

-- Insertar detalles de venta
INSERT INTO Detalle_Venta (id, producto_id, cantidad, valor, venta_id) VALUES 
(1, 'P001', 2, 20.00, 1),
(2, 'P002', 1, 10.00, 2);

-- Insertar planes
INSERT INTO Planes (id, nombre, inicio, fin, descuento) VALUES 
(1, 'Plan A', '2025-01-01', '2025-12-31', 10.0),
(2, 'Plan B', '2025-06-01', '2025-12-31', 15.0);

-- Insertar productos en planes
INSERT INTO PlanProductos (id, plan_id, producto_id) VALUES 
(1, 1, 'P001'),
(2, 2, 'P002');

-- Insertar tipos de movimiento de caja
INSERT INTO TipoMovCaja (id, nombre, tipo) VALUES 
(1, 'Ingreso', 'entrada'),
(2, 'Egreso', 'salida');

-- Insertar movimientos de caja
INSERT INTO MovCaja (id, fecha, tipo_mov_id, valor, concepto, tercero_id) VALUES 
(1, '2025-03-01', 1, 1000.00, 'Venta de productos', 'T001'),
(2, '2025-03-15', 2, 500.00, 'Compra de insumos', 'T002');

-- Insertar empleados
INSERT INTO Empleado (id, tercero_id, fecha_ingreso, salario_base) VALUES 
(1, 'T003', '2025-01-01', 2000.00);