﻿using Klondike.Entities;
using System;
using System.Diagnostics;
using System.Globalization;
using System.Runtime.InteropServices;

namespace Klondike {
    public class Program {
        public static int Main(string[] args) {
            if (args == null || args.Length == 0) {
                Console.WriteLine(
@$"Minimal Klondike
Klondike.exe [Options] [CardSet]

DrawCount (Default=1)
-D #

Initial Moves
-M ""Moves To Play Initially""

Max States (Default=50,000,000) (About 1GB RAM Per 22 Million)
-S #

Solve Seed 123 from GreenFelt:
Klondike.exe 123

Solve Given CardSet With Initial Moves:
Klondike.exe -D 1 -M ""HE KE @@@@AD GD LJ @@AH @@AJ GJ @@@@AG @AB"" 081054022072134033082024052064053012061013042093084124092122062031083121113023043074051114091014103044131063041102101133011111071073034123104112021132032094");
                return 0;
            }

            string cardSet = args[^1].Replace("\"", "");
            int drawCount = 1;
            string moveSet = null;
            string boardState = null;
            char simplified = ' ';
            int maxStates = 50_000_000;
            int returnValue = 0;
            SolveDetail solveResult;

            for (int i = 0; i < args.Length - 1; i++) {
                if (args[i] == "-D") {
                    if (!int.TryParse(args[i + 1], out drawCount)) {
                        Console.WriteLine($"Invalid DrawCount argument {args[i + 1]}. Defaulting to 1.");
                        drawCount = 1;
                    }
                    i++;
                } else if (args[i] == "-S") {
                    if (!int.TryParse(args[i + 1], NumberStyles.AllowThousands, null, out maxStates)) {
                        Console.WriteLine($"Invalid MaxStates argument {args[i + 1]}. Defaulting to 50,000,000.");
                        maxStates = 50_000_000;
                    }
                    i++;
                } else if (args[i] == "-X") {
                    boardState = args[i + 1];
                    i++;
                } else if (args[i] == "-M") {
                    moveSet = args[i + 1];
                    i++;
                } else if (args[i] == "-s") {
                    simplified = 's';
                }
            }
            if (args[args.Length - 1] == "-s") {
                simplified = 's';
            }

            Stopwatch sw = new Stopwatch();
            sw.Start();

            if (boardState != null)
            {
                solveResult = SolveGameForBoard(boardState, drawCount, maxStates, simplified);
            }
            else if (cardSet.Length < 11)
            {
                uint.TryParse(cardSet, out uint seed);
                solveResult = SolveGame(seed, drawCount, moveSet, maxStates, simplified);
            }
            else
            {
                solveResult = SolveGame(cardSet, drawCount, moveSet, maxStates, simplified);
            }

            sw.Stop();
            if (simplified == null) Console.WriteLine($"Done {sw.Elapsed}");

            if (simplified == 's') {
                switch (solveResult.Result) {
                    case SolveResult.Solved:
                    case SolveResult.Minimal:
                        returnValue = 0;
                        break;
                    case SolveResult.Unknown:
                        returnValue = 6;
                        break;
                    case SolveResult.Impossible:
                        returnValue = 5;
                        break;
                }
            }

            return returnValue;
        }
        private static SolveDetail SolveGameForBoard(string boardState, int drawCount = 1, int maxStates = 50_000_000, char simplified = ' ')
        {
            Board board = new Board(drawCount);
            board.SetState(boardState);
            board.AllowFoundationToTableau = true;

            return SolveGame(board, maxStates, simplified);
        }
        private static SolveDetail SolveGame(uint deal, int drawCount = 1, string movesMade = null, int maxStates = 50_000_000, char simplified = ' ') {
            Board board = new Board(drawCount);
            board.ShuffleGreenFelt(deal);
            if (!string.IsNullOrEmpty(movesMade)) {
                board.PlayMoves(movesMade);
            }
            board.AllowFoundationToTableau = true;

            return SolveGame(board, maxStates, simplified);
        }
        private static SolveDetail SolveGame(string deal, int drawCount = 1, string movesMade = null, int maxStates = 50_000_000, char simplified = ' ') {
            Board board = new Board(drawCount);
            board.SetDeal(deal);
            if (!string.IsNullOrEmpty(movesMade)) {
                board.PlayMoves(movesMade);
            }
            board.AllowFoundationToTableau = true;
            
            return SolveGame(board, maxStates, simplified);
        }
        private static SolveDetail SolveGame(Board board, int maxStates, char simplified) {
            if (simplified == ' ') {
                Console.WriteLine(board);
            }

            SolveDetail result = board.Solve(250, 15, maxStates);

            if (simplified == ' ') {
                Console.WriteLine($"Moves: {board.MovesMadeOutput}");
                Console.WriteLine();
                Console.WriteLine($"(Deal Result: {result.Result} Foundation: {board.CardsInFoundation} Moves: {board.MovesMade} Rounds: {board.TimesThroughDeck} States: {result.States} Took: {result.Time})");
            }
            else if (simplified == 's') {
                Console.WriteLine(result.Result);
            }

            return result;
        }
    }
}
