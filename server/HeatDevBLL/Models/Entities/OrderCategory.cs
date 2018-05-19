using LinqToDB.Mapping;
using System;
using System.Collections.Generic;
using System.Text;

namespace HeatDevBLL.Models.Entities
{
    [Table(Schema = "public", Name = "order_categories")]
    public partial class OrderCategory
    {
        [Column(@"id"), PrimaryKey, Identity] public int Id { get; set; } // integer
        [Column(@"description"), NotNull] public string Description { get; set; } // character varying(25)

        #region Associations

        [Association(ThisKey = "Id", OtherKey = "CategoryId", CanBeNull = true, Relationship = Relationship.OneToMany, IsBackReference = true)]
        public IEnumerable<Order> Orders { get; set; }

        #endregion
    }
}
