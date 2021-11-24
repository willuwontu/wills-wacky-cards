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
  - Counterfeit Ammo (Curse)
  - Crooked Legs (Curse)
  - Driven to Earth (Curse)
  - Easy Target (Curse)
  - Misfire (Curse)
  - Needle Bullets (Curse)
  - Pasta Shells (Curse)
  - Slow Reflexes (Curse)
  - Wild Shots (Curse)
  - Momentary Confusion (Curse)
  - Eroding Darkness (Curse)
  - Shaky Bullets (Curse)
  - Rabbits Foot (Curse)
  - Lucky Clover (Curse)
  - Defective Trigger (Curse)
  - Misaligned Sights (Curse)
  - Damnation (Curse)
  - Fragile Body (Curse)
  - Ammo Regulations (Curse)
  - Air Resistance (Curse)
  - Lead Bullets (Curse)
  - Anime Physics (Curse)
  - Take A Number (Curse)
  - Heavy Shields (Curse)
- Gatling Gun (Rare)
- Plasma Rifle (Rare)
- Plasma Shotgun (Rare)
- Unstoppable Force (Common)
- Immovable Object (Common)
- Hot Potato (Common)
- Reroll (Uncommon)
- Table Flip (Rare)
- Savage Wounds (Uncommon)
- Ritualistic Sacrifice (Rare)
- Forbidden Magics (Rare)
- Purifying Light (Rare)
- Cursed Knowledge (Common)
- Adrenaline Rush (Uncommon)
- Endurance Training (Uncommon)
- Hiltless Blade (Common)
- Holy Water (Common)
- Corrupted Ammunition (Rare)
- Runic Wards (Rare)
- Cleansing Ritual (Uncommon)

----
## v 1.4.8
- Renamed Rabbit's Foot to Rabbits Foot since `'` breaks the game.

----
## v 1.4.7
- More bug squashing

----
## v 1.4.6
- Fixed a bug where Wild Shots would stick around after being removed.

----
## v 1.4.5
- Fixed a bug with Runic Wards keping people from dying when they hit the floor and walls.
- Runic Shields now triggers on 600 damage taken instead of 200.
- Did a balancing pass on curses.
- Added 10 new curses (Defective Trigger, Misaligned Sights, Damnation, Fragile Body, Ammo Regulations, Air Resistance, Lead Bullets, Anime Physics, Take A Number, Heavy Shields)
- Added 2 new curses (Rabbit's Foot, Lucky Clover). These ones actually buff those who get them.
- Purifying Light only grants commons now.

----
## v 1.4.4
- Various Curse Rebalancing
- New Curse (Shaky Bullets)

----
## v 1.4.3
- Various bug fixes.

----
## v 1.4.2
- Runic Wards now grants a curse for every 200 damage dealt to the card's holder. Yes, this includes self-damage. Quit hitting yourself.

----
## v 1.4.1
- Quick patch to a bug

----
## v 1.4.0
- 5 new cards (Eroding Darkness, Holy Water, Cleansing Ritual, Runic Wards, Corrupted Ammunition)
- Lots of fixes.

----
## v 1.3.2
- 1 new card (Hiltless Blade)
- There is now a setting to toggle Reroll and Tableflip off (by default they're allowed), so you don't have to worry about if they're disabled or not.
- Patches to code logic and foundations laid for other cards.

----
## v 1.3.1
- 2 new cards added (Adrenaline Rush, Endurance Training)
- Momentary Confusion is no longer confused about being a curse.

----
## v 1.3.0
- 6 new cards added (Savage Wounds, Ritualistic Sacrifice, Forbidden Magics, Purifying Light, Cursed Knowledge, Momentary Confusion)

----
## v 1.2.10
- Hex now works again.

----
## v 1.2.9
- Patched some logic in Hex

----
## v 1.2.8
- Null reference errors caused by cards having issues when being removed, no longer causes reroll and table flip to break.
- CurseManager and RerollManager have been split off into their own mod.

----
## v 1.2.7
- Reroll no longer causes the game to enter into statis anymore.
- Curses will once again attempt to respect rarity.

----
## v 1.2.6
- hotfix for hex and table flip

----
## v 1.2.5
- The Curse Manager was refactored to have a static instance instead of static functions.
- Curses now respect rarity when being randomly picked, WWC currently only provides common curses.
- Added 2 new cards (Reroll, Table Flip)
- Minigun has been disabled while testing is being done to figure out the issues with it.
- Plasma weapons can no longer fire during the cease fire at the start of a match.

----
## v 1.2.4
- Hot Potato no longer deletes itself, nor gives away all curses you possess.
- Minigun overheat no longer persists after death. Phoenix + other revive cards may still not work properly with it though.
- Plasma Charge Bars now sync, projectile velocity remains elusive however.

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