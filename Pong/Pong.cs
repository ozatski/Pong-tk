using OpenTK.Graphics.OpenGL4;
using OpenTK.Mathematics;
using OpenTK.Windowing.Common;
using OpenTK.Windowing.Desktop;
using OpenTK.Windowing.GraphicsLibraryFramework;
using System.Diagnostics;

namespace Pong
{
    public class Pong : GameWindow
    {
        private int _vertexBufferObject;
        private int _elementBufferObject;
        private int _vertexArrayObject;

        private Shader _shader;
        private Stopwatch _timer;
        private Texture _texture;
        private Texture _texture2;
        private Matrix4 _view;
        private Matrix4 _projection;
        private double _time;
        private float _cameraSpeed = 1.5f;
        private float _mouseSensitivity = 0.2f;
        private Vector2 _lastMousePos;
        private bool _firstMove = true;
        private Camera _camera;
        private Cube _cube;
        private Cube _cube2;
        private Matrix4 model2;

        
        protected override void OnLoad()
        {
            base.OnLoad();

            GL.Enable(EnableCap.DepthTest);

            //Initialize VAO
            _vertexArrayObject = GL.GenVertexArray();
            GL.BindVertexArray(_vertexArrayObject);

            //Initialize VertexBuffer
            _vertexBufferObject = GL.GenBuffer();
            GL.BindBuffer(BufferTarget.ArrayBuffer, _vertexBufferObject);
     
            //Initialize EBO 
            _elementBufferObject = GL.GenBuffer();
            GL.BindBuffer(BufferTarget.ElementArrayBuffer, _elementBufferObject);
          
            //Create and compile shaders
            _shader = new Shader("Shaders/shader.vert", "Shaders/shader.frag");
            _texture = Texture.LoadFromFile("Resources/container.png");
            _texture2 = Texture.LoadFromFile("Resources/awesomeface.png");

            _cube = new Cube(_shader);
            
            _cube.addTexture(_texture, "Texture0", 0, "texture1");
            _cube.addTexture(_texture2, "Texture1", 1, "texture2");

            _cube2 = new Cube(_shader);
            _cube2.addTexture(_texture, "Texture0", 0, "texture1");
            _cube2.addTexture(_texture2, "Texture1", 1, "texture2");

            // Set the transformation for the second cube
             model2 = Matrix4.CreateTranslation(new Vector3(3.0f, 0.0f, -1.0f)); // Example translation
            _cube2.SetModelMatrix(model2);

            _camera = new Camera(Vector3.UnitZ * 3, Size.X / (float)Size.Y);

            CursorState = CursorState.Grabbed;

        }


        public Pong(int width, int height, string title) :
            base(GameWindowSettings.Default,
                new NativeWindowSettings() { Size = (width, height), Title = title }) {


        }

        protected override void OnRenderFrame(FrameEventArgs e)
        {
            base.OnRenderFrame(e);
            // Set _time using time since last rendered fram * 4 as spinning speed
            _time += 4.0 * e.Time;

            // Clear the color and depth buffer
            GL.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit);
 
            //Spin the first cube over time
            _cube.SetModelMatrix(Matrix4.Identity * Matrix4.CreateRotationX((float)MathHelper.DegreesToRadians(_time)));
            
            // Render the first cube
            _cube.Render();
          
            // Render the second cube
            _cube2.Render();

            // Update the view matrix and projection matrix
            _shader.SetMatrix4("view", _camera.GetViewMatrix());
            _shader.SetMatrix4("projection", _camera.GetProjectionMatrix());

            // Swap the front/back buffers so what we just rendered to the back buffer is displayed in the window
            SwapBuffers();
        }


        protected override void OnUpdateFrame(FrameEventArgs e)
        {
            base.OnUpdateFrame(e);

            var mouse = MouseState;

            if(!IsFocused)
            {
                return;
            }

            KeyboardState input = KeyboardState;

            if (KeyboardState.IsKeyDown(Keys.Escape))
            {
                Close();
            }

            //Camera movement 
            if(input.IsKeyDown(Keys.W))
            {
                // Move the camera forward relative to its current rotation based on the time since last frame.
                _camera.Position += _camera.Front * _cameraSpeed * (float)e.Time; // Forward
            }

            if (input.IsKeyDown(Keys.S))
            {
                _camera.Position -= _camera.Front * _cameraSpeed * (float)e.Time; //Backwards
            }

            if (input.IsKeyDown(Keys.A))
            {
                _camera.Position -= _camera.Right * _cameraSpeed * (float)e.Time; //Left
            }

            if (input.IsKeyDown(Keys.D))
            {
                _camera.Position += _camera.Right * _cameraSpeed * (float)e.Time; //Right
            }

            if (input.IsKeyDown(Keys.Space))
            {
                _camera.Position += _camera.Up * _cameraSpeed * (float)e.Time; //Up 
            }

            if (input.IsKeyDown(Keys.LeftShift))
            {
                _camera.Position -= _camera.Up * _cameraSpeed * (float)e.Time; //Down
            }

            //Mouse movement
            
            if (_firstMove)
            {
               _lastMousePos = new Vector2(mouse.X, mouse.Y);
                _firstMove = false;
            }
            else
            {
                var deltaX = mouse.X - _lastMousePos.X;
                var deltaY = mouse.Y - _lastMousePos.Y;
                _lastMousePos = new Vector2(mouse.X, mouse.Y);

                //Set the camera yaw and pitch based on mouse movement
                _camera.Yaw += deltaX * _mouseSensitivity;
                _camera.Pitch -= deltaY * _mouseSensitivity;
     
            }


        }


        protected override void OnResize(ResizeEventArgs e)
        {
            base.OnResize(e);

            GL.Viewport(0, 0, Size.X, Size.Y);
            // We need to update the aspect ratio once the window has been resized.
            _camera.AspectRatio = Size.X / (float)Size.Y;
        }

        protected override void OnUnload()
        {
            base.OnUnload();
            // Unbind all the resources by binding the targets to 0/null.
            GL.BindBuffer(BufferTarget.ArrayBuffer, 0);
            GL.BindVertexArray(0);
            GL.UseProgram(0);

            // Delete all the resources.
            GL.DeleteBuffer(_vertexBufferObject);
            GL.DeleteVertexArray(_vertexArrayObject);

            _shader.Dispose();
        }

        protected override void OnMouseWheel(MouseWheelEventArgs e)
        {
            base.OnMouseWheel(e);
            _camera.Fov -= e.OffsetY;
        }


    }
}
