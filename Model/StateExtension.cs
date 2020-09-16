using System.Collections.Generic;

namespace CheckersIA.Model
{
    public static class StateExtension
    {
        public static IEnumerable<State> Next(this State st, bool whiteplay = true)
        {
            Piece basic = whiteplay ? Piece.White : Piece.Black,
                  enemy = whiteplay ? Piece.Black : Piece.White,
                  enemychecker = whiteplay ? Piece.BlackChecker : Piece.WhiteChecker,
                  checker = whiteplay ? Piece.WhiteChecker : Piece.BlackChecker;
            List<State> capturelist = new List<State>();
            int maxcapturecount = 0, oldmcpt;
            for (int j = 0; j < 32; j++)
            {
                if (st[j] == basic)
                {
                    oldmcpt = maxcapturecount;
                    var list = captureBasicNext(st, enemy, enemychecker, j, ref maxcapturecount);
                    if (maxcapturecount > oldmcpt)
                        capturelist.Clear();
                    capturelist.AddRange(list);
                }
                else if (st[j] == checker)
                {
                    oldmcpt = maxcapturecount;
                    var list = captureCheckerNext(st, enemy, enemychecker, j, ref maxcapturecount);
                    if (maxcapturecount > oldmcpt)
                        capturelist.Clear();
                    capturelist.AddRange(list);
                }
            }
            if (maxcapturecount == 0)
            {
                return moveNext(st, whiteplay, basic, checker);
            }
            else
            {
                return capturelist;
            }
        }

        //move pieces
        private static List<State> moveNext(State st, bool whiteplay, Piece basic, Piece checker)
        {
            List<State> list = new List<State>();
            bool left;
            int leftp, rightp;
            State copy;
            for (int j = 0; j < 32; j++)
            {
                if (st[j] == basic)
                {
                    left = (j / 4) % 2 == 0;
                    leftp = j + 4 - (left ? 1 : 0) - (whiteplay ? 0 : 8);
                    rightp = j + 5 - (left ? 1 : 0) - (whiteplay ? 0 : 8);
                    if (leftp > -1 && leftp < 32 && st[leftp] == Piece.Empty)
                    {
                        copy = st.Copy();
                        copy[leftp] = basic;
                        copy[j] = Piece.Empty;
                        list.Add(copy);
                    }
                    if (rightp > -1 && rightp < 32 && st[rightp] == Piece.Empty)
                    {
                        copy = st.Copy();
                        copy[rightp] = basic;
                        copy[j] = Piece.Empty;
                        list.Add(copy);
                    }
                }
                else if (st[j] == checker)
                {
                    //Top
                    left = (j / 4) % 2 == 0;
                    leftp = j + 4 - (left ? 1 : 0) - (whiteplay ? 0 : 8);
                    while (leftp > -1 && leftp < 32 && st[leftp] == Piece.Empty)
                    {
                        copy = st.Copy();
                        copy[leftp] = basic;
                        copy[j] = Piece.Empty;
                        list.Add(copy);
                        
                        leftp += (left ? leftp + 3 : leftp + 4);
                        left = !left;
                    }
                    
                    //Bottom
                    left = (j / 4) % 2 == 0;
                    leftp = j + 4 - (left ? 1 : 0) - (whiteplay ? 0 : 8);
                    while (leftp > -1 && leftp < 32 && st[leftp] == Piece.Empty)
                    {
                        copy = st.Copy();
                        copy[leftp] = basic;
                        copy[j] = Piece.Empty;
                        list.Add(copy);
                        
                        leftp += (left ? leftp - 4 : leftp - 3);
                        left = !left;
                    }

                    //Top
                    left = (j / 4) % 2 == 0;
                    rightp = j + 4 - (left ? 1 : 0) - (whiteplay ? 0 : 8);
                    while (rightp > -1 && rightp < 32 && st[rightp] == Piece.Empty)
                    {
                        copy = st.Copy();
                        copy[rightp] = basic;
                        copy[j] = Piece.Empty;
                        list.Add(copy);
                        
                        rightp += (left ? rightp + 4 : rightp + 5);
                        left = !left;
                    }

                    //Bottom
                    left = (j / 4) % 2 == 0;
                    rightp = j + 4 - (left ? 1 : 0) - (whiteplay ? 0 : 8);
                    while (rightp > -1 && rightp < 32 && st[rightp] == Piece.Empty)
                    {
                        copy = st.Copy();
                        copy[rightp] = basic;
                        copy[j] = Piece.Empty;
                        list.Add(copy);
                        
                        rightp += (left ? rightp - 4 : rightp - 3);
                        left = !left;
                    }
                }
            }
            return list;
        }

