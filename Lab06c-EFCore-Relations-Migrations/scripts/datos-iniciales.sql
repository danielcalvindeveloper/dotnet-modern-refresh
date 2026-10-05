-- El POST necesita un cliente existente. Las reservas se crean desde la API.
-- Telefono es nullable en la migration AgregarTelefonoCliente: aquí queda NULL.
INSERT INTO Clientes (Id, Nombre, Email) VALUES
    (1, 'Ana García', 'ana@example.com'),
    (2, 'Bruno López', 'bruno@example.com'),
    (3, 'Carla Pérez', 'carla@example.com')
ON CONFLICT(Id) DO NOTHING;
