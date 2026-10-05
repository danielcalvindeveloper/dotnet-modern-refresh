namespace Lab07b.Services;

public class PruebaService : IPruebaService
{
    public void ProvocarError()
    {
        Console.WriteLine("[Service] ProvocarError");
        Console.WriteLine("[Exception] Se lanza InvalidOperationException");
        throw new InvalidOperationException(
            "Error provocado deliberadamente para estudiar el manejo global.");
    }
}
