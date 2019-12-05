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
        public int VersionId { get; set; }

        [ForeignKey("Article")]
        public int ArticleId { get; set; }

        [ForeignKey("ModifierUser")]
        public string ModifierUserId { get; set; }

        // nullable
        [ForeignKey("ChangedChapter")]
        public int? ChangedChapterId { get; set; }

        // nullable
        [ForeignKey("ChangedImage")]
        public int? ChangedImageId { get; set; }

        public int VersionNo { get; set; }

        public DateTime DateChange { get; set; }

        public string DescriptionChange { get; set; }

        /**
         * Navigation Properties
         */

        public virtual Article Article { get; set; }

        public virtual ApplicationUser ModifierUser { get; set; }

        [InverseProperty("AffectedVersion")]
        public virtual Chapter ChangedChapter { get; set; }

        [InverseProperty("AffectedVersion")]
        public virtual Image ChangedImage { get; set; }

        public virtual ICollection<Chapter> Chapters { get; set; }

        public virtual ICollection<Image> Images { get; set; }
    }
}