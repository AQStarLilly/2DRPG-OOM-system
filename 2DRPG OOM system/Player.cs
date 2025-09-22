using _2DRPG_OOM_system;
using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System.Diagnostics;



public class Player : Actor
{
    public int coins; 

    public Player(int iPosX, int iPosY)
    {
        _healthSystem.health = 15;  
        _healthSystem.power = 1;
        _healthSystem.shield = 3;  
        _healthSystem.life = 3;   
        tilemap_PosX = iPosX;
        tilemap_PosY = iPosY;
        _healthSystem.isStunned = false;
        active = true;
        turn = true;
        waitingPhase = false;
        _healthSystem.status = "Normal";
        _healthSystem.setMaxHP(15);
        _healthSystem.setMaxShield(3);
        keyPress = false;
        cropPositionX = 1;
        cropPositionY = 8; 
        AColor = Color.White;
        _healthSystem.invincibility = false;
        playerInventory.SetInventorySlots(5); 
        shot = false;
        coins = 0;
        resetInventory();         
    }

    public void AddCoins(int amount)
    {
        coins = Math.Max(0, coins + amount);
    }

    private KeyboardState oldState;     
    public bool keyPress;
    public Inventory playerInventory = new Inventory();     
    public Projectile fireBall = null;
    public bool shot; 
    public bool levelComplition;  // Its true when the current level is completed
    public bool goToNextLevel; // this allows the player to go to the next level
    public bool aimingMode; // This allow the player to shoot 
   

    public override void TurnUpdate(GameTime gameTime)
    {
        KeyboardState keyboardState = Keyboard.GetState();

        // visualization if the player has been damaged
        if (isDamage) 
        {             
            damageTiming(0.25f, gameTime); 
        }

        if (turn & !hasMoved)
        {
            if (active && !shot) 
            {
                moveDir = new Vector2(0, 0); 
                _healthSystem.defaultStatus();

                if (keyboardState.IsKeyDown(Keys.A))
                {
                    if (!oldState.IsKeyDown(Keys.A))
                    {
                        if (!aimingMode)
                            PlayerMovement(-1, 0);  // if is not in aiming mode, move normally
                        else
                            ShootFireBall(-1, 0);  // shoot the fire ball in the left direction
                    }
                    
                }
                else if (keyboardState.IsKeyDown(Keys.D))
                {
                    if (!oldState.IsKeyDown(Keys.D))
                    {
                        if (!aimingMode)
                            PlayerMovement(1, 0);  // if is not in aiming mode, move normally
                        else
                            ShootFireBall(1, 0);  // shoot the fire ball in the right direction
                    }
                }
                else if (keyboardState.IsKeyDown(Keys.W))
                {
                    if (!oldState.IsKeyDown(Keys.W))
                    {
                        if (!aimingMode)
                            PlayerMovement(0, -1);  // if is not in aiming mode, move normally
                        else
                            ShootFireBall(0, -1);  // shoot the fire ball in the up direction
                    }
                        
                    
                }
                else if (keyboardState.IsKeyDown(Keys.S))
                {
                    if (!oldState.IsKeyDown(Keys.S))
                    {
                        if(!aimingMode)
                        PlayerMovement(0, 1);  // if is not in aiming mode, move normally
                        else
                            ShootFireBall(0,1);  // shoot the fire ball in the down direction
                    }
                 
                }
                else if (keyboardState.IsKeyDown(Keys.D1)) 
                {
                    if (!oldState.IsKeyDown(Keys.D1)) 
                    {
                        // using the item in the first slot
                        consumeItem(0);
                    }
                }
                else if (keyboardState.IsKeyDown(Keys.D2))
                {
                    if (!oldState.IsKeyDown(Keys.D2))
                    {
                        // using the item in the second slot
                        consumeItem(1);
                    }
                }
                else if (keyboardState.IsKeyDown(Keys.D3))
                {
                    if (!oldState.IsKeyDown(Keys.D3))
                    {
                        // using the item in the third slot
                        consumeItem(2);
                    }
                }
                else if (keyboardState.IsKeyDown(Keys.D4))
                {
                    if (!oldState.IsKeyDown(Keys.D4))
                    {
                        // using the item in the fourth slot
                        consumeItem(3);
                    }
                }
                else if (keyboardState.IsKeyDown(Keys.D5))
                {
                    if (!oldState.IsKeyDown(Keys.D5))
                    {
                        // using the item in the fifth slot
                        consumeItem(4);
                    }
                }

            }          

            
            if (keyPress) 
            {
                if(CheckForUnWalkable((int)moveDir.X, (int)moveDir.Y))
                {
                    // if the player collides with a wall or a not walkable tile, the player won't move. 
                    moveDir = new Vector2(0, 0);                   
                    
                }
                else 
                {
                    for (int i = 1; i < Game1.characters.Count; i++)
                    {
                        // check if the player moves towards an enemy, so it inflict damage instead of moving
                        if (CheckForObjCollision(tilemap_PosX + (int)moveDir.X, tilemap_PosY + (int)moveDir.Y, Game1.characters[i].tilemap_PosX, Game1.characters[i].tilemap_PosY))
                        {
                            // Ghosts don't take damage in this way
                            if (!(Game1.characters[i] is Ghost))
                            {
                                Game1.characters[i]._healthSystem.TakeDamage(_healthSystem.power);
                                Game1.characters[i].damageVisualization(_healthSystem.power); 
                            }

                            moveDir = new Vector2(0, 0);
                        }
                    }

                    // Check if the player collides with a pickup item
                    for(int i = 0; i < Game1.itemsOnMap.Count; i++) 
                    {
                        // Check if the player moves towards a pickup item, if the player's inventory is full, the player won't be able to 
                        // pick up the item.
                        if(CheckForObjCollision(tilemap_PosX + (int)moveDir.X, tilemap_PosY + (int)moveDir.Y, (int)Game1.itemsOnMap[i].itemPosition.X, (int)Game1.itemsOnMap[i].itemPosition.Y) 
                            && playerInventory.inventory.Count < playerInventory.inventorySlots) 
                        {
                            pickItem(Game1.itemsOnMap[i]); 
                        }
                    }

                    if (!shot && !goToNextLevel)
                    {
                        Movement((int)moveDir.X, (int)moveDir.Y);
                        FinishTurn();
                    } 
                }
                keyPress = false;               

            }

            if (shot)
            {
                if (fireBall.hit)
                {
                    shot = false;
                    fireBall = null;
                    aimingMode = false;
                    FinishTurn();
                }
            }

            
        }        


        if (waitingPhase) 
        {
            // The turn ends, so this enable the change of turn transition 
            waitingTurnToFinish(2f, gameTime);
        }

        // Here it checks if the player collides with the door and the level is complited. If that's the case, it generates a new map
        if (levelComplition && Game1.characters[0] is Player && checkingForCollision(Game1.tileMap, '@', this, 0, 0)) 
        {
            goToNextLevel = true;
            levelComplition = false; 
        }
            

        oldState = keyboardState;

        if (!levelComplition)
        {
            levelComplition = true;
            for (int i = 0; i < Game1.characters.Count; i++)
            {
                // This check if the list of characters consist only in Player and ghost. If that's the case, the level is completed
                if (!(Game1.characters[i] is Player || Game1.characters[i] is Ghost))
                {
                    levelComplition = false; 
                }
            }
            if (levelComplition)
                Debug.Print("Level Completed!"); 
        }

    }

