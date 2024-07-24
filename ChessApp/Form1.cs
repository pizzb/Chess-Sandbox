using Microsoft.VisualBasic;
using Microsoft.VisualBasic.CompilerServices;
using System.Runtime.Intrinsics.X86;
using System.Windows.Forms;
using System.IO;
using System.Diagnostics;
using ChessApp.Properties;

namespace ChessApp
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
            textBox1.Text = getFEN();
            saveToolStripMenuItem.Enabled = false;
        }

        private void propertyGrid1_Click(object sender, EventArgs e)
        {

        }


        public int currentSelection = 0;
        public Color darkTileColor = Color.Gray;
        public Color lightTileColor = Color.LightGray;
        public Color selectedTileColor = Color.PaleGreen;
        public int[,] boardData = 
        {
            // Set the starting position



            // 0 = blank

            // 1 = pawn
            // 2 = knight
            // 3 = bishop
            // 4 = rook
            // 5 = queen
            // 6 = king

            // +6 = black's piece

            // 13 = En Passant is available

            {4, 1, 0, 0, 0, 0, 7, 10 }, // a
            {2, 1, 0, 0, 0, 0, 7, 8 }, // b
            {3, 1, 0, 0, 0, 0, 7, 9 }, // c
            {5, 1, 0, 0, 0, 0, 7, 11 }, // d
            {6, 1, 0, 0, 0, 0, 7, 12 }, // e
            {3, 1, 0, 0, 0, 0, 7, 9 }, // f
            {2, 1, 0, 0, 0, 0, 7, 8 }, // g
            {4, 1, 0, 0, 0, 0, 7, 10 }, // h
        };
        public List<string> boardDataHistory = new List<string>();
        public bool isUndo = false;


        public bool rotate = false;
        public bool wqcastle = true;
        public bool wkcastle = true;
        public bool bqcastle = true;
        public bool bkcastle = true;


        public string linkedFile = "0";


        // tileselection
        private string selectTile(int id)
        {
            if (rotate)
            {
                id = 99 - id;
            }


            string stringid = id.ToString();


            // success


            if (currentSelection == 0) // Select if there's no selection
            {
                if (getTile(id) == 13)
                {
                    setTile(id, 0);
                    currentSelection = 0;
                } else
                {
                    if (getTile(id) == 0)
                    {
                        currentSelection = 0;
                    }
                    else
                    {
                        currentSelection = id;
                    }
                }
                
                
            }
            else if (currentSelection == id) // Deselect if clicking on the same place twice
            {
                currentSelection = 0;
            }
            else // Eat
            {

                int currentTile = getTile(currentSelection); // Get current tile

                if (currentSelection >= 1000)
                {
                    if (currentTile == 13)
                    {
                        // Remove all en passants
                        for (int i = 0; i < 8; i++)
                        {
                            for (int j = 0; j < 8; j++)
                            {
                                if (boardData[i, j] == 13)
                                {
                                    boardData[i, j] = 0;
                                }
                            }
                        }
                        setTile(id, currentTile);
                    } else
                    {
                        setTile(id, currentTile);
                    }
                    
                } else
                {
                    if (getTile(currentSelection) == 0)
                    {

                    }
                    else
                    {
                        setTile(currentSelection, 0);

                        setTile(id, currentTile);



                    }
                    currentSelection = 0; // Deselect
                }
                

                
                
            }
            drawBoard();
            textBox1.Text = getFEN();
            
            return "success";

        }

        private string selectSandboxTile(int id)
        {

            // success


            if (currentSelection == id) // Deselect if clicking on the same place twice
            {
                currentSelection = 0;
            } else
            {
                currentSelection = id;
            }
            drawBoard();

            return "success";

        }





        private int getTile(int selectedTileId) // Get Tile from position
        {
            if (selectedTileId >= 1000)
            {
                int selectedpieceId = selectedTileId / 1000;
                if (selectedpieceId == 7)
                {
                    selectedpieceId = 0;
                }
                if (selectedpieceId == 8)
                {
                    selectedpieceId = 13;
                }
                else
                {
                    int isBlackPiece = int.Parse((selectedTileId / 100).ToString()[1].ToString()) - 1;
                    if (selectedpieceId == 0)
                    {
                        isBlackPiece = 0;
                    }
                    selectedpieceId = selectedpieceId + (isBlackPiece * 6);
                }
                return selectedpieceId;
            } else
            {
                int x = (int)Math.Floor((decimal)(selectedTileId / 10)) - 1;
                int y = (selectedTileId - (10 * (int)Math.Floor((decimal)(selectedTileId / 10)))) - 1;
                return boardData[x, y];
            }
        }

        private void setTile(int selectedTileId, int tile) // Set Tile
        {
            int x = (int)Math.Floor((decimal)(selectedTileId / 10)) - 1;
            int y = (selectedTileId - (10 * (int)Math.Floor((decimal)(selectedTileId / 10)))) - 1;
            boardData[x, y] = tile;
        }


        private string getFEN() // Update the generated FEN string.
        {
            // 0 = blank

            // 1 = pawn
            // 2 = knight
            // 3 = bishop
            // 4 = rook
            // 5 = queen
            // 6 = king

            // +6 = black's piece
            string outputString = "";
            int numBlanks = 0;
            for (int j = 7; j >= 0; j--) // j is the numbers
            {
                for (int i = 0; i < boardData.GetLength(0); i++) // i is the letters
                {
                    string stringCurrentTileId = (i + 1).ToString() + (j + 1).ToString();
                    int currenttileId = int.Parse((i + 1).ToString() + (j + 1).ToString());

                    int pieceType = getTile(currenttileId);
                    switch (pieceType)
                    {
                        case 0:
                            numBlanks++;
                            break;
                        case 1:
                            if (numBlanks > 0)
                            {
                                outputString = outputString + numBlanks;
                                numBlanks = 0;
                            }
                            outputString = outputString + "P";
                            break;
                        case 2:
                            if (numBlanks > 0)
                            {
                                outputString = outputString + numBlanks;
                                numBlanks = 0;
                            }
                            outputString = outputString + "N";
                            break;
                        case 3:
                            if (numBlanks > 0)
                            {
                                outputString = outputString + numBlanks;
                                numBlanks = 0;
                            }
                            outputString = outputString + "B";
                            break;
                        case 4:
                            if (numBlanks > 0)
                            {
                                outputString = outputString + numBlanks;
                                numBlanks = 0;
                            }
                            outputString = outputString + "R";
                            break;
                        case 5:
                            if (numBlanks > 0)
                            {
                                outputString = outputString + numBlanks;
                                numBlanks = 0;
                            }
                            outputString = outputString + "Q";
                            break;
                        case 6:
                            if (numBlanks > 0)
                            {
                                outputString = outputString + numBlanks;
                                numBlanks = 0;
                            }
                            outputString = outputString + "K";
                            break;
                        case 7:
                            if (numBlanks > 0)
                            {
                                outputString = outputString + numBlanks;
                                numBlanks = 0;
                            }
                            outputString = outputString + "p";
                            break;
                        case 8:
                            if (numBlanks > 0)
                            {
                                outputString = outputString + numBlanks;
                                numBlanks = 0;
                            }
                            outputString = outputString + "n";
                            break;
                        case 9:
                            if (numBlanks > 0)
                            {
                                outputString = outputString + numBlanks;
                                numBlanks = 0;
                            }
                            outputString = outputString + "b";
                            break;
                        case 10:
                            if (numBlanks > 0)
                            {
                                outputString = outputString + numBlanks;
                                numBlanks = 0;
                            }
                            outputString = outputString + "r";
                            break;
                        case 11:
                            if (numBlanks > 0)
                            {
                                outputString = outputString + numBlanks;
                                numBlanks = 0;
                            }
                            outputString = outputString + "q";
                            break;
                        case 12:
                            if (numBlanks > 0)
                            {
                                outputString = outputString + numBlanks;
                                numBlanks = 0;
                            }
                            outputString = outputString + "k";
                            break;
                        default:
                            numBlanks++;
                            break;
                    }







                    if (boardData[i, j] > 13) // Repair the board data in case of corruption
                    {
                        boardData[i, j] = 0;
                    }

                }

                if (numBlanks > 0)
                {
                    outputString = outputString + numBlanks;
                    numBlanks = 0;
                }
                if (j != 0)
                {
                    outputString = outputString + "/";
                } else
                {
                    if (rotate)
                    {
                        outputString = outputString + " b";
                    } else
                    {
                        outputString = outputString + " w";
                    }
                    outputString = outputString + " ";
                    if (!wkcastle && !wqcastle && !bkcastle && !bqcastle)
                    {
                        outputString = outputString + "-";
                    } else
                    {
                        if (wkcastle) { outputString = outputString + "K"; }
                        if (wqcastle) { outputString = outputString + "Q"; }
                        if (bkcastle) { outputString = outputString + "k"; }
                        if (bqcastle) { outputString = outputString + "q"; }
                    }
                    outputString = outputString + " ";

                    // Search for en passant
                    int enPassantTile = 0;
                    char[] intToCharArray = { 'a', 'b', 'c', 'd', 'e', 'f', 'g', 'h' };
                    for (int i = 0; i < 8; i++)
                    {
                        for (int k = 0; k < 8; k++)
                        {
                            if (boardData[i, k] == 13)
                            {
                                enPassantTile = ((i + 1) * 10) + (k + 1);
                            }
                        }
                    }

                    if (enPassantTile == 0)
                    {
                        outputString = outputString + "-";
                    } else
                    {
                        outputString = outputString + intToCharArray[int.Parse(enPassantTile.ToString()[0].ToString()) - 1] + enPassantTile.ToString()[1].ToString();
                    }
                    outputString = outputString + " 0 1";
                }
            }


            return outputString;


        }

        private bool setFEN(string FEN) // Update the board using the FEN string.
        {
            try // This is a very dangerous and buggy method, so if all else fails; discontinue.
            {

                // Cut the FEN into 6 neat little strings.
                string[] FENs = FEN.Split(' ');

                if (FENs.Length == 6) // Check if it is actually cut into 6 neat little strings.
                {
                    string FENpieces = FENs[0]; // Board data here
                    string FENturn = FENs[1]; // Whose turn is it? (b = black, w = white)
                    string FENcastlingRights = FENs[2]; // Can they castle? (KQkq = both players can castle queenside and kingside, Kq = White can castle kingside while black can castle queenside.)
                    string FENenPassant = FENs[3]; // This marks a recent pawn double move.
                    string FENhalfMove = FENs[4]; // Amount of turns each player has made since the last pawn advance/ piece capture.
                    string FENfullMove = FENs[5]; // Amount of turns black has made.

                    // Verify each individual miniFEN string before execution.

                    // field 1 (FENpieces)
                    // Replace blank ints with actual blanks for the first string
                    FENpieces = FENpieces.Replace("8", "00000000");
                    FENpieces = FENpieces.Replace("7", "0000000");
                    FENpieces = FENpieces.Replace("6", "000000");
                    FENpieces = FENpieces.Replace("5", "00000");
                    FENpieces = FENpieces.Replace("4", "0000");
                    FENpieces = FENpieces.Replace("3", "000");
                    FENpieces = FENpieces.Replace("2", "00");
                    FENpieces = FENpieces.Replace("1", "0");

                    Console.WriteLine("work");
                    if (FENpieces.Replace("/", "").Length != 64) { return false; } // Check if field 1 has a valid length


                    // field 2 (FENturn)

                    if (FENturn.Length != 1) { return false; } // Check if field 2 has a valid length.
                    if (FENturn != "w" && FENturn != "b") { return false; } // Check if field 2 has valid characters.

                    // field 3 (FENcastlingRights)

                    if (!(FENcastlingRights.Contains("q")) && !(FENcastlingRights.Contains("Q")) && !(FENcastlingRights.Contains("k")) && !(FENcastlingRights.Contains("K")) && !(FENcastlingRights.Contains("-")) && (FENcastlingRights.Length < 1 || FENcastlingRights.Length > 4)) { return false; } // Check if field 3 has valid characters.

                    // field 4 (FENenPassant)

                    if (FENenPassant != "-" && FENenPassant.Length != 2 && !FENfullMove[1].ToString().All(char.IsDigit) && !FENfullMove[0].ToString().All(char.IsLetter)) { return false; }

                    // field 5 (FENhalfMove)

                    if (!(FENhalfMove.All(char.IsDigit)) && FENhalfMove.Length < 1) { return false; } // Check if field 5 is a complete number.

                    // field 5 (FENfullMove)

                    if (!(FENfullMove.All(char.IsDigit)) && FENfullMove.Length < 1) { return false; } // Check if field 6 is a complete number.

                    // Verification step is done!


                    // Begin execution!
                    // For the meantime only focus on field 1.

                    // 0 = blank

                    // 1 = pawn
                    // 2 = knight
                    // 3 = bishop
                    // 4 = rook
                    // 5 = queen
                    // 6 = king

                    // +6 = black's piece

                    string[] FENpiecesData = FENpieces.Split("/");

                    for (int j = 0; j < 8; j++)
                    {
                        for (int i = 0; i < 8; i++)
                        {
                            // i = letter
                            // j = number
                            switch (FENpiecesData[j][i])
                            {
                                case 'P':
                                    setTile(((i + 1) * 10) + ((7 - j) + 1), 1);
                                    break;
                                case 'N':
                                    setTile(((i + 1) * 10) + ((7 - j) + 1), 2);
                                    break;
                                case 'B':
                                    setTile(((i + 1) * 10) + ((7 - j) + 1), 3);
                                    break;
                                case 'R':
                                    setTile(((i + 1) * 10) + ((7 - j) + 1), 4);
                                    break;
                                case 'Q':
                                    setTile(((i + 1) * 10) + ((7 - j) + 1), 5);
                                    break;
                                case 'K':
                                    setTile(((i + 1) * 10) + ((7 - j) + 1), 6);
                                    break;
                                case 'p':
                                    setTile(((i + 1) * 10) + ((7 - j) + 1), 7);
                                    break;
                                case 'n':
                                    setTile(((i + 1) * 10) + ((7 - j) + 1), 8);
                                    break;
                                case 'b':
                                    setTile(((i + 1) * 10) + ((7 - j) + 1), 9);
                                    break;
                                case 'r':
                                    setTile(((i + 1) * 10) + ((7 - j) + 1), 10);
                                    break;
                                case 'q':
                                    setTile(((i + 1) * 10) + ((7 - j) + 1), 11);
                                    break;
                                case 'k':
                                    setTile(((i + 1) * 10) + ((7 - j) + 1), 12);
                                    break;
                                default:
                                    setTile(((i + 1) * 10) + ((7 - j) + 1), 0);
                                    break;
                            }
                        }
                    }

                    // Field 2: Rotate based on field 2
                    if (FENturn == "w")
                    {
                        rotate = false;
                        rotateboard.BackgroundImage = Properties.Resources.wking;
                    } else
                    {
                        rotate = true;
                        rotateboard.BackgroundImage = Properties.Resources.bking;
                    }

                    // Field 3: Load castling rights
                    wkcastle = false;
                    wqcastle = false;
                    bkcastle = false;
                    bqcastle = false;
                    if (FENcastlingRights.Contains("K")) { wkcastle = true; }
                    if (FENcastlingRights.Contains("Q")) { wqcastle = true; }
                    if (FENcastlingRights.Contains("k")) { bkcastle = true; }
                    if (FENcastlingRights.Contains("q")) { bqcastle = true; }

                    // Field 4: En Passant Pawn
                    if (FENenPassant.Length == 2)
                    {
                        if (int.Parse(FENenPassant[1].ToString()) > 8 || int.Parse(FENenPassant[1].ToString()) < 1)
                        {
                            return false;
                        } else
                        {
                            int letterNumber = 0;
                            switch (FENenPassant[0])
                            {
                                case 'a':
                                    letterNumber = 1;
                                    break;
                                case 'b':
                                    letterNumber = 2;
                                    break;
                                case 'c':
                                    letterNumber = 3;
                                    break;
                                case 'd':
                                    letterNumber = 4;
                                    break;
                                case 'e':
                                    letterNumber = 5;
                                    break;
                                case 'f':
                                    letterNumber = 6;
                                    break;
                                case 'g':
                                    letterNumber = 7;
                                    break;
                                case 'h':
                                    letterNumber = 8;
                                    break;
                            }
                            if (letterNumber != 0)
                            {
                                int enPassantCoordinates = int.Parse((letterNumber.ToString()) + FENenPassant[1].ToString());
                                setTile(enPassantCoordinates, 13);
                            }
                            else
                            {
                                return false;
                            }
                        }
                        
                        
                    }
                    return true; // Success
                } else { return false; } // INVALID FEN!
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                return false; // false means the method encountered a problem with the given FEN string.
            }
        }






        private void drawBoard() // Update board graphic
        {
            for (int i = 0; i < boardData.GetLength(0); i++)
            {
                for (int j = 0; j < boardData.GetLength(1); j++)
                {
                    string currentTileIdUnmodified = (((i + 1) * 10) + (j + 1)).ToString();

                    string currentTileId = currentTileIdUnmodified;
                    if (rotate)
                    {
                        currentTileId = (99 - int.Parse(currentTileIdUnmodified)).ToString();
                    }

                    setTileImage(int.Parse(currentTileId), getTile(int.Parse((i + 1).ToString() + (j + 1).ToString())));

                    bool isSelected = (int.Parse(currentTileId) == currentSelection);
                    if (rotate)
                    {
                        isSelected = (99 - int.Parse(currentTileId) == currentSelection);
                    }
                    
                    if (isSelected) // Check if the tile is being selected right now.
                    {
                        setTileColor(int.Parse(currentTileId), selectedTileColor);

                    } else
                    {
                        if ((i + j + 2) % 2 == 0) // Is ID even?
                        {
                            setTileColor(int.Parse(currentTileId), darkTileColor);

                        }
                        else // Id is odd.
                        {
                            setTileColor(int.Parse(currentTileId), lightTileColor);

                        }
                    }

                    






                    if (boardData[i,j] > 13) // Repair the board data in case of corruption
                    {
                        boardData[i, j] = 0;
                    }

                }
            }
            sandbox0.BackColor = Color.RosyBrown;
            sandboxp.BackColor = Color.DimGray;
            sandboxn.BackColor = Color.DimGray;
            sandboxb.BackColor = Color.DimGray;
            sandboxr.BackColor = Color.DimGray;
            sandboxq.BackColor = Color.DimGray;
            sandboxk.BackColor = Color.DimGray;
            sandboxe.BackColor = Color.DimGray;
            if (currentSelection >= 1000)
            {
                if (currentSelection < 2000)
                {
                    sandboxp.BackColor = selectedTileColor;
                } else if (currentSelection < 3000)
                {
                    sandboxn.BackColor = selectedTileColor;
                } else if (currentSelection < 4000)
                {
                    sandboxb.BackColor = selectedTileColor;
                } else if (currentSelection < 5000)
                {
                    sandboxr.BackColor = selectedTileColor;
                } else if (currentSelection < 6000)
                {
                    sandboxq.BackColor = selectedTileColor;
                } else if (currentSelection < 7000)
                {
                    sandboxk.BackColor = selectedTileColor;
                } else if (currentSelection < 8000)
                {
                    sandbox0.BackColor = selectedTileColor;
                }
                else if (currentSelection < 9000)
                {
                    sandboxe.BackColor = selectedTileColor;
                }
            }
            if (rotate)
            {
                sandboxp.BackgroundImage = Properties.Resources.bpawn;
                sandboxn.BackgroundImage = Properties.Resources.bhorse;
                sandboxb.BackgroundImage = Properties.Resources.bbishop;
                sandboxr.BackgroundImage = Properties.Resources.brook;
                sandboxq.BackgroundImage = Properties.Resources.bqueen;
                sandboxk.BackgroundImage = Properties.Resources.bking;
                qcastle.BackgroundImage = Properties.Resources.bqcastle;
                kcastle.BackgroundImage = Properties.Resources.bkcastle;
                if (bqcastle)
                {
                    qcastle.BackColor = Color.White;
                } else
                {
                    qcastle.BackColor = Color.Brown;
                }
                if (bkcastle)
                {
                    kcastle.BackColor = Color.White;
                }
                else
                {
                    kcastle.BackColor = Color.Brown;
                }
            } else
            {
                sandboxp.BackgroundImage = Properties.Resources.wpawn;
                sandboxn.BackgroundImage = Properties.Resources.whorse;
                sandboxb.BackgroundImage = Properties.Resources.wbishop;
                sandboxr.BackgroundImage = Properties.Resources.wrook;
                sandboxq.BackgroundImage = Properties.Resources.wqueen;
                sandboxk.BackgroundImage = Properties.Resources.wking;
                qcastle.BackgroundImage = Properties.Resources.wqcastle;
                kcastle.BackgroundImage = Properties.Resources.wkcastle;
                if (wqcastle)
                {
                    qcastle.BackColor = Color.White;
                }
                else
                {
                    qcastle.BackColor = Color.Brown;
                }
                if (wkcastle)
                {
                    kcastle.BackColor = Color.White;
                }
                else
                {
                    kcastle.BackColor = Color.Brown;
                }
            }
        }






        
        private void setTileImage(int tileId, int pieceId) // Draw a specific piece on a specific tile
        {
            switch (tileId)
            {
                case 11:
                    a1.BackgroundImage = searchForImage(pieceId);
                    break;
                case 12:
                    a2.BackgroundImage = searchForImage(pieceId);
                    break;
                case 13:
                    a3.BackgroundImage = searchForImage(pieceId);
                    break;
                case 14:
                    a4.BackgroundImage = searchForImage(pieceId);
                    break;
                case 15:
                    a5.BackgroundImage = searchForImage(pieceId);
                    break;
                case 16:
                    a6.BackgroundImage = searchForImage(pieceId);
                    break;
                case 17:
                    a7.BackgroundImage = searchForImage(pieceId);
                    break;
                case 18:
                    a8.BackgroundImage = searchForImage(pieceId);
                    break;
                case 21:
                    b1.BackgroundImage = searchForImage(pieceId);
                    break;
                case 22:
                    b2.BackgroundImage = searchForImage(pieceId);
                    break;
                case 23:
                    b3.BackgroundImage = searchForImage(pieceId);
                    break;
                case 24:
                    b4.BackgroundImage = searchForImage(pieceId);
                    break;  
                case 25:
                    b5.BackgroundImage = searchForImage(pieceId);
                    break;
                case 26:
                    b6.BackgroundImage = searchForImage(pieceId);
                    break;
                case 27:
                    b7.BackgroundImage = searchForImage(pieceId);
                    break;
                case 28:
                    b8.BackgroundImage = searchForImage(pieceId);
                    break;
                case 31:
                    c1.BackgroundImage = searchForImage(pieceId);
                    break;
                case 32:
                    c2.BackgroundImage = searchForImage(pieceId);
                    break;
                case 33:
                    c3.BackgroundImage = searchForImage(pieceId);
                    break;
                case 34:
                    c4.BackgroundImage = searchForImage(pieceId);
                    break;
                case 35:
                    c5.BackgroundImage = searchForImage(pieceId);
                    break;
                case 36:
                    c6.BackgroundImage = searchForImage(pieceId);
                    break;
                case 37:
                    c7.BackgroundImage = searchForImage(pieceId);
                    break;
                case 38:
                    c8.BackgroundImage = searchForImage(pieceId);
                    break;
                case 41:
                    d1.BackgroundImage = searchForImage(pieceId);
                    break;
                case 42:
                    d2.BackgroundImage = searchForImage(pieceId);
                    break;
                case 43:
                    d3.BackgroundImage = searchForImage(pieceId);
                    break;
                case 44:
                    d4.BackgroundImage = searchForImage(pieceId);
                    break;
                case 45:
                    d5.BackgroundImage = searchForImage(pieceId);
                    break;
                case 46:
                    d6.BackgroundImage = searchForImage(pieceId);
                    break;
                case 47:
                    d7.BackgroundImage = searchForImage(pieceId);
                    break;
                case 48:
                    d8.BackgroundImage = searchForImage(pieceId);
                    break;
                case 51:
                    e1.BackgroundImage = searchForImage(pieceId);
                    break;
                case 52:
                    e2.BackgroundImage = searchForImage(pieceId);
                    break;
                case 53:
                    e3.BackgroundImage = searchForImage(pieceId);
                    break;
                case 54:
                    e4.BackgroundImage = searchForImage(pieceId);
                    break;
                case 55:
                    e5.BackgroundImage = searchForImage(pieceId);
                    break;
                case 56:
                    e6.BackgroundImage = searchForImage(pieceId);
                    break;
                case 57:
                    e7.BackgroundImage = searchForImage(pieceId);
                    break;
                case 58:
                    e8.BackgroundImage = searchForImage(pieceId);
                    break;
                case 61:
                    f1.BackgroundImage = searchForImage(pieceId);
                    break;
                case 62:
                    f2.BackgroundImage = searchForImage(pieceId);
                    break;
                case 63:
                    f3.BackgroundImage = searchForImage(pieceId);
                    break;
                case 64:
                    f4.BackgroundImage = searchForImage(pieceId);
                    break;
                case 65:
                    f5.BackgroundImage = searchForImage(pieceId);
                    break;
                case 66:
                    f6.BackgroundImage = searchForImage(pieceId);
                    break;
                case 67:
                    f7.BackgroundImage = searchForImage(pieceId);
                    break;
                case 68:
                    f8.BackgroundImage = searchForImage(pieceId);
                    break;
                case 71:
                    g1.BackgroundImage = searchForImage(pieceId);
                    break;
                case 72:
                    g2.BackgroundImage = searchForImage(pieceId);
                    break;
                case 73:
                    g3.BackgroundImage = searchForImage(pieceId);
                    break;
                case 74:
                    g4.BackgroundImage = searchForImage(pieceId);
                    break;
                case 75:
                    g5.BackgroundImage = searchForImage(pieceId);
                    break;
                case 76:
                    g6.BackgroundImage = searchForImage(pieceId);
                    break;
                case 77:
                    g7.BackgroundImage = searchForImage(pieceId);
                    break;
                case 78:
                    g8.BackgroundImage = searchForImage(pieceId);
                    break;
                case 81:
                    h1.BackgroundImage = searchForImage(pieceId);
                    break;
                case 82:
                    h2.BackgroundImage = searchForImage(pieceId);
                    break;
                case 83:
                    h3.BackgroundImage = searchForImage(pieceId);
                    break;
                case 84:
                    h4.BackgroundImage = searchForImage(pieceId);
                    break;
                case 85:
                    h5.BackgroundImage = searchForImage(pieceId);
                    break;
                case 86:
                    h6.BackgroundImage = searchForImage(pieceId);
                    break;
                case 87:
                    h7.BackgroundImage = searchForImage(pieceId);
                    break;
                case 88:
                    h8.BackgroundImage = searchForImage(pieceId);
                    break;
                default:
                    break;
            }
        }

        private void setTileColor(int tileId, Color color) // Draw a specific piece on a specific tile
        {
            switch (tileId)
            {
                case 11:
                    a1.BackColor = color;
                    break;
                case 12:
                    a2.BackColor = color;
                    break;
                case 13:
                    a3.BackColor = color;
                    break;
                case 14:
                    a4.BackColor = color;
                    break;
                case 15:
                    a5.BackColor = color;
                    break;
                case 16:
                    a6.BackColor = color;
                    break;
                case 17:
                    a7.BackColor = color;
                    break;
                case 18:
                    a8.BackColor = color;
                    break;
                case 21:
                    b1.BackColor = color;
                    break;
                case 22:
                    b2.BackColor = color;
                    break;
                case 23:
                    b3.BackColor = color;
                    break;
                case 24:
                    b4.BackColor = color;
                    break;
                case 25:
                    b5.BackColor = color;
                    break;
                case 26:
                    b6.BackColor = color;
                    break;
                case 27:
                    b7.BackColor = color;
                    break;
                case 28:
                    b8.BackColor = color;
                    break;
                case 31:
                    c1.BackColor = color;
                    break;
                case 32:
                    c2.BackColor = color;
                    break;
                case 33:
                    c3.BackColor = color;
                    break;
                case 34:
                    c4.BackColor = color;
                    break;
                case 35:
                    c5.BackColor = color;
                    break;
                case 36:
                    c6.BackColor = color;
                    break;
                case 37:
                    c7.BackColor = color;
                    break;
                case 38:
                    c8.BackColor = color;
                    break;
                case 41:
                    d1.BackColor = color;
                    break;
                case 42:
                    d2.BackColor = color;
                    break;
                case 43:
                    d3.BackColor = color;
                    break;
                case 44:
                    d4.BackColor = color;
                    break;
                case 45:
                    d5.BackColor = color;
                    break;
                case 46:
                    d6.BackColor = color;
                    break;
                case 47:
                    d7.BackColor = color;
                    break;
                case 48:
                    d8.BackColor = color;
                    break;
                case 51:
                    e1.BackColor = color;
                    break;
                case 52:
                    e2.BackColor = color;
                    break;
                case 53:
                    e3.BackColor = color;
                    break;
                case 54:
                    e4.BackColor = color;
                    break;
                case 55:
                    e5.BackColor = color;
                    break;
                case 56:
                    e6.BackColor = color;
                    break;
                case 57:
                    e7.BackColor = color;
                    break;
                case 58:
                    e8.BackColor = color;
                    break;
                case 61:
                    f1.BackColor = color;
                    break;
                case 62:
                    f2.BackColor = color;
                    break;
                case 63:
                    f3.BackColor = color;
                    break;
                case 64:
                    f4.BackColor = color;
                    break;
                case 65:
                    f5.BackColor = color;
                    break;
                case 66:
                    f6.BackColor = color;
                    break;
                case 67:
                    f7.BackColor = color;
                    break;
                case 68:
                    f8.BackColor = color;
                    break;
                case 71:
                    g1.BackColor = color;
                    break;
                case 72:
                    g2.BackColor = color;
                    break;
                case 73:
                    g3.BackColor = color;
                    break;
                case 74:
                    g4.BackColor = color;
                    break;
                case 75:
                    g5.BackColor = color;
                    break;
                case 76:
                    g6.BackColor = color;
                    break;
                case 77:
                    g7.BackColor = color;
                    break;
                case 78:
                    g8.BackColor = color;
                    break;
                case 81:
                    h1.BackColor = color;
                    break;
                case 82:
                    h2.BackColor = color;
                    break;
                case 83:
                    h3.BackColor = color;
                    break;
                case 84:
                    h4.BackColor = color;
                    break;
                case 85:
                    h5.BackColor = color;
                    break;
                case 86:
                    h6.BackColor = color;
                    break;
                case 87:
                    h7.BackColor = color;
                    break;
                case 88:
                    h8.BackColor = color;
                    break;
                default:
                    break;
            }
        }

        private System.Drawing.Bitmap searchForImage(int pieceId)
        {
            // 0 = blank

            // 1 = pawn
            // 2 = knight
            // 3 = bishop
            // 4 = rook
            // 5 = queen
            // 6 = king

            // +6 = black's piece

            switch (pieceId)
            {
                case 0:
                    return Properties.Resources.blank;
                case 1:
                    return Properties.Resources.wpawn;
                    case 2:
                    return Properties.Resources.whorse;
                case 3:
                    return Properties.Resources.wbishop;
                case 4:
                    return Properties.Resources.wrook;
                case 5:
                    return Properties.Resources.wqueen;
                case 6:
                    return Properties.Resources.wking;
                case 7:
                    return Properties.Resources.bpawn;
                case 8:
                    return Properties.Resources.bhorse;
                case 9:
                    return Properties.Resources.bbishop;
                case 10:
                    return Properties.Resources.brook;
                case 11:
                    return Properties.Resources.bqueen;
                case 12:
                    return Properties.Resources.bking;
                case 13:
                    return Properties.Resources.enpassantpawn;
                default:
                    return Properties.Resources.blank;
            }
        }







        // tile selection events

        private void b1_Click(object sender, EventArgs e)
        {
            selectTile(21);
        }

        private void h2_Click(object sender, EventArgs e)
        {
            selectTile(82);
        }

        private void h3_Click(object sender, EventArgs e)
        {
            selectTile(83);
        }

        private void h4_Click(object sender, EventArgs e)
        {
            selectTile(84);
        }

        private void h5_Click(object sender, EventArgs e)
        {
            selectTile(85);
        }

        private void h6_Click(object sender, EventArgs e)
        {
            selectTile(86);
        }

        private void h7_Click(object sender, EventArgs e)
        {
            selectTile(87);
        }

        private void h8_Click(object sender, EventArgs e)
        {
            selectTile(88);
        }

        private void g1_Click(object sender, EventArgs e)
        {
            selectTile(71);
        }

        private void g2_Click(object sender, EventArgs e)
        {
            selectTile(72);
        }

        private void g3_Click(object sender, EventArgs e)
        {
            selectTile(73);
        }

        private void g4_Click(object sender, EventArgs e)
        {
            selectTile(74);
        }

        private void g5_Click(object sender, EventArgs e)
        {
            selectTile(75);
        }

        private void g6_Click(object sender, EventArgs e)
        {
            selectTile(76);
        }

        private void g7_Click(object sender, EventArgs e)
        {
            selectTile(77);
        }

        private void g8_Click(object sender, EventArgs e)
        {
            selectTile(78);
        }

        private void f1_Click(object sender, EventArgs e)
        {
            selectTile(61);
        }

        private void f2_Click(object sender, EventArgs e)
        {
            selectTile(62);
        }

        private void f3_Click(object sender, EventArgs e)
        {
            selectTile(63);
        }

        private void f4_Click(object sender, EventArgs e)
        {
            selectTile(64);
        }

        private void f5_Click(object sender, EventArgs e)
        {
            selectTile(65);
        }

        private void f6_Click(object sender, EventArgs e)
        {
            selectTile(66);
        }

        private void f7_Click(object sender, EventArgs e)
        {
            selectTile(67);
        }

        private void f8_Click(object sender, EventArgs e)
        {
            selectTile(68);
        }

        private void e1_Click(object sender, EventArgs e)
        {
            selectTile(51);
        }

        private void e2_Click(object sender, EventArgs e)
        {
            selectTile(52);
        }

        private void e3_Click(object sender, EventArgs e)
        {
            selectTile(53);
        }

        private void e4_Click(object sender, EventArgs e)
        {
            selectTile(54);
        }

        private void e5_Click(object sender, EventArgs e)
        {
            selectTile(55);
        }

        private void e6_Click(object sender, EventArgs e)
        {
            selectTile(56);
        }

        private void e7_Click(object sender, EventArgs e)
        {
            selectTile(57);
        }

        private void e8_Click(object sender, EventArgs e)
        {
            selectTile(58);
        }

        private void d1_Click(object sender, EventArgs e)
        {
            selectTile(41);
        }

        private void d2_Click(object sender, EventArgs e)
        {
            selectTile(42);
        }

        private void d3_Click(object sender, EventArgs e)
        {
            selectTile(43);
        }

        private void d4_Click(object sender, EventArgs e)
        {
            selectTile(44);
        }

        private void d5_Click(object sender, EventArgs e)
        {
            selectTile(45);
        }

        private void d6_Click(object sender, EventArgs e)
        {
            selectTile(46);
        }

        private void d7_Click(object sender, EventArgs e)
        {
            selectTile(47);
        }

        private void d8_Click(object sender, EventArgs e)
        {
            selectTile(48);
        }

        private void c1_Click(object sender, EventArgs e)
        {
            selectTile(31);
        }

        private void c2_Click(object sender, EventArgs e)
        {
            selectTile(32);
        }

        private void c3_Click(object sender, EventArgs e)
        {
            selectTile(33);
        }

        private void c4_Click(object sender, EventArgs e)
        {
            selectTile(34);
        }

        private void c5_Click(object sender, EventArgs e)
        {
            selectTile(35);
        }

        private void c6_Click(object sender, EventArgs e)
        {
            selectTile(36);
        }

        private void c7_Click(object sender, EventArgs e)
        {
            selectTile(37);
        }

        private void c8_Click(object sender, EventArgs e)
        {
            selectTile(38);
        }

        private void h1_Click(object sender, EventArgs e)
        {
            selectTile(81);
        }

        private void b2_Click(object sender, EventArgs e)
        {
            selectTile(22);
        }

        private void b3_Click(object sender, EventArgs e)
        {
            selectTile(23);
        }

        private void b4_Click(object sender, EventArgs e)
        {
            selectTile(24);
        }

        private void b5_Click(object sender, EventArgs e)
        {
            selectTile(25);
        }

        private void b6_Click(object sender, EventArgs e)
        {
            selectTile(26);
        }

        private void b7_Click(object sender, EventArgs e)
        {
            selectTile(27);
        }

        private void b8_Click(object sender, EventArgs e)
        {
            selectTile(28);
        }

        private void a1_Click(object sender, EventArgs e)
        {
            selectTile(11);
        }

        private void a2_Click(object sender, EventArgs e)
        {
            selectTile(12);
        }

        private void a3_Click(object sender, EventArgs e)
        {
            selectTile(13);
        }

        private void a4_Click(object sender, EventArgs e)
        {
            selectTile(14);
        }

        private void a5_Click(object sender, EventArgs e)
        {
            selectTile(15);
        }

        private void a6_Click(object sender, EventArgs e)
        {
            selectTile(16);
        }

        private void a7_Click(object sender, EventArgs e)
        {
            selectTile(17);
        }

        private void a8_Click(object sender, EventArgs e)
        {
            selectTile(18);
        }





        private void textBox1_TextChanged(object sender, EventArgs e)
        {
            bool isFENvalid = setFEN(textBox1.Text);
            if (isFENvalid)
            {
                textBox1.BackColor = Color.FromArgb(30, 30, 30);
                drawBoard();
                if (!(isUndo))
                {
                    boardDataHistory.Add(getFEN());
                } else { isUndo = false; }
                
                if (boardDataHistory.Count < 2)
                {
                    resetToStartingPositionToolStripMenuItem.Enabled = false;
                    undoToolStripMenuItem.Enabled = false;
                }
                else
                {
                    resetToStartingPositionToolStripMenuItem.Enabled = true;
                    undoToolStripMenuItem.Enabled = true;
                }

            } else
            {
                textBox1.BackColor = Color.IndianRed;
                setFEN(boardDataHistory.Last());
            }
            if (linkedFile != "0")
            {
                if (getFEN() == File.ReadAllText(linkedFile))
                {
                    saveToolStripMenuItem.Enabled = false;
                } else
                {
                    saveToolStripMenuItem.Enabled = true;
                }
            } else
            {
                saveToolStripMenuItem.Enabled = false;
            }
            
        }

        private void label1_Click(object sender, EventArgs e)
        {

        }

        private void Form1_Load(object sender, EventArgs e)
        {

        }

        private void menuStrip1_ItemClicked(object sender, ToolStripItemClickedEventArgs e)
        {

        }

        private void comboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {

        }

        private void setLightTilesColorToolStripMenuItem_Click(object sender, EventArgs e)
        {
            ColorDialog colorDlg = new ColorDialog();
            colorDlg.AllowFullOpen = true;
            colorDlg.FullOpen = true;
            colorDlg.AnyColor = true;
            colorDlg.SolidColorOnly = false;
            colorDlg.Color = lightTileColor;

            if (colorDlg.ShowDialog() == DialogResult.OK)
            {
                lightTileColor = colorDlg.Color;
            }

            drawBoard();
        }

        private void setDarkTilesColorToolStripMenuItem_Click(object sender, EventArgs e)
        {
            ColorDialog colorDlg = new ColorDialog();
            colorDlg.AllowFullOpen = true;
            colorDlg.FullOpen = true;
            colorDlg.AnyColor = true;
            colorDlg.SolidColorOnly = false;
            colorDlg.Color = darkTileColor;

            if (colorDlg.ShowDialog() == DialogResult.OK)
            {
                darkTileColor = colorDlg.Color;
            }

            drawBoard();
        }

        private void setSelectedTileColorToolStripMenuItem_Click(object sender, EventArgs e)
        {
            ColorDialog colorDlg = new ColorDialog();
            colorDlg.AllowFullOpen = true;
            colorDlg.FullOpen = true;
            colorDlg.AnyColor = true;
            colorDlg.SolidColorOnly = false;
            colorDlg.Color = selectedTileColor;

            if (colorDlg.ShowDialog() == DialogResult.OK)
            {
                selectedTileColor = colorDlg.Color;
            }

            drawBoard();
        }

        private void resetAllToDefaultToolStripMenuItem_Click(object sender, EventArgs e)
        {
            darkTileColor = Color.LightGray;
            lightTileColor = Color.White;
            selectedTileColor = Color.LightGreen;
            drawBoard();
        }

        

        private void undoToolStripMenuItem_Click(object sender, EventArgs e) // Undo Functionality
        {
            if (boardDataHistory.Count != 1)
            {
                boardDataHistory.RemoveAt(boardDataHistory.Count - 1);
                isUndo = true;
                setFEN(boardDataHistory.Last());
                textBox1.Text = getFEN();
                drawBoard();
                if (boardDataHistory.Count < 2)
                {
                    undoToolStripMenuItem.Enabled = false;
                    resetToStartingPositionToolStripMenuItem.Enabled = false;
                } else
                {
                    undoToolStripMenuItem.Enabled = true;
                    resetToStartingPositionToolStripMenuItem.Enabled = true;
                }
            } else
            {
                undoToolStripMenuItem.Enabled = false;
                resetToStartingPositionToolStripMenuItem.Enabled = false;
            }
            
        }

        private void groupBox1_Enter(object sender, EventArgs e)
        {

        }

        private void panel1_Paint(object sender, PaintEventArgs e)
        {

        }

        private void label2_Click(object sender, EventArgs e)
        {

        }

        private void textBox2_TextChanged(object sender, EventArgs e)
        {
            if (textBox2.Text == "")
            {
                textBox2.Text = "Untitled Position";
                Form1.ActiveForm.Text = "Untitled Position - Chess Sandbox";
            } else
            {
                Form1.ActiveForm.Text = textBox2.Text + " - Chess Sandbox";
            }
            
        }

        private void rotateboard_Click(object sender, EventArgs e)
        {
            if (rotate)
            {
                rotate = false;
                rotateboard.BackgroundImage = Properties.Resources.wking;
            } else
            {
                rotate = true;
                rotateboard.BackgroundImage = Properties.Resources.bking;
            }
            textBox1.Text = getFEN();
            drawBoard();
        }

        private void resetToStartingPositionToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (boardDataHistory.Count != 1)
            {
                isUndo = true;
                setFEN(boardDataHistory.First());
                textBox1.Text = getFEN();
                boardDataHistory.Clear();
                boardDataHistory.Add(getFEN());
                drawBoard();
                resetToStartingPositionToolStripMenuItem.Enabled = false;
                undoToolStripMenuItem.Enabled = false;
            }
            else
            {
                resetToStartingPositionToolStripMenuItem.Enabled = false;
                undoToolStripMenuItem.Enabled = false;
            }
        }

        private void syncFENToolStripMenuItem_Click(object sender, EventArgs e)
        {
            isUndo = true;
            setFEN(textBox1.Text);
            isUndo = true;
            textBox1.Text = getFEN();
        }

        private void sandbox0_Click(object sender, EventArgs e)
        {
            selectSandboxTile(7000);
        }

        private void sandboxp_Click(object sender, EventArgs e)
        {
            if (rotate)
            {
                selectSandboxTile(1200);
            } else
            {
                selectSandboxTile(1100);
            }
        }

        private void sandboxn_Click(object sender, EventArgs e)
        {
            if (rotate)
            {
                selectSandboxTile(2200);
            }
            else
            {
                selectSandboxTile(2100);
            }
        }

        private void sandboxb_Click(object sender, EventArgs e)
        {
            if (rotate)
            {
                selectSandboxTile(3200);
            }
            else
            {
                selectSandboxTile(3100);
            }
        }

        private void sandboxr_Click(object sender, EventArgs e)
        {
            if (rotate)
            {
                selectSandboxTile(4200);
            }
            else
            {
                selectSandboxTile(4100);
            }
        }

        private void sandboxq_Click(object sender, EventArgs e)
        {
            if (rotate)
            {
                selectSandboxTile(5200);
            }
            else
            {
                selectSandboxTile(5100);
            }
        }

        private void sandboxk_Click(object sender, EventArgs e)
        {
            if (rotate)
            {
                selectSandboxTile(6200);
            }
            else
            {
                selectSandboxTile(6100);
            }
        }

        private void clearBoardToolStripMenuItem_Click(object sender, EventArgs e)
        {
            textBox1.Text = "8/8/8/8/8/8/8/8 w - - 0 1";
        }

        private void sandboxe_Click(object sender, EventArgs e)
        {
            selectSandboxTile(8000);
        }

        private void qcastle_Click(object sender, EventArgs e)
        {
            if (rotate)
            {
                if (bqcastle)
                {
                    bqcastle = false;
                }
                else
                {
                    bqcastle = true;
                }
            } else
            {
                if (wqcastle)
                {
                    wqcastle = false;
                } else
                {
                    wqcastle = true;
                }
            }
            textBox1.Text = getFEN();
            drawBoard();
        }

        private void kcastle_Click(object sender, EventArgs e)
        {
            if (rotate)
            {
                if (bkcastle)
                {
                    bkcastle = false;
                }
                else
                {
                    bkcastle = true;
                }
            }
            else
            {
                if (wkcastle)
                {
                    wkcastle = false;
                }
                else
                {
                    wkcastle = true;
                }
            }
            textBox1.Text = getFEN();
            drawBoard();
        }

        private void openToolStripMenuItem_Click(object sender, EventArgs e)
        {
            OpenFileDialog openFileDialog1 = new OpenFileDialog
            {
                Title = "Browse Chess Positions",

                CheckFileExists = true,
                CheckPathExists = true,

                DefaultExt = "fen",
                Filter = "All FEN file types (*.fen;s *.dfen)|*.fen;*.dfen",
                FilterIndex = 1,
                RestoreDirectory = true,

                ReadOnlyChecked = false,
                ShowReadOnly = false
            };

            if (openFileDialog1.ShowDialog() == DialogResult.OK)
            {
                if (openFileDialog1.FileName.Split('.').Last() == "fen")
                {
                    richTextBox1.Text = "(no description provided)";
                    textBox2.Text = openFileDialog1.FileName.Split('\\').Last().Split('.')[0];
                    textBox1.Text = File.ReadAllText(openFileDialog1.FileName);
                    linkedFile = openFileDialog1.FileName;
                    saveToolStripMenuItem.Text = "Save to " + openFileDialog1.FileName.Split("\\").Last();
                }
                else if (openFileDialog1.FileName.Split('.').Last() == "dfen")
                {
                    string[] dfenParameters = File.ReadAllText(openFileDialog1.FileName).Split("|||");
                    textBox1.Text = dfenParameters[0];
                    textBox2.Text = dfenParameters[1];
                    richTextBox1.Text = dfenParameters[2];
                    linkedFile = openFileDialog1.FileName;
                    saveToolStripMenuItem.Text = "Save to " + openFileDialog1.FileName.Split("\\").Last();
                }
                
            }
        }

        private void saveToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (linkedFile.Split('.').Last() == "fen")
            {
                File.WriteAllText(linkedFile, getFEN());
            } else if (linkedFile.Split('.').Last() == "dfen")
            {
                File.WriteAllText(linkedFile, getFEN() + "|||" + textBox2.Text + "|||" + richTextBox1.Text);
                saveToolStripMenuItem.Enabled = false;
            }
        }

        private void saveAsToolStripMenuItem_Click(object sender, EventArgs e)
        {

            SaveFileDialog saveFileDialog1 = new SaveFileDialog();  
            saveFileDialog1.Title = "Save Chess Position";
            saveFileDialog1.CheckFileExists = false;
            saveFileDialog1.CheckPathExists = true;
            saveFileDialog1.DefaultExt = "fen";
            saveFileDialog1.Filter = "Forsyth-Edwards Notation (*.fen)|*.fen|Descriptive FEN (*.dfen)|*.dfen";
            saveFileDialog1.FilterIndex = 1;
            saveFileDialog1.CreatePrompt = true;
            saveFileDialog1.OverwritePrompt = true;
            saveFileDialog1.InitialDirectory = linkedFile;
            saveFileDialog1.RestoreDirectory = true;
            if (richTextBox1.Text != "(no description provided)")
            {
                saveFileDialog1.FilterIndex = 2;
            }
            if (saveFileDialog1.ShowDialog() == DialogResult.OK)
            {
                if (saveFileDialog1.FileName.Split('.').Last() == "fen")
                {
                    linkedFile = saveFileDialog1.FileName;
                    saveToolStripMenuItem.Text = "Save to " + saveFileDialog1.FileName.Split("\\").Last();
                    File.WriteAllText(linkedFile, getFEN());
                } else if (saveFileDialog1.FileName.Split('.').Last() == "dfen")
                {
                    linkedFile = saveFileDialog1.FileName;
                    saveToolStripMenuItem.Text = "Save to " + saveFileDialog1.FileName.Split("\\").Last();
                    File.WriteAllText(linkedFile, getFEN() + "|||" + textBox2.Text + "|||" + richTextBox1.Text);
                }
            }
        }

        private string evaluateFEN(string FENinput) // WILL NOT WORK ON THIS BECAUSE ITS JUST PAIN.
        {
            var p = new System.Diagnostics.Process();
            p.StartInfo.FileName = @"";
            p.StartInfo.UseShellExecute = false;
            p.StartInfo.RedirectStandardInput = true;
            p.StartInfo.RedirectStandardOutput = true;
            p.StartInfo.WindowStyle = System.Diagnostics.ProcessWindowStyle.Hidden;
            p.Start();
            p.StandardInput.WriteLine("position fen " + FENinput);
            
            p.StandardInput.WriteLine("eval");

            string bestMoveInAlgebraicNotation = p.StandardOutput.ReadToEnd();

            p.StandardInput.WriteLine("quit");

            p.Close();

            return bestMoveInAlgebraicNotation;
        }
    }
}