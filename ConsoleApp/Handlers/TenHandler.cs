namespace ConsoleApp.Handlers
{
    public class TenHandler : IHandler
    {
        private IHandler _handler; //zamjirni keyingisi
        public string Handle(int amout) // 17 
        {
            if (amout >= 10)
            {
                int count = amout / 10; //1
                amout -= count * 10; // 17-10=7

                return $"Siz 10 $ likdan {count} ta olishingiz mumkin," + _handler?.Handle(amout) ?? string.Empty;
            }

            return _handler.Handle(amout);
        }

        public void MoveNext(IHandler handler)
        {
            _handler = handler;
        }
    }
}
