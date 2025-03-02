using StbImageSharp;
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
              -0.5f, 0.5f, 0f, //top left vertex - 0
              0.5f, 0.5f, 0f, //top right vertex - 1
              0.5f, -0.5f, 0f, //bottom right vertex - 2
              -0.5f, -0.5f, 0f //bottom left vertex - 3
        };

        private float[] _textureCoords =
        {
            0f, 1f,
            1f, 1f,
            1f, 0f,
            0f, 0f
        };

        private uint[] _indices =
        {
            //top triangle
            0, 1 ,2,
            //bottom triangle
            2, 3, 0
                 
        };


        //Render pipeline vars
        int _vao;
        int _vbo;
        int _textureVbo;
        int _shaderProgram;
        int _ebo;
        int _textureId;


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

            //Bind VAO
            GL.BindVertexArray(_vao);

            // VERTICES VBO

            _vbo = GL.GenBuffer();
            GL.BindBuffer(BufferTarget.ArrayBuffer, _vbo);
            GL.BufferData(BufferTarget.ArrayBuffer, _vertices.Length*sizeof(float), _vertices, BufferUsageHint.StaticDraw);

            //Put the vertex vbo in slot 0 of our vao
            // Pointer slot 0 of the VAO  to the currently bound VBO
            GL.VertexAttribPointer(0, 3, VertexAttribPointerType.Float, false, 0, 0);
            GL.EnableVertexArrayAttrib(_vao, 0);
            GL.BindBuffer(BufferTarget.ArrayBuffer, 0); //unbinding vbo
           

            // TEXTURE VBO
            _textureVbo = GL.GenBuffer();
            GL.BindBuffer(BufferTarget.ArrayBuffer, _textureVbo);
            GL.BufferData(BufferTarget.ArrayBuffer, _textureCoords.Length*sizeof(float), _textureCoords, BufferUsageHint.StaticDraw);

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
            GL.ClearColor(Color4.CornflowerBlue);
            GL.Clear(ClearBufferMask.ColorBufferBit);

            //Draw triangle
            GL.UseProgram(_shaderProgram);

            GL.BindTexture(TextureTarget.Texture2D, _textureId);

            GL.BindVertexArray(_vao);
            GL.BindBuffer(BufferTarget.ElementArrayBuffer, _ebo);
            GL.DrawElements(PrimitiveType.Triangles, _indices.Length, DrawElementsType.UnsignedInt, 0);

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
