DELIMITER //

CREATE PROCEDURE insertar_tercero_y_cliente (
    IN p_tercero_id VARCHAR(20),
    IN p_nombre VARCHAR(50),
    IN p_apellidos VARCHAR(50),
    IN p_email VARCHAR(80),
    IN p_tipo_doc_id INT,
    IN p_tipo_tercero_id INT,
    IN p_ciudad_id INT,
    IN p_fecha_nac DATE,
    IN p_fecha_ultima_compra DATE
)
BEGIN
    INSERT INTO Terceros (
        id, nombre, apellidos, email, tipo_doc_id, tipo_tercero_id, ciudad_id
    ) VALUES (
        p_tercero_id, p_nombre, p_apellidos, p_email, p_tipo_doc_id, p_tipo_tercero_id, p_ciudad_id
    );

    INSERT INTO Cliente (
        tercero_id, fecha_nac, fecha_compra
    ) VALUES (
        p_tercero_id, p_fecha_nac, p_fecha_ultima_compra
    );
END //

DELIMITER ;

DELIMITER //

CREATE PROCEDURE insertar_tercero_y_empleado(
    IN p_id VARCHAR(20),
    IN p_nombre VARCHAR(50),
    IN p_apellidos VARCHAR(50),
    IN p_email VARCHAR(80),
    IN p_tipo_doc_id INT,
    IN p_tipo_tercero_id INT,
    IN p_ciudad_id INT,
    IN p_fecha_ingreso DATE,
    IN p_salario_base DOUBLE,
    IN p_eps_id INT,
    IN p_arl_id INT
)
BEGIN
    INSERT INTO Terceros (id, nombre, apellidos, email, tipo_doc_id, tipo_tercero_id, ciudad_id)
    VALUES (p_id, p_nombre, p_apellidos, p_email, p_tipo_doc_id, p_tipo_tercero_id, p_ciudad_id);

    INSERT INTO Empleado (tercero_id, fecha_ingreso, salario_base, eps_id, arl_id)
    VALUES (p_id, p_fecha_ingreso, p_salario_base, p_eps_id, p_arl_id);
END //

DELIMITER ;

DELIMITER $$

CREATE PROCEDURE actualizar_tercero_cliente (
    IN p_id VARCHAR(20),
    IN p_nombre VARCHAR(50),
    IN p_apellidos VARCHAR(50),
    IN p_email VARCHAR(80),
    IN p_tipo_doc_id INT,
    IN p_tipo_tercero_id INT,
    IN p_ciudad_id INT,
    IN p_fecha_nac DATE,
    IN p_fecha_ultima_compra DATE
)
BEGIN
    UPDATE Terceros
    SET nombre = p_nombre,
        apellidos = p_apellidos,
        email = p_email,
        tipo_doc_id = p_tipo_doc_id,
        tipo_tercero_id = p_tipo_tercero_id,
        ciudad_id = p_ciudad_id
    WHERE id = p_id;

    UPDATE Cliente
    SET fecha_nac = p_fecha_nac,
        fecha_compra = p_fecha_ultima_compra
    WHERE tercero_id = p_id;
END$$

DELIMITER ;

DELIMITER $$

CREATE PROCEDURE actualizar_tercero_y_empleado(
    IN p_id VARCHAR(20),
    IN p_nombre VARCHAR(50),
    IN p_apellidos VARCHAR(50),
    IN p_email VARCHAR(80),
    IN p_tipo_doc_id INT,
    IN p_tipo_tercero_id INT,
    IN p_ciudad_id INT,
    IN p_fecha_ingreso DATE,
    IN p_salario_base DOUBLE,
    IN p_eps_id INT,
    IN p_arl_id INT
)
BEGIN
    UPDATE Terceros
    SET nombre = p_nombre,
        apellidos = p_apellidos,
        email = p_email,
        tipo_doc_id = p_tipo_doc_id,
        tipo_tercero_id = p_tipo_tercero_id,
        ciudad_id = p_ciudad_id
    WHERE id = p_id;

    UPDATE Empleado
    SET fecha_ingreso = p_fecha_ingreso,
        salario_base = p_salario_base,
        eps_id = p_eps_id,
        arl_id = p_arl_id
    WHERE tercero_id = p_id;
