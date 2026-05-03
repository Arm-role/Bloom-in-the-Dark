using System.Collections.Generic;

public interface IItemDefinitionProvider
{
  IItemDefinition GetItem(int itemId);
  IEnumerable<IItemDefinition> GetAll();
}