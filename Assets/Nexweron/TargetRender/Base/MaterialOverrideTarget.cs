using System;
using UnityEngine;

namespace Nexweron.TargetRender
{
	[Serializable]
	public class MaterialOverrideTarget : SubTarget
	{
		[SerializeField] Renderer m_renderer;
		private Renderer _renderer;
		public Renderer renderer {
			get => _renderer;
			set {
				if (_renderer == value) return;
				RevertDefaultTexture();
				_renderer = m_renderer = value;
				_isModified = true;
			}
		}

		[SerializeField] int m_rendererMaterialIndex = -1;
		private int _rendererMaterialIndex = -1;
		public int rendererMaterialIndex {
			get => _rendererMaterialIndex;
			set {
				if (_rendererMaterialIndex == value) return;
				RevertDefaultTexture();
				_rendererMaterialIndex = m_rendererMaterialIndex = value;
				_isModified = true;
			}
		}
		
		[SerializeField] string m_rendererMaterialProp = null;
		private string _rendererMaterialProp = null;
		public string rendererMaterialProp {
			get { return _rendererMaterialProp; }
			set {
				if (_rendererMaterialProp == value) return;
				RevertDefaultTexture();
				_rendererMaterialProp = m_rendererMaterialProp = value;
				_propTexID = Shader.PropertyToID(value);
				_isModified = true;
			}
		}
		
		private Texture _mainTex;
		private readonly int _mainTexID = Shader.PropertyToID("_MainTex");
		private int _propTexID = int.MinValue;
		
		private MaterialPropertyBlock _materialPropertyBlock;
		private MaterialPropertyBlock materialPropertyBlock => _materialPropertyBlock = _materialPropertyBlock ?? new MaterialPropertyBlock();
		
		public override bool isValidTarget => ValidateTarget();
		
		private bool ValidateTarget() {
			if (_renderer == null || _propTexID <= 0) return false;
			if (0 <= _rendererMaterialIndex && _rendererMaterialIndex < _renderer.sharedMaterials.Length) {
				var material = _renderer.sharedMaterials[_rendererMaterialIndex];
				var isValidProp = material != null && material.HasProperty(_propTexID);
				if (!isValidProp) return false;
				if (_isCachedDefaultTexture) return true;
					
				_renderer.GetPropertyBlock(materialPropertyBlock, rendererMaterialIndex);
				if (!materialPropertyBlock.isEmpty) {
					Debug.LogError($"{_renderer} already use another MaterialPropertyBlock");
					return false;
				}
				return true;
			}
			return false;
		}
		
		protected override Texture GetDefaultTexture() {
			if (_propTexID == _mainTexID && _renderer is SpriteRenderer spriteRenderer) {
				return spriteRenderer.sprite.texture;
			}
			return _renderer.sharedMaterials[_rendererMaterialIndex].GetTexture(_propTexID);
		}
		
		protected override void CacheDefaultTexture() {
			var sourceTex = GetDefaultTexture();
			var useNullAsDefault = !sourceTex && !_isCachedDefaultTexture;
			
			if (_defaultTexture != sourceTex || useNullAsDefault) {
				_defaultTexture = sourceTex;
				_isCachedDefaultTexture = true;
			}
			// Check mpb changed
			if (_propTexID == _mainTexID) return;
			
			_renderer.GetPropertyBlock(materialPropertyBlock);
			var mainTex = materialPropertyBlock.GetTexture(_mainTexID);
			
			if (_mainTex != mainTex) {
				_mainTex = mainTex;
				_isModified = true;
			}
		}
		
		protected override void RevertDefaultTexture(bool clearCache = false) {
			if (_isCachedDefaultTexture && _renderer) {
				_renderer.SetPropertyBlock(null, _rendererMaterialIndex);
			}
			_isCachedDefaultTexture = false;
			_defaultTexture = null;
		}
		
		public override Texture GetTargetTexture() {
			_renderer.GetPropertyBlock(materialPropertyBlock, _rendererMaterialIndex);
			return materialPropertyBlock.GetTexture(_propTexID);
		}
		
		private void GetPropertyBlock(MaterialPropertyBlock mpb, int materialIndex = 0) {
			_renderer.GetPropertyBlock(mpb, materialIndex);
		}
		
		private void SetPropertyBlock(MaterialPropertyBlock mpb, int materialIndex = 0) {
			if (mpb.isEmpty) mpb = null;
			_renderer.SetPropertyBlock(mpb, materialIndex);
		}
		
		protected override void SetTargetTexture(Texture texture) {
			_renderer.GetPropertyBlock(materialPropertyBlock);
			materialPropertyBlock.SetTexture(_propTexID, texture);
			SetPropertyBlock(materialPropertyBlock, _rendererMaterialIndex);
		}
		
		public override void UpdateSerialized() {
			renderer = m_renderer;
			rendererMaterialIndex = m_rendererMaterialIndex;
			rendererMaterialProp = m_rendererMaterialProp;
		}
	}
}