// --------------------------------------------------------------------------------------------------------------------
// <copyright file="CatalogBase.cs" company="RedCelular">
//   2011
// </copyright>
// <summary>
//   Base class for the catalogs. Every catalogs class must extend this class.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace RedCelular.Framework.Catalogs
{
    using System;

    using Castle.ActiveRecord;

    /// <summary>
    /// Base class for the catalogs. Every catalogs class must extend this class.
    /// </summary>
    /// <typeparam name="T">
    /// A catalog class.
    /// </typeparam>
    public abstract class CatalogBase<T> : RedCelularActiveRecordBase<T> where T : CatalogBase<T>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="CatalogBase&lt;T&gt;"/> class.
        /// </summary>
        protected CatalogBase()
        {
            CreationDate = DateTime.Now;
        }

        /// <summary>
        /// Gets or sets the name.
        /// </summary>
        /// <value>
        /// The name.
        /// </value>
        [Property]
        public string Name { get; set; }

        /// <summary>
        /// Gets or sets the creation date.
        /// </summary>
        /// <value>
        /// The creation date.
        /// </value>
        [Property]
        public DateTime CreationDate { get; set; }
    }
}