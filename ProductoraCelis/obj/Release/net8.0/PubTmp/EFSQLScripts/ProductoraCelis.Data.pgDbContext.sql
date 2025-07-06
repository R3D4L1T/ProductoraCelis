IF OBJECT_ID(N'[__EFMigrationsHistory]') IS NULL
BEGIN
    CREATE TABLE [__EFMigrationsHistory] (
        [MigrationId] nvarchar(150) NOT NULL,
        [ProductVersion] nvarchar(32) NOT NULL,
        CONSTRAINT [PK___EFMigrationsHistory] PRIMARY KEY ([MigrationId])
    );
END;
GO

BEGIN TRANSACTION;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20250529042449_mick1'
)
BEGIN
    CREATE TABLE [Carrito] (
        [IdCarrito] int NOT NULL IDENTITY,
        [UsuarioId] nvarchar(450) NOT NULL,
        [IdProducto] int NOT NULL,
        [Nombre] nvarchar(100) NOT NULL,
        [Categoria] nvarchar(100) NOT NULL,
        [Precio] decimal(18,2) NOT NULL,
        [Cantidad] int NOT NULL,
        [UrlImagen] nvarchar(max) NULL,
        [FechaHora] datetime2 NOT NULL,
        CONSTRAINT [PK_Carrito] PRIMARY KEY ([IdCarrito])
    );
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20250529042449_mick1'
)
BEGIN
    CREATE TABLE [Clientes_Mayoristas] (
        [IdClientes] int NOT NULL IDENTITY,
        [Nombres] nvarchar(50) NOT NULL,
        [Apellidos] nvarchar(50) NOT NULL,
        [Dni] nvarchar(8) NOT NULL,
        [Celular] nvarchar(9) NOT NULL,
        [Direccion] nvarchar(50) NOT NULL,
        [FechaRegistro] datetime NOT NULL,
        [Descripcion] nvarchar(100) NOT NULL,
        [Estado] bit NOT NULL,
        [Email] nvarchar(50) NOT NULL,
        CONSTRAINT [PK_Clientes_Mayoristas] PRIMARY KEY ([IdClientes])
    );
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20250529042449_mick1'
)
BEGIN
    CREATE TABLE [Producto] (
        [IdProducto] int NOT NULL IDENTITY,
        [Nombre] nvarchar(50) NOT NULL,
        [Categoria] nvarchar(50) NOT NULL,
        [Descripcion] nvarchar(100) NOT NULL,
        [Stock] int NOT NULL,
        [FechaProduccion] datetime NOT NULL,
        [FechaVencimiento] datetime NOT NULL,
        [Precio] decimal(18,2) NOT NULL,
        [Estado] bit NOT NULL,
        [UrlImagen] nvarchar(200) NULL,
        CONSTRAINT [PK_Producto] PRIMARY KEY ([IdProducto])
    );
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20250529042449_mick1'
)
BEGIN
    CREATE TABLE [Proveedor] (
        [IdProveedor] int NOT NULL IDENTITY,
        [Nombres] nvarchar(50) NOT NULL,
        [Apellidos] nvarchar(50) NOT NULL,
        [Dni] nvarchar(9) NOT NULL,
        [Celular] nvarchar(9) NOT NULL,
        [Direccion] nvarchar(50) NOT NULL,
        [FechaRegistro] datetime NOT NULL,
        [Descripcion] nvarchar(100) NOT NULL,
        [Tipo] nvarchar(100) NOT NULL,
        [Estado] bit NOT NULL,
        [Email] nvarchar(100) NOT NULL,
        CONSTRAINT [PK_Proveedor] PRIMARY KEY ([IdProveedor])
    );
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20250529042449_mick1'
)
BEGIN
    CREATE TABLE [Usuarios] (
        [IdUsuario] int NOT NULL IDENTITY,
        [Nombres] nvarchar(50) NOT NULL,
        [Apellidos] nvarchar(50) NOT NULL,
        [Dni] nvarchar(8) NOT NULL,
        [Celular] nvarchar(9) NOT NULL,
        [Email] nvarchar(50) NOT NULL,
        [Password] nvarchar(50) NOT NULL,
        [Rol] nvarchar(20) NOT NULL,
        CONSTRAINT [PK_Usuarios] PRIMARY KEY ([IdUsuario])
    );
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20250529042449_mick1'
)
BEGIN
    CREATE TABLE [Compra] (
        [IdCompra] int NOT NULL IDENTITY,
        [FechaCompra] datetime2 NOT NULL,
        [Total] decimal(10,2) NOT NULL,
        [IdUsuarioCliente] int NOT NULL,
        [UsuarioIdUsuario] int NULL,
        CONSTRAINT [PK_Compra] PRIMARY KEY ([IdCompra]),
        CONSTRAINT [FK_Compra_Usuarios_IdUsuarioCliente] FOREIGN KEY ([IdUsuarioCliente]) REFERENCES [Usuarios] ([IdUsuario]) ON DELETE NO ACTION,
        CONSTRAINT [FK_Compra_Usuarios_UsuarioIdUsuario] FOREIGN KEY ([UsuarioIdUsuario]) REFERENCES [Usuarios] ([IdUsuario])
    );
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20250529042449_mick1'
)
BEGIN
    CREATE TABLE [Comprobante] (
        [IdComprobante] int NOT NULL IDENTITY,
        [IdCompra] int NOT NULL,
        [ArchivoPdf] varbinary(max) NOT NULL,
        [FechaGeneracion] datetime NOT NULL,
        [RutaArchivo] nvarchar(max) NOT NULL,
        CONSTRAINT [PK_Comprobante] PRIMARY KEY ([IdComprobante]),
        CONSTRAINT [FK_Comprobante_Compra_IdCompra] FOREIGN KEY ([IdCompra]) REFERENCES [Compra] ([IdCompra]) ON DELETE CASCADE
    );
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20250529042449_mick1'
)
BEGIN
    CREATE TABLE [DetalleCompra] (
        [IdDetalleCompra] int NOT NULL IDENTITY,
        [IdCompra] int NOT NULL,
        [IdProducto] int NOT NULL,
        [Cantidad] int NOT NULL,
        [PrecioUnitario] decimal(18,2) NOT NULL,
        [SubTotal] decimal(18,2) NOT NULL,
        CONSTRAINT [PK_DetalleCompra] PRIMARY KEY ([IdDetalleCompra]),
        CONSTRAINT [FK_DetalleCompra_Compra_IdCompra] FOREIGN KEY ([IdCompra]) REFERENCES [Compra] ([IdCompra]) ON DELETE CASCADE,
        CONSTRAINT [FK_DetalleCompra_Producto_IdProducto] FOREIGN KEY ([IdProducto]) REFERENCES [Producto] ([IdProducto]) ON DELETE NO ACTION
    );
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20250529042449_mick1'
)
BEGIN
    CREATE TABLE [Pagos] (
        [IdPago] int NOT NULL IDENTITY,
        [IdCompra] int NOT NULL,
        [Monto] decimal(18,2) NOT NULL,
        [FechaPago] datetime2 NOT NULL,
        [MetodoPago] nvarchar(50) NOT NULL,
        [NumeroTarjeta] nvarchar(20) NOT NULL,
        [NombreTitular] nvarchar(100) NOT NULL,
        [FechaExpiracion] datetime2 NULL,
        [Cvv] nvarchar(10) NOT NULL,
        CONSTRAINT [PK_Pagos] PRIMARY KEY ([IdPago]),
        CONSTRAINT [FK_Pagos_Compra_IdCompra] FOREIGN KEY ([IdCompra]) REFERENCES [Compra] ([IdCompra]) ON DELETE CASCADE
    );
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20250529042449_mick1'
)
BEGIN
    CREATE TABLE [Reporte] (
        [IdReporte] int NOT NULL IDENTITY,
        [IdComprobante] int NOT NULL,
        [IdCompra] int NOT NULL,
        [IdProducto] int NOT NULL,
        [TotalCantidadVendida] int NOT NULL,
        [Categoria] nvarchar(max) NOT NULL,
        [Cantidad] int NOT NULL,
        [PrecioUnitario] decimal(18,2) NOT NULL,
        [Subtotal] decimal(18,2) NOT NULL,
        [FechaHora] datetime2 NOT NULL,
        [CompraIdCompra] int NULL,
        [ProductoIdProducto] int NULL,
        CONSTRAINT [PK_Reporte] PRIMARY KEY ([IdReporte]),
        CONSTRAINT [FK_Reporte_Compra_CompraIdCompra] FOREIGN KEY ([CompraIdCompra]) REFERENCES [Compra] ([IdCompra]),
        CONSTRAINT [FK_Reporte_Compra_IdCompra] FOREIGN KEY ([IdCompra]) REFERENCES [Compra] ([IdCompra]) ON DELETE NO ACTION,
        CONSTRAINT [FK_Reporte_Comprobante_IdComprobante] FOREIGN KEY ([IdComprobante]) REFERENCES [Comprobante] ([IdComprobante]) ON DELETE NO ACTION,
        CONSTRAINT [FK_Reporte_Producto_IdProducto] FOREIGN KEY ([IdProducto]) REFERENCES [Producto] ([IdProducto]) ON DELETE NO ACTION,
        CONSTRAINT [FK_Reporte_Producto_ProductoIdProducto] FOREIGN KEY ([ProductoIdProducto]) REFERENCES [Producto] ([IdProducto])
    );
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20250529042449_mick1'
)
BEGIN
    CREATE INDEX [IX_Compra_IdUsuarioCliente] ON [Compra] ([IdUsuarioCliente]);
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20250529042449_mick1'
)
BEGIN
    CREATE INDEX [IX_Compra_UsuarioIdUsuario] ON [Compra] ([UsuarioIdUsuario]);
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20250529042449_mick1'
)
BEGIN
    CREATE INDEX [IX_Comprobante_IdCompra] ON [Comprobante] ([IdCompra]);
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20250529042449_mick1'
)
BEGIN
    CREATE INDEX [IX_DetalleCompra_IdCompra] ON [DetalleCompra] ([IdCompra]);
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20250529042449_mick1'
)
BEGIN
    CREATE INDEX [IX_DetalleCompra_IdProducto] ON [DetalleCompra] ([IdProducto]);
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20250529042449_mick1'
)
BEGIN
    CREATE UNIQUE INDEX [IX_Pagos_IdCompra] ON [Pagos] ([IdCompra]);
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20250529042449_mick1'
)
BEGIN
    CREATE INDEX [IX_Reporte_CompraIdCompra] ON [Reporte] ([CompraIdCompra]);
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20250529042449_mick1'
)
BEGIN
    CREATE INDEX [IX_Reporte_IdCompra] ON [Reporte] ([IdCompra]);
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20250529042449_mick1'
)
BEGIN
    CREATE INDEX [IX_Reporte_IdComprobante] ON [Reporte] ([IdComprobante]);
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20250529042449_mick1'
)
BEGIN
    CREATE INDEX [IX_Reporte_IdProducto] ON [Reporte] ([IdProducto]);
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20250529042449_mick1'
)
BEGIN
    CREATE INDEX [IX_Reporte_ProductoIdProducto] ON [Reporte] ([ProductoIdProducto]);
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20250529042449_mick1'
)
BEGIN
    INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
    VALUES (N'20250529042449_mick1', N'8.0.10');
END;
GO

COMMIT;
GO

BEGIN TRANSACTION;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20250529042614_mick2'
)
BEGIN
    INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
    VALUES (N'20250529042614_mick2', N'8.0.10');
END;
GO

COMMIT;
GO

BEGIN TRANSACTION;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20250614044634_m1'
)
BEGIN
    INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
    VALUES (N'20250614044634_m1', N'8.0.10');
END;
GO

COMMIT;
GO

BEGIN TRANSACTION;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20250614044745_m2'
)
BEGIN
    INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
    VALUES (N'20250614044745_m2', N'8.0.10');
END;
GO

COMMIT;
GO

