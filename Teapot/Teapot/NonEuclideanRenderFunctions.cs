using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Content;

namespace Teapot
{
    static class NonEuclideanRenderFunctions
    {

        public static Effect SeparationPlaneShader;
        
        public static void LoadSepShader(ContentManager content)
        {
            SeparationPlaneShader = content.Load<Effect>("Shaders//SeparationPlaneRenderer");
        }
        public static VertexPositionColor[] CreateRenderPlane(Vector3[] points)
        {
            VertexPositionColor[] p = new VertexPositionColor[6];
            p[0] = new VertexPositionColor(points[0], new Color(255, 0, 0));
            p[1] = new VertexPositionColor(points[1], new Color(255, 0, 0));
            p[2] = new VertexPositionColor(points[2], new Color(255, 0, 0));

            p[3] = new VertexPositionColor(points[3], new Color(255, 0, 0));
            p[4] = new VertexPositionColor(points[4], new Color(255, 0, 0));
            p[5] = new VertexPositionColor(points[5], new Color(255, 0, 0));

            return p;
        }
        public static VertexBuffer GetPlaneBuffer(GraphicsDevice gdev,VertexPositionColor[] p)
        {
            return new VertexBuffer(gdev, typeof(VertexPositionColor), 6, BufferUsage.None);
        }
        public static void DrawPlane(GraphicsDevice gdev,VertexBuffer plane,ref Matrix view,ref Matrix proj)
        {
            Effect shader = Lib3DRadSpace_DX.CurrentProject.BasicColorShader.ShaderEffect;

            foreach(EffectPass epass in shader.CurrentTechnique.Passes)
            {
                shader.Parameters["World"].SetValue(Matrix.Identity);
                shader.Parameters["View"].SetValue(view);
                shader.Parameters["Projection"].SetValue(proj);
                shader.Parameters["PlainColor"].SetValue(new Vector4(0, 0, 0, 0));

                epass.Apply();
                gdev.SetVertexBuffer(plane);
                gdev.DrawPrimitives(PrimitiveType.TriangleList, 0, 2);
            }
        }
        public static Vector3 SimplifyScreenCoords(Vector3 worldpos,ref Matrix view,ref Matrix proj)
        {
             return Vector3.Transform(Vector3.Transform(worldpos, view),proj);
        }
        public static Vector2 GetSepPlaneUV(Vector3 p)
        {
            return new Vector2((p.X + 1) / 2, (p.Y + 1) / 2);
        }
        public static Vector3[] SimplifyScreenCoords(Vector3[] worldpos,ref Matrix view,ref Matrix proj)
        {
            Vector3[] r = new Vector3[worldpos.Length];
            for(int i =0; i < worldpos.Length;i++)
            {
                r[i] = SimplifyScreenCoords(worldpos[i],ref view,ref proj);
            }
            return r;
        }
        public static void Draw2DSeparationPlane(Texture2D texture,GraphicsDevice gdev,Vector3[] points,ref Matrix view,ref Matrix proj)
        {
            Vector3[] p = new Vector3[4];
            for(int i =0; i < 4;i++)
            {
                p[i] = points[i];
                p[i] = SimplifyScreenCoords(p[i], ref view, ref proj);
            }

            SeparationPlaneShader.Parameters["RenderTexture"].SetValue(texture);

            if(Game1.allowdebugging) System.Diagnostics.Debugger.Break();

            foreach(EffectPass efpass in SeparationPlaneShader.CurrentTechnique.Passes)
            {
                efpass.Apply();
                gdev.DrawUserPrimitives(PrimitiveType.TriangleList, new VertexPositionTexture[]
                    {
                        new VertexPositionTexture(p[0],GetSepPlaneUV(p[0])),
                        new VertexPositionTexture(p[1],GetSepPlaneUV(p[1])),
                        new VertexPositionTexture(p[2],GetSepPlaneUV(p[2])),
                        new VertexPositionTexture(p[1],GetSepPlaneUV(p[1])),
                        new VertexPositionTexture(p[2],GetSepPlaneUV(p[2])),
                        new VertexPositionTexture(p[3],GetSepPlaneUV(p[3]))
                    }, 0, 1);
            }
        }
    }
}
