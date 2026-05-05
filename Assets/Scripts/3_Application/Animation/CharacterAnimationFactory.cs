public class CharacterAnimationFactory
{
  public CharacterAnimationSystem Create(
    ICharacterAnimationLibrary library,
    ICharacterAnimationView view)
  {
    var system = new CharacterAnimationSystem(library);

    system.Initializa(view);

    return system;
  }
}
