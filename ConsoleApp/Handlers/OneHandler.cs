namespace ConsoleApp.Handlers
{
    public class OneHandler : IHandler
    {
        private IHandler _handler; //zamjirni keyingisi
        public string Handle(int amout) // 2 
        {
            if (amout >= 1)
            {
                int count = amout / 1; //2
                amout -= count * 1; // 2-2=0

                return $"Siz 1 $ likdan {count} ta olishingiz mumkin," + _handler?.Handle(amout) ?? string.Empty;
            }

            return _handler.Handle(amout);
        }

        public void MoveNext(IHandler handler)
        {
            _handler = handler;
        }
    }
}
