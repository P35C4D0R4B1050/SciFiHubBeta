namespace SciFiHub.Domain.ValueObjects;

/// <summary>
/// Value Object para dirección de envío
/// </summary>
public sealed class DireccionEnvio
{
    public string Calle { get; private set; }
    public string Ciudad { get; private set; }
    public string Departamento { get; private set; }
    public string? CodigoPostal { get; private set; }
    public string Pais { get; private set; }
    public string? Referencia { get; private set; }

    // Constructor privado para EF Core
    private DireccionEnvio()
    {
        Calle = string.Empty;
        Ciudad = string.Empty;
        Departamento = string.Empty;
        Pais = string.Empty;
    }

    public DireccionEnvio(
        string calle,
        string ciudad,
        string departamento,
        string pais,
        string? codigoPostal = null,
        string? referencia = null)
    {
        if (string.IsNullOrWhiteSpace(calle))
            throw new ArgumentException("La calle es requerida", nameof(calle));
        if (string.IsNullOrWhiteSpace(ciudad))
            throw new ArgumentException("La ciudad es requerida", nameof(ciudad));
        if (string.IsNullOrWhiteSpace(departamento))
            throw new ArgumentException("El departamento es requerido", nameof(departamento));
        if (string.IsNullOrWhiteSpace(pais))
            throw new ArgumentException("El país es requerido", nameof(pais));

        Calle = calle;
        Ciudad = ciudad;
        Departamento = departamento;
        CodigoPostal = codigoPostal;
        Pais = pais;
        Referencia = referencia;
    }

    public string DireccionCompleta =>
        $"{Calle}, {Ciudad}, {Departamento}{(string.IsNullOrWhiteSpace(CodigoPostal) ? "" : $" {CodigoPostal}")}, {Pais}";

    // Implementación de igualdad para Value Objects
    public override bool Equals(object? obj)
    {
        if (obj is not DireccionEnvio other)
            return false;

        return Calle == other.Calle &&
               Ciudad == other.Ciudad &&
               Departamento == other.Departamento &&
               CodigoPostal == other.CodigoPostal &&
               Pais == other.Pais &&
               Referencia == other.Referencia;
    }

    public override int GetHashCode()
    {
        return HashCode.Combine(Calle, Ciudad, Departamento, CodigoPostal, Pais, Referencia);
    }
}