        //capture for non-checker pieces
        private static List<State> captureBasicNext(State st, Piece enemy, Piece enemychecker, int p,
                                            ref int maxcapturecount)
        {
            var list = new List<State>();

            var stack = new Stack<(State, int, int, int)>();
            State copy = st.Copy(), crr;
            stack.Push((copy, 4, 0, p));
            stack.Push((copy, 3, 0, p));
            stack.Push((copy, 2, 0, p));
            stack.Push((copy, 1, 0, p));

            int dir, targetp, newp, capturecount;
            bool left;
            while (stack.Count > 0)
            {
                //Execute a play
                (crr, dir, capturecount, p) = stack.Pop();
                left = (p / 4) % 2 == 0;
                if (dir == 4 && p % 4 < 3 && p < 24) //Move Right-Top
                {
                    targetp = left ? p + 4 : p + 5;
                    newp = p + 9;
                }
                else if (dir == 3 && p % 4 > 0 && p < 24) //Move Left-Top
                {
                    targetp = left ? p + 3 : p + 4;
                    newp = p + 7;
                }
                else if (dir == 2 && p % 4 < 3 && p > 7) //Move Right-Bottom
                {
                    targetp = left ? p - 4 : p - 3;
                    newp = p - 7 ;
                }
                else if (dir == 1 && p % 4 > 0 && p > 7) //Move Left-Bottom
                {
                    targetp = left ? p - 5 : p - 4;
                    newp = p - 9;
                }
                else newp = targetp = -1;
                //can capture
                if (newp < 32 && newp > -1 && crr[newp] == Piece.Empty 
                    && (crr[targetp] == enemy || crr[targetp] == enemychecker))
                {
                    copy = crr.Copy();
                    copy[newp] = copy[p];
                    copy[p] = copy[targetp] = Piece.Empty;
                    stack.Push((copy, 4, capturecount + 1, newp));
                    stack.Push((copy, 3, capturecount + 1, newp));
                    stack.Push((copy, 2, capturecount + 1, newp));
                    stack.Push((copy, 1, capturecount + 1, newp));
                }
                else //cannot capture (end move)
                {
                    //best play
                    if (capturecount > maxcapturecount)
                    {
                        list.Clear();
                        maxcapturecount = capturecount;
                        list.Add(crr);
                    }
                    //in best play collection
                    else if (capturecount == maxcapturecount)
                        list.Add(crr);
                }
            }
            return list;
        }

        //capture for checker pieces
        private static List<State> captureCheckerNext(State st, Piece enemy, Piece enemychecker, int p,
                                            ref int maxcapturecount)
        {
            var list = new List<State>();

            var stack = new Stack<(State, int, int, int)>();
            State copy = st.Copy(), crr;
            stack.Push((copy, 4, 0, p));
            stack.Push((copy, 3, 0, p));
            stack.Push((copy, 2, 0, p));
            stack.Push((copy, 1, 0, p));

            int dir, targetp, newp, capturecount;
            bool left;
            while (stack.Count > 0)
            {
                //Execute a play
                (crr, dir, capturecount, p) = stack.Pop();
                left = (p / 4) % 2 == 0;
                targetp = newp = p;
                if (dir == 4  && p % 4 < 3 && p < 24) //Move Right-Top
                {
                    do
                    {
                        targetp = left ? targetp + 4 : targetp + 5;
                        newp = newp + 9;
                        left = !left;
                    } while(crr[targetp] == Piece.Empty);
                }
                else if (dir == 3 && p % 4 > 0 && p < 24) //Move Left-Top
                {
                    do
                    {
                        targetp = left ? targetp + 3 : targetp + 4;
                        newp = newp + 7;
                        left = !left;
                    } while(crr[targetp] == Piece.Empty);
                }
                else if (dir == 2 && p % 4 < 3 && p > 7) //Move Right-Bottom
                {
                    do
                    {
                        targetp = left ? targetp - 4 : targetp - 3;
                        newp = newp - 7 ;
                        left = !left;
                    } while(crr[targetp] == Piece.Empty);
                }
                else if (dir == 1  && p % 4 > 0 && p > 7)//Move Right-Top
                {
                    do
                    {
                        targetp = left ? targetp - 5 : targetp - 4;
                        newp = newp - 9;
                        left = !left;
                    } while(crr[targetp] == Piece.Empty);
                }
                else newp = targetp = -1;
                //can capture
                if (newp < 32 && newp > -1 && crr[newp] == Piece.Empty 
                    && (crr[targetp] == enemy || crr[targetp] == enemychecker))
                {
                    copy = crr.Copy();
                    copy[newp] = copy[p];
                    copy[p] = copy[targetp] = Piece.Empty;
                    stack.Push((copy, 4, capturecount + 1, newp));
                    stack.Push((copy, 3, capturecount + 1, newp));
                    stack.Push((copy, 2, capturecount + 1, newp));
                    stack.Push((copy, 1, capturecount + 1, newp));
                }
                else //cannot capture (end move)
                {
                    //best play
                    if (capturecount > maxcapturecount)
                    {
                        list.Clear();
                        maxcapturecount = capturecount;
                        list.Add(crr);
                    }
                    //in best play collection
                    else if (capturecount == maxcapturecount)
                        list.Add(crr);
                }
            }
            return list;
        }
    }
}