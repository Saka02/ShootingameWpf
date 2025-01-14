﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Windows.Threading;

namespace Shootingame
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        DispatcherTimer gameTimer = new DispatcherTimer();
        bool moveLeft,moveRight;
        List<Rectangle> itemRemover = new List<Rectangle>();
        Random rand = new Random();

        int enemySpiriteCounted = 0;
        int enemyCounted = 100;
        int playerSpeed = 10;
        int enemySpeed = 10;
        int limit = 50;
        int score = 0;
        int damage = 0;
        Rect playerHitBox;

        public MainWindow()
        {
            InitializeComponent();
            gameTimer.Interval = TimeSpan.FromMilliseconds(20);
            gameTimer.Tick += GameLoop;
            gameTimer.Start();
            MyCanvas.Focus();

            ImageBrush bg = new ImageBrush();
            bg.ImageSource = new BitmapImage(new Uri("pack://application:,,,/images/purple.png"));
            bg.TileMode = TileMode.Tile;
            bg.Viewport = new Rect(0,0,0.15,0.15);
            bg.ViewportUnits = BrushMappingMode.RelativeToBoundingBox;
            MyCanvas.Background = bg;
            
            ImageBrush playerImage = new ImageBrush();
            playerImage.ImageSource = new BitmapImage(new Uri("pack://application:,,,/images/player.png"));
            player.Fill = playerImage;
                  }

        private void GameLoop(object sender, EventArgs e)
        {
           playerHitBox = new Rect(Canvas.GetLeft(player),Canvas.GetTop(player),player.Width,player.Height);
            enemyCounted -= 1;
            scoreText.Content = "Score =" + score;
            damageText.Content = "Damage =" + damage;

            if(enemyCounted < 0)
            {
                MakeEnemies();
                enemyCounted = limit;
            }

            if(moveLeft == true && Canvas.GetLeft(player)>0)
            {
                Canvas.SetLeft(player, Canvas.GetLeft(player) - playerSpeed);
            }
            if (moveRight == true && Canvas.GetLeft(player) + 90 < Application.Current.MainWindow.Width)
            {
                Canvas.SetLeft(player, Canvas.GetLeft(player) + playerSpeed);
            }

            foreach( var x in MyCanvas.Children.OfType<Rectangle>())
            {
                if(x is Rectangle && (string)x.Tag == "bullet")
                {
                    Canvas.SetTop(x,Canvas.GetTop(x)-20);
                    Rect bulletHitBox = new Rect(Canvas.GetLeft(x),Canvas.GetTop(x),x.Width,x.Height);
                    if(Canvas.GetTop(x)<10 )
                    {
                        itemRemover.Add(x);
                    }

                    foreach( var y in MyCanvas.Children.OfType<Rectangle>())
                    {
                        if(y is Rectangle && (string)y.Tag == "enemy")
                        {
                            Rect enemyBox = new Rect(Canvas.GetLeft(y), Canvas.GetTop(y), y.Width, y.Height);
                            if (bulletHitBox.IntersectsWith(enemyBox))
                            {
                                itemRemover.Add(y);
                                itemRemover.Add(x);
                                score ++;
                            }
                        }
                    }
                  
                }
                if (x is Rectangle && (string)x.Tag == "enemy")
                {
                    Canvas.SetTop(x, Canvas.GetTop(x) + enemySpeed);
                    if (Canvas.GetTop(x) >750)
                    {
                        itemRemover.Add(x);
                        damage += 10;
                    }
                    Rect enemyHitBox = new Rect(Canvas.GetLeft(x),Canvas.GetTop(x),x.Width,x.Height);
                    if (playerHitBox.IntersectsWith(enemyHitBox))
                    {
                        itemRemover.Add(x);
                        damage += 5;
                    }
                    
                }
            }
            
            foreach(Rectangle i in itemRemover)
            {
                MyCanvas.Children.Remove(i);    
            }

            if (score > 5)
            {
                limit = 20;
                enemySpeed = 25;
                playerSpeed = 25;
            }
            if (damage > 99)
            {
                gameTimer.Stop();
                damageText.Content = 100;
                damageText.Foreground = Brushes.Red;
                MessageBox.Show("Kapetane ubili ste " + score + "vanzemaljskih brodova " + Environment.NewLine + "Pritisni U redu da bi igra pocela ponovo");
                System.Diagnostics.Process.Start(Application.ResourceAssembly.Location);
                Application.Current.Shutdown();

            }

        }

        private void OnKeyDown(object sender, KeyEventArgs e)
        {
            if(e.Key == Key.Left)
            {
                moveLeft = true;
            }
            if(e.Key == Key.Right)
            {
                moveRight = true;
            }
        }

        private void OnKeyUp(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Left)
            {
                moveLeft = false;
            }
            if (e.Key == Key.Right)
            {
                moveRight = false;
            }
            if(e.Key == Key.Space)
            {
                Rectangle newBullet = new Rectangle
                {
                    Tag = "bullet",
                    Height = 20,
                    Width = 5,
                    Fill = Brushes.White,
                    Stroke = Brushes.Red

                }; 
                Canvas.SetLeft(newBullet, Canvas.GetLeft(player) + player.Width / 2);
                Canvas.SetTop(newBullet, Canvas.GetTop(player) - newBullet.Height);
                MyCanvas.Children.Add(newBullet);

            }
        }
        private void MakeEnemies()
        {
            ImageBrush enemySprite = new ImageBrush();
            enemySpiriteCounted = rand.Next(1, 5);
            switch (enemySpiriteCounted)
            {
                case 1:
                    enemySprite.ImageSource = new BitmapImage(new Uri("pack://application:,,,/images/jedan.png"));
                    break;
                case 2:
                    enemySprite.ImageSource = new BitmapImage(new Uri("pack://application:,,,/images/dva.png"));
                    break;
                case 3:
                    enemySprite.ImageSource = new BitmapImage(new Uri("pack://application:,,,/images/dva.png"));
                    break;
                case 4:
                    enemySprite.ImageSource = new BitmapImage(new Uri("pack://application:,,,/images/dva.png"));
                    break;
                case 5:
                    enemySprite.ImageSource = new BitmapImage(new Uri("pack://application:,,,/images/pet.png"));
                    break; 
            }

            Rectangle Newenemy = new Rectangle {
                Tag = "enemy",
                Width = 56,
                Height = 50,
                Fill = enemySprite
            };
            Canvas.SetTop(Newenemy, -100);
            Canvas.SetLeft(Newenemy, rand.Next(30, 430));
            MyCanvas.Children.Add(Newenemy);
        }


    }
}
