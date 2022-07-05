

public class HealthSystem
{
   private int _health;
   private readonly int _maxHealth;
   public HealthSystem(int maxHealth)
   {
      this._maxHealth = maxHealth;
      this._health = maxHealth;
   }

   public int GetHealth()
   {
      return _health;
   }

   public void Damage(int damageAmount)
   {
      _health -= damageAmount;
      if (_health < 0) _health = 0;
   }

   public void Heal(int healAmount)
   {
      _health += healAmount;
      if (_health > _maxHealth) _health = _maxHealth;
   }
}
