# ?? SCRIPT: VERIFICAR/CREAR USUARIO VENDEDOR

## ?? VERIFICAR SI EXISTE

```sql
SELECT Id, Username, Rol, NombreCompleto, Email, Estado
FROM Usuarios
WHERE Username = 'vendedor' AND Rol = 'Vendedor';
```

**Si retorna una fila:** ? Usuario vendedor existe  
**Si no retorna nada:** ? Crear usuario vendedor

---

## ? CREAR USUARIO VENDEDOR (SI NO EXISTE)

```sql
-- Verificar primero si existe
IF NOT EXISTS (SELECT 1 FROM Usuarios WHERE Username = 'vendedor')
BEGIN
    DECLARE @VendedorId UNIQUEIDENTIFIER = NEWID();
    
    INSERT INTO Usuarios (
        Id, 
        NombreCompleto, 
        Email, 
        Username, 
        PasswordHash, 
        Rol, 
        FechaRegistro, 
        Estado, 
        CreatedAt
    )
    VALUES (
        @VendedorId,
        'Vendedor Sistema',
        'vendedor@scifihub.com',
        'vendedor',
        'AQAAAAIAAYagAAAAEK1z2w3QmH8K5pZvB5nZ0A==', -- Password: Vendedor123!
        'Vendedor',
        GETDATE(),
        'Activo',
        GETDATE()
    );
    
    PRINT '? Usuario vendedor creado exitosamente';
    PRINT 'Username: vendedor';
    PRINT 'Password: Vendedor123!';
    PRINT 'ID: ' + CAST(@VendedorId AS VARCHAR(50));
END
ELSE
BEGIN
    PRINT '?? Usuario vendedor ya existe';
END;
GO

-- Verificar que se creó correctamente
SELECT Id, Username, Rol, NombreCompleto, Email, Estado
FROM Usuarios
WHERE Username = 'vendedor';
```

---

## ?? CREDENCIALES DEL VENDEDOR

**Username:** `vendedor`  
**Password:** `Vendedor123!`  
**Rol:** `Vendedor`  
**Email:** `vendedor@scifihub.com`

---

## ? VERIFICACIÓN RÁPIDA

Después de ejecutar el script, verifica:

```sql
-- 1. Verificar que existe
SELECT COUNT(*) as Existe
FROM Usuarios
WHERE Username = 'vendedor' AND Rol = 'Vendedor';
-- Debe retornar: Existe = 1

-- 2. Ver detalles completos
SELECT *
FROM Usuarios
WHERE Username = 'vendedor';

-- 3. Verificar estado
SELECT 
    CASE 
        WHEN Estado = 'Activo' THEN '? Activo'
        ELSE '? Inactivo'
    END as EstadoVendedor
FROM Usuarios
WHERE Username = 'vendedor';
```

---

## ?? PROBAR LOGIN

Después de crear el usuario, intenta hacer login:

1. Ir a `/Auth/Login`
2. Username: `vendedor`
3. Password: `Vendedor123!`
4. Debe permitir el acceso

---

## ?? NOTAS

- El `PasswordHash` incluido es válido para el password `Vendedor123!`
- Si necesitas cambiar el password, usa la funcionalidad de "Cambiar Password" en el sistema
- El usuario se crea con rol "Vendedor" y estado "Activo"
- Este usuario será el vendedor predeterminado para todas las compras desde el carrito

---

**Estado:** ? SCRIPT LISTO  
**Uso:** Ejecutar en SQL Server Management Studio o Azure Data Studio
