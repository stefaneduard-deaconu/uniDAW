using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace WikipediaUniDAW.Models {
    public class Version {
        /**
         * Table Fields
         */

        [Key]
        //[ForeignKey("CurrentArticle")]
        public int VersionId { get; set; }

        [ForeignKey("Article")]
        public int ArticleId { get; set; }

        [ForeignKey("ModifierUser")]
        public string ModifierUserId { get; set; }

        [ForeignKey("ChangedChapter")]
        public int? ChangedChapterId { get; set; }  // nullable

        public int VersionNo { get; set; }

        public DateTime? DateChange { get; set; }

        [Required(ErrorMessage = "Description is required!")]
        public string DescriptionChange { get; set; }

        /**
         * Navigation Properties
         */

        [InverseProperty("Versions")]
        public virtual Article Article { get; set; }

        public virtual ICollection<Article> CurrentArticle { get; set; }

        public virtual ApplicationUser ModifierUser { get; set; }

        [InverseProperty("AffectedVersion")]
        public virtual Chapter ChangedChapter { get; set; }

        public virtual ICollection<Chapter> Chapters { get; set; }
    }
}