using NUnit.Framework;
using UnityEngine;

public class ProjectilePoolHandler : IPoolHandler<IProjectile>
{
    public ProjectilePoolHandler(ProjectileType projectileType)
    {
        _projectileType = projectileType;
    }

    public IProjectile Get()
    {
        return PoolManager.Instance.Get<ProjectileType, IProjectile>(_projectileType);
    }

    public void Return(IProjectile weapon)
    {
        Debug.Assert(null != weapon, $"Trying to return null {_projectileType}.");

        PoolManager.Instance.Return(_projectileType, weapon);
    }


    /****** Private Members ******/

    ProjectileType _projectileType;
}