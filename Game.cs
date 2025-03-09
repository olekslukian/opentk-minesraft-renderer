using StbImageSharp;
using OpenTK.Graphics.OpenGL4;
using OpenTK.Mathematics;
using OpenTK.Windowing.Common;
using OpenTK.Windowing.Desktop;
using OpenTK.Windowing.GraphicsLibraryFramework;
using System.Xml;


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

        private readonly uint[] _indices =
        {
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
                 
        };


        //Render pipeline vars
        private int _vao;
        private int _vbo;
        private int _textureVbo;
        private int _shaderProgram;
        private int _ebo;
        private int _textureId;

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

            _vao = GL.GenVertexArray();

            //Bind VAO
            GL.BindVertexArray(_vao);

            // VERTICES VBO

            _vbo = GL.GenBuffer();
            GL.BindBuffer(BufferTarget.ArrayBuffer, _vbo);
            GL.BufferData(BufferTarget.ArrayBuffer, _vertices.Count * Vector3.SizeInBytes, _vertices.ToArray(), BufferUsageHint.StaticDraw);

            //Put the vertex vbo in slot 0 of our vao
            // Pointer slot 0 of the VAO  to the currently bound VBO
            GL.VertexAttribPointer(0, 3, VertexAttribPointerType.Float, false, 0, 0);
            GL.EnableVertexArrayAttrib(_vao, 0);
            GL.BindBuffer(BufferTarget.ArrayBuffer, 0); //unbinding vbo
           

            // TEXTURE VBO
            _textureVbo = GL.GenBuffer();
            GL.BindBuffer(BufferTarget.ArrayBuffer, _textureVbo);
            GL.BufferData(BufferTarget.ArrayBuffer, _texCoords.Count * Vector3.SizeInBytes, _texCoords.ToArray(), BufferUsageHint.StaticDraw);

            //Pointer slot 1 of the VAO to the currently bount VBO
            GL.VertexAttribPointer(1, 2, VertexAttribPointerType.Float, false, 0, 0);
            GL.EnableVertexArrayAttrib(_vao, 1);
            GL.BindVertexArray(0);




            _ebo = GL.GenBuffer();
            GL.BindBuffer(BufferTarget.ElementArrayBuffer, _ebo);
            GL.BufferData(BufferTarget.ElementArrayBuffer, _indices.Length*sizeof(uint), _indices, BufferUsageHint.StaticDraw);
            GL.BindBuffer(BufferTarget.ElementArrayBuffer, 0);

            //Creating shader program
            _shaderProgram = GL.CreateProgram();
            int vertexShader = GL.CreateShader(ShaderType.VertexShader);
            GL.ShaderSource(vertexShader, LoadShaderSource("Default.vert"));
            GL.CompileShader(vertexShader);

            int fragShader = GL.CreateShader(ShaderType.FragmentShader);
            GL.ShaderSource(fragShader, LoadShaderSource("Default.frag"));
            GL.CompileShader(fragShader);

            GL.AttachShader(_shaderProgram, vertexShader);
            GL.AttachShader(_shaderProgram, fragShader);

            GL.LinkProgram(_shaderProgram);

            //Deleting shaders
            GL.DeleteShader(vertexShader);
            GL.DeleteShader(fragShader);

            // TEXTURES
            _textureId = GL.GenTexture();
            // activate the texture in the unit
            GL.ActiveTexture(TextureUnit.Texture0);
            GL.BindTexture(TextureTarget.Texture2D, _textureId);

            //Texture params
            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureWrapS, (int)TextureWrapMode.Repeat);
            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureWrapT, (int)TextureWrapMode.Repeat);
            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMinFilter, (int)TextureMinFilter.Nearest);
            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMagFilter, (int)TextureMagFilter.Nearest);

            //Load image
            StbImage.stbi_set_flip_vertically_on_load(1);
            ImageResult dirtTexture = ImageResult.FromStream(File.OpenRead("../../../Textures/dirt.png"), ColorComponents.RedGreenBlueAlpha);

            GL.TexImage2D(TextureTarget.Texture2D, 0, PixelInternalFormat.Rgba, dirtTexture.Width, dirtTexture.Height, 0, PixelFormat.Rgba, PixelType.UnsignedByte, dirtTexture.Data);
            //Unbind the texture
            GL.BindTexture(TextureTarget.Texture2D, 0);

            GL.Enable(EnableCap.DepthTest);

            _camera = new Camera(_width, _height, Vector3.Zero);
            CursorState = CursorState.Grabbed;
        }

        protected override void OnUnload()
        {
            base.OnUnload();

            GL.DeleteVertexArray(_vao);
            GL.DeleteBuffer(_vao);
            GL.DeleteBuffer(_ebo);
            GL.DeleteTexture(_textureId);
            GL.DeleteProgram(_shaderProgram);
        }

        protected override void OnRenderFrame(FrameEventArgs args)
        {
            if (_camera == null) throw new Exception("Camera is not initialized");

            GL.ClearColor(Color4.CornflowerBlue);
            GL.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit);

            //Draw triangle
            GL.UseProgram(_shaderProgram);
            GL.BindVertexArray(_vao);
            GL.BindBuffer(BufferTarget.ElementArrayBuffer, _ebo);

            GL.BindTexture(TextureTarget.Texture2D, _textureId);

            //transformation matrices
            Matrix4 model = Matrix4.Identity;
            Matrix4 view = _camera.GetViewMatrix();
            Matrix4 projection = Matrix4.CreatePerspectiveFieldOfView(MathHelper.DegreesToRadians(60.0f), _width / _height, 0.1f, 100.0f);

            model = Matrix4.CreateRotationY(yRot);
            yRot += 0.003f;
            model *= Matrix4.CreateTranslation(0f, 0f, -3f);

            int modelLocation = GL.GetUniformLocation(_shaderProgram, "model");
            int viewLocation = GL.GetUniformLocation(_shaderProgram, "view");
            int projectionlLocation = GL.GetUniformLocation(_shaderProgram, "projection");

            GL.UniformMatrix4(modelLocation, true, ref model);
            GL.UniformMatrix4(viewLocation, true, ref view);
            GL.UniformMatrix4(projectionlLocation, true, ref projection);


            GL.DrawElements(PrimitiveType.Triangles, _indices.Length, DrawElementsType.UnsignedInt, 0);

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

        public static string LoadShaderSource(string path)
        {
            string shaderSource = "";

            try
            {
                using(StreamReader rader =  new("../../../Shaders/" + path))
                {
                    shaderSource = rader.ReadToEnd();
                }
            }
            catch (Exception e) 
            {
                Console.WriteLine("Failed to load shader file: " + e.Message);
            }

            return shaderSource;
        }
    }
}
