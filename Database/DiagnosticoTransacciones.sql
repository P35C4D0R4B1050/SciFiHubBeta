-- =============================================
-- DIAGNÓSTICO DE TRANSACCIONES - SciFiHub
-- Verifica configuración de transacciones y bloqueos
-- =============================================

USE SciFiHubDB;
GO

PRINT '==============================================';
PRINT 'DIAGNÓSTICO DE TRANSACCIONES Y BLOQUEOS';
PRINT '==============================================';
PRINT '';

-- =============================================
-- 1. VERIFICAR NIVEL DE AISLAMIENTO
-- =============================================
PRINT '1. NIVEL DE AISLAMIENTO ACTUAL:';
PRINT '----------------------------------------------';

DBCC USEROPTIONS;

PRINT '';

-- =============================================
-- 2. VERIFICAR TRANSACCIONES ACTIVAS
-- =============================================
PRINT '2. TRANSACCIONES ACTIVAS:';
PRINT '----------------------------------------------';

SELECT 
    session_id,
    transaction_id,
    name,
    transaction_begin_time,
    DATEDIFF(SECOND, transaction_begin_time, GETDATE()) AS duracion_segundos,
    transaction_type,
    transaction_state
FROM sys.dm_tran_active_transactions t
LEFT JOIN sys.dm_tran_session_transactions st ON t.transaction_id = st.transaction_id
ORDER BY transaction_begin_time;

PRINT '';

-- =============================================
-- 3. VERIFICAR BLOQUEOS ACTIVOS
-- =============================================
PRINT '3. BLOQUEOS ACTIVOS:';
PRINT '----------------------------------------------';

SELECT 
    request_session_id,
    resource_type,
    resource_database_id,
    resource_description,
    request_mode,
    request_status
FROM sys.dm_tran_locks
WHERE resource_database_id = DB_ID('SciFiHubDB')
ORDER BY request_session_id;

PRINT '';

-- =============================================
-- 4. VERIFICAR CONFIGURACIÓN DE BASE DE DATOS
-- =============================================
PRINT '4. CONFIGURACIÓN DE BASE DE DATOS:';
PRINT '----------------------------------------------';

SELECT 
    name,
    recovery_model_desc,
    is_read_committed_snapshot_on,
    snapshot_isolation_state_desc,
    is_auto_close_on,
    is_auto_shrink_on
FROM sys.databases
WHERE name = 'SciFiHubDB';

PRINT '';

-- =============================================
-- 5. HABILITAR READ_COMMITTED_SNAPSHOT (RECOMENDADO)
-- =============================================
PRINT '5. HABILITANDO READ_COMMITTED_SNAPSHOT:';
PRINT '----------------------------------------------';
PRINT 'Esto permite transacciones concurrentes sin bloqueos de lectura';
PRINT '';

-- Verificar si ya está habilitado
IF (SELECT is_read_committed_snapshot_on FROM sys.databases WHERE name = 'SciFiHubDB') = 0
BEGIN
    PRINT '?? READ_COMMITTED_SNAPSHOT está DESHABILITADO';
    PRINT '?? Ejecutando habilitación...';
    
    -- Cerrar conexiones activas para poder cambiar la configuración
    ALTER DATABASE SciFiHubDB SET SINGLE_USER WITH ROLLBACK IMMEDIATE;
    ALTER DATABASE SciFiHubDB SET READ_COMMITTED_SNAPSHOT ON;
    ALTER DATABASE SciFiHubDB SET MULTI_USER;
    
    PRINT '? READ_COMMITTED_SNAPSHOT HABILITADO';
END
ELSE
BEGIN
    PRINT '? READ_COMMITTED_SNAPSHOT ya está HABILITADO';
END

PRINT '';

-- =============================================
-- 6. VERIFICAR ÍNDICES DE TABLAS PRINCIPALES
-- =============================================
PRINT '6. ÍNDICES DE TABLAS PRINCIPALES:';
PRINT '----------------------------------------------';

SELECT 
    OBJECT_NAME(i.object_id) AS Tabla,
    i.name AS Indice,
    i.type_desc AS Tipo,
    i.is_primary_key AS EsPK,
    i.is_unique AS EsUnico
FROM sys.indexes i
WHERE OBJECT_NAME(i.object_id) IN ('Usuarios', 'Libros', 'Ventas', 'DetallesVenta')
ORDER BY Tabla, i.index_id;

PRINT '';

-- =============================================
-- 7. VERIFICAR FOREIGN KEYS
-- =============================================
PRINT '7. FOREIGN KEYS ACTIVAS:';
PRINT '----------------------------------------------';

SELECT 
    OBJECT_NAME(fk.parent_object_id) AS TablaOrigen,
    COL_NAME(fkc.parent_object_id, fkc.parent_column_id) AS ColumnaOrigen,
    OBJECT_NAME(fk.referenced_object_id) AS TablaReferencia,
    COL_NAME(fkc.referenced_object_id, fkc.referenced_column_id) AS ColumnaReferencia,
    fk.name AS NombreFK,
    fk.delete_referential_action_desc AS AccionDelete
FROM sys.foreign_keys fk
INNER JOIN sys.foreign_key_columns fkc ON fk.object_id = fkc.constraint_object_id
WHERE OBJECT_NAME(fk.parent_object_id) IN ('Ventas', 'DetallesVenta', 'Usuarios')
ORDER BY TablaOrigen;

PRINT '';

-- =============================================
-- 8. ESTADÍSTICAS DE ESPACIO EN DISCO
-- =============================================
PRINT '8. ESTADÍSTICAS DE ESPACIO:';
PRINT '----------------------------------------------';

EXEC sp_spaceused 'Usuarios';
EXEC sp_spaceused 'Libros';
EXEC sp_spaceused 'Ventas';
EXEC sp_spaceused 'DetallesVenta';
EXEC sp_spaceused 'AuditoriasInventario';

PRINT '';

-- =============================================
-- RESUMEN Y RECOMENDACIONES
-- =============================================
PRINT '==============================================';
PRINT 'RESUMEN Y RECOMENDACIONES:';
PRINT '==============================================';
PRINT '';

DECLARE @ReadCommittedSnapshot BIT;
DECLARE @RecoveryModel NVARCHAR(60);

SELECT 
    @ReadCommittedSnapshot = is_read_committed_snapshot_on,
    @RecoveryModel = recovery_model_desc
FROM sys.databases
WHERE name = 'SciFiHubDB';

IF @ReadCommittedSnapshot = 1
BEGIN
    PRINT '? READ_COMMITTED_SNAPSHOT: HABILITADO (CORRECTO)';
    PRINT '   Permite transacciones concurrentes sin bloqueos de lectura';
END
ELSE
BEGIN
    PRINT '? READ_COMMITTED_SNAPSHOT: DESHABILITADO';
    PRINT '   RECOMENDACIÓN: Habilitar para evitar problemas de transacciones';
    PRINT '   EJECUTAR:';
    PRINT '   ALTER DATABASE SciFiHubDB SET READ_COMMITTED_SNAPSHOT ON;';
END

PRINT '';
PRINT 'MODELO DE RECUPERACIÓN: ' + @RecoveryModel;
IF @RecoveryModel = 'SIMPLE'
    PRINT '? Apropiado para desarrollo';
ELSE
    PRINT '?? Considerar cambiar a SIMPLE para desarrollo';

PRINT '';
PRINT '==============================================';
PRINT 'FIN DEL DIAGNÓSTICO';
PRINT '==============================================';
GO
