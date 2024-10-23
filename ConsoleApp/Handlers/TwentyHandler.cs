namespace ConsoleApp.Handlers
{
    public class TwentyHandler : IHandler
    {
        private IHandler _handler; //zamjirni keyingisi
        public string Handle(int amout) // 25 
        {
            if (amout >= 20)
            {
                int count = amout / 20; //1
                amout -= count * 20; // 25-20=5

                return $"Siz 20 $ likdan {count} ta olishingiz mumkin," + _handler?.Handle(amout) ?? string.Empty;
            }

            return _handler.Handle(amout);
        }

        public void MoveNext(IHandler handler)
        {
            _handler = handler;
        }
    }
}
