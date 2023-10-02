using System;
using SFML.Graphics;
using SFML.Learning;
using SFML.Window;

class Program : Game
{
    static string backgroundTexture = LoadTexture("background.png");
    static string playerTexture = LoadTexture("player.png");
    static string foodTexture = LoadTexture("food.png");
    static string chairTexture = LoadTexture("chair.png");

    static string meowSound = LoadSound("meow_sound.wav");
    static string crashSound = LoadSound("cat_crash_sound.wav");
    static string bgMusic = LoadMusic("bg_music.wav");

    static float playerX = 300;
    static float playerY = 220;
    static int playerSize = 56;
    static float playerSpeed = 400;
    static int playerDirection = 1;
    static int playerScore = 0;

    static float foodX;
    static float foodY;
    static int foodSize = 32;

    static float chairX;
    static float chairY;
    static int chairSize = 32;

    static void PlayerMove()
    {
        if (GetKey(Keyboard.Key.W) == true) playerDirection = 0;
        if (GetKey(Keyboard.Key.D) == true) playerDirection = 1;
        if (GetKey(Keyboard.Key.S) == true) playerDirection = 2;
        if (GetKey(Keyboard.Key.A) == true) playerDirection = 3;

        if (playerDirection == 0) playerY -= playerSpeed * DeltaTime;
        if (playerDirection == 1) playerX += playerSpeed * DeltaTime;
        if (playerDirection == 2) playerY += playerSpeed * DeltaTime;
        if (playerDirection == 3) playerX -= playerSpeed * DeltaTime;
    }

    static void DrawPlayer()
    {
        if (playerDirection == 0) DrawSprite(playerTexture, playerX, playerY, 64, 64, playerSize, playerSize);
        if (playerDirection == 1) DrawSprite(playerTexture, playerX, playerY, 0, 0, playerSize, playerSize);
        if (playerDirection == 2) DrawSprite(playerTexture, playerX, playerY, 0, 64, playerSize, playerSize);
        if (playerDirection == 3) DrawSprite(playerTexture, playerX, playerY, 64, 0, playerSize, playerSize);
    }

    static void Main()
    {
        InitWindow(800, 600, "Meow");

        SetFont("comic.ttf");

        Random rndFood = new Random();

        //Генерация еды вне стула.
        do
        {
            foodX = rndFood.Next(0, 800 - foodSize);
            foodY = rndFood.Next(165, 600 - foodSize);
        }
        while (chairX + chairSize > foodX && chairX < foodX + foodSize
      && chairY + chairSize > foodY && chairY < foodY + foodSize);

        Random rndChair = new Random();

        //Генерация стула вне игрока.
        do
        {
            chairX = rndChair.Next(100, 700 - chairSize);
            chairY = rndChair.Next(165, 435 - chairSize);
        }
        while (playerX + playerSize > chairX && playerX < chairX + chairSize
       && playerY + playerSize > chairY && playerY < chairY + chairSize);

        bool isLose = false;
        bool chairCreated = false;

        PlayMusic(bgMusic, 10);

        while (true)
        {
            DispatchEvents();

            if (isLose == false)
            {
                PlayerMove();

                if (playerX + playerSize > foodX && playerX < foodX + foodSize
                   && playerY + playerSize > foodY && playerY < foodY + foodSize)
                {
                    foodX = rndFood.Next(0, 800 - foodSize);
                    foodY = rndFood.Next(165, 600 - foodSize);

                    playerSpeed += 10;
                    playerScore += 1;

                    PlaySound(meowSound, 5);
                }

                if (playerX + playerSize > 800 || playerX < 0 || playerY + playerSize > 600 || playerY < 165)
                {
                    isLose = true;

                    PlaySound(crashSound, 5);

                    chairX = rndChair.Next(100, 700 - chairSize);
                    chairY = rndChair.Next(165, 435 - chairSize);
                }

                //Проверка столкновения со стулом.
                if (playerX + playerSize > chairX && playerX < chairX + chairSize
                   && playerY + playerSize > chairY && playerY < chairY + chairSize)
                {
                    isLose = true;

                    PlaySound(crashSound, 5);

                    chairX = rndChair.Next(100, 700 - chairSize);
                    chairY = rndChair.Next(165, 435 - chairSize);
                }

                //Проверка генерации стула.
                if (playerScore % 2 == 0 && playerScore != 0 && !chairCreated)
                {
                    chairX = rndChair.Next(100, 700 - chairSize);
                    chairY = rndChair.Next(165, 435 - chairSize);
                    chairCreated = true;
                }
                else if (playerScore % 2 != 0)
                {
                    chairCreated = false;
                }
            }

            if (isLose == true)
            {
                if (GetKeyDown(Keyboard.Key.R))
                {
                    isLose = false;
                    playerX = 400;
                    playerY = 300;
                    playerSpeed = 400;
                    playerDirection = 1;
                    playerScore = 0;
                }
            }

            ClearWindow();

            DrawSprite(backgroundTexture, 0, 0);

            if (isLose == true)
            {
                SetFillColor(70, 70, 70);
                DrawText(200, 300, "Вы врезались! Но успели съесть :" + playerScore.ToString(), 24);
                SetFillColor(50, 50, 50);
                DrawText(200, 400, "Нажмите \"R\" чтобы начать заного", 24);
            }

            DrawPlayer();

            DrawSprite(foodTexture, foodX, foodY);

            DrawSprite(chairTexture, chairX, chairY);

            SetFillColor(70, 70, 70);
            DrawText(20, 8, "Съедено корма: " + playerScore.ToString(), 18);

            DisplayWindow();

            Delay(1);
        }
    }
}
