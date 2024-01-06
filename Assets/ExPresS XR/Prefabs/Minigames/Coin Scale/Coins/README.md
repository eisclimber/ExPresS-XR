# Coin Scale Minigame

The goal is simple: Find the fake coin!  
What is not simple is the fact that you can not tell the wrong coin apart.   
The only difference between the coins is that the fake coin is made of lighter and cheaper metal.  
So all you got is your trusty scale to find it.

## Solution
The fastest solution would be to weigh all coins in equal size groups. If there is an odd number, put that coin to the side.  
There are three possible outcomes:

- The **left** is lighter, so it must contain the fake coin. Remove weighing the coins from the **right** group.
- The **right** is lighter, so it must contain the fake coin. Remove weighing the coins from the **left** group.
- Neither group is lighter, so both groups weigh equal while having the same number of coins. The **remaining odd coin** must be the fake. Choose it!

If you did not manage to reduce your selection to a single coin, repeat the process until you did.