using System;
using System.Runtime.CompilerServices;
namespace Klondike.Entities {
    public sealed class TalonHelper {
        public readonly Card[] StockWaste;
        public readonly int[] CardsDrawn;
        private readonly bool[] StockUsed;

        public TalonHelper(int talonSize) {
            StockWaste = new Card[talonSize];
            CardsDrawn = new int[talonSize];
            StockUsed = new bool[talonSize];
        }

        public override string ToString() {
            string result = "";
            for (int i = 0; i < StockWaste.Length; i++)
            {
                result += $"{StockWaste[i]} {CardsDrawn[i]} {StockUsed[i]}\n";
            }
            return result;
        }

        [MethodImpl(MethodImplOptions.AggressiveOptimization)]
        public int Calculate(int drawCount, Pile wastePile, Pile stockPile) {
            int Size = 0;
            Array.Fill(StockUsed, false);

            //Check waste
            int wasteSize = wastePile.Size;
            if (wasteSize > 0) {
                StockWaste[Size] = wastePile.BottomNoCheck;
                CardsDrawn[Size++] = 0;
            }
            if (drawCount == 0) return Size;

            //Check cards waiting to be turned over from stock
            int stockSize = stockPile.Size;
            int position = stockSize - drawCount;
            if (position < 0) { position = stockSize > 0 ? 0 : -1; }
            for (int i = position; i >= 0; i -= drawCount) {
                StockWaste[Size] = stockPile[i];
                CardsDrawn[Size++] = stockSize - i;
                StockUsed[i] = true;
            }

            //Check cards already turned over in the waste, meaning we have to "redeal" the deck to get to it
            int amountToDraw = stockSize + 1;
            wasteSize--;
            for (position = drawCount - 1; position < wasteSize; position += drawCount) {
                StockWaste[Size] = wastePile[position];
                CardsDrawn[Size++] = -amountToDraw - position;
            }

            //Check cards in stock after a "redeal". Only happens when draw count > 1 and you have access to more cards in the talon
            if (position > wasteSize && wasteSize >= 0) {
                amountToDraw += stockSize + wasteSize;
                position = stockSize - position + wasteSize;
                for (int i = position; i > 0; i -= drawCount) {
                    if (StockUsed[i]) { break; }
                    StockWaste[Size] = stockPile[i];
                    CardsDrawn[Size++] = i - amountToDraw;
                }
            }

            return Size;
        }
    }
}