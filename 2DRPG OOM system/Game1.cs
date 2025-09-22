using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;
using System.Security.Cryptography.X509Certificates;


namespace _2DRPG_OOM_system
{
    public class Game1 : Game
    {
        private GraphicsDeviceManager _graphics;
        private SpriteBatch _spriteBatch;        
        public static Texture2D mapTexture;         
        public static List<Actor> characters = new List<Actor>();

        public static List<Quest> Quests = new List<Quest>();

        //track progress
        public static int enemiesDefeated = 0;
        public static bool firstItemPicked = false;
        public static bool bossDefeated = false;

        
        // This determine the size the tiles are going to have        
        public static int tileSize = 16;

        public static SpriteFont mySpriteFont;
        // Creating the Tilemap 
        public static Tilemap tileMap = new Tilemap();
        public static string mString;
               
        
        // It tell if is the player phase or enemy phase
        public static string whosTurn = "";
       
        // The object which manages the turns
        public static TurnManager turnManager = new TurnManager();

        // The list of item that are going to be in the map
        public static List<Item> itemsOnMap = new List<Item>();

        // List of projectiles
        public static List<Projectile> projectiles = new List<Projectile>();

        // This object put the items or enemies in random positions
        public static PlacementManager placementManager = new PlacementManager();
        // this list has temporary positions
        public static List<Vector2> tempPoints = new List<Vector2>();

        // This list will contains all the scenes the game will have
        private List<Scene> GameScenes = new List<Scene>();

      
        public static int maxNumLevel;  // How many scenes the game has
        public static int currentScene;  // The scene that is currently playing 
        private Menu mainMenu = new Menu();  // Creating a Scene for the main menu
        private Texture2D button01;  // Creating a button for using in different scenes
        private Menu EndScreen = new Menu();  // Creating the scene when reach the end of the game and win
        private Menu GameOver = new Menu();  // Creating the scene when you lose
        
        
        public Game1()
        {
            _graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
            IsMouseVisible = true;             
        }

        protected override void Initialize()
        {
            // TODO: Add your initialization logic here

            base.Initialize();
            currentScene = 0; 
            mapTexture = Content.Load<Texture2D>("The_Tilemap");            
            mySpriteFont = Content.Load<SpriteFont>("Font");
            button01 = Content.Load<Texture2D>("button_brown"); 
                              
            // All the elements for the Main Menu
            mainMenu.buttons.Add(new UIButton(button01, mySpriteFont, "Start Game", new Vector2(325, 200), 160, 64, UIButton.ButtonType.StartGame));
            mainMenu.textList.Add(new UIText("A Soldier 2D Mission", mySpriteFont, new Vector2(300, 100), Color.Black));
            mainMenu.buttons.Add(new UIButton(button01, mySpriteFont, "Quit", new Vector2(325, 300), 160, 64, UIButton.ButtonType.Exit));
            mainMenu.backgroundColor = Color.Lerp(Color.Gray, Color.Honeydew, 0.5f);

            // All the element for the End Screen (When you win the game)
            EndScreen.textList.Add(new UIText("The End", mySpriteFont, new Vector2(Window.ClientBounds.Width/2 - 50, 200), Color.Black));
            EndScreen.textList.Add(new UIText("Congratulation, you won!", mySpriteFont, new Vector2(Window.ClientBounds.Width/2 - 125, 100), Color.Black));
            EndScreen.buttons.Add(new UIButton(button01, mySpriteFont, "Play Again", new Vector2(Window.ClientBounds.Width / 2, 300), 160, 64, UIButton.ButtonType.StartGame));
            EndScreen.buttons.Add(new UIButton(button01, mySpriteFont, "Go to Menu", new Vector2(Window.ClientBounds.Width / 2, 400), 160, 64, UIButton.ButtonType.ToMenu));
            EndScreen.backgroundColor = Color.Lerp(Color.Gray, Color.Honeydew, 0.75f); 

            // All the element for the Game Over Screen (When you lose)
            GameOver.textList.Add(new UIText("Game Over", mySpriteFont, new Vector2(Window.ClientBounds.Width / 2 - 50, 50), Color.White));
            GameOver.buttons.Add(new UIButton(button01, mySpriteFont, "Try Again", new Vector2(325, 200), 160, 64, UIButton.ButtonType.StartGame));
            GameOver.buttons.Add(new UIButton(button01, mySpriteFont, "Go to Menu", new Vector2(325, 300), 160, 64, UIButton.ButtonType.ToMenu)); 
            GameOver.backgroundColor = Color.Lerp(Color.Black, Color.Honeydew, 0.25f); 

            //Store all the scenes in the list of scene 
            GameScenes.Add(mainMenu);  
            GameScenes.Add(new Level(1));
            GameScenes.Add(new Level(2));
            GameScenes.Add(new Level(3));
            GameScenes.Add(new Level(4));
            GameScenes.Add(EndScreen);
            GameScenes.Add(GameOver);

            //Adds the 3 quests to the game, more can be added
            Quests.Clear();
            Quests.Add(new Quest(Quest.QuestType.Beat5Enemies, "Beat 5 enemies", "Defeat 5 enemies total"));
            Quests.Add(new Quest(Quest.QuestType.PickFirstItem, "Pick up First Item", "Pick up your First Item"));
            Quests.Add(new Quest(Quest.QuestType.BeatBoss, "Beat the Boss", "Defeat the Final Boss"));

            // Once all the scenes are created and stored in the list of scene, we have how many scenes the game has
            maxNumLevel = GameScenes.Count; 
        }

        protected override void LoadContent()
        {
            _spriteBatch = new SpriteBatch(GraphicsDevice);
           
            // TODO: use this.Content to load your game content here
        }

        protected override void Update(GameTime gameTime)
        {
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape))
                Exit();

            GameScenes[currentScene].SceneUpdate(gameTime);          
            
            
            base.Update(gameTime);
            
        }
               

        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(GameScenes[currentScene].backgroundColor);
            //GraphicsDevice.Clear(Color.DarkBlue); 

            // TODO: Add your drawing code here

            _spriteBatch.Begin(samplerState: SamplerState.PointClamp);


            GameScenes[currentScene].DrawScene(gameTime, _spriteBatch); 

            _spriteBatch.End();

            base.Draw(gameTime);
        }

        
    }
}
