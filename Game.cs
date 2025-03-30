using StbImageSharp;
using OpenTK.Graphics.OpenGL4;
using OpenTK.Mathematics;
using OpenTK.Windowing.Common;
using OpenTK.Windowing.Desktop;
using OpenTK.Windowing.GraphicsLibraryFramework;
using System.Xml;
using MinesraftRenderer.Graphics;


namespace MinesraftRenderer
{
    internal class Game : GameWindow
    {


        private readonly List<Vector3> _vertices =
        [
            //front face
            new Vector3(-0.5f, 0.5f, 0.5f), // top left vertex
            new Vector3(0.5f, 0.5f, 0.5f), // top right vertex
            new Vector3(0.5f, -0.5f, 0.5f), //bottom right vertex
            new Vector3(-0.5f, -0.5f, 0.5f), // bottom left vertex
            //right face
            new Vector3(0.5f, 0.5f, 0.5f),
            new Vector3(0.5f, 0.5f, -0.5f),
            new Vector3(0.5f, -0.5f, -0.5f),
            new Vector3(0.5f, -0.5f, 0.5f),
            //back face
            new Vector3(0.5f, 0.5f, -0.5f),
            new Vector3(-0.5f, 0.5f, -0.5f),
            new Vector3(-0.5f, -0.5f, -0.5f),
            new Vector3(0.5f, -0.5f, -0.5f),
            //left face
            new Vector3(-0.5f, 0.5f, -0.5f),
            new Vector3(-0.5f, 0.5f, 0.5f),
            new Vector3(-0.5f, -0.5f, 0.5f),
            new Vector3(-0.5f, -0.5f, -0.5f),
            //top face
            new Vector3(-0.5f, 0.5f, -0.5f),
            new Vector3(0.5f, 0.5f, -0.5f),
            new Vector3(0.5f, 0.5f, 0.5f),
            new Vector3(-0.5f, 0.5f, 0.5f),
            //bottom face
            new Vector3(-0.5f, -0.5f, 0.5f),
            new Vector3(0.5f, -0.5f, 0.5f),
            new Vector3(0.5f, -0.5f, -0.5f),
            new Vector3(-0.5f, -0.5f, -0.5f),

        ];

        private readonly List<Vector2> _texCoords =
        [
            //front face
            new Vector2(0f, 1f),
            new Vector2(1f, 1f),
            new Vector2(1f, 0f),
            new Vector2(0f, 0f),
            //right face
            new Vector2(0f, 1f),
            new Vector2(1f, 1f),
            new Vector2(1f, 0f),
            new Vector2(0f, 0f),
            //back face
            new Vector2(0f, 1f),
            new Vector2(1f, 1f),
            new Vector2(1f, 0f),
            new Vector2(0f, 0f),
            //left face
            new Vector2(0f, 1f),
            new Vector2(1f, 1f),
            new Vector2(1f, 0f),
            new Vector2(0f, 0f),
            //top face
            new Vector2(0f, 1f),
            new Vector2(1f, 1f),
            new Vector2(1f, 0f),
            new Vector2(0f, 0f),
            //bottom face
            new Vector2(0f, 1f),
            new Vector2(1f, 1f),
            new Vector2(1f, 0f),
            new Vector2(0f, 0f),
        ];

        private readonly List<uint> _indices =
        [
            //top triangle
            0, 1 ,2,
            //bottom triangle
            2, 3, 0,

            4, 5, 6,
            6, 7, 4,

            8, 9, 10,
            10, 11, 8,

            12, 13, 14,
            14, 15, 12,

            16, 17, 18,
            18, 19, 16,

            20, 21, 22,
            22, 23, 20

        ];

        private VAO _vao;
        private IBO _ibo;
        private ShaderProgram _program;
        private Texture _texture;

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

            GL.Viewport(0, 0, e.Width, e.Height);
            _width = e.Width;
            _height = e.Height;
        }

        protected override void OnLoad()
        {
            base.OnLoad();

            _vao = new VAO();
            VBO vbo = new(_vertices);
            _vao.LinkToVAO(0, 3, vbo);
            VBO textCoordVbo = new(_texCoords);
            _vao.LinkToVAO(1, 2, textCoordVbo);

            _ibo = new(_indices);

            _program = new("Default.vert", "Default.frag");

            _texture = new("dirt.png");

            GL.Enable(EnableCap.DepthTest);

            _camera = new Camera(_width, _height, Vector3.Zero);
            CursorState = CursorState.Grabbed;
        }

        protected override void OnUnload()
        {
            base.OnUnload();

            _program.Delete();
            _vao.Delete();
            _ibo.Delete();
            _texture.Delete();

        }

        protected override void OnRenderFrame(FrameEventArgs args)
        {
            if (_camera == null) throw new Exception("Camera is not initialized");

            GL.ClearColor(Color4.CornflowerBlue);
            GL.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit);

            _program.Bind();
            _vao.Bind();
            _ibo.Bind();
            _texture.Bind();

            //transformation matrices
            Matrix4 model = Matrix4.Identity;
            Matrix4 view = _camera.GetViewMatrix();
            Matrix4 projection = _camera.GetProjectionMatrix();

            model = Matrix4.CreateRotationY(yRot);
            yRot += 0.001f;
            Matrix4 translation = Matrix4.CreateTranslation(0f, 0f, -3f);
            model *= translation;


            GL.UniformMatrix4(_program.ModelLocation, true, ref model);
            GL.UniformMatrix4(_program.ViewLocation, true, ref view);
            GL.UniformMatrix4(_program.ProjectionLocation, true, ref projection);


            GL.DrawElements(PrimitiveType.Triangles, _indices.Count, DrawElementsType.UnsignedInt, 0);

            model *= Matrix4.CreateTranslation(new(2f, 0f, 0f));
            GL.UniformMatrix4(_program.ModelLocation, true, ref model);
            GL.DrawElements(PrimitiveType.Triangles, _indices.Count, DrawElementsType.UnsignedInt, 0);

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
