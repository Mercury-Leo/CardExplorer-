using System;

namespace Database.API.Card.Extra
{
    [Serializable]
    public abstract class ExtraData
    {
        /// <summary>
        /// Validate the extra data.
        /// </summary>
        /// <exception cref="ArgumentNullException">If any of the data fields is null</exception>
        public abstract void Validate();
    }
}