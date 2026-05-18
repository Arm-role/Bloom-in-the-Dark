public interface IInventoryUIRoot
{
  bool IsOpen { get; }
  void Open();
  void Close();
}