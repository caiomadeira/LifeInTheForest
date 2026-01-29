using RedLoader;
using Sons.Ai.Vail;
using Sons.Input;
using SonsSdk;
using SUI;
using TheForest.Utils;
using UnityEngine;
using UnityEngine.Rendering;
using Sons.Items.Core;

namespace PhotoMode;

public class PhotoMode : SonsMod
{
    private bool _isFreeCamActive = false;

    private GameObject _freeCamObj;
    private Camera _freeCamComp;

    private float _speed = 2.0f;
    private float _mouseSensitivity = 1.0f;
    private float _rotationX = 0.0f;
    private float _rotationY = 0.0f;

    private Dictionary<SkinnedMeshRenderer, ShadowCastingMode> _originalShadowModes = new();
    public PhotoMode()
    {
        OnUpdateCallback = OnUpdate;
        OnGUICallback = DebugUI;
    }

    protected override void OnInitializeMod() { Config.Init(); }
    protected override void OnSdkInitialized() { PhotoModeUi.Create(); }
    protected override void OnGameStart() { RLog.Msg("Photo Mode Iniciado"); }

    private void OnUpdate()
    {
        if (Input.GetKeyDown(KeyCode.F6))
        {
            ToggleFreeCamera();
        }

        if (_isFreeCamActive && _freeCamObj != null)
        {
            UpdateFreeCamMovement();
        }
    }

    private void ToggleFreeCamera()
    {
        var vailWorldSim = VailWorldSimulation.Instance();
        if (LocalPlayer.Transform == null || vailWorldSim == null)
        {
            RLog.Error("something is null");
            return;
        }
        _isFreeCamActive = !_isFreeCamActive;
        RLog.Msg($"FreeCam Active: {_isFreeCamActive}");

        if (_isFreeCamActive)
        {
            Time.timeScale = 0f;

            vailWorldSim._aiPaused = true;
            AudioListener.pause = true;

            if (LocalPlayer.FpCharacter != null)
            {
                LocalPlayer.FpCharacter.LockView(true);
            }

            _freeCamObj = new GameObject("photo_mode_freecam");
            _freeCamComp = _freeCamObj.AddComponent<Camera>();

            float lookAtDistance = 3.5f;
            float lookAtHeight = 1.6f;

            Vector3 lookAtTarget = LocalPlayer.Transform.position + (Vector3.up * lookAtHeight);
            Vector3 lookAtNewPos = lookAtTarget - (LocalPlayer.Transform.forward * lookAtDistance) + (Vector3.up * 0.5f);
            _freeCamObj.transform.position = lookAtNewPos;
            _freeCamObj.transform.LookAt(lookAtTarget);

            //Transform mainCamTrans = Camera.main.transform;
            // _freeCamObj.transform.position = mainCamTrans.position;
            // _freeCamObj.transform.rotation = mainCamTrans.rotation;

            _freeCamComp.fieldOfView = Camera.main.fieldOfView;
            _freeCamComp.nearClipPlane = 0.01f;

            Vector3 euler = _freeCamObj.transform.rotation.eulerAngles;
            _rotationY = euler.y;
            _rotationX = euler.x;

            Camera.main.enabled = false;
            SetPlayerBodyVisibility(true);
            var statRecipe = GameObject.FindAnyObjectByType<StatRecipe>();
            if (statRecipe != null)
            {
                RLog.Msg("stat recipe found.");
                statRecipe._showUI = false;
            } else
            {
                RLog.Msg("stat recipe NOT found.");
            }

        } else {
            if (_freeCamObj != null) { UnityEngine.Object.Destroy(_freeCamObj); }
            if (Camera.main != null) { Camera.main.enabled = true; }
            SetPlayerBodyVisibility(false);
            if (LocalPlayer.FpCharacter != null)
            {
                LocalPlayer.FpCharacter.LockView(false);
            }
            AudioListener.pause = false;
            vailWorldSim._aiPaused = false;
            Time.timeScale = 1f;
        }
    }

    private void UpdateFreeCamMovement()
    {
        var vailWorldSim = VailWorldSimulation.Instance();
        if (vailWorldSim == null) return;
        if (!vailWorldSim.IsPaused)
        {
            float mouseX = Input.GetAxis("Mouse X") * _mouseSensitivity;
            float mouseY = Input.GetAxis("Mouse Y") * _mouseSensitivity;

            _rotationY += mouseX;
            _rotationX -= mouseY;
            _rotationX = Mathf.Clamp(_rotationX, -90f, 90f); 

            _freeCamObj.transform.rotation = Quaternion.Euler(_rotationX, _rotationY, 0);

            // --- Movimento (WASD + Q/E) ---
            float dt = Time.unscaledDeltaTime;
            Vector3 dir = Vector3.zero;
            float moveSpeed = _speed;
            if (Input.GetKey(KeyCode.LeftShift)) moveSpeed *= 2.0f;
            if (Input.GetKey(KeyCode.LeftControl)) moveSpeed *= 0.2f;
            if (Input.GetKey(KeyCode.W)) dir += _freeCamObj.transform.forward;
            if (Input.GetKey(KeyCode.S)) dir -= _freeCamObj.transform.forward;
            if (Input.GetKey(KeyCode.D)) dir += _freeCamObj.transform.right;
            if (Input.GetKey(KeyCode.A)) dir -= _freeCamObj.transform.right;
            if (Input.GetKey(KeyCode.E)) dir += Vector3.up;
            if (Input.GetKey(KeyCode.Q)) dir += Vector3.down;
            _freeCamObj.transform.position += dir * moveSpeed * dt;
        }
    }

    private void SetPlayerBodyVisibility(bool show)
    {
        if (LocalPlayer.Transform == null) return;

        var renderers = LocalPlayer.Transform.GetComponentsInChildren<SkinnedMeshRenderer>(true);
        if (show)
        {
            _originalShadowModes.Clear();
            foreach(var r in renderers)
            {
                if (!_originalShadowModes.ContainsKey(r)) { _originalShadowModes.Add(r, r.shadowCastingMode); };
                if (r.shadowCastingMode == ShadowCastingMode.ShadowsOnly)
                {
                    r.shadowCastingMode = ShadowCastingMode.On;
                }
                r.enabled = true;
                r.gameObject.layer = 0;
            }
            RLog.Msg($"Corrigido {renderers.Count} partes do corpo.");
        } else {
            foreach (var kvp in _originalShadowModes)
            {
                if (kvp.Key != null)
                {
                    kvp.Key.shadowCastingMode = kvp.Value;
                }
            }
            _originalShadowModes.Clear();
        }
    }

    private void DebugUI()
    {
        if (_isFreeCamActive)
        {
            GUI.color = Color.red;
            GUI.Label(new Rect(Screen.width / 2 - 50, 20, 200, 20), "FREE CAM ATIVA (F6 para sair)");
            GUI.Label(new Rect(Screen.width / 2 - 50, 40, 200, 20), "WASD = Mover | Q/E = Subir/Descer | Shift = Rapido");
        }
    }
}