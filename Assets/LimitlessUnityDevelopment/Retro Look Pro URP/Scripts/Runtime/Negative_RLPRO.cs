using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

public class Negative_RLPRO : ScriptableRendererFeature
{
	Negative_RLPROPass RetroPass;
	public RenderPassEvent Event = RenderPassEvent.BeforeRenderingPostProcessing;


	public override void Create()
	{
		RetroPass = new Negative_RLPROPass(Event);
	}

	public override void AddRenderPasses(ScriptableRenderer renderer, ref RenderingData renderingData)
	{
		RetroPass.Setup(renderer.cameraColorTarget);
		renderer.EnqueuePass(RetroPass);
	}
	public class Negative_RLPROPass : ScriptableRenderPass
	{
		static readonly string k_RenderTag = "Renderr Glitch1 Effect";
		static readonly int MainTexId = Shader.PropertyToID("_InputTexture");
		static readonly int TV = Shader.PropertyToID("T");
		static readonly int LuminosityV = Shader.PropertyToID("Luminosity");
		static readonly int ContrastV = Shader.PropertyToID("Contrast");
		static readonly int VignetteV = Shader.PropertyToID("Vignette");
		static readonly int NegativeV = Shader.PropertyToID("Negative");
		static readonly int TempTargetId = Shader.PropertyToID("Glitch1rr");

		Negative retroEffect;
		Material RetroEffectMaterial;
		RenderTargetIdentifier currentTarget;
		float T;

		public Negative_RLPROPass(RenderPassEvent evt)
		{
			renderPassEvent = evt;
			var shader = Shader.Find("Hidden/Shader/NegativeEffect_RLPRO");
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
			retroEffect = stack.GetComponent<Negative>();
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

			int shaderPass = 0;
			T += Time.deltaTime;
			if (T > 100) T = 0;

			RetroEffectMaterial.SetFloat(TV, T);
			RetroEffectMaterial.SetFloat(LuminosityV, 2 - retroEffect.luminosity.value);
			RetroEffectMaterial.SetFloat(ContrastV, 1 - retroEffect.contrast.value);
			RetroEffectMaterial.SetFloat(VignetteV, 1 - retroEffect.vignette.value);
			RetroEffectMaterial.SetFloat(NegativeV, retroEffect.negative.value);

			cmd.SetGlobalTexture(MainTexId, source);

			cmd.GetTemporaryRT(destination, Screen.width, Screen.height, 0, FilterMode.Point, RenderTextureFormat.Default);


			cmd.Blit(source, destination);
			cmd.Blit(destination, source, RetroEffectMaterial, shaderPass);
		}
	}
}


