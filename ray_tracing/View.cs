using System;
using System.Diagnostics;
using System.IO;
using OpenTK;
using OpenTK.Graphics;
using OpenTK.Graphics.OpenGL;

namespace ray_tracing
{
    internal class View
    {
        private int BasicProgramID;
        private int BasicVertexShader;
        private int BasicFragmentShader;
        private int vao;
        private int vbo;

        private int cubeColorLoc;
        private int tetraColorLoc;
        private int RWL;
        private int transparencyLoc;
        private int maxDepthLoc;
        private int uCameraOffsetLoc;
        private int uCameraOffsetLoc1;
        private int uCameraOffsetLoc2;

        private Vector3 cubeColor = new Vector3(1f, 0.5f, 0.2f);

        private Vector3 tetraColor = new Vector3(0.2f, 0.8f, 0.4f);
        private float Right_wall = 0.0f;
        private float cameraOffset = 0.0f;
        private float cameraOffset1 = 0.0f;
        private float cameraOffset2 = -8.0f;
        private float transparency = 0.3f;
        private int maxDepth = 3;

        private void LoadShader(string filename, ShaderType type, int program, out int address)
        {
            string path = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, filename);
            if (!File.Exists(path))
                throw new FileNotFoundException($"Shader file not found: {path}");

            address = GL.CreateShader(type);
            string shaderSource = File.ReadAllText(path);
            GL.ShaderSource(address, shaderSource);
            GL.CompileShader(address);

            GL.GetShader(address, ShaderParameter.CompileStatus, out int success);
            if (success == 0)
            {
                string log = GL.GetShaderInfoLog(address);
                Debug.WriteLine($"Shader compilation error ({type}):\n{log}");
                throw new Exception($"Shader compilation failed: {log}");
            }

            GL.AttachShader(program, address);
        }

        public void InitShaders()
        {
            BasicProgramID = GL.CreateProgram();
            LoadShader("C:/Users/slana/Documents/c lang/c++/ray_tracing/raytracing.vert", ShaderType.VertexShader, BasicProgramID, out BasicVertexShader);
            LoadShader("C:/Users/slana/Documents/c lang/c++/ray_tracing/raytracing.frag", ShaderType.FragmentShader, BasicProgramID, out BasicFragmentShader);

            GL.LinkProgram(BasicProgramID);

            cubeColorLoc = GL.GetUniformLocation(BasicProgramID, "cubeColor");
            tetraColorLoc = GL.GetUniformLocation(BasicProgramID, "tetraColor");
            RWL = GL.GetUniformLocation(BasicProgramID, "RW");
            transparencyLoc = GL.GetUniformLocation(BasicProgramID, "transparency");
            maxDepthLoc = GL.GetUniformLocation(BasicProgramID, "maxDepth");
            uCameraOffsetLoc = GL.GetUniformLocation(BasicProgramID, "uCameraOffset");
            uCameraOffsetLoc1 = GL.GetUniformLocation(BasicProgramID, "uCameraOffset1");
            uCameraOffsetLoc2 = GL.GetUniformLocation(BasicProgramID, "uCameraOffset2");

            string programLog = GL.GetProgramInfoLog(BasicProgramID);
            if (!string.IsNullOrEmpty(programLog))
                Console.WriteLine($"Program link log:\n{programLog}");
        }

        public void InitBuffers()
        {
            float[] vertices = {

                -1.0f,  1.0f,  0.0f, 1.0f,
                -1.0f, -1.0f,  0.0f, 0.0f,
                 1.0f, -1.0f,  1.0f, 0.0f,
                -1.0f,  1.0f,  0.0f, 1.0f,
                 1.0f, -1.0f,  1.0f, 0.0f,
                 1.0f,  1.0f,  1.0f, 1.0f
            };

            GL.GenVertexArrays(1, out vao);
            GL.GenBuffers(1, out vbo);

            GL.BindVertexArray(vao);
            GL.BindBuffer(BufferTarget.ArrayBuffer, vbo);
            GL.BufferData(BufferTarget.ArrayBuffer, vertices.Length * sizeof(float), vertices, BufferUsageHint.StaticDraw);

            GL.EnableVertexAttribArray(0);
            GL.VertexAttribPointer(0, 2, VertexAttribPointerType.Float, false, 4 * sizeof(float), 0);

            GL.EnableVertexAttribArray(1);
            GL.VertexAttribPointer(1, 2, VertexAttribPointerType.Float, false, 4 * sizeof(float), 2 * sizeof(float));
        }

        public void Draw()
        {
            GL.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit);
            GL.UseProgram(BasicProgramID);

            GL.Uniform3(cubeColorLoc, cubeColor);
            GL.Uniform3(tetraColorLoc, tetraColor);
            GL.Uniform1(RWL, Right_wall);
            GL.Uniform1(transparencyLoc, transparency);
            GL.Uniform1(maxDepthLoc, maxDepth);
            GL.Uniform1(uCameraOffsetLoc, cameraOffset);
            GL.Uniform1(uCameraOffsetLoc1, cameraOffset1);
            GL.Uniform1(uCameraOffsetLoc2, cameraOffset2);

            GL.BindVertexArray(vao);
            GL.DrawArrays(PrimitiveType.Triangles, 0, 6);
        }

        public void SetCameraOffset(float offset)
        {
            cameraOffset = offset;
        }

        public void SetCameraOffset1(float offset)
        {
            cameraOffset1 = offset;
        }
        public void SetCameraOffset2(float offset)
        {
            cameraOffset2 = offset;
        }

        public void SetCubeColor(Vector3 color) => cubeColor = color;
        public void SetTetraColor(Vector3 color) => tetraColor = color;
        public void SetRW(float coef) => Right_wall = coef;
        public void SetTransparency(float trans) => transparency = trans;
        public void SetMaxDepth(int depth) => maxDepth = depth;
    }
}