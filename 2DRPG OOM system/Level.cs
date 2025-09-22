using _2DRPG_OOM_system;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


public class Level : Scene
{
    public Level (int iLevel) 
    {
        numberOfLevel = iLevel;
        backgroundColor = Color.DarkBlue; 
    }


    private int numberOfLevel;
    private bool openDoor = false; 

    public override void SceneUpdate(GameTime gameTime)
    {

        // This is the logic for the turns 
        Game1.turnManager.UpdateTurnManager(gameTime);
                    
               

        // update the projectiles for each actor who are able to create one: player, dark mage and the Boss
        for (int i = 0; i < Game1.characters.Count; i++)
        {
            if (Game1.characters[i] is DarkMage)
            {
                if (((DarkMage)Game1.characters[i]).miasmaFire != null)
                {
                    ((DarkMage)Game1.characters[i]).miasmaFire.ProjectileUpdate(gameTime);
                }
            }

            if (Game1.characters[i] is Player)
            {
                if (((Player)Game1.characters[i]).fireBall != null)
                {
                    ((Player)Game1.characters[i]).fireBall.ProjectileUpdate(gameTime);
                }
            }

            if (Game1.characters[i] is Boss) 
            {
                if (((Boss)Game1.characters[i]).VenomBreath != null) 
                {
                    ((Boss)Game1.characters[i]).VenomBreath.ProjectileUpdate(gameTime); 
                }
            }
        }

        //Adds coins to player whenever an enemy dies
        for(int i = Game1.characters.Count -1; i >= 0; i--)
        {
            if (Game1.characters[i]._healthSystem.health <= 0)
            {
                if(!(Game1.characters[i] is Player) && Game1.characters[0] is Player)
                {
                    ((Player)Game1.characters[0]).AddCoins(2);
                    Game1.enemiesDefeated++;

                    if(Game1.enemiesDefeated >= 5)
                    {
                        Game1.Quests.Find(q => q.Type == Quest.QuestType.Beat5Enemies).Cleared = true;
                    }

                    if (Game1.characters[i] is Boss)
                    {
                        Game1.bossDefeated = true;
                        Game1.Quests.Find(q => q.Type == Quest.QuestType.BeatBoss).Cleared = true;
                    }
                }
                Game1.characters.RemoveAt(i);
            }
        }

        // Check if the projectiles have collided
        for (int i = 0; i < Game1.projectiles.Count; i++)
        {
            if (Game1.projectiles[i].hit)
            {
                Game1.projectiles.Remove(Game1.projectiles[i]);
            }
        }

        // the logic of telling which phase is: Player or Enemies
        if (Game1.characters[0] is Player)  // The player is suppose to be in index 0, this conditional is for precaution
        {
            if (Game1.characters[0]._healthSystem.life > 0)
            {
                if (((Player)Game1.characters[0]).turn && !((Player)Game1.characters[0]).hasMoved)
                    Game1.whosTurn = "Player Turn";
                else
                    Game1.whosTurn = "Enemies Turn";

            }

            // This update the items on the map. Only the player can pick it up
            for (int i = 0; i < Game1.itemsOnMap.Count; i++)
            {
                if (Game1.itemsOnMap[i].isUsed)
                    Game1.itemsOnMap.Remove(Game1.itemsOnMap[i]);
            }

            // This open the door 
            if (((Player)Game1.characters[0]).levelComplition && !openDoor)
            {
                openDoor = true;
                Game1.tileMap.OpenDoor();
            }

            // When the player enters into the door, go to the next level
            if (((Player)Game1.characters[0]).goToNextLevel && Game1.currentScene + 1 <= Game1.maxNumLevel)
            {
                ((Player)Game1.characters[0]).goToNextLevel = false;
                openDoor = false;
                goToNextLevel(Game1.currentScene + 1);
                changeNextScene();
            }
        }        
        
    }

