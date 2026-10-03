CREATE DATABASE ReservaHotel;
GO

USE ReservaHotel;
GO


CREATE TABLE Hotel (
    id_hotel INT PRIMARY KEY,
    direccion VARCHAR(255) NOT NULL,
    telefono VARCHAR(20)
);

CREATE TABLE Categoria (
    id_categoria INT PRIMARY KEY,
    nombre_categoria VARCHAR(50) NOT NULL,--estandar, junior, premier
    precio_actual DECIMAL(10,2) NOT NULL
);

CREATE TABLE Cliente (
    cedula VARCHAR(20) PRIMARY KEY,
    nombre VARCHAR(100) NOT NULL
);

CREATE TABLE Telefono_Cliente (
    id_telefono INT IDENTITY(1,1) PRIMARY KEY,
    cedula_cliente VARCHAR(20),
    telefono VARCHAR(20) NOT NULL,

    CONSTRAINT FK_TelefonoCliente
        FOREIGN KEY (cedula_cliente)
        REFERENCES Cliente(cedula)
);

CREATE TABLE Mobiliario (
    cod_mueble INT PRIMARY KEY,
    descripcion VARCHAR(100) NOT NULL,
    precio_mueble DECIMAL(10,2) NOT NULL
);

CREATE TABLE Rol (
    id_rol INT PRIMARY KEY,
    nombre_rol VARCHAR(50) NOT NULL
);
CREATE TABLE Usuario (
    id_usuario INT IDENTITY(1,1) PRIMARY KEY,
    username VARCHAR(50) NOT NULL UNIQUE,
    password VARCHAR(100) NOT NULL,
    id_rol INT NOT NULL,
    cedula_cliente VARCHAR(20) NOT NULL,

    CONSTRAINT FK_UsuarioRol
    FOREIGN KEY (id_rol)
    REFERENCES Rol(id_rol),

    CONSTRAINT FK_UsuarioCliente
    FOREIGN KEY (cedula_cliente)
    REFERENCES Cliente(cedula)
);


CREATE TABLE Habitacion (
    num_habitacion INT,
    id_hotel INT,
    id_categoria INT,

    PRIMARY KEY (num_habitacion, id_hotel),
    CONSTRAINT FK_Hotel FOREIGN KEY (id_hotel) REFERENCES Hotel(id_hotel),
    CONSTRAINT FK_Categoria FOREIGN KEY (id_categoria) REFERENCES Categoria(id_categoria)
);

CREATE TABLE Inventario_Habitacion (
    num_habitacion INT,
    id_hotel INT,
    cod_mueble INT,
    cantidad INT DEFAULT 1,

    PRIMARY KEY (num_habitacion, id_hotel, cod_mueble),

    CONSTRAINT FK_Inventario_Habitacion
        FOREIGN KEY (num_habitacion, id_hotel)
        REFERENCES Habitacion(num_habitacion, id_hotel),

    CONSTRAINT FK_Mobiliario
        FOREIGN KEY (cod_mueble)
        REFERENCES Mobiliario(cod_mueble)
);

CREATE TABLE Caracteristica (
    id_caracteristica INT PRIMARY KEY,
    nombre_caracteristica VARCHAR(50) NOT NULL
);

CREATE TABLE Detalle_Habitacion (
    num_habitacion INT,
    id_hotel INT,
    id_caracteristica INT,

    PRIMARY KEY (num_habitacion, id_hotel, id_caracteristica),

    CONSTRAINT FK_Detalle_Habitacion
        FOREIGN KEY (num_habitacion, id_hotel)
        REFERENCES Habitacion(num_habitacion, id_hotel),

    CONSTRAINT FK_Caracteristica
        FOREIGN KEY (id_caracteristica)
        REFERENCES Caracteristica(id_caracteristica)
);


CREATE TABLE Reservacion (
    id_reservacion INT IDENTITY(1,1) PRIMARY KEY,
    cedula_cliente VARCHAR(20) NOT NULL,
    fecha_creacion DATETIME DEFAULT GETDATE(), -- Cuándo se hizo el trámite
    metodo_pago VARCHAR(20),
    monto_total DECIMAL(10,2) NOT NULL,        -- Sumatoria de todo
    estado VARCHAR(20) DEFAULT 'Pendiente',    -- Ej: Pendiente, Pagada, Cancelada

    CONSTRAINT FK_Cliente_Reserva
        FOREIGN KEY (cedula_cliente)
        REFERENCES Cliente(cedula)
);


