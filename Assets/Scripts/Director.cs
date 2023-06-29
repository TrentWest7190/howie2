using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;
using Cinemachine;

public class Director : MonoBehaviour
{
  [SerializeField] private InputActionAsset inputActions;
  [SerializeField] private GameObject cameraManPrefab;
  [SerializeField] private GameObject cameraMenGO;
  [SerializeField] private CinemachineBrain brain;
  [SerializeField] private CinemachineBlendDefinition.Style defaultBlendStyle;
  [SerializeField] private CinemachineBlendDefinition.Style overrideBlendStyle;
  private Dictionary<int, GameObject> cameraMen;
  private InputAction cameraSwitchAction;
  private InputAction cutModeAction;

  private void Awake()
  {
    GameObject[] cameraMenList = GameObject.FindGameObjectsWithTag("CameraMan");
    cameraMen = cameraMenList.Select((v, i) => new { Key = i, Value = v }).ToDictionary(o => o.Key + 1, o =>
    {
      Camera directorCamera = o.Value.GetComponentInChildren<Camera>();
      directorCamera.rect = new Rect(o.Key * 0.333f, 0, 0.333f, 0.333f);
      return o.Value;
    });
  }

  private void OnEnable()
  {
    cameraSwitchAction = inputActions.FindAction("Switch Camera");
    if (cameraSwitchAction != null)
    {
      cameraSwitchAction.started += OnCameraSwitch;
      cameraSwitchAction.Enable();
    }

    cutModeAction = inputActions.FindAction("Cut Mode");
    if (cutModeAction != null)
    {
      cutModeAction.started += OnCutMode;
      cutModeAction.canceled += OnCutMode;
      cutModeAction.Enable();
    }
  }

  private void Start()
  {
    Display.displays[1].Activate();
    brain.m_DefaultBlend.m_Style = defaultBlendStyle;
  }

  private void OnCameraSwitch(InputAction.CallbackContext context)
  {
    if (context.started)
    {
      int numKeyValue;
      int.TryParse(context.control.name, out numKeyValue);
      if (!cameraMen.ContainsKey(numKeyValue))
      {
        GameObject newCameraMan = Instantiate(cameraManPrefab, cameraMenGO.transform);
        cameraMen.Add(numKeyValue, newCameraMan);
        Camera directorCamera = newCameraMan.GetComponentInChildren<Camera>();
        directorCamera.rect = new Rect(((numKeyValue - 1) % 3) * 0.333f, Mathf.Floor((numKeyValue - 1) / 3) * 0.333f, 0.333f, 0.333f);
      }
      foreach (KeyValuePair<int, GameObject> _cameraMan in cameraMen)
      {
        CinemachineVirtualCamera vCam = _cameraMan.Value.GetComponentInChildren<CinemachineVirtualCamera>();
        FirstPersonCameraMan cameraManComponent = _cameraMan.Value.GetComponent<FirstPersonCameraMan>();
        Image recIndicator = _cameraMan.Value.GetComponentInChildren<Image>();
        vCam.enabled = false;
        cameraManComponent.SetMovementMode(EasyCharacterMovement.MovementMode.None);
        cameraManComponent.GetComponent<FirstPersonCameraMan>().isActiveCamera = false;
        recIndicator.enabled = false;
      }
      cameraMen[numKeyValue].GetComponentInChildren<CinemachineVirtualCamera>().enabled = true;
      cameraMen[numKeyValue].GetComponent<FirstPersonCameraMan>().SetMovementMode(EasyCharacterMovement.MovementMode.Flying);
      cameraMen[numKeyValue].GetComponent<FirstPersonCameraMan>().isActiveCamera = true;
      cameraMen[numKeyValue].GetComponentInChildren<Image>().enabled = true;

    }
  }

  public void SwitchToCamera(GameObject cameraPrefab)
  {
    foreach (KeyValuePair<int, GameObject> _cameraMan in cameraMen)
    {
      CinemachineVirtualCamera vCam = _cameraMan.Value.GetComponentInChildren<CinemachineVirtualCamera>();
      FirstPersonCameraMan cameraManComponent = _cameraMan.Value.GetComponent<FirstPersonCameraMan>();
      Image recIndicator = _cameraMan.Value.GetComponentInChildren<Image>();
      vCam.enabled = false;
      cameraManComponent.SetMovementMode(EasyCharacterMovement.MovementMode.None);
      cameraManComponent.isActiveCamera = false;
      recIndicator.enabled = false;
    }
    cameraPrefab.GetComponentInChildren<CinemachineVirtualCamera>().enabled = true;
    cameraPrefab.GetComponent<FirstPersonCameraMan>().SetMovementMode(EasyCharacterMovement.MovementMode.Flying);
    cameraPrefab.GetComponent<FirstPersonCameraMan>().isActiveCamera = true;
    cameraPrefab.GetComponentInChildren<Image>().enabled = true;
  }

  private void OnCutMode(InputAction.CallbackContext context)
  {
    if (context.started)
    {
      brain.m_DefaultBlend.m_Style = overrideBlendStyle;
    }

    if (context.canceled)
    {
      brain.m_DefaultBlend.m_Style = defaultBlendStyle;
    }
  }
}
