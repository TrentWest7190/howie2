using System;
using System.Collections;
using System.Linq;
using UnityEngine;

namespace Nexweron.WebCamPlayer
{
	public class WebCamStream : MonoBehaviour
	{
		public event Action onWebCamTextureChanged = delegate { };
		public event Action prepareCompleted = delegate { };
		
		// Rotation
		public enum RotationMode {
			Auto = -1,
			Manual0 = 0,
			Manual90 = 90,
			Manual180 = 180,
			Manual270 = 270
		}
		
		[SerializeField][HideInInspector]
		private RotationMode _rotationMode = RotationMode.Auto;
		public RotationMode rotationMode	{
			get => _rotationMode;
			set => _rotationMode = value;
		}
		public int videoRotationAngle =>
			rotationMode == RotationMode.Auto &&_webCamTexture != null ?
				_webCamTexture.videoRotationAngle : (int)_rotationMode;
		
		// Device
		public enum DeviceMode {
			Auto,
			FrontFace,
			BackFace,
			DeviceIndex,
			DeviceName
		}
		
		[SerializeField]
		private DeviceMode _deviceMode;
		public DeviceMode deviceMode	{
			get => _deviceMode;
			set => _deviceMode = value;
		}
		
		[SerializeField]
		private string _requestedDeviceName;
		public string requestedDeviceName {
			get => _requestedDeviceName;
			set => _requestedDeviceName = value;
		}
		
		[SerializeField]
		private int _requestedDeviceIndex;
		public int requestedDeviceIndex {
			get => _requestedDeviceIndex;
			set => _requestedDeviceIndex = value;
		}
		
		// Resolution
		public enum ResolutionMode {
			Auto,
			Screen,
			Manual
		}
		
		[SerializeField]
		private ResolutionMode _resolutionMode;
		public ResolutionMode resolutionMode {
			get => _resolutionMode;
			set => _resolutionMode = value;
		}
		public int requestedWidth {
			get => _requestedSize.x;
			set => _requestedSize.x = value;
		}
		public int requestedHeight {
			get => _requestedSize.y;
			set => _requestedSize.y = value;
		}
		
		[SerializeField]
		private Vector2Int _requestedSize = new Vector2Int(640, 480);
		public Vector2Int requestedSize {
			get => _requestedSize;
			set => _requestedSize = value;
		}
		
		// Fps
		public enum FPSMode {
			Auto,
			Manual
		}
		
		[SerializeField]
		private FPSMode _fpsMode;
		public FPSMode fpsMode {
			get => _fpsMode;
			set => _fpsMode = value;
		}
		
		[SerializeField]
		private int _requestedFPS = 30;
		public int requestedFPS	{
			get => _requestedFPS;
			set => _requestedFPS = value;
		}
		
		[SerializeField]
		private bool _playOnAwake = false;
		public bool playOnAwake	{
			get => _playOnAwake;
			set => _playOnAwake = value;
		}
		
		private WebCamDevice _webCamDevice;
		public WebCamDevice webCamDevice => _webCamDevice;
		
		private WebCamTexture _webCamTexture;
		public WebCamTexture webCamTexture => _webCamTexture;
		
		private Vector2Int _webCamTextureSize = Vector2Int.one;
		public Vector2Int webCamTextureSize => _webCamTextureSize;
		
		private bool _isAuthorization = false;
		public bool isAuthorization => _isAuthorization;
		
		private bool _isPrepared = false;
		public bool isPrepared => _isPrepared;
		
		public bool didUpdateThisFrame => _webCamTexture && _webCamTexture.didUpdateThisFrame;
		
		private Vector2Int _screenSize = new Vector2Int(Screen.width, Screen.height);
		private bool _isWebCamTextureChanged = false;
		
		void Start() {
			if (_playOnAwake) {
				Play(true);
			}
		}
		
		private IEnumerator AuthorizeWebCam(bool isAutoPlay) {
			if (!Application.HasUserAuthorization(UserAuthorization.WebCam)) {
				Debug.Log("WebCamStream | WebCam authorization...");
				yield return Application.RequestUserAuthorization(UserAuthorization.WebCam);
				_isAuthorization = true;
			}
			_isAuthorization = false;
			
			if (Application.HasUserAuthorization(UserAuthorization.WebCam)) {
				Debug.Log("WebCamStream | WebCam authorized");
				if (WebCamTexture.devices.Length > 0) {
					CreateWebCamTexture();
					if (isAutoPlay) {
						Play();
					}
				} else {
					Debug.LogError("WebCamStream | No WebCam devices found");
				}
			} else {
				Debug.LogError("WebCamStream | WebCam can't authorize");
			}
		}
		
