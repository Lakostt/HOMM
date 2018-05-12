using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace HOMM
{
    public partial class Form1 : Form
    {
        class Unit
        {
            public int Health { get; protected set; }
            public int Count { get; protected set; }
            public int Damage { get; protected set; }
            public int Speed { get; protected set; }
            public int X { get; protected set; }
            public int Y { get; protected set; }
            public Image Picture { get; protected set; }
            public bool Friend { get; protected set; }
            
            public void ShowMove(PictureBox[,] f)
            {
                for (int i = Y - Speed; i <= Y + Speed; i++)
                {
                    for (int j = X - Speed + Math.Abs(Y - i); j <= X + Speed - Math.Abs(Y-i); j++)
                    {
                        if (j < 0 || j > 9 || i < 0 || i > 7) continue;
                        f[j, i].BackColor = Color.Orange;
                    }
                }
            }
            public void Move(int x, int y, Form1 f)
            {
                f.field[X, Y].BackgroundImage = null;
                f.info[X, Y].Visible = false;
                X = x; Y = y;
                f.field[X, Y].BackgroundImage = Picture;
                f.info[X,Y].Text = Count.ToString();
                f.info[X, Y].Visible = true;
            }
        }

        class Centaur : Unit
        {
            public Centaur(int x, int y, int count, bool friend, Form1 f)
            {
                Health = 10;
                Speed = 3;
                Damage = 4;
                X = x; Y = y;
                Count = count;
                Friend = friend;
                if (Friend) Picture = HOMM.Properties.Resources.centaur;
                else Picture = HOMM.Properties.Resources.centaurEnemy;
                f.info[X, Y].Text = Count.ToString();
                f.info[X, Y].Visible = true;
                f.field[X, Y].BackgroundImage = Picture;
            }
        }

        public Form1()
        {
            InitializeComponent();
        }

        PictureBox[,] field = new PictureBox[10,8];
        Label[,] info = new Label[10, 8];
        byte[,] pathNumber = new byte[10, 8];

        List<Unit> player = new List<Unit>();
        List<Unit> enemy = new List<Unit>();
        List<Vector2> path;

        bool isPlayer = true;
        int turnPlayer = 0;
        int turnEnemy = 0;
        int i = 0;
        // Movement
        int X = 0, Y = 0;
        int tempX = 0, tempY = 0;
        

        private void Form1_Load(object sender, EventArgs e)
        {
            for (int i = 0; i < 8; i++)
            {
                for (int j = 0; j < 10; j++)
                {
                    info[j, i] = new Label();
                    info[j, i].Name = "lab" + j + i;
                    info[j, i].Size = new System.Drawing.Size(50, 25);
                    info[j, i].Top = 80 + 105 * i;
                    info[j, i].Left = 5 + 105 * j;
                    info[j, i].BackColor = Color.Blue;
                    info[j, i].ForeColor = Color.Yellow;
                    info[j, i].Visible = false;
                    this.Controls.Add(info[j, i]); // Добавляет объект на форму
                    field[j, i] = new PictureBox();
                    field[j, i].Name = "pic" + j + i;
                    field[j, i].Size = new System.Drawing.Size(100, 100);
                    field[j, i].Top = 5 + 105 * i;
                    field[j, i].Left = 5 + 105 * j;
                    field[j, i].BackColor = Color.Transparent;
                    field[j, i].BorderStyle = BorderStyle.FixedSingle;
                    field[j, i].BackgroundImageLayout = ImageLayout.Stretch;
                    field[j, i].Click += picClick; // Событие клика
                    this.Controls.Add(field[j, i]);
                }
            }
            // Allies
            player.Add(new Centaur(0, 0, 7, true, this));
            player.Add(new Centaur(5, 2, 7, true, this));
            player.Add(new Centaur(5, 3, 7, true, this));
            // Foes
            enemy.Add(new Centaur(2, 1, 5, false, this));
            enemy.Add(new Centaur(3, 3, 5, false, this));
            enemy.Add(new Centaur(3, 4, 5, false, this));

            ShowMove();
            

        }
        void ShowMove() // Отображение клеток
        {
            if(isPlayer)
            {
                player[turnPlayer].ShowMove(field);
            }
            else
            {
                enemy[turnEnemy].ShowMove(field);
            }
        }
        void UpdatePath()
        {
            for(int y = 0; y < 8; y++)
            {
                for(int x = 0; x < 10; x++)
                {
                    pathNumber[x, y] = 255;
                }
            }
        }
        void FindPath()
        {
            pathNumber[tempX, tempY] = 0;
            path = new List<Vector2>();
            for (int i = 1; ; i++) // Счетчик
            {
                for(int y = 0; y < 8; y++) // Счетчик строчек поля
                {
                    for(int x = 0; x < 10; x++) // Счетчик столбцов поля
                    {
                        if(pathNumber[x,y] == i-1) // Если нашли 0, то идем вокруг него 
                        {
                            for (int b = y - 1; b <= y + 1 ; b++)
                            {
                                for(int a = x - 1; a <= x + 1; a++)
                                {
                                    if (b < 0 || b > 7 || a < 0 || a > 9) continue;
                                    if (field[a, b].BackgroundImage != null) continue;
                                    if (pathNumber[a, b] != 255) continue;

                                    pathNumber[a, b] = 1;
                                }
                            }
                        }
                    }
                }
                if (pathNumber[X, Y] != 255) break;
            }
            timer1.Start();
        }

        void UpdateField()
        {
            foreach (PictureBox p in field)
            {
                p.BackColor = Color.Transparent;
            }
        }

        private void picClick(object sender, EventArgs e)
        {
            PictureBox p = (PictureBox)sender;
            X = int.Parse(p.Name[3].ToString());
            Y = int.Parse(p.Name[4].ToString());
            if (p.BackColor == Color.Orange)
            {
                if(isPlayer)
                {
                    tempX = player[turnPlayer].X;
                    tempY = player[turnPlayer].Y;
                    FindPath();
                }
                else
                {
                    tempX = enemy[turnEnemy].X;
                    tempY = enemy[turnEnemy].Y;
                    FindPath();
                }
                timer1.Start();
                
            }
            else
            {
                MessageBox.Show("Ты не пройдешь!");
            }
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            if(isPlayer)
            {
                Action(player, ref turnPlayer);
            }
            else
            {
                Action(enemy, ref turnEnemy);
            }
        }

        void Action(List<Unit> units, ref int turn)
        {
            units[turn].Move(path[i].X, path[i].Y, this);
            if (i == 0)
            {
                timer1.Stop();
                if (turn < units.Count - 1) turn++;
                else turn = 0;
                isPlayer = !isPlayer;
                UpdateField();
                ShowMove();
            }
            i--;
        }

        class Vector2
        {
            public int X { get; set; }
            public int Y { get; set; }
            public Vector2(int x, int y)
            {
                X = x; Y = y;
            }
        }
        
    }
}