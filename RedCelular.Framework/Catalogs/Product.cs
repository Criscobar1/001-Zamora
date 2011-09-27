// --------------------------------------------------------------------------------------------------------------------
// <copyright file="Product.cs" company="RedCelular">
//   2011
// </copyright>
// <summary>
//   Product catalog entity.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace RedCelular.Framework.Catalogs
{
    using System.Collections.Generic;

    using Castle.ActiveRecord;

    /// <summary>
    /// Product catalog entity.
    /// </summary>
    [ActiveRecord]
    public class Product : CatalogBase<Product>
    {
        /// <summary>
        /// Gets or sets the model.
        /// </summary>
        /// <value>
        /// The model.
        /// </value>
        [Property]
        public string Model { get; set; }

        /// <summary>
        /// Gets or sets the images.
        /// </summary>
        /// <value>
        /// The images.
        /// </value>
        public IList<ProductImage> Images { get; set; }
    }
}
