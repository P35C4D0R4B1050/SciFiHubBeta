namespace SciFiHub.Web.Helpers;

/// <summary>
/// Helper para obtener datos de ubicaciones de Perú (Departamento de Ayacucho)
/// Mapeo según BD: DireccionDepartamento=Departamento, DireccionPais=Provincia, DireccionCiudad=Distrito
/// </summary>
public static class UbicacionPeruHelper
{
    /// <summary>
    /// Obtener lista de departamentos/regiones de Perú
    /// Por ahora solo Ayacucho como ejemplo
    /// </summary>
    public static List<Departamento> ObtenerDepartamentos()
    {
        return new List<Departamento>
        {
            new Departamento
            {
                Id = "05",
                Nombre = "Ayacucho",
                Provincias = ObtenerProvinciasAyacucho()
            }
        };
    }

    /// <summary>
    /// Obtener provincias del departamento de Ayacucho (11 provincias)
    /// </summary>
    private static List<Provincia> ObtenerProvinciasAyacucho()
    {
        return new List<Provincia>
        {
            new Provincia { Id = "0501", Nombre = "Huamanga", Distritos = ObtenerDistritosHuamanga() },
            new Provincia { Id = "0502", Nombre = "Cangallo", Distritos = ObtenerDistritosCangallo() },
            new Provincia { Id = "0503", Nombre = "Huanca Sancos", Distritos = ObtenerDistritosHuancaSancos() },
            new Provincia { Id = "0504", Nombre = "Huanta", Distritos = ObtenerDistritosHuanta() },
            new Provincia { Id = "0505", Nombre = "La Mar", Distritos = ObtenerDistritosLaMar() },
            new Provincia { Id = "0506", Nombre = "Lucanas", Distritos = ObtenerDistritosLucanas() },
            new Provincia { Id = "0507", Nombre = "Parinacochas", Distritos = ObtenerDistritosParinacochas() },
            new Provincia { Id = "0508", Nombre = "Páucar del Sara Sara", Distritos = ObtenerDistritosPaucar() },
            new Provincia { Id = "0509", Nombre = "Sucre", Distritos = ObtenerDistritosSucre() },
            new Provincia { Id = "0510", Nombre = "Víctor Fajardo", Distritos = ObtenerDistritosVictorFajardo() },
            new Provincia { Id = "0511", Nombre = "Vilcas Huamán", Distritos = ObtenerDistritosVilcasHuaman() }
        };
    }

    /// <summary>
    /// Distritos de Huamanga (3 distritos principales)
    /// </summary>
    private static List<Distrito> ObtenerDistritosHuamanga()
    {
        return new List<Distrito>
        {
            new Distrito { Id = "050101", Nombre = "Ayacucho (Cercado)" },
            new Distrito { Id = "050102", Nombre = "Carmen Alto" },
            new Distrito { Id = "050103", Nombre = "San Juan Bautista" }
        };
    }

    /// <summary>
    /// Distritos de Cangallo (3 distritos principales)
    /// </summary>
    private static List<Distrito> ObtenerDistritosCangallo()
    {
        return new List<Distrito>
        {
            new Distrito { Id = "050201", Nombre = "Cangallo" },
            new Distrito { Id = "050202", Nombre = "Chuschi" },
            new Distrito { Id = "050203", Nombre = "Los Morochucos" }
        };
    }

    /// <summary>
    /// Distritos de Huanca Sancos (3 distritos principales)
    /// </summary>
    private static List<Distrito> ObtenerDistritosHuancaSancos()
    {
        return new List<Distrito>
        {
            new Distrito { Id = "050301", Nombre = "Sancos" },
            new Distrito { Id = "050302", Nombre = "Carapo" },
            new Distrito { Id = "050303", Nombre = "Sacsamarca" }
        };
    }