CREATE TABLE Detalle_Reservacion (
    id_detalle INT IDENTITY(1,1) PRIMARY KEY,
    id_reservacion INT NOT NULL,
    num_habitacion INT NOT NULL,
    id_hotel INT NOT NULL,
    fecha_llegada DATE NOT NULL,
    fecha_salida DATE NOT NULL,
    precio_noche_aplicado DECIMAL(10,2) NOT NULL, -- "Congela" el precio histórico
    check_in BIT DEFAULT 0,
    check_out BIT DEFAULT 0

    CONSTRAINT FK_Reserva_Detalle
        FOREIGN KEY (id_reservacion)
        REFERENCES Reservacion(id_reservacion),

    CONSTRAINT FK_Habitacion_Reserva
        FOREIGN KEY (num_habitacion, id_hotel)
        REFERENCES Habitacion(num_habitacion, id_hotel),

    CONSTRAINT CHK_Fechas_Detalle
        CHECK (fecha_salida > fecha_llegada)
);


-- La aplicación usa autenticación integrada de Windows (Integrated Security).
-- Por seguridad, este script no crea logins personales ni publica contraseñas
-- de SQL Server. Concede acceso a la base de datos desde SQL Server Management
-- Studio al usuario de Windows que ejecutará IIS Express.
GO

-- Datos iniciales

INSERT INTO Rol (id_rol, nombre_rol) VALUES (1, 'Administrador');
INSERT INTO Rol (id_rol, nombre_rol) VALUES (2, 'Cliente');

INSERT INTO Cliente (cedula, nombre) VALUES ('111111111', 'Claudio Mendez');

INSERT INTO Usuario (username, password, id_rol, cedula_cliente) VALUES ('admin', '*Admin01', 1, '111111111');

INSERT INTO Hotel (id_hotel, direccion, telefono) 
VALUES (1, '700 metros oeste de la Iglesia, La Fortuna, Alajuela', '2479-1000');
 
INSERT INTO Hotel (id_hotel, direccion, telefono) 
VALUES (2, 'Puntarenas, Manuel Antonio, Quepos, Carretera Principal', '2777-0000');
 
INSERT INTO Hotel (id_hotel, direccion, telefono) 
VALUES (3, 'Guanacaste, Playa Hermosa, 100 metros norte del campo de futbol', '2672-1234');
 
INSERT INTO Hotel (id_hotel, direccion, telefono) 
VALUES (4, 'Limon, Puerto Viejo, 200 metros sur de Playa Cocles', '2750-0987');
 
INSERT INTO Hotel (id_hotel, direccion, telefono) 
VALUES (5, 'San Jose, Paseo Colon, Calle 38, Avenida 0', '2255-4000');

INSERT INTO Categoria (id_categoria, nombre_categoria, precio_actual) 
VALUES (1, 'Estándar', 45000.00);
 
INSERT INTO Categoria (id_categoria, nombre_categoria, precio_actual) 
VALUES (2, 'Junior', 75000.00);
 
INSERT INTO Categoria (id_categoria, nombre_categoria, precio_actual) 
VALUES (3, 'Premier', 120000.00)

-- Habitaciones para el Hotel 1 (La Fortuna)
INSERT INTO Habitacion (num_habitacion, id_hotel, id_categoria) VALUES
(101, 1, 1), (102, 1, 1), (103, 1, 2), (104, 1, 2), (105, 1, 3);
 
-- Habitaciones para el Hotel 2 (Manuel Antonio)
INSERT INTO Habitacion (num_habitacion, id_hotel, id_categoria) VALUES
(201, 2, 1), (202, 2, 1), (203, 2, 2), (204, 2, 3), (205, 2, 3);
 
-- Habitaciones para el Hotel 3 (Playa Hermosa)
INSERT INTO Habitacion (num_habitacion, id_hotel, id_categoria) VALUES
(301, 3, 1), (302, 3, 2), (303, 3, 2), (304, 3, 3), (305, 3, 1);
 
