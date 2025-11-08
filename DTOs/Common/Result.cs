namespace SciFiHub.Web.DTOs.Common;

/// <summary>
/// Clase genérica para resultados de operaciones con estado de éxito/fallo
/// </summary>
public class Result<T>
{
    public bool Success { get; set; }
    public T? Data { get; set; }
    public string? ErrorMessage { get; set; }
    public List<string>? Errors { get; set; }

    public static Result<T> SuccessResult(T data)
    {
        return new Result<T>
        {
            Success = true,
            Data = data
        };
    }

    public static Result<T> FailureResult(string errorMessage)
    {
        return new Result<T>
        {
            Success = false,
            ErrorMessage = errorMessage
        };
    }

    public static Result<T> FailureResult(List<string> errors)
    {
        return new Result<T>
        {
            Success = false,
            Errors = errors,
            ErrorMessage = string.Join(", ", errors)
        };
    }
}

/// <summary>
/// Resultado sin datos genéricos
/// </summary>
public class Result
{
    public bool Success { get; set; }
    public string? ErrorMessage { get; set; }
    public List<string>? Errors { get; set; }

    public static Result SuccessResult()
    {
        return new Result { Success = true };
    }

    public static Result FailureResult(string errorMessage)
    {
        return new Result
        {
            Success = false,
            ErrorMessage = errorMessage
        };
    }

    public static Result FailureResult(List<string> errors)
    {
        return new Result
        {
            Success = false,
            Errors = errors,
            ErrorMessage = string.Join(", ", errors)
        };
    }
}
