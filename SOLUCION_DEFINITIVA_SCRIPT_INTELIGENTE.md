# ? SOLUCIÓN DEFINITIVA - Preparación de Datos

## ?? SOLUCIÓN SIMPLIFICADA

**IMPORTANTE**: Solo necesitas ejecutar **UN** script:

```sql
Database/PreparacionPruebas.sql
```

Este script hace TODO lo necesario:
- ? Restaura categorías eliminadas si existen
- ? Crea categorías nuevas si no existen
- ? Crea usuarios de prueba
- ? Verifica columnas BaseEntity
- ? Muestra resumen completo

---

## ?? EJECUCIÓN (10 SEGUNDOS)

### **Paso Único: Ejecutar Script**

```sql
-- Abrir SSMS y ejecutar:
Database/PreparacionPruebas.sql
```

---

## ? RESULTADO ESPERADO

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
...
(8 filas)

2. Verificando usuarios...
Usuarios activos: X

Distribución por roles:
Rol             | Cantidad | Activos
----------------|----------|--------
Administrador   | X        | X
Vendedor        | X        | X
Cliente         | X        | X

? Usuario vendedor.test ya existe
? Usuario admin.test ya existe

=== RESUMEN FINAL ===
Item                    | Cantidad
------------------------|----------
Categorías Activas      | 8
Usuarios Activos        | X
Libros Disponibles      | X
Ventas Registradas      | X
Columnas BaseEntity     | 24

? La base de datos está lista para pruebas

Usuarios de prueba creados:
  - admin.test / Test123! (Administrador)
  - vendedor.test / Test123! (Vendedor)

Próximo paso: Ejecutar la aplicación con dotnet run
```

---

## ?? DESPUÉS DE EJECUTAR

```bash
# Iniciar aplicación
cd E:\Proyecto\SciFiHub\SciFiHub
dotnet run

# Abrir navegador
https://localhost:XXXX
```

**Usuarios de prueba**:
- `admin.test` / `Test123!` (Administrador)
- `vendedor.test` / `Test123!` (Vendedor)

---

## ?? SI HAY ERRORES

### **Error: "Conversion failed when converting the nvarchar value 'Activo' to data type int"**

**Causa**: Versión antigua del script

**Solución**: 
1. Asegúrate de usar la versión corregida de `PreparacionPruebas.sql`
2. El script debe usar `Estado = 0` (no `Estado = 'Activo'`)

### **Error: "Cannot insert duplicate key"**

**Causa**: Las categorías ya existen

**Solución**: El script detecta automáticamente y no las inserta de nuevo. Este error no debería ocurrir.

Si ocurre, ejecutar:
```sql
-- Restaurar categorías eliminadas
UPDATE Categorias 
SET IsDeleted = 0, 
    DeletedAt = NULL,
    Estado = 0,
    UpdatedAt = GETUTCDATE()
WHERE IsDeleted = 1;

-- Luego re-ejecutar
Database/PreparacionPruebas.sql
```

---

## ?? VALORES DE ENUMS (Referencia Rápida)

```sql
-- EstadoCategoria
0 = Activo
1 = Inactivo

-- EstadoUsuario
0 = Activo
1 = Inactivo

-- RolUsuario
0 = Administrador
1 = Vendedor
2 = Cliente

-- EstadoLibro
0 = Disponible
1 = Agotado
2 = Descontinuado

-- Ver REFERENCIA_ENUMS.md para lista completa
```

---

## ?? LO QUE NO DEBES HACER

? **NO** ejecutar `LimpiezaCategorias.sql` (causa errores)  
? **NO** intentar insertar categorías manualmente  
? **NO** usar strings para valores de enum  

? **SÍ** ejecutar solo `PreparacionPruebas.sql`  
? **SÍ** usar valores numéricos para enums  
? **SÍ** consultar `REFERENCIA_ENUMS.md` si tienes dudas  

---

## ?? ARCHIVOS

| Archivo | Estado | Uso |
|---------|--------|-----|
| `Database/PreparacionPruebas.sql` | ? USAR ESTE | Preparación completa |
| ~~`Database/LimpiezaCategorias.sql`~~ | ? ELIMINADO | Causaba errores |

---

**Tiempo**: 10 segundos  
**Dificultad**: Muy Baja  
**Estado**: ? Script Único - Simple y Efectivo
