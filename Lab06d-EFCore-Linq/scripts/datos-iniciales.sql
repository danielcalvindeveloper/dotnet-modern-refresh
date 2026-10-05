-- Escenario pequeño para observar filtros y traducción LINQ -> SQL.
INSERT INTO Clientes (Id, Nombre, Email) VALUES
    (1, 'Ana García', 'ana@example.com'),
    (2, 'Bruno López', 'bruno@example.com')
ON CONFLICT(Id) DO NOTHING;

INSERT INTO Reservas (Id, ClienteId, Fecha, Estado) VALUES
    (1, 1, '2026-09-28 09:00:00', 'Pendiente'),
    (2, 1, '2026-10-01 09:00:00', 'Pendiente'),
    (3, 2, '2026-10-03 11:00:00', 'Confirmada'),
    (4, 2, '2026-10-05 10:00:00', 'Cancelada'),
    (5, 1, '2026-10-07 16:00:00', 'Pendiente')
ON CONFLICT(Id) DO NOTHING;
