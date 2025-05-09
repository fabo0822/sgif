
-- TABLAS DE SOPORTE GEOGRÁFICO
create database dbsgi;

use dbsgi;

CREATE TABLE Pais (
    id INT PRIMARY KEY AUTO_INCREMENT,
    nombre VARCHAR(100)
);

CREATE TABLE Region (
    id INT PRIMARY KEY AUTO_INCREMENT,
    nombre VARCHAR(100),
    pais_id INT,
    FOREIGN KEY (pais_id) REFERENCES Pais(id)
);

CREATE TABLE Ciudad (
    id INT PRIMARY KEY AUTO_INCREMENT,
    nombre VARCHAR(100),
    region_id INT,
    FOREIGN KEY (region_id) REFERENCES Region(id)
);

-- TABLAS DE TERCEROS Y TELÉFONOS

CREATE TABLE TipoDocumento (
    id INT PRIMARY KEY AUTO_INCREMENT,
    descripcion VARCHAR(50)
);

CREATE TABLE TipoTercero (
    id INT PRIMARY KEY AUTO_INCREMENT,
    descripcion VARCHAR(50)
);

CREATE TABLE Terceros (
    id VARCHAR(20) PRIMARY KEY,
    nombre VARCHAR(50),
    apellidos VARCHAR(50),
    email VARCHAR(80) UNIQUE,
    tipo_doc_id INT,
    tipo_tercero_id INT,
    ciudad_id INT,
    FOREIGN KEY (tipo_doc_id) REFERENCES TipoDocumento(id),
    FOREIGN KEY (tipo_tercero_id) REFERENCES TipoTercero(id),
    FOREIGN KEY (ciudad_id) REFERENCES Ciudad(id)
);

CREATE TABLE Tercero_Telefonos (
    id INT PRIMARY KEY AUTO_INCREMENT,
    numero VARCHAR(20),
    tipo VARCHAR(20),
    tercero_id VARCHAR(20),
    FOREIGN KEY (tercero_id) REFERENCES Terceros(id)
);

-- TABLAS DE PRODUCTOS Y PROVEEDORES

CREATE TABLE Productos (
    id VARCHAR(20) PRIMARY KEY,
    nombre VARCHAR(50),
    stock INT,
    stockMin INT,
    stockMax INT,
    createdAt DATETIME,
    updatedAt DATETIME,
    barcode VARCHAR(50) UNIQUE
);

CREATE TABLE Proveedor (
    id INT PRIMARY KEY AUTO_INCREMENT,
    tercero_id VARCHAR(20),
    dia_pago INT,
    descuento DOUBLE,
    FOREIGN KEY (tercero_id) REFERENCES Terceros(id)
);

CREATE TABLE Producto_Proveedor (
    id INT PRIMARY KEY AUTO_INCREMENT,
    producto_id VARCHAR(20),
    tercero_id VARCHAR(20),
    FOREIGN KEY (producto_id) REFERENCES Productos(id),
    FOREIGN KEY (tercero_id) REFERENCES Terceros(id)
);

-- TABLAS DE COMPRAS

CREATE TABLE Compras (
    id INT PRIMARY KEY AUTO_INCREMENT,
    tercero_prov_id VARCHAR(20),
    fecha DATE,
    tercero_empl_id VARCHAR(20),
    doc_compra VARCHAR(50),
    FOREIGN KEY (tercero_prov_id) REFERENCES Terceros(id),
    FOREIGN KEY (tercero_empl_id) REFERENCES Terceros(id)
);

CREATE TABLE Detalle_Compra (
    id INT PRIMARY KEY AUTO_INCREMENT,
    fecha DATE,
    producto_id VARCHAR(20),
    cantidad INT,
    valor DECIMAL(10,2),
    compra_id INT,
    entrada_salida VARCHAR(10),
    FOREIGN KEY (producto_id) REFERENCES Productos(id),
    FOREIGN KEY (compra_id) REFERENCES Compras(id)
);

-- TABLAS DE FACTURACIÓN Y VENTAS

CREATE TABLE Facturacion (
    id INT PRIMARY KEY AUTO_INCREMENT,
    fecha_resolucion DATE,
    num_inicio INT,
    num_final INT,
    fact_adicional VARCHAR(50)
);

CREATE TABLE Cliente (
    id INT PRIMARY KEY,
    tercero_id VARCHAR(20),
    fecha_nac DATE,
    fecha_compra DATE,
    FOREIGN KEY (tercero_id) REFERENCES Terceros(id)
);

CREATE TABLE Venta (
    id INT PRIMARY KEY AUTO_INCREMENT,
    fecha DATE,
    tercero_empl_id VARCHAR(20),
    cliente_id INT,
    fact_id INT,
    FOREIGN KEY (tercero_empl_id) REFERENCES Terceros(id),
    FOREIGN KEY (cliente_id) REFERENCES Cliente(id),
    FOREIGN KEY (fact_id) REFERENCES Facturacion(id)
);

CREATE TABLE Detalle_Venta (
    id INT PRIMARY KEY AUTO_INCREMENT,
    producto_id VARCHAR(20),
    cantidad INT,
    valor DECIMAL(10,2),
    venta_id INT,
    FOREIGN KEY (producto_id) REFERENCES Productos(id),
    FOREIGN KEY (venta_id) REFERENCES Venta(id)
);

-- TABLAS DE PLANES Y CAJA

CREATE TABLE Planes (
    id INT PRIMARY KEY AUTO_INCREMENT,
    nombre VARCHAR(30),
    inicio DATE,
    fin DATE,
    descuento DOUBLE
);

CREATE TABLE PlanProductos (
    id INT PRIMARY KEY AUTO_INCREMENT,
    plan_id INT,
    producto_id VARCHAR(20),
    FOREIGN KEY (plan_id) REFERENCES Planes(id),
    FOREIGN KEY (producto_id) REFERENCES Productos(id)
);

CREATE TABLE TipoMovCaja (
    id INT PRIMARY KEY AUTO_INCREMENT,
    nombre VARCHAR(50),
    tipo VARCHAR(10)
);

CREATE TABLE MovCaja (
    id INT PRIMARY KEY AUTO_INCREMENT,
    fecha DATE,
    tipo_mov_id INT,
    valor DECIMAL(10,2),
    concepto TEXT,
    tercero_id VARCHAR(20),
    FOREIGN KEY (tipo_mov_id) REFERENCES TipoMovCaja(id),
    FOREIGN KEY (tercero_id) REFERENCES Terceros(id)
);

-- TABLAS DE EMPLEADOS Y SALUD

CREATE TABLE EPS (
    id INT PRIMARY KEY AUTO_INCREMENT,
    nombre VARCHAR(50)
);

CREATE TABLE ARL (
    id INT PRIMARY KEY AUTO_INCREMENT,
    nombre VARCHAR(50)
);

CREATE TABLE Empleado (
    id INT PRIMARY KEY AUTO_INCREMENT,
    tercero_id VARCHAR(20),
    fecha_ingreso DATE,
    salario_base DECIMAL(10,2),
    eps_id INT,
    arl_id INT,
    FOREIGN KEY (tercero_id) REFERENCES Terceros(id),
    FOREIGN KEY (eps_id) REFERENCES EPS(id),
    FOREIGN KEY (arl_id) REFERENCES ARL(id)
);

-- TABLA DE EMPRESA

CREATE TABLE Empresa (
    id VARCHAR(20) PRIMARY KEY,
    nombre VARCHAR(50),
    ciudad_id INT,
    fecha_reg DATE,
    FOREIGN KEY (ciudad_id) REFERENCES Ciudad(id)
);
