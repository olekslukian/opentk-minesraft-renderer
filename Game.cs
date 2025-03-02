using OpenTK.Graphics.OpenGL4;
using OpenTK.Mathematics;
using OpenTK.Windowing.Common;
using OpenTK.Windowing.Desktop;
using OpenTK.Windowing.GraphicsLibraryFramework;


namespace MinesraftRenderer
{
    internal class Game : GameWindow
    {

        private readonly float[] _vertices =
        {
               0f, 0.5f, 0f, //top vertex
               -0.5f, -0.5f, 0f, //bottom left
               0.5f , -0.5f, 0f //bottom right
        };

        //Render pipeline vars
        int _vao;
        int _shaderProgram;


        private int _width, _height;
        public Game(int width, int height) : base(GameWindowSettings.Default, NativeWindowSettings.Default)
        {  
            _width = width;
            _height = height;
     
            this.CenterWindow(new(width, height));
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
            int vbo = GL.GenBuffer();
            GL.BindBuffer(BufferTarget.ArrayBuffer, vbo);
            GL.BufferData(BufferTarget.ArrayBuffer, _vertices.Length*sizeof(float), _vertices, BufferUsageHint.StaticDraw);

            //Bind VAO
            GL.BindVertexArray(_vao);
            GL.VertexAttribPointer(0, 3, VertexAttribPointerType.Float, false, 0, 0);
            GL.EnableVertexArrayAttrib(_vao, 0);

            GL.BindBuffer(BufferTarget.ArrayBuffer, 0); //unbinding vbo
            GL.BindVertexArray(0);

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
        }

        protected override void OnUnload()
        {
            base.OnUnload();

            GL.DeleteVertexArray(_vao);
            GL.DeleteProgram(_shaderProgram);
        }

        protected override void OnRenderFrame(FrameEventArgs args)
        {
            GL.ClearColor(Color4.CornflowerBlue);
            GL.Clear(ClearBufferMask.ColorBufferBit);

            //Draw triangle
            GL.UseProgram(_shaderProgram);
            GL.BindVertexArray(_vao);
            GL.DrawArrays(PrimitiveType.Triangles, 0, 3);

            Context.SwapBuffers();

            base.OnRenderFrame(args);
        }

        protected override void OnUpdateFrame(FrameEventArgs args)
        {
            base.OnUpdateFrame(args);   
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
