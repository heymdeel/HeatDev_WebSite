using LinqToDB.Mapping;
using System;
using System.Collections.Generic;
using System.Text;

namespace HeatDevBLL.Models.Entities
{
    [Table(Schema = "public", Name = "reviews")]
    public class Review
    {
        [Column(@"id"), PrimaryKey, Identity] public int Id { get; set; } // integer
        [Column(@"rate"), NotNull] public int Rating { get; set; } // integer
        [Column(@"commentary"), NotNull] public string Text { get; set; } // text

        #region Associations

        [Association(ThisKey = "Id", OtherKey = "Id", CanBeNull = false, Relationship = Relationship.ManyToOne, KeyName = "reviews_order_id_fkey", BackReferenceName = "reviewsorderidfkeys")]
        public Order Order { get; set; }

        #endregion
    }
}
