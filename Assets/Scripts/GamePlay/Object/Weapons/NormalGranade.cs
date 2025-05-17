using System.Collections;
using UnityEngine;

public class NormalGranade : GranadeBase
{
    /****** Public Members ******/

    public override WeaponType  WeaponType      => WeaponType.NormalGranade;
    public override EffectType  GranadeEffect   => EffectType.NormalExplosion;


    /****** Protected Members ******/


    /****** Private Members ******/
}
