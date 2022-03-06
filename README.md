Zombie Survivor Kata
====================
Source: [https://github.com/ardalis/kata-catalog](https://github.com/ardalis/kata-catalog)

# Instructions #

This kata constructs a model for a zombie boardgame's survivors. If you enjoy the kata, you may find the Zombicide series of boardgames fun as well. Complete each step before reading ahead to the next one. Revise your design to react to new requirements as they appear.

### Optional challenge

Do not read all the steps at once, try completing a single step before learning about the next one

## Step One: Survivors

The zombie apocalypse has occurred. You must model a Survivor in this harsh world. Sometimes, they get hurt, and even die.

- [x] Each **Survivor** has a **Name**.
- [x] Each Survivor begins with 0 **Wounds**.
- [x] A Survivor who receives 2 Wounds dies immediately; additional Wounds are ignored.
- [x] Each Survivor starts with the ability to perform 3 Actions per turn.

## Step Two : Equipment

Survivors can use equipment to help them in their mission.

- [x] Each Survivor can carry up to 5 pieces of **Equipment**. 
	- [x] Up to 2 pieces of carried Equipment are "In Hand"; the rest are "In Reserve".
	- [x] Examples of Equipment: "Baseball bat", "Frying pan", "Katana", "Pistol", "Bottled Water", "Molotov"

- [x] Each Wound a Survivor receives reduces the number of pieces of Equipment they can carry by 1.
	- [x] If the Survivor has more Equipment than their new capacity, choose a piece to discard (implement however you like).

## Step Three : The Game

A Game includes one or more Survivors, as well as other Game elements that are outside the scope of this kata.

- [x] A **Game** begins with 0 Survivors.
- [x] A Game can have Survivors added to it at any time.
	- [x] Survivor Names within a Game must be unique.
- [x] A Game ends immediately if all of its Survivors have died.

## Step Four : Experience and Levels

As Survivors overcome zombies, they gain experience.

- [x] Each Survivor begins with 0 **Experience**.
- [x] Each Survivor has a current **Level**.
- [x] Each Survivor begins at Level Blue.
- [x] Each time the Survivor kills a zombie, they can 1 Experience.
- [x] Levels consist of (in order): Blue, Yellow, Orange, Red.
	- [x] When a Survivor exceeds 6 Experience, they advance ("level up") to level Yellow.
	- [x] When a Survivor exceeds 18 Experience, they advance to level Orange.
	- [x] When a Survivor exceeds 42 Experience, they advance to level Red.
- [x] A Game has a Level (Level here is identical to Level for a Survivor).
- [x] A Game begins at Level Blue.
- [x] A Game Level is always equal to the level of the highest living Survivor's Level.

## Step Five : Output

The Game includes a running history of events that have taken place as it has been played. Managing game history is a Game responsibility.

- [x] A Game's **History** begins by recording the time the Game began.
- [x] A Game's History notes that a Survivor has been added to the Game.
- [x] A Game's History notes that a Survivor acquires a piece of Equipment.
- [x] A Game's History notes that a Survivor is wounded.
- [x] A Game's History notes that a Survivor dies.
- [x] A Game's History notes that a Survivor levels up.
- [x] A Game's History notes that the Game Level changes.
- [x] A Game's History notes that the Game has ended when the last Survivor dies.

## Step Six : Advancement

### Skill tree (Exp points)

|Yellow|Orange|Red|
|--|--|--|
|+1 Action (6)|+1 Die (Ranged) (18)|+1 Die (Melee) (42)
|--|+1 Free Move Action (61)|Hoard (86)
|--|--|Tough (129)


As the Game proceeds, Survivors get better.

NOTE on skills concepts:
- Potential skill: a skill that is available for the survivor to unlock at will after reaching a certain XP threshold
- Available skill: a.k.a. Potential skill
- Locked skill: a skill that is not yet available for the survivor to attain through any means
- AutoUnlockable skill: a skill that is automatically unlocked once the survivor reaches a specific XP threshold, these skills are applied immediately after reaching the XP threshold (> 50 XPs)
- Unlocked skill: a skill that is currently in use by a survivor, whether it has been unlocked willingly or auto-unlocked after reaching the XP threshold of 50 XPs


- [x] A set of potential **Skills** and the Levels where they are unlocked is called a **Skill Tree**.
- [x] Each Survivor begins with a Skill Tree.
	- [x] A Skill Tree consists of *potential* skills and *unlocked* skills.
	- [x] A Skill Tree begins with 1 *potential* skill at level Yellow, 2 at Orange, and 3 at Red.
	- [x] All Skill Trees have the same Yellow skill: "+1 Action".
	- [x] Each Survivor can have a Skill Tree unique to themselves.
- [x] When a Survivor first advances to a particular level they may unlock one available skill from that level.
	- [x] At level Yellow, only the "+1 Action" skill should ever be available, so it will be unlocked.
- [x] A Survivor who has "+1 Action" should have one additional Action (a total of 4).
- [x] Additional Skills: "+1 Die (Ranged)", "+1 Die (Melee)", "+1 Free Move Action", "Hoard", "Sniper", "Tough".
	- [x] A Survivor who has unlocked the "Hoard" skill can carry one additional piece of Equipment.
- [x] When a Survivor advanced beyond 43 experience, they remain Level Red but restart through the skill tree a second time.
	- [x] Upon reaching Yellow again (43 + 7 = 50 total), no more potential skills are available.
	- [x] Upon reaching Orange again (43 + 18 = 61 total), a second Orange skills is unlocked.
	- [x] Upon reaching Red again (43 + 43 = 86 total), a second Red skill is unlocked.
- [x] A Survivor can restart a second time, unlocking their last Red skill at (43 + 43 + 43 = 129 total) experience.
- [] The Game History notes that a Survivor has acquired a new Skill.

## Notes

How did you model interaction between the Survivors and the Game? Do Survivors have direct access to the Game in which they are playing? If so, can code in a Survivor's methods directly modify the state of other Survivors in the same game? Is that a potential problem?

How did you model the Game History? Did you make sure to keep the responsibility for tracking history a concern of the Game, and not of individual Survivors?