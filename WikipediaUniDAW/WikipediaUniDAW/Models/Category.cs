using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace WikipediaUniDAW.Models {
    public class Category {
        /**
         * Table Fields
         */

        [Key]
        public int CategoryId { get; set; }

        [Required(ErrorMessage = "Category name is required!")]
        public string Name { get; set; }

        /**
         * Navigation Properties
         */

        public virtual ICollection<Article> Articles { get; set; }
    }
}