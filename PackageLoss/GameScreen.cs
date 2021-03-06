﻿using FarseerPhysics;
using FarseerPhysics.Collision.Shapes;
using FarseerPhysics.Common;
using FarseerPhysics.Common.Decomposition;
using FarseerPhysics.Common.PolygonManipulation;
using FarseerPhysics.Dynamics;
using FarseerPhysics.Dynamics.Joints;
using FarseerPhysics.Factories;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace PackageLoss
{
    internal class GameScreen : BaseScreen
    {
        public Random random = new Random();
        Texture2D[] backgrounds;
        Dictionary<string, Texture2D> tileTextures;
        Dictionary<string, Texture2D> objectTextures;
        GameObject movingObject = null, tire1, tire2;
        Vector2 moveSpeed, axel;
        Vector2 mouseInWorld, mouseOnScreen, mouseFix = new Vector2(20, 16);
        Vector2 moveDelta;
        Vector2 scorePosition = new Vector2(200f, 20f);
        protected World World;
        WheelJoint frontWheelJoint, rearWheelJoint;
        int mouseMiddle;
        Body HiddenBody;        
        Rectangle bgRectangle;
        Dictionary<string, GameObject> gameObjects;
        //Body bottom;
        readonly float minScale = 0.4f, maxScale = 1.0f, carPower = 4f, minZoom = 0.7f;
        bool drivingState = false;
        GameObject car, carBridge;
        //Rectangle bottomRectangle;
        Texture2D mouseTexture;
        Joint mouseJoint;
        float acceleration, maxSpeed = 100f;
        Dictionary<string, SoundEffect> soundEffects;
        List<GameObject> scores = new List<GameObject>();
        GameTime lastGameTime;
        SpriteFont font;

        public Camera2D Camera { get; set; }
        public Game1 Game { get; set; }

        public GameScreen(Game1 game)
        {
            SetGame(game);
        }

        public void SetGame(Game1 game)
        {
            this.Game = game;
        }

        #region IDemoScreen Members

        public string GetTitle()
        {
            return "Package Loss";
        }

        public string GetDetails()
        {
            StringBuilder sb = new StringBuilder();
            sb.AppendLine("TODO: Add sample description!");
            
            return sb.ToString();
        }

        #endregion

        public void LoadContent()
        {
            bgRectangle = new Rectangle(0, 0, Game.Window.ClientBounds.Width, Game.Window.ClientBounds.Height);
            gameObjects = new Dictionary<string, GameObject>();
            World = new World(new Vector2(0f, 9.82f));
            mouseTexture = Game.Content.Load<Texture2D>("Mouse-cursor-hand-pointer");
            Camera = new Camera2D(Game.GraphicsDevice);
            
            HiddenBody = BodyFactory.CreateBody(World, Vector2.Zero);
            //load texture that will represent the physics body
            objectTextures = new Dictionary<string, Texture2D> {
                { "anvil", Game.Content.Load<Texture2D>("anvil") },
                { "basketBall01", Game.Content.Load<Texture2D>("basketBall01")},
                { "basketBall02", Game.Content.Load<Texture2D>("basketBall02")},
                { "beerCrate", Game.Content.Load<Texture2D>("beerCrate")},
                { "Cat1", Game.Content.Load<Texture2D>("Cat1")},
                { "chainsaw", Game.Content.Load<Texture2D>("chainsaw")},
                { "clownhat", Game.Content.Load<Texture2D>("clownhat")},
                { "clownShoes", Game.Content.Load<Texture2D>("clownShoes")},
                { "coffeeBrewer", Game.Content.Load<Texture2D>("coffeeBrewer")},
                { "crystal", Game.Content.Load<Texture2D>("crystal")},
                { "football", Game.Content.Load<Texture2D>("football")},
                { "goo", Game.Content.Load<Texture2D>("goo")},
                { "katana", Game.Content.Load<Texture2D>("katana")},
                { "nunchaku", Game.Content.Load<Texture2D>("nunchaku")},
                { "pillow01", Game.Content.Load<Texture2D>("pillow01")},
                { "riceHat", Game.Content.Load<Texture2D>("riceHat")},
                { "shovel02", Game.Content.Load<Texture2D>("shovel02")},
                { "shuriken", Game.Content.Load<Texture2D>("shuriken")},
                { "skates", Game.Content.Load<Texture2D>("skates")},
                { "sword", Game.Content.Load<Texture2D>("sword")},
                { "table", Game.Content.Load<Texture2D>("table")},
                { "tv", Game.Content.Load<Texture2D>("tv")},
                { "woodenBox", Game.Content.Load<Texture2D>("woodenBox")},
                { "washingMachine", Game.Content.Load<Texture2D>("washingMachine")},       
                { "Sprinter2_ulko", Game.Content.Load<Texture2D>("Sprinter2_ulko")},
                { "Sprinter2_luukku", Game.Content.Load<Texture2D>("Sprinter2_luukku")},
                { "Sprinter2_rengas", Game.Content.Load<Texture2D>("Sprinter2_rengas")},
            };

            tileTextures = new Dictionary<string, Texture2D> {                
                { "downhillBegin", Game.Content.Load<Texture2D>("Tiles/downhillBegin")},
                { "downHill01", Game.Content.Load<Texture2D>("Tiles/downHill01") },
                { "downhillGentle", Game.Content.Load<Texture2D>("Tiles/downhillGentle")},      
                { "flat01", Game.Content.Load<Texture2D>("Tiles/flat01")},
                { "uphillGentle", Game.Content.Load<Texture2D>("Tiles/uphillGentle")},
                { "uphill01", Game.Content.Load<Texture2D>("Tiles/uphill01")},                
                { "uphillEnd", Game.Content.Load<Texture2D>("Tiles/uphillEnd")},
                { "kerrostalo", Game.Content.Load<Texture2D>("Tiles/kerrostalo")},
            };

            soundEffects = new Dictionary<string, SoundEffect>() {
                { "kengat1", Game.Content.Load<SoundEffect>("SoundEffects/kengat1") },
                { "kengat2", Game.Content.Load<SoundEffect>("SoundEffects/kengat2") },
                { "kissa1", Game.Content.Load<SoundEffect>("SoundEffects/kissa1") },
                { "kissa2", Game.Content.Load<SoundEffect>("SoundEffects/kissa2") },
                { "kissa3", Game.Content.Load<SoundEffect>("SoundEffects/kissa3") },
                { "kling1", Game.Content.Load<SoundEffect>("SoundEffects/kling1") },
                { "kling2", Game.Content.Load<SoundEffect>("SoundEffects/kling2") },
                { "kling3", Game.Content.Load<SoundEffect>("SoundEffects/kling3") },
                { "klink1", Game.Content.Load<SoundEffect>("SoundEffects/klink1") },
                { "klink2", Game.Content.Load<SoundEffect>("SoundEffects/klink2") },
                { "klink3", Game.Content.Load<SoundEffect>("SoundEffects/klink3") },
                { "kliring1", Game.Content.Load<SoundEffect>("SoundEffects/kliring1") },
                { "kliring2", Game.Content.Load<SoundEffect>("SoundEffects/kliring2") },
                { "kliring3", Game.Content.Load<SoundEffect>("SoundEffects/kliring3") },
                { "klonk1", Game.Content.Load<SoundEffect>("SoundEffects/klonk1") },
                { "klonk2", Game.Content.Load<SoundEffect>("SoundEffects/klonk2") },
                { "klonk3", Game.Content.Load<SoundEffect>("SoundEffects/klonk3") },
                { "laatikko1", Game.Content.Load<SoundEffect>("SoundEffects/laatikko1") },
                { "laatikko2", Game.Content.Load<SoundEffect>("SoundEffects/laatikko2") },
                { "laatikko3", Game.Content.Load<SoundEffect>("SoundEffects/laatikko3") },
                { "pahvi1", Game.Content.Load<SoundEffect>("SoundEffects/pahvi1") },
                { "pahvi2", Game.Content.Load<SoundEffect>("SoundEffects/pahvi2") },
                { "pahvi3", Game.Content.Load<SoundEffect>("SoundEffects/pahvi3") },
                { "thump1", Game.Content.Load<SoundEffect>("SoundEffects/thump1") },
                { "thump2", Game.Content.Load<SoundEffect>("SoundEffects/thump2") },
                { "thump3", Game.Content.Load<SoundEffect>("SoundEffects/thump3") },
                { "thunk1", Game.Content.Load<SoundEffect>("SoundEffects/thunk1") },
                { "thunk2", Game.Content.Load<SoundEffect>("SoundEffects/thunk2") },
                { "thunk3", Game.Content.Load<SoundEffect>("SoundEffects/thunk3") },
                { "tyyny1", Game.Content.Load<SoundEffect>("SoundEffects/tyyny1") },
                { "tyyny2", Game.Content.Load<SoundEffect>("SoundEffects/tyyny2") },
            };
            font = Game.Content.Load<SpriteFont>("Segoe UI Mono");
            

            backgrounds = new Texture2D[] { 
                Game.Content.Load<Texture2D>("Background/silhouetteBack"),
                Game.Content.Load<Texture2D>("Background/silhouetteMiddle"),
                Game.Content.Load<Texture2D>("Background/silhouetteFront"),
            };
            float screenWidth = ConvertUnits.ToSimUnits(Game.GraphicsDevice.Viewport.Height), screenHeight = ConvertUnits.ToSimUnits(Game.GraphicsDevice.Viewport.Width);
            float areaWidth = Game.GraphicsDevice.Viewport.Width / 2f, x = 0f;
            Vector2 objPos = new Vector2(0f, Game.GraphicsDevice.Viewport.Height - tileTextures["flat01"].Height);
            foreach (KeyValuePair<string, Texture2D> textureKV in objectTextures)
            {
                AddGameObject(textureKV.Value).Compound.Position = Camera.ConvertScreenToWorld(objPos);
                x += Math.Max(textureKV.Value.Width, textureKV.Value.Height);
                objPos = new Vector2(x % areaWidth, x / areaWidth);
            }

            gameObjects["anvil"].SoundEffectHit = soundEffects["kling1"];
            gameObjects["basketBall01"].SoundEffectHit = soundEffects["tyyny2"];
            gameObjects["basketBall02"].SoundEffectHit = soundEffects["tyyny1"];
            gameObjects["beerCrate"].SoundEffectHit = soundEffects["kliring1"];
            gameObjects["Cat1"].SoundEffectHit = soundEffects["kissa1"];
            gameObjects["chainsaw"].SoundEffectHit = soundEffects["tyyny1"];
            gameObjects["clownhat"].SoundEffectHit = soundEffects["tyyny1"];
            gameObjects["clownShoes"].SoundEffectHit = soundEffects["kengat2"];
            gameObjects["coffeeBrewer"].SoundEffectHit = soundEffects["klonk3"];
            gameObjects["crystal"].SoundEffectHit = soundEffects["kliring1"];
            gameObjects["football"].SoundEffectHit = soundEffects["tyyny1"];
            gameObjects["goo"].SoundEffectHit = soundEffects["tyyny1"];
            gameObjects["katana"      ].SoundEffectHit = soundEffects["kling1"];
            gameObjects["nunchaku"].SoundEffectHit = soundEffects["kling2"];
            gameObjects["pillow01"    ].SoundEffectHit = soundEffects["tyyny1"];
            gameObjects["riceHat"     ].SoundEffectHit = soundEffects["tyyny1"];
            gameObjects["shovel02"    ].SoundEffectHit = soundEffects["klonk3"];
            gameObjects["shuriken"    ].SoundEffectHit = soundEffects["kengat2"];
            gameObjects["skates"].SoundEffectHit = soundEffects["kengat2"];
            gameObjects["sword"       ].SoundEffectHit = soundEffects["klink2"];
            gameObjects["table"       ].SoundEffectHit = soundEffects["laatikko2"];
            gameObjects["tv"          ].SoundEffectHit = soundEffects["laatikko3"];
            gameObjects["woodenBox"   ].SoundEffectHit = soundEffects["laatikko2"];
            gameObjects["washingMachine"].SoundEffectHit = soundEffects["laatikko3"];

            mouseMiddle = Mouse.GetState().ScrollWheelValue;
            car = FindGameObject("Sprinter2_ulko");
            Vector2 carStart = new Vector2(Game.Window.ClientBounds.Width / 2f + 130f, Game.GraphicsDevice.Viewport.Height - tileTextures["flat01"].Height - 30f);
            car.Compound.Position = Camera.ConvertScreenToWorld(carStart);
            car.Compound.CollisionGroup = 1;
            car.Compound.Mass = 6000;
            car.Compound.BodyType = BodyType.Static;
            CircleShape wheelShape = new CircleShape(0.5f, 0.8f);
            tire1 = FindGameObject("Sprinter2_rengas");

            tire1.Compound.CreateFixture(wheelShape);
            tire1.Compound.Friction = 0.95f;
            Vector2 tireAxel = new Vector2(220f, 150f);
            tire1.Compound.Position = Camera.ConvertScreenToWorld(carStart + tireAxel);
            tire1.Compound.CollisionGroup = 1;
            frontWheelJoint = JointFactory.CreateWheelJoint(World, car.Compound, tire1.Compound, Vector2.UnitY);
            frontWheelJoint.MaxMotorTorque = 50f;
            frontWheelJoint.MotorEnabled = true;
            frontWheelJoint.Frequency = 4.0f;
            frontWheelJoint.DampingRatio = 0.7f;
            tire2 = AddGameObject(tire1.PolygonTexture, "Sprinter2_rengas_2"); // TODO create method with fixture
            tire2.Compound.CreateFixture(wheelShape);
            tire2.Compound.Friction = 0.99f;
            Vector2 tireAxel2 = new Vector2(-tireAxel.X - 110f, tireAxel.Y);
            tire2.Compound.Position = Camera.ConvertScreenToWorld(carStart + tireAxel2);
            tire2.Compound.CollisionGroup = 1;
            rearWheelJoint = JointFactory.CreateWheelJoint(World, car.Compound, tire2.Compound, Vector2.UnitY);
            rearWheelJoint.MaxMotorTorque = 100f;
            rearWheelJoint.MotorEnabled = true;
            rearWheelJoint.Frequency = 4.0f;
            rearWheelJoint.DampingRatio = 0.7f;
            carBridge = FindGameObject("Sprinter2_luukku");
            carBridge.Compound.Position = Camera.ConvertScreenToWorld(carStart + tireAxel2 - new Vector2(220f, 180f));
            carBridge.Compound.CollisionGroup = 1;
            JointFactory.CreateRevoluteJoint(World, car.Compound, carBridge.Compound, Vector2.Zero);

            GenerateWorld("4444444444444444444445555555555557444444444444444444444555555555555574444444444444444444448444444");
            Camera.Zoom = 1.3f;
            Camera.MoveCamera(ConvertUnits.ToSimUnits(new Vector2(-150f, 150f)));
        }

        public GameObject AddTile(Texture2D texture, String name, ref Vector2 pos, int direction)
        {
            GameObject go1 = AddGameObject(texture, name);
            go1.Compound.Position = Camera.ConvertScreenToWorld(pos);
            go1.Compound.BodyType = BodyType.Static;
            go1.Compound.CollisionGroup = 1;
            pos.X += go1.PolygonTexture.Width;
            pos.Y += go1.PolygonTexture.Height * direction;
            return go1;
        }

        /*
         * 1: downhillBegin
         * 2: downHill01
         * 3: downhillGentle
         * 4: flat01
         * 5: uphillGentle
         * 6: uphill01
         * 7: uphillEnd
         * 8: finish
         * // DO NOT START WITH ANYTHING ELSE THAN FLAT
         */
        public void GenerateWorld(string tiles)
        {
            Vector2 pos = new Vector2(-200f, Game.GraphicsDevice.Viewport.Height + tileTextures["flat01"].Height);
            GameObject lastObject = null;
            Vector2 offset = Vector2.Zero;
            int id = 0;
            foreach (char c in tiles)
            {
                switch (c)
                {
                    case '1':
                        lastObject = AddTile(tileTextures["downhillBegin"], "tile_" + id, ref pos, 1);                        
                        break;
                    case '2':
                        lastObject = AddTile(tileTextures["downHill01"], "tile_" + id, ref pos, 1);
                        break;
                    case '3':
                        lastObject = AddTile(tileTextures["downhillGentle"], "tile_" + id, ref pos, 1);
                        break;
                    case '4':
                        lastObject = AddTile(tileTextures["flat01"], "tile_" + id, ref pos, 0);
                        offset = new Vector2(0f, -lastObject.PolygonTexture.Height);
                        break;
                    case '5':
                        lastObject = AddTile(tileTextures["uphillGentle"], "tile_" + id, ref pos, -1);
                        offset = Vector2.Zero;
                        break;
                    case '6':
                        lastObject = AddTile(tileTextures["uphill01"], "tile_" + id, ref pos, -1);
                        break;
                    case '7':
                        lastObject = AddTile(tileTextures["uphillEnd"], "tile_" + id, ref pos, 0);
                        break;
                    case '8':
                        lastObject = AddTile(tileTextures["kerrostalo"], "finish_" + id, ref pos, 0);
                        lastObject.Name = "Finish";
                        lastObject.Compound.CollisionGroup = 8;
                        break;
                    default:
                        break;
                }
                id++;
            }
        }

        public GameObject AddGameObject(Texture2D texture, String name = null)
        {
            GameObject gameObject = new GameObject(this, texture, World);
            gameObject.Compound.OnCollision += Compound_OnCollision;
            gameObject.Name = name == null ? texture.Name : name;
            gameObjects.Add(gameObject.Name, gameObject);
            gameObject.Compound.CollisionGroup = 2;
            return gameObject;
        }

        bool Compound_OnCollision(Fixture fixtureA, Fixture fixtureB, FarseerPhysics.Dynamics.Contacts.Contact contact)
        {
            if (fixtureA.Body.LinearVelocity.Length() > 1f)
            {
                FindGameObject(fixtureA.Body).PlayHit(lastGameTime);                
            }
            if (fixtureB.Body.LinearVelocity.Length() > 1f)
            {
                FindGameObject(fixtureB.Body).PlayHit(lastGameTime);
            }
            if (fixtureA.CollisionGroup == 8 && fixtureB.CollisionGroup != 8)
            {
                GameObject go = FindGameObject(fixtureB.Body);
                if (!scores.Contains(go))
                    scores.Add(go);
                return false;
            }
            return true;
        }

        public void Draw(GameTime gameTime)
        {
            Game.SpriteBatch.Begin(SpriteSortMode.FrontToBack, BlendState.Opaque, null, DepthStencilState.Default, RasterizerState.CullNone);
            Game.SpriteBatch.Draw(backgrounds[0], Vector2.Zero, bgRectangle, Color.White, 0, Vector2.Zero, 4.0f, SpriteEffects.None, 0);
            Game.SpriteBatch.Draw(backgrounds[1], Vector2.Zero, bgRectangle, Color.White, 0, Vector2.Zero, 4.0f, SpriteEffects.None, 0);
            Game.SpriteBatch.Draw(backgrounds[2], Vector2.Zero, bgRectangle, Color.White, 0, Vector2.Zero, 4.0f, SpriteEffects.None, 0);
            Game.SpriteBatch.End();

            foreach (KeyValuePair<string,GameObject> gameObjectKV in gameObjects)
            {
                gameObjectKV.Value.Draw(Game.SpriteBatch, Camera);
            }
            Game.SpriteBatch.Begin();
            Game.SpriteBatch.Draw(mouseTexture, mouseOnScreen - mouseFix, Color.White);
            Game.SpriteBatch.Draw(Game.Character, Vector2.Zero, Color.White * 0.5f);
            Game.SpriteBatch.DrawString(font, "" + scores.Count * 10, scorePosition, Color.White);
            Game.SpriteBatch.End();
        }

        float CalculateScale(Vector2 position)
        {
            float scale = Vector2.Distance(position, car.Compound.Position) / 10.0f;
            if (scale < minScale)
                return minScale;
            else if (scale > maxScale)
                return maxScale;
            return scale;
        }

        public void Update(GameTime gameTime)
        {
            lastGameTime = gameTime;
            World.Step(Math.Min((float)gameTime.ElapsedGameTime.TotalSeconds, (1f / 30f)));
            foreach (KeyValuePair<string, GameObject> gameObjectKV in gameObjects)
            {
                gameObjectKV.Value.SetScale(CalculateScale(gameObjectKV.Value.Compound.Position));
                gameObjectKV.Value.Update(gameTime);
            }
            if (drivingState && Camera.Zoom > minZoom)
                Camera.Zoom -= 0.01f;
             if (drivingState)
            {
                rearWheelJoint.MotorSpeed = Math.Sign(acceleration) * MathHelper.SmoothStep(0f, maxSpeed, Math.Abs(acceleration));
                frontWheelJoint.MotorSpeed = rearWheelJoint.MotorSpeed;
                if (Math.Abs(rearWheelJoint.MotorSpeed) < maxSpeed * 0.06f)
                {
                    rearWheelJoint.MotorEnabled = frontWheelJoint.MotorEnabled = false;
                }
                else
                {
                    rearWheelJoint.MotorEnabled = frontWheelJoint.MotorEnabled = true;
                }
            }
            Camera.Update(gameTime);
        }

        public void HandleMouse(MouseState mouseState, GameTime gameTime)
        {
            mouseOnScreen = new Vector2(mouseState.X + 25, mouseState.Y + 30);
            Vector2 newMouseInWorld = Camera.ConvertScreenToWorld(mouseOnScreen);
            if (drivingState)
                return;
            if (mouseState.LeftButton == ButtonState.Pressed)
            {                
                
                moveSpeed = (mouseInWorld - newMouseInWorld) * (float)gameTime.ElapsedGameTime.TotalMilliseconds;
                mouseInWorld = newMouseInWorld;
                if (movingObject == null)
                {
                    Fixture fixture = World.TestPoint(mouseInWorld);
                    if (fixture != null && fixture.CollisionGroup != 1)
                    {
                        GameObject gameObject = FindGameObject(fixture.Body);
                        if (gameObject != null)
                        {
                            movingObject = gameObject;
                            moveDelta = movingObject.Compound.Position - mouseInWorld;
                        }
                        mouseJoint = JointFactory.CreateFixedMouseJoint(World, gameObject.Compound, mouseInWorld);
                        mouseJoint.CollideConnected = true;
                        gameObject.Compound.Awake = true;
                    }
                }
                else
                {
                    mouseJoint.WorldAnchorB = mouseInWorld;
                 }
            }

            if (movingObject != null && mouseState.LeftButton == ButtonState.Released)
            {
                if (mouseJoint != null)
                {
                    World.RemoveJoint(mouseJoint);
                    mouseJoint = null;
                }
                movingObject = null;
            }

            Camera.Zoom += (mouseMiddle - mouseState.ScrollWheelValue) / 10000.0f;
            mouseMiddle = mouseState.ScrollWheelValue;
        }

        internal GameObject FindGameObject(Body body)
        {
            foreach (KeyValuePair<string, GameObject> gameObjectKV in gameObjects)
            {
                if (gameObjectKV.Value.Compound == body)
                    return gameObjectKV.Value;
            }
            return null;
        }

        internal GameObject FindGameObject(String name)
        {
            
            return gameObjects[name];
        }

        public void HandleKeyboard(KeyboardState keyboardState, GameTime gameTime)
        {
            if (keyboardState.IsKeyDown(Keys.Up))
            {
                if (!drivingState)
                {
                    car.Compound.BodyType = BodyType.Dynamic;
                    //car.Compound.LinearVelocity += new Vector2(10f, 0f);
                    drivingState = true;
                    Camera.TrackingBody = car.Compound;
                }
                acceleration = Math.Min(acceleration + (float)(carPower * gameTime.ElapsedGameTime.TotalSeconds), 1f);

            }
            else if (keyboardState.IsKeyDown(Keys.Down))
                acceleration = Math.Max(acceleration - (float)(carPower * gameTime.ElapsedGameTime.TotalSeconds), -1f);
            else
                acceleration = 0f;
            if (keyboardState.IsKeyDown(Keys.Q))
                Camera.Position += new Vector2(0f, 0.1f);
            if (keyboardState.IsKeyDown(Keys.A))
                Camera.Position -= new Vector2(0f, 0.1f);
        }

        public void StartScreen()
        {

        }
    }
}