		private void CreateWebCamTexture() {
			if (_webCamTexture != null) {
				DestroyImmediate(_webCamTexture);
			}
			_webCamDevice = WebCamTexture.devices[0];
			
			if (_deviceMode == DeviceMode.FrontFace || _deviceMode == DeviceMode.BackFace) {
				var isFrontFacing = _deviceMode == DeviceMode.FrontFace;
				var device = WebCamTexture.devices.FirstOrDefault(d => d.isFrontFacing == isFrontFacing);
				if (device.name != null) {
					_webCamDevice = device;
				} else {
					Debug.LogError($"WebCamStream | Cannot find webcam device with FrontFacing = {isFrontFacing}. Play default...");
				}
			} else
			if (_deviceMode == DeviceMode.DeviceName) {
				var device = WebCamTexture.devices.FirstOrDefault(x => x.name == _requestedDeviceName);
				if (device.name != null) {
					_webCamDevice = device;
				} else {
					Debug.LogError($"WebCamStream | Cannot find webcam device with name «{_requestedDeviceName}». Play default...");
				}
			} else 
			if (_deviceMode == DeviceMode.DeviceIndex) {
				if (_requestedDeviceIndex >= 0 && _requestedDeviceIndex < WebCamTexture.devices.Length) {
					_webCamDevice = WebCamTexture.devices[_requestedDeviceIndex];
				} else {
					Debug.LogError($"WebCamStream | Cannot find webcam device at index {_requestedDeviceIndex}. Play default...");
				}
			}
			if (_resolutionMode == ResolutionMode.Auto) {
				_webCamTexture = new WebCamTexture(_webCamDevice.name);
			} else {
				_webCamTexture = GetCustomWebCamTexture(_webCamDevice);
			}
			_isWebCamTextureChanged = true;
		}
		private WebCamTexture GetCustomWebCamTexture(WebCamDevice device) {
			var size = _requestedSize;
			if (_resolutionMode == ResolutionMode.Screen) {
				if (_rotationMode == RotationMode.Manual90 || _rotationMode == RotationMode.Manual270) {
					size = new Vector2Int(_screenSize.y, _screenSize.x);
				} else {
					size = _screenSize;
				}
			}
			if (_fpsMode == FPSMode.Auto) {
				return new WebCamTexture(device.name, size.x, size.y);
			} else {
				return new WebCamTexture(device.name, size.x, size.y, _requestedFPS);
			}
		}
		
		private void StopAllWebCamTextures(bool isExceptOwn = false) {
			var wcts = FindObjectsOfType<WebCamTexture>();
			foreach (var wct in wcts) {
				if (!isExceptOwn || _webCamTexture != wct) {
					if (_webCamTexture.deviceName == wct.deviceName) {
						wct.Stop();
					}
				}
			}
		}
		
		void Update() {
			_screenSize.Set(Screen.width, Screen.height);

			if (_isPrepared) return;
			if (_webCamTexture == null) return;
			if (!_webCamTexture.didUpdateThisFrame) return;
			
			if (_isWebCamTextureChanged) {
				_isWebCamTextureChanged = false;
				_webCamTextureSize = new Vector2Int(_webCamTexture.width, _webCamTexture.height);
				onWebCamTextureChanged.Invoke();
			}
			_isPrepared = true;
			prepareCompleted.Invoke();
		}
		
		public void Authorize(bool isAutoPlay = false) {
			if (!_isAuthorization) {
				StartCoroutine(AuthorizeWebCam(isAutoPlay));
			}
		}
		
		public void Play(bool isAutoAuthorize = false) {
			if (_webCamTexture != null) {
				if (!_webCamTexture.isPlaying) {
					StopAllWebCamTextures(true);
				}
				_webCamTexture.Play();
			} else if (isAutoAuthorize) {
				Authorize(true);
			}
		}
		
		public void Pause() {
			if (_webCamTexture != null) {
				_webCamTexture.Pause();
			} else {
				Debug.LogWarning("WebCamStream | WebCam in not authorized");
			}
		}
		
		public void Stop() {
			if (_webCamTexture != null) {
				_webCamTexture.Stop();
				DestroyImmediate(_webCamTexture);
				onWebCamTextureChanged.Invoke();
			} else {
				Debug.LogWarning("WebCamStream | WebCam in not authorized");
			}
			_isPrepared = false;
		}
		
		public void RePlay() {
			Stop();
			Play(true);
		}
		
		void OnDestroy() {
			Stop();
			prepareCompleted = null;
		}
	}
}

