namespace ArithmeticString.Compiling.Interfaces
{
    public interface IFunction<T>
    {
        T Calculate(T[] parameters);
    }
}
