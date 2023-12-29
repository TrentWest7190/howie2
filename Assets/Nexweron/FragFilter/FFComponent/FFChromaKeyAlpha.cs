using Nexweron.Common.Attributes;
using UnityEngine;

namespace Nexweron.FragFilter
{
	[AddComponentMenu("Nexweron/FragFilter/FF ChromaKey Alpha")]
	public class FFChromaKeyAlpha : FFComponent
	{
		private readonly int _colorSpaceID = Shader.PropertyToID("_ColorSpace");
		private readonly string[] _colorSpaceKeywords = {"_COLORSPACE_YCBCR", "_COLORSPACE_YIQ"};
		private readonly int _keyColorID = Shader.PropertyToID("_KeyColor");
		private readonly int _dChromaID = Shader.PropertyToID("_DChroma");
		private readonly int _dChromaTID = Shader.PropertyToID("_DChromaT");
		
		private readonly int _lumaModeID = Shader.PropertyToID("_LumaMode");
		private readonly string[] _lumaModeKeywords = {"_LUMAMODE_AUTO", "_LUMAMODE_MANUAL"};
		private readonly int _dLumaID = Shader.PropertyToID("_DLuma");
		private readonly int _dLumaTID = Shader.PropertyToID("_DLumaT");
		
		public enum ColorSpace { [InspectorName("YCbCr")]YCbCr, YIQ }
		[SerializeField] ColorSpace m_colorSpace = ColorSpace.YCbCr;
		private ColorSpace _colorSpace = ColorSpace.YCbCr;
		public ColorSpace colorSpace {
			get => _colorSpace;
			set => SetKeywordEnumProp(_colorSpaceID, ref _colorSpace, ref m_colorSpace, value, _colorSpaceKeywords);
		}
		public int colorSpaceInt {
			get => (int)colorSpace;
			set => colorSpace = (ColorSpace)value;
		}
		
		[SerializeField] Color m_keyColor = Color.green;
		private Color _keyColor = Color.green;
		public Color keyColor {
			get => _keyColor;
			set => SetColorProp(_keyColorID, ref _keyColor, ref m_keyColor, value);
		}
		
		[Range(0, 1)]
		[SerializeField] float m_dChroma = 0.5f;
		private float _dChroma = 0.5f;
		public float dChroma {
			get => _dChroma;
			set => SetFloatProp(_dChromaID, ref _dChroma, ref m_dChroma, Mathf.Clamp(value, 0, 1));
		}
		
		[Range(0, 1)]
		[SerializeField] float m_dChromaT = 0.05f;
		private float _dChromaT = 0.05f;
		public float dChromaT {
			get => _dChromaT;
			set => SetFloatProp(_dChromaTID, ref _dChromaT, ref m_dChromaT, Mathf.Clamp(value, 0, 1));
		}
		
		public enum LumaMode { Auto, Manual }
		[SerializeField] LumaMode m_lumaMode = LumaMode.Auto;
		private LumaMode _lumaMode = LumaMode.Auto;
		public LumaMode lumaMode {
			get => _lumaMode;
			set => SetKeywordEnumProp(_lumaModeID, ref _lumaMode, ref m_lumaMode, value, _lumaModeKeywords);
		}

		public int lumaModeInt {
			get => (int)lumaMode;
			set => lumaMode = (LumaMode)value;
		}
		
		[Range(0, 1)][InspectorShowIf("m_lumaMode", LumaMode.Manual)]
		[SerializeField] float m_dLuma = 0.5f;
		private float _dLuma = 0.5f;
		public float dLuma {
			get => _dLuma;
			set => SetFloatProp(_dLumaID, ref _dLuma, ref m_dLuma, Mathf.Clamp(value, 0, 1));
		}
		
		[Range(0, 1)][InspectorShowIf("m_lumaMode", LumaMode.Manual)]
		[SerializeField] float m_dLumaT = 0.05f;
		private float _dLumaT = 0.05f;
		public float dLumaT {
			get => _dLumaT;
			set => SetFloatProp(_dLumaID, ref _dLumaT, ref m_dLumaT, Mathf.Clamp(value, 0, 1));
		}
		
		protected override Shader  GetInternalShader() {
			return Shader.Find(@"Nexweron/Builtin/ChromaKey/UnlitBlendOff_ChromaKey_Alpha");
		}
		
		protected override void UpdateMaterial() {
			SetKeywordEnum(_colorSpaceID, (int)m_colorSpace, _colorSpaceKeywords[0], _colorSpaceKeywords[(int)m_colorSpace]);
			SetColor(_keyColorID, m_keyColor);
			SetFloat(_dChromaID, m_dChroma);
			SetFloat(_dChromaTID, m_dChromaT);
			SetKeywordEnum(_lumaModeID, (int)m_lumaMode, _lumaModeKeywords[0], _lumaModeKeywords[(int)m_lumaMode]);
			SetFloat(_dLumaID, m_dLuma);
			SetFloat(_dLumaTID, m_dLumaT);
		}

		protected override void UpdateSerialized() {
			base.UpdateSerialized();
			colorSpace = m_colorSpace;
			keyColor = m_keyColor;
			dChroma = m_dChroma;
			dChromaT = m_dChromaT;
			lumaMode = m_lumaMode;
			dLuma = m_dLuma;
			dLumaT = m_dLumaT;
		}
	}
}
