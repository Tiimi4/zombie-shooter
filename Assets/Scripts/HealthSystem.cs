

using System;

public class HealthSystem
{
   private int _health;
   private readonly int _maxHealth;
   public Action OnDeath;
   public Action OnDamaged;
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
      OnDamaged?.Invoke();
      if (_health < 0) _health = 0;
      if(_health == 0) Death();
   }

   public void Heal(int healAmount)
   {
      _health += healAmount;
      if (_health > _maxHealth) _health = _maxHealth;
   }

   public void Death()
   {
      OnDeath?.Invoke();
   }
}
