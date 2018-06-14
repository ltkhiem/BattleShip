# Battle Ship

#### CS494 - Internetworking Protocols - Final Project

This is a demonstration of Socket Programming which is implemented along with a small battle ship game. Here are some screenshots of the game:



## Introduction

The game rule is simple as other battle ship games. Two players will be connected and battle with each other. Everyone has **five** ships with various length:

- **Two** ships with length of **four**

- **Two** ships with length of **three**

- **One** ship with length of **one**



They can choose how to **deploy** their ships and change their **orientation** also (by clicking **right** mouse). After two players are ready, the battle would start. In each turn, the player will choose a position on the oponent's map to fire the rocket (by clicking on **a cell** of the grid). If the rocket hits a part of the oponent's ship, there will be an explosion animation. Otherwise, a water drop effect will appear. The game ends when one player ran out of ship (all of his ships were eliminated).



## Try It


First, open Command Prompt and check your IP Address by typing the command:

```
 ipconfig
```

Next, put your IP address in the ipconfig.txt at:

```
<PROJECT_DIRECTORY>/Release/Client/ipconfig.txt
```

Now, you can start the server by executing the command:

```
start /d "<PROJECT_DIRECTORY>/Release/Server" ServerSocket.exe
```
Finally, start the game at the location:

```
<PROJECT_DIRECTORY>/Release/Client/BattleShips.exe
```



## Implementation Details


This game is implemented in a simple client-server model. On the server side, there exists a single lobby where all the clients will wait for their match. The server will pick two players randomly to match them into a game. 

All the actions at client are captured and sent to the server as a package in socket channel. The server will then analyze these packages, update the game state and send back the information to the corresponding game players. Next, the clients will read the response from server and generate the appropriate animations and prompts.



## Authors

Tu-Khiem Le (github.com/ltkhiem)

Van-Tu Ninh (github.com/nvtu)

