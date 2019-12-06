using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace WikipediaUniDAW.Models {
    public class Article {
        /**
         * Table Fields
         */

        [Key]
        public int ArticleId { get; set; }

        [ForeignKey("Category")]
        [Required(ErrorMessage = "Category is required!")]
        public int CategoryId { get; set; }
        
        [ForeignKey("CreatorUser")]
        public string CreatorUserId { get; set; }

        [ForeignKey("CurrentVersion")]
        public int CurrentVersionId { get; set; }

        [Required(ErrorMessage = "Title is required!")]
        [StringLength(30, ErrorMessage = "Title's length can't be greater than 30 characters!")]
        public string Title { get; set; }

        // the article's owner or a moderator can specify if an article needs protection
        // i.e. make unregistered users unable to edit the article
        [DefaultValue(false)]
        public bool Protected { get; set; } = false;

        // the admin can specify if an article needs to be frozen
        // i.e. make any user (excluding the admin) unable to edit or revert changes of the article
        [DefaultValue(false)]
        public bool Frozen { get; set; } = false;

        /**
         * Navigation Properties
         */

        public virtual Category Category { get; set; }

        public virtual ApplicationUser CreatorUser { get; set; }

        public virtual Version CurrentVersion { get; set; }

        public virtual ICollection<Version> Versions { get; set; }
    }
}