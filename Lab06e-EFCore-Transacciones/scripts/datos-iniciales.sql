-- Los POST utilizan ClienteId=1. No insertar reservas ni movimientos aquí:
-- deben crearse mediante los experimentos de transacciones.
INSERT INTO Clientes (Id, Nombre, Email) VALUES
    (1, 'Ana García', 'ana@example.com')
ON CONFLICT(Id) DO NOTHING;
