using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Teapot
{
    public struct SpriteInstance
    {
        public Texture2D Texture;
        public RectangleF Bounds;
        public SpriteInstance(Texture2D txt,RectangleF r)
        {
            Texture = txt;
            Bounds = r;
        }
    }

    public struct HeapSpriteInstance
    {
        public VertexBuffer VertexBuffer;
        public Texture2D Texture;
        public HeapSpriteInstance(Texture2D txt,VertexBuffer vbuff)
        {
            Texture = txt;
            VertexBuffer = vbuff;
        }
    }
}
