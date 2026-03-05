INSERT INTO ClienteMembresia (Id, ClienteID, MembresiaID, FechaAsignacion, ProximaFechaPago, FechaBaja)
SELECT
    NEWID(),
    C.Id,
    C.MembresiaID,
    GETDATE(),
    DATEADD(day, M.Regularidad, GETDATE()),
    NULL
FROM Cliente C
INNER JOIN Membresia M ON C.MembresiaID = M.Id
WHERE C.MembresiaID IS NOT NULL;
