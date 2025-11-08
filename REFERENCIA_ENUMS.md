# ?? REFERENCIA DE ENUMS - SciFiHub

## ?? IMPORTANTE

En la base de datos, los **enums se almacenan como ENTEROS**, no como strings.

---

## ?? LISTADO DE ENUMS Y SUS VALORES

### **1. EstadoCategoria**

```csharp
public enum EstadoCategoria
{
    Activo = 0,
    Inactivo = 1
}
```

**En SQL**:
```sql
-- Correcto ?
INSERT INTO Categorias (..., Estado, ...) VALUES (..., 0, ...); -- Activo
UPDATE Categorias SET Estado = 1 WHERE ...; -- Inactivo

-- Incorrecto ?
INSERT INTO Categorias (..., Estado, ...) VALUES (..., 'Activo', ...);
UPDATE Categorias SET Estado = 'Activo' WHERE ...;
```

**Consultas**:
```sql
-- Categorías activas
SELECT * FROM Categorias WHERE Estado = 0;

-- Categorías inactivas
SELECT * FROM Categorias WHERE Estado = 1;

-- Mostrar nombre del estado
SELECT 
    Nombre,
    CASE Estado 
        WHEN 0 THEN 'Activo'
        WHEN 1 THEN 'Inactivo'
        ELSE 'Desconocido'
    END AS EstadoNombre
FROM Categorias;
```

---

### **2. EstadoUsuario**

```csharp
public enum EstadoUsuario
{
    Activo = 0,
    Inactivo = 1
}
```

**En SQL**:
```sql
-- Usuarios activos
SELECT * FROM Usuarios WHERE Estado = 0;

-- Usuarios inactivos
SELECT * FROM Usuarios WHERE Estado = 1;
```

---

### **3. RolUsuario**

```csharp
public enum RolUsuario
{
    Administrador = 0,
    Vendedor = 1,
    Cliente = 2
}
```

**En SQL**:
```sql
-- Administradores
SELECT * FROM Usuarios WHERE Rol = 0;

-- Vendedores
SELECT * FROM Usuarios WHERE Rol = 1;

-- Clientes
SELECT * FROM Usuarios WHERE Rol = 2;

-- Mostrar nombre del rol
SELECT 
    NombreCompleto,
    CASE Rol 
        WHEN 0 THEN 'Administrador'
        WHEN 1 THEN 'Vendedor'
        WHEN 2 THEN 'Cliente'
        ELSE 'Desconocido'
    END AS RolNombre
FROM Usuarios;
```

---

### **4. EstadoLibro**

```csharp
public enum EstadoLibro
{
    Disponible = 0,
    Agotado = 1,
    Descontinuado = 2
}
```

**En SQL**:
```sql
-- Libros disponibles
SELECT * FROM Libros WHERE Estado = 0;

-- Libros agotados
SELECT * FROM Libros WHERE Estado = 1;

-- Libros descontinuados
SELECT * FROM Libros WHERE Estado = 2;
```

---

### **5. EstadoVenta**

```csharp
public enum EstadoVenta
{
    Pendiente = 0,
    Procesando = 1,
    Completada = 2,
    Cancelada = 3,
    Reembolsada = 4
}
```

**En SQL**:
```sql
-- Ventas pendientes
SELECT * FROM Ventas WHERE EstadoVenta = 0;

-- Ventas completadas
SELECT * FROM Ventas WHERE EstadoVenta = 2;

-- Ventas canceladas
SELECT * FROM Ventas WHERE EstadoVenta = 3;
```

---

### **6. MetodoPago**

```csharp
public enum MetodoPago
{
    Efectivo = 0,
    TarjetaCredito = 1,
    TarjetaDebito = 2,
    Yape = 3,
    Plin = 4,
    Transferencia = 5
}
```

**En SQL**:
```sql
-- Ventas en efectivo
SELECT * FROM Ventas WHERE MetodoPago = 0;

-- Ventas con Yape
SELECT * FROM Ventas WHERE MetodoPago = 3;
```

