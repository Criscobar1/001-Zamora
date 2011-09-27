// --------------------------------------------------------------------------------------------------------------------
// <copyright file="Brand.cs" company="RedCelular">
//   2011
// </copyright>
// <summary>
//   Cellphones brands catalogs.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace RedCelular.Framework.Catalogs
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;

    using Castle.ActiveRecord;

    /// <summary>
    /// Cellphones brands catalogs.
    /// </summary>
    [ActiveRecord]
    public class Brand : CatalogBase<Brand>
    {
        /// <summary>
        /// Gets or sets the logo image path.
        /// </summary>
        /// <value>
        /// The logo image path.
        /// </value>
        public ImportAttribute Logo { get; set; }
    }
}
