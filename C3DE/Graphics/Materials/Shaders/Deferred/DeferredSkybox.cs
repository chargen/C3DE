﻿using C3DE.Components;
using C3DE.Components.Rendering;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

namespace C3DE.Graphics.Materials.Shaders
{
    public class DeferredSkybox : ShaderMaterial
    {
        private Skybox m_Skybox;
        protected EffectParameter m_EPWorld;
        protected EffectParameter m_EPView;
        protected EffectParameter m_EPProjection;
        protected EffectParameter m_EPMainTexture;
        protected EffectParameter m_EPEyePosition;
        protected EffectParameter m_EPFogEnabled;
        protected EffectParameter m_EPFogColor;
        protected EffectParameter m_EPFogData;

        public DeferredSkybox(Skybox skybox)
        {
            m_Skybox = skybox;
        }

        public override void LoadEffect(ContentManager content)
        {
            m_Effect = content.Load<Effect>("Shaders/Deferred/Skybox");
            //m_DefaultPass = m_Effect.CurrentTechnique.Passes["AmbientPass"];
            m_EPView = m_Effect.Parameters["View"];
            m_EPProjection = m_Effect.Parameters["Projection"];
            m_EPMainTexture = m_Effect.Parameters["Texture"];
            m_EPEyePosition = m_Effect.Parameters["EyePosition"];
            m_EPWorld = m_Effect.Parameters["World"];
        }

        public override void Pass(Renderer renderable)
        {
        }

        public override void PrePass(Camera camera)
        {
            m_EPWorld.SetValue(m_Skybox.WorldMatrix);
            m_EPProjection.SetValue(camera.m_ProjectionMatrix);
            m_EPView.SetValue(camera.m_ViewMatrix);
            m_EPMainTexture.SetValue(m_Skybox.Texture);
            m_EPEyePosition.SetValue(camera.Transform.Position);
            m_Effect.CurrentTechnique.Passes[0].Apply();
        }
    }
}