    public override void DrawScene(GameTime gameTime, SpriteBatch _spriteBatch)
    {
        // Draw the tilemap
        for (int i = 0; i < 25; i++)
        {
            for (int j = 0 + 5; j < 10 + 5; j++)
            {
                Game1.tileMap.getTheIndexes(Game1.tileMap.MapToChar(Game1.tileMap.multidimensionalMap, i, j - 5));
                _spriteBatch.Draw(Game1.mapTexture, new Rectangle(i * Game1.tileSize * 2, j * Game1.tileSize * 2, Game1.tileSize * 2, Game1.tileSize * 2), new Rectangle(Game1.tileMap.horizontalIndex * Game1.tileSize, Game1.tileMap.verticalIndex * Game1.tileSize, Game1.tileSize, Game1.tileSize), Color.White);
            }
        }


        // Draw all the items that are in the tilemap
        for (int i = 0; i < Game1.itemsOnMap.Count; i++)
        {
            Game1.itemsOnMap[i].DrawItem(_spriteBatch);
        }

        // Draw the player
        if (Game1.characters[0] is Player)
        {
            if (Game1.characters[0]._healthSystem.life > 0)
            {
                _spriteBatch.Draw(Game1.mapTexture, new Rectangle(Game1.characters[0].tilemap_PosX * Game1.tileSize * 2, (Game1.characters[0].tilemap_PosY + 5) * Game1.tileSize * 2, Game1.tileSize * 2, Game1.tileSize * 2), new Rectangle(Game1.characters[0].cropPositionX * Game1.tileSize, Game1.characters[0].cropPositionY * Game1.tileSize, Game1.tileSize, Game1.tileSize), Game1.characters[0].AColor);
                Game1.characters[0].DrawStats(_spriteBatch, 1, 0);

                if (((Player)Game1.characters[0]).levelComplition)
                    _spriteBatch.DrawString(Game1.mySpriteFont, "Level Completed", new Vector2(300, 0), Color.White);
            }

        }
        else
        {            
            goToGameOver();     // when the player's life reach to 0, is defeated so is game over
        }

        // Draw the enemy           

        for (int i = 1; i < Game1.characters.Count; i++)
        {
            if (Game1.characters[i]._healthSystem.life > 0 && Game1.characters[i] is Enemy)
                _spriteBatch.Draw(Game1.mapTexture, new Rectangle(Game1.characters[i].tilemap_PosX * Game1.tileSize * 2, (Game1.characters[i].tilemap_PosY + 5) * Game1.tileSize * 2, Game1.tileSize * 2, Game1.tileSize * 2), new Rectangle(Game1.characters[i].cropPositionX * Game1.tileSize, Game1.characters[i].cropPositionY * Game1.tileSize, Game1.tileSize, Game1.tileSize), Game1.characters[i].AColor);
        }

        _spriteBatch.DrawString(Game1.mySpriteFont, Game1.whosTurn, new Vector2(300f, 85f), Color.White);

        //Quest HUD
        int qY = 0;
        _spriteBatch.DrawString(Game1.mySpriteFont, "Quests:", new Vector2(280, qY), Color.Yellow);
        qY += 20;

        foreach (var quest in Game1.Quests)  //sets up quests
        {
            _spriteBatch.DrawString(
                Game1.mySpriteFont,
                $"{quest.Name}: {quest.Status}",
                new Vector2(280, qY),
                quest.Cleared ? Color.LightGreen : Color.White);
            qY += 20;
        }


        // Draw the projectiles if exits. Only player, dark mages and the boss can create projectiles, so it check only those three actors
        for (int i = 0; i < Game1.characters.Count; i++)
        {
            if (Game1.characters[i] is DarkMage)
            {
                if (((DarkMage)Game1.characters[i]).miasmaFire != null)
                    _spriteBatch.Draw(Game1.mapTexture, new Rectangle(((DarkMage)Game1.characters[i]).miasmaFire.X * Game1.tileSize * 2, (((DarkMage)Game1.characters[i]).miasmaFire.Y + 5) * Game1.tileSize * 2, Game1.tileSize * 2, Game1.tileSize * 2), new Rectangle(((DarkMage)Game1.characters[i]).miasmaFire.cropX * Game1.tileSize, ((DarkMage)Game1.characters[i]).miasmaFire.cropY * Game1.tileSize, Game1.tileSize, Game1.tileSize), ((DarkMage)Game1.characters[i]).miasmaFire.pColor);
            }

            if (Game1.characters[i] is Player)
            {
                if (((Player)Game1.characters[i]).fireBall != null)
                {
                    _spriteBatch.Draw(Game1.mapTexture, new Rectangle(((Player)Game1.characters[i]).fireBall.X * Game1.tileSize * 2, (((Player)Game1.characters[i]).fireBall.Y + 5) * Game1.tileSize * 2, Game1.tileSize * 2, Game1.tileSize * 2), new Rectangle(((Player)Game1.characters[i]).fireBall.cropX * Game1.tileSize, ((Player)Game1.characters[i]).fireBall.cropY * Game1.tileSize, Game1.tileSize, Game1.tileSize), ((Player)Game1.characters[i]).fireBall.pColor);
                }
            }

            if (Game1.characters[i] is Boss) 
            {
                if (((Boss)Game1.characters[i]).VenomBreath != null) 
                {
                    _spriteBatch.Draw(Game1.mapTexture, new Rectangle(((Boss)Game1.characters[i]).VenomBreath.X * Game1.tileSize * 2, (((Boss)Game1.characters[i]).VenomBreath.Y + 5) * Game1.tileSize * 2, Game1.tileSize * 2, Game1.tileSize * 2), new Rectangle(((Boss)Game1.characters[i]).VenomBreath.cropX * Game1.tileSize, ((Boss)Game1.characters[i]).VenomBreath.cropY * Game1.tileSize, Game1.tileSize, Game1.tileSize), ((Boss)Game1.characters[i]).VenomBreath.pColor);
                }
            }
        }

        // Draw the enemies stats 
        for (int i = 1; i < Game1.characters.Count; i++)
        {
            if (Game1.characters[i] is Enemy)
                ((Enemy)Game1.characters[i]).DrawStats(_spriteBatch, i, (i - 1) * 40);
        }

        if(numberOfLevel!=4)
        _spriteBatch.DrawString(Game1.mySpriteFont, "Level " + numberOfLevel, new Vector2(300, 120), Color.White);  // This show in which level the player is
        else
            _spriteBatch.DrawString(Game1.mySpriteFont, "Boss Level", new Vector2(300, 120), Color.White);  // The four level is the boss level


    }
      
     

    public void goToNextLevel(int numLvl) 
    {
        //This initialize the corresponding level when the player jumps into it

        switch (numLvl) 
        {
            case 2:
                goToSecondLevel();
                break;
            case 3: 
                goToThirdLevel();
                break;
            case 4:
                goToFourLevel();
                break;
            default:
                Debug.Write("There is no level yet");
                break; 
        }
    }
      
        
}


