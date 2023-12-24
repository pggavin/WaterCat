public interface IOnDamaged
{
    public void Damaged(int curHealth);
}
// Entities call this when they are hurt, for fx, healthbar, boss phases, hitsounds, etc