---

### **7. EstadoCarrito**

```csharp
public enum EstadoCarrito
{
    Activo = 0,
    Convertido = 1,
    Abandonado = 2
}
```

**En SQL**:
```sql
-- Carritos activos
SELECT * FROM CarritoCompras WHERE Estado = 0;

-- Carritos convertidos en venta
SELECT * FROM CarritoCompras WHERE Estado = 1;
```

---

### **8. TipoMovimientoInventario**

```csharp
public enum TipoMovimientoInventario
{
    Ingreso = 0,
    Venta = 1,
    Ajuste = 2,
    Devolucion = 3,
    Anulacion = 4
}
```

**En SQL**:
```sql
-- Ingresos de inventario
SELECT * FROM AuditoriaInventario WHERE TipoMovimiento = 0;

-- Ventas de inventario
SELECT * FROM AuditoriaInventario WHERE TipoMovimiento = 1;
```

---

## ?? FUNCIONES ÚTILES

### **Función SQL para convertir Estado a Nombre**

```sql
CREATE FUNCTION dbo.fn_EstadoCategoriaNombre(@Estado INT)
RETURNS NVARCHAR(20)
AS
BEGIN
    RETURN CASE @Estado 
        WHEN 0 THEN 'Activo'
        WHEN 1 THEN 'Inactivo'
        ELSE 'Desconocido'
    END
END
GO

-- Uso:
SELECT Nombre, dbo.fn_EstadoCategoriaNombre(Estado) AS Estado
FROM Categorias;
```

---

## ?? ERRORES COMUNES

### **Error 1: Usar String en lugar de Int**

```sql
-- ? INCORRECTO
INSERT INTO Categorias (..., Estado, ...) 
VALUES (..., 'Activo', ...);

-- Error:
-- The INSERT statement conflicted with the CHECK constraint
```

**Solución**:
```sql
-- ? CORRECTO
INSERT INTO Categorias (..., Estado, ...) 
VALUES (..., 0, ...);
```

---

### **Error 2: Comparar con String**

```sql
-- ? INCORRECTO
SELECT * FROM Categorias WHERE Estado = 'Activo';

-- No da error, pero no devuelve resultados
```

**Solución**:
```sql
-- ? CORRECTO
SELECT * FROM Categorias WHERE Estado = 0;
```

---

## ?? TABLA DE REFERENCIA RÁPIDA

| Enum | Valor | Nombre |
|------|-------|--------|
| **EstadoCategoria** | 0 | Activo |
| | 1 | Inactivo |
| **EstadoUsuario** | 0 | Activo |
| | 1 | Inactivo |
| **RolUsuario** | 0 | Administrador |
| | 1 | Vendedor |
| | 2 | Cliente |
| **EstadoLibro** | 0 | Disponible |
| | 1 | Agotado |
| | 2 | Descontinuado |
| **EstadoVenta** | 0 | Pendiente |
| | 1 | Procesando |
| | 2 | Completada |
| | 3 | Cancelada |
| | 4 | Reembolsada |
| **MetodoPago** | 0 | Efectivo |
| | 1 | TarjetaCredito |
| | 2 | TarjetaDebito |
| | 3 | Yape |
| | 4 | Plin |
| | 5 | Transferencia |
| **EstadoCarrito** | 0 | Activo |
| | 1 | Convertido |
| | 2 | Abandonado |
| **TipoMovimientoInventario** | 0 | Ingreso |
| | 1 | Venta |
| | 2 | Ajuste |
| | 3 | Devolucion |
| | 4 | Anulacion |

---

## ?? TIPS

1. **Siempre usar valores numéricos** en INSERT/UPDATE
2. **Usar CASE** para mostrar nombres en SELECT
3. **Crear funciones auxiliares** para conversión
4. **Documentar** los valores en comentarios del código

---

**Ubicación del archivo fuente**: `SciFiHub.Domain/Enums/Enumerations.cs`

**Última Actualización**: Enero 2025
