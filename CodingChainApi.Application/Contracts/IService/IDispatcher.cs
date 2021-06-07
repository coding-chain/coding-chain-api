namespace Application.Contracts.IService
{
    public interface IDispatcher<TMessage>
    {
        public void Dispatch(TMessage message);
    }
}