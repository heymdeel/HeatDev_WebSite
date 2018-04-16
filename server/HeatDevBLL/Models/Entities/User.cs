using LinqToDB.Mapping;
using System;
using System.Collections.Generic;
using System.Text;

namespace HeatDevBLL.Models.Entities
{
    [Table(Schema = "public", Name = "users")]
    public class User
    {
        [Column(@"id"), PrimaryKey, Identity] public int Id { get; set; } // integer
        [Column(@"login"), NotNull] public string Login { get; set; } // character varying(15)
        [Column(@"hash"), NotNull] public string Hash { get; set; } // text
        [Column(@"roles"), NotNull] public string[] Roles { get; set; } // ARRAY
        [Column(@"refresh_token"), Nullable] public string RefreshToken { get; set; } // text

        #region Associations

        [Association(ThisKey = "Id", OtherKey = "Id", CanBeNull = true, Relationship = Relationship.OneToOne, IsBackReference = true)]
        public UserProfiles Profile { get; set; }

        #endregion
    }
}
