using System;
using System.Linq;
using Nexweron.Common.Utils;
using UnityEngine;

namespace Nexweron.FragFilter
{
	[AddComponentMenu("Nexweron/FragFilter/FF Blur")]
	public class FFBlur : FFComponent
	{
		private readonly int _blurMatrixID = Shader.PropertyToID("_BlurMatrix");
		private readonly string[] _blurMatrixKeywords = {"_BLURMATRIX_GAUSS", "_BLURMATRIX_BOX"};
		private readonly int _blurOffsetID = Shader.PropertyToID("_BlurOffset");
		
		public enum BlurMatrix { Gauss, Box }
		[SerializeField] BlurMatrix m_blurMatrix = BlurMatrix.Gauss;
		private BlurMatrix _blurMatrix = BlurMatrix.Gauss;
		public BlurMatrix blurMatrix {
			get => _blurMatrix;
			set => SetKeywordEnumProp(_blurMatrixID, ref _blurMatrix, ref m_blurMatrix, value, _blurMatrixKeywords);
		}
		public int blurMatrixInt {
			get => (int)blurMatrix;
			set => blurMatrix = (BlurMatrix)value;
		}
		
		[SerializeField][HideInInspector]
		private BlurData _blurData = new BlurData();
		
		[Range(0, 100)]
		[SerializeField] int m_blurOffset = 1;
		private int _blurOffset = 1;
		public float blurOffset {
			get => _blurOffset;
			set {
				var valueInt = (int)Mathf.Clamp(value, 0, 100);
				if (_blurOffset != valueInt) {
					_blurOffset = m_blurOffset = valueInt;
					UpdateBlurOffset();
					SetModified();
				}
			}
		}
		private void UpdateBlurOffset() {
			_blurData.UpdateBlurValue(_blurOffset);
			UpdateRTSize();
		}
		private void UpdateRTSize() {
			var rtSize = _blurData.GetLastSize();
			RenderTextureUtils.SetSize(ref _rt, rtSize.x, rtSize.y);
		}
		
		protected override void UpdateRT() {
			var sourceSize = _sourceTexture ?
				new Vector2(_sourceTexture.width, _sourceTexture.height) : 
				24*Vector2Int.one;
			_blurData.UpdateSourceSize(sourceSize);
			UpdateRTSize();
		}
		
		private void BlitBlur(Texture rtS, RenderTexture rt, float offset, int passX = 1, int passY = 2) {
			internalMaterial.SetFloat(_blurOffsetID, offset);
			var rtT = RenderTexture.GetTemporary(rtS.width, rtS.height);
			Graphics.Blit(rtS, rtT, internalMaterial, 0);
			rtS = rtT;
			Graphics.Blit(rtS, rt, internalMaterial, 1);
			RenderTexture.ReleaseTemporary(rtT);
		}
		
		public override Texture GetRender(Texture textureIn) {
			_hasModifiedProps = false;
			if (_blurOffset == 0 || !isActiveAndEnabled) return textureIn;
			
			RenderTexture rt = null;
			var rtS = textureIn;
			var iterations = _blurData.iterations;
			
			foreach (var i in iterations) {
				if (i.num < iterations.Length - 1) {
					rt = i.GetTempRT();
				} else {
					_rt.DiscardContents();
					rt = _rt;
				}
				
				BlitBlur(rtS, rt, i.offset);
				if(i.num > 0) RenderTexture.ReleaseTemporary(rtS as RenderTexture);
				rtS = rt;
			}
			
			return rt;
		}
		
		protected override Shader  GetInternalShader() {
			return Shader.Find(@"Nexweron/Builtin/Blur/UnlitBlendOff_BlurPassXY");
		}
		
		protected override void UpdateMaterial() {
			SetKeywordEnum(_blurMatrixID, (int)m_blurMatrix, _blurMatrixKeywords[0], _blurMatrixKeywords[(int)m_blurMatrix]);
		}
		
		protected override void UpdateSerialized() {
			base.UpdateSerialized();
			blurMatrix = m_blurMatrix;
			blurOffset = m_blurOffset;
		}

		[Serializable]
		private class BlurData
		{
			[Serializable]
			public struct BlurIteration
			{
				public int num;
				public float offset;
				public Vector2Int size;

