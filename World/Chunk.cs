using MinesraftRenderer.Graphics;
using OpenTK.Mathematics;
using OpenTK.Graphics.OpenGL4;


namespace MinesraftRenderer.World
{
    internal class Chunk
    {
        public Vector3 position;

        const int SIZE = 16;
        const int HEIGHT = 32;

        private List<Vector3> _chunkVertices = [];
        private List<Vector2> _chunkUVs = [];
        private List<uint> _chunkIndices = [];
        private uint _indexCount;

        VAO chunkVAO; 
        VBO chunkVertexVBO;
        VBO chunkUVVBO;
        IBO chunkIBO;

        Texture texture;

        public Chunk(Vector3 position)
        {
            this.position = position;

            GenBlocks();
            BuildChunk();
        }

        public void GenChunk() { } // Generate the data
        public void GenBlocks() 
        {
            for(int i = 0; i < 3; i++)
            {
                Block block = new(new(i, 0, 0));

                int faceCount = 0;

                if (i == 0)
                {
                    var leftFaceData = block.GetFace(Faces.LEFT);
                    _chunkVertices.AddRange(leftFaceData.vertices);
                    _chunkUVs.AddRange(leftFaceData.uv);
                    faceCount++;
                }
                if (i == 2)
                {
                    var rightFaceData = block.GetFace(Faces.RIGHT);
                    _chunkVertices.AddRange(rightFaceData.vertices);
                    _chunkUVs.AddRange(rightFaceData.uv);
                    faceCount++;
                }

                var frontFaceData = block.GetFace(Faces.FRONT);
                _chunkVertices.AddRange(frontFaceData.vertices);
                _chunkUVs.AddRange(frontFaceData.uv);

                var backFaceData = block.GetFace(Faces.BACK);
                _chunkVertices.AddRange(backFaceData.vertices);
                _chunkUVs.AddRange(backFaceData.uv);

                var topFaceData = block.GetFace(Faces.TOP);
                _chunkVertices.AddRange(topFaceData.vertices);
                _chunkUVs.AddRange(topFaceData.uv);

                var bottomFaceData = block.GetFace(Faces.BOTTOM);
                _chunkVertices.AddRange(bottomFaceData.vertices);
                _chunkUVs.AddRange(bottomFaceData.uv);

                faceCount += 4;

                AddIndices(faceCount);
            }
        } // Generate the appropriate block faces given the data
        public void AddIndices(int amountFaces)
        {
            for(int i = 0; i < amountFaces; i++)
            {
                _chunkIndices.Add(0 + _indexCount);
                _chunkIndices.Add(1 + _indexCount);
                _chunkIndices.Add(2 + _indexCount);
                _chunkIndices.Add(2 + _indexCount);
                _chunkIndices.Add(3 + _indexCount);
                _chunkIndices.Add(0 + _indexCount);

                _indexCount += 4;
            }
        }
        public void BuildChunk() 
        {
            chunkVAO = new();
            chunkVAO.Bind();

            chunkVertexVBO = new(_chunkVertices);
            chunkVertexVBO.Bind();
            chunkVAO.LinkToVAO(0, 3, chunkVertexVBO);

            chunkUVVBO = new(_chunkUVs);
            chunkUVVBO.Bind();
            chunkVAO.LinkToVAO(1, 2, chunkUVVBO);

            chunkIBO = new(_chunkIndices);
            texture = new("dirt.png");
        } // Take data and process it for rendering
        public void Render(ShaderProgram shaderProgram) 
        {
            shaderProgram.Bind();
            chunkVAO.Bind();
            chunkIBO.Bind();
            texture.Bind();
            GL.DrawElements(PrimitiveType.Triangles, _chunkIndices.Count, DrawElementsType.UnsignedInt, 0);
        }
        public void Delete()
        {
            chunkVAO.Delete();
            chunkVertexVBO.Delete();
            chunkUVVBO.Delete();    
            chunkIBO.Delete();
            texture.Delete();
        }
    }
}