-- Habitaciones para el Hotel 4 (Puerto Viejo)
INSERT INTO Habitacion (num_habitacion, id_hotel, id_categoria) VALUES
(401, 4, 1), (402, 4, 2), (403, 4, 3), (404, 4, 1), (405, 4, 2);
 
-- Habitaciones para el Hotel 5 (San José)
INSERT INTO Habitacion (num_habitacion, id_hotel, id_categoria) VALUES
(501, 5, 1), (502, 5, 1), (503, 5, 2), (504, 5, 3), (505, 5, 3);

INSERT INTO Caracteristica (id_caracteristica, nombre_caracteristica)VALUES (1, 'Es Soleada');
INSERT INTO Caracteristica (id_caracteristica, nombre_caracteristica)VALUES (2, 'Tiene Lavado');
INSERT INTO Caracteristica (id_caracteristica, nombre_caracteristica)VALUES (3, 'Tiene Nevera');

-- Habitaciones Hotel 1 (La Fortuna)
INSERT INTO Detalle_Habitacion (num_habitacion, id_hotel, id_caracteristica) VALUES
(101, 1, 1), (101, 1, 3), -- Soleada y Nevera
(102, 1, 3),              -- Solo Nevera
(103, 1, 1), (103, 1, 2), -- Soleada y Lavado
(104, 1, 2), (104, 1, 3), -- Lavado y Nevera
(105, 1, 1), (105, 1, 2), (105, 1, 3); -- Todas (Premier)
 
-- Habitaciones Hotel 2 (Manuel Antonio)
INSERT INTO Detalle_Habitacion (num_habitacion, id_hotel, id_caracteristica) VALUES
(201, 2, 1),              -- Solo Soleada
(202, 2, 1), (202, 2, 3), -- Soleada y Nevera
(203, 2, 2), (203, 2, 3), -- Lavado y Nevera
(204, 2, 1), (204, 2, 2), (204, 2, 3), -- Todas
(205, 2, 1), (205, 2, 2), (205, 2, 3); -- Todas
 
-- Habitaciones Hotel 3 (Playa Hermosa)
INSERT INTO Detalle_Habitacion (num_habitacion, id_hotel, id_caracteristica) VALUES
(301, 3, 3),              -- Solo Nevera
(302, 3, 1), (302, 3, 3), -- Soleada y Nevera
(303, 3, 1), (303, 3, 3),
(304, 3, 1), (304, 3, 2), (304, 3, 3),
(305, 3, 1);              -- Solo Soleada
 
-- Habitaciones Hotel 4 (Puerto Viejo)
INSERT INTO Detalle_Habitacion (num_habitacion, id_hotel, id_caracteristica) VALUES
(401, 4, 1),
(402, 4, 1), (402, 4, 2),
(403, 4, 1), (403, 4, 2), (403, 4, 3),
(404, 4, 1), (404, 4, 3),
(405, 4, 1), (405, 4, 2);
 
-- Habitaciones Hotel 5 (San José)
INSERT INTO Detalle_Habitacion (num_habitacion, id_hotel, id_caracteristica) VALUES
(501, 5, 3),
(502, 5, 3),
(503, 5, 1), (503, 5, 2), (503, 5, 3),
(504, 5, 1), (504, 5, 2), (504, 5, 3),
(505, 5, 2), (505, 5, 3);

INSERT INTO Mobiliario (cod_mueble, descripcion, precio_mueble) VALUES
(1, 'Cama matrimonial', 150000.00),
(2, 'Cama individual', 90000.00),
(3, 'Cama King Size', 280000.00),
(4, 'Nevera pequeña', 120000.00),
(5, 'Armario', 85000.00),
(6, 'Televisor', 200000.00),
(7, 'Aire acondicionado', 250000.00),
(8, 'Mesa de noche', 25000.00),
(9, 'Escritorio', 45000.00),
(10, 'Silla', 15000.00),
(11, 'Sillón', 70000.00),
(12, 'Lámpara', 10000.00),
(13, 'Caja fuerte', 40000.00),
(14, 'Espejo', 12000.00),
(15, 'Cofre de maletas', 20000.00),
(16, 'Microondas', 45000.00),
(17, 'Cafetera', 18000.00),
(18, 'Teléfono fijo', 12000.00),
(19, 'Mueble de baño', 35000.00),
(20, 'Cuadro', 8000.00);
