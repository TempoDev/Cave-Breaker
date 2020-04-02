# Cave Breaker

A match 3 parody mobile game.

## About

Cave breaker is a mobile game parodying match 3 games created with godot for the EPITECH hub's projects.

### Sypnosys

A terrible monster is chasing you. As you flee you run into a cave. Sadly, this cave is a nest full of eggs.
How will you get out of this mess ?

### Gameplay

The Game is divided into three big parts : The Board, the levels and the menu.

#### The Board


![Board](https://github.com/TempoDev/Cave-Breaker/blob/doc/thumbnail_laby_1.png)

The board is the level selection screen. It's also where you can get all the informations you need and prepare yourself for adventure.

You begin at the tile below and have to find the exit at the top of the map. You can move by following the differents ways but :
  * each tile has his own level.
  * You'll have to finish the level of a tile to pass throught.
  * playing a level cost energy
  * the tiles you have not explored are hidden, so you can't know where it lead
  * you can see the status of a level and it's completion rank from here.

You can access the menu from this screen : simply slide up from the bottom and it will appear.
You can also see and edit your team from here (just click on it).

##### Choosing you team

![Board](https://github.com/TempoDev/Cave-Breaker/blob/doc/thumbnail_select.png)

You can click on the icons to look at your monsters and get informations.
Just drag them on you team's icons to change monsters.

#### The levels

![levels](https://github.com/TempoDev/Cave-Breaker/blob/doc/thumbnail_board.png)

Levels are the core of the gameplay.
   * If you leave, you won't get your energy back
   * You have a board full of colored eggs.
   * The eggs follows gravity rule : if there is not egg or ground beside it, it will fall.
   * If you align 3 eggs of the same color, you lose
   * You have to destroy ALL the eggs to win.
   * To destroy eggs, you'll have to use your monsters attacks.
   * Each monster has his own special attack, so manage your team wisely.
   * You can change monster / attack by clicking on another monster icon, beside the game board.
   * On top of the monster icon, the number of remaining possible attack is shown : when it attain 0, it can't attack anymore.
   * If you have no attacks remaining, you lose.
   * You can use items from your invetory to help you.
   
If you win, you can progress on the board, but if you loses, you'll get a sample of the egg you aligned into your hatchery, in order to get new monsters.

#### The menu

![menu](https://github.com/TempoDev/Cave-Breaker/blob/doc/thumbnail_menu%201.png)

The menu permit you to access different screen usefull to get prepared for level.

##### the hatchery

![hatchery](https://github.com/TempoDev/Cave-Breaker/blob/doc/thumbnail_hatcherie.png)

This is where you can hatch your eggs, put your egg on a nest to begin to hatch one. You'll have to wait the time written beside said egg to hatch it. Once hatched, you'll get a random monster between those possible, usable in your fighting team.
You have a limited number of nests, but you can buy some.

##### the farm

![farm](https://github.com/TempoDev/Cave-Breaker/blob/doc/thumbnail_Projet%201.png)

The farm is where you can see your monsters play and... do stuff...
But more importantly, it's where you can take care of your monsters, to make them level up or merge them.
You can merge two identical monsters of same rank to get a monster of higher rank, but you might also want to keep two lower ranked monster in order to use them on a fight.
You can also get basic informations of your monsters and send them to forage items, such as food.
You'll also have to feed them, or they might be less efficient for fight (sleep time can also do the trick).

##### the bestiary

![bestiary](https://github.com/TempoDev/Cave-Breaker/blob/doc/thumbnail_bestary.png)

The bestiary is where you can see all the type of monster you collected. Usefull when you want to have a fast look at what you have and their powers.
You can also have fun "catching them all".

##### the store

![store](https://github.com/TempoDev/Cave-Breaker/blob/doc/thumbnail_shop.png)

The store is where you can buy differents items

##### the inventory

![inventory](https://github.com/TempoDev/Cave-Breaker/blob/doc/thumbnail_items.png)

Your inventory, or backpack, is where you can see your items and use them.

## Installing

Coming soon

## Built With

* [Godot](https://godotengine.org/) - Game engine

## Versioning

Coming soon

## License

This project is licensed under the EPITECH license.
Don't copy or use the content.