END$$

DELIMITER ;

DELIMITER $$

CREATE PROCEDURE insertar_compra_y_detalle (
    IN p_proveedor_id VARCHAR(20),
    IN p_empleado_id VARCHAR(20),
    IN p_fecha_compra DATE,
    IN p_descripcion VARCHAR(255),
    IN p_fecha_detalle DATE,
    IN p_producto_id VARCHAR(20),
    IN p_cantidad INT,
    IN p_valor DECIMAL(10,2)
)
BEGIN
    DECLARE last_compra_id INT;

    INSERT INTO Compras (tercero_prov_id, tercero_empl_id, fecha, doc_compra)
    VALUES (p_proveedor_id, p_empleado_id, p_fecha_compra, p_descripcion);

    SET last_compra_id = LAST_INSERT_ID();

    INSERT INTO Detalle_Compra (compra_id, fecha, producto_id, cantidad, valor)
    VALUES (last_compra_id, p_fecha_detalle, p_producto_id, p_cantidad, p_valor);
END$$

DELIMITER ;

DELIMITER //

CREATE PROCEDURE sp_ActualizarCompra(
    IN p_id INT,
    IN p_tercero_prov_id VARCHAR(20),
    IN p_fecha DATE,
    IN p_tercero_empl_id VARCHAR(20),
    IN p_desc_compra TEXT,
    IN p_detalle_fecha DATE,
    IN p_producto_id VARCHAR(20),
    IN p_cantidad INT,
    IN p_valor DOUBLE
)
BEGIN
    UPDATE Compras
    SET 
        tercero_prov_id = p_tercero_prov_id,
        fecha = p_fecha,
        tercero_empl_id = p_tercero_empl_id,
        doc_compra = p_desc_compra
    WHERE id = p_id;

    UPDATE Detalle_Compra
    SET
        fecha = p_detalle_fecha,
        producto_id = p_producto_id,
        cantidad = p_cantidad,
        valor = p_valor
    WHERE compra_id = p_id;
END //

DELIMITER ;

DELIMITER $$

CREATE PROCEDURE RegistrarVenta (
    IN p_fecha DATE,
    IN p_cliente VARCHAR(20),
    IN p_empleado VARCHAR(20),
    IN p_forma_pago VARCHAR(50),
    IN p_detalles JSON
)
BEGIN
    DECLARE v_venta_id INT;

    INSERT INTO Venta (fecha, tercero_cli_id, tercero_emp_id, forma_pago)
    VALUES (p_fecha, p_cliente, p_empleado, p_forma_pago);

    SET v_venta_id = LAST_INSERT_ID();

    INSERT INTO Detalle_Venta (factura_id, producto_id, cantidad, valor)
    SELECT 
        v_venta_id,
        detalle.producto_id,
        detalle.cantidad,
        detalle.valor
    FROM JSON_TABLE(
        p_detalles,
        '$[*]' COLUMNS (
            producto_id VARCHAR(20) PATH '$.productoId',
            cantidad INT PATH '$.cantidad',
            valor DOUBLE PATH '$.valor'
        )
    ) AS detalle;
END$$

DELIMITER ;

DELIMITER //

CREATE PROCEDURE insertar_tercero_y_proveedor(
    IN p_id VARCHAR(20),
    IN p_nombre VARCHAR(50),
    IN p_apellidos VARCHAR(50),
    IN p_email VARCHAR(80),
    IN p_tipo_doc_id INT,
    IN p_tipo_tercero_id INT,
    IN p_ciudad_id INT,
    IN p_fecha_ingreso DATE,
    IN p_descuento DOUBLE
)
BEGIN
    INSERT INTO Terceros (
        id, nombre, apellidos, email, tipo_doc_id, tipo_tercero_id, ciudad_id
    ) VALUES (
        p_id, p_nombre, p_apellidos, p_email, p_tipo_doc_id, p_tipo_tercero_id, p_ciudad_id
    );

    INSERT INTO Proveedor (
        tercero_id, fecha_ingreso, descuento
    ) VALUES (
        p_id, p_fecha_ingreso, p_descuento
    );
END //

DELIMITER ; 