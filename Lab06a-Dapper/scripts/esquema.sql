-- El CREATE explícito que antes estaba en Program.cs; no hay EF migrations.
CREATE TABLE IF NOT EXISTS Clientes (
    Id INTEGER PRIMARY KEY,
    Nombre TEXT NOT NULL,
    Email TEXT NOT NULL
);
