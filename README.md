# Mod
A mod/patch for Sovereignty: Crown of Kings game. 

Compiled patch can be downloaded from releases tab or you can compile library from source youself. VS 2017 was used with .NET Framework 4.5. Compile with Release/x86.

Original decompiled game library code source is in "DecompiledSource" folder.

Game Data files changed by this fork (UI scripts, XML, text and images) are in the "GameData" folder, which mirrors the game folder. Copy its contents into the game folder, overwriting existing files.

====================================================

Bug Fixes

1. The Horde bug should now be fixed, along with some additional bug fixes.

2. Fixed bugged buildings.

3. Fixed an AI unit movement bug that led to stalled armies in enemy provinces without an ongoing siege.


Strategy Map

1. Added fog of war to the strategy layer. Armies with scouts have partial information about armies in bordering provinces.

2. Introduced unit attrition during sieges of enemy provinces. The level of attrition is based on terrain and commander presence.

3. Added building cards with descriptions and updated art: 69 cards.

4. Added a new map filter layer that shows provinces affected by plague. Plague duration has been increased.

5. Trade should not be possible with low relations. Good realms will not trade with orcs and necromancers.


Battles and AI

1. Tweaked strategic and tactical AI. Iron Barony has improved strategic AI. The Boruvian Empire has a custom unit draft scheme.

2. AI will no longer attack with small armies and will always retreat when totally outnumbered.

3. Improved autobattle. Added terrain and unit modifiers, including cavalry and flying units, a battle log, and commander cards.

4. In autobattle, you can now right-click a unit icon to open its unit card. The card can be dragged around with the mouse.

5. Armies will retreat from autobattle if many units have low morale and fail their morale checks.

6. Commanders have names now. Commander cards show the number of battles won.


Interface

1. The province hover card shows the province's buildings and free building slots, and has fixed Revolt, Income, Research and Terrain lines above the description.

2. Income and Research have breakdown tooltips: economy level, buildings, spells and distance to the capital for income, and the base value plus each building or spell for research.

3. Income breakdowns show how far the province is from the capital, e.g. "Distance to Capitol (4): -158", using the same calculation as the actual income.

4. The message log no longer lists treaties between AI realms. Battle reports are shown in white, other messages in gold.

5. The plague map button has its own icon, minimap tab and tooltip.

6. Unit icons in the army panel show promotion medals and a badge for Garrison units.

7. Tactical battle tiles always show health and morale bars, not only when damaged.

8. Fixed the message log showing a stretched image after loading a game, and a crash when returning to the main menu.

