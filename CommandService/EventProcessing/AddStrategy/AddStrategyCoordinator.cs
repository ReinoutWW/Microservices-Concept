namespace CommandService.EventProcessing.AddStrategy
{
    public class AddStrategyCoordinator
    {
        private IAddStrategy? addStrategy;

        public void SetStrategy(IAddStrategy strategy) 
        {
            this.addStrategy = strategy;
        }

        public void Add(string message)
        {
            if(addStrategy == null) 
            {
                throw new Exception("Strategy not set for the Add method");
            } else {
                this.addStrategy.Add(message);
            }
        }
    }    
    
}