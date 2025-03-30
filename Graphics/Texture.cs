using OpenTK.Graphics.OpenGL4;
using StbImageSharp;

namespace MinesraftRenderer.Graphics
{
    class Texture : IRenderObject
    {
        private static readonly string TEXTURES_DIR = "../../../Textures/";

        public int ID;

        public Texture(string filename)
        {
            ID = GL.GenTexture();

            GL.ActiveTexture(TextureUnit.Texture0);
            GL.BindTexture(TextureTarget.Texture2D, ID);

            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureWrapS, (int)TextureWrapMode.Repeat);
            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureWrapT, (int)TextureWrapMode.Repeat);
            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMinFilter, (int)TextureMinFilter.Nearest);
            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMagFilter, (int)TextureMagFilter.Nearest);

            StbImage.stbi_set_flip_vertically_on_load(1);
            ImageResult image = ImageResult.FromStream(File.OpenRead(TEXTURES_DIR + filename), ColorComponents.RedGreenBlueAlpha);

            GL.TexImage2D(TextureTarget.Texture2D, 0, PixelInternalFormat.Rgba, image.Width, image.Height, 0, PixelFormat.Rgba, PixelType.UnsignedByte, image.Data);

            Unbind();
        }

        public void Bind()
        {
            GL.BindTexture(TextureTarget.Texture2D, ID);
        }
        public void Unbind()
        {
            GL.BindTexture(TextureTarget.Texture2D, ID);
        }
        public void Delete()
        {
            GL.DeleteTexture(ID);
        }
    }
}
