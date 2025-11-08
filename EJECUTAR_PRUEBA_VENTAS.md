# ? EJECUTAR AHORA - Probar Registro de Ventas

## ?? OBJETIVO
Verificar que el registro de ventas funciona sin el error "Invalid column Value()"

---

## ?? PASOS RÁPIDOS

### 1. Compilar ?
```powershell
dotnet build
```
**Esperado:** Compilación correcta (ya verificado)

---

### 2. Ejecutar Aplicación
```powershell
cd E:\Proyecto\SciFiHub\SciFiHub
dotnet run
```

**URL:** https://localhost:7116

---

### 3. Probar como Admin/Vendedor

#### A. Login
- Usuario: `admin@scifihub.com` (o vendedor)
- Contraseña: La que configuraste

#### B. Ir a Nueva Venta
- Menu ? Ventas ? Nueva Venta

#### C. Completar Formulario
1. **Seleccionar Cliente:** Cualquier cliente de la lista
2. **Agregar Libro:** 
   - Buscar libro
   - Click "Agregar"
   - Cantidad: 1
3. **Dirección de Envío:** "Av. 28 de Julio 456, Ayacucho"
4. **Método de Pago:** Efectivo
5. **Notas:** (Opcional) "Prueba registro"

#### D. Registrar Venta
- Click "Registrar Venta"

---

## ? RESULTADO ESPERADO

### Pantalla:
```
? Venta registrada exitosamente
   Número de venta: V{fecha}-0001
```

### Consola (Logs):
```
? Venta insertada: {GUID}
? Venta V{fecha}-0001 registrada exitosamente
```

### Base de Datos:
```sql
-- Verificar en SSMS
SELECT TOP 1 * FROM Ventas ORDER BY FechaVenta DESC
SELECT TOP 1 * FROM DetallesVenta ORDER BY CreatedAt DESC
SELECT TOP 1 * FROM AuditoriasInventario ORDER BY FechaMovimiento DESC
```

---

## ? SI HAY ERROR

### Capturar Información:
1. **Mensaje de error** en pantalla
2. **Logs en consola** (InnerException)
3. **Ejecutar script de diagnóstico:**

```powershell
# En SSMS
USE SciFiHubDB;
EXEC sp_executesql N'Database/DiagnosticoVentas.sql'
```

4. **Copiar resultado completo** y enviármelo

---

## ?? DIAGNÓSTICO RÁPIDO

### Error: "Invalid column"
? **Ejecutar:** `Database/DiagnosticoVentas.sql` y copiarme la salida

### Error: "Constraint violation"
? Verificar que:
- Cliente existe en BD
- Vendedor existe en BD
- Libro tiene stock > 0

### Error: "Transaction rollback"
? Ver logs de consola para InnerException

---

## ?? VERIFICACIONES ADICIONALES

### A. Stock Actualizado
```sql
-- ANTES de registrar venta
SELECT ISBN, Titulo, Stock FROM Libros WHERE Id = '{GUID}'
-- Anotar Stock

-- DESPUÉS de registrar venta
SELECT ISBN, Titulo, Stock FROM Libros WHERE Id = '{GUID}'
-- Verificar: Stock = StockAnterior - Cantidad
```

### B. Auditoría Creada
```sql
SELECT 
    TipoMovimiento,
    StockAnterior,
    Cantidad,
    StockNuevo,
    Motivo
FROM AuditoriasInventario
WHERE LibroId = '{GUID}'
ORDER BY FechaMovimiento DESC
```

---

## ?? SI TODO FUNCIONA

### Próximos Pasos:
1. ? Ventas ? **FUNCIONAL**
2. ?? Carrito (Cliente) ? **POR CORREGIR**

---

## ?? NOTAS

- **Solución Aplicada:** ADO.NET puro (SqlConnection + SqlCommand)
- **Archivo Modificado:** `Services/VentaService.cs`
- **Método:** `RegistrarVentaAsync()`
- **Ventaja:** Control total, sin dependencia de mapeo EF

---

**Estado Actual:** ? Compilado y listo para probar  
**Siguiente:** Ejecutar y reportar resultado
