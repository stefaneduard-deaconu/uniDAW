using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace WikipediaUniDAW.Models {
    public class Image {
        /**
         * Table Fields
         */

        [Key]
        [ForeignKey("AffectedVersion")]
        public int ImageId { get; set; }

        [ForeignKey("Version")]
        public int VersionId { get; set; }

        public string AbsolutePath { get; set; }

        public string Width { get; set; }

        public string Height { get; set; }

        /**
         * Navigation Properties
         */
        
        // the version which is created by adding / removing this image
        public virtual Version AffectedVersion { get; set; }

        [InverseProperty("Images")]
        public virtual Version Version { get; set; }
    }
}