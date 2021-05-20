using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Teapot
{
    static class ExtraMath
    {
        public static bool FrustrumCameraCollision(BoundingFrustum frustum,Vector3 p)
        {
           Vector3 r = Vector3.Transform(p, frustum.Matrix);
            return Between(-1, r.X, 1) && Between(-1, r.X, 1) && Between(0, r.Z, 1);
        }
        internal static bool Between(float a, float x, float b)
        {
            return a <= x && x <= b;
        }
    }
}
