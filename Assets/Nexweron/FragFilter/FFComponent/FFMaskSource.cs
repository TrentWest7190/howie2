using Nexweron.Common.Attributes;
using UnityEngine;

namespace Nexweron.FragFilter
{
	[AddComponentMenu("Nexweron/FragFilter/FF Mask Source")]
	public class FFMaskSource : FFComponent
	{
		private readonly int _maskTexID = Shader.PropertyToID("_MaskTex");
		private readonly int _edgeModeID = Shader.PropertyToID("_EdgeMode");
		private readonly string[] _edgeModeKeywords = {"_EDGEMODE_DEFAULT", "_EDGEMODE_MANUAL"};
		private readonly int _alphaPowID = Shader.PropertyToID("_AlphaPow");
		private readonly int _alphaEdgeID = Shader.PropertyToID("_AlphaEdge");
		
		public enum EdgeMode { Default, Manual }
		[SerializeField] EdgeMode m_edgeMode = EdgeMode.Default;
		private EdgeMode _edgeMode = EdgeMode.Default;
		public EdgeMode edgeMode {
			get => _edgeMode;
			set => SetKeywordEnumProp(_edgeModeID, ref _edgeMode, ref m_edgeMode, value, _edgeModeKeywords);
		}
		public int edgeModeInt {
			get => (int)edgeMode;
			set => edgeMode = (EdgeMode)value;
		}
		
		[Range(1, 10)][InspectorShowIf("m_edgeMode", EdgeMode.Manual)]
		[SerializeField] float m_alphaPow = 1.0f;
		private float _alphaPow = 1.0f;
		public float alphaPow {
			get => _alphaPow;
			set => SetFloatProp(_alphaPowID, ref _alphaPow, ref m_alphaPow, Mathf.Clamp(value, 1, 10));
		}

		[Range(-0.5f, 0.5f)][InspectorShowIf("m_edgeMode", EdgeMode.Manual)]
		[SerializeField] float m_alphaEdge = 0.0f;
		private float _alphaEdge = 0.0f;
		public float alphaEdge {
			get => _alphaEdge;
			set => SetFloatProp(_alphaEdgeID, ref _alphaEdge, ref m_alphaEdge, Mathf.Clamp(value, -0.5f, 0.5f));
		}
		
		public override Texture GetRender(Texture textureIn) {
			_hasModifiedProps = false;
			if (_rt != null && isActiveAndEnabled) {
				internalMaterial.SetTexture(_maskTexID, textureIn);
				_rt.DiscardContents();
				Graphics.Blit(_sourceTexture, _rt, internalMaterial);
				return _rt;	
			}
			return textureIn;
		}
		
		protected override Shader  GetInternalShader() {
			return Shader.Find(@"Nexweron/Builtin/Mask/UnlitBlendOff_MaskAlpha");
		}
		
		protected override void UpdateMaterial() {
			SetKeywordEnum(_edgeModeID, (int)m_edgeMode, _edgeModeKeywords[0], _edgeModeKeywords[(int)m_edgeMode]);
			SetFloat(_alphaPowID, m_alphaPow);
			SetFloat(_alphaEdgeID, m_alphaEdge);
		}

		protected override void UpdateSerialized() {
			base.UpdateSerialized();
			edgeMode = m_edgeMode;
			alphaPow = m_alphaPow;
			alphaEdge = m_alphaEdge;
		}
	}
}
