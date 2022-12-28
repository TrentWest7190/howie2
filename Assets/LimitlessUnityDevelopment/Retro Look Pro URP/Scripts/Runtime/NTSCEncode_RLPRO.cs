using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

public class NTSCEncode_RLPRO : ScriptableRendererFeature
{
	NTSCEncode_RLPROPass RetroPass;
	public RenderPassEvent Event = RenderPassEvent.BeforeRenderingPostProcessing;

	public override void Create()
	{
		RetroPass = new NTSCEncode_RLPROPass(Event);
	}

	public override void AddRenderPasses(ScriptableRenderer renderer, ref RenderingData renderingData)
	{
		RetroPass.Setup(renderer.cameraColorTarget);
		renderer.EnqueuePass(RetroPass);
	}
	public class NTSCEncode_RLPROPass : ScriptableRenderPass
	{
		static readonly string k_RenderTag = "Renderr Glitch1 Effect";
		static readonly int MainTexId = Shader.PropertyToID("_InputTexture");
		static readonly int TV = Shader.PropertyToID("T");
		static readonly int BsizeV = Shader.PropertyToID("Bsize");
		static readonly int val1V = Shader.PropertyToID("val1");
		static readonly int val2V = Shader.PropertyToID("val2");
		static readonly int TempTargetId = Shader.PropertyToID("Glitch1rr");

		NTSCEncode retroEffect;
		Material RetroEffectMaterial;
		RenderTargetIdentifier currentTarget;
		private float T;
		public NTSCEncode_RLPROPass(RenderPassEvent evt)
		{
			renderPassEvent = evt;
			var shader = Shader.Find("Hidden/Shader/NTSCEncode_RLPRO");
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
			retroEffect = stack.GetComponent<NTSCEncode>();
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
			RetroEffectMaterial.SetFloat(TV, T);
			RetroEffectMaterial.SetFloat(BsizeV, retroEffect.brigtness.value);
			RetroEffectMaterial.SetFloat(val1V, retroEffect.lineSpeed.value);
			RetroEffectMaterial.SetFloat(val2V, retroEffect.blur.value);

			cmd.SetGlobalTexture(MainTexId, source);

			cmd.GetTemporaryRT(destination, Screen.width, Screen.height, 0, FilterMode.Point, RenderTextureFormat.Default);

			cmd.Blit(source, destination);
			cmd.Blit(destination, source, RetroEffectMaterial, shaderPass);
		}
	}

}


