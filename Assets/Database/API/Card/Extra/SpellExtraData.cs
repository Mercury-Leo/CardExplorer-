using System;
using UnityEngine;

namespace Database.API.Card.Extra
{
    [Serializable]
    public class SpellExtraData : ExtraData
    {
        [SerializeField]
        private SpellEffect m_Effect;
        public SpellEffect Effect => m_Effect;
        
        [SerializeField] 
        private int m_EffectAmount;
        public int EffectAmount => m_EffectAmount;
        
        public SpellExtraData()
        {
        }
        
        public SpellExtraData(SpellEffect effect, int effectAmount)
        {
            m_Effect = effect;
            m_EffectAmount = effectAmount;
        }

        public override void Validate()
        {
            
        }
    }
}