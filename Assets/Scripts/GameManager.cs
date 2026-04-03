using Root;
using UnityEngine;

public static class GameManager {
  public static PlayerInputActions Input;
  public static Player Player;

  [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
  private static void CreateInput() {
      Input = new PlayerInputActions();
      Input.Enable();
      Input.Movement.Enable();
      Input.CameraMovement.Enable();
      Input.Interaction.Enable();
  }

}