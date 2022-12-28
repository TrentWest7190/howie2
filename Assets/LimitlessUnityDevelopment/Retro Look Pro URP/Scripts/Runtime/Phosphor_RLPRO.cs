using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

public class Phosphor_RLPRO : ScriptableRendererFeature
{
	Phosphor_RLPROPass RetroPass;
	public RenderPassEvent Event = RenderPassEvent.BeforeRenderingPostProcessing;


	public override void Create()
	{
		RetroPass = new Phosphor_RLPROPass(Event);
	}

	public override void AddRenderPasses(ScriptableRenderer renderer, ref RenderingData renderingData)
	{
		RetroPass.Setup(renderer.cameraColorTarget);
		renderer.EnqueuePass(RetroPass);
	}
	public class Phosphor_RLPROPass : ScriptableRenderPass
	{
		static readonly string k_RenderTag = "Renderr Glitch1 Effect";
		static readonly int MainTexId = Shader.PropertyToID("_InputTexture");
		static readonly int TV = Shader.PropertyToID("T");
		static readonly int speedV = Shader.PropertyToID("speed");
		static readonly int amountV = Shader.PropertyToID("amount");
		static readonly int fadeV = Shader.PropertyToID("fade");
		static readonly int _TexV = Shader.PropertyToID("_Tex");
		static readonly int TempTargetId = Shader.PropertyToID("Glitch1rr");

		Phosphor retroEffect;
		Material RetroEffectMaterial;
		RenderTargetIdentifier currentTarget;
		private RenderTexture texTape = null;
		float T;

		public Phosphor_RLPROPass(RenderPassEvent evt)
		{
			renderPassEvent = evt;
			var shader = Shader.Find("Hidden/Shader/Phosphor_RLPRO");
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
			retroEffect = stack.GetComponent<Phosphor>();
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
			cmd.SetGlobalTexture(MainTexId, source);

			cmd.GetTemporaryRT(destination, Screen.width, Screen.height, 0, FilterMode.Point, RenderTextureFormat.Default);


			cmd.Blit(source, destination);

			if (texTape == null)
			{
				texTape = new RenderTexture(Screen.width, Screen.height, 1);
			}
			T = Time.time;
			RetroEffectMaterial.SetFloat(TV, T);
			RetroEffectMaterial.SetFloat(speedV, retroEffect.width.value);
			RetroEffectMaterial.SetFloat(amountV, retroEffect.amount.value + 1);
			RetroEffectMaterial.SetFloat(fadeV, retroEffect.fade.value);

			cmd.Blit(destination, texTape, RetroEffectMaterial, 1);

			RetroEffectMaterial.SetTexture(_TexV, texTape);
			cmd.Blit(destination, source, RetroEffectMaterial, 0);
		}

	}

}


