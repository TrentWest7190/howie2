using UnityEngine;

namespace Nexweron.Common.Utils
{
	public static class RenderTextureUtils
	{
		public static RenderTexture Create(int width, int height) {
			return new RenderTexture(width, height, 0);
		}

		public static RenderTexture Resize(RenderTexture rt, int width, int height) {
			SetSize(ref rt, width, height);
			return rt;
		}

		public static void SetSize(ref RenderTexture rt, Texture source) {
			if (source != null) {
				SetSize(ref rt, source.width, source.height);
			} else
			if (rt != null) {
				if (rt.IsCreated()) {
					Object.DestroyImmediate(rt);
				}
				rt = null;
			}
		}
		
		public static void SetSize(ref RenderTexture rt, int width, int height) {
			if (rt != null) {
				if (rt.width != width || rt.height != height) {
					if (rt.IsCreated()) {
						Object.DestroyImmediate(rt);
						rt = Create(width, height);
					} else {
						rt.width = width;
						rt.height = height;
					}
				}
			} else {
				rt = Create(width, height);
			}
		}
	}
}
