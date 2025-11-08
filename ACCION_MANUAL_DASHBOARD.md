# ?? ACCIÓN MANUAL REQUERIDA - Dashboard.cshtml

## ? ERROR RESTANTE

**Archivo:** `Views/Admin/Dashboard.cshtml`  
**Línea:** ~228  
**Error:** La sección `@section Scripts {` no está cerrada

---

## ? SOLUCIÓN INMEDIATA

### OPCIÓN 1: Copiar y Pegar (Recomendado)

1. **Abrir** `Views/Admin/Dashboard.cshtml`
2. **Ir al final** del archivo (última línea)
3. **Agregar** el siguiente código al final:

```javascript
                            data: ventasPorSemana,
                            borderColor: 'rgb(75, 192, 192)',
                            backgroundColor: 'rgba(75, 192, 192, 0.2)',
                            tension: 0.1,
                            fill: true
                        }]
                    },
                    options: {
                        responsive: true,
                        maintainAspectRatio: true,
                        plugins: {
                            legend: { position: 'top' },
                            title: {
                                display: true,
                                text: 'Evolución de Ventas Mensuales',
                                font: { size: 16 }
                            }
                        },
                        scales: {
                            y: {
                                beginAtZero: true,
                                ticks: {
                                    callback: function(value) {
                                        return 'S/ ' + value.toFixed(0);
                                    }
                                }
                            }
                        }
                    }
                });
            });
        
        function calcularVentasPorSemana(data) {
            if (!data || !data.data || !data.data.items) {
                return [0, 0, 0, 0];
            }
            
            const ventas = data.data.items;
            const ventasPorSemana = [0, 0, 0, 0];
            const hoy = new Date();
            const primerDia = new Date(hoy.getFullYear(), hoy.getMonth(), 1);
            
            ventas.forEach(venta => {
                const fechaVenta = new Date(venta.fechaVenta);
                const diaDiferencia = Math.floor((fechaVenta - primerDia) / (1000 * 60 * 60 * 24));
                const semana = Math.min(Math.floor(diaDiferencia / 7), 3);
                
                if (semana >= 0 && semana < 4) {
                    ventasPorSemana[semana] += venta.total || 0;
                }
            });
            
            return ventasPorSemana;
        }
    </script>
}
```

4. **Guardar** el archivo
5. **Compilar:**

```powershell
dotnet build
```

---

### OPCIÓN 2: Copiar desde archivo temporal

El código también está disponible en: `DASHBOARD_CIERRE_SCRIPTS.txt`

1. Abrir `DASHBOARD_CIERRE_SCRIPTS.txt`
2. Copiar TODO el contenido
3. Pegar al final de `Views/Admin/Dashboard.cshtml`
4. Guardar y compilar

---

## ? DESPUÉS DE AGREGAR EL CIERRE

Ejecutar compilación:

```powershell
dotnet build
```

**Resultado Esperado:**
```
Build SUCCEEDED
    0 Warning(s)
    0 Error(s)
```

---

## ?? LUEGO EJECUTAR

```powershell
dotnet run
```

Y probar:
- ? Login como Admin
- ? Ir a Dashboard
- ? Verificar que el gráfico carga con datos reales
- ? Ver detalle de venta (boleta con scroll)

---

## ?? ESTADO ACTUAL

| Corrección | Estado |
|------------|--------|
| DetalleVenta.cshtml (@page error) | ? CORREGIDO |
| AdminController (AddErrorMessage) | ? CORREGIDO |
| AdminController (Estado propiedad) | ? CORREGIDO |
| Dashboard.cshtml (Scripts sin cerrar) | ?? MANUAL REQUERIDO |

---

**Tiempo Estimado:** 2 minutos  
**Dificultad:** Muy Baja (Copiar y Pegar)
