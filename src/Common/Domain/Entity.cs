﻿using System.ComponentModel.DataAnnotations;

namespace Meta.Common.Domain
{
    /// <summary>
    /// Сущность.
    /// </summary>
    public abstract class Entity<TKey> where TKey : struct
    {
        /// <summary>
        /// Идентификатор.
        /// </summary>
        [Key]
        public virtual TKey Id { get; set; }
    }
}
