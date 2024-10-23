namespace ConsoleApp.Handlers
{
    public class FiftyHandler : IHandler
    {
        private IHandler _handler; //zamjirni keyingisi
        public string Handle(int amout) // 75 
        {
            if (amout >= 50)
            {
                int count = amout / 50; //1
                amout -= count * 50; // 75-50=25

                return $"Siz 50 $ likdan {count} ta olishingiz mumkin," + _handler?.Handle(amout) ?? string.Empty;
            }

            return _handler.Handle(amout);
        }

        public void MoveNext(IHandler handler)
        {
            _handler = handler;
        }
    }
}
