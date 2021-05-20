using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Lib3DRadSpace_DX;
using System.Diagnostics;

namespace Teapot
{
    public class Game1 : Game
    {
        private GraphicsDeviceManager _graphics;
        private SpriteBatch _spriteBatch;

        FirstPersonCamera Camera;
        Skybox Skybox;

        Matrix view;
        Matrix proj;

        BEPUphysics.Space Space;
        StaticRigidBody Map = new StaticRigidBody("Map", true, true, Vector3.Zero, Vector3.Zero, Vector3.One, "Maps\\Lab\\Labs_col");
        Skinmesh MapVisible = new Skinmesh("MapSkinmesh", true, "Maps\\Lab\\Labs", Vector3.Zero, Vector3.Zero, Vector3.Zero, Vector3.One);
        SpriteFont DefaultFont = null;
        Skinmesh StairsVisible = new Skinmesh("StairsSkinmesh", true, "Maps\\Lab\\Stairs", new Vector3(7.24f, 5.36f, -2.95f), Vector3.Zero, Vector3.Zero, Vector3.One);
        StaticRigidBody Stairs = new StaticRigidBody("StairsCollision", true, false, new Vector3(7.24f, 5.36f, -2.95f), Vector3.Zero, Vector3.One, "Maps\\Lab\\Stairs");

        Skinmesh Teapot = new Skinmesh("Teapot", true, "Objects//Teapot", new Vector3(4.32f, 0.76f, -6.73f), Vector3.Zero, Vector3.Zero, Vector3.One / 4);
        Skinmesh Cup = new Skinmesh("Cup", true, "Objects//Cup", new Vector3(4.32f, 0.17f, -6.73f), Vector3.Zero, Vector3.Zero, Vector3.One / 2);

        Skinmesh RoomIllusion1 = new Skinmesh("RoomNonEuclideanPart", true, "Maps//Lab//room_ill1", new Vector3(2,0,0), Vector3.Zero, Vector3.Zero, Vector3.One);

        Texture2D pblack;

        Skinmesh StairsCol2 = new Skinmesh("StairsRayCollider", false,"Maps\\Lab\\stairs_col2", Vector3.Zero, Vector3.Zero, Vector3.Zero, Vector3.One);

        public Game1()
        {
            _graphics = new GraphicsDeviceManager(this)
            {
                PreferredBackBufferWidth = 800,
                PreferredBackBufferHeight = 480
            };
            _graphics.ApplyChanges();
            Content.RootDirectory = "Content";
            IsMouseVisible = true;
        }

        protected override void Initialize()
        {
            Camera = new FirstPersonCamera("Camera", true, new Vector3(0,1f,0),Vector3.Forward,Vector3.Up,0.01f,500f,1.3446414f,10f,2f,4f,1f,0.5f,1.4f,0.5f,0.25f,2f,1f);
            Skybox = new Skybox("Skybox", true, "Skybox1\\Skybox.sky");
            base.Initialize();
        }

        VertexBuffer illusion2plane;
        VertexBuffer illusion3plane;

