using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace WikipediaUniDAW.Models {
    public class Chapter {
        /**
         * Table Fields
         */

        [Key]
        [ForeignKey("AffectedVersion")]
        public int ChapterId { get; set; }

        [ForeignKey("Version")]
        public int VersionId { get; set; }

        [Required(ErrorMessage = "Title is required!")]
        [StringLength(30, ErrorMessage = "Title's length can't be greater than 30 characters!")]
        public string Title { get; set; }

        [Required(ErrorMessage = "Content is required!")]
        public string Content { get; set; }

        /**
         * Navigation Properties
         */

        // the version which is created by adding / changing / removing this chapter
        public virtual Version AffectedVersion { get; set; }
        
        [InverseProperty("Chapters")]
        public virtual Version Version { get; set; }
    }
}