using UnityEngine;
using Nexweron.Common.Utils;

namespace Nexweron.FragFilter
{
	[RequireComponent(typeof(FFController))]
	public abstract class FFComponent : MaterialComponent
	{
		private bool _isDestroyed = false;
		public bool isDestroyed => _isDestroyed;
	
		protected RenderTexture _rt;
		protected Texture _sourceTexture;
		private FFController _controller;
		
		protected override void Awake() {
			base.Awake();
			_controller = GetComponent<FFController>();
			_controller.UpdateComponents();
		}
		
		protected virtual void OnEnable() {
			SetModified();
		}

		public void SetSourceTexture(Texture value) {
			_sourceTexture = value;
			UpdateRT();
			SetModified();
		}
		
		protected virtual void UpdateRT() {
			RenderTextureUtils.SetSize(ref _rt, _sourceTexture);
		}
		
		public virtual Texture GetRender(Texture textureIn) {
			_hasModifiedProps = false;
			if (_rt != null && isActiveAndEnabled) {
				_rt.DiscardContents();
				Graphics.Blit(textureIn, _rt, internalMaterial);
				return _rt;	
			}
			return textureIn;
		}
		
		protected virtual void OnDisable() {
			SetModified();
		}
		
		protected override void OnDestroy() {
			if (_rt != null) {
				_rt.DiscardContents();
				DestroyImmediate(_rt);
			}
			base.OnDestroy();
			_isDestroyed = true;
			_controller.UpdateComponents();
		}
	}
}
