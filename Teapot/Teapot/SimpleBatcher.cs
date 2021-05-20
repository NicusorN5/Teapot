using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Teapot
{
    class SimpleBatcher
    {
        GraphicsDevice _gref;
        public SimpleBatcher(GraphicsDevice dev)
        {
            _gref = dev;
        }
        public VertexPositionTexture[] GetVertexArray(RectangleF bounds)
        {
            VertexPositionTexture[] vdecl = new VertexPositionTexture[6];

            vdecl[0] = new VertexPositionTexture(new Vector3(bounds.X, bounds.Y, 0), Vector2.One);
            vdecl[1] = new VertexPositionTexture(new Vector3(bounds.X, bounds.Y + bounds.Height, 0), Vector2.UnitX);
            vdecl[2] = new VertexPositionTexture(new Vector3(bounds.X+bounds.Width, bounds.Y, 0), Vector2.UnitY);
            
            vdecl[3] = new VertexPositionTexture(new Vector3(bounds.X + bounds.Width, bounds.Y, 0), Vector2.UnitY);
            vdecl[4] = new VertexPositionTexture(new Vector3(bounds.X, bounds.Y + bounds.Height, 0), Vector2.UnitX);
            vdecl[5] = new VertexPositionTexture(new Vector3(bounds.X + bounds.Width, bounds.Y+bounds.Height, 0), Vector2.Zero);

            return vdecl;
        }
        public VertexBuffer GetVertexBuffer(VertexPositionTexture[] vpt)
        {
            VertexBuffer buff = new VertexBuffer(_gref, typeof(VertexPositionTexture), 6, BufferUsage.None);
            buff.SetData<VertexPositionTexture>(vpt);
            return buff;
        }
        
        public void Draw(ref HeapSpriteInstance image,Effect shader)
        {
            if(shader == null) return;
            foreach(EffectPass pass in shader.CurrentTechnique.Passes)
            {
                pass.Apply();
                shader.Parameters["World"].SetValue(Matrix.Identity);
                shader.Parameters["View"].SetValue(Matrix.Identity);
                shader.Parameters["Projection"].SetValue(Matrix.Identity);
                shader.Parameters["ModelTexture"].SetValue(image.Texture);
                _gref.SetVertexBuffer(image.VertexBuffer);
                _gref.DrawPrimitives(PrimitiveType.TriangleList, 0, 6);
            }
        }
    }
}
