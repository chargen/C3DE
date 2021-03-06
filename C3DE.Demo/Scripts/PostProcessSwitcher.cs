﻿using C3DE.Components;
using C3DE.Graphics.PostProcessing;
using C3DE.UI;
using C3DE.Utils;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System.Collections.Generic;

namespace C3DE.Demo.Scripts
{
    public class PostProcessSwitcher : Behaviour
    {
        struct Widget
        {
            public Rectangle Rect { get; set; }
            public string Name { get; set; }
            public Rectangle RectExt { get; set; }
        }

        private Rectangle _boxRect;
        private Widget[] _widgets;
        private Texture2D _backgroundTexture;
        private List<PostProcessPass> _passes;

        public PostProcessSwitcher()
            : base()
        {
            _passes = new List<PostProcessPass>();
        }

        public override void Start()
        {
            var graphics = Application.GraphicsDevice;

            // Setup PostProcess.
#if !DESKTOP
            var ssao = new ScreenSpaceAmbientObscurance(graphics);
            AddPostProcess(ssao);

           // var ssgi = new ScreenSpaceGlobalIllumination(graphics);
            //AddPostProcess(ssgi);
#endif
            var fastBloom = new FastBloom(graphics);
            AddPostProcess(fastBloom);

            AddPostProcess(new C64Filter(graphics));
            AddPostProcess(new CGAFilter(graphics));
            AddPostProcess(new ConvolutionFilter(graphics));
            AddPostProcess(new FilmFilter(graphics));
            AddPostProcess(new GrayScaleFilter(graphics));
            AddPostProcess(new AverageColorFilter(graphics));
            AddPostProcess(new MotionBlur(graphics));

            var refractionPass = new Refraction(graphics);
            AddPostProcess(refractionPass);
            refractionPass.RefractionTexture = Application.Content.Load<Texture2D>("Textures/hexagrid");
            refractionPass.TextureTiling = new Vector2(0.5f);

            var vignette = new Vignette(graphics);
            AddPostProcess(vignette);

            AddPostProcess(new GlobalFog(graphics));

            AddPostProcess(new FXAA(graphics));

            // Setup UI
            var titles = new List<string>();

#if !DESKTOP
            titles.AddRange(new string[] {
                "Ambient Obscurance",
               // "SSGI"
            });
#endif

            titles.AddRange(new string[]
            {
                "Bloom", "C64 Filter",
                "CGA Filter", "Convolution", "Film",
                "GrayScale", "Average Color", "Motion Blur",
                "Refraction", "Vignette", "Global Fog", "FXAA"
            });

            var count = titles.Count;
            _boxRect = new Rectangle(Screen.VirtualWidth - 220, 10, 210, 45 * count);
            _widgets = new Widget[count];

            for (int i = 0; i < count; i++)
            {
                _widgets[i] = new Widget();
                _widgets[i].Name = titles[i];

                if (i == 0)
                    _widgets[i].Rect = new Rectangle(_boxRect.X + 10, _boxRect.Y + 30, _boxRect.Width - 20, 30);
                else
                    _widgets[i].Rect = new Rectangle(_boxRect.X + 10, _widgets[i - 1].Rect.Y + 40, _boxRect.Width - 20, 30);

                _widgets[i].RectExt = new Rectangle(_widgets[i].Rect.X - 1, _widgets[i].Rect.Y - 1, _widgets[i].Rect.Width + 1, _widgets[i].Rect.Height + 1);
            }

            _backgroundTexture = GraphicsHelper.CreateTexture(Color.CornflowerBlue, 1, 1);

            var renderSettings = Scene.current.RenderSettings;
            renderSettings.FogDensity = 0.0085f;
            renderSettings.FogMode = FogMode.Linear;
            renderSettings.Skybox.OverrideSkyboxFog(FogMode.Exp2, 0.05f, 0, 0);
        }

        public override void Update()
        {
            if (Input.Keys.JustPressed(Keys.U) || Input.Gamepad.JustPressed(Buttons.Start) || Input.Touch.JustPressed(4))
                GUI.Enabled = !GUI.Enabled;
        }

        public override void OnGUI(GUI ui)
        {
            ui.Box(ref _boxRect, "Effects");

            for (int i = 0, l = _widgets.Length; i < l; i++)
            {
                if (_passes[i].Enabled)
                    ui.DrawTexture(_widgets[i].RectExt, _backgroundTexture);

                if (ui.Button(_widgets[i].Rect, _widgets[i].Name))
                    SetPassActive(i);
            }
        }

        private void AddPostProcess(PostProcessPass pass)
        {
            pass.Enabled = false;
            m_GameObject.Scene.Add(pass);
            _passes.Add(pass);
        }

        private void SetPassActive(int index)
        {
            _passes[index].Enabled = !_passes[index].Enabled;
        }
    }
}