				public BlurIteration(int num, float offset, Vector2 size) {
					this.num = num;
					this.offset = offset;
					this.size = Vector2Int.FloorToInt(size);
				}

				public RenderTexture GetTempRT() {
					return RenderTexture.GetTemporary(size.x, size.y, 0);
				}
			}
			
			[SerializeField]
			private Vector2 _sourceSize = 16*Vector2.one;
			
			[SerializeField]
			private int _blurValue = -1;
			
			public BlurIteration[] iterations;

			public Vector2Int GetLastSize() {
				if (iterations != null && iterations.Any()) {
					return iterations.Last().size;
				}
				return 16*Vector2Int.one;
			}
			
			public void UpdateSourceSize(Vector2 sourceSize) {
				if(_sourceSize != sourceSize)
					Update(sourceSize, _blurValue);
			}
			
			public void UpdateBlurValue(int blurValue) {
				if(_blurValue != blurValue)
					Update(_sourceSize, blurValue);
			}
			
			private void Update(Vector2 sourceSize, int blurValue) {
				_sourceSize = sourceSize;
				_blurValue = blurValue;
				Update();
			}

			private void Update() {
				if (_blurValue > 64) {
					var blurStep = (_blurValue - 64) / 64f;
					iterations = new[] {
						new BlurIteration(0, 3.0f * (2 + blurStep) /1, _sourceSize /4),
						new BlurIteration(1, 3.0f * (2 + blurStep) /4, _sourceSize /6),
						new BlurIteration(2, 3.0f * (2 + blurStep) /6, _sourceSize /6),
						new BlurIteration(3, 2.0f * (3 + blurStep) /6, _sourceSize /4)
					};
				} else
				
				if (_blurValue > 32) {
					var blurStep = (_blurValue - 32) / 32f;
					iterations = new[] {
						new BlurIteration(0, 2.0f * (2 + blurStep) /1, _sourceSize /3),
						new BlurIteration(1, 2.0f * (2 + blurStep) /3, _sourceSize /4),
						new BlurIteration(2, 2.0f * (2 + blurStep) /4, _sourceSize /4),
						new BlurIteration(3, 2.0f * (2 + blurStep) /4, _sourceSize /3)
					};
				} else
				
				if (_blurValue > 16) {
					var blurStep = (_blurValue - 16) / 16f;
					iterations = new[] {
						new BlurIteration(0, 1.0f * (2 + blurStep) /1, _sourceSize /3),
						new BlurIteration(1, 1.5f * (2 + blurStep) /3, _sourceSize /3),
						new BlurIteration(2, 2.0f * (2 + blurStep) /3, _sourceSize /2)
					};
				} else
				
				if (_blurValue > 8) {
					var blurStep = (_blurValue - 8) / 8f;
					iterations = new[] {
						new BlurIteration(0, 1.0f * (2 + blurStep) /1, _sourceSize /2),
						new BlurIteration(1, 1.5f * (1 + blurStep) /2, _sourceSize /2),
						new BlurIteration(2, 1.5f * (1 + blurStep) /2, _sourceSize /2)
					};
				} else
				
				if (_blurValue > 4) {
					var blurStep = (_blurValue - 4) / 4f;
					iterations = new[] {
						new BlurIteration(0, 2.0f, _sourceSize /2),
						new BlurIteration(1, (1 + blurStep) /2, _sourceSize /2)
					};
				} else
				
				if (_blurValue > 2) {
					var blurStep = (_blurValue - 2) / 2f;
					iterations = new[] {
						new BlurIteration(0, 1.0f, _sourceSize /2),
						new BlurIteration(1, (blurStep) /2, _sourceSize)
					};
				} else
				
				if (_blurValue > 1) {
					iterations = new[] {
						new BlurIteration(0, 1.0f, _sourceSize),
						new BlurIteration(1, 1.0f, _sourceSize)
					};
				} else
				
				if (_blurValue > 0) {
					iterations = new [] {
						new BlurIteration(0, 1.0f, _sourceSize)
					};
				} else {
					iterations = Array.Empty<BlurIteration>();
				}
			}
		}	
	}
}







