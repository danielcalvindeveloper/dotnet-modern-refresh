-- Agrega los ejemplos faltantes; conserva registros existentes con estos Ids.
INSERT INTO Clientes (Id, Nombre, Email) VALUES
    (1, 'Ana García', 'ana@example.com'),
    (2, 'Bruno López', 'bruno@example.com'),
    (3, 'Carla Pérez', 'carla@example.com')
ON CONFLICT(Id) DO NOTHING;
