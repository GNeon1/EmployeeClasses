[!["Buy Me A Coffee"](https://www.buymeacoffee.com/assets/img/custom_images/orange_img.png)](https://www.buymeacoffee.com/DaJadeNinja)

**EmployeeClasses**

~~~
DISCLAIMER: This project is in early stages of development. This version has been made public for beta testing
~~~

This mod adds a class selection mechanic to Lethal Company to provide more opportunities for teamwork and ways to play.

The class selection menu may be accessed from the pause screen while in orbit. (The host has the power to change class anywhere for testing purposes. This is a temporary feature that will be removed)

The available classes are as follows:

<details>
 <summary><b>Scout</b></summary>

Agile but fragile. They can quickly traverse the facility and outrun monsters with ease. This is coupled with less health and a heightened weight penalty. Getting in is easy, but getting out? Not so much.

<ins>Pros:</ins>
> +30% Sprint Speed <br/>
> +80% Oxygen Reserves

<ins>Cons:</ins>
> +70% Weight Penalty (when carrying over 20lbs) <br/>
> -50% Max HP

<ins>Ability: BEACON</ins>
> Activate a loud beacon for five seconds. During this period, nearby monsters will be alerted to your presence and target you over any other players. The sprint meter does not deplete while the beacon is active <br/>
> [Cooldown: 60 seconds]
</details>

<details>
 <summary><b>Brute</b></summary>

Brutes are tough and slow. They boast additional health and a very small weight penalty which allows them to carry heavier items with impunity. The brute can safely fight off monsters and haul heavy loot without much difficulty.

<ins>Pros:</ins>
> +100% Max HP <br/>
> +100% Attack Damage <br/>
> +Intimidates Baboon Hawks <br/>
> -50% Weight Penalty

<ins>Cons:</ins>
> -15% Overall Speed <br/>
> +5% Footstep Volume

<ins>Ability: KICK</ins> <br/>
> Wind up and kick a monster (or coworker) in the face, dealing one damage point and stunning them for two seconds. Kicking costs 40% of your stamina.
</details>

<details>
 <summary><b>Researcher</b></summary>

A sneaky boi capable of healing and enhanced perception. He turns fallen monsters into something useful.

<ins>Pros:</ins>
> +100% Scan Range <br/>
> +Scanner Goes Through Walls <br/>
> -50% Footstep Volume

<ins>Cons:</ins>
> -20% Max Health <br/>
> +40% Fall Damage <br/>
> +20% Weight Penalty

<ins>Ability: HARVEST</ins> <br/>
> Crouch over a dead enemy to harvest their body for monster extract.

<ins>Ability: SYNTHESIZE</ins> <br/>
> Hold F to synthesize a stim shot from monster extract. One will always be supplied automatically from orbit.

<ins>Ability: STIM</ins> <br/>
> Spend a stim shot to heal an ally or yourself 30% of their health instantly and 10% per second until full. Taking damage interrupts the regeneration. Grants a mild TZP effect which is even higher for players at full health.
</details>

<details>
 <summary><b>Maintenance</b></summary>

A specialized class capable of interacting with technology found in the facility. Maintenance carries a dim headlamp, freeing up an extra slot to carry loot. The extra equipment makes mobility clunky and loud.

<ins>Pros:</ins>
> +Time is always visible when crouching <br/>
> +15% Sprint Time <br/>
> -75% Bullet Damage <br/>
> -50% Fog Density

<ins>Cons:</ins>
> +10% Footstep Volume <br/>
> -10% Sprint Speed

<ins>Ability: HEADLAMP</ins> <br/>
> Toggle a dim headlamp by pressing F. The headlamp costs 0.3% energy per second.

<ins>Ability: HACK</ins> <br/>
> Look at a turret/door/mine and hold F for 3 seconds to temporarily deactivate the selected hazard or toggle blast doors. Hacking costs 15% energy.
> [Cooldown: 5 seconds]

<ins>Ability: SOLAR TANKS</ins> <br/>
> Maintenance's energy bar is fully restored when landing on a planet. Each ability expends energy from it. Standing outside of the facility will gradually recharge the energy. (variable due to weather conditions)
</details>

<br/>

**Update History**

0.1.2 - Bug fixes
 + Fixed jumping glitches
 + Maintenance
    + Hacking sound is quieter
    + Hacking no longer goes through walls
 + Brute
	+ Stats
		+ Nerfed weight modifier (40% => 50%)
	+ Kick
		+ Costs more stamina (20% => 40%)
		+ Cooldown removed
		+ Enemies no longer receive invincibility after being hit
		+ No longer knocks down doors


0.1.1 - Fixed a stoopid mistake with dependencies

0.1.0 - Public beta launched
