﻿using C3DE.Components.Rendering;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

namespace C3DE.Graphics.Materials.Shaders
{
    public class DeferredStandardWater : ForwardStandardWater
    {
        public DeferredStandardWater(StandardWaterMaterial material) : base(material)
        {
        }

        public override void LoadEffect(ContentManager content)
        {
            m_Effect = content.Load<Effect>("Shaders/Deferred/StandardWater");
            SetupParamaters();
            m_PassAmbient = m_Effect.CurrentTechnique.Passes[0];
        }

        public override void Pass(Renderer renderable)
        {
            m_EPSpecularLightColor.SetValue(m_Material.SpecularColor.ToVector3());
            m_EPSpecularPower.SetValue(m_Material.Shininess);
            m_EPSpecularIntensity.SetValue(m_Material.SpecularIntensity);
            m_EPSpecularTextureEnabled.SetValue(m_Material.SpecularTexture != null);
            m_EPSpecularTexture.SetValue(m_Material.SpecularTexture);
            base.Pass(renderable);
        }
    }
}