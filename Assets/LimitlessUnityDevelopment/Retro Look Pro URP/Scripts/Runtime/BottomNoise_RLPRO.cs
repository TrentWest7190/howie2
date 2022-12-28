using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

public class BottomNoise_RLPRO : ScriptableRendererFeature
{
	BottomNoise_RLPROPass RetroPass;
	public RenderPassEvent Event = RenderPassEvent.BeforeRenderingPostProcessing;


	public override void Create()
	{
		RetroPass = new BottomNoise_RLPROPass(Event);
	}

	public override void AddRenderPasses(ScriptableRenderer renderer, ref RenderingData renderingData)
	{
		RetroPass.Setup(renderer.cameraColorTarget);
		renderer.EnqueuePass(RetroPass);
	}
	public class BottomNoise_RLPROPass : ScriptableRenderPass
	{
		static readonly string k_RenderTag = "Renderr Glitch1 Effect";
		static readonly int MainTexId = Shader.PropertyToID("_InputTexture");
		static readonly int _OffsetNoiseYV = Shader.PropertyToID("_OffsetNoiseY");

		static readonly int _OffsetNoiseXV = Shader.PropertyToID("_OffsetNoiseX");
		static readonly int _NoiseBottomHeightV = Shader.PropertyToID("_NoiseBottomHeight");
		static readonly int _NoiseBottomIntensityV = Shader.PropertyToID("_NoiseBottomIntensity");
		static readonly int _NoiseTextureV = Shader.PropertyToID("_NoiseTexture");
		static readonly int tileXV = Shader.PropertyToID("tileX");
		static readonly int tileYV = Shader.PropertyToID("tileY");
		static readonly int TempTargetId = Shader.PropertyToID("Glitch1rr");

		BottomNoise retroEffect;
		Material RetroEffectMaterial;
		RenderTargetIdentifier currentTarget;


		public BottomNoise_RLPROPass(RenderPassEvent evt)
		{
			renderPassEvent = evt;
			var shader = Shader.Find("Hidden/Shader/BottomNoiseEffect_RLPRO");
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
			retroEffect = stack.GetComponent<BottomNoise>();
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

			if (RetroEffectMaterial.HasProperty(_OffsetNoiseYV))
			{
				float offsetNoise1 = RetroEffectMaterial.GetFloat(_OffsetNoiseYV);
				RetroEffectMaterial.SetFloat(_OffsetNoiseYV, offsetNoise1 + UnityEngine.Random.Range(-0.05f, 0.05f));
			}
			RetroEffectMaterial.SetFloat(_OffsetNoiseXV, UnityEngine.Random.Range(0f, 1.0f));

			RetroEffectMaterial.SetFloat(_NoiseBottomHeightV, retroEffect.height.value);

			RetroEffectMaterial.SetFloat(_NoiseBottomIntensityV, retroEffect.intencity.value);
			if (retroEffect.noiseTexture.value != null)
			{
				RetroEffectMaterial.SetTexture(_NoiseTextureV, retroEffect.noiseTexture.value);
			}
			RetroEffectMaterial.SetFloat(tileXV, retroEffect.tile.value.x);
			RetroEffectMaterial.SetFloat(tileYV, retroEffect.tile.value.y);

			cmd.SetGlobalTexture(MainTexId, source);

			cmd.GetTemporaryRT(destination, Screen.width, Screen.height, 0, FilterMode.Point, RenderTextureFormat.Default);


			cmd.Blit(source, destination);
			cmd.Blit(destination, source, RetroEffectMaterial, shaderPass);
		}

	}

}


