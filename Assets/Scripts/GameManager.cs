using Root;
using UnityEngine;

public static class GameManager {
  public static PlayerInputActions Input;
  public static Player Player;
  public static Camera Camera;
  public static Train Train;
  public static readonly Vector2 RTSize = new(640,360);  

  [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
  private static void CreateInput() {
      Input = new PlayerInputActions();
      Input.Enable();
      Input.Movement.Enable();
      Input.CameraMovement.Enable();
      Input.Interaction.Enable();
  }

}