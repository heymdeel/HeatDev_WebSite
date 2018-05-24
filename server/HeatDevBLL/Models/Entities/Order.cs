using LinqToDB.Mapping;
using System;
using System.Collections.Generic;
using System.Text;

namespace HeatDevBLL.Models.Entities
{
    [Table(Schema = "public", Name = "orders")]
    public  class Order
    {
        [Column(@"id"), PrimaryKey, Identity] public int Id { get; set; } // integer
        [Column(@"client"), NotNull] public int ClientId { get; set; } // integer
        [Column(@"beginning_time"), NotNull] public DateTime BeginningTime { get; set; } // timestamp (6) without time zone
        [Column(@"end_time"), NotNull] public DateTime EndTime { get; set; } // timestamp (6) without time zone
        [Column(@"category"), NotNull] public int CategoryId { get; set; } // integer
        [Column(@"status"), NotNull] public int StatusId { get; set; } // integer
        [Column(@"price"), Nullable] public double Price { get; set; } // double precision
        [Column(@"diagnostic_price"), NotNull] public double DiagnosticPrice { get; set; } // double precision
        [Column(@"address"), NotNull] public string Address { get; set; }
        [Column(@"visit_time"), NotNull] public DateTime VisitTime { get; set; }

        #region Associations

        [Association(ThisKey = "CategoryId", OtherKey = "Id", CanBeNull = false, Relationship = Relationship.ManyToOne, KeyName = "orders_category_id_fkey", BackReferenceName = "orderscategoryidfkeys")]
        public OrderCategory Category { get; set; }

        [Association(ThisKey = "ClientId", OtherKey = "Id", CanBeNull = false, Relationship = Relationship.ManyToOne, KeyName = "orders_client_id_fkey", BackReferenceName = "ordersclientidfkeys")]
        public UserProfile ClientProfile { get; set; }

        [Association(ThisKey = "StatusId", OtherKey = "Id", CanBeNull = false, Relationship = Relationship.ManyToOne, KeyName = "orders_status_fkey", BackReferenceName = "ordersstatusfkeys")]
        public OrderStatus Status { get; set; }

        #endregion
    }
}
