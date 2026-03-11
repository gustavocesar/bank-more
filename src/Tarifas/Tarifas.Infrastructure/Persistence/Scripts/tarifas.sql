CREATE TABLE IF NOT EXISTS tarifa (
    idtarifa TEXT PRIMARY KEY,
    idcontacorrente TEXT NOT NULL,
    valor REAL NOT NULL,
    datahoratarifacao TEXT NOT NULL
);