        protected override void LoadContent()
        {
            _spriteBatch = new SpriteBatch(GraphicsDevice);

            CurrentProject.GraphicsDevice = GraphicsDevice;
            CurrentProject.SkyboxShader.LoadShader(Content, GraphicsDevice);
            CurrentProject.TexturedMeshShader.LoadShader(Content, GraphicsDevice);
            CurrentProject.BasicColorShader.LoadShader(Content, GraphicsDevice);
            CurrentProject.Resolution = new Vector2(800, 600);

            CurrentProject.BasicColorShader.LoadShader(Content, GraphicsDevice);
            Skybox.Load(Content);

            Space = new BEPUphysics.Space();
            Space.ForceUpdater.Gravity = new BEPUutilities.Vector3(0, -9f, 0);
            Space.ForceUpdater.Enabled = true;

            Map.Load(Content);
            MapVisible.Load(Content);
            SetDefaultSkinmeshLightning(MapVisible.Model);
            Map.PhysicsInitialize(Space);

            StairsVisible.Load(Content);
            SetDefaultSkinmeshLightning(StairsVisible.Model);
            Stairs.Load(Content);
            Stairs.PhysicsInitialize(Space);

            Teapot.Load(Content);
            Cup.Load(Content);

            Camera.PhysicsInitialize(Space);
            Skybox.LinkCamera(Camera);

            DefaultFont = Content.Load<SpriteFont>("Font");

            RenderTargetMap = new RenderTarget2D(GraphicsDevice, 800, 480, false, SurfaceFormat.Color, DepthFormat.Depth24Stencil8, 0, RenderTargetUsage.DiscardContents);

            NonEuclideanRenders = new RenderTarget2D[10];
            for(int i =0; i < 10; i++)
            {
                NonEuclideanRenders[i] = new RenderTarget2D(GraphicsDevice, 800, 480, false, SurfaceFormat.Color, DepthFormat.Depth24Stencil8, 0, RenderTargetUsage.DiscardContents);
            }

            StairsCol2.Load(Content);

            illusion2plane = new VertexBuffer(GraphicsDevice, typeof(VertexPositionColor), 6, BufferUsage.None);
            illusion2plane.SetData(new VertexPositionColor[]
                {
                    new VertexPositionColor(new Vector3(-8.60f,1.4f,-13.46f),Color.Red),
                    new VertexPositionColor(new Vector3(-8.60f,1.4f,-15.73f),Color.Red),
                    new VertexPositionColor(new Vector3(-8.60f,0f,-13.46f),Color.Red),

                    new VertexPositionColor(new Vector3(-8.60f,1.4f,-15.73f),Color.Red),
                    new VertexPositionColor(new Vector3(-8.60f,0f,-15.73f),Color.Red),
                    new VertexPositionColor(new Vector3(-8.60f,0f,-13.46f),Color.Red),

                });

            illusion3plane = new VertexBuffer(GraphicsDevice, typeof(VertexPositionColor), 6, BufferUsage.None);
            illusion3plane.SetData(new VertexPositionColor[]
                {
                    new VertexPositionColor(new Vector3(-8.60f,1.4f,-18.73f),Color.Red),
                    new VertexPositionColor(new Vector3(-8.60f,1.4f,-21.00f),Color.Red),
                    new VertexPositionColor(new Vector3(-8.60f,0f,-18.73f),Color.Red),

                    new VertexPositionColor(new Vector3(-8.60f,1.4f,-21.00f),Color.Red),
                    new VertexPositionColor(new Vector3(-8.60f,0f,-21.00f),Color.Red),
                    new VertexPositionColor(new Vector3(-8.60f,0f,-18.73f),Color.Red),

                });

            RoomIllusion1.Load(Content);
            SetDefaultSkinmeshLightning(RoomIllusion1.Model);

            NonEuclideanRenderFunctions.LoadSepShader(Content);

            pblack = new Texture2D(GraphicsDevice, 1, 1);
            pblack.SetData(new Color[] { Color.Black });
        }

        public static bool allowdebugging = false;

        BoundingSphere StairsHitbox = new BoundingSphere(new Vector3(7.7071533f, 11.5f, -7.641618f),1.2f);
        BoundingSphere Illusion1Hitbox = new BoundingSphere(new Vector3(-8.744476f, 0.6892655f,-14.689411f),1.2f);
        BoundingSphere Illusion2Hitbox = new BoundingSphere(new Vector3(-8.744476f, 0.6892655f,-19.5f),1.2f);

        protected override void Update(GameTime gameTime)
        {
            MouseState mouse = Mouse.GetState();
            KeyboardState keyboard = Keyboard.GetState();
            float dt = (float)gameTime.ElapsedGameTime.TotalSeconds;
            Camera.Update(ref mouse,ref keyboard, gameTime);
            view = Camera.View;
            proj = Camera.Projection;
            // TODO: Add your update logic here
            if(keyboard.IsKeyDown(Keys.F9)) Debugger.Break();
            if(keyboard.IsKeyDown(Keys.F8)) allowdebugging = true;

            if(StairsHitbox.Contains(Camera.Position) == ContainmentType.Contains)
            {
                Camera.Position = new Vector3(7.7071533f, 7f, -7.641618f) + (Camera.Position - StairsHitbox.Center);
            }
            if(Illusion1Hitbox.Contains(Camera.Position) == ContainmentType.Contains)
            {
                Camera.Position = new Vector3(-13.212797f,  0.68984985f, -14.558798f) + (Camera.Position - Illusion1Hitbox.Center);
            }
            if(Illusion2Hitbox.Contains(Camera.Position) == ContainmentType.Contains)
            {
               Camera.Position = new Vector3(-12.98f, 0.68984985f, -19.84f) + (Camera.Position - Illusion2Hitbox.Center);
            }
            if(Camera.CameraFrustum.Contains(new Vector3(3.96f, 0.76f, -6.12f)) != ContainmentType.Disjoint ||
                Camera.CameraFrustum.Contains(new Vector3(4.67f, 0.17f, -6.12f)) != ContainmentType.Disjoint )
            {
                if(MollerTrumbore.RayMeshCollision(new Ray(Camera.Position,Camera.LookDir),StairsCol2.Model,Matrix.Identity) != null)
                _illusion_visible = true;
            }
            else _illusion_visible = false;
            
            Space.Update(dt);
            base.Update(gameTime);
        }
        bool _illusion_visible = false;

