
using OpenTK.Mathematics;
using OpenTK.Windowing.Common;
using OpenTK.Windowing.GraphicsLibraryFramework;



namespace MinesraftRenderer
{
    internal class Camera(float width, float height, Vector3 position)
    {
        // CONSTANTS
        private float SCREEN_WIDTH = width;
        private float SCREEN_HEIGHT = height;
        private float SPEED = 8f;
        private float SENSITIVITY = 180f;

        // POSITION VARS
        private Vector3 _position = position;
        private Vector3 _up = Vector3.UnitY;
        private Vector3 _front = -Vector3.UnitZ;
        private Vector3 _right = Vector3.UnitX;

        //VIEW ROTATIONS
        private float _pitch;
        private float _yaw = -90.0f;

        private bool _firstMove = true;

        public Vector2 lastPosition;
        public Matrix4 GetViewMatrix() 
        {
            return Matrix4.LookAt(_position, _position+_front, _up);
        }
        public Matrix4 GetProjectionMatrix() 
        {
            return Matrix4.CreatePerspectiveFieldOfView(MathHelper.DegreesToRadians(45.0f), SCREEN_WIDTH/SCREEN_HEIGHT, 1.0f, 100f);
        }

        private void UpdateVectors() 
        { 

            if  (_pitch > 89.0f)
            {
                _pitch = 89.0f;
            }
            if (_pitch < -89.0f)
            {
                _pitch = -89.0f;
            }


            _front.X = MathF.Cos(MathHelper.DegreesToRadians(_pitch)) * MathF.Cos(MathHelper.DegreesToRadians(_yaw));
            _front.Y = MathF.Sin(MathHelper.DegreesToRadians(_pitch));
            _front.Z = MathF.Cos(MathHelper.DegreesToRadians(_pitch)) * MathF.Sin(MathHelper.DegreesToRadians(_yaw));

            _front = Vector3.Normalize(_front);

            _right = Vector3.Normalize(Vector3.Cross(_front, Vector3.UnitY));
            _up = Vector3.Normalize(Vector3.Cross(_right, _front));
        }

        public void InputController(KeyboardState input, MouseState mouse, FrameEventArgs e) 
        {
            if (input.IsKeyDown(Keys.W))
            {
                _position += _front * SPEED * (float)e.Time;
            }

            if (input.IsKeyDown(Keys.S))
            {
                _position -= _front * SPEED * (float)e.Time;
                
            }

            if (input.IsKeyDown(Keys.A))
            {
                _position -= _right * SPEED * (float)e.Time;
            }

            if (input.IsKeyDown(Keys.D))
            {
                _position += _right * SPEED * (float)e.Time;
            }

            if(input.IsKeyDown(Keys.Space))
            {
                _position.Y += SPEED * (float)e.Time;
            }

            if (input.IsKeyDown(Keys.LeftShift))
            {
                _position.Y -= SPEED * (float)e.Time;
            }

            if (_firstMove)
            {
                lastPosition = new Vector2(mouse.X, mouse.Y);
                _firstMove = false;
            } else
            {
                var dX = mouse.X - lastPosition.X;
                var dY = mouse.Y - lastPosition.Y;

                lastPosition = new Vector2(mouse.X, mouse.Y);

                _yaw += dX * SENSITIVITY * (float)e.Time;
                _pitch -= dY * SENSITIVITY * (float)e.Time;
            }

            UpdateVectors();
        }
        
        public void Update(KeyboardState input, MouseState mouse, FrameEventArgs e)
        {
            InputController(input, mouse, e);
        }
    }
}
