using OpenTK.Graphics.OpenGL4;
using OpenTK.Mathematics;
using OpenTK.Windowing.Common;
using OpenTK.Windowing.Desktop;
using OpenTK.Windowing.GraphicsLibraryFramework;
using MinesraftRenderer.Graphics;
using MinesraftRenderer.World;


namespace MinesraftRenderer
{
    internal class Game : GameWindow
    {
        Chunk chunk;
        ShaderProgram program;

        //CAMERA
        private Camera? _camera;

        //Transformation vars
        private float yRot = 0f;


        private int _width, _height;
        public Game(int width, int height) : base(GameWindowSettings.Default, NativeWindowSettings.Default)
        {
            _width = width;
            _height = height;

            CenterWindow(new(width, height));
        }

        protected override void OnResize(ResizeEventArgs e)
        {
            base.OnResize(e);

            GL.Viewport(0, 0, FramebufferSize.X, FramebufferSize.Y);
            _width = e.Width;
            _height = e.Height;
        }

        protected override void OnLoad()
        {
            base.OnLoad();

            chunk = new(Vector3.Zero);

            program = new("Default.vert", "Default.frag");
            GL.Enable(EnableCap.DepthTest);
            _camera = new Camera(_width, _height, Vector3.Zero);
            CursorState = CursorState.Grabbed;
        }

        protected override void OnUnload()
        {
            base.OnUnload();

            chunk.Delete();
        }

        protected override void OnRenderFrame(FrameEventArgs args)
        {
            if (_camera == null) throw new Exception("Camera is not initialized");

            GL.ClearColor(Color4.CornflowerBlue);
            GL.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit);

            //transformation matrices
            Matrix4 model = Matrix4.Identity;
            Matrix4 view = _camera.GetViewMatrix();
            Matrix4 projection = _camera.GetProjectionMatrix();

            GL.UniformMatrix4(program.ModelLocation, true, ref model);
            GL.UniformMatrix4(program.ViewLocation, true, ref view);
            GL.UniformMatrix4(program.ProjectionLocation, true, ref projection);

            chunk.Render(program);

            Context.SwapBuffers();

            base.OnRenderFrame(args);
        }

        protected override void OnUpdateFrame(FrameEventArgs args)
        {
            if (_camera == null) throw new Exception("Camera is not initialized");

            MouseState mouse = MouseState;
            KeyboardState keyboard = KeyboardState;

            base.OnUpdateFrame(args);

            _camera.Update(keyboard, mouse, args);

            if (keyboard.IsKeyDown(Keys.Escape))
            {
                Environment.Exit(0);
            }
        }


    }
}
