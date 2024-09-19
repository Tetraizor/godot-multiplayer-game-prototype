namespace Tetraizor.Interface.Entity;

using Tetraizor.Entity;
using Tetraizor.Enums;

public interface IHittable
{
    public void Hit(IEntity sender, int damage, DamageType damageType);
}