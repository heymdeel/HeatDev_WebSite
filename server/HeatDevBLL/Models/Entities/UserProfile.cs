using LinqToDB.Mapping;
using System;
using System.Collections.Generic;
using System.Text;

namespace HeatDevBLL.Models.Entities
{
    [Table(Schema = "public", Name = "user_profiles")]
    public partial class UserProfile
    {
        [Column(@"id"), PrimaryKey, NotNull] public int Id { get; set; } // integer
        [Column(@"name"), NotNull] public string Name { get; set; } // character varying(15)
        [Column(@"surname"), NotNull] public string Surname { get; set; } // character varying(15)
        [Column(@"phone"), Nullable] public string Phone { get; set; } // character varying(11)
        [Column(@"avatar"), NotNull] public string Avatar { get; set; }

        #region Associations

        [Association(ThisKey = "Id", OtherKey = "ClientId", CanBeNull = true, Relationship = Relationship.OneToMany, IsBackReference = true)]
        public IEnumerable<Order> Orders { get; set; }

        [Association(ThisKey = "Id", OtherKey = "Id", CanBeNull = false, Relationship = Relationship.OneToOne, KeyName = "user_profiles_id_fkey", BackReferenceName = "userprofilesidfkey")]
        public User User { get; set; }

        #endregion
    }
}
