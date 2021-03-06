﻿using System.Drawing;
using System.IO;

namespace CSharpGL.Demos
{
    /// <summary>
    /// 正方形
    /// </summary>
    internal class FixedPercentRenderer : Renderer
    {
        public static FixedPercentRenderer Create(GroundModel model)
        {
            var shaderCodes = new ShaderCode[2];
            shaderCodes[0] = new ShaderCode(File.ReadAllText(@"shaders\FixedPercent.vert"), ShaderType.VertexShader);
            shaderCodes[1] = new ShaderCode(File.ReadAllText(@"shaders\FixedPercent.frag"), ShaderType.FragmentShader);
            var provider = new ShaderCodeArray(shaderCodes);
            var map = new AttributeMap();
            map.Add("in_Position", GroundModel.strPosition);
            var renderer = new FixedPercentRenderer(model, provider, map);
            return renderer;
        }

        public Color LineColor { get; set; }

        private FixedPercentRenderer(IBufferable model, IShaderProgramProvider shaderProgramProvider,
            AttributeMap attributeMap, params GLState[] switches)
            : base(model, shaderProgramProvider, attributeMap, switches)
        {
            this.LineColor = Color.White;
        }

        protected override void DoInitialize()
        {
            base.DoInitialize();
        }

        protected override void DoRender(RenderEventArgs arg)
        {
            mat4 projection = arg.Camera.GetProjectionMatrix();
            mat4 view = arg.Camera.GetViewMatrix();
            mat4 model = glm.scale(mat4.identity(), this.Scale);
            this.SetUniform("projectionMatrix", projection);
            this.SetUniform("viewMatrix", view);
            this.SetUniform("modelMatrix", model);
            this.SetUniform("lineColor", this.LineColor.ToVec3());
            base.DoRender(arg);
        }
    }
}