    /// <summary>
    /// Distritos de Huanta (3 distritos principales)
    /// </summary>
    private static List<Distrito> ObtenerDistritosHuanta()
    {
        return new List<Distrito>
        {
            new Distrito { Id = "050401", Nombre = "Huanta" },
            new Distrito { Id = "050402", Nombre = "Luricocha" },
            new Distrito { Id = "050403", Nombre = "Santillana" }
        };
    }

    /// <summary>
    /// Distritos de La Mar (3 distritos principales)
    /// </summary>
    private static List<Distrito> ObtenerDistritosLaMar()
    {
        return new List<Distrito>
        {
            new Distrito { Id = "050501", Nombre = "San Miguel" },
            new Distrito { Id = "050502", Nombre = "Anco" },
            new Distrito { Id = "050503", Nombre = "Ayna" }
        };
    }

    /// <summary>
    /// Distritos de Lucanas (3 distritos principales)
    /// </summary>
    private static List<Distrito> ObtenerDistritosLucanas()
    {
        return new List<Distrito>
        {
            new Distrito { Id = "050601", Nombre = "Puquio" },
            new Distrito { Id = "050602", Nombre = "Aucará" },
            new Distrito { Id = "050603", Nombre = "Cabana" }
        };
    }

    /// <summary>
    /// Distritos de Parinacochas (3 distritos principales)
    /// </summary>
    private static List<Distrito> ObtenerDistritosParinacochas()
    {
        return new List<Distrito>
        {
            new Distrito { Id = "050701", Nombre = "Coracora" },
            new Distrito { Id = "050702", Nombre = "Chumpi" },
            new Distrito { Id = "050703", Nombre = "Coronel Castañeda" }
        };
    }

    /// <summary>
    /// Distritos de Páucar del Sara Sara (3 distritos principales)
    /// </summary>
    private static List<Distrito> ObtenerDistritosPaucar()
    {
        return new List<Distrito>
        {
            new Distrito { Id = "050801", Nombre = "Pausa" },
            new Distrito { Id = "050802", Nombre = "Colta" },
            new Distrito { Id = "050803", Nombre = "Corculla" }
        };
    }

    /// <summary>
    /// Distritos de Sucre (3 distritos principales)
    /// </summary>
    private static List<Distrito> ObtenerDistritosSucre()
    {
        return new List<Distrito>
        {
            new Distrito { Id = "050901", Nombre = "Querobamba" },
            new Distrito { Id = "050902", Nombre = "Belén" },
            new Distrito { Id = "050903", Nombre = "Chalcos" }
        };
    }

    /// <summary>
    /// Distritos de Víctor Fajardo (3 distritos principales)
    /// </summary>
    private static List<Distrito> ObtenerDistritosVictorFajardo()
    {
        return new List<Distrito>
        {
            new Distrito { Id = "051001", Nombre = "Huancapi" },
            new Distrito { Id = "051002", Nombre = "Alcamenca" },
            new Distrito { Id = "051003", Nombre = "Apongo" }
        };
    }

    /// <summary>
    /// Distritos de Vilcas Huamán (3 distritos principales)
    /// </summary>
    private static List<Distrito> ObtenerDistritosVilcasHuaman()
    {
        return new List<Distrito>
        {
            new Distrito { Id = "051101", Nombre = "Vilcas Huamán" },
            new Distrito { Id = "051102", Nombre = "Accomarca" },
            new Distrito { Id = "051103", Nombre = "Carhuanca" }
        };
    }
}

/// <summary>
/// Modelo de Departamento/Región
/// </summary>
public class Departamento
{
    public string Id { get; set; } = string.Empty;
    public string Nombre { get; set; } = string.Empty;
    public List<Provincia> Provincias { get; set; } = new();
}

/// <summary>
/// Modelo de Provincia
/// </summary>
public class Provincia
{
    public string Id { get; set; } = string.Empty;
    public string Nombre { get; set; } = string.Empty;
    public List<Distrito> Distritos { get; set; } = new();
}

/// <summary>
/// Modelo de Distrito
/// </summary>
public class Distrito
{
    public string Id { get; set; } = string.Empty;
    public string Nombre { get; set; } = string.Empty;
}
