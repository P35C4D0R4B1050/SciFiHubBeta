# ? CORRECCIÓN APLICADA - Error de Check Constraint en Estado

## ?? PROBLEMA IDENTIFICADO

Error al ejecutar `PreparacionPruebas.sql`:
```
The INSERT statement conflicted with the CHECK constraint "CK_Categorias_Estado". 
The conflict occurred in database "SciFiHubDB", table "dbo.Categorias", column 'Estado'.
```

**Causa Raíz**: 
- Scripts usaban `'Activa'` (string)
- La columna `Estado` es de tipo `INT` (enum `EstadoCategoria`)
- Los enums en C# se almacenan como **enteros** en SQL Server

---

## ? SOLUCIÓN APLICADA

### **Scripts Corregidos**:

1. **`Database/PreparacionPruebas.sql`** ?
   - Cambiado: `Estado = 'Activa'` ? `Estado = 0`
   - Agregado: Columna `Orden` en INSERT

2. **`Database/LimpiezaCategorias.sql`** ?
   - Cambiado: `Estado = 'Activa'` ? `Estado = 0`
   - Agregado: Columna `Orden` en INSERT

### **Valores Correctos**:

```sql
-- EstadoCategoria enum
0 = Activo
1 = Inactivo

-- Ejemplos correctos:
INSERT INTO Categorias (..., Estado, ...) VALUES (..., 0, ...); -- ? Activo
UPDATE Categorias SET Estado = 0 WHERE ...; -- ? Activo
SELECT * FROM Categorias WHERE Estado = 0; -- ? Categorías activas
```

---

## ?? DOCUMENTACIÓN CREADA

**`REFERENCIA_ENUMS.md`** ?

Contiene:
- ? Valores de todos los enums del sistema
- ? Ejemplos de INSERT/UPDATE correctos
- ? Queries de ejemplo
- ? Tabla de referencia rápida
- ? Errores comunes y soluciones

**Enums documentados**:
1. EstadoCategoria (0=Activo, 1=Inactivo)
2. EstadoUsuario (0=Activo, 1=Inactivo)
3. RolUsuario (0=Administrador, 1=Vendedor, 2=Cliente)
4. EstadoLibro (0=Disponible, 1=Agotado, 2=Descontinuado)
5. EstadoVenta (0=Pendiente, 1=Procesando, 2=Completada, 3=Cancelada, 4=Reembolsada)
6. MetodoPago (0=Efectivo, 1=TarjetaCredito, 2=TarjetaDebito, 3=Yape, 4=Plin, 5=Transferencia)
7. EstadoCarrito (0=Activo, 1=Convertido, 2=Abandonado)
8. TipoMovimientoInventario (0=Ingreso, 1=Venta, 2=Ajuste, 3=Devolucion, 4=Anulacion)

---

## ?? PRÓXIMA ACCIÓN (30 SEGUNDOS)

### **Opción A: Limpieza + Preparación** ? RECOMENDADO

```sql
-- 1. Ejecutar en SSMS:
Database/LimpiezaCategorias.sql

-- 2. Luego ejecutar:
Database/PreparacionPruebas.sql
```

### **Opción B: Solo Preparación**

```sql
-- Ejecutar en SSMS:
Database/PreparacionPruebas.sql
```

---

## ? RESULTADO ESPERADO

Después de ejecutar los scripts:

