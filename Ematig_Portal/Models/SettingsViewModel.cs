using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace Ematig_Portal.Models
{
    public class SettingsViewModel
    {
        [Required]
        [DataType(DataType.Text)]
        [Display(Name = "Chave")]
        public string Key { get; set; }

        [Required]
        [DataType(DataType.Text)]
        [Display(Name = "Valor")]
        public string Value { get; set; }

        [Required]
        [DataType(DataType.Text)]
        [Display(Name = "Descrição")]
        public string Name { get; set; }
    }
}