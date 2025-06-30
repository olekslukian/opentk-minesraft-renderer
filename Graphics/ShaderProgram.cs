using OpenTK.Graphics.OpenGL4;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MinesraftRenderer.Graphics
{
    class ShaderProgram : IRenderObject
    {
        private static readonly string SHADERS_DIR = "Shaders/";

        public int ID;
        public ShaderProgram(string vertShaderFileName, string fragShaderFileName)
        {
            ID = GL.CreateProgram();

            int vertexShader = GL.CreateShader(ShaderType.VertexShader);
            GL.ShaderSource(vertexShader, LoadShaderSource(vertShaderFileName));
            GL.CompileShader(vertexShader);

            int fragShader = GL.CreateShader(ShaderType.FragmentShader);
            GL.ShaderSource(fragShader, LoadShaderSource(fragShaderFileName));
            GL.CompileShader(fragShader);

            GL.AttachShader(ID, vertexShader);
            GL.AttachShader(ID, fragShader);

            GL.LinkProgram(ID);

            GL.DeleteShader(vertexShader);
            GL.DeleteShader(fragShader);
        }

        public void Bind()
        {
            GL.UseProgram(ID);
        }
        public void Unbind()
        {
            GL.UseProgram(0);
        }
        public void Delete()
        {
            GL.DeleteShader(ID);
        }

        private static string LoadShaderSource(string fileName)
        {
            string shaderSource = "";

            try
            {
                using (StreamReader rader = new(SHADERS_DIR + fileName))
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

        public int ModelLocation
        {
            get
            {
                return GL.GetUniformLocation(ID, "model");
            }

        }
        public int ViewLocation
        {
            get
            {
                return GL.GetUniformLocation(ID, "view");
            }
        }
        public int ProjectionLocation
        {
            get
            {
                return GL.GetUniformLocation(ID, "projection");
            }
        }
    }
}
