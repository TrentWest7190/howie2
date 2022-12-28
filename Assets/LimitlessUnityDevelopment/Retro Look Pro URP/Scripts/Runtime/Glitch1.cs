using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

public class Glitch1 : ScriptableRendererFeature
{
    Glitch1Pass GlitchPass;
    public RenderPassEvent Event = RenderPassEvent.AfterRenderingOpaques;


    public override void Create()
    {
        GlitchPass = new Glitch1Pass(Event);
    }

    public override void AddRenderPasses(ScriptableRenderer renderer, ref RenderingData renderingData)
    {
        GlitchPass.Setup(renderer.cameraColorTarget);
        renderer.EnqueuePass(GlitchPass);
    }
    public class Glitch1Pass : ScriptableRenderPass
    {
        static readonly string k_RenderTag = "Render Glitch1 Effect";
        static readonly int MainTexId = Shader.PropertyToID("_MainTex");
        static readonly int _Mask = Shader.PropertyToID("_Mask");

        static readonly int Strength = Shader.PropertyToID("Strength");
        static readonly int x = Shader.PropertyToID("x");
        static readonly int y = Shader.PropertyToID("y");
        static readonly int angleY = Shader.PropertyToID("angleY");
        static readonly int Stretch = Shader.PropertyToID("Stretch");
        static readonly int Speed = Shader.PropertyToID("Speed");
        static readonly int mR = Shader.PropertyToID("mR");
        static readonly int mG = Shader.PropertyToID("mG");
        static readonly int mB = Shader.PropertyToID("mB");
        static readonly int Fade = Shader.PropertyToID("Fade");

        static readonly int TempTargetId = Shader.PropertyToID("Glitch1");

        LimitlessGlitch1 glitch1;
        Material Glitch1Material;
        RenderTargetIdentifier currentTarget;
        private float T;

        public Glitch1Pass(RenderPassEvent evt)
        {
            renderPassEvent = evt;
            var shader = Shader.Find("LimitlessGlitch/Glitch1");
            if (shader == null)
            {
                Debug.LogError("Shader not found.");
                return;
            }
            Glitch1Material = CoreUtils.CreateEngineMaterial(shader);

        }

        public override void Execute(ScriptableRenderContext context, ref RenderingData renderingData)
        {
            if (Glitch1Material == null)
            {
                Debug.LogError("Material not created.");
                return;
            }
            if (!renderingData.cameraData.postProcessEnabled) return;

            var stack = VolumeManager.instance.stack;
            glitch1 = stack.GetComponent<LimitlessGlitch1>();
            if (glitch1 == null) { return; }
            if (!glitch1.IsActive()) { return; }

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
            
            Glitch1Material.SetFloat(Strength, glitch1.amount.value);

            Glitch1Material.SetFloat(x, glitch1.x.value);
            Glitch1Material.SetFloat(y, glitch1.y.value);
            Glitch1Material.SetFloat(angleY, glitch1.z.value);
            Glitch1Material.SetFloat(Stretch, glitch1.stretch.value);
            Glitch1Material.SetFloat(Speed, glitch1.speed.value);

            Glitch1Material.SetFloat(mR, glitch1.rMultiplier.value);
            Glitch1Material.SetFloat(mG, glitch1.gMultiplier.value);
            Glitch1Material.SetFloat(mB, glitch1.bMultiplier.value);
            Glitch1Material.SetFloat(Fade, glitch1.fade.value);
            Glitch1Material.SetFloat("T", T);

            cmd.SetGlobalTexture(MainTexId, source);

            cmd.GetTemporaryRT(destination, Screen.width, Screen.height, 0, FilterMode.Point, RenderTextureFormat.Default);


            cmd.Blit(source, destination);
            cmd.Blit(destination, source, Glitch1Material, shaderPass);
        }
    }

}


