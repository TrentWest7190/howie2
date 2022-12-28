using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

public class VHSScanlines_RLPRO : ScriptableRendererFeature
{
	VHSScanlines_RLPROPass RetroPass;
	public RenderPassEvent Event = RenderPassEvent.BeforeRenderingPostProcessing;


	public override void Create()
	{
		RetroPass = new VHSScanlines_RLPROPass(Event);
	}

	public override void AddRenderPasses(ScriptableRenderer renderer, ref RenderingData renderingData)
	{
		RetroPass.Setup(renderer.cameraColorTarget);
		renderer.EnqueuePass(RetroPass);
	}
	public class VHSScanlines_RLPROPass : ScriptableRenderPass
	{
		static readonly string k_RenderTag = "Renderr Glitch1 Effect";
		static readonly int MainTexId = Shader.PropertyToID("_InputTexture");
		static readonly int TimeV = Shader.PropertyToID("Time");

		static readonly int _ScanLinesV = Shader.PropertyToID("_ScanLines");
		static readonly int speedV = Shader.PropertyToID("speed");
		static readonly int fadeV = Shader.PropertyToID("fade");
		static readonly int _OffsetDistortionV = Shader.PropertyToID("_OffsetDistortion");
		static readonly int sfericalV = Shader.PropertyToID("sferical");
		static readonly int barrelV = Shader.PropertyToID("barrel");
		static readonly int scaleV = Shader.PropertyToID("scale");
		static readonly int _ScanLinesColorV = Shader.PropertyToID("_ScanLinesColor");
		static readonly int TempTargetId = Shader.PropertyToID("Glitch1rr");
		private float T;
		VHSScanlines retroEffect;
		Material RetroEffectMaterial;
		RenderTargetIdentifier currentTarget;

		public VHSScanlines_RLPROPass(RenderPassEvent evt)
		{
			renderPassEvent = evt;
			var shader = Shader.Find("Hidden/Shader/VHSScanlinesEffect_RLPRO");
			if (shader == null)
			{
				Debug.LogError("Shader not found.");
				return;
			}
			RetroEffectMaterial = CoreUtils.CreateEngineMaterial(shader);
		}

		public override void Execute(ScriptableRenderContext context, ref RenderingData renderingData)
		{
			if (RetroEffectMaterial == null)
			{
				Debug.LogError("Material not created.");
				return;
			}
			if (!renderingData.cameraData.postProcessEnabled) return;

			var stack = VolumeManager.instance.stack;
			retroEffect = stack.GetComponent<VHSScanlines>();
			if (retroEffect == null) { return; }
			if (!retroEffect.IsActive()) { return; }

			var cmd = CommandBufferPool.Get(k_RenderTag);
			Render(cmd, ref renderingData);
			context.ExecuteCommandBuffer(cmd);
			CommandBufferPool.Release(cmd);
		}

		public void Setup(in RenderTargetIdentifier currentTarget)
		{
			this.currentTarget = currentTarget;
		}

		void Render(CommandBuffer cmd, ref RenderingData renderingData)
		{
			ref var cameraData = ref renderingData.cameraData;
			var source = currentTarget;
			int destination = TempTargetId;
			int pass;
			cmd.SetGlobalTexture(MainTexId, source);

			cmd.GetTemporaryRT(destination, Screen.width, Screen.height, 0, FilterMode.Point, RenderTextureFormat.Default);


			T += Time.deltaTime;

			RetroEffectMaterial.SetFloat(TimeV, T);
			RetroEffectMaterial.SetFloat(_ScanLinesV, retroEffect.scanLines.value);
			RetroEffectMaterial.SetFloat(speedV, retroEffect.speed.value);
			RetroEffectMaterial.SetFloat(_OffsetDistortionV, retroEffect.distortion.value);
			RetroEffectMaterial.SetFloat(fadeV, retroEffect.fade.value);
			RetroEffectMaterial.SetFloat(sfericalV, retroEffect.distortion1.value);
			RetroEffectMaterial.SetFloat(barrelV, retroEffect.distortion2.value);
			RetroEffectMaterial.SetFloat(scaleV, retroEffect.scale.value);
			RetroEffectMaterial.SetColor(_ScanLinesColorV, retroEffect.scanLinesColor.value);
			if (retroEffect.horizontal.value)
			{
				if (retroEffect.distortion.value != 0)
					pass = 1;
				else
					pass = 0;
			}
			else
			{
				if (retroEffect.distortion.value != 0)
					pass = 3;
				else
					pass = 2;
			}

			cmd.Blit(destination, source, RetroEffectMaterial, pass);
		}
	}

}


