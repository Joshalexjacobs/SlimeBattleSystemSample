using UnityEngine;

public class ScreenShake : MonoBehaviour
{

  private enum ScreenShakeState {
    Idle,
    Shaking
  }

  private const float DefaultMagnitude = 0.15f;

  private const float DefaultDuration = 1f;
  
  [SerializeField] private Transform cameraTransform;
  
  private Vector3 initialPosition;

  private ScreenShakeState screenShakeState;
  
  private float shakeDuration = 0f;
  
  private void Awake() {
    initialPosition = cameraTransform.localPosition;
  }
  
  public void TriggerShake() {
    shakeDuration = DefaultDuration;
    
    screenShakeState = ScreenShakeState.Shaking;
  }
  
  private void Update() {
    if (screenShakeState == ScreenShakeState.Idle) return;

    if (shakeDuration > 0) {
      cameraTransform.localPosition = initialPosition + Random.insideUnitSphere * DefaultMagnitude;

      shakeDuration -= Time.deltaTime;
    }
    else {
      StopShaking();
    }
  }

  private void StopShaking() {
    shakeDuration = 0f;
    
    cameraTransform.localPosition = initialPosition;

    screenShakeState = ScreenShakeState.Idle;
  }
  
}
