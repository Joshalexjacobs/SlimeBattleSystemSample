# Slime Battle System Unity Sample
A sample Unity project that utilizes the [SlimeBattleSystem library.](https://github.com/Joshalexjacobs/SlimeBattleSystem)

![Unity Sample Gif](https://i.imgur.com/S3mjjGf.gif)

To run, just clone this repo and open it with Unity version 2021.3.5f1 or later.

This project currently features 2 example scenes that allows the player to fight a Slime or a Skeleton. I also included the ability to cast Hurt or Heal, use an Herb, and attempt to flee from combat. The combat logic is handled by the SlimeBattleSystem and takes very few liberties (eg. handling item drops). 

The combat loop can be found in [`BattleController.cs`](https://github.com/Joshalexjacobs/SlimeBattleSystemSample/blob/main/Assets/Scripts/BattleController.cs#L77).

All battle system methods can easily be accessed via the `SlimeBattleSystem` namespace:
```csharp
using SlimeBattleSystem;

...

var orderedParticipants = BattleSystem.DetermineTurnOrder(participants);

```
