#nullable enable

using System.Collections.Generic;

// Collection contract — concrete impl คือ HintLibrary SO ใน 4_Infrastructure
public interface IHintLibrary
{
  IReadOnlyList<IHintEntry> Entries { get; }

  // คืน null ถ้าไม่เจอ — caller log warning เอง
  IHintEntry? GetById(string id);
}
