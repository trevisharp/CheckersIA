using System.Windows;
using System.Windows.Forms;
using System.Drawing;
using System.Linq;

namespace CheckersIA.View
{
    using Model;

    public class FrmMain : Form
    {
        Bitmap bmp;
        Graphics g;
        PictureBox pb;
        Timer tm;
        Game game;

        public FrmMain()
        {
            this.WindowState = FormWindowState.Maximized;
            this.FormBorderStyle = FormBorderStyle.Fixed3D;
            this.Text = "Checkers AI";

            pb = new PictureBox();
            pb.Dock = DockStyle.Fill;

            Point? p = null;
            Piece piece = Piece.Empty;
            int origin = -1, target = -1;
            pb.MouseDown += delegate (object sender, MouseEventArgs e)
            {
                p = e.Location;
                origin = (e.Location.X - bmp.Width / 2 + 360) / 180 +
                        4 * (7 - (e.Location.Y - bmp.Height / 2 + 360) / 90);
                piece = game.State[origin];
                game.State[origin] = Piece.Empty;
                
            };
            pb.MouseMove += delegate (object sender, MouseEventArgs e)
            {
                if (!p.HasValue)
                    return;
                p = e.Location;
            };
            pb.MouseUp += delegate (object sender, MouseEventArgs e)
            {
                p = null;
                target = (e.Location.X - bmp.Width / 2 + 360) / 180 +
                        4 * (7 - (e.Location.Y - bmp.Height / 2 + 360) / 90);
                game.State[origin] = piece;
                game.TryMove(origin, target);
                piece = Piece.Empty;
            };
            this.Controls.Add(pb);
            
            tm = new Timer();
            tm.Interval = 20;
            tm.Tick += delegate
            {
                g.Clear(Color.White);
                #if false
                g.DrawString(game.ListMoves().Count().ToString(), this.Font,
                    Brushes.Black, Point.Empty);
                #endif

                Pen piecepen = new Pen(Brushes.Black, 1f);
                Rectangle toprec;

                for (int x = 0; x < 8; x++)
                {
                    for (int y = 0; y < 8; y++)
                    {
                        var rec = new Rectangle(bmp.Width / 2 - 360 + 90 * x,
                                 bmp.Height / 2 + 270 - 90 * y, 90, 90);
                        if ((x + y) % 2 == 0)
                        {
                            g.FillRectangle(Brushes.DarkGreen, rec);
                            g.DrawRectangle(Pens.Black, rec);
                        
                            rec = new Rectangle(rec.X + 10, rec.Y + 10, 70, 70);
                            switch (game.State[x / 2 + 4 * y])
                            {
                                case Piece.White:
                                    g.FillEllipse(Brushes.White, rec);
                                    g.DrawEllipse(piecepen, rec);
                                    break;
                                case Piece.Black:
                                    g.FillEllipse(Brushes.DarkGray, rec);
                                    g.DrawEllipse(piecepen, rec);
                                    break;
                                case Piece.WhiteChecker:
                                    toprec =new Rectangle(rec.X + 5, rec.Y - 5, 70, 70);
                                    g.FillEllipse(Brushes.White, toprec);
                                    g.DrawEllipse(piecepen, toprec);
                                    g.FillEllipse(Brushes.White, rec);
                                    g.DrawEllipse(piecepen, rec);
                                    break;
                                case Piece.BlackChecker:
                                    toprec =new Rectangle(rec.X + 5, rec.Y - 5, 70, 70);
                                    g.FillEllipse(Brushes.DarkGray, toprec);
                                    g.DrawEllipse(piecepen, toprec);
                                    g.FillEllipse(Brushes.DarkGray, rec);
                                    g.DrawEllipse(piecepen, rec);
                                    break;
                            }
                        }
                        else 
                        {
                            g.FillRectangle(Brushes.WhiteSmoke, rec);
                            g.DrawRectangle(Pens.Black, rec);
                        }
                    }
                }

                if (p.HasValue)
                {
                    var currec = new Rectangle(p.Value.X - 35, p.Value.Y - 35, 70, 70);
                    switch (piece)
                    {
                        case Piece.WhiteChecker:
                        case Piece.White:
                            g.FillEllipse(Brushes.White, currec);
                            g.DrawEllipse(piecepen, currec);
                            break;
                        case Piece.BlackChecker:
                        case Piece.Black:
                            g.FillEllipse(Brushes.DarkGray, currec);
                            g.DrawEllipse(piecepen, currec);
                            break;
                    }
                }

                g.DrawRectangle(new Pen(Color.Black, 3f), 
                    new Rectangle(bmp.Width / 2 - 360, bmp.Height / 2 - 360, 720, 720));
                pb.Image = bmp;
            };

            game = new Game();

            this.Load += delegate
            {
                bmp = new Bitmap(pb.Width, pb.Height);
                pb.Image = bmp;
                g = Graphics.FromImage(bmp);
                tm.Start();
            };
        }
    }
}