    public override void DrawStats(SpriteBatch _spriteBatch, int num, int posY) 
    {
        // UI for the player's stats
        _spriteBatch.DrawString(Game1.mySpriteFont, "Player: ", new Vector2(0, posY), Color.White);
        _spriteBatch.DrawString(Game1.mySpriteFont, "HP: " + _healthSystem.health, new Vector2(125, posY), Color.White);
        _spriteBatch.DrawString(Game1.mySpriteFont, "Shield: " + _healthSystem.shield, new Vector2(0, posY + 25), Color.White);
        _spriteBatch.DrawString(Game1.mySpriteFont, "Lives: " + _healthSystem.life, new Vector2(125, posY +25), Color.White);
        _spriteBatch.DrawString(Game1.mySpriteFont, "Coins: " + coins, new Vector2(125, posY + 50), Color.White);
        _spriteBatch.DrawString(Game1.mySpriteFont, "Inventory", new Vector2(0, posY + 50), Color.White);
        _spriteBatch.DrawString(Game1.mySpriteFont, feedback, new Vector2(tilemap_PosX * Game1.tileSize * 2 - 5, ((tilemap_PosY + 5) * Game1.tileSize * 2) - 25), Color.White); 

        // The UI for the inventory
        for(int i = 0; i < 5; i++) 
        {
            _spriteBatch.DrawString(Game1.mySpriteFont, (i+1).ToString(), new Vector2(12 + 25 * i, posY + 75),  Color.White);            
            _spriteBatch.Draw(Game1.mapTexture, new Rectangle(i * 25, posY + 100, Game1.tileSize * 2, Game1.tileSize * 2), new Rectangle(0, 0, Game1.tileSize, Game1.tileSize), Color.White);
            _spriteBatch.DrawString(Game1.mySpriteFont, "|", new Vector2(i * 25 + 2, posY + 100), Color.White);
        }

        _spriteBatch.DrawString(Game1.mySpriteFont, "|", new Vector2(127, posY + 100), Color.White);

        //The items from the inventory are drawn 
        for (int i = 0; i < playerInventory.inventory.Count; i++) 
        {
            _spriteBatch.Draw(Game1.mapTexture, new Rectangle(i * 25, posY + 100, Game1.tileSize * 2, Game1.tileSize * 2), new Rectangle(playerInventory.inventory[i].cropPosX * Game1.tileSize, playerInventory.inventory[i].cropPosY * Game1.tileSize, Game1.tileSize, Game1.tileSize) , Color.White);
        }
    }

    public void pickItem(Item _item) 
    {
        // The player pick the item, remove the item from map and place it in its inventory
        playerInventory.inventory.Add(_item); 
        Game1.itemsOnMap.Remove(_item);
        _item.isPickUp = true; 
    }        

    private void consumeItem(int iIndex) 
    {
        // the player use the item

        if (playerInventory.inventory.Count > iIndex && playerInventory.inventory[iIndex] != null)
        {
            playerInventory.inventory[iIndex].itemEffect();
            if (playerInventory.inventory[iIndex].isUsed)
                playerInventory.inventory.Remove(playerInventory.inventory[iIndex]);

            if(!shot && !aimingMode)
            FinishTurn();

            keyPress = false;
        }
    }

    private void resetInventory() 
    {
        playerInventory.inventory.Clear();
    }

    private void PlayerMovement(int dx, int dy) 
    {
        moveDir = new Vector2(dx, dy);  // Vector for moving right
        facingDir = moveDir;    // the direction the player will be facing (Right)
        keyPress = true;         // A button has been pressed
    }

    private void ShootFireBall(int dx, int dy) 
    {
        fireBall = new FireBall(new Vector2(tilemap_PosX, tilemap_PosY), new Vector2(dx, dy), 5, Color.Red);
        fireBall.isFromPlayer = true;
        shot = true;
    }

}

