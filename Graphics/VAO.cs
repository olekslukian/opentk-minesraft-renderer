﻿using OpenTK.Graphics.OpenGL4;

namespace MinesraftRenderer.Graphics
{
    class VAO : IRenderObject
    {
        public int ID;
        public VAO()
        {
            ID = GL.GenVertexArray();
            GL.BindVertexArray(ID);
        }

        public void LinkToVAO(int location, int size, VBO vbo)
        {
            Bind();
            GL.VertexAttribPointer(location, size, VertexAttribPointerType.Float, false, 0, 0);
            GL.EnableVertexAttribArray(location);
            Unbind();
        }
        public void Bind()
        {
            GL.BindVertexArray(ID);
        }
        public void Unbind()
        {
            GL.BindVertexArray(0);
        }
        public void Delete()
        {
            GL.DeleteVertexArray(ID);
        }
    }
}
