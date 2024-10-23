namespace ConsoleApp.Handlers
{
    public class FiveHandler : IHandler
    {
        private IHandler _handler; //zamjirni keyingisi
        public string Handle(int amout) // 7 
        {
            if (amout >= 5)
            {
                int count = amout / 5; //1
                amout -= count * 5; // 7-5=2

                return $"Siz 5 $ likdan {count} ta olishingiz mumkin," + _handler?.Handle(amout) ?? string.Empty;
            }

            return _handler.Handle(amout);
        }

        public void MoveNext(IHandler handler)
        {
            _handler = handler;
        }
    }
}
