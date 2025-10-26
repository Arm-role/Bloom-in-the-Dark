public interface IActionPerformer
{
    void Setup();
    void Execute(IDataProvider data);
}