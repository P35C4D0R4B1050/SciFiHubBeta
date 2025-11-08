# ? GUÍA DEFINITIVA SIMPLIFICADA - SciFiHub

## ?? ESTADO ACTUAL

**Problema Resuelto**: ? Todos los errores de enums corregidos  
**Script Listo**: ? `Database/PreparacionPruebas.sql`  
**Compilación**: ? Exitosa  

---

## ?? EJECUTAR AHORA (3 PASOS - 2 MINUTOS)

### **PASO 1: Ejecutar Script SQL** (10 segundos)

```sql
-- Abrir SQL Server Management Studio (SSMS)
-- Conectar a localhost
-- Abrir archivo: Database/PreparacionPruebas.sql
-- Presionar F5 (Ejecutar)
```

**Resultado esperado**:
```
? Categorías insertadas exitosamente (8)
? Usuario vendedor.test ya existe
? Usuario admin.test ya existe
? La base de datos está lista para pruebas
```

---

### **PASO 2: Iniciar Aplicación** (30 segundos)

```bash
cd E:\Proyecto\SciFiHub\SciFiHub
dotnet run
```

**Resultado esperado**:
```
Now listening on: https://localhost:XXXX
Application started. Press Ctrl+C to shut down.
```

---

### **PASO 3: Probar en Navegador** (1 minuto)

1. Abrir: `https://localhost:XXXX`

2. **Login con usuario de prueba**:
   ```
   Username: admin.test
   Password: Test123!
   ```

3. **Ir a Admin ? Inventario**:
   - Click en "Agregar Libro"
   - ? Verificar que el select de Categoría tiene opciones
   - ? NO debe aparecer error "Invalid column name"

---

## ? VERIFICACIÓN RÁPIDA

Si todo funciona bien, verás:

1. ? **Categorías cargan** en el modal de Agregar Libro
2. ? **No hay errores** en la consola del navegador (F12)
3. ? **No hay errores** en el servidor (terminal de dotnet run)

---

## ?? DATOS CREADOS

### **Categorías** (8):
1. Ciencia Ficción
2. Fantasía
3. Cyberpunk
4. Distopía
5. Space Opera
6. Horror Cósmico
7. Steampunk
8. Viajes en el Tiempo

### **Usuarios de Prueba** (2):

| Username | Password | Rol |
|----------|----------|-----|
| `admin.test` | `Test123!` | Administrador |
| `vendedor.test` | `Test123!` | Vendedor |

---

## ?? PRUEBAS FUNCIONALES

Después de verificar que carga:

1. **Crear Usuario** (Admin):
   - Admin ? Usuarios ? Crear Usuario
   - Completar formulario
   - Guardar
   - ? NO debe aparecer "Error desconocido"

2. **Registro Rápido** (Vendedor):
   - Vendedor ? Nueva Venta
   - Click "Nuevo Cliente"
   - Registrar cliente
   - ? Debe mostrar credenciales generadas

3. **Usuarios Eliminados** (Admin):
   - Admin ? Usuarios
   - Eliminar un usuario
   - Activar toggle "Ver eliminados"
   - Restaurar usuario
   - ? Debe volver a la tabla activos

---

## ?? SOLUCIÓN DE PROBLEMAS

### **Error: "Conversion failed when converting the nvarchar value 'Activo'"**

**Causa**: Versión antigua del script

**Solución**:
```bash
# Descargar versión corregida
git pull

# O actualizar manualmente líneas:
-- Cambiar: Estado = 'Activo'
-- Por:     Estado = 0
```

---

### **Error: "Cannot insert duplicate key"**

**Causa**: Categorías ya existen

**Solución**:
```sql
-- Ejecutar ANTES de PreparacionPruebas.sql:
UPDATE Categorias 
SET IsDeleted = 0, Estado = 0, DeletedAt = NULL
WHERE IsDeleted = 1;
```

---

### **Error: "Login failed for user"**

**Causa**: SQL Server no está corriendo

**Solución**:
1. Abrir "Servicios" (Win + R ? services.msc)
2. Buscar "SQL Server"
3. Iniciar servicio

---

## ?? ARCHIVOS IMPORTANTES

| Archivo | Uso |
|---------|-----|
| `Database/PreparacionPruebas.sql` | ? **EJECUTAR ESTE** |
| `REFERENCIA_ENUMS.md` | ?? Valores de enums |
| `GUIA_PRUEBAS_FUNCIONALES.md` | ?? Pruebas detalladas |

---

## ?? IMPORTANTE

### **LO QUE SÍ DEBES HACER**:
- ? Ejecutar `PreparacionPruebas.sql`
- ? Usar valores numéricos para enums (0, 1, 2...)
- ? Consultar `REFERENCIA_ENUMS.md` si tienes dudas

### **LO QUE NO DEBES HACER**:
- ? Ejecutar `LimpiezaCategorias.sql` (fue eliminado)
- ? Usar strings para enums ('Activo', 'Vendedor', etc.)
- ? Intentar insertar categorías manualmente

---

## ?? DESPUÉS DE PROBAR

Si todo funciona:
1. ? Marcar como completo
2. ? Continuar con desarrollo normal
3. ? Consultar `GUIA_PRUEBAS_FUNCIONALES.md` para pruebas completas

Si algo falla:
1. ?? Copiar el error exacto
2. ?? Ver logs del servidor
3. ?? Consultar esta guía

---

**Tiempo Total**: 2 minutos  
**Dificultad**: Muy Baja  
**Estado**: ? Listo para Ejecutar

---

**Última Actualización**: Enero 2025  
**Versión**: 4.0.0 - SIMPLIFICADA  
**Estado**: ? Guía Definitiva - Sin Scripts Problemáticos
