﻿using System;
using System.Drawing;
using System.Text;

using System.Windows.Forms;

namespace CSharpGL.Demos
{
    public partial class Form28HemisphereLighting : Form
    {
        private HemisphereLightingRenderer renderer;
        private Scene scene;

        private void Form_Load(object sender, EventArgs e)
        {
            {
                var camera = new Camera(
                   new vec3(5, 3, 4), new vec3(0, 0, 0), new vec3(0, 1, 0),
                   CameraType.Perspecitive, this.glCanvas1.Width, this.glCanvas1.Height);
                var cameraManipulater = new SatelliteManipulater();
                cameraManipulater.Bind(camera, this.glCanvas1);
                this.cameraManipulater = cameraManipulater;
                this.scene = new Scene(camera, this.glCanvas1);
                this.scene.RootViewPort.ClearColor = Color.SkyBlue;
                this.glCanvas1.Resize += this.scene.Resize;
            }
            {
                HemisphereLightingRenderer renderer = HemisphereLightingRenderer.Create();
                SceneObject obj = renderer.WrapToSceneObject(true, new ModelScript(this.glCanvas1, this.scene.FirstCamera));
                this.scene.RootObject.Children.Add(obj);
                this.renderer = renderer;

                var frmPropertyGrid = new FormProperyGrid(renderer);
                frmPropertyGrid.Show();
            }
            {
                var uiAxis = new UIAxis(AnchorStyles.Left | AnchorStyles.Bottom,
                    new Padding(3, 3, 3, 3), new Size(128, 128));
                this.scene.RootUI.Children.Add(uiAxis);
            }
            {
                var builder = new StringBuilder();
                builder.AppendLine("1: Scenes' property grid.");
                builder.AppendLine("2: Canvas' property grid.");
                MessageBox.Show(builder.ToString());
            }
            {
                this.scene.Start();
            }
        }
    }
}