        RenderTarget2D RenderTargetMap;
        SimpleBatcher Render2D;
        RenderTarget2D[] NonEuclideanRenders;

        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.CornflowerBlue);

            Camera.Draw(gameTime, null, ref view, ref proj);

            GraphicsDevice.BlendFactor = new Color(255, 255, 255, 255);
            GraphicsDevice.BlendState = BlendState.Opaque;
            GraphicsDevice.DepthStencilState = DepthStencilState.Default;
            GraphicsDevice.RasterizerState = RasterizerState.CullCounterClockwise;
            GraphicsDevice.SamplerStates[0] = SamplerState.PointWrap;

            GraphicsDevice.SetRenderTarget(RenderTargetMap);
            GraphicsDevice.Clear(Color.Transparent);

            MapVisible.DrawMeshWithoutPart(0, 3, Camera.CameraFrustum, ref view, ref proj);
            NonEuclideanRenderFunctions.DrawPlane(GraphicsDevice, illusion2plane, ref view, ref proj);
            NonEuclideanRenderFunctions.DrawPlane(GraphicsDevice, illusion3plane, ref view, ref proj);

            Teapot.Position = new Vector3(4.32f, 0.76f, -6.73f);
            Teapot.Draw(ref view, ref proj);
            Teapot.Position = new Vector3(-10.764509f, 0f, -19.796597f);
            Teapot.Draw(ref view, ref proj);
            Cup.Position = new Vector3(-6.42751f,  0f ,-20.183052f);
            Cup.Draw(ref view, ref proj);

            for(int i =0; i < 30;i++)
            {
                StairsVisible.Draw(ref view, ref proj);
                StairsVisible.Position.Y += 5.36f;
            }
            StairsVisible.Position.Y = 5.36f;

            GraphicsDevice.SetRenderTarget(NonEuclideanRenders[0]);
            GraphicsDevice.Clear(Color.Transparent);
            Teapot.Position = new Vector3(4.32f, 0.17f, -6.73f);
            Teapot.Draw(ref view, ref proj);

            GraphicsDevice.SetRenderTarget(NonEuclideanRenders[1]);
            GraphicsDevice.Clear(Color.Transparent);
            RoomIllusion1.DrawMeshWithoutPart(0, 3, Camera.CameraFrustum,ref view, ref proj);

            GraphicsDevice.SetRenderTarget(null);

             Skybox.Draw(gameTime, Camera.CameraFrustum, ref view, ref proj);
            
            _spriteBatch.Begin();
            _spriteBatch.Draw(NonEuclideanRenders[1], new Rectangle(0, 0, 800, 480), Color.White);
            _spriteBatch.Draw(RenderTargetMap, new Rectangle(0, 0, 800, 480), Color.White);
            if(_illusion_visible)
            {
                /*
                Vector3 p = Vector3.Transform(new Vector3(3.96f, 0.71f, -6.12f), view *proj);
                Vector3 p2 = Vector3.Transform(new Vector3(4.67f, 0.17f, -6.12f), view * proj);

                p += Vector3.One;
                p *= 0.5f;
                p2 += Vector3.One;
                p2 *= 0.5f;
                p.X *= 800;
                p2.X *= 800;

                p.Y *= 480;
                p2.Y *= 480;

                _spriteBatch.Draw(NonEuclideanRenders[0],
                   new Rectangle((int)p.X, 0,(int) p2.X, 480),
                   new Rectangle((int)p.X, 0,(int) p2.X, 480), Color.White); ;
                if(allowdebugging) Debugger.Break();
                _spriteBatch.DrawString(DefaultFont, "Second demonstration visible", new Vector2(10, 460), Color.White);
                */
            }
            _spriteBatch.DrawString(DefaultFont, "DEBUG: Position = " + Camera.Position, Vector2.One, Color.Black);
            _spriteBatch.End();
            base.Draw(gameTime);
        }
        void SetDefaultSkinmeshLightning(Model model)
        {
            for(int i = 0; i < model.Meshes.Count; i++)
            {
                for(int j = 0; j < model.Meshes[i].MeshParts.Count; j++)
                {
                    BasicEffect defshader = (BasicEffect)model.Meshes[i].MeshParts[j].Effect;
                    defshader.AmbientLightColor = new Vector3(1, 1, 1);
                    defshader.LightingEnabled = true;
                    defshader.DirectionalLight0.DiffuseColor = new Vector3(1, 1, 1);
                    defshader.DirectionalLight0.Enabled = true;
                    defshader.DirectionalLight0.SpecularColor = Vector3.Zero;
                    defshader.DirectionalLight0.Direction = new Vector3(0f, 1f, 0f);
                }
            }
        }
    }
}
