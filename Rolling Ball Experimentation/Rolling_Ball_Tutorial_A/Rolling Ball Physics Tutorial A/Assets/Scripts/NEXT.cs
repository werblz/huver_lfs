using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NEXT : MonoBehaviour
{

    // Use this for initialization
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }

    /*
     * 
     


    AIRSHIPS
     - Make a collider box that stretches out the entire path of the airship animation, parented 
       to each airship. When placing them, turn it on, start at LowestBuildingHeight (track that)
       and if it collides, move it up a bit. Keep going until it doesn't collide. Then turn off
       the collider. This will ensure ships at below the level of the tall buildings, causing
       even more chaos!
    -  Make collisions with airships do no damage (done) but DO take away from tip for 
       jostling passengers. Make it known. (Audio "Ouch!")?
    - Write a script that moves each airship randomly the animation timeline. I know this was 
      done for GOT
    - ALso make a collider for the sky sphere. When a ship hits it, trigger a script method that
      scales the ship up by scaling the rotator Or the mesh?? This may not work. May be too difficult. 
    - If that works, make eve ry ship hit Restart event if it leaves the collider.


    POWERUPS
    - Make powerups appear over random buildings.
    - Collide with them and you get temporary bonuses, or small permanent ones.
    - Temporary ones can set temporary flags, reset at Shift Over 



    RADAR
     - Make four crack images, each progressing (did that)
     - Now add them to the radars, and make them come in progressively on the 25%s

    - DONE - Make home base. The base you start at but does nothing for you except fill you up 
      and fix your repairs for cheap. Landing back here at any time does nothing. 
      Perhaps it will adomonish you or something

    Actually, make landing there fill and repair at the discounted cost, as if a shift ended

    - Sort upgrades so you get 3 good ones, and the most affordable first.
      I realise this will eventually force you to buy cheap upgrades you may not want
      but it is part of the economy, and I can even build it into corporate culture with
      aphorisms

    - Make upgrades you can't afford red with a red aura, and not choosable, but you CAN see them

    - Make negative cash red.

    - Put in a physics cone to the rear that stops buildings from blocking your view.
      Do this by having a collision thing that turns the building you collide with transparent
      by swapping out materials. While colliding, swap it with a temp Alpha version of the same
      material, and then swap it back when you are no longer colliding with it






    POSSIBLE UPGRADES
    - Station Radar
    - Platform Radar
    - Turbo
    - Strafe
    - Stronger Shields
    - Larger Gas Tank
    - + Strafe Thrust
    - + Turbo Thrust
    - + general thrust
    - Lower Crash Deductible
    - Lower Gas Cost
    - Lower Repair Cost
    - Car Color
    - Car Add-ons (extra meshes we can toss on easily)

    Defaults:
    0 - +5% tanks size
    1 - +5% shields
    2 - -5% crash deductible


    APHORISMS: 

    If you or your family suffers a medical crisis while you are in Huver's emply, don't worry.
    We have a Family Leave program. It's Leave.

    Don't worry, Pilot. Huver will let you hold a deficit balance. You can work it off.
    But you can't buy an upgrade unless you can afford it.

    Huver cares about you, Pilot 149-673-3719. Pilots are our existence!
    We are nothing without your warm body in our seat.

    Don't jostle the customers! Your tip depends on it!

    Watch that fuel! If you run out, it would be an insurance
    nightmare!

    Huver invests in you! Fuel and repairs at a Huver Home Base is always discounted!

    Your radar is your frenemy!

    Aphorisms are good for the soul. Good for company morale. Good for the spreadsheets.

    Cash in the red? Don't worry. Huvver cares about you, Pilot 149-673-3719! You can work it off!

    ANother shift, another ($)

    Your full manual is under the rear passenger seat.

    Crashed? Shit happens! You just pay your deductible and move on. You can always work it off.

    Oh no! Crash! Know what? Bygones. Decutible is $500 and it's all water under the bridgework.

    (At the beginning of the game, input name, then NEVER USE IT!)

    At Huver, we care about you. Input your name.
    Welcome, Pilot 149-673-3719!

    (However, the name is in the save file filename.)

    The smart money upgrades to Strafe early, because it's all about control... right Pilot #149-673-3719?

    Turbo Boost can really save you some $heckles





     */
}
