# ? CHECKLIST DE REESTRUCTURACIÓN SCIFIHUB

---

## ?? OBJETIVO
Migrar de arquitectura monolítica a arquitectura en capas (MVC)

---

## ?? FASE 1: PREPARACIÓN

- [ ] **Backup del código**
  ```powershell
  git add .
  git commit -m "Backup antes de reestructuración"
  ```

- [ ] **Cerrar Visual Studio**
  - Cerrar todas las instancias de VS
  - Cerrar VS Code si está abierto

- [ ] **Verificar directorio**
  ```powershell
  cd E:\Proyecto\SciFiHub\SciFiHub\
  pwd  # Debe mostrar: E:\Proyecto\SciFiHub\SciFiHub
  ```

- [ ] **Verificar archivos existen**
  - [ ] `ReestructurarTodoEnUno.ps1`
  - [ ] `ReestructurarProyecto.ps1`
  - [ ] `VerificarReestructuracion.ps1`

---

## ?? FASE 2: EJECUCIÓN

- [ ] **Ejecutar script principal**
  ```powershell
  .\ReestructurarTodoEnUno.ps1
  ```

- [ ] **Revisar output del script**
  - [ ] No hay errores rojos
  - [ ] Archivos movidos exitosamente
  - [ ] Referencias actualizadas

- [ ] **Verificar mensajes de confirmación**
  - [ ] "? Reestructuración completada"
  - [ ] "? Build exitoso"
  - [ ] "? Verificación completada"

---

## ?? FASE 3: VERIFICACIÓN MANUAL

### Estructura de Archivos

