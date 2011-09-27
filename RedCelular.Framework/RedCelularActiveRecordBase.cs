// --------------------------------------------------------------------------------------------------------------------
// <copyright file="RedCelularActiveRecordBase.cs" company="RedCelular">
//   2011
// </copyright>
// <summary>
//   Base class of the framework. Every class maped to a table in databse must extend this class.
//   Exception made ofr relational tables.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace RedCelular.Framework
{
    using System.Web.Script.Serialization;

    using Castle.ActiveRecord;
    using Castle.ActiveRecord.Framework;

    /// <summary>
    /// Base class of the framework. Every class maped to a table in databse must extend this class.
    /// Exception made ofr relational tables.
    /// </summary>
    /// <typeparam name="T">
    /// An ActiveRecord descending class.
    /// </typeparam>
    public abstract class RedCelularActiveRecordBase<T> : ActiveRecordBase<T>
    {
        /// <summary>
        /// Tracks the control of ActiveRecord framework initialization.
        /// </summary>
        private static bool __initialized;

        /// <summary>
        /// Gets or sets the id.
        /// </summary>
        /// <value>The database id.</value>
        [PrimaryKey(Generator = PrimaryKeyType.HiLo, Params = "max_lo=100")]
        public int Id { get; set; }

        /// <summary>
        /// Gets or sets the version.
        /// </summary>
        /// <value>The version.</value>
        [Version]
        private int Version { get; set; }


        /// <summary>
        /// Initializes Castle ActiveRecord framework.
        /// </summary>
        /// <param name="source">The source.</param>
        public static void Initialize(IConfigurationSource source)
        {
            if (!__initialized)
            {
                if (!ActiveRecordStarter.IsInitialized)
                {
                    ActiveRecordStarter.Initialize(typeof(RedCelularActiveRecordBase<T>).Assembly, source);
                }

                __initialized = true;
            }
        }

        /// <summary>
        /// Implements the operator ==.
        /// </summary>
        /// <param name="left">The left hand operand.</param>
        /// <param name="right">The right hand operand.</param>
        /// <returns>The result of the operator.</returns>
        public static bool operator ==(RedCelularActiveRecordBase<T> left, RedCelularActiveRecordBase<T> right)
        {
            return Equals(left, right);
        }

        /// <summary>
        /// Implements the operator !=.
        /// </summary>
        /// <param name="left">The left hand operator.</param>
        /// <param name="right">The right hand operator.</param>
        /// <returns>The result of the operator.</returns>
        public static bool operator !=(RedCelularActiveRecordBase<T> left, RedCelularActiveRecordBase<T> right)
        {
            return !Equals(left, right);
        }

        /// <summary>
        /// Equalses the specified other.
        /// </summary>
        /// <param name="other">The other.</param>
        /// <returns><c>true</c> if are the same reference or have the same Id and Version, otherwise false.</returns>
        public bool Equals(RedCelularActiveRecordBase<T> other)
        {
            if (ReferenceEquals(null, other))
            {
                return false;
            }

            if (ReferenceEquals(this, other))
            {
                return true;
            }

            return other.Id.Equals(Id) && other.Version == Version;
        }

        /// <summary>
        /// Determines whether the specified <see cref="T:System.Object"/> is equal to the current <see cref="T:System.Object"/>.
        /// </summary>
        /// <returns>
        /// true if the specified <see cref="T:System.Object"/> is equal to the current <see cref="T:System.Object"/>; otherwise, false.
        /// </returns>
        /// <param name="obj">The <see cref="T:System.Object"/> to compare with the current <see cref="T:System.Object"/>. 
        /// </param><exception cref="T:System.NullReferenceException">The <paramref name="obj"/> parameter is null.
        /// </exception><filterpriority>2</filterpriority>
        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj))
            {
                return false;
            }

            if (ReferenceEquals(this, obj))
            {
                return true;
            }

            if (obj.GetType() != typeof(RedCelularActiveRecordBase<T>))
            {
                return false;
            }

            return Equals((RedCelularActiveRecordBase<T>)obj);
        }

        /// <summary>
        /// Serves as a hash function for a particular type. 
        /// </summary>
        /// <returns>
        /// A hash code for the current <see cref="T:System.Object"/>.
        /// </returns>
        /// <filterpriority>2</filterpriority>
        public override int GetHashCode()
        {
            unchecked
            {
                return (Id.GetHashCode() * 397) ^ Version;
            }
        }

        /// <summary>
        /// Gets the query.
        /// </summary>
        [ScriptIgnore]
        public System.Linq.IQueryable Query
        {
            get
            {
                return SessionScope.Current.AsQueryable<T>();
            }
        }
    }
}
