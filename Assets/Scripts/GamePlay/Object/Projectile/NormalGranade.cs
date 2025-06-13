using System.Collections;
using UnityEngine;

public class NormalGranade : GranadeBase
{
    /****** Public Members ******/

    public override ProjectileType  CurrentPojectileType        => ProjectileType.NormalGranade;
    public override EffectType  GranadeEffect                   => EffectType.NormalExplosion;


    /****** Protected Members ******/


    /****** Private Members ******/
}
