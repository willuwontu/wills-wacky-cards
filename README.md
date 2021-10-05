# Will's Wacky Cards

This is a mod containing a few cards I came up with while fiddling around with the game.

- Ammo Cache (Common)
- Slow Death (Rare)
- Vampirism (Rare)
- Shotgun (Uncommon)
- Basic Physics (Common)
- Alternate Universe Physics (Common)
- Minigun (Rare)
- Wild Aim (Common)
- Boots of Leaping (Common)
- Running Shoes (Common)
- Hex (Uncommon)
  - Bleeding Wounds (Curse)
  - Cheap Magazines (Curse)
  - Crooked Legs (Curse)
  - Driven to Earth (Curse)
  - Easy Target (Curse)
  - Misfire (Curse)
  - Needle Bullets (Curse)
  - Pasta Shells (Curse)
  - Slow Reflexes (Curse)
  - Wild Shots (Curse)
- Gatling Gun (Rare)
- Plasma Rifle (Rare)
- Plasma Shotgun (Rare)
- Unstoppable Force (Common)
- Immovable Object (Common)
- Hot Potato (Common)

---- 
## Features for other mods
### CurseManager
You can add curses for usage by utilizing the `CurseManager` found in `WillsWackyCards.Utils`.

Simply toss in a `using WillsWackyCards.Utils;` and use `CustomCard.BuildCard<CurseCardName>(cardInfo => { CurseManager.RegisterCurse(cardInfo); });` to register a curse for usage.

Make sure to add `CurseManager.curseCategory` to your cards categories, so that it cannot be selected by players.

It also contains various other utilities for handling curses.

----
## v 1.2.3
- New card (Hot Potato) only shows up if you're cursed.
- Attempts to Sync Plasma weapon charge bars and bullet velocity have beeen made.
- Hex no longer gives additional curses when readded as part of removing another card.

----
## v 1.2.2
- Curses are no longer hidden, they can be disabled or enabled individually in the toggle cards menu. If there are no enabled curses, hex will simply do nothing.
- Immovable Object and Unstoppable Force are re-enabled and now work properly.

----
## v 1.2.1
- Disabled Unstoppable Force, and Immovable Object while they're being fixed.
----
## v 1.2.0
- New Cards (Unstoppable Force, Immovable Object)
----
## v 1.1.6
- New Cards (Plasma Rifle, Plasma Shotgun)
----
## v 1.1.5
- Small fixes and some debugging tools added.
----
## v 1.1.4
- New Curse
- Fixes for Hex
----
## v 1.1.3
- Gatling Gun is actually a rare now.
----
## v 1.1.2
- Shotgun is now a rare.
- New Card (Gatling Gun)
- Fixes
----
## v 1.1.1
- Shotgun was changed to have a minimum reload time and had other stat changes made as well.
----
## v 1.1.0
- Minigun persisting after being stolen and between rounds is fixed.
- New cards
----
## v 1.0.8
- Code logic patched.
- New Cards
----
## v 1.0.7
- Balance Changes
- More cards blacklisted by Shotgun and Minigun
----
## v 1.0.6
- New Card
- More cards blacklisted by Shotgun and Minigun
----
## v 1.0.5
- Fixed an isue where sometimes regular cards would not be added to a player after selecting them.
----
## v 1.0.4
- Fixed issues where blacklisting was improperly done.
----
## v 1.0.3
- More tweaks to cards
- Added two new cards
----
## v 1.0.3
- More tweaks to cards
- Added two new cards

----
## v 1.0.2
- Apparently the noise for Vampirism was annoying. Made it happen less.
- Couple of other balancing tweaks.
----
## v 1.0.1
- Minor patches on card logic
- Shotgun is now incompatible with Pong (BSC) and Flamethrower (CR)
- Slow Death doesn't slow as much.