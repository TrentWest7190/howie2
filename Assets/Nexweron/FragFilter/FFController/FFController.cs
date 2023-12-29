using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Nexweron.Common.Utils;

namespace Nexweron.FragFilter
{
	public interface IFFBridge
	{
		FFController ffController { set; get; }
	}
	
	[DisallowMultipleComponent][ExecuteAlways]
	[AddComponentMenu("Nexweron/FragFilter/FF Controller")]
	public class FFController : MonoBehaviour
	{
		private IFFBridge _bridge;
		public IFFBridge bridge => _bridge;

		public FFController SetBridgeNull(IFFBridge value) {
			if (_bridge != null && _bridge != value) {
				Debug.LogWarning($"Bridges not equals: {_bridge} != {value}");
			} else _bridge = null;
			return null;
		}

		public FFController TrySetBridge(IFFBridge value) {
			if (_bridge != null && _bridge.Equals(null)) _bridge = null;
			if (_bridge == value) return null;
			
			if (value == null) {
				Debug.LogError($"Use SetBridgeNull from bridge: ({_bridge})");
				return null;
			}
			if (_bridge != null) {
				Debug.LogError($"FFController ({this.name}) is already in use by another bridge: {_bridge}");
				return value.ffController;
			}
				
			if (value.ffController) {
				value.ffController.SetBridgeNull(value);	
			}
			_bridge = value;
			return this;
		}
		
		[SerializeField][HideInInspector]
		private Texture _sourceTexture;
		public Texture sourceTexture => _sourceTexture;
		private Texture _sourceTextureBuffer;

		private RenderTexture _renderInTexture;
		protected RenderTexture renderInTexture => _renderInTexture;

		private List<FFComponent> _components = new List<FFComponent>();
		public List<FFComponent> ffComponents => _components.ToList();
		
		private bool _isModified = false;
		private uint _rtUpdateCount = 0;
		private bool _isActiveAndEnabled = false;
		
		public bool CheckModified() {
			return _isModified = _isModified || CheckModifiedComps() || CheckSourceRenderTexture();
		}
		
		private bool CheckSourceRenderTexture() {
			return sourceTexture is RenderTexture sourceRt && (_rtUpdateCount != sourceRt.updateCount);
		}
		
		private bool CheckModifiedComps() {
			if (!renderInTexture) return false;
			foreach (var component in _components) {
				if (component.hasModifiedProps) return true;
			}
			return false;
		}
		
		private void UpdateEnabled() {
			if (_isActiveAndEnabled != isActiveAndEnabled) {
				_isActiveAndEnabled = !_isActiveAndEnabled;
				_isModified = true;
			}
		}

		public void SetSourceTexture(Texture texture, IFFBridge bridge = null) {
			if (_bridge != bridge) {
				Debug.LogWarning($"Bridges not equals: {_bridge} != {bridge}");
			}
			if (_sourceTexture != texture) {
				_sourceTexture = texture;
				UpdateSourceTexture();
			}
		}

		private void UpdateSourceTexture() {
			if (_sourceTextureBuffer != _sourceTexture) {
				_sourceTextureBuffer = _sourceTexture;
				RenderTextureUtils.SetSize(ref _renderInTexture, _sourceTexture);
				foreach (var component in _components) {
					component.SetSourceTexture(_sourceTexture);
				}
				_isModified = true;
			}
		}
		
		public void UpdateComponents() {
			var components = GetComponents<FFComponent>();
			
			_components.Clear();
			foreach (var component in components) {
				if (component && !component.isDestroyed) {
					_components.Add(component);
					component.SetSourceTexture(_sourceTexture);
				}
			}
			_isModified = true;
		}
		
		private void ResetModified() {
			_isModified = false;
			if (sourceTexture is RenderTexture sourceRt) {
				_rtUpdateCount = sourceRt.updateCount;
			}
		}
		
		public RenderTexture RenderIn() {
			ResetModified();
			if(!_renderInTexture) return null;
			
			var t = _sourceTexture;
			if (_isActiveAndEnabled) {
				foreach (var component in _components) {
					t = component.GetRender(t);
				}
			}

			_renderInTexture.DiscardContents();
			Graphics.Blit(t, _renderInTexture);
			_renderInTexture.IncrementUpdateCount();
			return _renderInTexture;
		}

		public void RenderOut(RenderTexture rt) {
			ResetModified();
			if(!_sourceTexture) return;
			
			var t = _sourceTexture;
			if (_isActiveAndEnabled) {
				foreach (var component in _components) {
					t = component.GetRender(t);
				}
			}

			if (rt != t) {
				rt.DiscardContents();
				Graphics.Blit(t, rt);
			}
		}
		
		void OnEnable() {
			UpdateEnabled();
			UpdateSourceTexture();
		}

		void OnDisable() {
			UpdateEnabled();
		}

		void OnDestroy() {
			if (_renderInTexture != null) {
				DestroyImmediate(_renderInTexture);
			}

			if (_bridge != null) {
				_bridge.ffController = null;
			}
		}

	#if UNITY_EDITOR
		void OnValidate() {
			UpdateSourceTexture();
		}
	#endif
	}
}