```
=== INICIANDO VERIFICACIÓN Y PREPARACIÓN DE DATOS ===

1. Verificando categorías...
Categorías activas encontradas: 0
??  No hay categorías activas. Verificando si existen categorías eliminadas...
??  No hay categorías. Insertando categorías de prueba...
? Categorías insertadas exitosamente

Categorías disponibles:
Id                                   Nombre                  Descripcion                         Estado  IsDeleted
------------------------------------ ----------------------- ----------------------------------- ------- ---------
XXXXXXXX-XXXX-XXXX-XXXX-XXXXXXXXXXXX Ciencia Ficción        Novelas de ciencia ficción...      Activo  0
XXXXXXXX-XXXX-XXXX-XXXX-XXXXXXXXXXXX Cyberpunk              Futuros tecnológicos...            Activo  0
XXXXXXXX-XXXX-XXXX-XXXX-XXXXXXXXXXXX Distopía               Sociedades oscuras...              Activo  0
XXXXXXXX-XXXX-XXXX-XXXX-XXXXXXXXXXXX Fantasía               Mundos fantásticos...              Activo  0
XXXXXXXX-XXXX-XXXX-XXXX-XXXXXXXXXXXX Horror Cósmico         Terror lovecraftiano...            Activo  0
XXXXXXXX-XXXX-XXXX-XXXX-XXXXXXXXXXXX Space Opera            Aventuras espaciales...            Activo  0
XXXXXXXX-XXXX-XXXX-XXXX-XXXXXXXXXXXX Steampunk              Tecnología victoriana...           Activo  0
XXXXXXXX-XXXX-XXXX-XXXX-XXXXXXXXXXXX Viajes en el Tiempo    Paradojas temporales...            Activo  0

(8 filas afectadas)

2. Verificando usuarios...
? Usuario vendedor.test ya existe
? Usuario admin.test ya existe

=== RESUMEN FINAL ===
Item                    | Cantidad
------------------------|----------
Categorías Activas      | 8
Usuarios Activos        | X
...

? La base de datos está lista para pruebas
```

---

## ?? ARCHIVOS MODIFICADOS/CREADOS

| Archivo | Estado | Cambios |
|---------|--------|---------|
| `Database/PreparacionPruebas.sql` | ? CORREGIDO | Estado = 0, agregado Orden |
| `Database/LimpiezaCategorias.sql` | ? CORREGIDO | Estado = 0, agregado Orden |
| `REFERENCIA_ENUMS.md` | ? NUEVO | Documentación completa de enums |

---

## ?? PRÓXIMOS PASOS

1. ? Ejecutar `Database/LimpiezaCategorias.sql` (opcional)
2. ? Ejecutar `Database/PreparacionPruebas.sql`
3. ? Iniciar aplicación: `dotnet run`
4. ? Seguir `GUIA_PRUEBAS_FUNCIONALES.md`

---

## ?? IMPORTANTE PARA EL FUTURO

**Al escribir scripts SQL para SciFiHub**:

1. ? **NUNCA** usar strings para enums
2. ? **SIEMPRE** usar valores numéricos (0, 1, 2, etc.)
3. ? **CONSULTAR** `REFERENCIA_ENUMS.md` para valores correctos
4. ? **AGREGAR** columna `Orden` al insertar categorías

**Ejemplo**:
```sql
-- ? INCORRECTO
INSERT INTO Categorias (..., Estado, ...) VALUES (..., 'Activa', ...);

-- ? CORRECTO
INSERT INTO Categorias (..., Estado, Orden, ...) VALUES (..., 0, 1, ...);
```

---

## ? VERIFICACIÓN

Para verificar que los scripts funcionan:

```sql
-- Debe devolver 8 categorías
SELECT COUNT(*) AS Total
FROM Categorias 
WHERE IsDeleted = 0 AND Estado = 0;

-- Debe mostrar las 8 categorías
SELECT 
    Nombre,
    CASE Estado 
        WHEN 0 THEN 'Activo'
        WHEN 1 THEN 'Inactivo'
    END AS Estado,
    Orden
FROM Categorias
WHERE IsDeleted = 0 AND Estado = 0
ORDER BY Orden;
```

---

**Compilación**: ? **EXITOSA**  
**Scripts**: ? **CORREGIDOS**  
**Documentación**: ? **COMPLETA**  
**Estado**: ? **LISTO PARA EJECUTAR**

---

**Última Actualización**: Enero 2025  
**Versión**: 3.2.0  
**Estado**: ? Error de Check Constraint Corregido
