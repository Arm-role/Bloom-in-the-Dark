public class CharacterAnimationFactory
{
  public CharacterAnimationSystem Create(
    ICharacterAnimationLibrary library,
    ICharacterAnimationView view)
  {
    var system = new CharacterAnimationSystem(library);

    system.Initialize(view);

    return system;
  }
}
