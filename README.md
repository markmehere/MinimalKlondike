# Minimal Klondike Solver
Finds minimal length solutions for the thoughtful version of Klondike (Patience) Solitaire.

### Piles
  - A = Waste Pile
  - B = Clubs Pile
  - C = Diamonds Pile
  - D = Spades Pile
  - E = Hearts Pile
  - F = Tableau 1
  - G = Tableau 2
  - H = Tableau 3
  - I = Tableau 4
  - J = Tableau 5
  - K = Tableau 6
  - L = Tableau 7

### Deck Format
I took the format from an old web site, it's not the best, but haven't got around to adding anything better.

Each card is 'RRS' where R=Rank and S=Suit.
Rank goes from 01 - 13 and Suit goes from 1 - 4 (Clubs,Diamonds,Hearts,Spades)

ie) 052 = 5 of diamonds

Position of cards in deck string:
```
 A        B  C  D  E

 F  G  H  I  J  K  L
01 02 03 04 05 06 07
   08 09 10 11 12 13
      14 15 16 17 18
         19 20 21 22
            23 24 25
               26 27
                  28

Draw pile 29-52
```

072103023042094134111092051034044074114052123011083122012131091082124064014093033112071104132053133102084041013073063031061043081054113062024021101022032121

Would equate to this (+ represents visible cards), the 7C is the first card to be turned over in the draw pile when drawing one at a time, then TS, KD, etc...:
```
  A        B  C  D  E

  F  G  H  I  J  K  L
+7D TH 2H 4D 9S KS JC
   +9D 5C 3S 4S 7S JS
      +5D QH AC 8H QD
         +AD KC 9C 8D
            +QS 6S AS
               +9H 3H
                  +JD

 7C TS KD 5H KH TD 8S
 4C AH 7H 6H 3C 6C 4H
 8C 5S JH 6D 2S 2C TC
 2D 3D QC
```

### Moves
Are in the format XY, where X is the character of the source pile, and Y is the character of the destination pile. '@' represents a draw

ie) For the above deal with a draw count of 1 running this sequnce of moves would result in the following state:
```
 "IC @@AL KL @@AK @@@@@AE LJ AK LK"

  A        B  C  D  E
 8S          AD    AH
  F  G  H  I  J  K  L
+7D TH 2H 4D 9S KS JC
   +9D 5C 3S 4S 7S JS
      +5D+QH AC 8H QD
             KC 9C 8D
            +QS+6S+AS
            +JD+5H
            +TS+4C
            +9H+3H

 7H 6H 3C 6C 4H 8C 5S
 JH 6D 2S 2C TC 2D 3D
 QC
+TD+KH+KD+7C
```

`
-M "IC @@AL KL @@AK @@@@@AE LJ AK LK" 072103023042094134111092051034044074114052123011083122012131091082124064014093033112071104132053133102084041013073063031061043081054113062024021101022032121
`

### Board State
This release allows you to explicitly specify the board state to work from. This was needed for compatibility with programs that might not
wish to record all moves from scratch. The board state in the diagram just above would be specified like so:

```
8STDKHKD7C7h6h3c6c4h8c5sJh6d2s2cTc2d3dQc;
;
AD;
;
AH;
7D;
Th9D;
2h5c5D;
4d3sQH;
9s4sAcKcQSJDTS9H;
Ks7s8h9c6S5H4C3H;
JcJsQd8dAS
```

Whitespace is ignored however the capitalization of the suit determines whether the card is face-up or face-down.

Also the order of piles is critical with the semi-colon separating the piles. Empty piles are relevant and excluding will result in a
misalignment.

As such "8S" is the card on which there is action. "7h" is the next one to be flipped from the stock and "TD" is
the card that would appear if "8S" was placed on "9D".

Similarly "Th" is face-down as is "8d". But "6S", "5H", "4C" and "3H" are all face-up.

A quirk that has tripped me up is the foundation piles are actually ordered and must be Clubs-Diamonds-Hearts-Spades.


`
-X "8STDKHKD7C7h6h3c6c4h8c5sJh6d2s2cTc2d3dQc;;AD;;AH;7D;Th9D;2h5c5D;4d3sQH;9s4sAcKcQSJDTS9H;Ks7s8h9c6S5H4C3H;JcJsQd8dAS"
`

### Unit tests

The following are solvable yet showing up wrong:

```
./Klondike -D 3 -S 100000 -X "7sJd9hJc4dKd6cQc;AS2S3S4S;AC2C;AH2H3H4H5H6H;AD;KSQHJSTD9S8D7C6D;5D4C3D;8S7H6S;KCQD;3c5sTcTH9C8H;7d2d5C;KHQSJHTS9D8C"  
```

```
./Klondike -D 3 -S 100000 -X "7sJd9hJc4dKd6cQc;AS2S3S4S;AC2C;AH2H3H4H5H6H;AD;KSQHJSTD9S8D7C6D;5D4C3D;8S7H6S;KCQD;3c5sTcTH9C8H;7d2d5C;KHQSJHTS9D8C"
```

```
./Klondike -D 3 -S 100000 -X "7sJd9hJc4dKd6cQc;AC2C;AD;AH2H3H4H5H6H;AS2S3S4S;KSQHJSTD9S8D7C6D;5D4C3D;8S7H6S;KCQD;3c5sTcTH9C8H;7d2d5C;KHQSJHTS9D8C"
```