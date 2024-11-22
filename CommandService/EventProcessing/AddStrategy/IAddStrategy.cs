namespace CommandService.EventProcessing.AddStrategy
{
    public interface IAddStrategy
    {
        public void Add(string publishedMessage);
    }
}