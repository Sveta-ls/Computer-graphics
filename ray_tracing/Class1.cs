using OpenTK;
using OpenTK.Graphics.OpenGL;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ray_tracing
{
    public class GeometryManager
    {
        private int vbo_position;
        private Vector3[] vertdata;

        public void InitBuffers()
        {
            vertdata = new Vector3[]
            {
            new Vector3(-1f, -1f, 0f),
            new Vector3(1f, -1f, 0f),
            new Vector3(1f, 1f, 0f),
            new Vector3(-1f, 1f, 0f)
            };

            GL.GenBuffers(1, out vbo_position);
            GL.BindBuffer(BufferTarget.ArrayBuffer, vbo_position);
            GL.BufferData(
                BufferTarget.ArrayBuffer,
                (IntPtr)(vertdata.Length * Vector3.SizeInBytes),
                vertdata,
                BufferUsageHint.StaticDraw
            );
        }

        public void SetupAttributes(int attribute_vpos, int uniform_pos, int uniform_aspect, Vector3 campos, float aspect)
        {
            GL.VertexAttribPointer(attribute_vpos, 3, VertexAttribPointerType.Float, false, 0, 0);
            GL.Uniform3(uniform_pos, campos);
            GL.Uniform1(uniform_aspect, aspect);
        }
    }
}