- [ ] **SciFiHub.Domain/** (proyecto)
  - [ ] `Common/BaseEntity.cs`
  - [ ] `Entities/Usuario.cs`
  - [ ] `Entities/Libro.cs`
  - [ ] `Entities/Categoria.cs`
  - [ ] `Entities/Venta.cs`
  - [ ] `Entities/CarritoCompra.cs`
  - [ ] `Entities/DetalleVenta.cs`
  - [ ] `Entities/DetalleCarrito.cs`
  - [ ] `Entities/AuditoriaInventario.cs`
  - [ ] `Interfaces/IRepository.cs`
  - [ ] `Interfaces/IUnitOfWork.cs`
  - [ ] `Interfaces/IUsuarioRepository.cs`
  - [ ] `Interfaces/ILibroRepository.cs`
  - [ ] `Interfaces/ICategoriaRepository.cs`
  - [ ] `Interfaces/IVentaRepository.cs`
  - [ ] `Interfaces/ICarritoCompraRepository.cs`
  - [ ] `Enums/Enumerations.cs`
  - [ ] `ValueObjects/DireccionEnvio.cs`

- [ ] **SciFiHub.Infrastructure/** (proyecto)
  - [ ] `Data/SciFiHubDbContext.cs`
  - [ ] `Data/Configurations/UsuarioConfiguration.cs`
  - [ ] `Data/Configurations/LibroConfiguration.cs`
  - [ ] `Data/Configurations/CategoriaConfiguration.cs`
  - [ ] `Data/Configurations/VentaConfiguration.cs`
  - [ ] `Data/Configurations/DetalleVentaConfiguration.cs`
  - [ ] `Data/Configurations/CarritoCompraConfiguration.cs`
  - [ ] `Data/Configurations/DetalleCarritoConfiguration.cs`
  - [ ] `Data/Configurations/AuditoriaInventarioConfiguration.cs`
  - [ ] `Repositories/Repository.cs`
  - [ ] `Repositories/UnitOfWork.cs`
  - [ ] `Repositories/UsuarioRepository.cs`
  - [ ] `Repositories/LibroRepository.cs`
  - [ ] `Repositories/CategoriaRepository.cs`
  - [ ] `Repositories/VentaRepository.cs`
  - [ ] `Repositories/CarritoCompraRepository.cs`

- [ ] **SciFiHub.Web/** (proyecto)
  - [ ] `Controllers/` (sin cambios)
  - [ ] `Views/` (sin cambios)
  - [ ] `Services/` (sin cambios)
  - [ ] `DTOs/` (sin cambios)
  - [ ] `wwwroot/` (sin cambios)

### Carpetas Eliminadas

- [ ] **SciFiHub.Web/SciFiHub.Domain/** (carpeta eliminada)
- [ ] **SciFiHub.Web/SciFiHub.Infrastructure/** (carpeta eliminada)

---

## ?? FASE 4: REFERENCIAS DE PROYECTOS

- [ ] **SciFiHub.Domain.csproj**
  - [ ] Sin referencias a otros proyectos
  - [ ] Solo paquetes base de .NET

- [ ] **SciFiHub.Infrastructure.csproj**
  - [ ] Referencia a `SciFiHub.Domain`
  - [ ] Paquetes:
    - [ ] EntityFrameworkCore
    - [ ] EntityFrameworkCore.SqlServer
    - [ ] EntityFrameworkCore.Design
    - [ ] Dapper
    - [ ] BCrypt.Net-Next

- [ ] **SciFiHub.Web.csproj**
  - [ ] Referencia a `SciFiHub.Infrastructure`
  - [ ] Paquetes:
    - [ ] AutoMapper
    - [ ] FluentValidation
    - [ ] QuestPDF
    - [ ] EntityFrameworkCore.Design

---

## ?? FASE 5: COMPILACIÓN

- [ ] **Limpiar solución**
  ```powershell
  dotnet clean
  ```

- [ ] **Restaurar paquetes**
  ```powershell
  dotnet restore
  ```
  - [ ] No hay errores
  - [ ] Todos los paquetes restaurados

- [ ] **Compilar proyecto**
  ```powershell
  dotnet build
  ```
  - [ ] Build succeeded
  - [ ] 0 Error(s)
  - [ ] Warnings < 5

---

## ?? FASE 6: PRUEBAS EN VISUAL STUDIO

- [ ] **Abrir Visual Studio**
  - [ ] Abrir `SciFiHub.sln`

- [ ] **Verificar Solution Explorer**
  - [ ] SciFiHub.Domain muestra archivos
  - [ ] SciFiHub.Infrastructure muestra archivos
  - [ ] SciFiHub.Web sin carpetas Domain/Infrastructure

- [ ] **Verificar dependencias**
  - [ ] SciFiHub.Domain ? Sin dependencias de proyecto
  - [ ] SciFiHub.Infrastructure ? Depende de Domain
  - [ ] SciFiHub.Web ? Depende de Infrastructure

- [ ] **Rebuild desde VS**
  - [ ] Build ? Rebuild Solution
  - [ ] Sin errores

---

## ?? FASE 7: PRUEBAS FUNCIONALES

- [ ] **Ejecutar aplicación**
  ```powershell
  dotnet run
  ```
  - [ ] Inicia sin errores
  - [ ] Base de datos conecta correctamente

- [ ] **Probar funcionalidades clave**
  - [ ] Login de usuario
  - [ ] Catálogo de libros
  - [ ] Agregar al carrito
  - [ ] Checkout
  - [ ] Dashboard admin
  - [ ] Gestión de inventario

- [ ] **Verificar sin errores**
  - [ ] No hay errores 500
  - [ ] No hay excepciones en consola
  - [ ] Todas las páginas cargan

---

## ?? FASE 8: MÉTRICAS DE ÉXITO

### Estructura
- [ ] ? 3 proyectos activos con código
- [ ] ? 0 carpetas Domain/Infrastructure en Web
- [ ] ? ~25 archivos movidos correctamente

### Compilación
- [ ] ? 0 errores de compilación
- [ ] ? < 5 warnings
- [ ] ? Tiempo de build < 30 segundos

### Funcionalidad
- [ ] ? Aplicación inicia correctamente
- [ ] ? Todas las funcionalidades operativas
- [ ] ? Sin errores en runtime

---

## ?? FASE 9: DOCUMENTACIÓN

- [ ] **Actualizar README.md del proyecto**
  - [ ] Agregar sección de arquitectura
  - [ ] Documentar estructura de proyectos

- [ ] **Commit de cambios**
  ```powershell
  git add .
  git commit -m "Reestructuración a arquitectura en capas"
  ```

- [ ] **Documentar decisiones**
  - [ ] Por qué se hizo la reestructuración
  - [ ] Beneficios obtenidos
  - [ ] Cambios realizados

---

## ?? FASE 10: FINALIZACIÓN

- [ ] **Revisar checklist completo**
  - [ ] Todas las fases completadas
  - [ ] Todos los checkboxes marcados

- [ ] **Crear tag de versión**
  ```powershell
  git tag -a v2.0-arquitectura-capas -m "Migración a arquitectura en capas"
  ```

- [ ] **Push de cambios** (opcional)
  ```powershell
  git push origin main
  git push origin --tags
  ```

- [ ] **Notificar al equipo**
  - Informar de la nueva estructura
  - Compartir documentación
  - Explicar beneficios

---

## ?? RESULTADOS ESPERADOS

### Antes
```
? 1 proyecto con código
? Arquitectura monolítica
? Difícil de mantener
? Difícil de testear
? Acoplamiento alto
```

### Después
```
? 3 proyectos con código
? Arquitectura en capas
? Fácil de mantener
? Fácil de testear
? Acoplamiento bajo
```

---

## ?? SI ALGO SALE MAL

### Opción 1: Revertir con Git
```powershell
git reset --hard HEAD
```

### Opción 2: Ejecutar nuevamente
```powershell
# Limpiar
dotnet clean
Remove-Item bin, obj -Recurse -Force

# Re-ejecutar
.\ReestructurarTodoEnUno.ps1
```

### Opción 3: Manual
1. Restaurar backup
2. Revisar logs de los scripts
3. Ejecutar paso a paso
4. Consultar `GUIA_REESTRUCTURACION_COMPLETA.md`

---

## ?? RECURSOS DE AYUDA

- **Guía completa:** `GUIA_REESTRUCTURACION_COMPLETA.md`
- **Resumen ejecutivo:** `RESUMEN_EJECUTIVO_REESTRUCTURACION.md`
- **README:** `README_REESTRUCTURACION.md`
- **Este checklist:** `CHECKLIST_REESTRUCTURACION.md`

---

## ? FIRMA DE COMPLETADO

**Fecha:** _______________

**Realizado por:** _______________

**Tiempo total:** _______________ minutos

**Resultado:** 
- [ ] ? Exitoso
- [ ] ?? Con advertencias
- [ ] ? Requiere correcciones

**Notas adicionales:**
```
_________________________________________________
_________________________________________________
_________________________________________________
_________________________________________________
```

---

**?? ¡Arquitectura en capas implementada con éxito!**

---

*Checklist de Reestructuración SciFiHub v1.0*  
*Migración de Monolítico a Arquitectura en Capas*
