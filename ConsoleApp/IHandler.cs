namespace ConsoleApp
{
    public interface IHandler
    {
        string Handle(int amout);
        void MoveNext(IHandler handler);
    }
}
