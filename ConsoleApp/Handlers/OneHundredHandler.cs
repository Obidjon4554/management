namespace ConsoleApp.Handlers
{
    public class OneHundredHandler : IHandler
    {
        private IHandler _handler; //zamjirni keyingisi
        public string Handle(int amout) // 275 
        {
            if (amout >= 100)
            {
                int count = amout / 100; //2
                amout -= count * 100; // 275-200=75

                return $"Siz 100 $ likdan {count} ta olishingiz mumkin," + _handler?.Handle(amout) ?? string.Empty;
            }

            return _handler.Handle(amout);
        }

        public void MoveNext(IHandler handler)
        {
            _handler = handler;
        }
    }
}
