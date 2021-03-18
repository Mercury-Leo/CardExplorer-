using System;
using UnityEngine;

namespace Database.API.Card.Extra
{
    [Serializable]
    public class MinionExtraData : ExtraData
    {
        [SerializeField] 
        private int m_Health;
        public int Health => m_Health;
        
        [SerializeField]
        private int m_AttackDamage;
        public int AttackDamage => m_AttackDamage;
        
        public MinionExtraData()
        {
        }
        
        public MinionExtraData(int health, int attackDamage)
        {
            m_Health = health;
            m_AttackDamage = attackDamage;
        }
        
        public override void Validate()
        {
            
        }
    }
}