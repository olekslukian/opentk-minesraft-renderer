using OpenTK.Mathematics;


namespace MinesraftRenderer.World
{
    internal class Block
    {
        public Vector3 position;
        private readonly Dictionary<Faces, FaceData> _faces;

        readonly List<Vector2> defaultUv =
        [
            new(0f, 1f),
            new(1f, 1f),
            new(1f, 0f),
            new(0f, 0f)
        ];

        public Block(Vector3 position)
        {
            this.position = position;

            _faces = new()
            {
                {
                    Faces.FRONT,
                    new()
                        {
                            vertices = AddTransformedVertices(RawFaceData.rawVertexData[Faces.FRONT]),
                            uv = defaultUv
                        }
                },
                {
                    Faces.BACK,
                    new()
                        {
                            vertices = AddTransformedVertices(RawFaceData.rawVertexData[Faces.BACK]),
                            uv = defaultUv
                        }
                },
                {
                    Faces.LEFT,
                    new()
                        {
                            vertices = AddTransformedVertices(RawFaceData.rawVertexData[Faces.LEFT]),
                            uv = defaultUv
                        }
                },
                {
                    Faces.RIGHT,
                    new()
                        {
                            vertices = AddTransformedVertices(RawFaceData.rawVertexData[Faces.RIGHT]),
                            uv = defaultUv
                        }
                },
                {
                    Faces.TOP,
                    new()
                        {
                            vertices = AddTransformedVertices(RawFaceData.rawVertexData[Faces.TOP]),
                            uv = defaultUv
                        }
                },
                {
                    Faces.BOTTOM,
                    new()
                        {
                            vertices = AddTransformedVertices(RawFaceData.rawVertexData[Faces.BOTTOM]),
                            uv = defaultUv
                        }
                }
            };
        }
        
       
        public List<Vector3> AddTransformedVertices(List<Vector3> vertices) 
        {
            List<Vector3> transformedVertices = [];
            foreach(var vert in vertices)
            {
                transformedVertices.Add(vert + position);
            }
            return transformedVertices;
        }
        public FaceData GetFace(Faces face) 
        {
            return _faces[face];
        }
    }
}
