﻿using System;
using System.IO;

namespace CSharpGL.Demos
{
    internal class PointSpriteRenderer : Renderer
    {
        private Texture spriteTexture;
        private PointSpriteState pointSpriteState;

        private bool pointSpritEnabled = true;
        public bool PointSpriteEnabled
        {
            get
            {
                return this.pointSpritEnabled;
            }
            set
            {
                this.pointSpritEnabled = value;
                this.pointSpriteState.Enabled = value;
            }
        }

        public static PointSpriteRenderer Create(int particleCount)
        {
            var shaderCodes = new ShaderCode[2];
            shaderCodes[0] = new ShaderCode(File.ReadAllText(@"shaders\PointSprite.vert"), ShaderType.VertexShader);
            shaderCodes[1] = new ShaderCode(File.ReadAllText(@"shaders\PointSprite.frag"), ShaderType.FragmentShader);
            var provider = new ShaderCodeArray(shaderCodes);
            var map = new AttributeMap();
            map.Add("position", PointSpriteModel.strposition);
            var model = new PointSpriteModel(particleCount);
            var renderer = new PointSpriteRenderer(model, provider, map);
            renderer.ModelSize = model.Lengths;

            return renderer;
        }

        public PointSpriteRenderer(IBufferable model, IShaderProgramProvider shaderProgramProvider,
            AttributeMap attributeMap, params GLState[] switches)
            : base(model, shaderProgramProvider, attributeMap, switches)
        {
            this.pointSpriteState = new PointSpriteState();
            this.stateList.Add(this.pointSpriteState);
            this.PointSpriteEnabled = true;
        }

        protected override void DoInitialize()
        {
            {
                // This is the texture that the compute program will write into
                var bitmap = new System.Drawing.Bitmap(@"Textures\PointSprite.png");
                var texture = new Texture(TextureTarget.Texture2D, bitmap, new SamplerParameters());
                texture.Initialize();
                bitmap.Dispose();
                this.spriteTexture = texture;
            }
            base.DoInitialize();
            this.SetUniform("spriteTexture", this.spriteTexture);
            this.SetUniform("factor", 50.0f);
        }

        protected override void DoRender(RenderEventArgs arg)
        {
            mat4 model = mat4.identity();
            mat4 view = arg.Camera.GetViewMatrix();
            mat4 projection = arg.Camera.GetProjectionMatrix();
            this.SetUniform("mvp", projection * view * model);
            this.SetUniform("pointSpritEnabled", this.PointSpriteEnabled);

            base.DoRender(arg);
        }

        protected override void DisposeUnmanagedResources()
        {
            base.DisposeUnmanagedResources();

            this.spriteTexture.Dispose();
        }

        private class PointSpriteModel : IBufferable
        {
            public PointSpriteModel(int particleCount)
            {
                this.particleCount = particleCount;
            }

            public const string strposition = "position";
            private VertexBuffer positionBuffer = null;
            private IndexBuffer indexBuffer;
            private int particleCount;
            private Random random = new Random();
            private float factor = 1;

            public VertexBuffer GetVertexAttributeBuffer(string bufferName, string varNameInShader)
            {
                if (bufferName == strposition)
                {
                    if (this.positionBuffer == null)
                    {
                        var array = new vec3[particleCount];
                        for (int i = 0; i < particleCount; i++)
                        {
                            if (i % 2 == 0)
                            {
                                while (true)
                                {
                                    var x = (float)(random.NextDouble() * 2 - 1) * factor;
                                    var y = (float)(random.NextDouble() * 2 - 1) * factor;
                                    var z = (float)(random.NextDouble() * 2 - 1) * factor;
                                    if (y < 0 && x * x + y * y + z * z >= factor * factor)
                                    {
                                        array[i] = new vec3(x, y, z);
                                        break;
                                    }
                                }
                            }
                            else
                            {
                                double theta = random.NextDouble() * 2 * Math.PI - Math.PI;
                                double alpha = random.NextDouble() * 2 * Math.PI - Math.PI;
                                array[i] = new vec3(
                                    (float)(Math.Sin(theta) * Math.Cos(alpha)) * factor,
                                    (float)(Math.Sin(theta) * Math.Sin(alpha)) * factor,
                                    (float)(Math.Cos(theta)) * factor);
                            }
                        }
                        VertexBuffer buffer = array.GenVertexBuffer(VBOConfig.Vec3, varNameInShader, BufferUsage.StaticDraw);
                        this.positionBuffer = buffer;
                    }

                    return this.positionBuffer;
                }
                else
                {
                    throw new ArgumentException();
                }
            }

            public IndexBuffer GetIndexBuffer()
            {
                if (this.indexBuffer == null)
                {
                    ZeroIndexBuffer buffer = ZeroIndexBuffer.Create(DrawMode.Points, 0, particleCount);
                    this.indexBuffer = buffer;
                }

                return indexBuffer;
            }

            /// <summary>
            /// Uses <see cref="ZeroIndexBuffer"/> or <see cref="OneIndexBuffer"/>.
            /// </summary>
            /// <returns></returns>
            public bool UsesZeroIndexBuffer() { return true; }

            public vec3 Lengths { get { return new vec3(2, 2, 2); } }
        }

        internal void UpdateTexture(string filename)
        {
            // This is the texture that the compute program will write into
            var bitmap = new System.Drawing.Bitmap(filename);
            var texture = new Texture(TextureTarget.Texture2D, bitmap, new SamplerParameters());
            texture.Initialize();
            bitmap.Dispose();
            Texture old = this.spriteTexture;
            this.spriteTexture = texture;
            this.SetUniform("sprite_texture", texture);

            old.Dispose();
        }
    }
}