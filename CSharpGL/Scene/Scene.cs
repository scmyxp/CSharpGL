﻿using System;
using System.ComponentModel;
using System.Drawing;
using System.Drawing.Design;
using System.Windows.Forms;

namespace CSharpGL
{
    /// <summary>
    /// Manages a scene to be rendered and updated.
    /// </summary>
    [Editor(typeof(PropertyGridEditor), typeof(UITypeEditor))]
    public partial class Scene : IDisposable
    {
        //private SceneRootUI rootCursor = new SceneRootUI();
        private SceneRootUI rootUI;

        private SceneRootObject rootObject;

        private SceneRootViewPort rootViewPort;

        private Size canvasLastSize;

        /// <summary>
        /// Manages a scene to be rendered and updated.
        /// </summary>
        /// <param name="camera">Camera of the scene</param>
        /// <param name="canvas">Canvas that this scene binds to.</param>
        /// <param name="objects">Objects to be rendered</param>
        public Scene(ICamera camera, ICanvas canvas, params SceneObject[] objects)
        {
            if (camera == null || canvas == null) { throw new ArgumentNullException(); }

            {
                this.Canvas = canvas;
                this.canvasLastSize = canvas.Size;
            }
            {
                this.rootUI = new SceneRootUI();
            }
            {
                var rootObject = new SceneRootObject(this);
                rootObject.Children.AddRange(objects);
                this.rootObject = rootObject;
            }
            {
                var rootViewPort = new SceneRootViewPort(
                     AnchorStyles.Left | AnchorStyles.Right | AnchorStyles.Bottom | AnchorStyles.Top, new Padding(0, 0, 0, 0), canvas.Size);
                rootViewPort.Children.Add(new ViewPort(camera,
                    AnchorStyles.Left | AnchorStyles.Right | AnchorStyles.Top | AnchorStyles.Bottom,
                    new Padding(0, 0, 0, 0), canvas.Size));
                this.rootViewPort = rootViewPort;
            }
            //var cursor = UICursor.CreateDefault();
            //cursor.Enabled = false;
            //this.rootCursor.Children.Add(cursor);
            //this.Cursor = cursor;
        }

        /// <summary>
        /// Please bind this method to Control.Resize event.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        public void Resize(object sender, EventArgs e)
        {
            var control = sender as ICanvas;
            if (control == null) { throw new ArgumentException(); }

            Size currentSize = control.Size;
            if (currentSize.Width > 0 && currentSize.Height > 0)
            {
                this.FirstCamera.Resize(this.canvasLastSize, currentSize);
                this.canvasLastSize = currentSize;
                this.rootViewPort.Size = currentSize;
                this.rootUI.Size = currentSize;
                //this.rootCursor.Size = currentSize;
            }
        }

        /// <summary>
        ///
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            return string.Format("{0}", this.Running ? "Scripts running" : "Scripts not running");
        }
    }
}