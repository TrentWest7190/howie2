using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Nexweron.TargetRender
{
	[ExecuteAlways]
	public class TargetValueBase : MonoBehaviour, IMaterialModifier
	{
		// Target is apiOnly
		[SerializeField][HideInInspector]
		private SubTarget _apiOnlyTarget = new ApiOnlyTarget();

		// Target is RenderTexture
		[SerializeField][HideInInspector]
		private RenderTextureTarget _renderTextureTarget = new RenderTextureTarget();

		public RenderTexture targetRenderTexture {
			get => _renderTextureTarget.renderTexture;
			set => _renderTextureTarget.renderTexture = value;
		}

		// Target is MaterialOverride
		[SerializeField][HideInInspector]
		private MaterialOverrideTarget _materialOverrideTarget = new MaterialOverrideTarget();

		public Renderer targetRenderer {
			get => _materialOverrideTarget.renderer;
			set => _materialOverrideTarget.renderer = value;
		}

		public int targetRendererMaterialIndex {
			get => _materialOverrideTarget.rendererMaterialIndex;
			set => _materialOverrideTarget.rendererMaterialIndex = value;
		}

		public string targetRendererMaterialProp {
			get => _materialOverrideTarget.rendererMaterialProp;
			set => _materialOverrideTarget.rendererMaterialProp = value;
		}

		// Target is RawImage
		[SerializeField][HideInInspector]
		private RawImageTarget _rawImageTarget = new RawImageTarget();
		public RawImage targetRawImage {
			get => _rawImageTarget.rawImage;
			private set => _rawImageTarget.rawImage = value;
		}
		
		// Render Mode
		public enum RenderMode {
			APIOnly,
			RenderTexture,
			MaterialOverride,
			RawImage
		}

		private Dictionary<RenderMode, SubTarget> _subTargets;
		private Dictionary<RenderMode, SubTarget> subTargets {
			get {
				_subTargets = _subTargets ?? new Dictionary<RenderMode, SubTarget>() {
					{RenderMode.APIOnly, _apiOnlyTarget},
					{RenderMode.RenderTexture, _renderTextureTarget},
					{RenderMode.MaterialOverride, _materialOverrideTarget},
					{RenderMode.RawImage, _rawImageTarget}
				};
				return _subTargets;
			}
		}

		[SerializeField]
		private RenderMode m_renderMode;
		protected RenderMode _renderMode;
		public RenderMode renderMode {
			get => _renderMode;
			set {
				if (_renderMode == value) return;
				subTargets[_renderMode].SetSelected(false);
				_renderMode = m_renderMode = value;
				UpdateRenderMode();
			}
		}
		protected virtual void UpdateRenderMode() {
			UpdateGraphic();
			UpdateProvided();
		}
		
		protected virtual bool isProvided => isActiveAndEnabled;
		protected void UpdateProvided() {
			subTargets[_renderMode].SetSelected(isProvided);
		}
		
		// Init
		[SerializeField][HideInInspector]
		private bool _inited = false;
		private void CheckInit() {
			if (_inited) return;
			_inited = true;

			_rawImage = GetComponent<RawImage>();
			if (_rawImage != null) {
				targetRawImage = _rawImage;
				renderMode = RenderMode.RawImage;
				return;
			}
				
			var _renderer = GetComponent<Renderer>();
			if (_renderer) {
				targetRenderer = _renderer;
				renderMode = RenderMode.MaterialOverride;
				return;
			}
		}
		
		// Default texture
		protected Texture defaultTexture => subTargets[_renderMode].defaultTexture;
		protected bool ReCacheDefault() {
			// return isCached
			return subTargets[_renderMode].ReCacheDefaultTexture();
		}
		
		// Value texture
		private Texture _valueTexture;
		protected Texture valueTexture => _valueTexture;
		protected void SetValueTexture(Texture texture) {
			_valueTexture = texture;
			subTargets[_renderMode].SetValueTexture(texture);
		}
		protected void RenderValue() {
			subTargets[_renderMode].UpdateValueTexture();
		}

		// Graphic required
		private RawImage _rawImage;
		public void UpdateGraphic() {
			if (_renderMode != RenderMode.RawImage) return;
			if (!_rawImage) {
				targetRawImage = _rawImage = GetComponent<RawImage>();
			}
		}
		public Material GetModifiedMaterial(Material baseMaterial) {
			// on Graphic component added
			UpdateGraphic();
			return baseMaterial;
		}

		// MonoBehaviour
		protected virtual void Start() {
			UpdateSerialized();
			CheckInit();
		}

		protected virtual void OnEnable() {
			UpdateProvided();
		}

		protected virtual void OnDisable() {
			UpdateProvided();
		}

		protected virtual void OnDestroy() {
			SetValueTexture(null);
		}
		
		// Serialize
		protected virtual void UpdateSerialized() {
			foreach (var subTarget in subTargets.Values) {
				subTarget.UpdateSerialized();
			}
			renderMode = m_renderMode;
		}
		
	#if UNITY_EDITOR
		protected virtual void OnValidate() {
			UpdateSerialized();
			UpdateGraphic();
			UpdateProvided();
		}
	#endif
	}
}